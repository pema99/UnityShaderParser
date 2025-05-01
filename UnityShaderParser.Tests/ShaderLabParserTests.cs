using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL.PreProcessor;

namespace UnityShaderParser.ShaderLab.Tests
{
    public class PositiveTests
    {
        public static string[] GetBuiltinUnityShaders()
        {
            return Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.shader", SearchOption.AllDirectories)
                .Select(path => Path.GetRelativePath(Directory.GetCurrentDirectory(), path))
                .Where(path => !File.ReadAllText(path).Contains("GLSLPROGRAM")) // Filter out GLSL
                .ToArray();
        }

        [Test, TestCaseSource(nameof(GetBuiltinUnityShaders))]
        public void ParseUnityShader(string path)
        {
            string source = File.ReadAllText(path);

            var tokens = ShaderLabLexer.Lex(source, null, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, lexerDiags.FirstOrDefault().ToString());

            var config = new ShaderLabParserConfig
            {
                ParseEmbeddedHLSL = false,
                ThrowExceptionOnError = false,
                DiagnosticFilter = DiagnosticFlags.OnlyErrors,
            };
            var parsed = ShaderLabParser.Parse(tokens, config, out var parserDiags);
            Assert.IsEmpty(parserDiags, parserDiags.FirstOrDefault().ToString());
        }

        [Test, TestCaseSource(nameof(GetBuiltinUnityShaders))]
        public void ParseUnityShaderAndEmbeddedHLSL(string path)
        {
            string source = File.ReadAllText(path);

            var tokens = ShaderLabLexer.Lex(source, null, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, lexerDiags.FirstOrDefault().ToString());

            string cgIncludesPath = Path.Combine(Directory.GetCurrentDirectory(), "TestShaders/UnityBuiltinShaders/CGIncludes");
            var config = new ShaderLabParserConfig
            {
                ParseEmbeddedHLSL = true,
                ThrowExceptionOnError = false,
                DiagnosticFilter = DiagnosticFlags.OnlyErrors,
                IncludeResolver = new DefaultPreProcessorIncludeResolver(new List<string> { cgIncludesPath }),
                BasePath = Directory.GetParent(path)?.FullName,
                Defines = new Dictionary<string, string>()
                {
                    { "SHADER_API_D3D11", "1" }
                },
            };
            var parsed = ShaderLabParser.Parse(tokens, config, out var parserDiags);
            Assert.IsEmpty(parserDiags, parserDiags.FirstOrDefault().ToString());
        }

        [Test, TestCaseSource(nameof(GetBuiltinUnityShaders))]
        public void RoundTripTestShadersWithPrettyPrintedHLSL(string path)
        {
            RoundTripTestShaders(path, true);
        }

        [Test, TestCaseSource(nameof(GetBuiltinUnityShaders))]
        public void RoundTripTestShadersWithOriginalHLSL(string path)
        {
            RoundTripTestShaders(path, false);
        }

        public void RoundTripTestShaders(string path, bool prettyPrintEmbeddedHLSL)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            var tokens = ShaderLabLexer.Lex(source, null, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Parse
            string cgIncludesPath = Path.Combine(Directory.GetCurrentDirectory(), "TestShaders/UnityBuiltinShaders/CGIncludes");
            var config = new ShaderLabParserConfig
            {
                ParseEmbeddedHLSL = prettyPrintEmbeddedHLSL,
                ThrowExceptionOnError = false,
                IncludeResolver = new DefaultPreProcessorIncludeResolver(new List<string> { cgIncludesPath }),
                BasePath = Directory.GetParent(path)?.FullName,
                DiagnosticFilter = DiagnosticFlags.OnlyErrors,
                Defines = new Dictionary<string, string>()
                {
                    { "SHADER_API_D3D11", "1" }
                },
            };
            var shader = ShaderLabParser.Parse(tokens, config, out var parserDiags);
            Assert.IsEmpty(parserDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");

            // Pretty print
            var printer = new ShaderLabPrinter();
            printer.PrettyPrintEmbeddedHLSL = prettyPrintEmbeddedHLSL;
            printer.Visit(shader);
            string prettyPrinted = printer.Text;

            // Re-lex
            tokens = ShaderLabLexer.Lex(prettyPrinted, null, null, false, out var relexerDiags);
            Assert.IsEmpty(relexerDiags, $"Expected no lexer errors, got: {relexerDiags.FirstOrDefault()}");

            // Re-parse
            var reshader = ShaderLabParser.Parse(tokens, config, out var reparserDiags);
            Assert.IsEmpty(reparserDiags, $"Expected no parser errors, got: {reparserDiags.FirstOrDefault()}");

            // Re-pretty print
            printer = new ShaderLabPrinter();
            printer.PrettyPrintEmbeddedHLSL = prettyPrintEmbeddedHLSL;
            printer.Visit(reshader);
            string roundtripped = printer.Text;

            // Compare
            Assert.AreEqual(prettyPrinted, roundtripped);
        }
    }

    public class NegativeTests
    {
        [Test]
        public void SimpleErrorRecoversAndEmitsAllDiagnostics()
        {
            var decl = ShaderParser.ParseUnityShader(@"
                Shader ""Foo""
                {
                    SubShader
                    {
                        Blend deez nuts lmao
                        ZWrite On
                        ZWrite what even is that
                        ZTest Always
                    }
                }
            ", out var diags);

            Assert.AreEqual(2, diags.Count);

            var pass = decl.SubShaders[0];
            Assert.IsTrue(pass!.Commands[1] is ShaderLabCommandZWriteNode zw && zw.Enabled.Value);
            Assert.IsTrue(pass!.Commands[3] is ShaderLabCommandZTestNode zt && zt.Mode.Value == ComparisonMode.Always);
        }

        [Test]
        public void MalformedPropertyDoesntThrow()
        {
            // This tests a malformed property (makes null children)
            var decl = ShaderParser.ParseUnityShader(@"
                Shader ""Unlit/NewUnlitShader""
                {
                    Properties
                    {
                        d _MainTex(""Texture"", 2D) = ""white"" { }
                    }
                    SubShader
                    {
                        LOD 100
                    }
                }
            ", out var diags);
            Assert.AreEqual(1, diags.Count);
        }

        [Test]
        public void MalformedCommandDoesntThrow()
        {
            var decl = ShaderParser.ParseUnityShader(@"
                Shader ""Unlit/NewUnlitShader""
                {
                    Properties
                    {
                        _MainTex(""Texture"", 2D) = ""white"" { }
                    }
                    SubShader
                    {
                        LOsD 100
                    }
                }
            ", out var diags);
            Assert.AreEqual(1, diags.Count);
        }
    }
}
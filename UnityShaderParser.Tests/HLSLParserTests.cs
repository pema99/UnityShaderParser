using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL.PreProcessor;

namespace UnityShaderParser.HLSL.Tests
{
    public class PositiveTests
    {
        public static string[] GetTestShaders()
        {
            string[] extensions = { "*.hlsl", "*.fx", "*.fxh", "*.h" };
            return extensions
                .SelectMany(ext => Directory.EnumerateFiles(Directory.GetCurrentDirectory(), ext, SearchOption.AllDirectories))
                .Select(path => Path.GetRelativePath(Directory.GetCurrentDirectory(), path))
                .ToArray();
        }

        [Test, TestCaseSource(nameof(GetTestShaders))]
        public void LexTestShaders(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            var tokens = HLSLLexer.Lex(source, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");
        }

        public static string[] GetTestShadersNotContainingMacros()
        {
            return GetTestShaders().Where(path => !File.ReadAllText(path).Contains("#")).ToArray();
        }

        [Test, TestCaseSource(nameof(GetTestShadersNotContainingMacros))]
        public void ParseTestShadersNotContainingMacros(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            var tokens = HLSLLexer.Lex(source, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Parse
            var config = new HLSLParserConfig
            {
                PreProcessorMode = PreProcessorMode.DoNothing,
                ThrowExceptionOnError = false,
            };
            HLSLParser.ParseTopLevelDeclarations(tokens, config, out var parserDiags, out _);
            Assert.IsEmpty(parserDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");
        }

        public static string[] GetTestShadersContainingMacros()
        {
            return GetTestShaders().Where(path => File.ReadAllText(path).Contains("#")).ToArray();
        }

        [Test, TestCaseSource(nameof(GetTestShadersContainingMacros))]
        public void ParseTestShadersContainingMacros(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            var tokens = HLSLLexer.Lex(source, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Expand and parse
            var config = new HLSLParserConfig
            {
                PreProcessorMode = PreProcessorMode.ExpandAll,
                ThrowExceptionOnError = false,
                BasePath = Directory.GetParent(path)?.FullName,
                IncludeResolver = new DefaultPreProcessorIncludeResolver(new List<string>()
                {
                    "TestShaders/Nvidia"
                }),
            };
            var nodes = HLSLParser.ParseTopLevelDeclarations(tokens, config, out var parserDiags, out _);
            Assert.IsEmpty(parserDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");
        }

        [Test, TestCaseSource(nameof(GetTestShadersContainingMacros))]
        public void PreProcessShadersContainingMacros(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            var tokens = HLSLLexer.Lex(source, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            var resolver = new DefaultPreProcessorIncludeResolver(new List<string>()
            {
                "TestShaders/Nvidia"
            });

            // Expand with different strategies
            HLSLPreProcessor.PreProcess(tokens, false, DiagnosticFlags.All, PreProcessorMode.ExpandAllExceptIncludes, Directory.GetParent(path)?.FullName!, resolver, new Dictionary<string, string>(), out var pragmas, out var preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            HLSLPreProcessor.PreProcess(tokens, false, DiagnosticFlags.All, PreProcessorMode.ExpandIncludesOnly, Directory.GetParent(path)?.FullName!, resolver, new Dictionary<string, string>(), out pragmas, out preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            HLSLPreProcessor.PreProcess(tokens, false, DiagnosticFlags.All, PreProcessorMode.StripDirectives, Directory.GetParent(path)?.FullName!, resolver, new Dictionary<string, string>(), out pragmas, out preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");
        }

        private class HLSLSyntaxContractResolver : DefaultContractResolver
        {
            private readonly static HashSet<string> Exclusions = new HashSet<string>
            {
                "GetChildren",
                "Children",
                "Span",
                "OriginalSpan",
                "Tokens",
            };

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                if (Exclusions.Contains(property.PropertyName ?? string.Empty))
                {
                    property.ShouldSerialize =
                        o => false;
                }

                return property;
            }
        }

        [Test, TestCaseSource(nameof(GetTestShaders))]
        public void RoundTripTestShaders(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            var tokens = HLSLLexer.Lex(source, null, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Parse
            var config = new HLSLParserConfig
            {
                PreProcessorMode = PreProcessorMode.ExpandAll,
                ThrowExceptionOnError = false,
                BasePath = Directory.GetParent(path)?.FullName,
                IncludeResolver = new DefaultPreProcessorIncludeResolver(new List<string>()
                {
                    "TestShaders/Nvidia"
                }),
            };
            var decls = HLSLParser.ParseTopLevelDeclarations(tokens, config, out var parserDiags, out _);
            Assert.IsEmpty(parserDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");

            // Pretty print
            var printer = new HLSLPrinter();
            printer.VisitMany(decls);
            string prettyPrinted = printer.Text;

            // Re-lex
            tokens = HLSLLexer.Lex(prettyPrinted, null, false, out var relexerDiags);
            Assert.IsEmpty(relexerDiags, $"Expected no lexer errors, got: {relexerDiags.FirstOrDefault()}");

            // Re-parse
            var redecls = HLSLParser.ParseTopLevelDeclarations(tokens, config, out var reparserDiags, out _);
            Assert.IsEmpty(reparserDiags, $"Expected no parser errors, got: {reparserDiags.FirstOrDefault()}");

            // Re-pretty print
            printer = new HLSLPrinter();
            printer.VisitMany(redecls);
            string roundtripped = printer.Text;

            // Compare
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new HLSLSyntaxContractResolver(),
            };
            Assert.That(JsonConvert.SerializeObject(redecls, settings), Is.EqualTo(JsonConvert.SerializeObject(decls, settings)).NoClip);
            
            Assert.AreEqual(prettyPrinted, roundtripped);
        }
    }

    public class NegativeTests
    {
        [Test]
        public void SimpleErrorRecoversAndEmitsAllDiagnostics()
        {
            var decl = ShaderParser.ParseTopLevelDeclaration(@"
                void foo()
                {
                    int a a a a a a;
                    int bar = 2;
                    int b b b b b b;
                }
            ", out var diags, out var prags) as FunctionDefinitionNode;

            Assert.AreEqual(2, diags.Count);
            Assert.AreEqual(3, decl!.Body.Children.Count);
            Assert.IsTrue(decl.Body.Children[1] is VariableDeclarationStatementNode node &&
                node.Declarators[0].Name == "bar" &&
                node.Declarators[0].Initializer is ValueInitializerNode init &&
                init.Expression is LiteralExpressionNode);
        }
    }
}
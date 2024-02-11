using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityShaderParser.PreProcessor;

namespace UnityShaderParser.ShaderLab.Tests
{
    public class Tests
    {
        public static string[] GetBuiltinUnityShaders()
        {
            return Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.shader", SearchOption.AllDirectories)
                .Select(path => Path.GetRelativePath(Directory.GetCurrentDirectory(), path))
                .ToArray();
        }

        [Test, TestCaseSource(nameof(GetBuiltinUnityShaders))]
        public void ParseUnityShader(string path)
        {
            string source = File.ReadAllText(path);

            var tokens = ShaderLabLexer.Lex(source, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, lexerDiags.FirstOrDefault().ToString());

            var config = new ShaderLabParserConfig
            {
                ParseEmbeddedHLSL = false,
                ThrowExceptionOnError = false,
            };
            var parsed = ShaderLabParser.Parse(tokens, config, out var parserDiags);
            Assert.IsEmpty(parserDiags, parserDiags.FirstOrDefault().ToString());
        }

        [Test, TestCaseSource(nameof(GetBuiltinUnityShaders))]
        public void ParseUnityShaderAndEmbeddedHLSL(string path)
        {
            string source = File.ReadAllText(path);
            if (source.Contains("GLSLPROGRAM"))
                Assert.Ignore("GLSL parsing not supported");

            var tokens = ShaderLabLexer.Lex(source, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, lexerDiags.FirstOrDefault().ToString());

            string cgIncludesPath = Path.Combine(Directory.GetCurrentDirectory(), "TestShaders/UnityBuiltinShaders/CGIncludes");
            var config = new ShaderLabParserConfig
            {
                ParseEmbeddedHLSL = true,
                ThrowExceptionOnError = false,
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
    }
}
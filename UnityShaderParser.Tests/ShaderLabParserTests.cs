using NUnit.Framework;
using System.IO;
using System.Linq;

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

            ShaderLabLexer.Lex(source, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            ShaderLabParser.Parse(tokens, out ShaderNode shader, out var parserDiags);
            Assert.IsEmpty(parserDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");
        }
    }
}
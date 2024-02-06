using NUnit.Framework;
using System.IO;
using System.Linq;

namespace UnityShaderParser.HLSL.Tests
{
    public class Tests
    {
        public static string[] GetTestShaders()
        {
            string[] extensions = { "*.hlsl", "*.fx", "*.vert", "*.frag", "*.fxh" };
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
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");
        }

        public static string[] GetTestShadersNotContainingMacros()
        {
            string[] extensions = { "*.hlsl", "*.fx", "*.vert", "*.frag", "*.fxh" };
            return extensions
                .SelectMany(ext => Directory.EnumerateFiles(Directory.GetCurrentDirectory(), ext, SearchOption.AllDirectories))
                .Select(path => Path.GetRelativePath(Directory.GetCurrentDirectory(), path))
                .Where(path => !File.ReadAllText(path).Contains("#"))
                .ToArray();
        }

        [Test, TestCaseSource(nameof(GetTestShadersNotContainingMacros))]
        public void ParseTestShadersNotContainingMacros(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Lex
            HLSLParser.ParseTopLevelDeclarations(tokens, out var nodes, out var parserDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");
        }


    }
}
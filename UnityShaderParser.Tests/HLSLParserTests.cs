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
        public void LexTestShadersWithoutMacros(string path)
        {
            // Read text
            string[] source = File.ReadAllLines(path);

            // De-macro-ify
            source = source.Where(line => !line.Trim().StartsWith("#") && !line.Trim().EndsWith('\\')).ToArray();
            string concatted = string.Join('\n', source);

            // Lex
            HLSLLexer.Lex(concatted, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");
        }
    }
}
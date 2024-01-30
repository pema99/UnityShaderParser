using NUnit.Framework;
using System.IO;
using System.Linq;

namespace UnityShaderParser.HLSL.Tests
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
        public void LexUnityShaderWithoutMacros(string path)
        {
            // Read text
            string[] source = File.ReadAllLines(path);

            // De-macro-ify
            source = source.Where(line => !line.Trim().StartsWith("#")).ToArray();
            string concatted = string.Join('\n', source);

            // Lex
            HLSLLexer.Lex(concatted, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");
        }
    }
}
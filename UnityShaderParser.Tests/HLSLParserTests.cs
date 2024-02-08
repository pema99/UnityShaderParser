using NUnit.Framework;
using System.IO;
using System.Linq;
using UnityShaderParser.PreProcessor;

namespace UnityShaderParser.HLSL.Tests
{
    public class Tests
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
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
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
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Lex
            HLSLParser.ParseTopLevelDeclarations(tokens, out var nodes, out var parserDiags);
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
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Expand
            HLSLPreProcessor.PreProcess(tokens, PreProcessorMode.ExpandAll, Directory.GetParent(path)?.FullName!, out var preProcessedTokens, out var pragmas, out var preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            // Lex
            HLSLParser.ParseTopLevelDeclarations(preProcessedTokens, out var nodes, out var parserDiags);
            Assert.IsEmpty(parserDiags, $"Expected no parser errors, got: {parserDiags.FirstOrDefault()}");
        }

        [Test, TestCaseSource(nameof(GetTestShadersContainingMacros))]
        public void PreProcessShadersContainingMacros(string path)
        {
            // Read text
            string source = File.ReadAllText(path);

            // Lex
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Expand with different strategies
            HLSLPreProcessor.PreProcess(tokens, PreProcessorMode.ExpandAllExceptIncludes, Directory.GetParent(path)?.FullName!, out var preProcessedTokens, out var pragmas, out var preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            HLSLPreProcessor.PreProcess(tokens, PreProcessorMode.ExpandIncludesOnly, Directory.GetParent(path)?.FullName!, out preProcessedTokens, out pragmas, out preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            HLSLPreProcessor.PreProcess(tokens, PreProcessorMode.StripDirectives, Directory.GetParent(path)?.FullName!, out preProcessedTokens, out pragmas, out preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");
        }
    }
}
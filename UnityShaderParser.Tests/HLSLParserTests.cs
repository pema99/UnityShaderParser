﻿using NUnit.Framework;
using System.IO;
using System.Linq;
using UnityShaderParser.Common;
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
            var tokens = HLSLLexer.Lex(source, false, out var lexerDiags);
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
            var tokens = HLSLLexer.Lex(source, false, out var lexerDiags);
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
            var tokens = HLSLLexer.Lex(source, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Expand and parse
            var config = new HLSLParserConfig
            {
                PreProcessorMode = PreProcessorMode.ExpandAll,
                ThrowExceptionOnError = false,
                BasePath = Directory.GetParent(path)?.FullName,
                IncludeResolver = new DefaultPreProcessorIncludeResolver(),
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
            var tokens = HLSLLexer.Lex(source, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            // Expand with different strategies
            HLSLPreProcessor.PreProcess(tokens, false, PreProcessorMode.ExpandAllExceptIncludes, Directory.GetParent(path)?.FullName!, new DefaultPreProcessorIncludeResolver(), out var pragmas, out var preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            HLSLPreProcessor.PreProcess(tokens, false, PreProcessorMode.ExpandIncludesOnly, Directory.GetParent(path)?.FullName!, new DefaultPreProcessorIncludeResolver(), out pragmas, out preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");

            HLSLPreProcessor.PreProcess(tokens, false, PreProcessorMode.StripDirectives, Directory.GetParent(path)?.FullName!, new DefaultPreProcessorIncludeResolver(), out pragmas, out preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");
        }

        [Test]
        public void IdentifierConcatenationInMacro()
        {
            string source = @"
                #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
                TRANSFORM_TEX(v.texcoord, _MainTex);
            ";

            // Lex
            var tokens = HLSLLexer.Lex(source, false, out var lexerDiags);
            Assert.IsEmpty(lexerDiags, $"Expected no lexer errors, got: {lexerDiags.FirstOrDefault()}");

            tokens = HLSLPreProcessor.PreProcess(tokens, false, PreProcessorMode.ExpandAll, "", new DefaultPreProcessorIncludeResolver(), out var pragmas, out var preProcessorDiags);
            Assert.IsEmpty(preProcessorDiags, $"Expected no preprocessing errors, got: {preProcessorDiags.FirstOrDefault()}");
        }
    }
}
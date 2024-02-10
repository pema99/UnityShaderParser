using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.HLSL;
using UnityShaderParser.ShaderLab;

namespace UnityShaderParser.Common
{
    public static class ShaderParser
    {
        private static HLSLParserConfig DefaultHLSLConfig = new HLSLParserConfig();
        private static ShaderLabParserConfig DefaultShaderLabConfig = new ShaderLabParserConfig();

        public static ShaderNode ParseUnityShader(string source, ShaderLabParserConfig config, out List<Diagnostic> diagnostics)
        {
            var tokens = ShaderLabLexer.Lex(source, config.ThrowExceptionOnError, out var lexerDiags);
            var rootNode = ShaderLabParser.Parse(tokens, config, out var parserDiags);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return rootNode;
        }
        public static ShaderNode ParseUnityShader(string source, ShaderLabParserConfig config) => ParseUnityShader(source, config, out _);
        public static ShaderNode ParseUnityShader(string source, out List<Diagnostic> diagnostics) => ParseUnityShader(source, DefaultShaderLabConfig, out diagnostics);
        public static ShaderNode ParseUnityShader(string source) => ParseUnityShader(source, DefaultShaderLabConfig, out _);

        public static List<HLSLSyntaxNode> ParseTopLevelDeclarations(string source, HLSLParserConfig config, out List<Diagnostic> diagnostics, out List<string> pragmas)
        {
            var tokens = HLSLLexer.Lex(source, config.ThrowExceptionOnError, out var lexerDiags);
            var decls = HLSLParser.ParseTopLevelDeclarations(tokens, config, out var parserDiags, out pragmas);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return decls;
        }
        public static List<HLSLSyntaxNode> ParseTopLevelDeclarations(string source, HLSLParserConfig config) => ParseTopLevelDeclarations(source, config, out _, out _);
        public static List<HLSLSyntaxNode> ParseTopLevelDeclarations(string source, out List<Diagnostic> diagnostics, out List<string> pragmas) => ParseTopLevelDeclarations(source, DefaultHLSLConfig, out diagnostics, out pragmas);
        public static List<HLSLSyntaxNode> ParseTopLevelDeclarations(string source) => ParseTopLevelDeclarations(source, DefaultHLSLConfig, out _, out _);

        public static HLSLSyntaxNode ParseTopLevelDeclaration(string source, HLSLParserConfig config, out List<Diagnostic> diagnostics, out List<string> pragmas)
        {
            var tokens = HLSLLexer.Lex(source, config.ThrowExceptionOnError, out var lexerDiags);
            var decl = HLSLParser.ParseTopLevelDeclaration(tokens, config, out var parserDiags, out pragmas);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return decl;
        }
        public static HLSLSyntaxNode ParseTopLevelDeclaration(string source, HLSLParserConfig config) => ParseTopLevelDeclaration(source, config, out _, out _);
        public static HLSLSyntaxNode ParseTopLevelDeclaration(string source, out List<Diagnostic> diagnostics, out List<string> pragmas) => ParseTopLevelDeclaration(source, DefaultHLSLConfig, out diagnostics, out pragmas);
        public static HLSLSyntaxNode ParseTopLevelDeclaration(string source) => ParseTopLevelDeclaration(source, DefaultHLSLConfig, out _, out _);

        public static StatementNode ParseStatement(string source, HLSLParserConfig config, out List<Diagnostic> diagnostics, out List<string> pragmas)
        {
            var tokens = HLSLLexer.Lex(source, config.ThrowExceptionOnError, out var lexerDiags);
            var stmt = HLSLParser.ParseStatement(tokens, config, out var parserDiags, out pragmas);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return stmt;
        }
        public static StatementNode ParseStatement(string source, HLSLParserConfig config) => ParseStatement(source, config, out _, out _);
        public static StatementNode ParseStatement(string source, out List<Diagnostic> diagnostics, out List<string> pragmas) => ParseStatement(source, DefaultHLSLConfig, out diagnostics, out pragmas);
        public static StatementNode ParseStatement(string source) => ParseStatement(source, DefaultHLSLConfig, out _, out _);

        public static ExpressionNode ParseExpression(string source, HLSLParserConfig config, out List<Diagnostic> diagnostics, out List<string> pragmas)
        {
            var tokens = HLSLLexer.Lex(source, config.ThrowExceptionOnError, out var lexerDiags);
            var expr = HLSLParser.ParseExpression(tokens, config, out var parserDiags, out pragmas);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return expr;
        }
        public static ExpressionNode ParseExpression(string source, HLSLParserConfig config) => ParseExpression(source, config, out _, out _);
        public static ExpressionNode ParseExpression(string source, out List<Diagnostic> diagnostics, out List<string> pragmas) => ParseExpression(source, DefaultHLSLConfig, out diagnostics, out pragmas);
        public static ExpressionNode ParseExpression(string source) => ParseExpression(source, DefaultHLSLConfig, out _, out _);
    }
}

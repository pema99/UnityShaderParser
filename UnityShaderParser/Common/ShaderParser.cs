using UnityShaderParser.HLSL;
using UnityShaderParser.ShaderLab;

namespace UnityShaderParser.Common
{
    public static class ShaderParser
    {
        public static ShaderNode ParseUnityShader(string source, out List<string> diagnostics)
        {
            ShaderLabLexer.Lex(source, out var tokens, out var lexerDiags);
            ShaderLabParser.Parse(tokens, out var rootNode, out var parserDiags);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return rootNode;
        }

        public static ShaderNode ParseUnityShader(string source)
            => ParseUnityShader(source, out _);

        public static List<HLSLSyntaxNode> ParseTopLevelDeclarations(string source, out List<string> diagnostics)
        {
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            HLSLParser.ParseTopLevelDeclarations(tokens, out var decls, out var parserDiags);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return decls;
        }

        public static List<HLSLSyntaxNode> ParseTopLevelDeclarations(string source)
            => ParseTopLevelDeclarations(source, out _);

        public static HLSLSyntaxNode ParseTopLevelDeclaration(string source, out List<string> diagnostics)
        {
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            HLSLParser.ParseTopLevelDeclaration(tokens, out var decl, out var parserDiags);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return decl;
        }

        public static HLSLSyntaxNode ParseTopLevelDeclaration(string source)
            => ParseTopLevelDeclaration(source, out _);

        public static StatementNode ParseStatement(string source, out List<string> diagnostics)
        {
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            HLSLParser.ParseStatement(tokens, out var stmt, out var parserDiags);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return stmt;
        }

        public static HLSLSyntaxNode ParseStatement(string source)
            => ParseStatement(source, out _);

        public static ExpressionNode ParseExpression(string source, out List<string> diagnostics)
        {
            HLSLLexer.Lex(source, out var tokens, out var lexerDiags);
            HLSLParser.ParseExpression(tokens, out var expr, out var parserDiags);
            diagnostics = lexerDiags.Concat(parserDiags).ToList();
            return expr;
        }

        public static ExpressionNode ParseExpression(string source)
            => ParseExpression(source, out _);
    }
}

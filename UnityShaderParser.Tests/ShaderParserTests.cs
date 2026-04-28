using NUnit.Framework;
using System.Collections.Generic;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using SLTokenKind = UnityShaderParser.ShaderLab.TokenKind;
using SLLexer = UnityShaderParser.ShaderLab.ShaderLabLexer;

namespace UnityShaderParser.Tests
{
    public class PositiveTests
    {
        public void CheckParents<T>(SyntaxNode<T> parent)
            where T : SyntaxNode<T>
        {
            foreach (var child in parent.Children)
            {
                Assert.IsNotNull(child);
                Assert.AreEqual(parent, child.Parent);
                CheckParents(child);
            }
        }

        [Test]
        public void ShaderParserFunctionComputesAllParents()
        {
            List<Diagnostic> diags = new List<Diagnostic>();
            List<string> pragmas = new List<string>();

            var shader = ShaderParser.ParseUnityShader("Shader \"foobar\" { SubShader { Pass { CGINCLUDE\n void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } void bar() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } \nENDCG\n } } }", out diags);
            Assert.Greater(shader.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(shader);

            var decls = ShaderParser.ParseTopLevelDeclarations("void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } void bar() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } ", out diags, out pragmas);
            Assert.Greater(decls.Count, 0);
            Assert.IsEmpty(diags);
            foreach (var c in decls)
            {
                Assert.Greater(c.Children.Count, 0);
                CheckParents(c);
            }

            var decl = ShaderParser.ParseTopLevelDeclaration("void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; }", out diags, out pragmas);
            Assert.Greater(decl.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(decl);

            var stmt = ShaderParser.ParseStatement("int a = 1 + (3 * 4 + { 1, 2, 3 }) * a;", out diags, out pragmas);
            Assert.Greater(stmt.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(stmt);

            var expr = ShaderParser.ParseExpression("1 + (3 * 4 + { 1, 2, 3 }) * a", out diags, out pragmas);
            Assert.Greater(expr.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(expr);

            var subShader = ShaderParser.ParseUnitySubShader("SubShader { Pass { CGINCLUDE\n void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } void bar() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } \nENDCG\n } }", out diags);
            Assert.Greater(subShader.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(subShader);

            var shaderPass = ShaderParser.ParseUnityShaderPass("Pass { ZTest Always CGINCLUDE\n void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } void bar() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } \nENDCG\n }", out diags);
            Assert.Greater(shaderPass.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(shaderPass);

            var shaderProperty = ShaderParser.ParseUnityShaderProperty("_Foo(\"Bar\", Float) = 3.0", out diags);
            Assert.Greater(shaderProperty.Children.Count, 0);
            Assert.IsEmpty(diags);
            CheckParents(shaderProperty);

            var shaderProperties = ShaderParser.ParseUnityShaderProperties("_Foo(\"Bar\", Float) = 3.0\n_Baz(\"Bom\", Float) = 5.0", out diags);
            Assert.AreEqual(2, shaderProperties.Count);
            Assert.IsEmpty(diags);
            foreach (var prop in shaderProperties)
            {
                Assert.Greater(prop.Children.Count, 0);
                CheckParents(prop);
            }

            var shaderPropertyBlock = ShaderParser.ParseUnityShaderPropertyBlock("Properties { _Foo(\"Bar\", Float) = 3.0\n_Baz(\"Bom\", Float) = 5.0 }", out diags);
            Assert.AreEqual(2, shaderPropertyBlock.Count);
            Assert.IsEmpty(diags);
            foreach (var prop in shaderPropertyBlock)
            {
                Assert.Greater(prop.Children.Count, 0);
                CheckParents(prop);
            }

            var shaderLabCommand = ShaderParser.ParseUnityShaderCommand("ZWrite On", out diags);
            Assert.AreEqual(shaderLabCommand.Children.Count, 0); // This is a terminal
            Assert.IsEmpty(diags);
            CheckParents(shaderLabCommand);

            var shaderLabCommands = ShaderParser.ParseUnityShaderCommands("ZWrite On ZTest Off", out diags);
            Assert.AreEqual(2, shaderLabCommands.Count);
            Assert.IsEmpty(diags);
            foreach (var commands in shaderLabCommands)
            {
                Assert.AreEqual(commands.Children.Count, 0); // This is a terminal
                CheckParents(commands);
            }
        }

        [Test]
        public void ParseGenericVectorAndMatrixTypes()
        {
            List<Diagnostic> diags;

            var decl = ShaderParser.ParseTopLevelDeclaration("matrix<float, 2+2, 1+1> a;", out diags, out _);
            Assert.AreEqual(decl.Children[0].Children.Count, 2);
            Assert.IsEmpty(diags);
            Assert.IsTrue(decl.Children[0].Children[0] is ExpressionNode);
            Assert.IsTrue(decl.Children[0].Children[1] is ExpressionNode);

            decl = ShaderParser.ParseTopLevelDeclaration("vector<float, 2+2> a;", out diags, out _);
            Assert.AreEqual(decl.Children[0].Children.Count, 1);
            Assert.IsEmpty(diags);
            Assert.IsTrue(decl.Children[0].Children[0] is ExpressionNode);
        }
    }

    public class CommentTriviaTests
    {
        private static Token<TokenKind> FindToken(IEnumerable<Token<TokenKind>> tokens, TokenKind kind, string? identifier = null)
        {
            foreach (var token in tokens)
            {
                if (token.Kind != kind) continue;
                if (identifier != null && token.Identifier != identifier) continue;
                return token;
            }
            Assert.Fail($"Did not find token of kind {kind} (identifier '{identifier}').");
            return null;
        }

        [Test]
        public void LeadingSingleLineCommentAttachesToFirstToken()
        {
            var tokens = HLSLLexer.Lex("// header\nint a;", null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.AreEqual(1, intTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, intTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("// header", intTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void InlineSingleLineCommentBecomesLeadingTriviaOfNextToken()
        {
            var tokens = HLSLLexer.Lex("int a; // trailing\nint b;", null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var secondInt = tokens[3];
            Assert.AreEqual(TokenKind.IntKeyword, secondInt.Kind);
            Assert.AreEqual(1, secondInt.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, secondInt.LeadingTrivia[0].Kind);
            Assert.AreEqual("// trailing", secondInt.LeadingTrivia[0].Text);
        }

        [Test]
        public void InlineMultiLineCommentBecomesLeadingTriviaOfNextToken()
        {
            var tokens = HLSLLexer.Lex("int /* between */ a;", null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var aTok = FindToken(tokens, TokenKind.IdentifierToken, "a");
            Assert.AreEqual(1, aTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, aTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* between */", aTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void MultiLineCommentOnNewLineBecomesLeadingTriviaOfNextToken()
        {
            var tokens = HLSLLexer.Lex("int a;\n/* hello\nworld */\nint b;", null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var secondInt = tokens[3];
            Assert.AreEqual(TokenKind.IntKeyword, secondInt.Kind);
            Assert.AreEqual(1, secondInt.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, secondInt.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* hello\nworld */", secondInt.LeadingTrivia[0].Text);
        }

        [Test]
        public void CommentBetweenFunctionParametersIsLeadingTriviaOfNextParameter()
        {
            var decl = ShaderParser.ParseTopLevelDeclaration(
                "void foo(int a, /* the b */ int b) {}", out var diags, out _);
            Assert.IsEmpty(diags);

            int sawIntKeyword = 0;
            Token<TokenKind> secondInt = null;
            foreach (var t in decl.Tokens)
            {
                if (t.Kind == TokenKind.IntKeyword)
                {
                    sawIntKeyword++;
                    if (sawIntKeyword == 2) { secondInt = t; break; }
                }
            }
            Assert.IsNotNull(secondInt);
            Assert.AreEqual(1, secondInt.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, secondInt.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* the b */", secondInt.LeadingTrivia[0].Text);
        }

        [Test]
        public void MixedCommentStylesAreCapturedSeparately()
        {
            var tokens = HLSLLexer.Lex(
                "/* block */ // line\nint a; // tail",
                null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.AreEqual(2, intTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, intTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* block */", intTok.LeadingTrivia[0].Text);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, intTok.LeadingTrivia[1].Kind);
            Assert.AreEqual("// line", intTok.LeadingTrivia[1].Text);

            var eofTok = FindToken(tokens, TokenKind.EndOfFileToken);
            Assert.AreEqual(1, eofTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, eofTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("// tail", eofTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void TrailingCommentAtEndOfFileAttachesToEndOfFileToken()
        {
            var tokens = HLSLLexer.Lex("int a;\n// goodbye", null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var eofTok = tokens[tokens.Count - 1];
            Assert.AreEqual(TokenKind.EndOfFileToken, eofTok.Kind);
            Assert.AreEqual(1, eofTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, eofTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("// goodbye", eofTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void CommentsSurviveThroughTheParser()
        {
            var stmt = ShaderParser.ParseStatement(
                "int a = /* comment */ 42; // tail", out var diags, out _);
            Assert.IsEmpty(diags);

            var literalTok = FindToken(stmt.Tokens, TokenKind.IntegerLiteralToken);
            Assert.AreEqual(1, literalTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, literalTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* comment */", literalTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void MacroParameterSubstitutionPreservesArgumentTrivia()
        {
            var tokens = ShaderParser.PreProcessToTokens(
                "#define id(x) x;\nid(foo /*c*/)", out var diags, out _);
            Assert.IsEmpty(diags);

            Assert.AreEqual(TokenKind.IdentifierToken, tokens[0].Kind);
            Assert.AreEqual("foo", tokens[0].Identifier);
            Assert.AreEqual(TokenKind.SemiToken, tokens[1].Kind);
            Assert.AreEqual(1, tokens[1].LeadingTrivia.Count);
            Assert.AreEqual("/*c*/", tokens[1].LeadingTrivia[0].Text);
        }

        [Test]
        public void MacroExpansionPreservesLeadingTriviaOnMacroIdentifier()
        {
            var tokens = ShaderParser.PreProcessToTokens(
                "#define F x\n// hi\nF;", out var diags, out _);
            Assert.IsEmpty(diags);

            var xTok = FindToken(tokens, TokenKind.IdentifierToken, "x");
            Assert.AreEqual(1, xTok.LeadingTrivia.Count);
            Assert.AreEqual("// hi", xTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void IncludeDirectivePreservesLeadingTrivia()
        {
            var config = new HLSLParserConfig
            {
                BasePath = System.IO.Directory.GetCurrentDirectory()
            };
            var tokens = ShaderParser.PreProcessToTokens(
                "// before include\n#include \"TestShaders/Homemade/Include/IncludeB.hlsl\"",
                config, out var diags, out _);
            Assert.IsEmpty(diags);

            var voidTok = FindToken(tokens, TokenKind.VoidKeyword);
            Assert.AreEqual(1, voidTok.LeadingTrivia.Count);
            Assert.AreEqual("// before include", voidTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void ConditionalDirectivePreservesTriviaAroundDirectives()
        {
            var config = new HLSLParserConfig
            {
                Defines = new Dictionary<string, string> { { "X", "1" } }
            };
            var tokens = ShaderParser.PreProcessToTokens(
                "/*before*/\n#ifdef X\nint a;\n#endif", config, out var diags, out _);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.AreEqual(1, intTok.LeadingTrivia.Count);
            Assert.AreEqual("/*before*/", intTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void DefineDirectivePreservesLeadingTriviaOntoNextToken()
        {
            var tokens = ShaderParser.PreProcessToTokens(
                "/*doc*/\n#define X 1\nint a;", out var diags, out _);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.AreEqual(1, intTok.LeadingTrivia.Count);
            Assert.AreEqual("/*doc*/", intTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void OtherDirectivesPreserveLeadingTriviaOntoNextToken()
        {
            var config = new HLSLParserConfig
            {
                Defines = new Dictionary<string, string> { { "FOO", "1" } }
            };
            var tokens = ShaderParser.PreProcessToTokens(
                "/*pragma*/\n#pragma multi_compile _ FANCY\n/*undef*/\n#undef FOO\nint a;",
                config, out var diags, out _);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.AreEqual(2, intTok.LeadingTrivia.Count);
            Assert.AreEqual("/*pragma*/", intTok.LeadingTrivia[0].Text);
            Assert.AreEqual("/*undef*/", intTok.LeadingTrivia[1].Text);
        }

        [Test]
        public void StripDirectivesModePreservesLeadingTrivia()
        {
            var config = new HLSLParserConfig
            {
                PreProcessorMode = UnityShaderParser.HLSL.PreProcessor.PreProcessorMode.StripDirectives
            };
            var tokens = ShaderParser.PreProcessToTokens(
                "/*doc*/\n#define X 1\nint a;", config, out var diags, out _);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.AreEqual(1, intTok.LeadingTrivia.Count);
            Assert.AreEqual("/*doc*/", intTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void ConditionalDirectiveDropsTrailingCommentOnConditionLine()
        {
            var config = new HLSLParserConfig
            {
                Defines = new Dictionary<string, string> { { "X", "1" } }
            };
            var tokens = ShaderParser.PreProcessToTokens(
                "#ifdef X // for D3D11 only\nint a;\n#endif", config, out var diags, out _);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.IsEmpty(intTok.LeadingTrivia);
        }

        [Test]
        public void FunctionLikeMacroInvocationPreservesArgumentTriviaSpatially()
        {
            var tokens = ShaderParser.PreProcessToTokens(
                "#define f(x,y) x+y\nf(/*open*/ a, /*comma*/ b /*close*/)", out var diags, out _);
            Assert.IsEmpty(diags);

            var aTok = FindToken(tokens, TokenKind.IdentifierToken, "a");
            Assert.AreEqual(1, aTok.LeadingTrivia.Count);
            Assert.AreEqual("/*open*/", aTok.LeadingTrivia[0].Text);

            var bTok = FindToken(tokens, TokenKind.IdentifierToken, "b");
            Assert.AreEqual(1, bTok.LeadingTrivia.Count);
            Assert.AreEqual("/*comma*/", bTok.LeadingTrivia[0].Text);

            var eofTok = FindToken(tokens, TokenKind.EndOfFileToken);
            Assert.AreEqual(1, eofTok.LeadingTrivia.Count);
            Assert.AreEqual("/*close*/", eofTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void DirectiveDropsIntermediateFrameTrivia()
        {
            var tokens = ShaderParser.PreProcessToTokens(
                "#define X 1\n#undef X /*hint*/\nint a;", out var diags, out _);
            Assert.IsEmpty(diags);

            var intTok = FindToken(tokens, TokenKind.IntKeyword);
            Assert.IsEmpty(intTok.LeadingTrivia);
        }

        [Test]
        public void StringizeInMacroBodyDoesNotEmitStrayEndDirectiveToken()
        {
            var tokens = ShaderParser.PreProcessToTokens(
                "#define str(x) #x\nstr(foo)", out var diags, out _);
            Assert.IsEmpty(diags);

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.StringLiteralToken, tokens[0].Kind);
            Assert.AreEqual("foo", tokens[0].Identifier);
            Assert.AreEqual(TokenKind.EndOfFileToken, tokens[1].Kind);
        }

        [Test]
        public void NonMacroIdentifierTriviaIsNotDoubled()
        {
            var tokens = ShaderParser.PreProcessToTokens("// hi\na;", out var diags, out _);
            Assert.IsEmpty(diags);

            var aTok = FindToken(tokens, TokenKind.IdentifierToken, "a");
            Assert.AreEqual(1, aTok.LeadingTrivia.Count);
            Assert.AreEqual("// hi", aTok.LeadingTrivia[0].Text);
        }
    }

    public class ShaderLabCommentTriviaTests
    {
        private static Token<SLTokenKind> FindToken(IEnumerable<Token<SLTokenKind>> tokens, SLTokenKind kind)
        {
            foreach (var token in tokens)
            {
                if (token.Kind == kind)
                    return token;
            }
            Assert.Fail($"Did not find token of kind {kind}.");
            return null;
        }

        [Test]
        public void SingleLineCommentBeforeShaderKeywordIsLeadingTrivia()
        {
            var tokens = SLLexer.Lex("// my shader\nShader \"Foo\" {}", null, null, false, out var diags);
            Assert.IsEmpty(diags);

            var shaderTok = FindToken(tokens, SLTokenKind.ShaderKeyword);
            Assert.AreEqual(1, shaderTok.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, shaderTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("// my shader", shaderTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void MultiLineCommentBetweenTagsIsLeadingTriviaOfNextToken()
        {
            var source = "Shader \"Foo\" { SubShader { Tags { \"Queue\" /* render queue */ = \"Geometry\" \"RenderType\" = \"Opaque\" } } }";
            var tokens = SLLexer.Lex(source, null, null, false, out var diags);
            Assert.IsEmpty(diags);

            Token<SLTokenKind>? equalsTok = null;
            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];
                if (t.Kind == SLTokenKind.EqualsToken && t.HasLeadingTrivia)
                {
                    equalsTok = t;
                    break;
                }
            }
            Assert.IsNotNull(equalsTok);
            Assert.AreEqual(1, equalsTok!.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, equalsTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* render queue */", equalsTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void CommentInsidePropertyBlockSurvivesParsing()
        {
            var source = "Shader \"Foo\" {\n  Properties {\n    // the diffuse color\n    _Color (\"Color\", Color) = (1,1,1,1)\n  }\n  SubShader { Pass {} }\n}";
            var shader = ShaderParser.ParseUnityShader(source, out var diags);
            Assert.IsEmpty(diags);

            Token<SLTokenKind>? colorTok = null;
            foreach (var t in shader.Tokens)
            {
                if (t.Kind == SLTokenKind.IdentifierToken && t.Identifier == "_Color")
                {
                    colorTok = t;
                    break;
                }
            }
            Assert.IsNotNull(colorTok);
            Assert.AreEqual(1, colorTok!.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.SingleLineComment, colorTok.LeadingTrivia[0].Kind);
            Assert.AreEqual("// the diffuse color", colorTok.LeadingTrivia[0].Text);
        }

        [Test]
        public void CommentAfterPassBlockBecomesLeadingTriviaOfFollowingBrace()
        {
            var source = "SubShader { Pass {} /* end pass */ }";
            var subShader = ShaderParser.ParseUnitySubShader(source, out var diags);
            Assert.IsEmpty(diags);

            Token<SLTokenKind>? subShaderCloseBrace = null;
            for (int i = subShader.Tokens.Count - 1; i >= 0; i--)
            {
                var t = subShader.Tokens[i];
                if (t.Kind == SLTokenKind.CloseBraceToken && t.HasLeadingTrivia)
                {
                    subShaderCloseBrace = t;
                    break;
                }
            }
            Assert.IsNotNull(subShaderCloseBrace);
            Assert.AreEqual(1, subShaderCloseBrace!.LeadingTrivia.Count);
            Assert.AreEqual(SyntaxTriviaKind.MultiLineComment, subShaderCloseBrace.LeadingTrivia[0].Kind);
            Assert.AreEqual("/* end pass */", subShaderCloseBrace.LeadingTrivia[0].Text);
        }
    }
}

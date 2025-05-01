using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL.PreProcessor.Tests
{
    public class PositiveTests
    {
        [Test]
        public void PreProcessWithPredefinedFunctionLikeMacro()
        {
            var config = new HLSLParserConfig
            {
                Defines = new Dictionary<string, string>()
                {
                    { "FOO(texName)", "Texture2D texName;" }
                }
            };

            var decl = ShaderParser.ParseTopLevelDeclaration("FOO(bar)", config, out var diags, out _);

            Assert.IsEmpty(diags);

            var varDecl = decl as VariableDeclarationStatementNode;
            Assert.IsNotNull(varDecl);
            Assert.AreEqual("bar", varDecl?.Declarators[0].Name.Identifier);

            var typeDecl = varDecl?.Kind as PredefinedObjectTypeNode;
            Assert.AreEqual(PredefinedObjectType.Texture2D, typeDecl?.Kind);
        }

        private void CheckPositions(List<Token<HLSL.TokenKind>> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(i, tokens[i].Position);
            }
        }

        [Test]
        public void ExpandMacroUsesCorrectSpanAndPosition()
        {
            var expr = ShaderParser.ParseExpression("#define bar 33\n#define foo bar\n foo");

            Assert.AreEqual(32, expr.Span.StartIndex);
            Assert.AreEqual(3, expr.Span.Length);

            Assert.AreEqual(3, expr.Span.Start.Line);
            Assert.AreEqual(2, expr.Span.Start.Column);
            Assert.AreEqual(3, expr.Span.End.Line);
            Assert.AreEqual(5, expr.Span.End.Column);

            Assert.AreEqual(12, expr.OriginalSpan.StartIndex);
            Assert.AreEqual(2, expr.OriginalSpan.Length);

            Assert.AreEqual(1, expr.OriginalSpan.Start.Line);
            Assert.AreEqual(13, expr.OriginalSpan.Start.Column);
            Assert.AreEqual(1, expr.OriginalSpan.End.Line);
            Assert.AreEqual(15, expr.OriginalSpan.End.Column);

            CheckPositions(expr.Tokens);
        }

        [Test]
        public void TokenGluingPreservesSpan()
        {
            var expr = ShaderParser.ParseExpression("\"a\"\"b\"");

            Assert.AreEqual(0, expr.Span.StartIndex);
            Assert.AreEqual(6, expr.Span.Length);

            Assert.AreEqual(1, expr.Span.Start.Line);
            Assert.AreEqual(1, expr.Span.Start.Column);
            Assert.AreEqual(1, expr.Span.End.Line);
            Assert.AreEqual(7, expr.Span.End.Column);

            CheckPositions(expr.Tokens);
        }

        [Test]
        public void TokenGluingFromExpandedTokensPreservesSpan()
        {
            var expr = ShaderParser.ParseExpression("#define foo \"a\"\"b\"\n foo");

            Assert.AreEqual(20, expr.Span.StartIndex);
            Assert.AreEqual(3, expr.Span.Length);

            Assert.AreEqual(2, expr.Span.Start.Line);
            Assert.AreEqual(2, expr.Span.Start.Column);
            Assert.AreEqual(2, expr.Span.End.Line);
            Assert.AreEqual(5, expr.Span.End.Column);

            Assert.AreEqual(12, expr.OriginalSpan.StartIndex);
            Assert.AreEqual(6, expr.OriginalSpan.Length);

            Assert.AreEqual(1, expr.OriginalSpan.Start.Line);
            Assert.AreEqual(13, expr.OriginalSpan.Start.Column);
            Assert.AreEqual(1, expr.OriginalSpan.End.Line);
            Assert.AreEqual(19, expr.OriginalSpan.End.Column);

            CheckPositions(expr.Tokens);
        }

        [Test]
        public void FunctionLikeMacroDoesNotRoundtripSpan()
        {
            var expr = ShaderParser.ParseExpression("#define f(a) a\nf(b)");

            Assert.AreEqual(15, expr.Span.StartIndex);
            Assert.AreEqual(4, expr.Span.Length);

            Assert.AreEqual(2, expr.Span.Start.Line);
            Assert.AreEqual(1, expr.Span.Start.Column);
            Assert.AreEqual(2, expr.Span.End.Line);
            Assert.AreEqual(5, expr.Span.End.Column);

            Assert.AreEqual(13, expr.OriginalSpan.StartIndex);
            Assert.AreEqual(1, expr.OriginalSpan.Length);

            Assert.AreEqual(1, expr.OriginalSpan.Start.Line);
            Assert.AreEqual(14, expr.OriginalSpan.Start.Column);
            Assert.AreEqual(1, expr.OriginalSpan.End.Line);
            Assert.AreEqual(15, expr.OriginalSpan.End.Column);

            CheckPositions(expr.Tokens);
        }

        [Test]
        public void TokenConcatenationPreservesSpan()
        {
            var expr = ShaderParser.ParseExpression("#define f(a) foo##a\n f(c)");

            Assert.AreEqual(21, expr.Span.StartIndex);
            Assert.AreEqual(4, expr.Span.Length);

            Assert.AreEqual(2, expr.Span.Start.Line);
            Assert.AreEqual(2, expr.Span.Start.Column);
            Assert.AreEqual(2, expr.Span.End.Line);
            Assert.AreEqual(6, expr.Span.End.Column);

            Assert.AreEqual(13, expr.OriginalSpan.StartIndex);
            Assert.AreEqual(6, expr.OriginalSpan.Length);

            Assert.AreEqual(1, expr.OriginalSpan.Start.Line);
            Assert.AreEqual(14, expr.OriginalSpan.Start.Column);
            Assert.AreEqual(1, expr.OriginalSpan.End.Line);
            Assert.AreEqual(19, expr.OriginalSpan.End.Column);

            CheckPositions(expr.Tokens);
        }

        [Test]
        public void LineOffsetWorks()
        {
            var expr = ShaderParser.ParseExpression("#define foo 3\n #line 6\n foo");

            Assert.AreEqual(6, expr.Span.Start.Line);
            Assert.AreEqual(1, expr.OriginalSpan.Start.Line);

            CheckPositions(expr.Tokens);
        }

        [Test]
        public void IncludeDirectivePreservesSpan()
        {
            var config = new HLSLParserConfig
            {
                BasePath = Directory.GetCurrentDirectory()
            };
            var decls = ShaderParser.ParseTopLevelDeclarations("void C() {}\n#include \"TestShaders/Homemade/IncludeA.hlsl\"", config);


            CheckPositions(decls.SelectMany(x => x.Tokens).ToList());
        }

        [Test]
        public void DelayedFunctionLikeMacrosAreExpandedCorrectly()
        {
            var testCode = @"
                #define XX(x,y) x y
                #define YY XX
                #define ZZ YY

                ZZ(int, foo;)

                #define RR() int bar;
                #define GG RR
                #define BB GG

                BB()
            ";

            string expanded = ShaderParser.PreProcessToString(testCode, new HLSLParserConfig() { ThrowExceptionOnError = true });

            Assert.AreEqual("int foo ; int bar ;", expanded.Trim());
        }

        [Test]
        public void StringizingMacroTokenWorks()
        {
            var testCode = @"
                #define FOO(x, y) string x = #x; string x##x = #y;
                FOO(bar, baz)
            ";

            string expanded = ShaderParser.PreProcessToString(testCode, new HLSLParserConfig() { ThrowExceptionOnError = true });

            Assert.AreEqual("string bar = \"bar\" ; string barbar = \"baz\" ;", expanded.Trim());
        }
    }

    public class NegativeTests
    {
        [Test]
        public void PreProcessUnterminatedDirective()
        {
            var decl = ShaderParser.ParseTopLevelDeclaration("#ifdef bla bla bla bla #if #endif", out var diags, out _);
            Assert.AreEqual(diags.Count, 2);
        }
    }
}

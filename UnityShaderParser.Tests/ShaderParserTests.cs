using NUnit.Framework;
using System.Collections.Generic;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

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
            var shader = ShaderParser.ParseUnityShader("Shader \"foobar\" { SubShader { Pass { CGINCLUDE\n void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } void bar() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } \nENDCG\n } } }");
            Assert.Greater(shader.Children.Count, 0);
            CheckParents(shader);

            var decls = ShaderParser.ParseTopLevelDeclarations("void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } void bar() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; } ");
            Assert.Greater(decls.Count, 0);
            foreach (var c in decls)
            {
                Assert.Greater(c.Children.Count, 0);
                CheckParents(c);
            }

            var decl = ShaderParser.ParseTopLevelDeclaration("void foo() { int a = 1 + (3 * 4 + { 1, 2, 3 }) * a; }");
            Assert.Greater(decl.Children.Count, 0);
            CheckParents(decl);

            var stmt = ShaderParser.ParseStatement("int a = 1 + (3 * 4 + { 1, 2, 3 }) * a;");
            Assert.Greater(stmt.Children.Count, 0);
            CheckParents(stmt);

            var expr = ShaderParser.ParseExpression("1 + (3 * 4 + { 1, 2, 3 }) * a");
            Assert.Greater(expr.Children.Count, 0);
            CheckParents(expr);
        }

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
            Assert.AreEqual("bar", varDecl?.Declarators[0].Name);

            var typeDecl = varDecl?.Kind as PredefinedObjectTypeNode;
            Assert.AreEqual(PredefinedObjectType.Texture2D, typeDecl?.Kind);
        }
    }
}

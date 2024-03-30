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
    }
}

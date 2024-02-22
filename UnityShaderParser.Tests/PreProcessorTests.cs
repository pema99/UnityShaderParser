using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            Assert.AreEqual("bar", varDecl?.Declarators[0].Name);

            var typeDecl = varDecl?.Kind as PredefinedObjectTypeNode;
            Assert.AreEqual(PredefinedObjectType.Texture2D, typeDecl?.Kind);
        }
    }

    public class NegativeTests
    {
        [Test]
        public void PreProcessUnterminatedDirective()
        {
            var decl = ShaderParser.ParseTopLevelDeclaration("#ifdef bla bla bla bla #if #endif", out var diags, out _);
            Assert.AreEqual(diags.Count, 1);
        }
    }
}

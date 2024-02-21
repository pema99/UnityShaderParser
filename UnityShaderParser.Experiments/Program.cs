﻿using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.PreProcessor;

string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Tests\TestShaders\Homemade\Tricky.hlsl";
string shaderSource = File.ReadAllText(shaderPath);

// Ignore macros for the purpose of editing
var config = new HLSLParserConfig() { PreProcessorMode = PreProcessorMode.StripDirectives };

List<HLSLSyntaxNode> decls = ShaderParser.ParseTopLevelDeclarations(shaderSource, config);

string editedShaderSource = HLSLEditor.RunEditor<IfConditionReplacer>(shaderSource, decls);
Console.WriteLine(editedShaderSource);

class IfConditionReplacer : HLSLEditor
{
    public IfConditionReplacer(string source, List<Token<UnityShaderParser.HLSL.TokenKind>> tokens)
        : base(source, tokens) { }

    public override void VisitIfStatementNode(IfStatementNode node)
    {
        Edit(node.Condition, "true"); // replace conditions with 'true'
        base.VisitIfStatementNode(node);
    }
}
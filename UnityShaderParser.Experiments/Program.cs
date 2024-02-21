using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.PreProcessor;

string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Tests\TestShaders\Homemade\Tricky.hlsl";
string shaderSource = File.ReadAllText(shaderPath);

var config = new HLSLParserConfig()
{
    // Ignore macros for the purpose of editing
    PreProcessorMode = PreProcessorMode.StripDirectives
};

List<HLSLSyntaxNode> decls = ShaderParser.ParseTopLevelDeclarations(shaderSource, config);

string editedShaderSource = HLSLEditor.RunEditor<HLSLEditorTest>(shaderSource, decls);
Console.WriteLine(editedShaderSource);

class HLSLEditorTest : HLSLEditor
{
    public HLSLEditorTest(string source, List<Token<UnityShaderParser.HLSL.TokenKind>> tokens)
        : base(source, tokens) { }

    public override void VisitIfStatementNode(IfStatementNode node)
    {
        Edit(node.Condition, "true"); // replace conditions with 'true'
        base.VisitIfStatementNode(node);
    }
}
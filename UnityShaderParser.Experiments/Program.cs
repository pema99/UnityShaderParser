using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.PreProcessor;
using UnityShaderParser.ShaderLab;

string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Tests\TestShaders\Homemade\Tricky.hlsl";
string shaderSource = File.ReadAllText(shaderPath);

var config = new HLSLParserConfig()
{
    PreProcessorMode = PreProcessorMode.StripDirectives
};

var decls = ShaderParser.ParseTopLevelDeclarations(
    shaderSource,
    config,
    out var diags,
    out var pragmas);

if (diags.Count > 0)
{
    throw new Exception(diags.First().ToString());
}

Console.WriteLine(HLSLEditor.RunEditor<HLSLEditorTest>(shaderSource, decls));

//HLSLPrinter printer  = new HLSLPrinter();
//foreach (var decl in decls)
//{
//    printer.Visit(decl);
//}
//Console.Write(printer.Text);


class HLSLEditorTest : HLSLEditor
{
    public HLSLEditorTest(string source, List<Token<UnityShaderParser.HLSL.TokenKind>> tokens)
        : base(source, tokens)
    {}

    public override void VisitIfStatementNode(IfStatementNode node)
    {
        Edit(node.Condition, "BLYAT");

        base.VisitIfStatementNode(node);
    }
}
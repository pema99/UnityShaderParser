using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.PreProcessor;
using UnityShaderParser.ShaderLab;

var decls = ShaderParser.ParseTopLevelDeclarations(
    File.ReadAllText(@"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Tests\TestShaders\Homemade\Tricky.hlsl"),
    out var diags,
    out var pragmas);
if (diags.Count > 0)
    throw new Exception(diags.First().ToString());

HLSLPrinter printer  = new HLSLPrinter();
foreach (var decl in decls)
{
    printer.Visit(decl);
}
Console.Write(printer.Text);

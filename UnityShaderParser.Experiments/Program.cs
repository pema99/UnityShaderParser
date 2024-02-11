using UnityShaderParser.PreProcessor;
using UnityShaderParser.ShaderLab;

string path = "D:\\Projects\\UnityShaderParser\\UnityShaderParser\\UnityShaderParser.Tests\\TestShaders\\PoiyomiToon\\Poiyomi Early Outline.shader";
string source = File.ReadAllText(path);

var tokens = ShaderLabLexer.Lex(source, false, out var lexerDiags);

string cgIncludesPath = Path.Combine(Directory.GetCurrentDirectory(), "D:\\Projects\\UnityShaderParser\\UnityShaderParser\\UnityShaderParser.Tests\\TestShaders\\UnityBuiltinShaders\\CGIncludes");
var config = new ShaderLabParserConfig
{
    ParseEmbeddedHLSL = true,
    ThrowExceptionOnError = false,
    IncludeResolver = new DefaultPreProcessorIncludeResolver(new List<string> { cgIncludesPath }),
    BasePath = Directory.GetParent(path)?.FullName,
    Defines = new Dictionary<string, string>()
                {
                    { "SHADER_API_D3D11", "1" }
                },
};
var parsed = ShaderLabParser.Parse(tokens, config, out var parserDiags);
;
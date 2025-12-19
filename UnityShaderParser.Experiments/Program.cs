using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.HLSL.PreProcessor;
using UnityShaderParser.Test;
using System.IO;

public class Program
{
    public static void Main()
    {
        string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Experiments\Test.hlsl";
        string shaderSource = File.ReadAllText(shaderPath);

        // Ignore macros for the purpose of editing
        var config = new HLSLParserConfig() { PreProcessorMode = PreProcessorMode.StripDirectives };

        var stmts = ShaderParser.ParseStatements(shaderSource, config, out var diags, out var prags);

        HLSLInterpreter interpreter = new HLSLInterpreter();
        interpreter.VisitMany(stmts);

    }
}
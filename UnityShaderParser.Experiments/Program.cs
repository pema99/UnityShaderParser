using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.HLSL.PreProcessor;
using UnityShaderParser.Test;
using System.IO;
using System.Collections.Generic;
using System;

public class Program
{
    public static void Main()
    {
        string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Experiments\Test.hlsl";
        string shaderSource = File.ReadAllText(shaderPath);

        // Ignore macros for the purpose of editing
        var config = new HLSLParserConfig()
        {
            PreProcessorMode = PreProcessorMode.ExpandAll,
            Defines = new Dictionary<string, string>() { { "HLSL_TEST", "1" } }
        };

        HLSLRunner runner = new HLSLRunner();
        runner.ProcessCode(shaderSource, config, out var diags, out var prags);
        var results = runner.RunTests();

        foreach (var result in results)
        {
            Console.WriteLine($"=== Test: {result.TestName} ===");
            Console.WriteLine("Result: " + (result.Pass ? "Pass" : "Fail"));
            if (!string.IsNullOrEmpty(result.Message))
                Console.WriteLine("Message: " + result.Message);
            if (!string.IsNullOrEmpty(result.Log))
            {
                Console.WriteLine("Log:");
                Console.WriteLine(result.Log);
            }
            Console.WriteLine();
        }
    }
}
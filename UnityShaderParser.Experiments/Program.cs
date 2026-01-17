using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.HLSL.PreProcessor;
using UnityShaderParser.Test;

public class Program
{
    public struct ColorRGBA
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public ColorRGBA(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    public static class BitmapWriter
    {
        public static void WriteBmp(string filePath, ColorRGBA[,] pixels)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            // BMP rows are padded to multiples of 4 bytes
            int bytesPerPixel = 3; // 24-bit RGB
            int rowSize = (width * bytesPerPixel + 3) & ~3;
            int pixelDataSize = rowSize * height;
            int fileSize = 54 + pixelDataSize; // 54 = BMP header size

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                // === BITMAP FILE HEADER (14 bytes) ===
                bw.Write((byte)'B');
                bw.Write((byte)'M');
                bw.Write(fileSize);
                bw.Write(0); // Reserved
                bw.Write(54); // Offset to pixel data

                // === DIB HEADER (40 bytes) ===
                bw.Write(40);        // DIB header size
                bw.Write(width);
                bw.Write(height);
                bw.Write((short)1); // Color planes
                bw.Write((short)24); // Bits per pixel
                bw.Write(0);        // Compression (0 = none)
                bw.Write(pixelDataSize);
                bw.Write(0);        // Horizontal resolution (pixels/meter)
                bw.Write(0);        // Vertical resolution
                bw.Write(0);        // Colors in palette
                bw.Write(0);        // Important colors

                // === PIXEL DATA ===
                // BMP stores pixels bottom-to-top
                byte[] padding = new byte[rowSize - width * bytesPerPixel];

                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var c = pixels[x, y];

                        // BMP uses BGR order
                        bw.Write(c.B);
                        bw.Write(c.G);
                        bw.Write(c.R);
                    }

                    // Row padding
                    bw.Write(padding);
                }
            }
        }
    }

    public static void RunRaymarcher()
    {
        string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Experiments\Examples\Raymarcher.hlsl";
        string shaderSource = File.ReadAllText(shaderPath);

        // Ignore macros for the purpose of editing
        var config = new HLSLParserConfig()
        {
            PreProcessorMode = PreProcessorMode.ExpandAll,
            Defines = new Dictionary<string, string>() { { "HLSL_TEST", "1" } }
        };

        var toks = ShaderParser.ParseTopLevelDeclarations(shaderSource, config, out var diags, out var prags);

        int resolutionX = 92;
        int resolutionY = 92;
        int warpSize = 4;
        ColorRGBA[,] colors = new ColorRGBA[resolutionX, resolutionY];
        int progress = 0;

        var sw = System.Diagnostics.Stopwatch.StartNew();
#if DEBUG
        for (int y = 0; y < resolutionY / warpSize; y++)
#else
        Parallel.For(0, resolutionY / warpSize, y =>
#endif
        {
            HLSLRunner runner = new HLSLRunner();
            runner.ProcessCode(toks);
            runner.SetVariable("_Time", new VectorValue(ScalarType.Float, new HLSLRegister<object[]>(new object[] { 0f, 0f })));
            runner.SetVariable("_Resolution", new ScalarValue(ScalarType.Float, new HLSLRegister<object>(1f)));

            var uvs = new object[warpSize * warpSize][];
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = new object[2];

            for (int x = 0; x < resolutionX / warpSize; x++)
            {
                var v2fdict = new Dictionary<string, HLSLValue>();
                for (int warpY = 0; warpY < warpSize; warpY++)
                {
                    for (int warpX = 0; warpX < warpSize; warpX++)
                    {
                        uvs[warpY * warpSize + warpX][0] = ((float)x * warpSize + warpX) / resolutionX;
                        uvs[warpY * warpSize + warpX][1] = 1.0f - ((float)y * warpSize + warpY) / resolutionY;
                    }
                }
                v2fdict["uv"] = new VectorValue(ScalarType.Float, new HLSLRegister<object[]>(uvs).Converge());
                var v2f = new StructValue("v2f", v2fdict);

                var color = runner.CallFunctionWithWarpSize("frag", warpSize, warpSize, v2f);
                for (int warpY = 0; warpY < warpSize; warpY++)
                {
                    for (int warpX = 0; warpX < warpSize; warpX++)
                    {
                        var colorVec = ((VectorValue)color).Values.Get(warpY * warpSize + warpX);
                        colors[x * warpSize + warpX, y * warpSize + warpY] = new ColorRGBA(
                            (byte)(Math.Clamp(Convert.ToSingle(colorVec[0]), 0, 1) * 255),
                            (byte)(Math.Clamp(Convert.ToSingle(colorVec[1]), 0, 1) * 255),
                            (byte)(Math.Clamp(Convert.ToSingle(colorVec[2]), 0, 1) * 255),
                            (byte)(Math.Clamp(Convert.ToSingle(colorVec[3]), 0, 1) * 255)
                        );
                    }
                }
            }
            Console.WriteLine($"{Interlocked.Add(ref progress, 1) / (float)(resolutionY / warpSize) * 100f}%");
        }
#if !DEBUG
        );
#endif
        sw.Stop();
        Console.WriteLine("Took " + sw.ElapsedMilliseconds / 1000.0f + " seconds.");
        BitmapWriter.WriteBmp("output.bmp", colors);

    }

    public static void RunTests()
    {
        List<HLSLRunner.TestResult> results = new List<HLSLRunner.TestResult>();
        foreach (var file in Directory.GetFiles(@"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Experiments\Tests"))
        {
            string shaderSource = File.ReadAllText(file);

        // Ignore macros for the purpose of editing
        var config = new HLSLParserConfig()
        {
            PreProcessorMode = PreProcessorMode.ExpandAll,
            Defines = new Dictionary<string, string>() { { "HLSL_TEST", "1" } }
        };

        HLSLRunner runner = new HLSLRunner();
        runner.ProcessCode(shaderSource, config, out var diags, out var prags);
            results.AddRange(runner.RunTests());
        }

        foreach (var result in results)
        {
            bool hasMessage = !string.IsNullOrEmpty(result.Message);
            bool hasLog = !string.IsNullOrEmpty(result.Log);

            if (hasMessage || hasLog || !result.Pass)
            {
                Console.WriteLine($"Test: {result.TestName}");
                Console.WriteLine("Result: " + (result.Pass ? "Pass" : "Fail"));
            }
            if (hasMessage)
                Console.WriteLine("Message: " + result.Message);
            if (hasLog)
            {
                Console.WriteLine("Log:");
                Console.WriteLine(result.Log);
            }
            if (hasMessage || hasLog || !result.Pass)
                Console.WriteLine();
        }

        Console.WriteLine($"=== Results: {results.Count(x => x.Pass)} passed, {results.Count(x => !x.Pass)} failed ===");
    }

    public static void Main()
    {
        //RunRaymarcher();
        RunTests();   
    }
}
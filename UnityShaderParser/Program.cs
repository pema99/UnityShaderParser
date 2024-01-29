using UnityShaderParser.ShaderLab;

public class Program
{
    public static void Main()
    {
        ShaderLabLexer.Lex(File.ReadAllText("NewUnlitShader 1.shader"), out var tokens, out var lexerDiags);
        Console.WriteLine("== Lexer errors ==");
        foreach (var diag in lexerDiags)
        {
            Console.WriteLine(diag);
        }

        Console.WriteLine("== Parser errors ==");
        ShaderLabParser.Parse(tokens, out ShaderNode shader, out var parserDiags);
        foreach (var diag in parserDiags)
        {
            Console.WriteLine(diag);
        }
    }
}
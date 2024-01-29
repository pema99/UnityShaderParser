using UnityShaderParser.ShaderLab;

public class Program
{
    // This example visitor implementation just counts the amount of properties which are 2D textures
    public class TestVisitor : ShaderLabSyntaxVisitor
    {
        public int Num2DTextureProperties { get; private set; } = 0;

        public override void VisitShaderPropertyValueTextureNode(ShaderPropertyValueTextureNode node)
        {
            if (node.Kind == TextureType.Texture2D)
            {
                Num2DTextureProperties++;
            }

            base.VisitShaderPropertyValueTextureNode(node);
        }
    }

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

        TestVisitor test = new TestVisitor();
        test.VisitSyntaxNode(shader);

        Console.WriteLine(test.Num2DTextureProperties);
    }
}
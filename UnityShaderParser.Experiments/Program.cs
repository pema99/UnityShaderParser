using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

string shaderPath = @"D:\Projects\UnityShaderParser\UnityShaderParser\UnityShaderParser.Tests\TestShaders\Homemade\Tricky.hlsl";
string shaderSource = File.ReadAllText(shaderPath);
Console.WriteLine($"Before:\n{shaderSource}\n");

List<HLSLSyntaxNode> decls = ShaderParser.ParseTopLevelDeclarations(shaderSource);

string editedShaderSource = HLSLEditor.RunEditor<DemoReplacer>(shaderSource, decls);
Console.WriteLine($"After (Default/FirstWins):\n{editedShaderSource}\n");

editedShaderSource = HLSLEditor.RunEditor<DemoReplacer>(shaderSource, decls, EditConflictResolutionMode.FirstWins);
Console.WriteLine($"After (FirstWins):\n{editedShaderSource}\n");

editedShaderSource = HLSLEditor.RunEditor<DemoReplacer>(shaderSource, decls, EditConflictResolutionMode.LastWins);
Console.WriteLine($"After (LastWins):\n{editedShaderSource}\n");

editedShaderSource = HLSLEditor.RunEditor<DemoReplacer>(shaderSource, decls, EditConflictResolutionMode.Custom, (l, r) =>
{
    // Always keep edits to body of an if-statement with condition 'true'. Never keep if condition is 'false'.
    if (l.Node.Parent is IfStatementNode { Condition: LiteralExpressionNode { Lexeme: var lcond } })
    {
        return lcond == "true" ? EditConflictResolution.KeepFirst : EditConflictResolution.KeepSecond;
    }
    else if (r.Node.Parent is IfStatementNode { Condition: LiteralExpressionNode { Lexeme: var rcond } })
    {
        return rcond == "true" ? EditConflictResolution.KeepSecond : EditConflictResolution.KeepFirst;
    }
    // Default to keeping the first edit.
    return EditConflictResolution.KeepFirst;
});
Console.WriteLine($"After (Custom):\n{editedShaderSource}\n");

class DemoReplacer : HLSLEditor
{
    public DemoReplacer(
        string source,
        List<Token<TokenKind>> tokens,
        EditConflictResolutionMode conflictResolutionMode,
        EditConflictHandler<TokenKind, HLSLSyntaxNode> conflictHandler)
        : base(source, tokens, conflictResolutionMode, conflictHandler) { }

    public override void VisitIfStatementNode(IfStatementNode node)
    {
        Edit(node.Body, "{}"); // replace body with empty block
        base.VisitIfStatementNode(node);
    }

    public override void VisitVariableDeclarationStatementNode(VariableDeclarationStatementNode node)
    {
        Edit(node.Declarators[0].Name, "foo"); // replace variable name with "foo"
        base.VisitVariableDeclarationStatementNode(node);
    }
}
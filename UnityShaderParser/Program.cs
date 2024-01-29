using System.Text;

public enum TokenKind
{
    None,

    TrueKeyword,
    FalseKeyword,

    OpenParenToken,
    CloseParenToken,
    OpenBracketToken,
    CloseBracketToken,
    OpenBraceToken,
    CloseBraceToken,

    SemiToken,
    CommaToken,

    LessThanToken,
    LessThanEqualsToken,
    GreaterThanToken,
    GreaterThanEqualsToken,
    LessThanLessThanToken,
    GreaterThanGreaterThanToken,
    PlusToken,
    PlusPlusToken,
    MinusToken,
    MinusMinusToken,
    AsteriskToken,
    SlashToken,
    PercentToken,
    AmpersandToken,
    BarToken,
    AmpersandAmpersandToken,
    BarBarToken,
    CaretToken,
    NotToken,
    TildeToken,
    QuestionToken,
    ColonToken,
    ColonColonToken,

    EqualsToken,
    AsteriskEqualsToken,
    SlashEqualsToken,
    PercentEqualsToken,
    PlusEqualsToken,
    MinusEqualsToken,
    LessThanLessThanEqualsToken,
    GreaterThanGreaterThanEqualsToken,
    AmpersandEqualsToken,
    CaretEqualsToken,
    BarEqualsToken,

    EqualsEqualsToken,
    ExclamationEqualsToken,
    DotToken,

    IdentifierToken,
    IntegerLiteralToken,
    FloatLiteralToken,
    StringLiteralToken,
    BracketedStringLiteralToken,

    ShaderKeyword,
    PropertiesKeyword,
    RangeKeyword,
    FloatKeyword,
    IntKeyword,
    IntegerKeyword,
    ColorKeyword,
    VectorKeyword,
    _2DKeyword,
    _3DKeyword,
    CubeKeyword,
    AnyKeyword,
    CategoryKeyword,
    SubShaderKeyword,
    TagsKeyword,
    PassKeyword,
    CgProgramKeyword,
    CgIncludeKeyword,
    EndCgKeyword,
    HlslProgramKeyword,
    HlslIncludeKeyword,
    EndHlslKeyword,
    FallbackKeyword,
    CustomEditorKeyword,
    CullKeyword,
    ZWriteKeyword,
    ZTestKeyword,
    OffsetKeyword,
    BlendKeyword,
    BlendOpKeyword,
    ColorMaskKeyword,
    AlphaToMaskKeyword,
    LodKeyword,
    NameKeyword,
    LightingKeyword,
    StencilKeyword,
    RefKeyword,
    ReadMaskKeyword,
    WriteMaskKeyword,
    CompKeyword,
    CompBackKeyword,
    CompFrontKeyword,
    FailKeyword,
    ZFailKeyword,
    UsePassKeyword,
    GrabPassKeyword,
    DependencyKeyword,
    MaterialKeyword,
    DiffuseKeyword,
    AmbientKeyword,
    ShininessKeyword,
    SpecularKeyword,
    EmissionKeyword,
    FogKeyword,
    ModeKeyword,
    DensityKeyword,
    SeparateSpecularKeyword,
    SetTextureKeyword,
    CombineKeyword,
    AlphaKeyword,
    LerpKeyword,
    DoubleKeyword,
    QuadKeyword,
    ConstantColorKeyword,
    MatrixKeyword,
    AlphaTestKeyword,
    ColorMaterialKeyword,
    BindChannelsKeyword,
    BindKeyword,

    IncludeBlock,
    ProgramBlock,
}

public struct SourceSpan
{
    public (int Line, int Column) Start;
    public (int Line, int Column) End;

    public override string ToString() => $"({Start.Line}:{Start.Column} - {End.Line}:{End.Column})";
}

public struct Token
{
    public TokenKind Kind;
    public string? Identifier;
    public SourceSpan Span;

    public override string ToString()
    {
        if (Identifier == null)
            return Kind.ToString();
        else if (Kind == TokenKind.BracketedStringLiteralToken)
            return $"{Kind}: [{Identifier}]";
        else if (Kind == TokenKind.StringLiteralToken)
            return $"{Kind}: \"{Identifier}\"";
        else
            return $"{Kind}: {Identifier}";
    }
}

public static class SyntaxFacts
{
    public static bool IsAlphaNumericOrUnderscore(char c) => c == '_' || char.IsLetterOrDigit(c);

    public static bool TryParseShaderLabKeyword(string keyword, out TokenKind token)
    {
        token = default;

        switch (keyword.ToLower())
        {
            case "shader": token = TokenKind.ShaderKeyword; return true;
            case "properties": token = TokenKind.PropertiesKeyword; return true;
            case "range": token = TokenKind.RangeKeyword; return true;
            case "float": token = TokenKind.FloatKeyword; return true;
            case "integer": token = TokenKind.IntegerKeyword; return true;
            case "int": token = TokenKind.IntKeyword; return true;
            case "color": token = TokenKind.ColorKeyword; return true;
            case "vector": token = TokenKind.VectorKeyword; return true;
            case "2d": token = TokenKind._2DKeyword; return true;
            case "3d": token = TokenKind._3DKeyword; return true;
            case "cube": token = TokenKind.CubeKeyword; return true;
            case "any": token = TokenKind.AnyKeyword; return true;
            case "category": token = TokenKind.CategoryKeyword; return true;
            case "subshader": token = TokenKind.SubShaderKeyword; return true;
            case "tags": token = TokenKind.TagsKeyword; return true;
            case "pass": token = TokenKind.PassKeyword; return true;
            case "cgprogram": token = TokenKind.CgProgramKeyword; return true;
            case "cginclude": token = TokenKind.CgIncludeKeyword; return true;
            case "endcg": token = TokenKind.EndCgKeyword; return true;
            case "hlslprogram": token = TokenKind.HlslProgramKeyword; return true;
            case "hlslinclude": token = TokenKind.HlslIncludeKeyword; return true;
            case "endhlsl": token = TokenKind.EndHlslKeyword; return true;
            case "fallback": token = TokenKind.FallbackKeyword; return true;
            case "customeditor": token = TokenKind.CustomEditorKeyword; return true;
            case "cull": token = TokenKind.CullKeyword; return true;
            case "zwrite": token = TokenKind.ZWriteKeyword; return true;
            case "ztest": token = TokenKind.ZTestKeyword; return true;
            case "offset": token = TokenKind.OffsetKeyword; return true;
            case "blend": token = TokenKind.BlendKeyword; return true;
            case "blendop": token = TokenKind.BlendOpKeyword; return true;
            case "colormask": token = TokenKind.ColorMaskKeyword; return true;
            case "alphatomask": token = TokenKind.AlphaToMaskKeyword; return true;
            case "lod": token = TokenKind.LodKeyword; return true;
            case "name": token = TokenKind.NameKeyword; return true;
            case "lighting": token = TokenKind.LightingKeyword; return true;
            case "stencil": token = TokenKind.StencilKeyword; return true;
            case "ref": token = TokenKind.RefKeyword; return true;
            case "readmask": token = TokenKind.ReadMaskKeyword; return true;
            case "writemask": token = TokenKind.WriteMaskKeyword; return true;
            case "comp": token = TokenKind.CompKeyword; return true;
            case "compback": token = TokenKind.CompBackKeyword; return true;
            case "compfront": token = TokenKind.CompFrontKeyword; return true;
            case "fail": token = TokenKind.FailKeyword; return true;
            case "zfail": token = TokenKind.ZFailKeyword; return true;
            case "usepass": token = TokenKind.UsePassKeyword; return true;
            case "grabpass": token = TokenKind.GrabPassKeyword; return true;
            case "dependency": token = TokenKind.DependencyKeyword; return true;
            case "material": token = TokenKind.MaterialKeyword; return true;
            case "diffuse": token = TokenKind.DiffuseKeyword; return true;
            case "ambient": token = TokenKind.AmbientKeyword; return true;
            case "shininess": token = TokenKind.ShininessKeyword; return true;
            case "specular": token = TokenKind.SpecularKeyword; return true;
            case "emission": token = TokenKind.EmissionKeyword; return true;
            case "fog": token = TokenKind.FogKeyword; return true;
            case "mode": token = TokenKind.ModeKeyword; return true;
            case "density": token = TokenKind.DensityKeyword; return true;
            case "separatespecular": token = TokenKind.SeparateSpecularKeyword; return true;
            case "settexture": token = TokenKind.SetTextureKeyword; return true;
            case "combine": token = TokenKind.CombineKeyword; return true;
            case "alpha": token = TokenKind.AlphaKeyword; return true;
            case "lerp": token = TokenKind.LerpKeyword; return true;
            case "double": token = TokenKind.DoubleKeyword; return true;
            case "quad": token = TokenKind.QuadKeyword; return true;
            case "constantcolor": token = TokenKind.ConstantColorKeyword; return true;
            case "matrix": token = TokenKind.MatrixKeyword; return true;
            case "alphatest": token = TokenKind.AlphaTestKeyword; return true;
            case "colormaterial": token = TokenKind.ColorMaterialKeyword; return true;
            case "bindchannels": token = TokenKind.BindChannelsKeyword; return true;
            case "bind": token = TokenKind.BindKeyword; return true;
            case "true": token = TokenKind.TrueKeyword; return true;
            case "false": token = TokenKind.FalseKeyword; return true;
            default: return false;
        }
    }
}

public class ShaderLabLexer
{
    private string source = string.Empty;
    private int position = 0;
    private int line = 1;
    private int column = 1;
    private int anchorLine = 1;
    private int anchorColumn = 1;

    private List<Token> tokens = new();
    private List<string> diagnostics = new();

    private ShaderLabLexer(string source)
    {
        this.source = source;
    }

    public char Peek() => IsAtEnd() ? '\0' : source[position];
    public char LookAhead(int offset = 1) => IsAtEnd(offset) ? '\0' : source[position + offset];
    public bool LookAhead(char c, int offset = 1) => LookAhead(offset) == c;
    public bool Match(char tok) => Peek() == tok;
    public bool IsAtEnd(int offset = 0) => position + offset >= source.Length;
    public void Add(string identifier, TokenKind kind) => tokens.Add(new Token { Identifier = identifier, Kind = kind, Span = GetCurrentSpan() });
    public void Add(TokenKind kind) => tokens.Add(new() { Kind = kind, Span = GetCurrentSpan() });
    public void Eat(char tok)
    {
        if (!Match(tok))
            diagnostics.Add($"Error at line {line} column {column}: Expected token '{tok}', got '{Peek()}'.");
        Advance();
    }
    public char Advance(int amount = 1)
    {
        if (IsAtEnd(amount - 1))
            return '\0';
        column++;
        if (Peek() == '\n')
        {
            column = 1;
            line++;
        }
        char result = source[position];
        position += amount;
        return result;
    }

    private void StartCurrentSpan()
    {
        anchorLine = line;
        anchorColumn = column;
    }

    private SourceSpan GetCurrentSpan()
    {
        return new SourceSpan
        {
            Start = (anchorLine, anchorColumn),
            End = (line, column)
        };
    }

    public static void Lex(string source, out List<Token> tokens, out List<string> diagnostics)
    {
        ShaderLabLexer lexer = new(source);

        lexer.Lex();

        tokens = lexer.tokens;
        diagnostics = lexer.diagnostics;
    }

    private void Lex()
    {
        while (!IsAtEnd())
        {
            StartCurrentSpan();

            switch (Peek())
            {
                case char c when char.IsLetter(c) || c == '_':
                    LexIdentifier();
                    break;

                case '2' when LookAhead('D') || LookAhead('d'): Advance(2); Add(TokenKind._2DKeyword); break;
                case '3' when LookAhead('D') || LookAhead('d'): Advance(2); Add(TokenKind._3DKeyword); break;

                case char c when char.IsDigit(c):
                    LexNumber();
                    break;

                case '"':
                    LexString('"', '"', TokenKind.StringLiteralToken);
                    break;

                case '[' when SyntaxFacts.IsAlphaNumericOrUnderscore(LookAhead()):
                    LexString('[', ']', TokenKind.StringLiteralToken);
                    break;

                case ' ' or '\t' or '\r' or '\n':
                    Advance();
                    break;

                case '(': Advance(); Add(TokenKind.OpenParenToken); break;
                case ')': Advance(); Add(TokenKind.CloseParenToken); break;
                case '[': Advance(); Add(TokenKind.OpenBracketToken); break;
                case ']': Advance(); Add(TokenKind.CloseBracketToken); break;
                case '{': Advance(); Add(TokenKind.OpenBraceToken); break;
                case '}': Advance(); Add(TokenKind.CloseBraceToken); break;
                case ';': Advance(); Add(TokenKind.SemiToken); break;
                case ',': Advance(); Add(TokenKind.CommaToken); break;
                case '.': Advance(); Add(TokenKind.DotToken); break;
                case '~': Advance(); Add(TokenKind.TildeToken); break;
                case '?': Advance(); Add(TokenKind.QuestionToken); break;

                case '<' when LookAhead('='): Advance(2); Add(TokenKind.LessThanEqualsToken); break;
                case '<' when LookAhead('<') && LookAhead('=', 2): Advance(3); Add(TokenKind.LessThanLessThanEqualsToken); break;
                case '<' when LookAhead('<'): Advance(2); Add(TokenKind.LessThanLessThanToken); break;
                case '<': Advance(); Add(TokenKind.LessThanToken); break;

                case '>' when LookAhead('='): Advance(2); Add(TokenKind.GreaterThanEqualsToken); break;
                case '>' when LookAhead('>') && LookAhead('=', 2): Advance(3); Add(TokenKind.GreaterThanGreaterThanEqualsToken); break;
                case '>' when LookAhead('>'): Advance(2); Add(TokenKind.GreaterThanGreaterThanToken); break;
                case '>': Advance(); Add(TokenKind.GreaterThanToken); break;

                case '+' when LookAhead('+'): Advance(2); Add(TokenKind.PlusPlusToken); break;
                case '+' when LookAhead('='): Advance(2); Add(TokenKind.PlusEqualsToken); break;
                case '+': Advance(); Add(TokenKind.PlusToken); break;

                case '-' when LookAhead('+'): Advance(2); Add(TokenKind.MinusMinusToken); break;
                case '-' when LookAhead('='): Advance(2); Add(TokenKind.MinusEqualsToken); break;
                case '-': Advance(); Add(TokenKind.MinusToken); break;

                case '*' when LookAhead('='): Advance(2); Add(TokenKind.AsteriskEqualsToken); break;
                case '*': Advance(); Add(TokenKind.AsteriskToken); break;

                case '/' when LookAhead('='): Advance(2); Add(TokenKind.SlashEqualsToken); break;
                case '/': Advance(); Add(TokenKind.SlashToken); break;

                case '%' when LookAhead('='): Advance(2); Add(TokenKind.PercentEqualsToken); break;
                case '%': Advance(); Add(TokenKind.PercentToken); break;

                case '&' when LookAhead('&'): Advance(2); Add(TokenKind.AmpersandAmpersandToken); break;
                case '&' when LookAhead('='): Advance(2); Add(TokenKind.AmpersandEqualsToken); break;
                case '&': Advance(); Add(TokenKind.AmpersandToken); break;

                case '|' when LookAhead('|'): Advance(2); Add(TokenKind.BarBarToken); break;
                case '|' when LookAhead('='): Advance(2); Add(TokenKind.BarEqualsToken); break;
                case '|': Advance(); Add(TokenKind.BarToken); break;

                case '^' when LookAhead('='): Advance(2); Add(TokenKind.CaretEqualsToken); break;
                case '^': Advance(); Add(TokenKind.CaretToken);  break;

                case ':' when LookAhead(':'): Advance(2); Add(TokenKind.ColonColonToken); break;
                case ':': Advance(); Add(TokenKind.ColonToken); break;

                case '=' when LookAhead('='): Advance(2); Add(TokenKind.EqualsEqualsToken); break;
                case '=': Advance(); Add(TokenKind.EqualsToken); break;

                case '!' when LookAhead('='): Advance(2); Add(TokenKind.ExclamationEqualsToken); break;
                case '!': Advance(); Add(TokenKind.NotToken); break;

                case char c:
                    Advance();
                    diagnostics.Add($"Error at line {line} column {column}: Unexpected token '{c}'.");
                    break;
            }
        }
    }

    private string SkipProgramBody(string expectedEnd)
    {
        StringBuilder builder = new();
        while (true)
        {
            // If there is still space for the terminator
            if (!IsAtEnd(expectedEnd.Length))
            {
                // And we have reached the terminator, stop
                if (source.Substring(position, expectedEnd.Length) == expectedEnd)
                {
                    Advance(expectedEnd.Length);
                    break;
                }

                // Otherwise advance
                builder.Append(Advance());
            }
            // No space for terminator, error
            else
            {
                diagnostics.Add($"Error at line {line} column {column}: Unterminated program block.");
                break;
            }
        }

        return builder.ToString();
    }

    private void LexIdentifier()
    {
        StringBuilder builder = new();
        while (SyntaxFacts.IsAlphaNumericOrUnderscore(Peek()))
        {
            builder.Append(Advance());
        }
        string identifier = builder.ToString();
        if (SyntaxFacts.TryParseShaderLabKeyword(identifier, out TokenKind token))
        {
            if (token == TokenKind.CgProgramKeyword)
            {
                string body = SkipProgramBody("ENDCG");
                Add(body, TokenKind.ProgramBlock);
            }
            else if (token == TokenKind.CgIncludeKeyword)
            {
                string body = SkipProgramBody("ENDCG");
                Add(body, TokenKind.IncludeBlock);
            }
            else if (token == TokenKind.HlslProgramKeyword)
            {
                string body = SkipProgramBody("ENDHLSL");
                Add(body, TokenKind.ProgramBlock);
            }
            else if (token == TokenKind.HlslIncludeKeyword)
            {
                string body = SkipProgramBody("ENDHLSL");
                Add(body, TokenKind.IncludeBlock);
            }
            else
            {
                Add(token);
            }
        }
        else
        {
            Add(identifier, TokenKind.IdentifierToken);
        }
    }

    private void LexNumber()
    {
        StringBuilder builder = new();
        while (true)
        {
            char c = Peek();
            if (char.IsDigit(c) || c == '.' || c == 'f' || c == 'F')
            {
                builder.Append(Advance());
            }
            else
            {
                break;
            }
        }
        string number = builder.ToString();
        TokenKind kind = TokenKind.IntegerLiteralToken;
        if (number.Contains('.') || number.Contains('f') || number.Contains('F'))
        {
            kind = TokenKind.FloatLiteralToken;
        }
        Add(number, kind);
    }

    private void LexString(char start, char end, TokenKind kind)
    {
        StringBuilder builder = new();
        Eat(start);
        while (Peek() != end)
        {
            builder.Append(Advance());
        }
        Eat(end);
        Add(builder.ToString(), TokenKind.StringLiteralToken);
    }
}

public abstract class ShaderLabSyntaxNode
{
    // TODO: Feed in span data
    public SourceSpan Span { get; set; } = new();
}

public class ShaderNode : ShaderLabSyntaxNode
{
    public string Name { get; set; } = string.Empty;
    public List<PropertyNode> Properties { get; set; } = new();

    public List<SubShaderNode> SubShaders { get; set; } = new();
    public string? Fallback { get; set; }
    public string? CustomEditor { get; set; }
    public List<string> IncludeBlocks { get; set; } = new();
}

public class PropertyNode : ShaderLabSyntaxNode
{
    public List<string> Attributes { get; set; } = new();
    public string Uniform { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PropertyKind Kind = PropertyKind.None;
    public (float Min, float Max)? RangeMinMax { get; set; }
    public PropertyValueNode Value { get; set; } = new();
}

public class PropertyValueNode : ShaderLabSyntaxNode
{
}

public class PropertyValueFloatNode : PropertyValueNode
{
    public float Number { get; set; } = 0;
}

public class PropertyValueIntegerNode : PropertyValueNode
{
    public int Number { get; set; } = 0;
}

public class PropertyValueVectorNode : PropertyValueNode
{
    public bool HasWChannel { get; set; } = false;
    public (float x, float y, float z, float w) Vector { get; set; } = default;
}

public class PropertyValueColorNode : PropertyValueNode
{
    public bool HasAlphaChannel { get; set; } = false;
    public (float x, float y, float z, float w) Color { get; set; } = default;
}

public class PropertyValueTextureNode : PropertyValueNode
{
    public string TextureName { get; set; } = string.Empty;
}

public enum PropertyKind
{
    None,
    Texture2D,
    Texture3D,
    TextureCube,
    Float,
    Int,
    Integer,
    Color,
    Vector,
    Range,
}

public class SubShaderNode : ShaderLabSyntaxNode
{
    public List<ShaderPassNode> Passes { get; set; } = new();
    public List<ShaderLabCommandNode> Commands { get; set; } = new();
    public List<string> IncludeBlocks { get; set; } = new();
}

public class ShaderPassNode : ShaderLabSyntaxNode
{
}

public class ShaderCodePassNode : ShaderPassNode
{
    public string? ProgramBlock { get; set; }
    public List<ShaderLabCommandNode> Commands { get; set; } = new();
    public List<string> IncludeBlocks { get; set; } = new();
}

public class ShaderGrabPassNode : ShaderPassNode
{
    public string? TextureName { get; set; } = string.Empty;
    public List<ShaderLabCommandNode> Commands { get; set; } = new();
    public List<string> IncludeBlocks { get; set; } = new();

    public bool IsUnnamed => string.IsNullOrEmpty(TextureName);
}

public class ShaderUsePassNode : ShaderPassNode
{
    public string? PassName { get; set; } = string.Empty;
}

public class ShaderLabCommandNode : ShaderLabSyntaxNode
{
}

public class ShaderLabCommandTagsNode : ShaderLabCommandNode
{
    public Dictionary<string, string> Tags { get; set; } = new();
}

public class ShaderLabCommandLodNode : ShaderLabCommandNode
{
    public int LodLevel { get; set; } = 0;
}

public class ShaderLabParser
{
    private List<Token> tokens = new();
    private int position = 0;
    private SourceSpan anchorSpan = default;

    private List<string> diagnostics = new();

    private ShaderLabParser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public Token Peek() => IsAtEnd() ? default : tokens[position];
    public Token LookAhead(int offset = 1) => IsAtEnd(offset) ? default : tokens[position + offset];
    public bool Match(Func<Token, bool> predicate) => predicate(Peek());
    public bool MatchKind(TokenKind kind) => Match(tok => tok.Kind == kind);
    public bool IsAtEnd(int offset = 0) => position + offset >= tokens.Count;
    public Token Eat(Func<Token, bool> predicate)
    {
        if (!Match(predicate))
            Error($"Unexpected token '{Peek()}'.");
        return Advance();
    }
    public Token EatKind(TokenKind kind)
    {
        if (!MatchKind(kind))
            Error($"Expected token type '{kind}', got '{Peek().Kind}'.");
        return Advance();
    }
    public Token Advance(int amount = 1)
    {
        if (IsAtEnd(amount - 1))
            return default;
        Token result = tokens[position];
        position += amount;
        anchorSpan = Peek().Span;
        return result;
    }
    public void Error(string msg)
    {
        diagnostics.Add($"Error at line {anchorSpan.Start.Line} column {anchorSpan.Start.Column}: {msg}");
    }

    public void Error(string msg, SourceSpan span)
    {
        anchorSpan = span;
        Error(msg);
    }

    public void Error(string expected, Token token)
    {
        Error($"Expected {expected}, got token ({token})", token.Span);
    }

    public static void Parse(List<Token> tokens, out ShaderNode rootNode, out List<string> diagnostics)
    {
        ShaderLabParser parser = new(tokens);

        rootNode = parser.ParseShader();
        diagnostics = parser.diagnostics;
    }

    public ShaderNode ParseShader()
    {
        EatKind(TokenKind.ShaderKeyword);
        string name = EatKind(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
        EatKind(TokenKind.OpenBraceToken);

        List<string> includeBlocks = new List<string>();

        ParseIncludeBlockIfPresent(includeBlocks);

        List<PropertyNode> properties = new();
        if (Peek().Kind == TokenKind.PropertiesKeyword)
        {
            ParsePropertySection(properties);
        }

        List<SubShaderNode> subshaders = new();
        string? fallback = null;
        string? customEditor = null;

        while (!IsAtEnd())
        {
            ParseIncludeBlockIfPresent(includeBlocks);

            int lastPosition = position;

            Token next = Peek();
            if (next.Kind == TokenKind.CloseBraceToken)
                break;

            switch (next.Kind)
            {
                case TokenKind.SubShaderKeyword:
                    subshaders.Add(ParseSubShader());
                    break;
                case TokenKind.FallbackKeyword:
                    Advance();
                    fallback = EatKind(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
                    break;
                case TokenKind.CustomEditorKeyword:
                    Advance();
                    customEditor = EatKind(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
                    break;
                default:
                    Advance();
                    Error($"SubShader, Fallback or CustomEditor", next);
                    break;
            }

            if (position == lastPosition)
            {
                Error($"Parser got stuck on token type '{next.Kind}'. Please file a bug report.");
                break;
            }
        }

        ParseIncludeBlockIfPresent(includeBlocks);

        EatKind(TokenKind.CloseBraceToken);

        return new ShaderNode
        {
            Name = name,
            Properties = properties,
            SubShaders = subshaders,
            Fallback = fallback,
            CustomEditor = customEditor,
            IncludeBlocks = includeBlocks,
        };
    }

    public void ParseIncludeBlockIfPresent(List<string> outIncludeBlocks)
    {
        Token next = Peek();
        if (next.Kind == TokenKind.IncludeBlock && !string.IsNullOrEmpty(next.Identifier)) 
        {
            outIncludeBlocks.Add(next.Identifier);
        }
    }

    public void ParsePropertySection(List<PropertyNode> outProperties)
    {
        EatKind(TokenKind.PropertiesKeyword);
        EatKind(TokenKind.OpenBraceToken);

        while (Peek().Kind == TokenKind.IdentifierToken)
        {
            outProperties.Add(ParseProperty());
        }

        EatKind(TokenKind.CloseBraceToken);
    }

    public string ParseIdentifier()
    {
        Token identifierToken = EatKind(TokenKind.IdentifierToken);
        string identifier = identifierToken.Identifier ?? string.Empty;
        if (string.IsNullOrEmpty(identifier))
            Error("a valid identifier", identifierToken);
        return identifier;
    }

    public string ParseStringLiteral()
    {
        Token literalToken = EatKind(TokenKind.StringLiteralToken);
        string literal = literalToken.Identifier ?? string.Empty;
        if (string.IsNullOrEmpty(literal))
            Error("a valid string literal", literalToken);
        return literal;
    }

    public float ParseNumericLiteral()
    {
        Token literalToken = Eat(tok => tok.Kind == TokenKind.FloatLiteralToken || tok.Kind == TokenKind.IntegerLiteralToken);
        string literal = literalToken.Identifier ?? string.Empty;
        if (string.IsNullOrEmpty(literal))
            Error("a valid numeric literal", literalToken);
        return float.Parse(literal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
    }

    public PropertyNode ParseProperty()
    {
        List<string> attributes = new();
        while (Peek().Kind == TokenKind.OpenBracketToken)
        {
            EatKind(TokenKind.OpenBracketToken);

            attributes.Add(ParseIdentifier());

            EatKind(TokenKind.CloseBracketToken);
        }

        string uniform = ParseIdentifier();

        EatKind(TokenKind.OpenParenToken);

        string name = ParseStringLiteral();
        EatKind(TokenKind.CommaToken);

        PropertyKind kind = PropertyKind.None;
        (float Min, float Max)? rangeMinMax = null;
        Token typeToken = Advance();
        switch (typeToken.Kind)
        {
            case TokenKind.FloatKeyword: kind = PropertyKind.Float; break;
            case TokenKind.IntegerKeyword: kind = PropertyKind.Integer; break;
            case TokenKind.IntKeyword: kind = PropertyKind.Int; break;
            case TokenKind.ColorKeyword: kind = PropertyKind.Color; break;
            case TokenKind.VectorKeyword: kind = PropertyKind.Vector; break;
            case TokenKind._2DKeyword: kind = PropertyKind.Texture2D; break;
            case TokenKind._3DKeyword: kind = PropertyKind.Texture3D; break;
            case TokenKind.CubeKeyword: kind = PropertyKind.TextureCube; break;
            case TokenKind.RangeKeyword:
                EatKind(TokenKind.OpenParenToken);
                float min = ParseNumericLiteral();
                EatKind(TokenKind.CommaToken);
                float max = ParseNumericLiteral();
                EatKind(TokenKind.CloseParenToken);
                rangeMinMax = (min, max);
                break;
            default:
                Error("a valid type", typeToken);
                break;
        }

        EatKind(TokenKind.CloseParenToken);

        EatKind(TokenKind.EqualsToken);

        PropertyValueNode valueNode = new();
        switch (kind)
        {
            case PropertyKind.Color:
            case PropertyKind.Vector:
                EatKind(TokenKind.OpenParenToken);
                float x = ParseNumericLiteral();
                EatKind(TokenKind.CommaToken);
                float y = ParseNumericLiteral();
                EatKind(TokenKind.CommaToken);
                float z = ParseNumericLiteral();
                float w = 0;
                bool hasLastChannel = false;
                if (Peek().Kind == TokenKind.CommaToken)
                {
                    EatKind(TokenKind.CommaToken);
                    w = ParseNumericLiteral();
                    hasLastChannel = true;
                }
                EatKind(TokenKind.CloseParenToken);
                if (kind == PropertyKind.Color)
                    valueNode = new PropertyValueColorNode { HasAlphaChannel = hasLastChannel, Color = (x, y, z, w) };
                else
                    valueNode = new PropertyValueVectorNode { HasWChannel = hasLastChannel, Vector = (x, y, z, w) };
                break;

            case PropertyKind.TextureCube:
            case PropertyKind.Texture2D:
            case PropertyKind.Texture3D:
                valueNode = new PropertyValueTextureNode { TextureName = ParseStringLiteral() };
                break;

            case PropertyKind.Integer:
            case PropertyKind.Int:
                valueNode = new PropertyValueIntegerNode { Number = (int)ParseNumericLiteral() };
                break;

            case PropertyKind.Float:
            case PropertyKind.Range:
                valueNode = new PropertyValueFloatNode { Number = ParseNumericLiteral() };
                break;

            default:
                break;
        }

        if (Peek().Kind == TokenKind.OpenBraceToken)
        {
            EatKind(TokenKind.OpenBraceToken);
            while (Peek().Kind != TokenKind.CloseBraceToken)
                Advance();
            EatKind(TokenKind.CloseBraceToken);
        }

        return new PropertyNode
        {
            Attributes = attributes,
            Uniform = uniform,
            Name = name,
            Kind = kind,
            RangeMinMax = rangeMinMax,
            Value = valueNode,
        };
    }

    public SubShaderNode ParseSubShader()
    {
        EatKind(TokenKind.SubShaderKeyword);
        EatKind(TokenKind.OpenBraceToken);

        List<ShaderPassNode> passes = new();
        List<ShaderLabCommandNode> commands = new();
        List<string> includeBlocks = new();

        while (!IsAtEnd())
        {
            ParseIncludeBlockIfPresent(includeBlocks);

            int lastPosition = position;

            Token next = Peek();
            if (next.Kind == TokenKind.CloseBraceToken)
                break;

            switch (next.Kind)
            {
                case TokenKind.PassKeyword: passes.Add(ParseCodePass()); break;
                case TokenKind.GrabPassKeyword: passes.Add(ParseGrabPass()); break;
                case TokenKind.UsePassKeyword: passes.Add(ParseUsePass()); break;
                default:
                    commands.AddRange(ParseCommands());
                    break;
            }

            if (position == lastPosition)
            {
                Error($"Parser got stuck on token type '{next.Kind}'. Please file a bug report.");
                break;
            }
        }

        ParseIncludeBlockIfPresent(includeBlocks);

        EatKind(TokenKind.CloseBraceToken);

        return new SubShaderNode
        {
            Passes = passes,
            Commands = commands,
            IncludeBlocks = includeBlocks
        };
    }

    public ShaderCodePassNode ParseCodePass()
    {
        EatKind(TokenKind.PassKeyword);
        EatKind(TokenKind.OpenBraceToken);

        string? program = null;
        List<ShaderLabCommandNode> commands = new();
        List<string> includeBlocks = new();

        // TODO: Parse commands

        if (Peek().Kind == TokenKind.ProgramBlock)
        {
            program = EatKind(TokenKind.ProgramBlock).Identifier;
        }

        // TODO: Parse comamnds

        EatKind(TokenKind.CloseBraceToken);

        return new ShaderCodePassNode
        {
            ProgramBlock = program,
            Commands = commands,
            IncludeBlocks = includeBlocks
        };
    }

    public ShaderGrabPassNode ParseGrabPass()
    {
        EatKind(TokenKind.GrabPassKeyword);
        EatKind(TokenKind.OpenBraceToken);

        List<ShaderLabCommandNode> commands = new();
        List<string> includeBlocks = new();

        ParseIncludeBlockIfPresent(includeBlocks);
        // TODO: Parse commands

        string? name = null;
        if (Peek().Kind != TokenKind.CloseParenToken)
        {
            name = ParseStringLiteral();

            ParseIncludeBlockIfPresent(includeBlocks);
        }

        // TODO: Parse commands
        EatKind(TokenKind.CloseBraceToken);

        return new ShaderGrabPassNode
        {
            TextureName = name,
            Commands = commands,
            IncludeBlocks = includeBlocks
        };
    }

    public ShaderUsePassNode ParseUsePass()
    {
        EatKind(TokenKind.UsePassKeyword);
        return new ShaderUsePassNode
        {
            PassName = ParseStringLiteral()
        };
    }

    public List<ShaderLabCommandNode> ParseCommands()
    {
        List<ShaderLabCommandNode> commands = new();
        bool run = true;
        while (run)
        {
            switch (Peek().Kind)
            {
                case TokenKind.TagsKeyword: commands.Add(ParseTagsCommand()); break;
                case TokenKind.LodKeyword: commands.Add(ParseLodCommand()); break;

                default:
                    run = false;
                    break;
            }
        }
        return commands;
    }

    public ShaderLabCommandTagsNode ParseTagsCommand()
    {
        EatKind(TokenKind.TagsKeyword);
        EatKind(TokenKind.OpenBraceToken);

        Dictionary<string, string> tags = new();
        while (Peek().Kind != TokenKind.CloseBraceToken)
        {
            string key = ParseStringLiteral();
            EatKind(TokenKind.EqualsToken);
            string val = ParseStringLiteral();

            tags.Add(key, val);
        }

        EatKind(TokenKind.CloseBraceToken);

        return new ShaderLabCommandTagsNode
        {
            Tags = tags
        };
    }

    public ShaderLabCommandLodNode ParseLodCommand()
    {
        EatKind(TokenKind.LodKeyword);
        int level = (int)ParseNumericLiteral();
        return new ShaderLabCommandLodNode
        {
            LodLevel = level,
        };
    }
}

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

        ;
    }
}
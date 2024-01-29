using System.Text;

public enum TokenKind
{
    None,

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
    _2DArrayKeyword,
    _3DArrayKeyword,
    CubeArrayKeyword,
    AnyKeyword,
    RectKeyword,
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
    GlslProgramKeyword,
    GlslIncludeKeyword,
    EndGlslKeyword,
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
    ZClipKeyword,
    ConservativeKeyword,
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
    FailBackKeyword,
    FailFrontKeyword,
    ZFailBackKeyword,
    ZFailFrontKeyword,
    PassFrontKeyword,
    PassBackKeyword,
    UsePassKeyword,
    GrabPassKeyword,
    DependencyKeyword,
    MaterialKeyword,
    DiffuseKeyword,
    AmbientKeyword,
    ShininessKeyword,
    SpecularKeyword,
    EmissionKeyword,
    AmbientAndDiffuseKeyword,
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

    TrueKeyword,
    FalseKeyword,
    OffKeyword,
    OnKeyword,
    FrontKeyword,
    BackKeyword,
    OneKeyword,
    ZeroKeyword,
    SrcColorKeyword,
    SrcAlphaKeyword,
    SrcAlphaSaturateKeyword,
    DstColorKeyword,
    DstAlphaKeyword,
    OneMinusSrcColorKeyword,
    OneMinusSrcAlphaKeyword,
    OneMinusDstColorKeyword,
    OneMinusDstAlphaKeyword,
    GlobalKeyword,
    AddKeyword,
    SubKeyword,
    RevSubKeyword,
    MinKeyword,
    MaxKeyword,
    LogicalClearKeyword,
    LogicalSetKeyword,
    LogicalCopyKeyword,
    LogicalCopyInvertedKeyword,
    LogicalNoopKeyword,
    LogicalInvertKeyword,
    LogicalAndKeyword,
    LogicalNandKeyword,
    LogicalOrKeyword,
    LogicalNorKeyword,
    LogicalXorKeyword,
    LogicalEquivKeyword,
    LogicalAndReverseKeyword,
    LogicalOrReverseKeyword,
    LogicalOrInvertedKeyword,
    MultiplyKeyword,
    ScreenKeyword,
    OverlayKeyword,
    DarkenKeyword,
    LightenKeyword,
    ColorDodgeKeyword,
    ColorBurnKeyword,
    HardLightKeyword,
    SoftLightKeyword,
    DifferenceKeyword,
    ExclusionKeyword,
    HSLHueKeyword,
    HSLSaturationKeyword,
    HSLColorKeyword,
    HSLLuminosityKeyword,

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
            case "2darray": token = TokenKind._2DArrayKeyword; return true;
            case "3darray": token = TokenKind._3DArrayKeyword; return true;
            case "cubearray": token = TokenKind.CubeArrayKeyword; return true;
            case "any": token = TokenKind.AnyKeyword; return true;
            case "rect": token = TokenKind.RectKeyword; return true;
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
            case "glslprogram": token = TokenKind.GlslProgramKeyword; return true;
            case "glslinclude": token = TokenKind.GlslIncludeKeyword; return true;
            case "endglsl": token = TokenKind.EndGlslKeyword; return true;
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
            case "zclip": token = TokenKind.ZClipKeyword; return true;
            case "conservative": token = TokenKind.ConservativeKeyword; return true;
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
            case "failback": token = TokenKind.FailBackKeyword; return true;
            case "failfront": token = TokenKind.FailFrontKeyword; return true;
            case "zfailback": token = TokenKind.ZFailBackKeyword; return true;
            case "zfailfront": token = TokenKind.ZFailFrontKeyword; return true;
            case "passfront": token = TokenKind.PassFrontKeyword; return true;
            case "passback": token = TokenKind.PassBackKeyword; return true;
            case "usepass": token = TokenKind.UsePassKeyword; return true;
            case "grabpass": token = TokenKind.GrabPassKeyword; return true;
            case "dependency": token = TokenKind.DependencyKeyword; return true;
            case "material": token = TokenKind.MaterialKeyword; return true;
            case "diffuse": token = TokenKind.DiffuseKeyword; return true;
            case "ambient": token = TokenKind.AmbientKeyword; return true;
            case "shininess": token = TokenKind.ShininessKeyword; return true;
            case "specular": token = TokenKind.SpecularKeyword; return true;
            case "emission": token = TokenKind.EmissionKeyword; return true;
            case "ambientanddiffuse": token = TokenKind.AmbientAndDiffuseKeyword; return true;
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
            case "off": token = TokenKind.OffKeyword; return true;
            case "on": token = TokenKind.OnKeyword; return true;
            case "front": token = TokenKind.FrontKeyword; return true;
            case "back": token = TokenKind.BackKeyword; return true;
            case "one": token = TokenKind.OneKeyword; return true;
            case "zero": token = TokenKind.ZeroKeyword; return true;
            case "srccolor": token = TokenKind.SrcColorKeyword; return true;
            case "srcalpha": token = TokenKind.SrcAlphaKeyword; return true;
            case "srcalphasaturate": token = TokenKind.SrcAlphaSaturateKeyword; return true;
            case "dstcolor": token = TokenKind.DstColorKeyword; return true;
            case "dstalpha": token = TokenKind.DstAlphaKeyword; return true;
            case "oneminussrccolor": token = TokenKind.OneMinusSrcColorKeyword; return true;
            case "oneminussrcalpha": token = TokenKind.OneMinusSrcAlphaKeyword; return true;
            case "oneminusdstcolor": token = TokenKind.OneMinusDstColorKeyword; return true;
            case "oneminusdstalpha": token = TokenKind.OneMinusDstAlphaKeyword; return true;
            case "global": token = TokenKind.GlobalKeyword; return true;
            case "add": token = TokenKind.AddKeyword; return true;
            case "sub": token = TokenKind.SubKeyword; return true;
            case "revsub": token = TokenKind.RevSubKeyword; return true;
            case "min": token = TokenKind.MinKeyword; return true;
            case "max": token = TokenKind.MaxKeyword; return true;
            case "logicalclear": token = TokenKind.LogicalClearKeyword; return true;
            case "logicalset": token = TokenKind.LogicalSetKeyword; return true;
            case "logicalcopy": token = TokenKind.LogicalCopyKeyword; return true;
            case "logicalcopyinverted": token = TokenKind.LogicalCopyInvertedKeyword; return true;
            case "logicalnoop": token = TokenKind.LogicalNoopKeyword; return true;
            case "logicalinvert": token = TokenKind.LogicalInvertKeyword; return true;
            case "logicaland": token = TokenKind.LogicalAndKeyword; return true;
            case "logicalnand": token = TokenKind.LogicalNandKeyword; return true;
            case "logicalor": token = TokenKind.LogicalOrKeyword; return true;
            case "logicalnor": token = TokenKind.LogicalNorKeyword; return true;
            case "logicalxor": token = TokenKind.LogicalXorKeyword; return true;
            case "logicalequiv": token = TokenKind.LogicalEquivKeyword; return true;
            case "logicalandreverse": token = TokenKind.LogicalAndReverseKeyword; return true;
            case "logicalorreverse": token = TokenKind.LogicalOrReverseKeyword; return true;
            case "logicalorinverted": token = TokenKind.LogicalOrInvertedKeyword; return true;
            case "multiply": token = TokenKind.MultiplyKeyword; return true;
            case "screen": token = TokenKind.ScreenKeyword; return true;
            case "overlay": token = TokenKind.OverlayKeyword; return true;
            case "darken": token = TokenKind.DarkenKeyword; return true;
            case "lighten": token = TokenKind.LightenKeyword; return true;
            case "colordodge": token = TokenKind.ColorDodgeKeyword; return true;
            case "colorburn": token = TokenKind.ColorBurnKeyword; return true;
            case "hardlight": token = TokenKind.HardLightKeyword; return true;
            case "softlight": token = TokenKind.SoftLightKeyword; return true;
            case "difference": token = TokenKind.DifferenceKeyword; return true;
            case "exclusion": token = TokenKind.ExclusionKeyword; return true;
            case "hslhue": token = TokenKind.HSLHueKeyword; return true;
            case "hslsaturation": token = TokenKind.HSLSaturationKeyword; return true;
            case "hslcolor": token = TokenKind.HSLColorKeyword; return true;
            case "hslluminosity": token = TokenKind.HSLLuminosityKeyword; return true;
            default: return false;
        }
    }

    public static bool TryParseBindChannelName(string name, out BindChannel bindChannel)
    {
        bindChannel = default;

        switch (name.ToLower())
        {
            case "vertex": bindChannel = BindChannel.Vertex; return true;
            case "normal": bindChannel = BindChannel.Normal; return true;
            case "tangent": bindChannel = BindChannel.Tangent; return true;
            case "texcoord0" or "texcoord": bindChannel = BindChannel.TexCoord0; return true;
            case "texcoord1": bindChannel = BindChannel.TexCoord1; return true;
            case "texcoord2": bindChannel = BindChannel.TexCoord2; return true;
            case "texcoord3": bindChannel = BindChannel.TexCoord3; return true;
            case "texcoord4": bindChannel = BindChannel.TexCoord4; return true;
            case "texcoord5": bindChannel = BindChannel.TexCoord5; return true;
            case "texcoord6": bindChannel = BindChannel.TexCoord6; return true;
            case "texcoord7": bindChannel = BindChannel.TexCoord7; return true;
            case "color": bindChannel = BindChannel.Color; return true;
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

                case '2' when LookAhead('D') || LookAhead('d'):
                case '3' when LookAhead('D') || LookAhead('d'):
                    LexDimensionalTextureType();
                    break;

                case char c when char.IsDigit(c) || c == '.' || c == '-':
                    LexNumber();
                    break;

                case '"':
                    LexString('"', '"', TokenKind.StringLiteralToken);
                    break;

                case '[' when SyntaxFacts.IsAlphaNumericOrUnderscore(LookAhead()):
                    LexString('[', ']', TokenKind.BracketedStringLiteralToken);
                    break;

                case ' ' or '\t' or '\r' or '\n':
                    Advance();
                    break;

                case '/' when LookAhead('/'):
                    Advance(2);
                    while (!Match('\n'))
                    {
                        Advance();
                    }
                    Advance();
                    break;

                case '/' when LookAhead('*'):
                    Advance(2);
                    while (!(Match('*') && LookAhead('/')))
                    {
                        Advance();
                    }
                    Advance(2);
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

    private void LexDimensionalTextureType()
    {
        StringBuilder builder = new();
        builder.Append(Advance());
        while (char.IsLetter(Peek()))
        {
            builder.Append(Advance());
        }

        switch (builder.ToString().ToLower())
        {
            case "2darray": Add(TokenKind._2DArrayKeyword); break;
            case "3darray": Add(TokenKind._3DArrayKeyword); break;
            case "2d": Add(TokenKind._2DKeyword); break;
            case "3d": Add(TokenKind._3DKeyword); break;
        }
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
            else if (token == TokenKind.GlslProgramKeyword)
            {
                string body = SkipProgramBody("ENDGLSL");
                Add(body, TokenKind.ProgramBlock);
            }
            else if (token == TokenKind.GlslIncludeKeyword)
            {
                string body = SkipProgramBody("ENDGLSL");
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
        if (Match('-'))
        {
            builder.Append(Advance());
        }
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
        Add(builder.ToString(), kind);
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
    public List<ShaderPropertyNode> Properties { get; set; } = new();
    public List<SubShaderNode> SubShaders { get; set; } = new();
    public string? Fallback { get; set; }
    public bool FallbackDisabledExplicitly { get; set; }
    public string? CustomEditor { get; set; }
    public Dictionary<string, string> Dependencies { get; set; } = new();
    public List<string> IncludeBlocks { get; set; } = new();
}

public class ShaderPropertyNode : ShaderLabSyntaxNode
{
    public List<string> Attributes { get; set; } = new();
    public string Uniform { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ShaderPropertyKind Kind = ShaderPropertyKind.None;
    public (float Min, float Max)? RangeMinMax { get; set; }
    public ShaderPropertyValueNode Value { get; set; } = new();
}

public class ShaderPropertyValueNode : ShaderLabSyntaxNode
{
}

public class ShaderPropertyValueFloatNode : ShaderPropertyValueNode
{
    public float Number { get; set; } = 0;
}

public class ShaderPropertyValueIntegerNode : ShaderPropertyValueNode
{
    public int Number { get; set; } = 0;
}

public class ShaderPropertyValueVectorNode : ShaderPropertyValueNode
{
    public bool HasWChannel { get; set; } = false;
    public (float x, float y, float z, float w) Vector { get; set; } = default;
}

public class ShaderPropertyValueColorNode : ShaderPropertyValueNode
{
    public bool HasAlphaChannel { get; set; } = false;
    public (float x, float y, float z, float w) Color { get; set; } = default;
}

public class ShaderPropertyValueTextureNode : ShaderPropertyValueNode
{
    public string TextureName { get; set; } = string.Empty;
}

public enum ShaderPropertyKind
{
    None,
    Texture2D,
    Texture3D,
    TextureCube,
    TextureAny,
    Texture2DArray,
    Texture3DArray,
    TextureCubeArray,
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
    public List<string> ProgramBlocks { get; set; } = new();
    public List<string> IncludeBlocks { get; set; } = new();
    public string? ProgramBlock => ProgramBlocks.Count > 0 ? ProgramBlocks[0] : null;
}

public class ShaderPassNode : ShaderLabSyntaxNode
{
}

public class ShaderCodePassNode : ShaderPassNode
{
    public List<ShaderLabCommandNode> Commands { get; set; } = new();
    public List<string> ProgramBlocks { get; set; } = new();
    public List<string> IncludeBlocks { get; set; } = new();
    public string? ProgramBlock => ProgramBlocks.Count > 0 ? ProgramBlocks[0] : null;
}

public class ShaderGrabPassNode : ShaderPassNode
{
    public string? TextureName { get; set; } = string.Empty;
    public List<ShaderLabCommandNode> Commands { get; set; } = new();
    public List<string> ProgramBlocks { get; set; } = new();
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

// Either a reference to a property or some other type
public struct PropertyReferenceOr<TOther>
{
    public TOther? Value;
    public string? Property;

    public bool IsValue => Value != null;
    public bool IsPropertyReference => Property != null;
    public bool IsValid => IsValue || IsPropertyReference;

    public override string ToString()
    {
        if (Value != null) return Value.ToString() ?? string.Empty;
        else if (Property != null) return Property;
        else return string.Empty;
    }
}

public class ShaderLabBasicToggleCommandNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<bool> Enabled { get; set; }
}

public class ShaderLabCommandLightingNode : ShaderLabBasicToggleCommandNode { }
public class ShaderLabCommandSeparateSpecularNode : ShaderLabBasicToggleCommandNode { }
public class ShaderLabCommandZWriteNode : ShaderLabBasicToggleCommandNode { }
public class ShaderLabCommandAlphaToMaskNode : ShaderLabBasicToggleCommandNode { }
public class ShaderLabCommandZClipNode : ShaderLabBasicToggleCommandNode { }
public class ShaderLabCommandConservativeNode : ShaderLabBasicToggleCommandNode { }

public enum CullMode
{
    Off,
    Front,
    Back,
}

public class ShaderLabCommandCullNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<CullMode> Mode { get; set; }
}

public enum ComparisonMode
{
    Off,
    Never,
    Less,
    Equal,
    LEqual,
    Greater,
    NotEqual,
    GEqual,
    Always,
}

public class ShaderLabCommandZTestNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<ComparisonMode> Mode { get; set; }
}

public enum BlendFactor
{
    One,
    Zero,
    SrcColor,
    SrcAlpha,
    SrcAlphaSaturate,
    DstColor,
    DstAlpha,
    OneMinusSrcColor,
    OneMinusSrcAlpha,
    OneMinusDstColor,
    OneMinusDstAlpha,
}

public class ShaderLabCommandBlendNode : ShaderLabCommandNode
{
    public int RenderTarget { get; set; } = 0;
    public bool Enabled { get; set; } = false;
    public PropertyReferenceOr<BlendFactor>? SourceFactorRGB { get; set; } = null;
    public PropertyReferenceOr<BlendFactor>? DestinationFactorRGB { get; set; } = null;
    public PropertyReferenceOr<BlendFactor>? SourceFactorAlpha { get; set; } = null;
    public PropertyReferenceOr<BlendFactor>? DestinationFactorAlpha { get; set; } = null;
}

public class ShaderLabCommandOffsetNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<float> Factor { get; set; }
    public PropertyReferenceOr<float> Units { get; set; }
}

public class ShaderLabCommandColorMaskNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<string> Mask { get; set; }
    public int RenderTarget { get; set; } = 0;

    public bool IsZeroMask => Mask.Value == "0";
}

public class ShaderLabCommandAlphaTestNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<ComparisonMode> Mode { get; set; }
    public PropertyReferenceOr<float>? AlphaValue { get; set; }
}

public class ShaderLabCommandFogNode : ShaderLabCommandNode
{
    public bool Enabled { get; set; } = false;
    public (float r, float g, float b, float a)? Color { get; set; }
}

public class ShaderLabCommandNameNode : ShaderLabCommandNode
{
    public string Name { get; set; } = string.Empty;
}

public enum BindChannel
{
    Vertex,
    Normal,
    Tangent,
    TexCoord0,
    TexCoord1,
    TexCoord2,
    TexCoord3,
    TexCoord4,
    TexCoord5,
    TexCoord6,
    TexCoord7,
    Color,
}

public class ShaderLabCommandBindChannelsNode : ShaderLabCommandNode
{
    public Dictionary<BindChannel, BindChannel> Bindings { get; set; } = new();
}

public class ShaderLabCommandColorNode : ShaderLabCommandNode
{
    public bool HasAlphaChannel { get; set; } = false;
    public PropertyReferenceOr<(float r, float g, float b, float a)> Color { get; set; }
}

public enum BlendOp
{
    Add,
    Sub,
    RevSub,
    Min,
    Max,
    LogicalClear,
    LogicalSet,
    LogicalCopy,
    LogicalCopyInverted,
    LogicalNoop,
    LogicalInvert,
    LogicalAnd,
    LogicalNand,
    LogicalOr,
    LogicalNor,
    LogicalXor,
    LogicalEquiv,
    LogicalAndReverse,
    LogicalOrReverse,
    LogicalOrInverted,
    Multiply,
    Screen,
    Overlay,
    Darken,
    Lighten,
    ColorDodge,
    ColorBurn,
    HardLight,
    SoftLight,
    Difference,
    Exclusion,
    HSLHue,
    HSLSaturation,
    HSLColor,
    HSLLuminosity,
}

public class ShaderLabCommandBlendOpNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<BlendOp> BlendOp { get; set; }
}

public enum FixedFunctionMaterialProperty
{
    Diffuse,
    Ambient,
    Shininess,
    Specular,
    Emission,
}

public class ShaderLabCommandMaterialNode : ShaderLabCommandNode
{
    public Dictionary<FixedFunctionMaterialProperty, PropertyReferenceOr<(float r, float g, float b, float a)>> Properties { get; set; } = new();
}

public class ShaderLabCommandSetTextureNode : ShaderLabCommandNode
{
    // TODO: Not the lazy way
    public string TextureName { get; set; } = string.Empty;
    public List<Token> Body { get; set; } = new();
}

public class ShaderLabCommandColorMaterialNode : ShaderLabCommandNode
{
    public bool AmbientAndDiffuse { get; set; } = false;
    public bool Emission => !AmbientAndDiffuse;
}

public enum StencilOp
{
    Keep,
    Zero,
    Replace,
    IncrSat,
    DecrSat,
    Invert,
    IncrWrap,
    DecrWrap,
}

public class ShaderLabCommandStencilNode : ShaderLabCommandNode
{
    public PropertyReferenceOr<byte> Ref { get; set; }
    public PropertyReferenceOr<byte> ReadMask { get; set; }
    public PropertyReferenceOr<byte> WriteMask { get; set; }
    public PropertyReferenceOr<ComparisonMode> ComparisonOperationBack { get; set; }
    public PropertyReferenceOr<StencilOp> PassOperationBack { get; set; }
    public PropertyReferenceOr<StencilOp> FailOperationBack { get; set; }
    public PropertyReferenceOr<StencilOp> ZFailOperationBack { get; set; }
    public PropertyReferenceOr<ComparisonMode> ComparisonOperationFront { get; set; }
    public PropertyReferenceOr<StencilOp> PassOperationFront { get; set; }
    public PropertyReferenceOr<StencilOp> FailOperationFront { get; set; }
    public PropertyReferenceOr<StencilOp> ZFailOperationFront { get; set; }
    public PropertyReferenceOr<ComparisonMode> ComparisonOperation => ComparisonOperationFront;
    public PropertyReferenceOr<StencilOp> PassOperation => PassOperationFront;
    public PropertyReferenceOr<StencilOp> FailOperation => FailOperationFront;
    public PropertyReferenceOr<StencilOp> ZFailOperation => ZFailOperationFront;

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
    public bool Match(TokenKind kind) => Match(tok => tok.Kind == kind);
    public bool Match(params TokenKind[] alternatives) => Match(tok => alternatives.Contains(tok.Kind));
    public bool IsAtEnd(int offset = 0) => position + offset >= tokens.Count;
    public Token Eat(Func<Token, bool> predicate)
    {
        if (!Match(predicate))
            Error($"Unexpected token '{Peek()}'.");
        return Advance();
    }
    public Token Eat(TokenKind kind)
    {
        if (!Match(kind))
            Error($"Expected token type '{kind}', got '{Peek().Kind}'.");
        return Advance();
    }
    public Token Eat(params TokenKind[] alternatives)
    {
        if (!Match(alternatives))
        {
            string allowed = string.Join(", ", alternatives);
            Error($"Unexpected token '{Peek()}', expected one of the following token types: {allowed}.");
        }
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
        Eat(TokenKind.ShaderKeyword);
        string name = Eat(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
        Eat(TokenKind.OpenBraceToken);

        List<string> includeBlocks = new List<string>();

        ParseIncludeBlocksIfPresent(includeBlocks);

        List<ShaderPropertyNode> properties = new();
        if (Match(TokenKind.PropertiesKeyword))
        {
            ParsePropertySection(properties);
        }

        List<SubShaderNode> subshaders = new();
        string? fallback = null;
        bool fallbackDisabledExplicitly = false;
        string? customEditor = null;
        Dictionary<string, string> dependencies = new();

        // Keep track of commands inherited by categories as we parse.
        // We essentially pretend categories don't exist, since they are a niche feature.
        Stack<List<ShaderLabCommandNode>> categoryCommands = new();

        while (!IsAtEnd())
        {
            ParseIncludeBlocksIfPresent(includeBlocks);

            // If we are in a category, put the commands there
            if (categoryCommands.Count > 0)
                ParseCommandsIfPresent(categoryCommands.Peek());

            int lastPosition = position;

            Token next = Peek();
            if (next.Kind == TokenKind.CloseBraceToken)
                break;

            switch (next.Kind)
            {
                case TokenKind.SubShaderKeyword:
                    var subShader = ParseSubShader();
                    subShader.Commands.AddRange(categoryCommands.SelectMany(x => x));
                    subshaders.Add(subShader);
                    break;
                case TokenKind.FallbackKeyword:
                    Advance();
                    if (Match(TokenKind.OffKeyword, TokenKind.FalseKeyword))
                    {
                        fallbackDisabledExplicitly = true;
                        Advance();
                    }
                    else
                    {
                        fallback = Eat(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
                    }
                    break;
                case TokenKind.DependencyKeyword:
                    Advance();
                    string key = ParseStringLiteral();
                    Eat(TokenKind.EqualsToken);
                    string val = ParseStringLiteral();
                    dependencies[key] = val;
                    break;
                case TokenKind.CustomEditorKeyword:
                    Advance();
                    customEditor = Eat(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
                    break;
                case TokenKind.CategoryKeyword:
                    Advance();
                    Eat(TokenKind.OpenBraceToken);
                    categoryCommands.Push(new());
                    break;
                case TokenKind.CloseBraceToken when categoryCommands.Count > 0:
                    Advance();
                    categoryCommands.Pop();
                    break;
                default:
                    Advance();
                    Error($"SubShader, Fallback, Dependency or CustomEditor", next);
                    break;
            }

            if (position == lastPosition)
            {
                // TODO: Get rid of these
                Error($"Parser got stuck on token type '{next.Kind}'. Please file a bug report.");
                break;
            }
        }

        ParseIncludeBlocksIfPresent(includeBlocks);

        Eat(TokenKind.CloseBraceToken);

        return new ShaderNode
        {
            Name = name,
            Properties = properties,
            SubShaders = subshaders,
            Fallback = fallback,
            FallbackDisabledExplicitly = fallbackDisabledExplicitly,
            CustomEditor = customEditor,
            Dependencies = dependencies,
            IncludeBlocks = includeBlocks,
        };
    }

    public void ParseIncludeBlocksIfPresent(List<string> outIncludeBlocks)
    {
        while (true)
        {
            Token next = Peek();
            if (next.Kind == TokenKind.IncludeBlock && !string.IsNullOrEmpty(next.Identifier))
            {
                outIncludeBlocks.Add(next.Identifier);
                Advance();
            }
            else
            {
                break;
            }
        }
    }

    public void ParsePropertySection(List<ShaderPropertyNode> outProperties)
    {
        Eat(TokenKind.PropertiesKeyword);
        Eat(TokenKind.OpenBraceToken);

        while (Match(TokenKind.IdentifierToken, TokenKind.BracketedStringLiteralToken))
        {
            outProperties.Add(ParseProperty());
        }

        Eat(TokenKind.CloseBraceToken);
    }

    public string ParseIdentifier()
    {
        Token identifierToken = Eat(TokenKind.IdentifierToken);
        string identifier = identifierToken.Identifier ?? string.Empty;
        if (string.IsNullOrEmpty(identifier))
            Error("a valid identifier", identifierToken);
        return identifier;
    }

    public string ParseStringLiteral()
    {
        Token literalToken = Eat(TokenKind.StringLiteralToken);
        return literalToken.Identifier ?? string.Empty;
    }

    public string ParseBracketedStringLiteral()
    {
        Token literalToken = Eat(TokenKind.BracketedStringLiteralToken);
        string literal = literalToken.Identifier ?? string.Empty;
        if (string.IsNullOrEmpty(literal))
            Error("a valid bracketed string literal / property reference", literalToken);
        return literal;
    }

    public float ParseNumericLiteral()
    {
        Token literalToken = Eat(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken);
        string literal = literalToken.Identifier ?? string.Empty;
        if (string.IsNullOrEmpty(literal))
            Error("a valid numeric literal", literalToken);
        return float.Parse(literal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
    }

    public int ParseIntegerLiteral()
    {
        return (int)ParseNumericLiteral();
    }

    public byte ParseByteLiteral()
    {
        return (byte)ParseNumericLiteral();
    }

    public ShaderPropertyNode ParseProperty()
    {
        List<string> attributes = new();
        while (Match(TokenKind.BracketedStringLiteralToken))
        {
            attributes.Add(ParseBracketedStringLiteral());
        }

        string uniform = ParseIdentifier();

        Eat(TokenKind.OpenParenToken);

        string name = ParseStringLiteral();
        Eat(TokenKind.CommaToken);

        ShaderPropertyKind kind = ShaderPropertyKind.None;
        (float Min, float Max)? rangeMinMax = null;
        Token typeToken = Advance();
        switch (typeToken.Kind)
        {
            case TokenKind.FloatKeyword: kind = ShaderPropertyKind.Float; break;
            case TokenKind.IntegerKeyword: kind = ShaderPropertyKind.Integer; break;
            case TokenKind.IntKeyword: kind = ShaderPropertyKind.Int; break;
            case TokenKind.ColorKeyword: kind = ShaderPropertyKind.Color; break;
            case TokenKind.VectorKeyword: kind = ShaderPropertyKind.Vector; break;
            case TokenKind._2DKeyword or TokenKind.RectKeyword: kind = ShaderPropertyKind.Texture2D; break;
            case TokenKind._3DKeyword: kind = ShaderPropertyKind.Texture3D; break;
            case TokenKind.CubeKeyword: kind = ShaderPropertyKind.TextureCube; break;
            case TokenKind._2DArrayKeyword: kind = ShaderPropertyKind.Texture2DArray; break;
            case TokenKind._3DArrayKeyword: kind = ShaderPropertyKind.Texture3DArray; break;
            case TokenKind.CubeArrayKeyword: kind = ShaderPropertyKind.TextureCubeArray; break;
            case TokenKind.AnyKeyword: kind = ShaderPropertyKind.TextureAny; break;
            case TokenKind.RangeKeyword:
                kind = ShaderPropertyKind.Range;
                Eat(TokenKind.OpenParenToken);
                float min = ParseNumericLiteral();
                Eat(TokenKind.CommaToken);
                float max = ParseNumericLiteral();
                Eat(TokenKind.CloseParenToken);
                rangeMinMax = (min, max);
                break;
            default:
                Error("a valid type", typeToken);
                break;
        }

        Eat(TokenKind.CloseParenToken);

        Eat(TokenKind.EqualsToken);

        ShaderPropertyValueNode valueNode = new();
        switch (kind)
        {
            case ShaderPropertyKind.Color or ShaderPropertyKind.Vector:
                Eat(TokenKind.OpenParenToken);
                float x = ParseNumericLiteral();
                Eat(TokenKind.CommaToken);
                float y = ParseNumericLiteral();
                Eat(TokenKind.CommaToken);
                float z = ParseNumericLiteral();
                float w = 1;
                bool hasLastChannel = false;
                if (Match(TokenKind.CommaToken))
                {
                    Eat(TokenKind.CommaToken);
                    w = ParseNumericLiteral();
                    hasLastChannel = true;
                }
                Eat(TokenKind.CloseParenToken);
                if (kind == ShaderPropertyKind.Color)
                    valueNode = new ShaderPropertyValueColorNode { HasAlphaChannel = hasLastChannel, Color = (x, y, z, w) };
                else
                    valueNode = new ShaderPropertyValueVectorNode { HasWChannel = hasLastChannel, Vector = (x, y, z, w) };
                break;

            case ShaderPropertyKind.TextureCube or ShaderPropertyKind.Texture2D or ShaderPropertyKind.Texture3D or ShaderPropertyKind.TextureAny or
                 ShaderPropertyKind.TextureCubeArray or ShaderPropertyKind.Texture2DArray or ShaderPropertyKind.Texture3DArray:
                valueNode = new ShaderPropertyValueTextureNode { TextureName = ParseStringLiteral() };
                break;

            case ShaderPropertyKind.Integer or ShaderPropertyKind.Int:
                valueNode = new ShaderPropertyValueIntegerNode { Number = ParseIntegerLiteral() };
                break;

            case ShaderPropertyKind.Float or ShaderPropertyKind.Range:
                valueNode = new ShaderPropertyValueFloatNode { Number = ParseNumericLiteral() };
                break;

            default:
                break;
        }

        if (Match(TokenKind.OpenBraceToken))
        {
            Eat(TokenKind.OpenBraceToken);
            while (Peek().Kind != TokenKind.CloseBraceToken)
                Advance();
            Eat(TokenKind.CloseBraceToken);
        }

        return new ShaderPropertyNode
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
        Eat(TokenKind.SubShaderKeyword);
        Eat(TokenKind.OpenBraceToken);

        List<ShaderPassNode> passes = new();
        List<ShaderLabCommandNode> commands = new();
        List<string> programBlocks = new();
        List<string> includeBlocks = new();

        while (!IsAtEnd())
        {
            int lastPosition = position;

            Token next = Peek();
            if (next.Kind == TokenKind.CloseBraceToken)
                break;

            switch (next.Kind)
            {
                case TokenKind.PassKeyword: passes.Add(ParseCodePass()); break;
                case TokenKind.GrabPassKeyword: passes.Add(ParseGrabPass()); break;
                case TokenKind.UsePassKeyword: passes.Add(ParseUsePass()); break;
                case TokenKind.ProgramBlock: programBlocks.Add(Eat(TokenKind.ProgramBlock).Identifier ?? string.Empty); break;
                default:
                    ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);
                    break;
            }

            if (position == lastPosition)
            {
                Error($"Parser got stuck on token type '{next.Kind}'. Please file a bug report.");
                break;
            }
        }

        ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);

        Eat(TokenKind.CloseBraceToken);

        return new SubShaderNode
        {
            Passes = passes,
            Commands = commands,
            IncludeBlocks = includeBlocks
        };
    }

    public ShaderCodePassNode ParseCodePass()
    {
        Eat(TokenKind.PassKeyword);
        Eat(TokenKind.OpenBraceToken);

        List<ShaderLabCommandNode> commands = new();
        List<string> programBlocks = new();
        List<string> includeBlocks = new();

        ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);

        while (Match(TokenKind.ProgramBlock))
        {
            programBlocks.Add(Eat(TokenKind.ProgramBlock).Identifier ?? string.Empty);
            ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);
        }

        Eat(TokenKind.CloseBraceToken);

        return new ShaderCodePassNode
        {
            ProgramBlocks = programBlocks,
            Commands = commands,
            IncludeBlocks = includeBlocks
        };
    }

    public ShaderGrabPassNode ParseGrabPass()
    {
        Eat(TokenKind.GrabPassKeyword);
        Eat(TokenKind.OpenBraceToken);

        List<ShaderLabCommandNode> commands = new();
        List<string> includeBlocks = new();

        ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);

        string? name = null;
        if (Peek().Kind != TokenKind.CloseBraceToken)
        {
            name = ParseStringLiteral();

            ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);
        }

        // TODO: Parse commands
        Eat(TokenKind.CloseBraceToken);

        return new ShaderGrabPassNode
        {
            TextureName = name,
            Commands = commands,
            IncludeBlocks = includeBlocks
        };
    }

    public ShaderUsePassNode ParseUsePass()
    {
        Eat(TokenKind.UsePassKeyword);
        return new ShaderUsePassNode
        {
            PassName = ParseStringLiteral()
        };
    }

    public void ParseCommandsAndIncludeBlocksIfPresent(List<ShaderLabCommandNode> outCommands, List<string> outIncludeBlocks)
    {
        while (true)
        {
            int lastPosition = position;

            ParseCommandsIfPresent(outCommands);
            ParseIncludeBlocksIfPresent(outIncludeBlocks);

            if (lastPosition == position)
                break;
        }
    }

    public void ParseCommandsIfPresent(List<ShaderLabCommandNode> outCommands)
    {
        bool run = true;
        while (run)
        {
            int lastPosition = position;

            Token next = Peek();
            switch (next.Kind)
            {
                case TokenKind.LightingKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandLightingNode>(next.Kind)); break;
                case TokenKind.SeparateSpecularKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandSeparateSpecularNode>(next.Kind)); break;
                case TokenKind.ZWriteKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandZWriteNode>(next.Kind)); break;
                case TokenKind.AlphaToMaskKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandAlphaToMaskNode>(next.Kind)); break;
                case TokenKind.ZClipKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandZClipNode>(next.Kind)); break;
                case TokenKind.ConservativeKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandConservativeNode>(next.Kind)); break;
                case TokenKind.TagsKeyword: outCommands.Add(ParseTagsCommand()); break;
                case TokenKind.LodKeyword: outCommands.Add(ParseLodCommand()); break;
                case TokenKind.CullKeyword: outCommands.Add(ParseCullCommand()); break;
                case TokenKind.ZTestKeyword: outCommands.Add(ParseZTestCommand()); break;
                case TokenKind.BlendKeyword: outCommands.Add(ParseBlendCommand()); break;
                case TokenKind.OffsetKeyword: outCommands.Add(ParseOffsetCommand()); break;
                case TokenKind.ColorMaskKeyword: outCommands.Add(ParseColorMaskCommand()); break;
                case TokenKind.AlphaTestKeyword: outCommands.Add(ParseAlphaTestCommand()); break;
                case TokenKind.FogKeyword: outCommands.Add(ParseFogCommand()); break;
                case TokenKind.NameKeyword: outCommands.Add(ParseNameCommand()); break;
                case TokenKind.BindChannelsKeyword: outCommands.Add(ParseBindChannelsCommand()); break;
                case TokenKind.ColorKeyword: outCommands.Add(ParseColorCommand()); break;
                case TokenKind.BlendOpKeyword: outCommands.Add(ParseBlendOpCommand()); break;
                case TokenKind.MaterialKeyword: outCommands.Add(ParseMaterialCommand()); break;
                case TokenKind.SetTextureKeyword: outCommands.Add(ParseSetTextureCommand()); break;
                case TokenKind.ColorMaterialKeyword: outCommands.Add(ParseColorMaterialNode()); break;
                case TokenKind.StencilKeyword: outCommands.Add(ParseStencilNode()); break;

                default:
                    run = false;
                    break;
            }

            if (run && position == lastPosition)
            {
                Error($"Parser got stuck on token type '{next.Kind}'. Please file a bug report.");
                break;
            }
        }
    }

    public ShaderLabCommandTagsNode ParseTagsCommand()
    {
        Eat(TokenKind.TagsKeyword);
        Eat(TokenKind.OpenBraceToken);

        Dictionary<string, string> tags = new();
        while (Peek().Kind != TokenKind.CloseBraceToken)
        {
            string key = ParseStringLiteral();
            Eat(TokenKind.EqualsToken);
            string val = ParseStringLiteral();

            tags.Add(key, val);
        }

        Eat(TokenKind.CloseBraceToken);

        return new ShaderLabCommandTagsNode
        {
            Tags = tags
        };
    }

    public ShaderLabCommandLodNode ParseLodCommand()
    {
        Eat(TokenKind.LodKeyword);
        int level = ParseIntegerLiteral();
        return new ShaderLabCommandLodNode
        {
            LodLevel = level,
        };
    }

    public PropertyReferenceOr<TOther> ParsePropertyReferenceOr<TOther>(Func<TOther> otherParser)
    {
        if (Match(TokenKind.BracketedStringLiteralToken))
        {
            return new PropertyReferenceOr<TOther> { Property = ParseBracketedStringLiteral() };
        }
        else
        {
            return new PropertyReferenceOr<TOther> { Value = otherParser() };
        }
    }

    public T ParseBasicToggleCommand<T>(TokenKind keyword)
        where T : ShaderLabBasicToggleCommandNode, new()
    {
        Eat(keyword);
        var prop = ParsePropertyReferenceOr(() =>
        {
            var kind = Eat(TokenKind.OnKeyword, TokenKind.OffKeyword, TokenKind.TrueKeyword, TokenKind.FalseKeyword).Kind;
            return kind is TokenKind.OnKeyword or TokenKind.TrueKeyword;
        });
        return new T { Enabled = prop };
    }

    public ShaderLabCommandCullNode ParseCullCommand()
    {
        Eat(TokenKind.CullKeyword);
        var prop = ParsePropertyReferenceOr(() =>
        {
            var kind = Eat(TokenKind.OffKeyword, TokenKind.FrontKeyword, TokenKind.BackKeyword, TokenKind.FalseKeyword).Kind;
            CullMode mode = default;
            if (kind is TokenKind.OffKeyword or TokenKind.FalseKeyword)
                mode = CullMode.Off;
            else if (kind == TokenKind.FrontKeyword)
                mode = CullMode.Front;
            else if (kind == TokenKind.BackKeyword)
                mode = CullMode.Back;
            return mode;
        });
        return new ShaderLabCommandCullNode { Mode = prop };
    }

    public TEnum ParseEnum<TEnum>(string expected)
        where TEnum : struct
    {
        Token next = Advance();
        // ShaderLab has a lot of ambiguous syntax, many keywords are reused in multiple places as regular identifiers.
        // If we fail to use the identifier directly, it might be an overlapping keyword, so try that instead.
        string identifier = next.Identifier ?? next.Kind.ToString().Replace("Keyword", "");
        if (Enum.TryParse(identifier, true, out TEnum result))
        {
            return result;
        }
        else
        {
            Error(expected, next);
            return default;
        }
    }

    public ShaderLabCommandZTestNode ParseZTestCommand()
    {
        Eat(TokenKind.ZTestKeyword);
        var prop = ParsePropertyReferenceOr(() => ParseEnum<ComparisonMode>("a valid comparison operator"));
        return new ShaderLabCommandZTestNode { Mode = prop };
    }

    private static readonly Dictionary<TokenKind, BlendFactor> blendFactors = new()
    {
        { TokenKind.OneKeyword, BlendFactor.One },
        { TokenKind.ZeroKeyword, BlendFactor.Zero },
        { TokenKind.SrcColorKeyword, BlendFactor.SrcColor },
        { TokenKind.SrcAlphaKeyword, BlendFactor.SrcAlpha },
        { TokenKind.SrcAlphaSaturateKeyword, BlendFactor.SrcAlphaSaturate },
        { TokenKind.DstColorKeyword, BlendFactor.DstColor },
        { TokenKind.DstAlphaKeyword, BlendFactor.DstAlpha },
        { TokenKind.OneMinusSrcColorKeyword, BlendFactor.OneMinusSrcColor },
        { TokenKind.OneMinusSrcAlphaKeyword, BlendFactor.OneMinusSrcAlpha },
        { TokenKind.OneMinusDstColorKeyword, BlendFactor.OneMinusDstColor },
        { TokenKind.OneMinusDstAlphaKeyword, BlendFactor.OneMinusDstAlpha }
    };
    private static readonly TokenKind[] blendFactorsKeys = blendFactors.Keys.ToArray();

    public ShaderLabCommandBlendNode ParseBlendCommand()
    {
        Eat(TokenKind.BlendKeyword);

        int renderTarget = 0;
        if (Match(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken))
        {
            renderTarget = ParseIntegerLiteral();
        }

        if (Match(TokenKind.OffKeyword, TokenKind.FalseKeyword))
        {
            Advance();
            return new ShaderLabCommandBlendNode { RenderTarget = renderTarget, Enabled = false };
        }

        var srcRGB = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));
        var dstRGB = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));

        var srcAlpha = srcRGB;
        var dstAlpha = dstRGB;
        if (Match(TokenKind.CommaToken))
        {
            Eat(TokenKind.CommaToken);
            srcAlpha = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));
            dstAlpha = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));
        }

        return new ShaderLabCommandBlendNode
        {
            RenderTarget = renderTarget,
            Enabled = true,
            SourceFactorRGB = srcRGB,
            DestinationFactorRGB = dstRGB,
            SourceFactorAlpha = srcAlpha,
            DestinationFactorAlpha = dstAlpha
        };
    }

    public ShaderLabCommandOffsetNode ParseOffsetCommand()
    {
        Eat(TokenKind.OffsetKeyword);
        var factor = ParsePropertyReferenceOr(ParseNumericLiteral);
        Eat(TokenKind.CommaToken);
        var units = ParsePropertyReferenceOr(ParseNumericLiteral);
        return new ShaderLabCommandOffsetNode { Factor = factor, Units = units };
    }

    public ShaderLabCommandColorMaskNode ParseColorMaskCommand()
    {
        Eat(TokenKind.ColorMaskKeyword);
        var mask = ParsePropertyReferenceOr(() =>
        {
            Token next = Peek();
            if (next.Kind is TokenKind.FloatLiteralToken or TokenKind.IntegerLiteralToken)
            {
                string result = ParseNumericLiteral().ToString();
                if (result != "0")
                    Error("the numeric literal 0", next);
                return result;
            }
            else
            {
                string result = ParseIdentifier();
                if (!result.ToLower().All(x => x is 'r' or 'g' or 'b' or 'a'))
                    Error("a valid mask containing only the letter 'r', 'g', 'b', 'a'", next);
                return result;
            }
        });
        int renderTarget = 0;
        if (Match(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken))
        {
            renderTarget = ParseIntegerLiteral();
        }
        return new ShaderLabCommandColorMaskNode { RenderTarget = renderTarget, Mask = mask };
    }

    public ShaderLabCommandAlphaTestNode ParseAlphaTestCommand()
    {
        Eat(TokenKind.AlphaTestKeyword);
        var prop = ParsePropertyReferenceOr(() => ParseEnum<ComparisonMode>("a valid comparison operator"));
        PropertyReferenceOr<float>? alpha = null;
        if (Match(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken, TokenKind.BracketedStringLiteralToken))
        {
            alpha = ParsePropertyReferenceOr(ParseNumericLiteral);
        }
        return new ShaderLabCommandAlphaTestNode { Mode = prop, AlphaValue = alpha };
    }

    private void ParseColor(out (float r, float g, float b, float a) color, out bool hasAlphaChannel)
    {
        hasAlphaChannel = false;
        float r, g, b, a = 1;
        Eat(TokenKind.OpenParenToken);
        r = ParseNumericLiteral();
        Eat(TokenKind.CommaToken);
        g = ParseNumericLiteral();
        Eat(TokenKind.CommaToken);
        b = ParseNumericLiteral();
        if (Match(TokenKind.CommaToken))
        {
            Eat(TokenKind.CommaToken);
            a = ParseNumericLiteral();
            hasAlphaChannel = true;
        }
        Eat(TokenKind.CloseParenToken);
        color = (r, g, b, a);
    }

    public ShaderLabCommandFogNode ParseFogCommand()
    {
        Eat(TokenKind.FogKeyword);
        Eat(TokenKind.OpenBraceToken);

        (float, float, float, float)? color = null;
        bool isEnabled;
        if (Match(TokenKind.ColorKeyword))
        {
            isEnabled = true;

            Eat(TokenKind.ColorKeyword);
            ParseColor(out var parsedColor, out _);
            color = parsedColor;
        }
        else
        {
            Eat(TokenKind.ModeKeyword);

            TokenKind modeKind = Eat(TokenKind.OffKeyword, TokenKind.GlobalKeyword).Kind;
            isEnabled = modeKind == TokenKind.GlobalKeyword;
        }

        Eat(TokenKind.CloseBraceToken);
        return new ShaderLabCommandFogNode { Enabled = isEnabled, Color = color };
    }

    public ShaderLabCommandNameNode ParseNameCommand()
    {
        Eat(TokenKind.NameKeyword);
        string name = ParseStringLiteral();
        return new ShaderLabCommandNameNode { Name = name };
    }

    public ShaderLabCommandBindChannelsNode ParseBindChannelsCommand()
    {
        Eat(TokenKind.BindChannelsKeyword);
        Eat(TokenKind.OpenBraceToken);

        Dictionary<BindChannel, BindChannel> bindings = new();
        while (Peek().Kind != TokenKind.CloseBraceToken)
        {
            Eat(TokenKind.BindKeyword);
            string source = ParseStringLiteral();
            Eat(TokenKind.CommaToken);
            Token targetToken = Advance();
            // Handle ShaderLab's ambiguous syntax: Could be a keyword or an identifier here, in the case of color.
            string target = targetToken.Kind == TokenKind.ColorKeyword ? "color" : targetToken.Identifier ?? String.Empty;
            if (SyntaxFacts.TryParseBindChannelName(source, out BindChannel sourceChannel) &&
                SyntaxFacts.TryParseBindChannelName(target, out BindChannel targetChannel))
            {
                bindings[sourceChannel] = targetChannel;
            }
            else
            {
                Error($"Failed to parse channel binding from '{source}' to '{target}'.");
            }
        }

        Eat(TokenKind.CloseBraceToken);

        return new ShaderLabCommandBindChannelsNode { Bindings = bindings };
    }

    public ShaderLabCommandColorNode ParseColorCommand()
    {
        Eat(TokenKind.ColorKeyword);
        bool hasAlphaChannel = false;
        var prop = ParsePropertyReferenceOr(() =>
        {
            ParseColor(out var color, out hasAlphaChannel);
            return color;
        });
        return new ShaderLabCommandColorNode { Color = prop, HasAlphaChannel = hasAlphaChannel };
    }

    private static readonly Dictionary<TokenKind, BlendOp> blendOps = new()
    {
        { TokenKind.AddKeyword, BlendOp.Add }, 
        { TokenKind.SubKeyword, BlendOp.Sub }, 
        { TokenKind.RevSubKeyword, BlendOp.RevSub }, 
        { TokenKind.MinKeyword, BlendOp.Min }, 
        { TokenKind.MaxKeyword, BlendOp.Max }, 
        { TokenKind.LogicalClearKeyword, BlendOp.LogicalClear }, 
        { TokenKind.LogicalSetKeyword, BlendOp.LogicalSet }, 
        { TokenKind.LogicalCopyKeyword, BlendOp.LogicalCopy }, 
        { TokenKind.LogicalCopyInvertedKeyword, BlendOp.LogicalCopyInverted }, 
        { TokenKind.LogicalNoopKeyword, BlendOp.LogicalNoop }, 
        { TokenKind.LogicalInvertKeyword, BlendOp.LogicalInvert }, 
        { TokenKind.LogicalAndKeyword, BlendOp.LogicalAnd }, 
        { TokenKind.LogicalNandKeyword, BlendOp.LogicalNand }, 
        { TokenKind.LogicalOrKeyword, BlendOp.LogicalOr }, 
        { TokenKind.LogicalNorKeyword, BlendOp.LogicalNor }, 
        { TokenKind.LogicalXorKeyword, BlendOp.LogicalXor }, 
        { TokenKind.LogicalEquivKeyword, BlendOp.LogicalEquiv }, 
        { TokenKind.LogicalAndReverseKeyword, BlendOp.LogicalAndReverse }, 
        { TokenKind.LogicalOrReverseKeyword, BlendOp.LogicalOrReverse }, 
        { TokenKind.LogicalOrInvertedKeyword, BlendOp.LogicalOrInverted }, 
        { TokenKind.MultiplyKeyword, BlendOp.Multiply }, 
        { TokenKind.ScreenKeyword, BlendOp.Screen }, 
        { TokenKind.OverlayKeyword, BlendOp.Overlay }, 
        { TokenKind.DarkenKeyword, BlendOp.Darken }, 
        { TokenKind.LightenKeyword, BlendOp.Lighten }, 
        { TokenKind.ColorDodgeKeyword, BlendOp.ColorDodge }, 
        { TokenKind.ColorBurnKeyword, BlendOp.ColorBurn }, 
        { TokenKind.HardLightKeyword, BlendOp.HardLight }, 
        { TokenKind.SoftLightKeyword, BlendOp.SoftLight }, 
        { TokenKind.DifferenceKeyword, BlendOp.Difference }, 
        { TokenKind.ExclusionKeyword, BlendOp.Exclusion }, 
        { TokenKind.HSLHueKeyword, BlendOp.HSLHue }, 
        { TokenKind.HSLSaturationKeyword, BlendOp.HSLSaturation }, 
        { TokenKind.HSLColorKeyword, BlendOp.HSLColor }, 
        { TokenKind.HSLLuminosityKeyword, BlendOp.HSLLuminosity }, 
    };
    private static readonly TokenKind[] blendOpsKeys = blendOps.Keys.ToArray();
    public ShaderLabCommandBlendOpNode ParseBlendOpCommand()
    {
        Eat(TokenKind.BlendOpKeyword);
        var op = ParsePropertyReferenceOr(() => blendOps.GetValueOrDefault(Eat(blendOpsKeys).Kind));
        return new ShaderLabCommandBlendOpNode { BlendOp = op };
    }

    private static readonly Dictionary<TokenKind, FixedFunctionMaterialProperty> fixedFunctionsMatProps = new()
    {
        { TokenKind.DiffuseKeyword, FixedFunctionMaterialProperty.Diffuse },
        { TokenKind.SpecularKeyword, FixedFunctionMaterialProperty.Specular },
        { TokenKind.AmbientKeyword, FixedFunctionMaterialProperty.Ambient },
        { TokenKind.EmissionKeyword, FixedFunctionMaterialProperty.Emission },
        { TokenKind.ShininessKeyword, FixedFunctionMaterialProperty.Shininess },
    };
    private static readonly TokenKind[] fixedFunctionsMatPropsKeys = fixedFunctionsMatProps.Keys.ToArray();
    public ShaderLabCommandMaterialNode ParseMaterialCommand()
    {
        Eat(TokenKind.MaterialKeyword);
        Eat(TokenKind.OpenBraceToken);

        Dictionary<FixedFunctionMaterialProperty, PropertyReferenceOr<(float, float, float, float)>> props = new();
        while (!Match(TokenKind.CloseBraceToken))
        {
            var prop = fixedFunctionsMatProps.GetValueOrDefault(Eat(fixedFunctionsMatPropsKeys).Kind);
            var val = ParsePropertyReferenceOr(() =>
            {
                ParseColor(out var color, out _);
                return color;
            });

        }

        Eat(TokenKind.CloseBraceToken);

        return new ShaderLabCommandMaterialNode { Properties = props };
    }

    public ShaderLabCommandSetTextureNode ParseSetTextureCommand()
    {
        Eat(TokenKind.SetTextureKeyword);
        string name = ParseBracketedStringLiteral();
        Eat(TokenKind.OpenBraceToken);

        List<Token> tokens = new();
        while (!Match(TokenKind.CloseBraceToken))
        {
            tokens.Add(Advance());
        }

        Eat(TokenKind.CloseBraceToken);

        return new ShaderLabCommandSetTextureNode { TextureName = name, Body = tokens };
    }

    public ShaderLabCommandColorMaterialNode ParseColorMaterialNode()
    {
        Eat(TokenKind.ColorMaterialKeyword);
        bool ambient = Eat(TokenKind.EmissionKeyword, TokenKind.AmbientAndDiffuseKeyword).Kind == TokenKind.AmbientAndDiffuseKeyword;
        return new ShaderLabCommandColorMaterialNode { AmbientAndDiffuse = ambient };
    }

    public ShaderLabCommandStencilNode ParseStencilNode()
    {
        Eat(TokenKind.StencilKeyword);
        Eat(TokenKind.OpenBraceToken);

        // Set defaults
        var result = new ShaderLabCommandStencilNode
        {
            Ref = new PropertyReferenceOr<byte> { Value = 0 },
            ReadMask = new PropertyReferenceOr<byte> { Value = 255 },
            WriteMask = new PropertyReferenceOr<byte> { Value = 255 },
            ComparisonOperationBack = new PropertyReferenceOr<ComparisonMode> { Value = ComparisonMode.Always },
            PassOperationBack = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
            FailOperationBack = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
            ZFailOperationBack = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
            ComparisonOperationFront = new PropertyReferenceOr<ComparisonMode> { Value = ComparisonMode.Always },
            PassOperationFront = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
            FailOperationFront = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
            ZFailOperationFront = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
        };

        StencilOp ParseStencilOp() => ParseEnum<StencilOp>("a valid stencil operator");
        ComparisonMode ParseComparisonMode() => ParseEnum<ComparisonMode>("a valid stencil comparison operator");

        while (!Match(TokenKind.CloseBraceToken))
        {
            Token next = Advance();
            switch (next.Kind)
            {
                case TokenKind.RefKeyword: result.Ref = ParsePropertyReferenceOr(ParseByteLiteral); break;
                case TokenKind.ReadMaskKeyword: result.ReadMask = ParsePropertyReferenceOr(ParseByteLiteral); break;
                case TokenKind.WriteMaskKeyword: result.WriteMask = ParsePropertyReferenceOr(ParseByteLiteral); break;
                case TokenKind.CompKeyword: result.ComparisonOperationBack = result.ComparisonOperationFront = ParsePropertyReferenceOr(ParseComparisonMode); break;
                case TokenKind.PassKeyword: result.PassOperationBack = result.PassOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.FailKeyword: result.FailOperationBack = result.FailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.ZFailKeyword: result.ZFailOperationBack = result.ZFailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.CompBackKeyword: result.ComparisonOperationBack = ParsePropertyReferenceOr(ParseComparisonMode); break;
                case TokenKind.PassBackKeyword: result.PassOperationBack = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.FailBackKeyword: result.FailOperationBack = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.ZFailBackKeyword: result.ZFailOperationBack = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.CompFrontKeyword: result.ComparisonOperationFront = ParsePropertyReferenceOr(ParseComparisonMode); break;
                case TokenKind.PassFrontKeyword: result.PassOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.FailFrontKeyword: result.FailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                case TokenKind.ZFailFrontKeyword: result.ZFailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;

                default:
                    Error("a valid stencil operation", next);
                    break;
            }
        }

        Eat(TokenKind.CloseBraceToken);

        return result;
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
    }
}
namespace UnityShaderParser.ShaderLab
{
    #region Tokens
    public struct SourceSpan
    {
        public (int Line, int Column) Start;
        public (int Line, int Column) End;

        public override string ToString() => $"({Start.Line}:{Start.Column} - {End.Line}:{End.Column})";
    }

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
    #endregion

    #region Common types
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

    public enum CullMode
    {
        Off,
        Front,
        Back,
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

    public enum FixedFunctionMaterialProperty
    {
        Diffuse,
        Ambient,
        Shininess,
        Specular,
        Emission,
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
    #endregion

    #region Syntax Tree
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

    public class ShaderLabCommandCullNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<CullMode> Mode { get; set; }
    }

    public class ShaderLabCommandZTestNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<ComparisonMode> Mode { get; set; }
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

    public class ShaderLabCommandBindChannelsNode : ShaderLabCommandNode
    {
        public Dictionary<BindChannel, BindChannel> Bindings { get; set; } = new();
    }

    public class ShaderLabCommandColorNode : ShaderLabCommandNode
    {
        public bool HasAlphaChannel { get; set; } = false;
        public PropertyReferenceOr<(float r, float g, float b, float a)> Color { get; set; }
    }

    public class ShaderLabCommandBlendOpNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<BlendOp> BlendOp { get; set; }
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
    #endregion
}

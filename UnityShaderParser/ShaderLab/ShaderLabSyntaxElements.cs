using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.ShaderLab
{
    using SLToken = Token<TokenKind>;

    #region Tokens
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
    #endregion

    #region Common types
    // Either a reference to a property or some other type
    public struct PropertyReferenceOr<TOther>
    {
        public TOther Value;
        public string Property;

        public bool IsValue => Value != null;
        public bool IsPropertyReference => Property != null;
        public bool IsValid => IsValue || IsPropertyReference;

        public override string ToString()
        {
            if (Property != null) return Property;
            else if (Value != null) return Value.ToString();
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

    public enum TextureType
    {
        Texture2D,
        Texture3D,
        TextureCube,
        TextureAny,
        Texture2DArray,
        Texture3DArray,
        TextureCubeArray,
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
    // Embedded HLSL
    public struct HLSLBlock
    {
        public string FullCode;
        public string CodeWithoutIncludes;
        public List<string> Pragmas;
        public List<HLSLSyntaxNode> TopLevelDeclarations;
    }

    public abstract class ShaderLabSyntaxNode : SyntaxNode<ShaderLabSyntaxNode>
    {
        public abstract void Accept(ShaderLabSyntaxVisitor visitor);

        public override SourceSpan Span => span;
        private SourceSpan span;

        public ShaderLabSyntaxNode(SLToken first, SLToken last)
        {
            this.span = new SourceSpan
            {
                Start = first.Span.Start,
                End = last.Span.End,
            };
        }

        public ShaderLabSyntaxNode(SourceSpan span)
        {
            this.span = span;
        }
    }

    public class ShaderNode : ShaderLabSyntaxNode
    {
        public string Name { get; set; }
        public List<ShaderPropertyNode> Properties { get; set; }
        public List<SubShaderNode> SubShaders { get; set; }
        public string Fallback { get; set; } // Optional
        public bool FallbackDisabledExplicitly { get; set; }
        public string CustomEditor { get; set; } // Optional
        public Dictionary<string, string> Dependencies { get; set; }
        public List<string> IncludeBlocks { get; set; }

        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => MergeChildren(Properties, SubShaders);
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderNode(this);

        public ShaderNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyNode : ShaderLabSyntaxNode
    {
        public List<string> Attributes { get; set; }
        public string Uniform { get; set; }
        public string Name { get; set; }
        public ShaderPropertyKind Kind = ShaderPropertyKind.None;
        public (float Min, float Max)? RangeMinMax { get; set; } // Optional
        public ShaderPropertyValueNode Value { get; set; }

        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => new[] { Value };
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyNode(this);

        public ShaderPropertyNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyValueNode : ShaderLabSyntaxNode
    {
        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => Enumerable.Empty<ShaderLabSyntaxNode>();
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyValueNode(this);

        public ShaderPropertyValueNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyValueNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyValueFloatNode : ShaderPropertyValueNode
    {
        public float Number { get; set; } = 0;

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyValueFloatNode(this);

        public ShaderPropertyValueFloatNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyValueFloatNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyValueIntegerNode : ShaderPropertyValueNode
    {
        public int Number { get; set; } = 0;

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyValueIntegerNode(this);

        public ShaderPropertyValueIntegerNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyValueIntegerNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyValueVectorNode : ShaderPropertyValueNode
    {
        public bool HasWChannel { get; set; }
        public (float x, float y, float z, float w) Vector { get; set; } = default;

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyValueVectorNode(this);

        public ShaderPropertyValueVectorNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyValueVectorNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyValueColorNode : ShaderPropertyValueNode
    {
        public bool HasAlphaChannel { get; set; }
        public (float x, float y, float z, float w) Color { get; set; } = default;

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyValueColorNode(this);

        public ShaderPropertyValueColorNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyValueColorNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPropertyValueTextureNode : ShaderPropertyValueNode
    {
        public TextureType Kind { get; set; }
        public string TextureName { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPropertyValueTextureNode(this);

        public ShaderPropertyValueTextureNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPropertyValueTextureNode(SourceSpan span) : base(span) { }
    }

    public class SubShaderNode : ShaderLabSyntaxNode
    {
        public List<ShaderLabCommandNode> Commands { get; set; }
        public List<ShaderPassNode> Passes { get; set; }
        public List<HLSLBlock> ProgramBlocks { get; set; }
        public List<string> IncludeBlocks { get; set; }
        public HLSLBlock? ProgramBlock => ProgramBlocks.Count > 0 ? (HLSLBlock?)ProgramBlocks[0] : null;

        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => MergeChildren(Passes, Commands);
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitSubShaderNode(this);

        public SubShaderNode(SLToken first, SLToken last) : base(first, last) { }
        public SubShaderNode(SourceSpan span) : base(span) { }
    }

    public class ShaderPassNode : ShaderLabSyntaxNode
    {
        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => Enumerable.Empty<ShaderLabSyntaxNode>();
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderPassNode(this);

        public ShaderPassNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderPassNode(SourceSpan span) : base(span) { }
    }

    public class ShaderCodePassNode : ShaderPassNode
    {
        public List<ShaderLabCommandNode> Commands { get; set; }
        public List<HLSLBlock> ProgramBlocks { get; set; }
        public List<string> IncludeBlocks { get; set; }
        public HLSLBlock? ProgramBlock => ProgramBlocks.Count > 0 ? (HLSLBlock?)ProgramBlocks[0] : null;

        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => Commands;
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderCodePassNode(this);

        public ShaderCodePassNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderCodePassNode(SourceSpan span) : base(span) { }
    }

    public class ShaderGrabPassNode : ShaderPassNode
    {
        public string TextureName { get; set; } // Optional
        public List<ShaderLabCommandNode> Commands { get; set; }
        public List<string> IncludeBlocks { get; set; }

        public bool IsUnnamed => string.IsNullOrEmpty(TextureName);
        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => Commands;
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderGrabPassNode(this);

        public ShaderGrabPassNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderGrabPassNode(SourceSpan span) : base(span) { }
    }

    public class ShaderUsePassNode : ShaderPassNode
    {
        public string PassName { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderUsePassNode(this);

        public ShaderUsePassNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderUsePassNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandNode : ShaderLabSyntaxNode
    {
        protected override IEnumerable<ShaderLabSyntaxNode> GetChildren => Enumerable.Empty<ShaderLabSyntaxNode>();
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandNode(this);

        public ShaderLabCommandNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandTagsNode : ShaderLabCommandNode
    {
        public Dictionary<string, string> Tags { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandTagsNode(this);

        public ShaderLabCommandTagsNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandTagsNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandLodNode : ShaderLabCommandNode
    {
        public int LodLevel { get; set; } = 0;

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandLodNode(this);

        public ShaderLabCommandLodNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandLodNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabBasicToggleCommandNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<bool> Enabled { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabBasicToggleCommandNode(this);

        public ShaderLabBasicToggleCommandNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabBasicToggleCommandNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandLightingNode : ShaderLabBasicToggleCommandNode
    {
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandLightingNode(this);

        public ShaderLabCommandLightingNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandLightingNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandSeparateSpecularNode : ShaderLabBasicToggleCommandNode
    {
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandSeparateSpecularNode(this);

        public ShaderLabCommandSeparateSpecularNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandSeparateSpecularNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandZWriteNode : ShaderLabBasicToggleCommandNode
    {
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandZWriteNode(this);

        public ShaderLabCommandZWriteNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandZWriteNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandAlphaToMaskNode : ShaderLabBasicToggleCommandNode
    {
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandAlphaToMaskNode(this);

        public ShaderLabCommandAlphaToMaskNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandAlphaToMaskNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandZClipNode : ShaderLabBasicToggleCommandNode
    {
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandZClipNode(this);

        public ShaderLabCommandZClipNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandZClipNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandConservativeNode : ShaderLabBasicToggleCommandNode
    {
        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandConservativeNode(this);

        public ShaderLabCommandConservativeNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandConservativeNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandCullNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<CullMode> Mode { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandCullNode(this);

        public ShaderLabCommandCullNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandCullNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandZTestNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<ComparisonMode> Mode { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandZTestNode(this);

        public ShaderLabCommandZTestNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandZTestNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandBlendNode : ShaderLabCommandNode
    {
        public int RenderTarget { get; set; } = 0;
        public bool Enabled { get; set; }
        public PropertyReferenceOr<BlendFactor>? SourceFactorRGB { get; set; }
        public PropertyReferenceOr<BlendFactor>? DestinationFactorRGB { get; set; }
        public PropertyReferenceOr<BlendFactor>? SourceFactorAlpha { get; set; }
        public PropertyReferenceOr<BlendFactor>? DestinationFactorAlpha { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandBlendNode(this);

        public ShaderLabCommandBlendNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandBlendNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandOffsetNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<float> Factor { get; set; }
        public PropertyReferenceOr<float> Units { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandOffsetNode(this);

        public ShaderLabCommandOffsetNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandOffsetNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandColorMaskNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<string> Mask { get; set; }
        public int RenderTarget { get; set; } = 0;

        public bool IsZeroMask => Mask.Value == "0";

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandColorMaskNode(this);

        public ShaderLabCommandColorMaskNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandColorMaskNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandAlphaTestNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<ComparisonMode> Mode { get; set; }
        public PropertyReferenceOr<float>? AlphaValue { get; set; } // Optional

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandAlphaTestNode(this);

        public ShaderLabCommandAlphaTestNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandAlphaTestNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandFogNode : ShaderLabCommandNode
    {
        public bool Enabled { get; set; }
        public (float r, float g, float b, float a)? Color { get; set; } // Optional

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandFogNode(this);

        public ShaderLabCommandFogNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandFogNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandNameNode : ShaderLabCommandNode
    {
        public string Name { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandNameNode(this);

        public ShaderLabCommandNameNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandNameNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandBindChannelsNode : ShaderLabCommandNode
    {
        public Dictionary<BindChannel, BindChannel> Bindings { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandBindChannelsNode(this);

        public ShaderLabCommandBindChannelsNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandBindChannelsNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandColorNode : ShaderLabCommandNode
    {
        public bool HasAlphaChannel { get; set; }
        public PropertyReferenceOr<(float r, float g, float b, float a)> Color { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandColorNode(this);

        public ShaderLabCommandColorNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandColorNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandBlendOpNode : ShaderLabCommandNode
    {
        public PropertyReferenceOr<BlendOp> BlendOp { get; set; }
        public PropertyReferenceOr<BlendOp>? BlendOpAlpha { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandBlendOpNode(this);

        public ShaderLabCommandBlendOpNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandBlendOpNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandMaterialNode : ShaderLabCommandNode
    {
        public Dictionary<FixedFunctionMaterialProperty, PropertyReferenceOr<(float r, float g, float b, float a)>> Properties { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandMaterialNode(this);

        public ShaderLabCommandMaterialNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandMaterialNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandSetTextureNode : ShaderLabCommandNode
    {
        // TODO: Not the lazy way
        public string TextureName { get; set; }
        public List<Token<TokenKind>> Body { get; set; }

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandSetTextureNode(this);

        public ShaderLabCommandSetTextureNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandSetTextureNode(SourceSpan span) : base(span) { }
    }

    public class ShaderLabCommandColorMaterialNode : ShaderLabCommandNode
    {
        public bool AmbientAndDiffuse { get; set; }
        public bool Emission => !AmbientAndDiffuse;

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandColorMaterialNode(this);

        public ShaderLabCommandColorMaterialNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandColorMaterialNode(SourceSpan span) : base(span) { }
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

        public override void Accept(ShaderLabSyntaxVisitor visitor) => visitor.VisitShaderLabCommandStencilNode(this);

        public ShaderLabCommandStencilNode(SLToken first, SLToken last) : base(first, last) { }
        public ShaderLabCommandStencilNode(SourceSpan span) : base(span) { }
    }
    #endregion
}

using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    #region Common types
    public enum TokenKind
    {
        None,

        AppendStructuredBufferKeyword,
        BlendStateKeyword,
        BoolKeyword,
        Bool1Keyword,
        Bool2Keyword,
        Bool3Keyword,
        Bool4Keyword,
        Bool1x1Keyword,
        Bool1x2Keyword,
        Bool1x3Keyword,
        Bool1x4Keyword,
        Bool2x1Keyword,
        Bool2x2Keyword,
        Bool2x3Keyword,
        Bool2x4Keyword,
        Bool3x1Keyword,
        Bool3x2Keyword,
        Bool3x3Keyword,
        Bool3x4Keyword,
        Bool4x1Keyword,
        Bool4x2Keyword,
        Bool4x3Keyword,
        Bool4x4Keyword,
        BufferKeyword,
        ByteAddressBufferKeyword,
        BreakKeyword,
        CaseKeyword,
        CBufferKeyword,
        CentroidKeyword,
        ClassKeyword,
        ColumnMajorKeyword,
        CompileKeyword,
        CompileShaderKeyword,
        ConstantBufferKeyword,
        ConstKeyword,
        ConsumeStructuredBufferKeyword,
        ContinueKeyword,
        DefaultKeyword,
        DefKeyword,
        DepthStencilStateKeyword,
        DiscardKeyword,
        DoKeyword,
        DoubleKeyword,
        Double1Keyword,
        Double2Keyword,
        Double3Keyword,
        Double4Keyword,
        Double1x1Keyword,
        Double1x2Keyword,
        Double1x3Keyword,
        Double1x4Keyword,
        Double2x1Keyword,
        Double2x2Keyword,
        Double2x3Keyword,
        Double2x4Keyword,
        Double3x1Keyword,
        Double3x2Keyword,
        Double3x3Keyword,
        Double3x4Keyword,
        Double4x1Keyword,
        Double4x2Keyword,
        Double4x3Keyword,
        Double4x4Keyword,
        ElseKeyword,
        ErrorKeyword,
        ExportKeyword,
        ExternKeyword,
        FloatKeyword,
        Float1Keyword,
        Float2Keyword,
        Float3Keyword,
        Float4Keyword,
        Float1x1Keyword,
        Float1x2Keyword,
        Float1x3Keyword,
        Float1x4Keyword,
        Float2x1Keyword,
        Float2x2Keyword,
        Float2x3Keyword,
        Float2x4Keyword,
        Float3x1Keyword,
        Float3x2Keyword,
        Float3x3Keyword,
        Float3x4Keyword,
        Float4x1Keyword,
        Float4x2Keyword,
        Float4x3Keyword,
        Float4x4Keyword,
        ForKeyword,
        GeometryShaderKeyword,
        GloballycoherentKeyword,
        GroupsharedKeyword,
        HalfKeyword,
        Half1Keyword,
        Half2Keyword,
        Half3Keyword,
        Half4Keyword,
        Half1x1Keyword,
        Half1x2Keyword,
        Half1x3Keyword,
        Half1x4Keyword,
        Half2x1Keyword,
        Half2x2Keyword,
        Half2x3Keyword,
        Half2x4Keyword,
        Half3x1Keyword,
        Half3x2Keyword,
        Half3x3Keyword,
        Half3x4Keyword,
        Half4x1Keyword,
        Half4x2Keyword,
        Half4x3Keyword,
        Half4x4Keyword,
        IfKeyword,
        IndicesKeyword,
        InKeyword,
        InlineKeyword,
        InoutKeyword,
        InputPatchKeyword,
        IntKeyword,
        Int1Keyword,
        Int2Keyword,
        Int3Keyword,
        Int4Keyword,
        Int1x1Keyword,
        Int1x2Keyword,
        Int1x3Keyword,
        Int1x4Keyword,
        Int2x1Keyword,
        Int2x2Keyword,
        Int2x3Keyword,
        Int2x4Keyword,
        Int3x1Keyword,
        Int3x2Keyword,
        Int3x3Keyword,
        Int3x4Keyword,
        Int4x1Keyword,
        Int4x2Keyword,
        Int4x3Keyword,
        Int4x4Keyword,
        InterfaceKeyword,
        LineKeyword,
        LineAdjKeyword,
        LinearKeyword,
        LineStreamKeyword,
        MatrixKeyword,
        MessageKeyword,
        Min10FloatKeyword,
        Min10Float1Keyword,
        Min10Float2Keyword,
        Min10Float3Keyword,
        Min10Float4Keyword,
        Min10Float1x1Keyword,
        Min10Float1x2Keyword,
        Min10Float1x3Keyword,
        Min10Float1x4Keyword,
        Min10Float2x1Keyword,
        Min10Float2x2Keyword,
        Min10Float2x3Keyword,
        Min10Float2x4Keyword,
        Min10Float3x1Keyword,
        Min10Float3x2Keyword,
        Min10Float3x3Keyword,
        Min10Float3x4Keyword,
        Min10Float4x1Keyword,
        Min10Float4x2Keyword,
        Min10Float4x3Keyword,
        Min10Float4x4Keyword,
        Min12IntKeyword,
        Min12Int1Keyword,
        Min12Int2Keyword,
        Min12Int3Keyword,
        Min12Int4Keyword,
        Min12Int1x1Keyword,
        Min12Int1x2Keyword,
        Min12Int1x3Keyword,
        Min12Int1x4Keyword,
        Min12Int2x1Keyword,
        Min12Int2x2Keyword,
        Min12Int2x3Keyword,
        Min12Int2x4Keyword,
        Min12Int3x1Keyword,
        Min12Int3x2Keyword,
        Min12Int3x3Keyword,
        Min12Int3x4Keyword,
        Min12Int4x1Keyword,
        Min12Int4x2Keyword,
        Min12Int4x3Keyword,
        Min12Int4x4Keyword,
        Min16FloatKeyword,
        Min16Float1Keyword,
        Min16Float2Keyword,
        Min16Float3Keyword,
        Min16Float4Keyword,
        Min16Float1x1Keyword,
        Min16Float1x2Keyword,
        Min16Float1x3Keyword,
        Min16Float1x4Keyword,
        Min16Float2x1Keyword,
        Min16Float2x2Keyword,
        Min16Float2x3Keyword,
        Min16Float2x4Keyword,
        Min16Float3x1Keyword,
        Min16Float3x2Keyword,
        Min16Float3x3Keyword,
        Min16Float3x4Keyword,
        Min16Float4x1Keyword,
        Min16Float4x2Keyword,
        Min16Float4x3Keyword,
        Min16Float4x4Keyword,
        Min16IntKeyword,
        Min16Int1Keyword,
        Min16Int2Keyword,
        Min16Int3Keyword,
        Min16Int4Keyword,
        Min16Int1x1Keyword,
        Min16Int1x2Keyword,
        Min16Int1x3Keyword,
        Min16Int1x4Keyword,
        Min16Int2x1Keyword,
        Min16Int2x2Keyword,
        Min16Int2x3Keyword,
        Min16Int2x4Keyword,
        Min16Int3x1Keyword,
        Min16Int3x2Keyword,
        Min16Int3x3Keyword,
        Min16Int3x4Keyword,
        Min16Int4x1Keyword,
        Min16Int4x2Keyword,
        Min16Int4x3Keyword,
        Min16Int4x4Keyword,
        Min16UintKeyword,
        Min16Uint1Keyword,
        Min16Uint2Keyword,
        Min16Uint3Keyword,
        Min16Uint4Keyword,
        Min16Uint1x1Keyword,
        Min16Uint1x2Keyword,
        Min16Uint1x3Keyword,
        Min16Uint1x4Keyword,
        Min16Uint2x1Keyword,
        Min16Uint2x2Keyword,
        Min16Uint2x3Keyword,
        Min16Uint2x4Keyword,
        Min16Uint3x1Keyword,
        Min16Uint3x2Keyword,
        Min16Uint3x3Keyword,
        Min16Uint3x4Keyword,
        Min16Uint4x1Keyword,
        Min16Uint4x2Keyword,
        Min16Uint4x3Keyword,
        Min16Uint4x4Keyword,
        NamespaceKeyword,
        NointerpolationKeyword,
        NoperspectiveKeyword,
        NullKeyword,
        OutKeyword,
        OutputPatchKeyword,
        PackMatrixKeyword,
        PackoffsetKeyword,
        PassKeyword,
        PayloadKeyword,
        PixelShaderKeyword,
        PointKeyword,
        PointStreamKeyword,
        PragmaKeyword,
        PreciseKeyword,
        PrimitivesKeyword,
        RasterizerOrderedBufferKeyword,
        RasterizerOrderedByteAddressBufferKeyword,
        RasterizerOrderedStructuredBufferKeyword,
        RasterizerOrderedTexture1DKeyword,
        RasterizerOrderedTexture1DArrayKeyword,
        RasterizerOrderedTexture2DKeyword,
        RasterizerOrderedTexture2DArrayKeyword,
        RasterizerOrderedTexture3DKeyword,
        RasterizerStateKeyword,
        RegisterKeyword,
        ReturnKeyword,
        RowMajorKeyword,
        RWBufferKeyword,
        RWByteAddressBufferKeyword,
        RWStructuredBufferKeyword,
        RWTexture1DKeyword,
        RWTexture1DArrayKeyword,
        RWTexture2DKeyword,
        RWTexture2DArrayKeyword,
        RWTexture3DKeyword,
        SampleKeyword,
        SamplerKeyword,
        Sampler1DKeyword,
        Sampler2DKeyword,
        Sampler3DKeyword,
        SamplerCubeKeyword,
        SamplerComparisonStateKeyword,
        SamplerStateKeyword,
        SamplerStateLegacyKeyword,
        SharedKeyword,
        SNormKeyword,
        StaticKeyword,
        StringKeyword,
        StructKeyword,
        StructuredBufferKeyword,
        SwitchKeyword,
        TBufferKeyword,
        TechniqueKeyword,
        Technique10Keyword,
        Technique11Keyword,
        TextureKeyword,
        Texture2DLegacyKeyword,
        TextureCubeLegacyKeyword,
        Texture1DKeyword,
        Texture1DArrayKeyword,
        Texture2DKeyword,
        Texture2DArrayKeyword,
        Texture2DMSKeyword,
        Texture2DMSArrayKeyword,
        Texture3DKeyword,
        TextureCubeKeyword,
        TextureCubeArrayKeyword,
        TriangleKeyword,
        TriangleAdjKeyword,
        TriangleStreamKeyword,
        TypedefKeyword,
        UniformKeyword,
        UNormKeyword,
        UintKeyword,
        Uint1Keyword,
        Uint2Keyword,
        Uint3Keyword,
        Uint4Keyword,
        Uint1x1Keyword,
        Uint1x2Keyword,
        Uint1x3Keyword,
        Uint1x4Keyword,
        Uint2x1Keyword,
        Uint2x2Keyword,
        Uint2x3Keyword,
        Uint2x4Keyword,
        Uint3x1Keyword,
        Uint3x2Keyword,
        Uint3x3Keyword,
        Uint3x4Keyword,
        Uint4x1Keyword,
        Uint4x2Keyword,
        Uint4x3Keyword,
        Uint4x4Keyword,
        VectorKeyword,
        VertexShaderKeyword,
        VerticesKeyword,
        VolatileKeyword,
        VoidKeyword,
        WarningKeyword,
        WhileKeyword,
        TrueKeyword,
        FalseKeyword,
        UnsignedKeyword,
        DwordKeyword,
        CompileFragmentKeyword,
        DepthStencilViewKeyword,
        ComputeShaderKeyword,
        DomainShaderKeyword,
        HullShaderKeyword,
        PixelfragmentKeyword,
        RenderTargetViewKeyword,
        StateblockStateKeyword,
        StateblockKeyword,

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
        HashToken,
        HashHashToken,

        IdentifierToken,
        IntegerLiteralToken,
        FloatLiteralToken,
        CharacterLiteralToken,
        StringLiteralToken,
        BracketedStringLiteralToken,
    }

    public enum ScalarType
    {
        Void,
        Bool,
        Int,
        Uint,
        Half,
        Float,
        Double,
        Min16Float,
        Min10Float,
        Min16Int,
        Min12Int,
        Min16Uint,
        String
    }

    public enum LiteralKind
    {
        String,
        BracketedString,
        Float,
        Integer,
        Character,
    }

    public enum RegisterKind
    {
        Texture,
        Sampler,
        UAV,
        Buffer,
    }

    public enum PredefinedObjectType
    {
        Texture,
        Texture1D,
        Texture1DArray,
        Texture2D,
        Texture2DArray,
        Texture3D,
        TextureCube,
        TextureCubeArray,
        Texture2DMS,
        Texture2DMSArray,
        RWTexture1D,
        RWTexture1DArray,
        RWTexture2D,
        RWTexture2DArray,
        RWTexture3D,
        AppendStructuredBuffer,
        Buffer,
        ByteAddressBuffer,
        ConsumeStructuredBuffer,
        StructuredBuffer,
        ConstantBuffer,
        RasterizerOrderedBuffer,
        RasterizerOrderedByteAddressBuffer,
        RasterizerOrderedStructuredBuffer,
        RasterizerOrderedTexture1D,
        RasterizerOrderedTexture1DArray,
        RasterizerOrderedTexture2D,
        RasterizerOrderedTexture2DArray,
        RasterizerOrderedTexture3D,
        RWBuffer,
        RWByteAddressBuffer,
        RWStructuredBuffer,
        InputPatch,
        OutputPatch,
        PointStream,
        LineStream,
        TriangleStream,
        BlendState,
        DepthStencilState,
        RasterizerState,
        Sampler,
        Sampler1D,
        Sampler2D,
        Sampler3D,
        SamplerCube,
        SamplerState,
        SamplerComparisonState,
        GeometryShader,
        PixelShader,
        VertexShader,
        BuiltInTriangleIntersectionAttributes,
        RayDesc,
        RaytracingAccelerationStructure
    }
    #endregion

    public abstract class HLSLSyntaxNode
    {
        protected static IEnumerable<HLSLSyntaxNode> MergeChildren(params IEnumerable<HLSLSyntaxNode>[] children)
            => children.SelectMany(x => x);
        public abstract IEnumerable<HLSLSyntaxNode> Children { get; }
        //public abstract void Accept(ShaderLabSyntaxVisitor visitor);

        // TODO: Feed in span data
        public SourceSpan Span { get; set; }
    }

    public abstract class FunctionNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }
        // TODO public List<Modifier> Modifiers { get; set; }
        public TypeNode ReturnType { get; set; }
        public UserDefinedTypeNode Name { get; set; }
        public List<FormalParameterNode> Parameters { get; set; }
        public SemanticNode? Semantic { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => MergeChildren(Attributes); // TODO
    }

    public class FormalParameterNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }
        // TODO public List<Modifier> Modifiers { get; set; }
        public TypeNode ParamType { get; set; }
        public VariableDeclaratorNode Declarator { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => MergeChildren(Attributes, new HLSLSyntaxNode[] { ParamType, Declarator });
    }

    public class VariableDeclaratorNode : HLSLSyntaxNode
    {
        public string Name { get; set; }
        // TODO public List<ArraySpecifierNode> 
        public List<VariableDeclaratorQualifierNode> Qualifiers { get; set; }
        // TODO List<annotation> s
        public ExpressionNode? Initializer { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => throw new NotImplementedException();
    }

    public class FunctionDeclarationNode : FunctionNode
    {
    }

    public class FunctionDefinitionNode : FunctionNode
    {
        public BlockNode Body { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => MergeChildren(base.Children, new[] { Body });
    }

    public class StructDefinitionNode : HLSLSyntaxNode
    {
        public UserDefinedTypeNode Name { get; set; }
        public List<UserDefinedTypeNode> Inherits { get; set; }
        public List<VariableDeclarationStatementNode> Declarations { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => MergeChildren(new[] { Name }, Inherits, Declarations);
    }

    public class ConstantBufferNode : HLSLSyntaxNode
    {
        public UserDefinedTypeNode Name { get; set; }
        public RegisterLocationNode? RegisterLocation { get; set; }
        public List<VariableDeclarationStatementNode> Declarations { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children =>
            MergeChildren(RegisterLocation == null ?
                new HLSLSyntaxNode[] { Name } :
                new HLSLSyntaxNode[] { Name, RegisterLocation },
                Declarations);
    }

    public abstract class VariableDeclaratorQualifierNode : HLSLSyntaxNode
    {
    }
    // TODO: PackOffset
    // TODO: RegisterLocation

    public class SemanticNode : VariableDeclaratorQualifierNode
    {
        public string Name { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public class RegisterLocationNode : VariableDeclaratorQualifierNode
    {
        public RegisterKind Kind { get; set; }
        public int Location { get; set; }
        public int? Space { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public abstract class StatementNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Attributes;
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> Statements { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => MergeChildren(base.Children, Statements);
    }

    public class VariableDeclarationStatementNode : StatementNode
    {
        // TODO: modifiers
        public TypeNode Kind { get; set; }
        public List<VariableDeclaratorNode> Declarators { get; set; }
    }

    public class ReturnStatementNode : StatementNode
    {
        public ExpressionNode? Expression { get; set; }
    }

    public class ExpressionStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }
    }

    public class AttributeNode : HLSLSyntaxNode
    {
        public string Name { get; set; }
        public List<LiteralExpressionNode> Arguments { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Arguments;
    }

    public abstract class ExpressionNode : HLSLSyntaxNode { }

    public abstract class NamedExpressionNode : ExpressionNode
    {
    }

    public class QualifiedIdentifierExpressionNode : NamedExpressionNode
    {
        public IdentifierExpressionNode Left { get; set; }
        public NamedExpressionNode Right { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Left, Right };
    }

    public class IdentifierExpressionNode : NamedExpressionNode
    {
        public string Name { get; set; } = string.Empty;

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public class LiteralExpressionNode : ExpressionNode
    {
        public string Lexeme { get; set; }
        public LiteralKind Kind { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public class AssignmentExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public TokenKind Operator { get; set; } // TODO: Not TokenKind
        public ExpressionNode Right { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Left, Right };
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public TokenKind Operator { get; set; } // TODO: Not TokenKind
        public ExpressionNode Right { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Left, Right };
    }

    public class PrefixUnaryExpressionNode : ExpressionNode
    {
        public TokenKind Operator { get; set; } // TODO: Not TokenKind
        public ExpressionNode Expression { get; set; }
        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Expression };
    }

    public class PostfixUnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Expression { get; set; }
        public TokenKind Operator { get; set; } // TODO: Not TokenKind
        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Expression };
    }

    public class FieldAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string Name { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Target };
    }

    public class MethodCallExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Target };
    }

    public class FunctionCallExpressionNode : ExpressionNode
    {
        public NamedExpressionNode Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public class NumericConstructorCallExpressionNode : ExpressionNode
    {
        public NumericTypeNode Kind { get; set; } // TODO: Not TokenKind
        public List<ExpressionNode> Arguments { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public class CastExpressionNode : ExpressionNode
    {
        public TypeNode Kind { get; set; }
        public ExpressionNode Expression { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new HLSLSyntaxNode[] { Kind, Expression };
    }

    public abstract class TypeNode : HLSLSyntaxNode { }
    public abstract class UserDefinedTypeNode : TypeNode { } // TODO: Different Name node for declarations vs other places?
    public abstract class PredefinedTypeNode : TypeNode { }
    public abstract class TypeDefinitionNode : TypeNode { } // NOTE: Anon structs

    public class ModifiedTypeSyntax : TypeNode
    {
        //public List<Modifier> Modifiers { get; set; }
        public TypeNode Type { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => throw new NotImplementedException(); // TODO
    }

    public class QualifiedNamedTypeNode : UserDefinedTypeNode
    {
        public NamedTypeNode Left { get; set; }
        public UserDefinedTypeNode Right { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => new[] { Left, Right };
    }

    public class NamedTypeNode : UserDefinedTypeNode
    {
        public string Name { get; set; } = string.Empty;

        public override IEnumerable<HLSLSyntaxNode> Children => Enumerable.Empty<HLSLSyntaxNode>();
    }

    public class PredefinedObjectTypeNode : PredefinedTypeNode
    {
        public PredefinedObjectType Kind { get; set; }
        public List<ExpressionNode> TemplateArguments { get; set; }

        public override IEnumerable<HLSLSyntaxNode> Children => TemplateArguments;
    }

    public abstract class NumericTypeNode : PredefinedTypeNode { }

    public class ScalarTypeNode : NumericTypeNode
    {
        public ScalarType Kind { get; set; }
        public override IEnumerable<HLSLSyntaxNode> Children => throw new NotImplementedException();
    }

    public class MatrixTypeNode : NumericTypeNode
    {
        public int FirstDimension { get; set; }
        public int SecondDimension { get; set; }
        public ScalarType Kind { get; set; }
        public override IEnumerable<HLSLSyntaxNode> Children => throw new NotImplementedException();
    }

    public class VectorTypeNode : NumericTypeNode
    {
        public int Dimension { get; set; }
        public ScalarType Kind { get; set; }
        public override IEnumerable<HLSLSyntaxNode> Children => throw new NotImplementedException();
    }
}

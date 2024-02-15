﻿using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    using HLSLToken = Token<TokenKind>;

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
        Min12UintKeyword,
        Min12Uint1Keyword,
        Min12Uint2Keyword,
        Min12Uint3Keyword,
        Min12Uint4Keyword,
        Min12Uint1x1Keyword,
        Min12Uint1x2Keyword,
        Min12Uint1x3Keyword,
        Min12Uint1x4Keyword,
        Min12Uint2x1Keyword,
        Min12Uint2x2Keyword,
        Min12Uint2x3Keyword,
        Min12Uint2x4Keyword,
        Min12Uint3x1Keyword,
        Min12Uint3x2Keyword,
        Min12Uint3x3Keyword,
        Min12Uint3x4Keyword,
        Min12Uint4x1Keyword,
        Min12Uint4x2Keyword,
        Min12Uint4x3Keyword,
        Min12Uint4x4Keyword,
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

        DefineDirectiveKeyword,
        IncludeDirectiveKeyword,
        LineDirectiveKeyword,
        UndefDirectiveKeyword,
        ErrorDirectiveKeyword,
        PragmaDirectiveKeyword,
        IfDirectiveKeyword,
        IfdefDirectiveKeyword,
        IfndefDirectiveKeyword,
        ElifDirectiveKeyword,
        ElseDirectiveKeyword,
        EndifDirectiveKeyword,
        SystemIncludeLiteralToken,
        EndDirectiveToken,
        OpenFunctionLikeMacroParenToken,
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
        Min12Uint,
        String,
        UNormFloat,
        SNormFloat,
    }

    public enum LiteralKind
    {
        String,
        Float,
        Integer,
        Character,
        Boolean,
        Null,
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
        BuiltInTriangleIntersectionAttributes,
        RayDesc,
        RaytracingAccelerationStructure
    }

    public enum BindingModifier
    {
        Const,
        RowMajor,
        ColumnMajor,
        Export,
        Extern,
        Inline,
        Precise,
        Shared,
        Globallycoherent,
        Groupshared,
        Static,
        Uniform,
        Volatile,
        SNorm,
        UNorm,
        Linear,
        Centroid,
        Nointerpolation,
        Noperspective,
        Sample,

        In,
        Out,
        Inout,
        Point,
        Triangle,
        TriangleAdj,
        Line,
        LineAdj,
    }

    public enum StateKind
    {
        SamplerState,
        SamplerComparisonState,
        BlendState,
    }

    public enum OperatorKind
    {
        Assignment,
        PlusAssignment,
        MinusAssignment,
        MulAssignment,
        DivAssignment,
        ModAssignment,
        ShiftLeftAssignment,
        ShiftRightAssignment,
        BitwiseAndAssignment,
        BitwiseXorAssignment,
        BitwiseOrAssignment,

        LogicalOr,
        LogicalAnd,
        BitwiseOr,
        BitwiseAnd,
        BitwiseXor,

        Compound,
        Ternary,

        Equals,
        NotEquals,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,

        ShiftLeft,
        ShiftRight,

        Plus,
        Minus,
        Mul,
        Div,
        Mod,

        Increment,
        Decrement,

        Not,
        BitFlip,
    }
    #endregion

    #region Syntax tree
    public abstract class HLSLSyntaxNode : SyntaxNode<HLSLSyntaxNode>
    {
        public abstract void Accept(HLSLSyntaxVisitor visitor);

        public override SourceSpan Span => span;
        private SourceSpan span;
        
        public HLSLSyntaxNode(HLSLToken first, HLSLToken last)
        {
            this.span = new SourceSpan
            {
                Start = first.Span.Start,
                End = last.Span.End,
            };
        }

        public HLSLSyntaxNode(SourceSpan span)
        {
            this.span = span;
        }
    }

    public abstract class FunctionNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }
        public List<BindingModifier> Modifiers { get; set; }
        public TypeNode ReturnType { get; set; }
        public UserDefinedNamedTypeNode Name { get; set; }
        public List<FormalParameterNode> Parameters { get; set; }
        public SemanticNode Semantic { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Attributes, Child(ReturnType), Child(Name), Parameters, OptionalChild(Semantic));

        public FunctionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public FunctionNode(SourceSpan span) : base(span) { }
    }

    public class FormalParameterNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }
        public List<BindingModifier> Modifiers { get; set; }
        public TypeNode ParamType { get; set; }
        public VariableDeclaratorNode Declarator { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Attributes, Child(ParamType), Child(Declarator));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFormalParameterNode(this);

        public FormalParameterNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public FormalParameterNode(SourceSpan span) : base(span) { }   
    }

    public class VariableDeclaratorNode : HLSLSyntaxNode
    {
        public string Name { get; set; }
        public List<ArrayRankNode> ArrayRanks { get; set; }
        public List<VariableDeclaratorQualifierNode> Qualifiers { get; set; }
        public List<VariableDeclarationStatementNode> Annotations { get; set; }
        public InitializerNode Initializer { get; set; } // Optional

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(ArrayRanks, Qualifiers, Annotations, OptionalChild(Initializer));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitVariableDeclaratorNode(this);

        public VariableDeclaratorNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public VariableDeclaratorNode(SourceSpan span) : base(span) { }   
    }

    public class ArrayRankNode : HLSLSyntaxNode
    {
        public ExpressionNode Dimension { get; set; } // Optional

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            OptionalChild(Dimension);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitArrayRankNode(this);

        public ArrayRankNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ArrayRankNode(SourceSpan span) : base(span) { }   
    }

    public abstract class InitializerNode : HLSLSyntaxNode
    {
        public InitializerNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public InitializerNode(SourceSpan span) : base(span) { }   
    }

    public class ValueInitializerNode : InitializerNode
    {
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Expression);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitValueInitializerNode(this);

        public ValueInitializerNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ValueInitializerNode(SourceSpan span) : base(span) { }   
    }

    // BlendState, SamplerState, etc.
    public class StateInitializerNode : InitializerNode
    {
        public List<StatePropertyNode> States { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            States;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStateInitializerNode(this);

        public StateInitializerNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public StateInitializerNode(SourceSpan span) : base(span) { }   
    }

    public class StateArrayInitializerNode : InitializerNode
    {
        public List<StateInitializerNode> Initializers { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Initializers;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStateArrayInitializerNode(this);

        public StateArrayInitializerNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public StateArrayInitializerNode(SourceSpan span) : base(span) { }   
    }

    public class FunctionDeclarationNode : FunctionNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFunctionDeclarationNode(this);

        public FunctionDeclarationNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public FunctionDeclarationNode(SourceSpan span) : base(span) { }   
    }

    public class FunctionDefinitionNode : FunctionNode
    {
        public BlockNode Body { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Body));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFunctionDefinitionNode(this);

        public FunctionDefinitionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public FunctionDefinitionNode(SourceSpan span) : base(span) { }   
    }

    public class StructDefinitionNode : StatementNode
    {
        public StructTypeNode StructType { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(StructType));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStructDefinitionNode(this);

        public StructDefinitionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public StructDefinitionNode(SourceSpan span) : base(span) { }   
    }

    public class InterfaceDefinitionNode : StatementNode
    {
        public UserDefinedNamedTypeNode Name { get; set; }
        public List<FunctionDeclarationNode> Functions { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Name), Functions);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitInterfaceDefinitionNode(this);

        public InterfaceDefinitionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public InterfaceDefinitionNode(SourceSpan span) : base(span) { }   
    }

    public class ConstantBufferNode : HLSLSyntaxNode
    {
        public UserDefinedNamedTypeNode Name { get; set; }
        public RegisterLocationNode RegisterLocation { get; set; } // Optional
        public List<VariableDeclarationStatementNode> Declarations { get; set; }
        public bool IsTextureBuffer { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Name), OptionalChild(RegisterLocation), Declarations);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitConstantBufferNode(this);

        public ConstantBufferNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ConstantBufferNode(SourceSpan span) : base(span) { }   
    }

    public class TypedefNode : StatementNode
    {
        public TypeNode FromType { get; set; }
        public List<UserDefinedNamedTypeNode> ToNames { get; set; }
        public bool IsConst { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(FromType), ToNames);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitTypedefNode(this);

        public TypedefNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public TypedefNode(SourceSpan span) : base(span) { }   
    }

    public abstract class VariableDeclaratorQualifierNode : HLSLSyntaxNode
    {
        public VariableDeclaratorQualifierNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public VariableDeclaratorQualifierNode(SourceSpan span) : base(span) { }   
    }

    public class SemanticNode : VariableDeclaratorQualifierNode
    {
        public string Name { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSemanticNode(this);

        public SemanticNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SemanticNode(SourceSpan span) : base(span) { }   
    }

    public class RegisterLocationNode : VariableDeclaratorQualifierNode
    {
        public RegisterKind Kind { get; set; }
        public int Location { get; set; }
        public int? Space { get; set; } // Optional

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitRegisterLocationNode(this);

        public RegisterLocationNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public RegisterLocationNode(SourceSpan span) : base(span) { }   
    }

    public class PackoffsetNode : VariableDeclaratorQualifierNode
    {
        public int Location { get; set; }
        public string Swizzle { get; set; } // Optional

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPackoffsetNode(this);

        public PackoffsetNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public PackoffsetNode(SourceSpan span) : base(span) { }   
    }

    public abstract class StatementNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Attributes;

        public StatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public StatementNode(SourceSpan span) : base(span) { }   
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> Statements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Statements);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitBlockNode(this);

        public BlockNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public BlockNode(SourceSpan span) : base(span) { }   
    }

    public class VariableDeclarationStatementNode : StatementNode
    {
        public List<BindingModifier> Modifiers { get; set; }
        public TypeNode Kind { get; set; }
        public List<VariableDeclaratorNode> Declarators { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Kind), Declarators);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitVariableDeclarationStatementNode(this);

        public VariableDeclarationStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public VariableDeclarationStatementNode(SourceSpan span) : base(span) { }   
    }

    public class ReturnStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; } // Optional

        protected override IEnumerable<HLSLSyntaxNode> GetChildren => 
            MergeChildren(base.GetChildren, OptionalChild(Expression));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitReturnStatementNode(this);

        public ReturnStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ReturnStatementNode(SourceSpan span) : base(span) { }   
    }

    public class BreakStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitBreakStatementNode(this);

        public BreakStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public BreakStatementNode(SourceSpan span) : base(span) { }   
    }

    public class ContinueStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitContinueStatementNode(this);

        public ContinueStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ContinueStatementNode(SourceSpan span) : base(span) { }   
    }

    public class DiscardStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitDiscardStatementNode(this);

        public DiscardStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public DiscardStatementNode(SourceSpan span) : base(span) { }   
    }

    public class EmptyStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitEmptyStatementNode(this);

        public EmptyStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public EmptyStatementNode(SourceSpan span) : base(span) { }   
    }

    public class ForStatementNode : StatementNode
    {
        public VariableDeclarationStatementNode Declaration { get; set; } // This is mutually exclusive with Initializer
        public ExpressionNode Initializer { get; set; }

        public ExpressionNode Condition { get; set; } // Optional
        public ExpressionNode Increment { get; set; } // Optional
        public StatementNode Body { get; set; }

        public bool FirstIsDeclaration => Declaration != null;
        public bool FirstIsExpression => Initializer != null;

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, OptionalChild(Declaration), OptionalChild(Initializer), OptionalChild(Condition), OptionalChild(Increment), Child(Body));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitForStatementNode(this);

        public ForStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ForStatementNode(SourceSpan span) : base(span) { }   
    }

    public class WhileStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode Body { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Condition), Child(Body));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitWhileStatementNode(this);

        public WhileStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public WhileStatementNode(SourceSpan span) : base(span) { }   
    }

    public class DoWhileStatementNode : StatementNode
    {
        public StatementNode Body { get; set; }
        public ExpressionNode Condition { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Body), Child(Condition));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitDoWhileStatementNode(this);

        public DoWhileStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public DoWhileStatementNode(SourceSpan span) : base(span) { }   
    }

    public class IfStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode Body { get; set; }
        public StatementNode ElseClause { get; set; } // Optional

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Condition), Child(Body), OptionalChild(ElseClause));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitIfStatementNode(this);

        public IfStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public IfStatementNode(SourceSpan span) : base(span) { }   
    }

    public class SwitchStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }
        public List<SwitchClauseNode> Clauses { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Expression), Clauses);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchStatementNode(this);

        public SwitchStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SwitchStatementNode(SourceSpan span) : base(span) { }   
    }

    public class SwitchClauseNode : HLSLSyntaxNode
    {
        public List<SwitchLabelNode> Labels { get; set; }
        public List<StatementNode> Statements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Labels, Statements);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchClauseNode(this);

        public SwitchClauseNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SwitchClauseNode(SourceSpan span) : base(span) { }   
    }

    public abstract class SwitchLabelNode : HLSLSyntaxNode
    {
        public SwitchLabelNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SwitchLabelNode(SourceSpan span) : base(span) { }   
    }

    public class SwitchCaseLabelNode : SwitchLabelNode
    {
        public ExpressionNode Value { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Value);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchCaseLabelNode(this);

        public SwitchCaseLabelNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SwitchCaseLabelNode(SourceSpan span) : base(span) { }   
    }

    public class SwitchDefaultLabelNode : SwitchLabelNode
    {
        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchDefaultLabelNode(this);

        public SwitchDefaultLabelNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SwitchDefaultLabelNode(SourceSpan span) : base(span) { }   
    }

    public class ExpressionStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Expression));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitExpressionStatementNode(this);

        public ExpressionStatementNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ExpressionStatementNode(SourceSpan span) : base(span) { }   
    }

    public class AttributeNode : HLSLSyntaxNode
    {
        public string Name { get; set; }
        public List<LiteralExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Arguments;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitAttributeNode(this);

        public AttributeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public AttributeNode(SourceSpan span) : base(span) { }   
    }

    public abstract class ExpressionNode : HLSLSyntaxNode
    {
        public ExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ExpressionNode(SourceSpan span) : base(span) { }   
    }

    public abstract class NamedExpressionNode : ExpressionNode
    {
        public abstract string GetName();
        public abstract string GetUnqualifiedName();

        public NamedExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public NamedExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class QualifiedIdentifierExpressionNode : NamedExpressionNode
    {
        public IdentifierExpressionNode Left { get; set; }
        public NamedExpressionNode Right { get; set; }

        public override string GetName() => $"{Left.GetName()}::{Right.GetName()}";
        public override string GetUnqualifiedName() => Right.GetUnqualifiedName();

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitQualifiedIdentifierExpressionNode(this);

        public QualifiedIdentifierExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public QualifiedIdentifierExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class IdentifierExpressionNode : NamedExpressionNode
    {
        public string Name { get; set; }

        public override string GetName() => Name;
        public override string GetUnqualifiedName() => Name;

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitIdentifierExpressionNode(this);

        public IdentifierExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public IdentifierExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class LiteralExpressionNode : ExpressionNode
    {
        public string Lexeme { get; set; }
        public LiteralKind Kind { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitLiteralExpressionNode(this);

        public LiteralExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public LiteralExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class AssignmentExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public OperatorKind Operator { get; set; }
        public ExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitAssignmentExpressionNode(this);

        public AssignmentExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public AssignmentExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public OperatorKind Operator { get; set; }
        public ExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitBinaryExpressionNode(this);

        public BinaryExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public BinaryExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class CompoundExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitCompoundExpressionNode(this);

        public CompoundExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public CompoundExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class PrefixUnaryExpressionNode : ExpressionNode
    {
        public OperatorKind Operator { get; set; }
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Expression);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPrefixUnaryExpressionNode(this);

        public PrefixUnaryExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public PrefixUnaryExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class PostfixUnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Expression { get; set; }
        public OperatorKind Operator { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Expression);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPostfixUnaryExpressionNode(this);

        public PostfixUnaryExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public PostfixUnaryExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class FieldAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string Name { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Target);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFieldAccessExpressionNode(this);

        public FieldAccessExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public FieldAccessExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class MethodCallExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Target), Arguments);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitMethodCallExpressionNode(this);

        public MethodCallExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public MethodCallExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class FunctionCallExpressionNode : ExpressionNode
    {
        public NamedExpressionNode Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Name), Arguments);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFunctionCallExpressionNode(this);

        public FunctionCallExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public FunctionCallExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class NumericConstructorCallExpressionNode : ExpressionNode
    {
        public NumericTypeNode Kind { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Kind), Arguments);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitNumericConstructorCallExpressionNode(this);

        public NumericConstructorCallExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public NumericConstructorCallExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class ElementAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public ExpressionNode Index { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Target), Child(Index));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitElementAccessExpressionNode(this);

        public ElementAccessExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ElementAccessExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class CastExpressionNode : ExpressionNode
    {
        public TypeNode Kind { get; set; }
        public ExpressionNode Expression { get; set; }
        public List<ArrayRankNode> ArrayRanks { get; set; }
        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Kind), Child(Expression), ArrayRanks);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitCastExpressionNode(this);

        public CastExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public CastExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class ArrayInitializerExpressionNode : ExpressionNode
    {
        public List<ExpressionNode> Elements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Elements;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitArrayInitializerExpressionNode(this);

        public ArrayInitializerExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ArrayInitializerExpressionNode(SourceSpan span) : base(span) { }   
    }

    public class TernaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Condition { get; set; }
        public ExpressionNode TrueCase { get; set; }
        public ExpressionNode FalseCase { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Condition), Child(TrueCase), Child(FalseCase));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitTernaryExpressionNode(this);

        public TernaryExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public TernaryExpressionNode(SourceSpan span) : base(span) { }   
    }

    // Part of legacy sampler syntax (d3d9)
    public class SamplerStateLiteralExpressionNode : ExpressionNode
    {
        public List<StatePropertyNode> States { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            States;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSamplerStateLiteralExpressionNode(this);

        public SamplerStateLiteralExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public SamplerStateLiteralExpressionNode(SourceSpan span) : base(span) { }   
    }

    // From FX framework
    public class CompileExpressionNode : ExpressionNode
    {
        public string Target { get; set; }
        public FunctionCallExpressionNode Invocation { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Invocation);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitCompileExpressionNode(this);

        public CompileExpressionNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public CompileExpressionNode(SourceSpan span) : base(span) { }   
    }

    public abstract class TypeNode : HLSLSyntaxNode
    {
        public TypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public TypeNode(SourceSpan span) : base(span) { }   
    }
    public abstract class UserDefinedTypeNode : TypeNode
    {
        public UserDefinedTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public UserDefinedTypeNode(SourceSpan span) : base(span) { }   
    }
    public abstract class UserDefinedNamedTypeNode : UserDefinedTypeNode
    {
        public abstract string GetName();
        public abstract string GetUnqualifiedName();

        public UserDefinedNamedTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public UserDefinedNamedTypeNode(SourceSpan span) : base(span) { }   
    }
    public abstract class PredefinedTypeNode : TypeNode
    {
        public PredefinedTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public PredefinedTypeNode(SourceSpan span) : base(span) { }   
    }

    public class QualifiedNamedTypeNode : UserDefinedNamedTypeNode
    {
        public NamedTypeNode Left { get; set; }
        public UserDefinedNamedTypeNode Right { get; set; }

        public override string GetName() => $"{Left.GetName()}::{Right.GetName()}";
        public override string GetUnqualifiedName() => Right.GetUnqualifiedName();

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitQualifiedNamedTypeNode(this);

        public QualifiedNamedTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public QualifiedNamedTypeNode(SourceSpan span) : base(span) { }   
    }

    public class NamedTypeNode : UserDefinedNamedTypeNode
    {
        public string Name { get; set; }

        public override string GetName() => Name;
        public override string GetUnqualifiedName() => Name;

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitNamedTypeNode(this);

        public NamedTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public NamedTypeNode(SourceSpan span) : base(span) { }   
    }

    public class PredefinedObjectTypeNode : PredefinedTypeNode
    {
        public PredefinedObjectType Kind { get; set; }
        public List<TypeNode> TemplateArguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            TemplateArguments;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPredefinedObjectTypeNode(this);

        public PredefinedObjectTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public PredefinedObjectTypeNode(SourceSpan span) : base(span) { }   
    }

    public class StructTypeNode : UserDefinedTypeNode
    {
        public UserDefinedNamedTypeNode Name { get; set; }
        public List<UserDefinedNamedTypeNode> Inherits { get; set; }
        public List<VariableDeclarationStatementNode> Fields { get; set; }
        public List<FunctionNode> Methods { get; set; }
        public bool IsClass { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(OptionalChild(Name), Inherits, Fields, Methods);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStructTypeNode(this);

        public StructTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public StructTypeNode(SourceSpan span) : base(span) { }   
    }

    public abstract class NumericTypeNode : PredefinedTypeNode
    {
        public ScalarType Kind { get; set; }

        public NumericTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public NumericTypeNode(SourceSpan span) : base(span) { }   
    }

    public class ScalarTypeNode : NumericTypeNode
    {
        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitScalarTypeNode(this);

        public ScalarTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public ScalarTypeNode(SourceSpan span) : base(span) { }   
    }

    public class MatrixTypeNode : NumericTypeNode
    {
        public int FirstDimension { get; set; }
        public int SecondDimension { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitMatrixTypeNode(this);

        public MatrixTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public MatrixTypeNode(SourceSpan span) : base(span) { }   
    }

    public class VectorTypeNode : NumericTypeNode
    {
        public int Dimension { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitVectorTypeNode(this);

        public VectorTypeNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public VectorTypeNode(SourceSpan span) : base(span) { }   
    }

    // This type mostly exists such that template can receive literal arguments.
    // It's basically constexpr.
    public class LiteralTemplateArgumentType : TypeNode
    {
        public LiteralExpressionNode Literal { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Literal);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitLiteralTemplateArgumentType(this);

        public LiteralTemplateArgumentType(HLSLToken first, HLSLToken last) : base(first, last) { }
        public LiteralTemplateArgumentType(SourceSpan span) : base(span) { }   
    }

    // Part of an object literal (SamplerState, BlendState, etc)
    public class StatePropertyNode : StatementNode
    {
        public UserDefinedNamedTypeNode Name { get; set; }
        public ArrayRankNode ArrayRank { get; set; } // Optional
        public ExpressionNode Value { get; set; }
        public bool IsReference { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(OptionalChild(ArrayRank), Child(Value));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStatePropertyNode(this);

        public StatePropertyNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public StatePropertyNode(SourceSpan span) : base(span) { }   
    }

    // Old FX pipeline syntax
    public class TechniqueNode : HLSLSyntaxNode
    {
        public int Version { get; set; }
        public UserDefinedNamedTypeNode Name { get; set; } // Optional
        public List<VariableDeclarationStatementNode> Annotations { get; set; }
        public List<PassNode> Passes { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(OptionalChild(Name), Annotations, Passes);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitTechniqueNode(this);

        public TechniqueNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public TechniqueNode(SourceSpan span) : base(span) { }   
    }

    // Old FX pipeline syntax
    public class PassNode : HLSLSyntaxNode
    {
        public UserDefinedNamedTypeNode Name { get; set; } // Optional
        public List<VariableDeclarationStatementNode> Annotations { get; set; }
        public List<StatementNode> Statements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(OptionalChild(Name), Annotations, Statements);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPassNode(this);

        public PassNode(HLSLToken first, HLSLToken last) : base(first, last) { }
        public PassNode(SourceSpan span) : base(span) { }   
    }
    #endregion
}

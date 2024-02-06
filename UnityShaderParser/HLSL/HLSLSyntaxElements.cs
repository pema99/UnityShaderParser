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
        String
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
    }

    public abstract class FunctionNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }
        public List<BindingModifier> Modifiers { get; set; }
        public TypeNode ReturnType { get; set; }
        public UserDefinedTypeNode Name { get; set; }
        public List<FormalParameterNode> Parameters { get; set; }
        public SemanticNode? Semantic { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Attributes, Child(ReturnType), Child(Name), Parameters, OptionalChild(Semantic));
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
    }

    public class VariableDeclaratorNode : HLSLSyntaxNode
    {
        public string Name { get; set; }
        public List<ArrayRankNode> ArrayRanks { get; set; }
        public List<VariableDeclaratorQualifierNode> Qualifiers { get; set; }
        public List<VariableDeclarationStatementNode> Annotations { get; set; }
        public InitializerNode? Initializer { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(ArrayRanks, Qualifiers, Annotations, OptionalChild(Initializer));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitVariableDeclaratorNode(this);
    }

    public class ArrayRankNode : HLSLSyntaxNode
    {
        public ExpressionNode? Dimension { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            OptionalChild(Dimension);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitArrayRankNode(this);
    }

    public abstract class InitializerNode : HLSLSyntaxNode { }

    public class ValueInitializerNode : InitializerNode
    {
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Expression);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitValueInitializerNode(this);
    }

    // BlendState, SamplerState, etc.
    public class StateInitializerNode : InitializerNode
    {
        public List<StatePropertyNode> States { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            States;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStateInitializerNode(this);
    }

    public class FunctionDeclarationNode : FunctionNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFunctionDeclarationNode(this);
    }

    public class FunctionDefinitionNode : FunctionNode
    {
        public BlockNode Body { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Body));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFunctionDefinitionNode(this);
    }

    public class StructDefinitionNode : StatementNode
    {
        public UserDefinedTypeNode? Name { get; set; }
        public List<UserDefinedTypeNode> Inherits { get; set; }
        public List<VariableDeclarationStatementNode> Fields { get; set; }
        public List<FunctionNode> Methods { get; set; }
        public bool IsClass { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, OptionalChild(Name), Inherits, Fields, Methods);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStructDefinitionNode(this);
    }

    public class InterfaceDefinitionNode : StatementNode
    {
        public UserDefinedTypeNode Name { get; set; }
        public List<FunctionDeclarationNode> Functions { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Name), Functions);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitInterfaceDefinitionNode(this);
    }

    public class ConstantBufferNode : HLSLSyntaxNode
    {
        public UserDefinedTypeNode Name { get; set; }
        public RegisterLocationNode? RegisterLocation { get; set; }
        public List<VariableDeclarationStatementNode> Declarations { get; set; }
        public bool IsTextureBuffer { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Name), OptionalChild(RegisterLocation), Declarations);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitConstantBufferNode(this);
    }

    public class TypedefNode : StatementNode
    {
        public TypeNode FromType { get; set; }
        public List<UserDefinedTypeNode> ToNames { get; set; }
        public bool IsConst { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.Children, Child(FromType), ToNames);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitTypedefNode(this);
    }

    public abstract class VariableDeclaratorQualifierNode : HLSLSyntaxNode
    {
    }

    public class SemanticNode : VariableDeclaratorQualifierNode
    {
        public string Name { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSemanticNode(this);
    }

    public class RegisterLocationNode : VariableDeclaratorQualifierNode
    {
        public RegisterKind Kind { get; set; }
        public int Location { get; set; }
        public int? Space { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitRegisterLocationNode(this);
    }

    public class PackoffsetNode : VariableDeclaratorQualifierNode
    {
        public int Location { get; set; }
        public string? Swizzle { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPackoffsetNode(this);
    }

    public abstract class StatementNode : HLSLSyntaxNode
    {
        public List<AttributeNode> Attributes { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Attributes;
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> Statements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Statements);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitBlockNode(this);
    }

    public class VariableDeclarationStatementNode : StatementNode
    {
        public List<BindingModifier> Modifiers { get; set; }
        public TypeNode Kind { get; set; }
        public List<VariableDeclaratorNode> Declarators { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Kind), Declarators);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitVariableDeclarationStatementNode(this);
    }

    public class ReturnStatementNode : StatementNode
    {
        public ExpressionNode? Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren => 
            MergeChildren(base.GetChildren, OptionalChild(Expression));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitReturnStatementNode(this);
    }

    public class BreakStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitBreakStatementNode(this);
    }

    public class ContinueStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitContinueStatementNode(this);
    }

    public class DiscardStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitDiscardStatementNode(this);
    }

    public class EmptyStatementNode : StatementNode
    {
        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitEmptyStatementNode(this);
    }

    public class ForStatementNode : StatementNode
    {
        public VariableDeclarationStatementNode? Declaration { get; set; }
        public ExpressionNode? Condition { get; set; }
        public ExpressionNode? Increment { get; set; }
        public StatementNode Body { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, OptionalChild(Declaration), OptionalChild(Condition), OptionalChild(Increment), Child(Body));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitForStatementNode(this);
    }

    public class WhileStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode Body { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Condition), Child(Body));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitWhileStatementNode(this);
    }

    public class DoWhileStatementNode : StatementNode
    {
        public StatementNode Body { get; set; }
        public ExpressionNode Condition { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Body), Child(Condition));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitDoWhileStatementNode(this);
    }

    public class IfStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode Body { get; set; }
        public StatementNode? ElseClause { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Condition), Child(Body), OptionalChild(ElseClause));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitIfStatementNode(this);
    }

    public class SwitchStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }
        public List<SwitchClauseNode> Clauses { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Expression), Clauses);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchStatementNode(this);
    }

    public class SwitchClauseNode : HLSLSyntaxNode
    {
        public List<SwitchLabelNode> Labels { get; set; }
        public List<StatementNode> Statements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Labels, Statements);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchClauseNode(this);
    }

    public abstract class SwitchLabelNode : HLSLSyntaxNode { }

    public class SwitchCaseLabelNode : SwitchLabelNode
    {
        public ExpressionNode Value { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Value);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchCaseLabelNode(this);
    }

    public class SwitchDefaultLabelNode : SwitchLabelNode
    {
        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSwitchDefaultLabelNode(this);
    }

    public class ExpressionStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(base.GetChildren, Child(Expression));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitExpressionStatementNode(this);
    }

    public class AttributeNode : HLSLSyntaxNode
    {
        public string Name { get; set; }
        public List<LiteralExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Arguments;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitAttributeNode(this);
    }

    public abstract class ExpressionNode : HLSLSyntaxNode { }

    public abstract class NamedExpressionNode : ExpressionNode { }

    public class QualifiedIdentifierExpressionNode : NamedExpressionNode
    {
        public IdentifierExpressionNode Left { get; set; }
        public NamedExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitQualifiedIdentifierExpressionNode(this);
    }

    public class IdentifierExpressionNode : NamedExpressionNode
    {
        public string Name { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitIdentifierExpressionNode(this);
    }

    public class LiteralExpressionNode : ExpressionNode
    {
        public string Lexeme { get; set; }
        public LiteralKind Kind { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitLiteralExpressionNode(this);
    }

    public class AssignmentExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public OperatorKind Operator { get; set; }
        public ExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitAssignmentExpressionNode(this);
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public OperatorKind Operator { get; set; }
        public ExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitBinaryExpressionNode(this);
    }

    public class CompoundExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitCompoundExpressionNode(this);
    }

    public class PrefixUnaryExpressionNode : ExpressionNode
    {
        public OperatorKind Operator { get; set; }
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Expression);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPrefixUnaryExpressionNode(this);
    }

    public class PostfixUnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Expression { get; set; }
        public OperatorKind Operator { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Expression);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPostfixUnaryExpressionNode(this);
    }

    public class FieldAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string Name { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Target);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFieldAccessExpressionNode(this);
    }

    public class MethodCallExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Target), Arguments);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitMethodCallExpressionNode(this);
    }

    public class FunctionCallExpressionNode : ExpressionNode
    {
        public NamedExpressionNode Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Name), Arguments);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitFunctionCallExpressionNode(this);
    }

    public class NumericConstructorCallExpressionNode : ExpressionNode
    {
        public NumericTypeNode Kind { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Kind), Arguments);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitNumericConstructorCallExpressionNode(this);
    }

    public class ElementAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public ExpressionNode Index { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Target), Child(Index));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitElementAccessExpressionNode(this);
    }

    public class CastExpressionNode : ExpressionNode
    {
        public TypeNode Kind { get; set; }
        public ExpressionNode Expression { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Kind), Child(Expression));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitCastExpressionNode(this);
    }

    public class ArrayInitializerExpressionNode : ExpressionNode
    {
        public List<ExpressionNode> Elements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Elements;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitArrayInitializerExpressionNode(this);
    }

    public class TernaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Condition { get; set; }
        public ExpressionNode TrueCase { get; set; }
        public ExpressionNode FalseCase { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Condition), Child(TrueCase), Child(FalseCase));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitTernaryExpressionNode(this);
    }

    // Part of legacy sampler syntax (d3d9)
    public class SamplerStateLiteralExpressionNode : ExpressionNode
    {
        public List<StatePropertyNode> States { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            States;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitSamplerStateLiteralExpressionNode(this);
    }

    // From FX framework
    public class CompileExpressionNode : ExpressionNode
    {
        public string Target { get; set; }
        public FunctionCallExpressionNode Invocation { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Invocation);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitCompileExpressionNode(this);
    }

    public abstract class TypeNode : HLSLSyntaxNode { }
    public abstract class UserDefinedTypeNode : TypeNode { }
    public abstract class PredefinedTypeNode : TypeNode { }

    public class QualifiedNamedTypeNode : UserDefinedTypeNode
    {
        public NamedTypeNode Left { get; set; }
        public UserDefinedTypeNode Right { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Left), Child(Right));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitQualifiedNamedTypeNode(this);
    }

    public class NamedTypeNode : UserDefinedTypeNode
    {
        public string Name { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitNamedTypeNode(this);
    }

    public class PredefinedObjectTypeNode : PredefinedTypeNode
    {
        public PredefinedObjectType Kind { get; set; }
        public List<TypeNode> TemplateArguments { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            TemplateArguments;

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPredefinedObjectTypeNode(this);
    }

    public abstract class NumericTypeNode : PredefinedTypeNode
    {
        public ScalarType Kind { get; set; }
    }

    public class ScalarTypeNode : NumericTypeNode
    {
        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitScalarTypeNode(this);
    }

    public class MatrixTypeNode : NumericTypeNode
    {
        public int FirstDimension { get; set; }
        public int SecondDimension { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitMatrixTypeNode(this);
    }

    public class VectorTypeNode : NumericTypeNode
    {
        public int Dimension { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Enumerable.Empty<HLSLSyntaxNode>();

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitVectorTypeNode(this);
    }

    // This type mostly exists such that template can receive literal arguments.
    // It's basically constexpr.
    public class LiteralTemplateArgumentType : TypeNode
    {
        public LiteralExpressionNode Literal { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            Child(Literal);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitLiteralTemplateArgumentType(this);
    }

    // Part of an object literal (SamplerState, BlendState, etc)
    public class StatePropertyNode : StatementNode
    {
        public UserDefinedTypeNode Name { get; set; }
        public ArrayRankNode? ArrayRank { get; set; }
        public ExpressionNode Value { get; set; }
        public bool IsReference { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(OptionalChild(ArrayRank), Child(Value));

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitStatePropertyNode(this);
    }

    // Old FX pipeline syntax
    public class TechniqueNode : HLSLSyntaxNode
    {
        public int Version { get; set; }
        public UserDefinedTypeNode Name { get; set; }
        public List<VariableDeclarationStatementNode> Annotations { get; set; }
        public List<PassNode> Passes { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Name), Annotations, Passes);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitTechniqueNode(this);
    }

    // Old FX pipeline syntax
    public class PassNode : HLSLSyntaxNode
    {
        public UserDefinedTypeNode Name { get; set; }
        public List<VariableDeclarationStatementNode> Annotations { get; set; }
        public List<StatementNode> Statements { get; set; }

        protected override IEnumerable<HLSLSyntaxNode> GetChildren =>
            MergeChildren(Child(Name), Annotations, Statements);

        public override void Accept(HLSLSyntaxVisitor visitor) => visitor.VisitPassNode(this);
    }
    #endregion
}

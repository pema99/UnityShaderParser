namespace UnityShaderParser.HLSL
{
    public abstract class HLSLSyntaxVisitor
    {
        protected void DefaultVisit(HLSLSyntaxNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public virtual void VisitSyntaxNode(HLSLSyntaxNode node) => DefaultVisit(node);
        public virtual void VisitFormalParameterNode(FormalParameterNode node) => DefaultVisit(node);
        public virtual void VisitVariableDeclaratorNode(VariableDeclaratorNode node) => DefaultVisit(node);
        public virtual void VisitArrayRankNode(ArrayRankNode node) => DefaultVisit(node);
        public virtual void VisitValueInitializerNode(ValueInitializerNode node) => DefaultVisit(node);
        public virtual void VisitStateInitializerNode(StateInitializerNode node) => DefaultVisit(node);
        public virtual void VisitFunctionDeclarationNode(FunctionDeclarationNode node) => DefaultVisit(node);
        public virtual void VisitFunctionDefinitionNode(FunctionDefinitionNode node) => DefaultVisit(node);
        public virtual void VisitStructDefinitionNode(StructDefinitionNode node) => DefaultVisit(node);
        public virtual void VisitInterfaceDefinitionNode(InterfaceDefinitionNode node) => DefaultVisit(node);
        public virtual void VisitConstantBufferNode(ConstantBufferNode node) => DefaultVisit(node);
        public virtual void VisitSemanticNode(SemanticNode node) => DefaultVisit(node);
        public virtual void VisitRegisterLocationNode(RegisterLocationNode node) => DefaultVisit(node);
        public virtual void VisitPackoffsetNode(PackoffsetNode node) => DefaultVisit(node);
        public virtual void VisitBlockNode(BlockNode node) => DefaultVisit(node);
        public virtual void VisitVariableDeclarationStatementNode(VariableDeclarationStatementNode node) => DefaultVisit(node);
        public virtual void VisitReturnStatementNode(ReturnStatementNode node) => DefaultVisit(node);
        public virtual void VisitBreakStatementNode(BreakStatementNode node) => DefaultVisit(node);
        public virtual void VisitContinueStatementNode(ContinueStatementNode node) => DefaultVisit(node);
        public virtual void VisitDiscardStatementNode(DiscardStatementNode node) => DefaultVisit(node);
        public virtual void VisitEmptyStatementNode(EmptyStatementNode node) => DefaultVisit(node);
        public virtual void VisitForStatementNode(ForStatementNode node) => DefaultVisit(node);
        public virtual void VisitWhileStatementNode(WhileStatementNode node) => DefaultVisit(node);
        public virtual void VisitIfStatementNode(IfStatementNode node) => DefaultVisit(node);
        public virtual void VisitSwitchStatementNode(SwitchStatementNode node) => DefaultVisit(node);
        public virtual void VisitSwitchClauseNode(SwitchClauseNode node) => DefaultVisit(node);
        public virtual void VisitSwitchCaseLabelNode(SwitchCaseLabelNode node) => DefaultVisit(node);
        public virtual void VisitSwitchDefaultLabelNode(SwitchDefaultLabelNode node) => DefaultVisit(node);
        public virtual void VisitExpressionStatementNode(ExpressionStatementNode node) => DefaultVisit(node);
        public virtual void VisitAttributeNode(AttributeNode node) => DefaultVisit(node);
        public virtual void VisitQualifiedIdentifierExpressionNode(QualifiedIdentifierExpressionNode node) => DefaultVisit(node);
        public virtual void VisitIdentifierExpressionNode(IdentifierExpressionNode node) => DefaultVisit(node);
        public virtual void VisitLiteralExpressionNode(LiteralExpressionNode node) => DefaultVisit(node);
        public virtual void VisitAssignmentExpressionNode(AssignmentExpressionNode node) => DefaultVisit(node);
        public virtual void VisitBinaryExpressionNode(BinaryExpressionNode node) => DefaultVisit(node);
        public virtual void VisitCompoundExpressionNode(CompoundExpressionNode node) => DefaultVisit(node);
        public virtual void VisitPrefixUnaryExpressionNode(PrefixUnaryExpressionNode node) => DefaultVisit(node);
        public virtual void VisitPostfixUnaryExpressionNode(PostfixUnaryExpressionNode node) => DefaultVisit(node);
        public virtual void VisitFieldAccessExpressionNode(FieldAccessExpressionNode node) => DefaultVisit(node);
        public virtual void VisitMethodCallExpressionNode(MethodCallExpressionNode node) => DefaultVisit(node);
        public virtual void VisitFunctionCallExpressionNode(FunctionCallExpressionNode node) => DefaultVisit(node);
        public virtual void VisitNumericConstructorCallExpressionNode(NumericConstructorCallExpressionNode node) => DefaultVisit(node);
        public virtual void VisitElementAccessExpressionNode(ElementAccessExpressionNode node) => DefaultVisit(node);
        public virtual void VisitCastExpressionNode(CastExpressionNode node) => DefaultVisit(node);
        public virtual void VisitArrayInitializerExpressionNode(ArrayInitializerExpressionNode node) => DefaultVisit(node);
        public virtual void VisitTernaryExpressionNode(TernaryExpressionNode node) => DefaultVisit(node);
        public virtual void VisitSamplerStateLiteralExpressionNode(SamplerStateLiteralExpressionNode node) => DefaultVisit(node);
        public virtual void VisitCompileExpressionNode(CompileExpressionNode node) => DefaultVisit(node);
        public virtual void VisitQualifiedNamedTypeNode(QualifiedNamedTypeNode node) => DefaultVisit(node);
        public virtual void VisitNamedTypeNode(NamedTypeNode node) => DefaultVisit(node);
        public virtual void VisitPredefinedObjectTypeNode(PredefinedObjectTypeNode node) => DefaultVisit(node);
        public virtual void VisitScalarTypeNode(ScalarTypeNode node) => DefaultVisit(node);
        public virtual void VisitMatrixTypeNode(MatrixTypeNode node) => DefaultVisit(node);
        public virtual void VisitVectorTypeNode(VectorTypeNode node) => DefaultVisit(node);
        public virtual void VisitTechniqueNode(TechniqueNode node) => DefaultVisit(node);
        public virtual void VisitLiteralTemplateArgumentType(LiteralTemplateArgumentType node) => DefaultVisit(node);
        public virtual void VisitStatePropertyNode(StatePropertyNode node) => DefaultVisit(node);
        public virtual void VisitPassNode(PassNode node) => DefaultVisit(node);
    }
}

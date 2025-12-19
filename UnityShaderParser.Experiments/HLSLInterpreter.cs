using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLInterpreter : HLSLSyntaxVisitor
    {
        protected HLSLExpressionEvaluator expressionEvaluator = new HLSLExpressionEvaluator();

        public override void VisitVariableDeclarationStatementNode(VariableDeclarationStatementNode node)
        {
            base.VisitVariableDeclarationStatementNode(node);
        }

        public override void VisitExpressionStatementNode(ExpressionStatementNode node)
        {
            expressionEvaluator.Visit(node.Expression);
        }
    }
}

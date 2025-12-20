using System;
using System.Linq;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLInterpreter : HLSLSyntaxVisitor
    {
        protected HLSLInterpreterContext context;
        protected HLSLExecutionState executionState;
        protected HLSLExpressionEvaluator expressionEvaluator;

        public HLSLInterpreter(int threadsX = 2, int threadsY = 2)
        {
            context = new HLSLInterpreterContext();
            executionState = new HLSLExecutionState(threadsX, threadsY);
            expressionEvaluator = new HLSLExpressionEvaluator(context, executionState);
        }

        public override void VisitVariableDeclarationStatementNode(VariableDeclarationStatementNode node)
        {
            // TODO: Modifiers
            // TODO: Multiple declarations
            // TODO: Check type?

            foreach (VariableDeclaratorNode decl in node.Declarators)
            {
                // TODO: StateInitializer, StateArrayInitializer
                var initializer = decl.Initializer as ValueInitializerNode;
                if (initializer != null)
                {
                    var initializerValue = expressionEvaluator.Visit(initializer.Expression);

                    NumericValue numeric = initializerValue as NumericValue;
                    if (node.Kind is NumericTypeNode && (object)numeric == null)
                        throw new Exception("Invalid cast.");

                    switch (node.Kind)
                    {
                        case ScalarTypeNode scalarType:
                            initializerValue = HLSLValueUtils.CastNumeric(scalarType.Kind, numeric);
                            break;
                        case VectorTypeNode vectorType:
                            initializerValue = HLSLValueUtils.BroadcastToVector(HLSLValueUtils.CastNumeric(vectorType.Kind, numeric), vectorType.Dimension);
                            break;
                        case MatrixTypeNode matrixType:
                            initializerValue = HLSLValueUtils.BroadcastToMatrix(HLSLValueUtils.CastNumeric(matrixType.Kind, numeric), matrixType.FirstDimension, matrixType.SecondDimension);
                            break;
                        case PredefinedObjectTypeNode predefinedObjectType:
                            throw new Exception("Invalid cast.");
                        case QualifiedNamedTypeNode qualifiedNamedTypeNodeType:
                        case NamedTypeNode namedTypeNodeType:
                        case GenericVectorTypeNode genVectorType:
                        case GenericMatrixTypeNode genMatrixType:
                        case StructTypeNode structType:
                        default:
                            throw new NotImplementedException();
                    }
                    context.SetVariable(decl.Name, initializerValue);
                }
                else
                {
                    HLSLValue defaultValue;
                    switch (node.Kind)
                    {
                        case ScalarTypeNode scalarType:
                            defaultValue = new ScalarValue(scalarType.Kind, new HLSLRegister<object>(HLSLValueUtils.GetZeroValue(scalarType.Kind)));
                            break;
                        case VectorTypeNode vectorType:
                            defaultValue = new VectorValue(vectorType.Kind, 
                                new HLSLRegister<object[]>(Enumerable.Repeat(HLSLValueUtils.GetZeroValue(vectorType.Kind), vectorType.Dimension).ToArray()));
                            break;
                        case MatrixTypeNode matrixType:
                            defaultValue = new MatrixValue(matrixType.Kind, matrixType.FirstDimension, matrixType.SecondDimension,
                                new HLSLRegister<object[]>(Enumerable.Repeat(HLSLValueUtils.GetZeroValue(matrixType.Kind), matrixType.FirstDimension * matrixType.SecondDimension).ToArray()));
                            break;
                        case PredefinedObjectTypeNode predefinedObjectType:
                            defaultValue = new PredefinedObjectValue(predefinedObjectType.Kind, null);
                            break;
                        case QualifiedNamedTypeNode qualifiedNamedTypeNodeType:
                        case NamedTypeNode namedTypeNodeType:
                        case GenericVectorTypeNode genVectorType:
                        case GenericMatrixTypeNode genMatrixType:
                        case StructTypeNode structType:
                        default:
                            throw new NotImplementedException();
                    }
                    context.SetVariable(decl.Name, defaultValue);
                }
            }
        }

        public override void VisitIfStatementNode(IfStatementNode node)
        {
            NumericValue condValue = expressionEvaluator.Visit(node.Condition) as NumericValue;
            if (condValue is null)
                throw new Exception("Expected a numeric value for the condition.");

            ScalarValue boolCondValue = HLSLValueUtils.CastNumeric(ScalarType.Bool, condValue) as ScalarValue;
            if (boolCondValue is null)
                throw new Exception("Expected a scalar boolean value for the condition.");

            executionState.PushExecutionMask();
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                executionState.SetThreadState(threadIndex, (bool)boolCondValue.Value.Get(threadIndex));

            Visit(node.Body);

            executionState.FlipExecutionMask();

            if (node.ElseClause != null)
                Visit(node.ElseClause);

            executionState.PopExecutionMask();
        }

        public override void VisitExpressionStatementNode(ExpressionStatementNode node)
        {
            expressionEvaluator.Visit(node.Expression);
        }
    }
}

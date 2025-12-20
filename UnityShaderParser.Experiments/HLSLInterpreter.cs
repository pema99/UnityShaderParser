using System;
using System.Linq;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLInterpreter : HLSLSyntaxVisitor
    {
        protected HLSLInterpreterContext context;
        protected HLSLExpressionEvaluator expressionEvaluator;

        public HLSLInterpreter()
        {
            context = new HLSLInterpreterContext();
            expressionEvaluator = new HLSLExpressionEvaluator(context);
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
                            initializerValue = HLSLValueUtils.Map(numeric, x => HLSLValueUtils.CastNumeric(scalarType.Kind, x));
                            break;
                        case VectorTypeNode vectorType:
                            initializerValue = HLSLValueUtils.BroadcastToVector(HLSLValueUtils.Map(numeric, x => HLSLValueUtils.CastNumeric(vectorType.Kind, x)), vectorType.Dimension);
                            break;
                        case MatrixTypeNode matrixType:
                            initializerValue = HLSLValueUtils.BroadcastToMatrix(HLSLValueUtils.Map(numeric, x => HLSLValueUtils.CastNumeric(matrixType.Kind, x)), matrixType.FirstDimension, matrixType.SecondDimension);
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
                            defaultValue = new ScalarValue(scalarType.Kind, HLSLValueUtils.GetZeroValue(scalarType.Kind));
                            break;
                        case VectorTypeNode vectorType:
                            defaultValue = new VectorValue(vectorType.Kind, Enumerable.Repeat(HLSLValueUtils.GetZeroValue(vectorType.Kind), vectorType.Dimension).ToArray());
                            break;
                        case MatrixTypeNode matrixType:
                            defaultValue = new MatrixValue(matrixType.Kind, matrixType.FirstDimension, matrixType.SecondDimension,
                                Enumerable.Repeat(HLSLValueUtils.GetZeroValue(matrixType.Kind), matrixType.FirstDimension * matrixType.SecondDimension).ToArray());
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

        public override void VisitExpressionStatementNode(ExpressionStatementNode node)
        {
            expressionEvaluator.Visit(node.Expression);
        }
    }
}

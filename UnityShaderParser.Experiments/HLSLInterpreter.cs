using System;
using System.Collections.Generic;
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
            expressionEvaluator = new HLSLExpressionEvaluator(this, context, executionState);
        }

        public void SetWarpSize(int threadsX, int threadsY)
        {
            executionState = new HLSLExecutionState(threadsX, threadsY);
            expressionEvaluator = new HLSLExpressionEvaluator(this, context, executionState);
        }

        public void Reset()
        {
            context = new HLSLInterpreterContext();
            executionState = new HLSLExecutionState(executionState.GetThreadsX(), executionState.GetThreadsY());
            expressionEvaluator = new HLSLExpressionEvaluator(this, context, executionState);
        }

        public void AddCallback(string name, Func<ExpressionNode[], HLSLValue> callback) => expressionEvaluator.AddCallback(name, callback);
        public void RemoveCallback(string name) => expressionEvaluator.RemoveCallback(name);
        public HLSLValue CallFunction(string name, params HLSLValue[] args) => expressionEvaluator.CallFunction(name, args);

        public HLSLValue CallFunction(string name, params object[] args)
        {
            List<ExpressionNode> exprArgs = new List<ExpressionNode>();
            for (int i = 0; i < args.Length; i++)
            {
                LiteralKind kind;
                switch (args[i])
                {
                    case string _: kind = LiteralKind.String; break;
                    case float _: kind = LiteralKind.Float; break;
                    case int _: kind = LiteralKind.Integer; break;
                    case char _: kind = LiteralKind.Character; break;
                    case bool _: kind = LiteralKind.Boolean; break;
                    default: kind = LiteralKind.Null; break;
                }

                exprArgs.Add(new LiteralExpressionNode(null)
                {
                    Kind = kind,
                    Lexeme = args[i].ToString(),
                });
            }

            return expressionEvaluator.Visit(new FunctionCallExpressionNode(null)
            {
                Name = new IdentifierExpressionNode(null) { Name = new IdentifierNode(null) { Identifier = name } },
                Arguments = exprArgs
            });
        }

        public FunctionDefinitionNode[] GetFunctions() => context.GetFunctions();

        public HLSLValue RunExpression(ExpressionNode node) => expressionEvaluator.Visit(node);

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
                            initializerValue = numeric.Cast(scalarType.Kind);
                            break;
                        case VectorTypeNode vectorType:
                            initializerValue = numeric.Cast(vectorType.Kind).BroadcastToVector(vectorType.Dimension);
                            break;
                        case MatrixTypeNode matrixType:
                            initializerValue = numeric.Cast(matrixType.Kind).BroadcastToMatrix(matrixType.FirstDimension, matrixType.SecondDimension);
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

            ScalarValue boolCondValue = condValue.Cast(ScalarType.Bool) as ScalarValue;
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

        public override void VisitWhileStatementNode(WhileStatementNode node)
        {
            executionState.PushExecutionMask();
            bool anyRunning = true;
            while (anyRunning)
            {
                anyRunning = false;

                NumericValue condValue = expressionEvaluator.Visit(node.Condition) as NumericValue;
                if (condValue is null)
                    throw new Exception("Expected a numeric value for the condition.");

                ScalarValue boolCondValue = condValue.Cast(ScalarType.Bool) as ScalarValue;
                if (boolCondValue is null)
                    throw new Exception("Expected a scalar boolean value for the condition.");

                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    bool threadCond = (bool)boolCondValue.Value.Get(threadIndex);
                    executionState.SetThreadState(threadIndex, threadCond);
                    anyRunning |= threadCond;
                }

                Visit(node.Body);
            }
            executionState.PopExecutionMask();
        }

        public override void VisitFunctionDefinitionNode(FunctionDefinitionNode node)
        {
            context.AddFunction(node.Name.GetName(), node);
        }

        public override void VisitReturnStatementNode(ReturnStatementNode node)
        {
            if (node.Expression != null)
                context.PushReturn(expressionEvaluator.Visit(node.Expression));
        }

        public override void VisitExpressionStatementNode(ExpressionStatementNode node)
        {
            expressionEvaluator.Visit(node.Expression);
        }
    }
}

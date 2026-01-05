using System;
using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.Common;
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

        // Public interface
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

        public HLSLValue EvaluateExpression(ExpressionNode node) => expressionEvaluator.Visit(node);

        // Helpers
        private Exception Error(HLSLSyntaxNode node, string message)
        {
            return new Exception($"Error at line {node.Span.Start.Line}, column {node.Span.Start.Column}: {message}");
        }

        private Exception Error(string message)
        {
            return new Exception($"Error: {message}");
        }

        private NumericValue EvaluateNumeric(ExpressionNode node, ScalarType type = ScalarType.Void)
        {
            var value = EvaluateExpression(node);
            if (value is NumericValue num)
            {
                if (type != ScalarType.Void && num.Type != type)
                    throw Error(node, $"Expected an expression of type '{PrintingUtil.GetEnumName(type)}', but got one of type '{PrintingUtil.GetEnumName(num.Type)}'.");
                return num;
            }
            else
            {
                throw Error(node, $"Expected a numeric expression, but got a {value.GetType().Name}.");
            }
        }

        private ScalarValue EvaluateScalar(ExpressionNode node, ScalarType type = ScalarType.Void)
        {
            var value = EvaluateExpression(node);
            if (value is ScalarValue num)
            {
                if (type != ScalarType.Void && num.Type != type)
                    throw Error(node, $"Expected an expression of type '{PrintingUtil.GetEnumName(type)}', but got one of type '{PrintingUtil.GetEnumName(num.Type)}'.");
                return num;
            }
            else
            {
                throw Error(node, $"Expected a scalar expression, but got a {value.GetType().Name}.");
            }
        }

        // Visitor implementation
        protected override void DefaultVisit(HLSLSyntaxNode node)
        {
            if (node is ExpressionNode expr)
                expressionEvaluator.Visit(expr);
            else
                base.DefaultVisit(node);
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
                    var initializerValue = EvaluateNumeric(initializer.Expression);

                    switch (node.Kind)
                    {
                        case ScalarTypeNode scalarType:
                            initializerValue = initializerValue.Cast(scalarType.Kind);
                            break;
                        case VectorTypeNode vectorType:
                            initializerValue = initializerValue.Cast(vectorType.Kind).BroadcastToVector(vectorType.Dimension);
                            break;
                        case MatrixTypeNode matrixType:
                            initializerValue = initializerValue.Cast(matrixType.Kind).BroadcastToMatrix(matrixType.FirstDimension, matrixType.SecondDimension);
                            break;
                        case PredefinedObjectTypeNode predefinedObjectType:
                            throw Error(node, "Invalid cast.");
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
            ScalarValue boolCondValue = EvaluateScalar(node.Condition);

            context.PushScope();
            executionState.PushExecutionMask(ExecutionScope.Conditional);
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
            {
                if (!Convert.ToBoolean(boolCondValue.Value.Get(threadIndex)))
                    executionState.DisableThread(threadIndex);
            }

            Visit(node.Body);

            context.PopScope();
            executionState.FlipExecutionMask();
            context.PushScope();

            if (node.ElseClause != null)
                Visit(node.ElseClause);

            executionState.PopExecutionMask();
            context.PopScope();
        }

        public override void VisitWhileStatementNode(WhileStatementNode node)
        {
            context.PushScope();
            executionState.PushExecutionMask(ExecutionScope.Loop);
            bool anyRunning = true;
            while (anyRunning)
            {
                anyRunning = false;

                ScalarValue boolCondValue = EvaluateScalar(node.Condition);

                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    bool threadCond = Convert.ToBoolean(boolCondValue.Value.Get(threadIndex));
                    if (!threadCond)
                        executionState.DisableThread(threadIndex);
                    anyRunning |= threadCond;
                }

                Visit(node.Body);
                executionState.ResumeSuspendedThreadsInLoop();
            }
            executionState.PopExecutionMask();
            context.PopScope();
        }

        public override void VisitDoWhileStatementNode(DoWhileStatementNode node)
        {
            context.PushScope();
            executionState.PushExecutionMask(ExecutionScope.Loop);
            bool anyRunning = true;
            while (anyRunning)
            {
                anyRunning = false;

                Visit(node.Body);
                executionState.ResumeSuspendedThreadsInLoop();

                ScalarValue boolCondValue = EvaluateScalar(node.Condition);

                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    bool threadCond = Convert.ToBoolean(boolCondValue.Value.Get(threadIndex));
                    if (!threadCond)
                        executionState.DisableThread(threadIndex);
                    anyRunning |= threadCond;
                }
            }
            executionState.PopExecutionMask();
            context.PopScope();
        }

        public override void VisitForStatementNode(ForStatementNode node)
        {
            // For loops are weird in HLSL, they declare a variable in outer scope
            if (node.Declaration != null)
                Visit(node.Declaration);
            else if (node.Initializer != null)
                Visit(node.Initializer);

            context.PushScope();
            executionState.PushExecutionMask(ExecutionScope.Loop);
            bool anyRunning = true;
            while (anyRunning)
            {
                anyRunning = false;

                ScalarValue boolCondValue = EvaluateScalar(node.Condition);

                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    bool threadCond = Convert.ToBoolean(boolCondValue.Value.Get(threadIndex));
                    if (!threadCond)
                        executionState.DisableThread(threadIndex);
                    anyRunning |= threadCond;
                }

                Visit(node.Body);
                executionState.ResumeSuspendedThreadsInLoop();

                Visit(node.Increment);
            }
            executionState.PopExecutionMask();
            context.PopScope();
        }

        public override void VisitFunctionDefinitionNode(FunctionDefinitionNode node)
        {
            context.AddFunction(node.Name.GetName(), node);
        }

        public override void VisitReturnStatementNode(ReturnStatementNode node)
        {
            if (node.Expression != null)
            {
                var returnValue = expressionEvaluator.Visit(node.Expression);

                // If we are in varying control flow, vectorize the value so we can splat each active thread.
                if (executionState.IsVaryingExecution())
                    returnValue = HLSLValueUtils.Vectorize(returnValue, executionState.GetThreadCount());

                // For each active thread, kill the thread and splat the return.
                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    if (executionState.IsThreadActive(threadIndex))
                    {
                        context.SetReturn(threadIndex, returnValue);
                        executionState.KillThreadInFunction(threadIndex);
                    }
                }
            }
        }

        public override void VisitContinueStatementNode(ContinueStatementNode node)
        {
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
            {
                if (executionState.IsThreadActive(threadIndex))
                {
                    executionState.SuspendThreadInLoop(threadIndex);
                }
            }
        }

        public override void VisitBreakStatementNode(BreakStatementNode node)
        {
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
            {
                if (executionState.IsThreadActive(threadIndex))
                {
                    executionState.KillThreadInLoop(threadIndex);
                }
            }
        }

        public override void VisitDiscardStatementNode(DiscardStatementNode node)
        {
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
            {
                if (executionState.IsThreadActive(threadIndex))
                    executionState.KillThreadGlobally(threadIndex);
            }
        }

        public override void VisitBlockNode(BlockNode node)
        {
            context.PushScope();
            VisitMany(node.Statements);
            context.PopScope();
        }

        public override void VisitExpressionStatementNode(ExpressionStatementNode node)
        {
            expressionEvaluator.Visit(node.Expression);
        }
    }
}

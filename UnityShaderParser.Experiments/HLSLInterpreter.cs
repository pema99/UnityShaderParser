using System;
using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    // TODO:
    // 
    // Statements:
    //     public class StructDefinitionNode : StatementNode
    //     public class InterfaceDefinitionNode : StatementNode
    //     public class TypedefNode : StatementNode
    //     public class StatePropertyNode : StatementNode
    //     CBuffer/TBuffer
    //     Namespace
    // 
    // Expressions:
    //     VisitMethodCallExpressionNode
    //     VisitCastExpressionNode
    //     VisitSamplerStateLiteralExpressionNode
    //     Swizzling
    // 
    // Semantics support
    // More test attributes
    // Out/Inout parameters
    // Texture/StructuredBuffer
    // Function overloading
    // Proper lvalue/rvalue handling. Think stuff like `(a) = 3;`, array writes, field writes, ternary.

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
        
        private StructValue CreateStructValue(StructTypeNode structType)
        {
            // TODO: Inheritance
            Dictionary<string, HLSLValue> members = new Dictionary<string, HLSLValue>();
            foreach (var field in structType.Fields)
            {
                foreach (var decl in field.Declarators)
                {
                    members[decl.Name] = expressionEvaluator.Visit(decl.Initializer);
                }
            }
            return new StructValue(structType, members);
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
                    HLSLValue initializerValue;
                    if (node.Kind is NumericTypeNode)
                    {
                        NumericValue numericInitializerValue = EvaluateNumeric(initializer.Expression);

                        switch (node.Kind)
                        {
                            case ScalarTypeNode scalarType:
                                initializerValue = numericInitializerValue.Cast(scalarType.Kind);
                                break;
                            case VectorTypeNode vectorType:
                                initializerValue = numericInitializerValue.Cast(vectorType.Kind)
                                    .BroadcastToVector(vectorType.Dimension);
                                break;
                            case MatrixTypeNode matrixType:
                                initializerValue = numericInitializerValue.Cast(matrixType.Kind)
                                    .BroadcastToMatrix(matrixType.FirstDimension, matrixType.SecondDimension);
                                break;

                            case GenericVectorTypeNode genVectorType:
                            case GenericMatrixTypeNode genMatrixType:
                            case StructTypeNode structType:
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        initializerValue = expressionEvaluator.Visit(initializer);
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
                            var qualNamedStruct = context.GetStruct(qualifiedNamedTypeNodeType.GetName());
                            if (qualNamedStruct == null)
                                throw Error($"Undefined named type '{qualifiedNamedTypeNodeType.GetName()}'.");
                            defaultValue = CreateStructValue(qualNamedStruct);
                            break;
                        case NamedTypeNode namedTypeNodeType:
                            var namedStruct = context.GetStruct(namedTypeNodeType.GetName());
                            if (namedStruct == null)
                                throw Error($"Undefined named type '{namedTypeNodeType.GetName()}'.");
                            defaultValue = CreateStructValue(namedStruct);
                            break;
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

        public override void VisitSwitchStatementNode(SwitchStatementNode node)
        {
            NumericValue expr = EvaluateNumeric(node.Expression);
            context.PushScope();
            executionState.PushExecutionMask(ExecutionScope.Conditional);

            foreach (var clause in node.Clauses)
            {
                executionState.PushExecutionMask(ExecutionScope.Block);

                // For each thread, check if a label matches
                bool[] pass = new bool[executionState.GetThreadCount()];
                foreach (var label in clause.Labels)
                {
                    if (label is SwitchDefaultLabelNode)
                    {
                        // Default matches everything
                        Array.Fill(pass, true);
                    }
                    else if (label is SwitchCaseLabelNode caseLabel)
                    {
                        // Do comparison for each thread
                        var labelValue = EvaluateNumeric(caseLabel.Value);
                        var cond = labelValue == expr;
                        
                        for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                        {
                            var boolCondValue = cond.GetThreadValue(threadIndex);
                            if (cond is ScalarValue sv)
                                pass[threadIndex] |= Convert.ToBoolean(boolCondValue);
                            else
                                pass[threadIndex] |= boolCondValue is object[] components && components.All(x => Convert.ToBoolean(x));
                        }
                    }
                }

                // Disable all threads which didn't pass
                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    if (!pass[threadIndex])
                        executionState.DisableThread(threadIndex);
                }

                // Run body
                foreach (var stmt in clause.Statements)
                {
                    if (stmt is BreakStatementNode)
                        break;
                    Visit(stmt);
                }

                // Disable all threads that passed for the rest of the switch statement
                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    if (pass[threadIndex])
                        executionState.KillThreadInConditional(threadIndex);
                }
                executionState.PopExecutionMask();
            }

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

        public override void VisitStructDefinitionNode(StructDefinitionNode node)
        {
            // TODO: Inline struct types
            context.AddStruct(node.StructType.Name.GetName(), node.StructType);
        }

        public override void VisitNamespaceNode(NamespaceNode node)
        {
            context.EnterNamespace(node.Name.GetName());
            base.VisitNamespaceNode(node);
            context.ExitNamespace();
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

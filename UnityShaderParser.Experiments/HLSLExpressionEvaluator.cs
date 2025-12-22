using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLExpressionEvaluator : HLSLSyntaxVisitor<HLSLValue>
    {
        protected HLSLInterpreter interpreter;
        protected HLSLInterpreterContext context;
        protected HLSLExecutionState executionState;

        protected Dictionary<string, Func<ExpressionNode[], HLSLValue>> callbacks = new Dictionary<string, Func<ExpressionNode[], HLSLValue>>();

        public HLSLExpressionEvaluator(HLSLInterpreter interpreter, HLSLInterpreterContext context, HLSLExecutionState executionState)
        {
            this.interpreter = interpreter;
            this.context = context;
            this.executionState = executionState;
        }

        public void AddCallback(string name, Func<ExpressionNode[], HLSLValue> callback) => callbacks.Add(name, callback);
        public void RemoveCallback(string name) => callbacks.Remove(name);

        public HLSLValue CallFunction(string name, params HLSLValue[] args)
        {
            // Try to invoke basic intrinsics
            if (HLSLIntrinsics.TryInvokeIntrinsic(name, args, out HLSLValue result))
                return result;

            // Now handle special intrinsics that affect or read from the execution state
            switch (name)
            {
                case "printf":
                    return HLSLIntrinsics.Printf(executionState, args);
                case "WaveGetLaneIndex":
                    return HLSLIntrinsics.WaveGetLaneIndex(executionState);
                case "WaveGetLaneCount":
                    return HLSLIntrinsics.WaveGetLaneCount(executionState);
                case "WaveIsFirstLane":
                    return HLSLIntrinsics.WaveIsFirstLane(executionState);
                case "ddx":
                    return HLSLIntrinsics.Ddx(executionState, (NumericValue)args[0]);
                case "ddy":
                    return HLSLIntrinsics.Ddy(executionState, (NumericValue)args[0]);
                case "ddx_fine":
                    return HLSLIntrinsics.DdxFine(executionState, (NumericValue)args[0]);
                case "ddy_fine":
                    return HLSLIntrinsics.DdyFine(executionState, (NumericValue)args[0]);
                default:
                    break;
            }

            FunctionDefinitionNode func = context.GetFunction(name, args);
            if (func != null)
            {
                if (args.Length != func.Parameters.Count)
                    throw new Exception($"Argument count mismatch in call to '{name}'.");

                context.PushScope();
                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    var param = func.Parameters[i];
                    var declarator = param.Declarator;
                    context.SetVariable(declarator.Name, args[i]);
                }
                interpreter.Visit(func.Body);
                context.PopScope();

                bool voidReturn = func.ReturnType is ScalarTypeNode scalarType && scalarType.Kind == ScalarType.Void;
                if (voidReturn)
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                else
                    return context.PopReturn();
            }

            throw new Exception($"Unknown function '{name}' called.");
        }

        protected override HLSLValue DefaultVisit(HLSLSyntaxNode node)
        {
            throw new InvalidOperationException($"{nameof(HLSLExpressionEvaluator)} should only be used to evaluate expressions.");
        }

        public override HLSLValue VisitQualifiedIdentifierExpressionNode(QualifiedIdentifierExpressionNode node)
        {
            if (context.TryGetVariable(node.GetName(), out var variable))
                return variable;
            else
                throw new Exception($"Unknown variable '{node.GetName()}' referenced.");
        }
        
        public override HLSLValue VisitIdentifierExpressionNode(IdentifierExpressionNode node)
        {
            if (context.TryGetVariable(node.GetName(), out var variable))
                return variable;
            else
                throw new Exception($"Unknown variable '{node.GetName()}' referenced.");
        }
        
        public override HLSLValue VisitLiteralExpressionNode(LiteralExpressionNode node)
        {
            switch (node.Kind)
            {
                case LiteralKind.String:
                    return new ScalarValue(ScalarType.String, new HLSLRegister<object>(node.Lexeme));
                case LiteralKind.Float:
                    if (float.TryParse(node.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedFloat))
                        return new ScalarValue(ScalarType.Float, new HLSLRegister<object>(parsedFloat));
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Integer:
                    if (int.TryParse(node.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedInt))
                        return new ScalarValue(ScalarType.Int, new HLSLRegister<object>(parsedInt));
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Character:
                    if (char.TryParse(node.Lexeme, out char parsedChar))
                        return new ScalarValue(ScalarType.Char, new HLSLRegister<object>(parsedChar));
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Boolean:
                    if (bool.TryParse(node.Lexeme, out bool parsedBool))
                        return new ScalarValue(ScalarType.Bool, new HLSLRegister<object>(parsedBool));
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Null:
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                default:
                    throw new Exception($"Unknown literal '{node.Lexeme}'.");
            }
        }
        
        public override HLSLValue VisitAssignmentExpressionNode(AssignmentExpressionNode node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            HLSLValue SetValue(HLSLValue value)
            {
                if (node.Left is NamedExpressionNode named)
                {
                    context.SetVariable(named.GetName(), value);
                    return value;
                }
                else if (node.Left is ElementAccessExpressionNode elem && elem.Target is NamedExpressionNode arr)
                {
                    var index = Visit(elem.Index) as ScalarValue;
                    var arrValue = (ArrayValue)context.GetVariable(arr.GetName());
                    if (index.Value.IsUniform)
                    {
                        arrValue.Values[Convert.ToInt32(index.Value.UniformValue)] = value;
                    }
                    else
                    {
                        for (int threadIndex = 0;  threadIndex < executionState.GetThreadCount() ; threadIndex++)
                        {
                            // Scattered write. First get the array element for this thread.
                            int myIndex = Convert.ToInt32(index.Value.Get(threadIndex));
                            var arrElem = arrValue.Values[myIndex] as NumericValue;

                            // Expand it to a VGPR.
                            arrElem = arrElem.Vectorize(executionState.GetThreadCount());

                            // Splat to the element for the correct thread.
                            arrValue.Values[myIndex] = arrElem.SetThreadValue(threadIndex, value);
                        }
                    }
                    return value;
                }
                // TODO: StructuredBuffer/Resource writes
                // TODO: Write to struct array
                else
                    throw new Exception("Invalid assignment");
            }

            var leftNum = left as NumericValue;
            var rightNum = right as NumericValue;
            switch (node.Operator)
            {
                case OperatorKind.Assignment:
                    return SetValue(right);
                case OperatorKind.PlusAssignment:
                    return SetValue(leftNum + rightNum);
                case OperatorKind.MinusAssignment:
                    return SetValue(leftNum - rightNum);
                case OperatorKind.MulAssignment:
                    return SetValue(leftNum * rightNum);
                case OperatorKind.DivAssignment:
                    return SetValue(leftNum / rightNum);
                case OperatorKind.ModAssignment:
                    return SetValue(leftNum % rightNum);
                case OperatorKind.ShiftLeftAssignment:
                    return SetValue(HLSLOperators.BitSHL(leftNum, rightNum));
                case OperatorKind.ShiftRightAssignment:
                    return SetValue(HLSLOperators.BitSHR(leftNum, rightNum));
                case OperatorKind.BitwiseAndAssignment:
                    return SetValue(leftNum & rightNum);
                case OperatorKind.BitwiseXorAssignment:
                    return SetValue(leftNum ^ rightNum);
                case OperatorKind.BitwiseOrAssignment:
                    return SetValue(leftNum | rightNum);
            }

            throw new Exception("Invalid assignment.");
        }
        
        public override HLSLValue VisitBinaryExpressionNode(BinaryExpressionNode node)
        {
            HLSLValue left = Visit(node.Left);
            HLSLValue right = Visit(node.Right);
            if (node.Operator == OperatorKind.Compound)
            {
                return right;
            }
            else if (left is NumericValue nl && right is NumericValue nr)
            {
                switch (node.Operator)
                {
                    case OperatorKind.LogicalOr: return HLSLOperators.BoolOr(nl, nr);
                    case OperatorKind.LogicalAnd: return HLSLOperators.BoolAnd(nl, nr);
                    case OperatorKind.BitwiseOr: return nl | nr;
                    case OperatorKind.BitwiseAnd: return nl & nr;
                    case OperatorKind.BitwiseXor: return nl ^ nr;
                    case OperatorKind.Equals: return nl == nr;
                    case OperatorKind.NotEquals: return nl != nr;
                    case OperatorKind.LessThan: return nl < nr;
                    case OperatorKind.LessThanOrEquals: return nl <= nr;
                    case OperatorKind.GreaterThan: return nl > nr;
                    case OperatorKind.GreaterThanOrEquals: return nl >= nr;
                    case OperatorKind.ShiftLeft: return HLSLOperators.BitSHL(nl, nr);
                    case OperatorKind.ShiftRight: return HLSLOperators.BitSHR(nl, nr);
                    case OperatorKind.Plus: return nl + nr;
                    case OperatorKind.Minus: return nl - nr;
                    case OperatorKind.Mul: return nl * nr;
                    case OperatorKind.Div: return nl / nr;
                    case OperatorKind.Mod: return nl % nr;
                    default:
                        throw new Exception($"Unexpected operator '{PrintingUtil.GetEnumName(node.Operator)}' in binary expression.");
                }
            }
            else
            {
                throw new Exception("Expected numeric types for binary operator.");
            }
        }
        
        public override HLSLValue VisitCompoundExpressionNode(CompoundExpressionNode node)
        {
            Visit(node.Left);
            return Visit(node.Right);
        }
        
        public override HLSLValue VisitPrefixUnaryExpressionNode(PrefixUnaryExpressionNode node)
        {
            var value = Visit(node.Expression);
            if (value is NumericValue num)
            {
                switch (node.Operator)
                {
                    case OperatorKind.Plus: return num;
                    case OperatorKind.Minus: return -num;
                    case OperatorKind.Not: return !num;
                    case OperatorKind.BitFlip: return ~num;
                    case OperatorKind.Increment when node.Expression is NamedExpressionNode named:
                        context.SetVariable(named.GetName(), num + 1);
                        return num + 1;
                    case OperatorKind.Decrement when node.Expression is NamedExpressionNode named:
                        context.SetVariable(named.GetName(), num - 1);
                        return num - 1;
                }
            }
            throw new Exception("Invalid prefix unary operation.");
        }
        
        public override HLSLValue VisitPostfixUnaryExpressionNode(PostfixUnaryExpressionNode node)
        {
            var value = Visit(node.Expression);
            if (value is NumericValue num)
            {
                switch (node.Operator)
                {
                    case OperatorKind.Increment when node.Expression is NamedExpressionNode named:
                        context.SetVariable(named.GetName(), num + 1);
                        return num;
                    case OperatorKind.Decrement when node.Expression is NamedExpressionNode named:
                        context.SetVariable(named.GetName(), num - 1);
                        return num;
                }
            }
            throw new Exception("Invalid postfix unary operation.");
        }
        
        public override HLSLValue VisitFieldAccessExpressionNode(FieldAccessExpressionNode node)
        {
            var target = Visit(node.Target) as StructValue;
            return target.Members[node.Name];
        }
        
        public override HLSLValue VisitMethodCallExpressionNode(MethodCallExpressionNode node) => throw new NotImplementedException();
        
        public override HLSLValue VisitFunctionCallExpressionNode(FunctionCallExpressionNode node)
        {
            HLSLValue[] args = new HLSLValue[node.Arguments.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Visit(node.Arguments[i]);
            }

            string name = node.Name.GetName();
            if (callbacks.ContainsKey(name))
                return callbacks[name](node.Arguments.ToArray());

            return CallFunction(name, args);
        }

        public override HLSLValue VisitNumericConstructorCallExpressionNode(NumericConstructorCallExpressionNode node)
        {
            // Get arguments, keep track of SGPR and VGPR
            NumericValue[] args = new NumericValue[node.Arguments.Count];
            int maxThreadCount = 1;
            bool anyUniform = false;
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Visit(node.Arguments[i]) as NumericValue;
                if (args[i] is null)
                    throw new Exception("Expected numeric arguments as inputs to vector constructor.");

                int argThreadCount = args[i].ThreadCount;
                maxThreadCount = Math.Max(maxThreadCount, argThreadCount);
                anyUniform |= argThreadCount == 1;
            }

            // If we are mixing VGPR and SGPR, vectorize inputs
            if (anyUniform && maxThreadCount > 1)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = args[i].Vectorize(maxThreadCount);
                }
            }

            object[][] lanes = new object[maxThreadCount][];
            for (int threadIdx = 0; threadIdx < maxThreadCount; threadIdx++)
            {
                List<object> flattened = new List<object>();
                foreach (var numeric in args)
                {
                    if (numeric is ScalarValue scalar)
                        flattened.Add(scalar.Value.Get(threadIdx));
                    if (numeric is VectorValue vector)
                        flattened.AddRange(vector.Values.Get(threadIdx));
                }
                lanes[threadIdx] = flattened.ToArray();
            }

            switch (node.Kind)
            {
                case VectorTypeNode _:
                case GenericVectorTypeNode _:
                    if (maxThreadCount == 1)
                        return new VectorValue(node.Kind.Kind, new HLSLRegister<object[]>(lanes[0]));
                    else
                        return new VectorValue(node.Kind.Kind, new HLSLRegister<object[]>(lanes));
                case MatrixTypeNode matrix:
                    if (maxThreadCount == 1)
                        return new MatrixValue(node.Kind.Kind, matrix.FirstDimension, matrix.SecondDimension, new HLSLRegister<object[]>(lanes[0]));
                    else
                        return new MatrixValue(node.Kind.Kind, matrix.FirstDimension, matrix.SecondDimension, new HLSLRegister<object[]>(lanes));
                case GenericMatrixTypeNode genMatrix:
                    var d1 = Visit(genMatrix.FirstDimension) as ScalarValue;
                    var d2 = Visit(genMatrix.SecondDimension) as ScalarValue;
                    if (maxThreadCount == 1)
                        return new MatrixValue(node.Kind.Kind, Convert.ToInt32(d1.Value), Convert.ToInt32(d2.Value), new HLSLRegister<object[]>(lanes[0]));
                    else
                        return new MatrixValue(node.Kind.Kind, Convert.ToInt32(d1.Value), Convert.ToInt32(d2.Value), new HLSLRegister<object[]>(lanes));
                default:
                    throw new Exception("Unknown numeric constructor");
            }
        }

        public override HLSLValue VisitElementAccessExpressionNode(ElementAccessExpressionNode node)
        {
            HLSLValue arr = Visit(node.Target);
            HLSLValue target = Visit(node.Index);
            if (arr is ArrayValue arrValue && target is ScalarValue targetValue)
            {
                if (targetValue.Value.IsVarying)
                {
                    object[] values = new object[executionState.GetThreadCount()];
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        var index = targetValue.Value.Get(threadIndex);
                        values[threadIndex] = arrValue.Values[Convert.ToInt32(index)];
                    }
                }
                else
                {
                    return arrValue.Values[Convert.ToInt32(targetValue.Value.UniformValue)];
                }
            }
            throw new Exception("Invalid element access.");
        }

        public override HLSLValue VisitCastExpressionNode(CastExpressionNode node) => throw new NotImplementedException();

        public override HLSLValue VisitArrayInitializerExpressionNode(ArrayInitializerExpressionNode node)
        {
            var elems = VisitMany(node.Elements);
            return new ArrayValue(elems.ToArray());
        }

        public override HLSLValue VisitTernaryExpressionNode(TernaryExpressionNode node)
        {
            var cond = Visit(node.Condition) as NumericValue;
            var left = Visit(node.TrueCase) as NumericValue;
            var right = Visit(node.FalseCase) as NumericValue;

            (left, right) = HLSLValueUtils.Promote(left, right, false);
            if (cond is MatrixValue matrix)
            {
                left = left.BroadcastToMatrix(matrix.Rows, matrix.Columns);
                right = right.BroadcastToMatrix(matrix.Rows, matrix.Columns);
            }
            else if (cond is VectorValue vector)
            {
                left = left.BroadcastToVector(vector.Size);
                right = right.BroadcastToVector(vector.Size);
            }

            object[] values = new object[executionState.GetThreadCount()];
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
            {
                if (cond is ScalarValue condScalar && left is ScalarValue leftScalar && right is ScalarValue rightScalar)
                {
                    var channel = Convert.ToBoolean(condScalar.Value.Get(threadIndex))
                        ? leftScalar.Value.Get(threadIndex)
                        : rightScalar.Value.Get(threadIndex);
                    values[threadIndex] = channel;
                }
                else if (cond is VectorValue condVector && left is VectorValue leftVector && right is VectorValue rightVector)
                {
                    object[] channels = new object[condVector.Size];
                    for (int channel = 0; channel < channels.Length; channel++)
                    {
                        channels[channel] = Convert.ToBoolean(condVector.Values.Get(threadIndex)[channel])
                            ? leftVector.Values.Get(threadIndex)[channel]
                            : rightVector.Values.Get(threadIndex)[channel];
                    }
                    values[threadIndex] = channels;
                }
                else if (cond is MatrixValue condMatrix && left is MatrixValue leftMatrix && right is MatrixValue rightMatrix)
                {
                    object[] channels = new object[condMatrix.Rows * condMatrix.Columns];
                    for (int channel = 0; channel < channels.Length; channel++)
                    {
                        channels[channel] = Convert.ToBoolean(condMatrix.Values.Get(threadIndex)[channel])
                            ? leftMatrix.Values.Get(threadIndex)[channel]
                            : rightMatrix.Values.Get(threadIndex)[channel];
                    }
                    values[threadIndex] = channels;
                }
                else
                    throw new Exception("Invalid ternary.");
            }

            if (cond is ScalarValue)
                return new ScalarValue(left.Type, new HLSLRegister<object>(values).Converge());
            if (cond is VectorValue)
                return new VectorValue(left.Type, new HLSLRegister<object[]>(values.Select(x => (object[])x).ToArray()).Converge());
            if (cond is MatrixValue finalMatrix)
                return new MatrixValue(left.Type, finalMatrix.Rows, finalMatrix.Columns, new HLSLRegister<object[]>(values.Select(x => (object[])x).ToArray()).Converge());
            throw new Exception("Invalid ternary.");
        }

        public override HLSLValue VisitSamplerStateLiteralExpressionNode(SamplerStateLiteralExpressionNode node) => throw new NotImplementedException();
    }
}

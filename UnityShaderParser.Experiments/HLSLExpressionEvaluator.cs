using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    // TODO: Put this functionality as recursive functions in interpreter rather than using SyntaxVisitor
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

        // Public API
        public void AddCallback(string name, Func<ExpressionNode[], HLSLValue> callback) => callbacks.Add(name, callback);
        public void RemoveCallback(string name) => callbacks.Remove(name);

        public HLSLValue CallFunction(string name, params HLSLValue[] args)
        {
            FunctionDefinitionNode func = context.GetFunction(name, args);
            if (func != null)
            {
                if (args.Length != func.Parameters.Count)
                    throw Error($"Argument count mismatch in call to '{name}'.");

                context.PushScope(isFunction: true);
                context.PushReturn();
                executionState.PushExecutionMask(ExecutionScope.Function);

                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    var param = func.Parameters[i];
                    var declarator = param.Declarator;
                    context.SetVariable(declarator.Name, args[i]);
                }
                interpreter.Visit(func.Body);

                executionState.PopExecutionMask();
                context.PopScope();
                
                return context.PopReturn();
            }
            
            // Try to invoke basic intrinsics
            if (HLSLIntrinsics.TryInvokeIntrinsic(name, args, out HLSLValue result))
                return result;

            // Now handle special intrinsics that affect or read from the execution state
            switch (name)
            {
                case "printf":
                case "errorf":
                    HLSLIntrinsics.Printf(executionState, args);
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                case "WaveGetLaneIndex":
                    return HLSLIntrinsics.WaveGetLaneIndex(executionState);
                case "WaveGetLaneCount":
                    return HLSLIntrinsics.WaveGetLaneCount(executionState);
                case "WaveIsFirstLane":
                    return HLSLIntrinsics.WaveIsFirstLane(executionState);
                case "WaveReadLaneAt":
                    return HLSLIntrinsics.WaveReadLaneAt(executionState, (NumericValue)args[0], (ScalarValue)args[1]);
                case "ddx":
                    return HLSLIntrinsics.Ddx(executionState, (NumericValue)args[0]);
                case "ddy":
                    return HLSLIntrinsics.Ddy(executionState, (NumericValue)args[0]);
                case "ddx_fine":
                    return HLSLIntrinsics.DdxFine(executionState, (NumericValue)args[0]);
                case "ddy_fine":
                    return HLSLIntrinsics.DdyFine(executionState, (NumericValue)args[0]);
                case "fwidth":
                    return HLSLIntrinsics.Fwidth(executionState, (NumericValue)args[0]);
                case "clip":
                    HLSLIntrinsics.Clip(executionState, (NumericValue)args[0]);
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                case "abort":
                    HLSLIntrinsics.Abort(executionState);
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                default:
                    break;
            }

            if (HLSLIntrinsics.IsUnsupportedIntrinsic(name))
                throw Error($"Intrinsic function '{name}' is not supported.");

            throw Error($"Unknown function '{name}' called.");
        }

        // Helpers
        private static Exception Error(HLSLSyntaxNode node, string message)
        {
            return new Exception($"Error at line {node.Span.Start.Line}, column {node.Span.Start.Column}: {message}");
        }

        private static Exception Error(string message)
        {
            return new Exception($"Error: {message}");
        }

        private NumericValue EvaluateNumeric(ExpressionNode node, ScalarType type = ScalarType.Void)
        {
            var value = Visit(node);
            if (value is NumericValue num)
            {
                if (type != ScalarType.Void && num.Type != type)
                    throw Error(node, $"Expected an expression of type '{PrintingUtil.GetEnumName(type)}', but got one of type '{PrintingUtil.GetEnumName(num.Type)}'.");
                return num;
            }
            else if (value is ReferenceValue refVal && refVal.Get() is NumericValue refNum)
            {
                if (type != ScalarType.Void && refNum.Type != type)
                    throw Error(node, $"Expected an expression of type '{PrintingUtil.GetEnumName(type)}', but got one of type '{PrintingUtil.GetEnumName(refNum.Type)}'.");
                return refNum;
            }
            else
            {
                throw Error(node, $"Expected a numeric expression, but got a {value.GetType().Name}.");
            }
        }

        private ScalarValue EvaluateScalar(ExpressionNode node, ScalarType type = ScalarType.Void)
        {
            var value = Visit(node);
            if (value is ScalarValue num)
            {
                if (type != ScalarType.Void && num.Type != type)
                    throw Error(node, $"Expected an expression of type '{PrintingUtil.GetEnumName(type)}', but got one of type '{PrintingUtil.GetEnumName(num.Type)}'.");
                return num;
            }
            else if (value is ReferenceValue refVal && refVal.Get() is ScalarValue refNum)
            {
                if (type != ScalarType.Void && refNum.Type != type)
                    throw Error(node, $"Expected an expression of type '{PrintingUtil.GetEnumName(type)}', but got one of type '{PrintingUtil.GetEnumName(refNum.Type)}'.");
                return refNum;
            }
            else
            {
                throw Error(node, $"Expected a scalar expression, but got a {value.GetType().Name}.");
            }
        }

        // Visit implementation
        protected override HLSLValue DefaultVisit(HLSLSyntaxNode node)
        {
            throw new Exception($"{nameof(HLSLExpressionEvaluator)} should only be used to evaluate expressions.");
        }

        public override HLSLValue VisitQualifiedIdentifierExpressionNode(QualifiedIdentifierExpressionNode node)
        {
            if (context.TryGetVariable(node.GetName(), out var variable))
            {
                if (variable is ReferenceValue reference)
                    return reference.Get();
                else
                    return variable;
            }
            else
            {
                throw Error(node, $"Use of unknown variable '{node.GetName()}'.");
            }
        }
        
        public override HLSLValue VisitIdentifierExpressionNode(IdentifierExpressionNode node)
        {
            if (context.TryGetVariable(node.GetName(), out var variable))
            {
                if (variable is ReferenceValue reference)
                    return reference.Get();
                else
                    return variable;
            }
            else
            {
                throw Error(node, $"Use of unknown variable '{node.GetName()}'.");
            }
        }
        
        public override HLSLValue VisitLiteralExpressionNode(LiteralExpressionNode node)
        {
            switch (node.Kind)
            {
                case LiteralKind.String:
                    return new ScalarValue(ScalarType.String, new HLSLRegister<object>(node.Lexeme));
                case LiteralKind.Float:
                    string floatLexeme = node.Lexeme;
                    if (floatLexeme.EndsWith('f'))
                        floatLexeme = floatLexeme.Substring(0, node.Lexeme.Length - 1);
                    if (float.TryParse(floatLexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedFloat))
                        return new ScalarValue(ScalarType.Float, new HLSLRegister<object>(parsedFloat));
                    else
                        throw Error(node, $"Invalid float literal '{node.Lexeme}'.");
                case LiteralKind.Integer:
                    if (node.Lexeme.EndsWith('u'))
                    {
                        string lexeme = node.Lexeme.Substring(0, node.Lexeme.Length - 1);
                        if (uint.TryParse(lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out uint parsedUint))
                            return new ScalarValue(ScalarType.Uint, new HLSLRegister<object>(parsedUint));
                        else if (lexeme.StartsWith("0x") && uint.TryParse(lexeme.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint parsedHexUint))
                            return new ScalarValue(ScalarType.Uint, new HLSLRegister<object>(parsedHexUint));
                    }
                    else
                    {
                        if (int.TryParse(node.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedInt))
                            return new ScalarValue(ScalarType.Int, new HLSLRegister<object>(parsedInt));
                        else if (node.Lexeme.StartsWith("0x") && int.TryParse(node.Lexeme.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int parsedHexInt))
                            return new ScalarValue(ScalarType.Int, new HLSLRegister<object>(parsedHexInt));
                    }
                    throw Error(node, $"Invalid integer literal '{node.Lexeme}'.");
                case LiteralKind.Character:
                    if (char.TryParse(node.Lexeme, out char parsedChar))
                        return new ScalarValue(ScalarType.Char, new HLSLRegister<object>(parsedChar));
                    else
                        throw Error(node, $"Invalid character literal '{node.Lexeme}'.");
                case LiteralKind.Boolean:
                    if (bool.TryParse(node.Lexeme, out bool parsedBool))
                        return new ScalarValue(ScalarType.Bool, new HLSLRegister<object>(parsedBool));
                    else
                        throw Error(node, $"Invalid boolean literal '{node.Lexeme}'.");
                case LiteralKind.Null:
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                default:
                    throw Error(node, $"Unknown literal '{node.Lexeme}'.");
            }
        }
        
        public override HLSLValue VisitAssignmentExpressionNode(AssignmentExpressionNode node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);
            right = HLSLValueUtils.CastForAssignment(left, right);

            HLSLValue SplatActiveThreadValues(HLSLValue prevValue, HLSLValue value)
            {
                HLSLValue newValue = HLSLValueUtils.Vectorize(prevValue, executionState.GetThreadCount());
                for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                {
                    if (executionState.IsThreadActive(threadIndex))
                    {
                        newValue = HLSLValueUtils.SetThreadValue(newValue, threadIndex, value);
                    }
                }
                return newValue;
            }

            // TODO: Inout/Out array
            // TODO: Inout/Out struct
            // TODO: Handle assignment in varying control flow
            HLSLValue SetValue(HLSLValue value)
            {
                if (node.Left is NamedExpressionNode named)
                {
                    string name = named.GetName();
                    if (executionState.IsVaryingExecution())
                    {
                        HLSLValue curr;
                        if (context.TryGetVariable(name, out var variable) &&
                            variable is ReferenceValue reference)
                        {
                            curr = reference.Get();
                        }
                        else
                        {
                            curr = context.GetVariable(name);
                        }
                        value = SplatActiveThreadValues(curr, value);
                    }

                    {
                        if (context.TryGetVariable(name, out var variable) &&
                            variable is ReferenceValue reference)
                        {
                            reference.Set(value);
                        }
                        else
                        {
                            context.SetVariable(name, value);
                        }
                    }
                    return value;
                }
                else if (node.Left is FieldAccessExpressionNode fieldAccess && fieldAccess.Target is NamedExpressionNode namedTarget)
                {
                    // TODO: Swizzle assign
                    string name = namedTarget.GetName();
                    var structVal = (StructValue)context.GetVariable(name);
                    if (executionState.IsVaryingExecution())
                        value = SplatActiveThreadValues(structVal.Members[fieldAccess.Name.Identifier], value);
                    structVal.Members[fieldAccess.Name.Identifier] = value;
                    return value;
                }
                else if (node.Left is ElementAccessExpressionNode elem && elem.Target is NamedExpressionNode arr)
                {
                    var index = Visit(elem.Index) as ScalarValue;
                    var arrValue = (ArrayValue)context.GetVariable(arr.GetName());
                    if (index.Value.IsUniform)
                    {
                        if (executionState.IsVaryingExecution())
                            value = SplatActiveThreadValues(arrValue.Values[Convert.ToInt32(index.Value.UniformValue)], value);
                        arrValue.Values[Convert.ToInt32(index.Value.UniformValue)] = value;
                    }
                    else
                    {
                        for (int threadIndex = 0;  threadIndex < executionState.GetThreadCount() ; threadIndex++)
                        {
                            if (!executionState.IsThreadActive(threadIndex))
                                continue;

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
                    throw Error(node, $"Invalid assignment.");
            }

            var leftNum = left as NumericValue;
            var rightNum = right as NumericValue;
            switch (node.Operator)
            {
                case OperatorKind.Assignment:
                    return SetValue(right.Copy());
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

            throw Error(node, $"Invalid assignment.");
        }
        
        public override HLSLValue VisitBinaryExpressionNode(BinaryExpressionNode node)
        {
            if (node.Operator == OperatorKind.Compound)
            {
                Visit(node.Left);
                return Visit(node.Right);
            }
            else
            {
                NumericValue nl = EvaluateNumeric(node.Left);
                NumericValue nr = EvaluateNumeric(node.Right);

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
                        throw Error(node, $"Unexpected operator '{PrintingUtil.GetEnumName(node.Operator)}' in binary expression.");
                }
            }
        }
        
        public override HLSLValue VisitCompoundExpressionNode(CompoundExpressionNode node)
        {
            Visit(node.Left);
            return Visit(node.Right);
        }
        
        public override HLSLValue VisitPrefixUnaryExpressionNode(PrefixUnaryExpressionNode node)
        {
            // Special case for negative to handle INT_MIN
            if (node.Operator == OperatorKind.Minus && node.Expression is LiteralExpressionNode literal && literal.Kind == LiteralKind.Integer)
            {
                if (int.TryParse("-" + literal.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedInt))
                    return new ScalarValue(ScalarType.Int, new HLSLRegister<object>(parsedInt));
                else if (literal.Lexeme.StartsWith("0x") && int.TryParse("-" + literal.Lexeme.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int parsedHexInt))
                    return new ScalarValue(ScalarType.Int, new HLSLRegister<object>(parsedHexInt));
            }

            var num = EvaluateNumeric(node.Expression);
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
            throw Error(node, "Invalid prefix unary expression.");
        }
        
        public override HLSLValue VisitPostfixUnaryExpressionNode(PostfixUnaryExpressionNode node)
        {
            var num = EvaluateNumeric(node.Expression);
            switch (node.Operator)
            {
                case OperatorKind.Increment when node.Expression is NamedExpressionNode named:
                    context.SetVariable(named.GetName(), num + 1);
                    return num;
                case OperatorKind.Decrement when node.Expression is NamedExpressionNode named:
                    context.SetVariable(named.GetName(), num - 1);
                    return num;
            }
            throw Error(node, "Invalid postfix unary expression.");
        }
        
        public override HLSLValue VisitFieldAccessExpressionNode(FieldAccessExpressionNode node)
        {
            // TODO: Matrix swizzle
            var target = Visit(node.Target);
            var targetStruct = target as StructValue;
            var targetNumeric = target as VectorValue;

            // Vector swizzle
            if (!(targetNumeric is null))
            {
                string swizzle = node.Name;
                if (swizzle.Length > 4)
                    throw Error($"Invalid vector swizzle '{node.Name}'.");
                object[][] perThreadSwizzle = new object[targetNumeric.ThreadCount][];
                for (int threadIndex = 0; threadIndex < perThreadSwizzle.Length; threadIndex++)
                {
                    perThreadSwizzle[threadIndex] = new object[swizzle.Length];
                    for (int component = 0; component < swizzle.Length; component++)
                    {
                        switch (swizzle[component])
                        {
                            case 'r':
                            case 'x':
                                perThreadSwizzle[threadIndex][component] = targetNumeric.Values.Get(threadIndex)[0];
                                break;
                            case 'g':
                            case 'y':
                                perThreadSwizzle[threadIndex][component] = targetNumeric.Values.Get(threadIndex)[1];
                                break;
                            case 'b':
                            case 'z':
                                perThreadSwizzle[threadIndex][component] = targetNumeric.Values.Get(threadIndex)[2];
                                break;
                            case 'a':
                            case 'w':
                                perThreadSwizzle[threadIndex][component] = targetNumeric.Values.Get(threadIndex)[3];
                                break;
                        }
                    }
                }
                if (targetNumeric.ThreadCount == 1)
                {
                    if (swizzle.Length == 1)
                        return new ScalarValue(targetNumeric.Type, HLSLValueUtils.MakeScalarSGPR(perThreadSwizzle[0][0]));
                    else
                        return new VectorValue(targetNumeric.Type, HLSLValueUtils.MakeVectorSGPR(perThreadSwizzle[0]));
                }
                else
                {
                    if (swizzle.Length == 1)
                        return new ScalarValue(targetNumeric.Type, HLSLValueUtils.MakeScalarVGPR(perThreadSwizzle.Select(x => x[0])));
                    else
                        return new VectorValue(targetNumeric.Type, HLSLValueUtils.MakeVectorVGPR(perThreadSwizzle));
                }
                return targetNumeric;
            }

            if (targetStruct is null && targetNumeric is null)
                throw Error(node.Target, "Expected a struct or numeric type for field access.");
            return targetStruct.Members[node.Name];
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
            
            // Handle out/inout parameters
            FunctionDefinitionNode func = context.GetFunction(name, args);
            if (func != null)
            {
                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    if (func.Parameters[i].Modifiers.Contains(BindingModifier.Inout) ||
                        func.Parameters[i].Modifiers.Contains(BindingModifier.Out))
                    {
                        // TODO: Other kinds of Lvalues
                        if (node.Arguments[i] is NamedExpressionNode named)
                        {
                            args[i] = context.GetReference(named.GetName());
                        }
                    }
                }
            }

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
                    throw Error(node, "Expected numeric arguments as inputs to vector constructor.");

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
                    throw Error(node, "Unknown numeric constructor.");
            }
        }

        public override HLSLValue VisitElementAccessExpressionNode(ElementAccessExpressionNode node)
        {
            HLSLValue arr = Visit(node.Target);
            ScalarValue target = EvaluateScalar(node.Index);
            if (arr is ArrayValue arrValue)
            {
                if (target.Value.IsVarying)
                {
                    HLSLValue[] values = new HLSLValue[executionState.GetThreadCount()];
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        var index = target.Value.Get(threadIndex);
                        values[threadIndex] = HLSLValueUtils.Scalarize(arrValue.Values[Convert.ToInt32(index)], threadIndex);
                    }
                    HLSLValue result = HLSLValueUtils.Vectorize(values[0], executionState.GetThreadCount());
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        result = HLSLValueUtils.SetThreadValue(result, threadIndex, values[threadIndex]);
                    }
                    return result;
                }
                else
                {
                    return arrValue.Values[Convert.ToInt32(target.Value.UniformValue)];
                }
            }
            else if (arr is VectorValue vec)
            {
                if (target.Value.IsVarying)
                {
                    HLSLValue[] values = new HLSLValue[executionState.GetThreadCount()];
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        var index = target.Value.Get(threadIndex);
                        values[threadIndex] = HLSLValueUtils.Scalarize(vec[Convert.ToInt32(index)], threadIndex);
                    }
                    HLSLValue result = HLSLValueUtils.Vectorize(values[0], executionState.GetThreadCount());
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        result = HLSLValueUtils.SetThreadValue(result, threadIndex, values[threadIndex]);
                    }
                    return result;
                }
                else
                {
                    return vec[Convert.ToInt32(target.Value.UniformValue)];
                }
            }
            else if (arr is MatrixValue mat)
            {
                if (target.Value.IsVarying)
                {
                    HLSLValue[] values = new HLSLValue[executionState.GetThreadCount()];
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        var index = target.Value.Get(threadIndex);
                        ScalarValue[] rowVec = new ScalarValue[mat.Columns];
                        for (int i = 0; i < mat.Columns; i++)
                            rowVec[i] = mat[Convert.ToInt32(index), i];
                        values[threadIndex] = HLSLValueUtils.Scalarize(VectorValue.FromScalars(rowVec), threadIndex);
                    }
                    HLSLValue result = HLSLValueUtils.Vectorize(values[0], executionState.GetThreadCount());
                    for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                    {
                        result = HLSLValueUtils.SetThreadValue(result, threadIndex, values[threadIndex]);
                    }
                    return result;
                }
                else
                {
                    ScalarValue[] rowVec = new ScalarValue[mat.Columns];
                    for (int i = 0; i < mat.Columns; i++)
                        rowVec[i] = mat[Convert.ToInt32(target.Value.UniformValue), i];
                    return VectorValue.FromScalars(rowVec);
                }
            }
            throw Error(node, "Invalid element access.");
        }

        public override HLSLValue VisitCastExpressionNode(CastExpressionNode node)
        {
            // TODO: Array cast
            // TODO: Struct initialization (0)
            var numeric = EvaluateNumeric(node.Expression);
            switch (node.Kind)
            {
                case ScalarTypeNode scalarType when numeric is ScalarValue scalar:
                    return scalar.Cast(scalarType.Kind);
                case VectorTypeNode vectorType:
                    return numeric.BroadcastToVector(vectorType.Dimension).Cast(vectorType.Kind);
                case MatrixTypeNode matrixType:
                    return numeric.BroadcastToMatrix(matrixType.FirstDimension, matrixType.SecondDimension).Cast(matrixType.Kind);
                case GenericVectorTypeNode genVectorType:
                case GenericMatrixTypeNode genMatrixType:
                default:
                    throw new NotImplementedException();
            }
        }

        public override HLSLValue VisitArrayInitializerExpressionNode(ArrayInitializerExpressionNode node)
        {
            var elems = VisitMany(node.Elements);
            return new ArrayValue(elems.ToArray());
        }

        public override HLSLValue VisitTernaryExpressionNode(TernaryExpressionNode node)
        {
            var cond = EvaluateNumeric(node.Condition);
            var left = EvaluateNumeric(node.TrueCase);
            var right = EvaluateNumeric(node.FalseCase);

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
                    throw Error(node, "Invalid ternary expression.");
            }

            if (cond is ScalarValue)
                return new ScalarValue(left.Type, new HLSLRegister<object>(values).Converge());
            if (cond is VectorValue)
                return new VectorValue(left.Type, new HLSLRegister<object[]>(values.Select(x => (object[])x).ToArray()).Converge());
            if (cond is MatrixValue finalMatrix)
                return new MatrixValue(left.Type, finalMatrix.Rows, finalMatrix.Columns, new HLSLRegister<object[]>(values.Select(x => (object[])x).ToArray()).Converge());
            throw Error(node, "Invalid ternary expression.");
        }

        public override HLSLValue VisitSamplerStateLiteralExpressionNode(SamplerStateLiteralExpressionNode node) => throw new NotImplementedException();
    }
}

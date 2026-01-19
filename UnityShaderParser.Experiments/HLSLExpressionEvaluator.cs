using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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

        protected Dictionary<string, Func<HLSLExecutionState, ExpressionNode[], HLSLValue>> callbacks = new Dictionary<string, Func<HLSLExecutionState, ExpressionNode[], HLSLValue>>();

        public HLSLExpressionEvaluator(HLSLInterpreter interpreter, HLSLInterpreterContext context, HLSLExecutionState executionState)
        {
            this.interpreter = interpreter;
            this.context = context;
            this.executionState = executionState;
        }

        // Public API
        public void AddCallback(string name, Func<HLSLExecutionState, ExpressionNode[], HLSLValue> callback) => callbacks.Add(name, callback);
        public void RemoveCallback(string name) => callbacks.Remove(name);

        public HLSLValue CallFunction(string name, params HLSLValue[] args)
        {
            FunctionDefinitionNode func = context.GetFunction(name, args);
            if (func != null)
            {
                if (args.Length != func.Parameters.Count)
                    throw Error($"Argument count mismatch in call to '{name}'.");

                // Enter namespace
                string[] namespaces = null;
                if (name.Contains("::"))
                {
                    namespaces = name.Split("::");
                    for (int i = 0; i < namespaces.Length - 1; i++)
                        context.EnterNamespace(namespaces[i]);
                }

                // Call function
                context.PushScope(isFunction: true);
                context.PushReturn();
                executionState.PushExecutionMask(ExecutionScope.Function);

                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    var param = func.Parameters[i];
                    var declarator = param.Declarator;
                    context.AddVariable(declarator.Name, HLSLValueUtils.CastForParameter(args[i], param.ParamType));
                }
                interpreter.Visit(func.Body);

                executionState.PopExecutionMask();
                context.PopScope();

                // Exit namespace
                if (namespaces != null)
                {
                    for (int i = 0; i < namespaces.Length - 1; i++)
                        context.ExitNamespace();
                }

                return context.PopReturn();
            }
            
            // Try to invoke intrinsics
            if (HLSLIntrinsics.TryInvokeIntrinsic(executionState, name, args, out HLSLValue result))
                return result;

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

        private HLSLValue SplatActiveThreadValues(HLSLValue prevValue, HLSLValue value)
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

        private HLSLValue SetValueSimpleNamed(string name, HLSLValue value)
        {
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
                    return ScalarValue.Null;
                default:
                    throw Error(node, $"Unknown literal '{node.Lexeme}'.");
            }
        }
        
        public override HLSLValue VisitAssignmentExpressionNode(AssignmentExpressionNode node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);
            right = HLSLValueUtils.CastForAssignment(left, right);

            // TODO: Inout/Out array
            // TODO: Inout/Out struct
            // TODO: Matrix column assignment
            // TODO: Nested swizzle assignment like a.zyx.yx = float2(1,2);
            // TODO: StructuredBuffer/Resource writes
            // TODO: Write to struct array
            HLSLValue SetValue(HLSLValue value)
            {
                if (node.Left is NamedExpressionNode named)
                {
                    return SetValueSimpleNamed(named.GetName(), value);
                }
                else if (node.Left is FieldAccessExpressionNode fieldAccess && fieldAccess.Target is NamedExpressionNode namedTarget)
                {
                    string name = namedTarget.GetName();
                    var variable = context.GetVariable(name);

                    if (variable is VectorValue vec)
                    {
                        return SetValueSimpleNamed(name, vec.SwizzleAssign(fieldAccess.Name, (NumericValue)value));
                    }
                    else if (variable is ReferenceValue refVec && refVec.Get() is VectorValue vecInner)
                    {
                        return SetValueSimpleNamed(name, vecInner.SwizzleAssign(fieldAccess.Name, (NumericValue)value));
                    }
                    else
                    {
                        var structVal = (StructValue)variable;
                        if (executionState.IsVaryingExecution())
                            value = SplatActiveThreadValues(structVal.Members[fieldAccess.Name.Identifier], value);
                        structVal.Members[fieldAccess.Name.Identifier] = value;
                        return value;
                    }
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
                    SetValueSimpleNamed(named.GetName(), num + 1);
                    return num + 1;
                case OperatorKind.Decrement when node.Expression is NamedExpressionNode named:
                    SetValueSimpleNamed(named.GetName(), num - 1);
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
                    SetValueSimpleNamed(named.GetName(), num + 1);
                    return num;
                case OperatorKind.Decrement when node.Expression is NamedExpressionNode named:
                    SetValueSimpleNamed(named.GetName(), num - 1);
                    return num;
            }
            throw Error(node, "Invalid postfix unary expression.");
        }
        
        public override HLSLValue VisitFieldAccessExpressionNode(FieldAccessExpressionNode node)
        {
            // TODO: Matrix swizzle
            var target = Visit(node.Target);
            var targetStruct = target as StructValue;
            var targetNumeric = target as NumericValue;

            // Vector swizzle
            if (!(targetNumeric is null))
            {
                if (node.Name.Identifier.Length > 4)
                    throw Error($"Invalid vector swizzle '{node.Name.Identifier}'.");
                if (targetNumeric is VectorValue vec)
                    return vec.Swizzle(node.Name);
                else
                    return targetNumeric.BroadcastToVector(1).Swizzle(node.Name);
            }

            if (targetStruct is null && targetNumeric is null)
                throw Error(node.Target, "Expected a struct or numeric type for field access.");
            return targetStruct.Members[node.Name];
        }
        
        public override HLSLValue VisitMethodCallExpressionNode(MethodCallExpressionNode node)
        {
            // TODO: Methods on builtin types like Texture2D
            // TODO: Namespace handling
            var target = Visit(node.Target);
            if (target is StructValue str)
            {
                HLSLValue[] args = new HLSLValue[node.Arguments.Count];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = Visit(node.Arguments[i]);
                }

                var methods = context.GetStruct(str.Name).Methods;
                foreach (var method in methods)
                {
                    if (method.Name.GetName() == node.Name.Identifier)
                    {
                        // Handle out/inout parameters
                        for (int i = 0; i < args.Length; i++)
                        {
                            if (method.Parameters[i].Modifiers.Contains(BindingModifier.Inout) ||
                                method.Parameters[i].Modifiers.Contains(BindingModifier.Out))
                            {
                                // TODO: Other kinds of Lvalues
                                if (node.Arguments[i] is NamedExpressionNode named)
                                {
                                    args[i] = context.GetReference(named.GetName());
                                }
                            }
                        }

                        if (args.Length != method.Parameters.Count)
                            throw Error($"Argument count mismatch in call to '{method}'.");

                        context.PushScope(isFunction: true);
                        context.PushReturn();
                        executionState.PushExecutionMask(ExecutionScope.Function);

                        // Push local state as references
                        if (!method.Modifiers.Contains(BindingModifier.Static))
                        {
                            foreach (string field in str.Members.Keys)
                            {
                                context.AddVariable(field, new ReferenceValue(
                                    () => str.Members[field],
                                    val => str.Members[field] = val));
                            }
                        }

                        for (int i = 0; i < method.Parameters.Count; i++)
                        {
                            var param = method.Parameters[i];
                            var declarator = param.Declarator;
                            context.AddVariable(declarator.Name, HLSLValueUtils.CastForParameter(args[i], param.ParamType));
                        }
                        interpreter.Visit(((FunctionDefinitionNode)method).Body);

                        executionState.PopExecutionMask();
                        context.PopScope();

                        return context.PopReturn();
                    }
                }

                throw Error(node, $"Couldn't find method '{node.Name.Identifier}' on type '{str.Name}'.");
            }

            throw Error(node, $"Can't call method '{node.Name.Identifier}' on non-struct type.");
        }
        
        public override HLSLValue VisitFunctionCallExpressionNode(FunctionCallExpressionNode node)
        {
            HLSLValue[] args = new HLSLValue[node.Arguments.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Visit(node.Arguments[i]);
            }

            string name = node.Name.GetName();
            if (callbacks.ContainsKey(name))
                return callbacks[name](executionState, node.Arguments.ToArray());
            
            // Handle out/inout parameters
            FunctionDefinitionNode func = context.GetFunction(name, args);
            for (int i = 0; i < args.Length; i++)
            {
                bool isInoutIntrinsic = HLSLIntrinsics.IsIntrinsicInoutParameter(name, i);

                bool isInoutUser =
                    func != null &&
                    (func.Parameters[i].Modifiers.Contains(BindingModifier.Inout) ||
                    func.Parameters[i].Modifiers.Contains(BindingModifier.Out));

                if (isInoutIntrinsic || isInoutUser)
                {
                    // TODO: Other kinds of Lvalues
                    if (node.Arguments[i] is NamedExpressionNode named)
                    {
                        args[i] = context.GetReference(named.GetName());
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
                for (int i = 0; i < flattened.Count; i++)
                {
                    flattened[i] = HLSLValueUtils.CastNumeric(node.Kind.Kind, flattened[i]);
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

            return HLSLIntrinsics.Select(cond, left, right);
        }

        public override HLSLValue VisitSamplerStateLiteralExpressionNode(SamplerStateLiteralExpressionNode node) => throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLExpressionEvaluator : HLSLSyntaxVisitor<HLSLValue>
    {
        protected HLSLInterpreterContext context;
        protected HLSLExecutionState executionState;

        public HLSLExpressionEvaluator(HLSLInterpreterContext context, HLSLExecutionState executionState)
        {
            this.context = context;
            this.executionState = executionState;
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
        
        public override HLSLValue VisitAssignmentExpressionNode(AssignmentExpressionNode node) => throw new NotImplementedException();
        
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
        
        public override HLSLValue VisitCompoundExpressionNode(CompoundExpressionNode node) => throw new NotImplementedException();
        
        public override HLSLValue VisitPrefixUnaryExpressionNode(PrefixUnaryExpressionNode node) => throw new NotImplementedException();
        
        public override HLSLValue VisitPostfixUnaryExpressionNode(PostfixUnaryExpressionNode node) => throw new NotImplementedException();
        
        public override HLSLValue VisitFieldAccessExpressionNode(FieldAccessExpressionNode node) => throw new NotImplementedException();
        
        public override HLSLValue VisitMethodCallExpressionNode(MethodCallExpressionNode node) => throw new NotImplementedException();
        
        public override HLSLValue VisitFunctionCallExpressionNode(FunctionCallExpressionNode node)
        {
            HLSLValue[] args = new HLSLValue[node.Arguments.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Visit(node.Arguments[i]);
            }

            string name = node.Name.GetName();

            // Try to invoke basic intrinsics
            if (HLSLIntrinsics.TryInvokeIntrinsic(name, args, out HLSLValue result))
                return result;

            // Now handle special intrinsics that affect or read from the execution state
            switch (name)
            {
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

            throw new Exception($"Unknown function '{node.Name.GetName()}' called.");
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

                int argThreadCount = HLSLValueUtils.GetThreadCount(args[i]);
                maxThreadCount = Math.Max(maxThreadCount, argThreadCount);
                anyUniform |= argThreadCount == 1;
            }

            // If we are mixing VGPR and SGPR, vectorize inputs
            if (anyUniform && maxThreadCount > 1)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = HLSLValueUtils.Vectorize(args[i], maxThreadCount);
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

        public override HLSLValue VisitElementAccessExpressionNode(ElementAccessExpressionNode node) => throw new NotImplementedException();

        public override HLSLValue VisitCastExpressionNode(CastExpressionNode node) => throw new NotImplementedException();

        public override HLSLValue VisitArrayInitializerExpressionNode(ArrayInitializerExpressionNode node) => throw new NotImplementedException();

        public override HLSLValue VisitTernaryExpressionNode(TernaryExpressionNode node) => throw new NotImplementedException();

        public override HLSLValue VisitSamplerStateLiteralExpressionNode(SamplerStateLiteralExpressionNode node) => throw new NotImplementedException();
    }
}

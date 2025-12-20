using System;
using System.Collections.Generic;
using System.Globalization;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLExpressionEvaluator : HLSLSyntaxVisitor<HLSLValue>
    {
        protected HLSLInterpreterContext context;

        public HLSLExpressionEvaluator(HLSLInterpreterContext context)
        {
            this.context = context;
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
                    return new ScalarValue(ScalarType.String, node.Lexeme);
                case LiteralKind.Float:
                    if (float.TryParse(node.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedFloat))
                        return new ScalarValue(ScalarType.Float, parsedFloat);
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Integer:
                    if (int.TryParse(node.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedInt))
                        return new ScalarValue(ScalarType.Int, parsedInt);
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Character:
                    if (char.TryParse(node.Lexeme, out char parsedChar))
                        return new ScalarValue(ScalarType.Char, parsedChar);
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Boolean:
                    if (bool.TryParse(node.Lexeme, out bool parsedBool))
                        return new ScalarValue(ScalarType.Bool, parsedBool);
                    else
                        throw new Exception($"Failed to parse float '{node.Lexeme}'.");
                case LiteralKind.Null:
                    return new ScalarValue(ScalarType.Void, null);
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

            if (HLSLIntrinsics.TryInvokeIntrinsic(node.Name.GetName(), args, out HLSLValue result))
                return result;

            throw new Exception($"Unknown function '{node.Name.GetName()}' called.");
        }

        public override HLSLValue VisitNumericConstructorCallExpressionNode(NumericConstructorCallExpressionNode node)
        {
            NumericValue[] args = new NumericValue[node.Arguments.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Visit(node.Arguments[i]) as NumericValue;
                if (args[i] is null)
                    throw new Exception("Expected numeric arguments as inputs to vector constructor.");
            }

            List<object> values = new List<object>();
            foreach (var numeric in args)
            {
                if (numeric is ScalarValue scalar)
                    values.Add(scalar.Value);
                if (numeric is VectorValue vector)
                    values.AddRange(vector.Values);
            }

            switch (node.Kind)
            {
                case VectorTypeNode _:
                case GenericVectorTypeNode _:
                    return new VectorValue(node.Kind.Kind, values.ToArray());
                case MatrixTypeNode matrix:
                    return new MatrixValue(node.Kind.Kind, matrix.FirstDimension, matrix.SecondDimension, values.ToArray());
                case GenericMatrixTypeNode genMatrix:
                    var d1 = Visit(genMatrix.FirstDimension) as ScalarValue;
                    var d2 = Visit(genMatrix.SecondDimension) as ScalarValue;
                    return new MatrixValue(node.Kind.Kind, Convert.ToInt32(d1.Value), Convert.ToInt32(d2.Value), values.ToArray());
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

﻿
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.PreProcessor
{
    using HLSLToken = Token<TokenKind>;

    internal class ConstExpressionEvaluator
    {
        public static bool EvaluateConstExprTokens(List<HLSLToken> exprTokens, out List<string> diagnostics)
        {
            HLSLParser localParser = new HLSLParser(exprTokens);
            var expr = localParser.ParseExpression();

            if (localParser.Diagnostics.Count > 0)
            {
                diagnostics = new() { "Failed to evaluated const expression in preprocessor directive." };
                return false;
            }

            var self = new ConstExpressionEvaluator();
            bool result = self.EvaluateConstExpr(expr) != 0;
            diagnostics = self.diagnostics;
            return result;
        }

        private List<string> diagnostics = new();

        private static bool ToBool(long i) => i != 0;
        private static long ToNum(bool i) => i ? 1 : 0;

        public long EvaluateConstExpr(ExpressionNode node)
        {
            switch (node)
            {
                case LiteralExpressionNode literalExpr:
                    switch (literalExpr.Kind)
                    {
                        case LiteralKind.Integer:
                            return int.Parse(literalExpr.Lexeme);
                        case LiteralKind.Character:
                            return char.Parse(literalExpr.Lexeme);
                        default:
                            diagnostics.Add($"Literals of type '{literalExpr.Kind}' are not supported in constant expressions.");
                            return 0;
                    }
                case BinaryExpressionNode binExpr:
                    long left = EvaluateConstExpr(binExpr.Left);
                    long right = EvaluateConstExpr(binExpr.Right);
                    switch (binExpr.Operator)
                    {
                        case OperatorKind.LogicalOr: return ToNum(ToBool(left) || ToBool(right));
                        case OperatorKind.LogicalAnd: return ToNum(ToBool(left) && ToBool(right));
                        case OperatorKind.BitwiseOr: return left | right;
                        case OperatorKind.BitwiseAnd: return left & right;
                        case OperatorKind.BitwiseXor: return left ^ right;
                        case OperatorKind.Equals: return ToNum(left == right);
                        case OperatorKind.NotEquals: return ToNum(left != right);
                        case OperatorKind.LessThan: return ToNum(left < right);
                        case OperatorKind.LessThanOrEquals: return ToNum(left <= right);
                        case OperatorKind.GreaterThan: return ToNum(left > right);
                        case OperatorKind.GreaterThanOrEquals: return ToNum(left >= right);
                        case OperatorKind.ShiftLeft: return left << (int)right;
                        case OperatorKind.ShiftRight: return left >> (int)right;
                        case OperatorKind.Plus: return left + right;
                        case OperatorKind.Minus: return left - right;
                        case OperatorKind.Mul: return left * right;
                        case OperatorKind.Div: return left / right;
                        case OperatorKind.Mod: return left % right;
                        default:
                            diagnostics.Add($"Binary operators of type '{binExpr.Operator}' are not supported in constant expressions.");
                            return 0;
                    }
                case PrefixUnaryExpressionNode unExpr:
                    long unary = EvaluateConstExpr(unExpr.Expression);
                    switch (unExpr.Operator)
                    {
                        case OperatorKind.Not: return ToNum(!ToBool(unary));
                        case OperatorKind.BitFlip: return ~unary;
                        default:
                            diagnostics.Add($"Unary operators of type '{unExpr.Operator}' are not supported in constant expressions.");
                            return 0;
                    }
                default:
                    diagnostics.Add($"Illegal expression type '{node.GetType().Name}' found in constant expression.");
                    return 0;
            }
        }
    }
}
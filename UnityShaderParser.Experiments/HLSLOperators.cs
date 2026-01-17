using System;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public static class HLSLOperators
    {
        #region Helpers for binary and unary operators
        private static object Add(object left, object right)
        {
            switch (left)
            {
                case int x: return x + (int)right;
                case uint x: return x + (uint)right;
                case float x: return x + (float)right;
                case double x: return x + (double)right;
                case bool x: return ((x ? 1 : 0) + ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object Mul(object left, object right)
        {
            switch (left)
            {
                case int x: return x * (int)right;
                case uint x: return x * (uint)right;
                case float x: return x * (float)right;
                case double x: return x * (double)right;
                case bool x: return ((x ? 1 : 0) * ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object Sub(object left, object right)
        {
            switch (left)
            {
                case int x: return x - (int)right;
                case uint x: return x - (uint)right;
                case float x: return x - (float)right;
                case double x: return x - (double)right;
                case bool x: return ((x ? 1 : 0) - ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object Div(object left, object right)
        {
            switch (left)
            {
                case int x: return x / (int)right;
                case uint x: return x / (uint)right;
                case float x: return x / (float)right;
                case double x: return x / (double)right;
                case bool x: return ((x ? 1 : 0) / ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object Mod(object left, object right)
        {
            switch (left)
            {
                case int x: return x % (int)right;
                case uint x: return x % (uint)right;
                case float x: return x % (float)right;
                case double x: return x % (double)right;
                case bool x: return ((x ? 1 : 0) % ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object BitSHL(object left, object right)
        {
            switch (left)
            {
                case int x: return x << (int)right;
                case uint x: return x << (int)(uint)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BitSHR(object left, object right)
        {
            switch (left)
            {
                case int x: return x >> (int)right;
                case uint x: return x >> (int)(uint)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BitAnd(object left, object right)
        {
            switch (left)
            {
                case int x: return x & (int)right;
                case uint x: return x & (uint)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BitOr(object left, object right)
        {
            switch (left)
            {
                case int x: return x | (int)right;
                case uint x: return x | (uint)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BitXor(object left, object right)
        {
            switch (left)
            {
                case int x: return x ^ (int)right;
                case uint x: return x ^ (uint)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BoolAnd(object left, object right)
        {
            switch (left)
            {
                case bool x: return x && (bool)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BoolOr(object left, object right)
        {
            switch (left)
            {
                case bool x: return x || (bool)right;
                default: throw new InvalidOperationException();
            }
        }

        private static object BitNot(object left)
        {
            switch (left)
            {
                case int x: return ~x;
                case uint x: return ~x;
                default: throw new InvalidOperationException();
            }
        }

        private static object Negate(object left)
        {
            switch (left)
            {
                case int x: return -x;
                case uint x: return (int)-x;
                case float x: return -x;
                case double x: return -x;
                case bool x: return (-(x ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object BoolNegate(object left)
        {
            switch (left)
            {
                case bool x: return !x;
                default: throw new InvalidOperationException();
            }
        }

        private static object Less(object left, object right)
        {
            switch (left)
            {
                case int x: return x < (int)right;
                case uint x: return x < (uint)right;
                case float x: return x < (float)right;
                case double x: return x < (double)right;
                case bool x: return ((x ? 1 : 0) < ((bool)right ? 1 : 0));
                default: throw new InvalidOperationException();
            }
        }

        private static object Greater(object left, object right)
        {
            switch (left)
            {
                case int x: return x > (int)right;
                case uint x: return x > (uint)right;
                case float x: return x > (float)right;
                case double x: return x > (double)right;
                case bool x: return ((x ? 1 : 0) > ((bool)right ? 1 : 0));
                default: throw new InvalidOperationException();
            }
        }

        private static object LessEqual(object left, object right)
        {
            switch (left)
            {
                case int x: return x <= (int)right;
                case uint x: return x <= (uint)right;
                case float x: return x <= (float)right;
                case double x: return x <= (double)right;
                case bool x: return ((x ? 1 : 0) <= ((bool)right ? 1 : 0));
                default: throw new InvalidOperationException();
            }
        }

        private static object GreaterEqual(object left, object right)
        {
            switch (left)
            {
                case int x: return x >= (int)right;
                case uint x: return x >= (uint)right;
                case float x: return x >= (float)right;
                case double x: return x >= (double)right;
                case bool x: return ((x ? 1 : 0) >= ((bool)right ? 1 : 0));
                default: throw new InvalidOperationException();
            }
        }

        private static object Equal(object left, object right)
        {
            return left.Equals(right);
        }

        private static object NotEqual(object left, object right)
        {
            return !left.Equals(right);
        }
        #endregion

        #region Binary and unary operators
        public static NumericValue Add(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Add);
        }

        public static NumericValue Mul(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Mul);
        }

        public static NumericValue Sub(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Sub);
        }

        public static NumericValue Div(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Div);
        }

        public static NumericValue Mod(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Mod);
        }

        public static NumericValue BitSHL(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, true);
            return HLSLValueUtils.Map2(left, right, BitSHL);
        }

        public static NumericValue BitSHR(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, true);
            return HLSLValueUtils.Map2(left, right, BitSHR);
        }

        public static NumericValue BitAnd(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, true);
            return HLSLValueUtils.Map2(left, right, BitAnd);
        }

        public static NumericValue BitOr(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, true);
            return HLSLValueUtils.Map2(left, right, BitOr);
        }

        public static NumericValue BitXor(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, true);
            return HLSLValueUtils.Map2(left, right, BitXor);
        }

        public static NumericValue BoolAnd(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, BoolAnd);
        }

        public static NumericValue BoolOr(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, BoolOr);
        }

        public static NumericValue Negate(NumericValue left)
        {
            var res = left.Map(Negate);
            if (res.Type == ScalarType.Uint)
                return res.Cast(ScalarType.Int);
            return res;
        }

        public static NumericValue BoolNegate(NumericValue left)
        {
            return left.Map(BoolNegate);
        }

        public static NumericValue BitNot(NumericValue left)
        {
            return left.Map(BitNot);
        }

        public static NumericValue Less(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Less).Cast(ScalarType.Bool);
        }

        public static NumericValue Greater(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Greater).Cast(ScalarType.Bool);
        }

        public static NumericValue LessEqual(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, LessEqual).Cast(ScalarType.Bool);
        }

        public static NumericValue GreaterEqual(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, GreaterEqual).Cast(ScalarType.Bool);
        }

        public static NumericValue Equal(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, Equal).Cast(ScalarType.Bool);
        }

        public static NumericValue NotEqual(NumericValue left, NumericValue right)
        {
            (left, right) = HLSLValueUtils.Promote(left, right, false);
            return HLSLValueUtils.Map2(left, right, NotEqual).Cast(ScalarType.Bool);
        }
        #endregion
    }
}

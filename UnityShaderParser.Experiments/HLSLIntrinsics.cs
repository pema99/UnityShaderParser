using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public static class HLSLIntrinsics
    {
        #region Helpers
        private static void CheckArity(string name, HLSLValue[] args, int arity)
        {
            if (arity >= 0 && args.Length != arity)
                throw new ArgumentException($"Expected {arity} arguments for builtin '{name}', but got '{args.Length}'.");
        }

        private static void CheckNumeric(string name, HLSLValue[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                bool isNumeric = args[i] is NumericValue;
                bool isRefNumeric = args[i] is ReferenceValue refVal && refVal.Get() is NumericValue;
                if (!isNumeric && !isRefNumeric)
                    throw new ArgumentException($"Expected argument in position '{i}' to builtin '{name}' to be a numeric value.");
            }
        }
        
        private static VectorValue CastToVector(NumericValue v, int size = 1)
        {
            if (v is VectorValue vec)
                return vec;
            else
                return v.BroadcastToVector(size);
        }

        private static MatrixValue CastToMatrix(NumericValue v, int rows = 1, int cols = 1)
        {
            if (v is MatrixValue mat)
                return mat;
            else
                return v.BroadcastToMatrix(rows, cols);
        }
        
        private static NumericValue ToFloatLike(NumericValue value)
        {
            if (HLSLValueUtils.IsFloat(value.Type))
                return value;
            return value.Cast(ScalarType.Float);
        }
        
        private static object Min(object left, object right)
        {
            switch (left)
            {
                case int x: return Math.Min(x, (int)right);
                case uint x: return Math.Min(x, (uint)right);
                case float x: return Math.Min(x, (float)right);
                case double x: return Math.Min(x, (double)right);
                case bool x: return Math.Min((x ? 1 : 0), ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }

        private static object Max(object left, object right)
        {
            switch (left)
            {
                case int x: return Math.Max(x, (int)right);
                case uint x: return Math.Max(x, (uint)right);
                case float x: return Math.Max(x, (float)right);
                case double x: return Math.Max(x, (double)right);
                case bool x: return Math.Max((x ? 1 : 0), ((bool)right ? 1 : 0)) != 0;
                default: throw new InvalidOperationException();
            }
        }
        #endregion

        #region Basic intrinsics
        private delegate HLSLValue BasicIntrinsic(HLSLValue[] args);

        private static (int arity, BasicIntrinsic) N1(Func<NumericValue, NumericValue> fn) =>
            (1, args => fn((NumericValue)args[0]));

        private static (int arity, BasicIntrinsic) N2(Func<NumericValue, NumericValue, NumericValue> fn) =>
            (2, args => fn((NumericValue)args[0], (NumericValue)args[1]));

        private static (int arity, BasicIntrinsic) N3(Func<NumericValue, NumericValue, NumericValue, NumericValue> fn) =>
            (3, args => fn((NumericValue)args[0], (NumericValue)args[1], (NumericValue)args[2]));

        private static readonly HashSet<string> unsupportedIntrinsics = new HashSet<string>()
        {
            "CheckAccessFullyMapped",
            "EvaluateAttributeCentroid",
            "EvaluateAttributeAtSample",
            "EvaluateAttributeSnapped",
            "GetRenderTargetSampleCount",
            "GetRenderTargetSamplePosition",
            "Process2DQuadTessFactorsAvg",
            "Process2DQuadTessFactorsMax",
            "Process2DQuadTessFactorsMin",
            "ProcessIsolineTessFactors",
            "ProcessQuadTessFactorsAvg",
            "ProcessQuadTessFactorsMax",
            "ProcessQuadTessFactorsMin",
            "ProcessTriTessFactorsAvg",
            "ProcessTriTessFactorsMax",
            "ProcessTriTessFactorsMin",
        };

        public static bool IsUnsupportedIntrinsic(string name) => unsupportedIntrinsics.Contains(name);

        private static readonly Dictionary<string, (int arity, BasicIntrinsic fn)> basicIntrinsics = new Dictionary<string, (int arity, BasicIntrinsic fn)>()
        {
            // Need multi-group support:
            //AllMemoryBarrierWithGroupSync
            //DeviceMemoryBarrierWithGroupSync
            //GroupMemoryBarrierWithGroupSync

            // Need atomic support:
            //InterlockedAdd
            //InterlockedAnd
            //InterlockedCompareExchange
            //InterlockedCompareStore
            //InterlockedExchange
            //InterlockedMax
            //InterlockedMin
            //InterlockedOr
            //InterlockedXor

            // Need texture support:
            //tex1/2/3D/CUBE
            //tex1/2/3D/CUBEbias
            //tex1/2/3D/CUBEgrad
            //tex1/2/3D/CUBElod
            //tex1/2/3D/CUBEproj

            ["abs"] = N1(Abs),
            ["acos"] = N1(Acos),
            ["all"] = N1(All),
            ["any"] = N1(Any),
            ["asdouble"] = N2(Asdouble),
            ["asfloat"] = N1(Asfloat),
            ["asin"] = N1(Asin),
            ["asint"] = N1(Asint),
            ["atan"] = N1(Atan),
            ["atan2"] = N2(Atan2),
            ["ceil"] = N1(Ceil),
            ["clamp"] = N3(Clamp),
            ["cos"] = N1(Cos),
            ["cosh"] = N1(Cosh),
            ["countbits"] = N1(Countbits),
            ["cross"] = N2(Cross),
            ["D3DCOLORtoUBYTE4"] = N1(D3DCOLORtoUBYTE4),
            ["degrees"] = N1(Degrees),
            ["determinant"] = N1(Determinant),
            ["distance"] = N2(Distance),
            ["dot"] = N2(Dot),
            ["dst"] = N2(Dst),
            ["exp"] = N1(Exp),
            ["exp2"] = N1(Exp2),
            ["f16tof32"] = N1(F16tof32),
            ["f32tof16"] = N1(F32tof16),
            ["faceforward"] = N3(Faceforward),
            ["firstbithigh"] = N1(Firstbithigh),
            ["firstbitlow"] = N1(Firstbitlow),
            ["floor"] = N1(Floor),
            ["fma"] = N3(Fma),
            ["fmod"] = N2(Fmod),
            ["frac"] = N1(Frac),
            ["isfinite"] = N1(Isfinite),
            ["isinf"] = N1(Isinf),
            ["isnan"] = N1(Isnan),
            ["ldexp"] = N2(Ldexp),
            ["length"] = N1(Length),
            ["lerp"] = N3(Lerp),
            ["lit"] = N3(Lit),
            ["log"] = N1(Log),
            ["log10"] = N1(Log10),
            ["log2"] = N1(Log2),
            ["mad"] = N3(Mad),
            ["max"] = N2(Max),
            ["min"] = N2(Min),
            ["msad4"] = N3(Msad4),
            ["mul"] = N2(Mul),
            ["noise"] = N1(Noise),
            ["normalize"] = N1(Normalize),
            ["pow"] = N2(Pow),
            ["radians"] = N1(Radians),
            ["rcp"] = N1(Rcp),
            ["reflect"] = N2(Reflect),
            ["refract"] = N3(Refract),
            ["reversebits"] = N1(Reversebits),
            ["round"] = N1(Round),
            ["rsqrt"] = N1(Rsqrt),
            ["saturate"] = N1(Saturate),
            ["sign"] = N1(Sign),
            ["sin"] = N1(Sin),
            ["sinh"] = N1(Sinh),
            ["smoothstep"] = N3(Smoothstep),
            ["sqrt"] = N1(Sqrt),
            ["step"] = N2(Step),
            ["tan"] = N1(Tan),
            ["tanh"] = N1(Tanh),
            ["transpose"] = N1(Transpose),
            ["trunc"] = N1(Trunc),

            // Inout intrinsics
            ["modf"] = (2, args => Modf((NumericValue)args[0], (ReferenceValue)args[1])),
            ["frexp"] = (2, args => Frexp((NumericValue)args[0], (ReferenceValue)args[1])),
            ["asuint"] = (-1, args =>
            {
                if (args.Length == 0 || args.Length > 3)
                    throw new ArgumentException($"Expected 1-3 arguments for builtin 'asuint', but got '{args.Length}'.");

                if (args.Length > 1)
                {
                    Asuint((NumericValue)args[0], (ReferenceValue)args[1], (ReferenceValue)args[2]);
                    return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                }
                return Asuint((NumericValue)args[0]);
            }),
            ["sincos"] = (3, args =>
            {
                Sincos((NumericValue)args[0], (ReferenceValue)args[1], (ReferenceValue)args[2]);
                return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
            }),
        };

        public static bool IsIntrinsicInoutParameter(string name, int paramIndex)
        {
            switch (name)
            {
                case "modf" when paramIndex == 1: return true;
                case "frexp" when paramIndex == 1: return true;
                case "sincos" when paramIndex == 1 || paramIndex == 2: return true;
                case "asuint" when paramIndex == 1 || paramIndex == 2: return true;
                default: return false;
            }
        }

        public static bool TryInvokeIntrinsic(string name, HLSLValue[] args, out HLSLValue result)
        {
            if (!basicIntrinsics.TryGetValue(name, out var entry))
            {
                result = null;
                return false;
            }

            CheckArity(name, args, entry.arity);
            CheckNumeric(name, args);

            result = entry.fn(args);
            return true;
        }

        // https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-lerp
        public static NumericValue Lerp(NumericValue x, NumericValue y, NumericValue s)
        {
            return x * (1 - s) + y * s;
        }
        
        public static NumericValue Exp(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Exp(Convert.ToSingle(val)));
        }
        
        public static NumericValue Exp2(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Pow(2.0f, Convert.ToSingle(val)));
        }

        public static NumericValue Abs(NumericValue x)
        {
            if (HLSLValueUtils.IsInt(x.Type))
                return x.Map(val => Math.Abs(Convert.ToInt32(val)));

            if (HLSLValueUtils.IsUint(x.Type))
                return x;

            return x.Map(val => Math.Abs(Convert.ToSingle(val)));
        }
        
        public static NumericValue Acos(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Acos(Convert.ToSingle(val)));
        }
        
        public static NumericValue All(NumericValue x)
        {
            var scalars = x.ToScalars();
            if (scalars.Length == 0) return true;

            var acc = scalars[0].Cast(ScalarType.Bool);
            foreach (var scalar in scalars)
            {
                acc = HLSLOperators.BoolAnd(acc, scalar.Cast(ScalarType.Bool));
            }
            return acc;
        }

        public static NumericValue Any(NumericValue x)
        {
            var scalars = x.ToScalars();
            if (scalars.Length == 0) return true;

            var acc = scalars[0].Cast(ScalarType.Bool);
            foreach (var scalar in scalars)
            {
                acc = HLSLOperators.BoolOr(acc, scalar.Cast(ScalarType.Bool));
            }
            return acc;
        }

        public static NumericValue Asdouble(NumericValue lowbits, NumericValue highbits)
        {
            ScalarValue lowbitsScalar = (ScalarValue)lowbits.Cast(ScalarType.Uint);
            ScalarValue highbitsScalar = (ScalarValue)highbits.Cast(ScalarType.Uint);

            NumericValue result = 0.0;
            return result.MapThreads((_, threadIndex) =>
            {
                var low = (uint)lowbitsScalar.GetThreadValue(threadIndex);
                var high = (uint)highbitsScalar.GetThreadValue(threadIndex);
                return BitConverter.ToDouble(BitConverter.GetBytes(low).Concat(BitConverter.GetBytes(high)).ToArray());
            });
        }

        public static NumericValue Asfloat(NumericValue x)
        {
            return x.Map(y =>
            {
                byte[] bytes;
                if (HLSLValueUtils.IsUint(x.Type))
                    bytes = BitConverter.GetBytes(Convert.ToUInt32(y));
                else if (HLSLValueUtils.IsInt(x.Type))
                    bytes = BitConverter.GetBytes(Convert.ToInt32(y));
                else
                    return y;
                return BitConverter.ToSingle(bytes);
            }).Cast(ScalarType.Float);
        }

        public static NumericValue Asint(NumericValue x)
        {
            return x.Map(y =>
            {
                byte[] bytes;
                if (HLSLValueUtils.IsFloat(x.Type))
                    bytes = BitConverter.GetBytes(Convert.ToSingle(y));
                else if (HLSLValueUtils.IsUint(x.Type))
                    bytes = BitConverter.GetBytes(Convert.ToUInt32(y));
                else
                    return y;
                return BitConverter.ToInt32(bytes);
            }).Cast(ScalarType.Int);
        }

        public static NumericValue Asuint(NumericValue x)
        {
            return x.Map(y =>
            {
                byte[] bytes;
                if (HLSLValueUtils.IsFloat(x.Type))
                    bytes = BitConverter.GetBytes(Convert.ToSingle(y));
                else if (HLSLValueUtils.IsInt(x.Type))
                    bytes = BitConverter.GetBytes(Convert.ToInt32(y));
                else
                    return y;
                return BitConverter.ToUInt32(bytes);
            }).Cast(ScalarType.Uint);
        }

        public static NumericValue Asin(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Asin(Convert.ToSingle(val)));
        }

        public static NumericValue Atan(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Atan(Convert.ToSingle(val)));
        }
        
        public static NumericValue Atan2(NumericValue y, NumericValue x)
        {
            x = ToFloatLike(x);
            y = ToFloatLike(y);

            return 
            Select(x > 0, Atan(y / x),
            Select(HLSLOperators.BoolAnd(x < 0, y >= 0), Atan(y / x) + MathF.PI,
            Select(HLSLOperators.BoolAnd(x < 0, y < 0), Atan(y / x) - MathF.PI,
            Select(HLSLOperators.BoolAnd(x == 0, y > 0), MathF.PI / 2.0f,
            Select(HLSLOperators.BoolAnd(x == 0, y < 0), -MathF.PI / 2.0f,
            0)))));
        }
        
        public static NumericValue Select(NumericValue cond, NumericValue a, NumericValue b)
        {
            (cond, a) = HLSLValueUtils.Promote(cond, a, false);
            (a, b) = HLSLValueUtils.Promote(a, b, false);

            return cond.MapThreads((condVal, threadIndex) =>
            {
                if (condVal is object[] arr)
                {
                    object[] result = new object[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (Convert.ToBoolean(arr[i]))
                            result[i] = ((object[])a.GetThreadValue(threadIndex))[i];
                        else
                            result[i] = ((object[])b.GetThreadValue(threadIndex))[i];
                    }
                    return result;
                }
                
                if (Convert.ToBoolean(cond.GetThreadValue(threadIndex)))
                    return a.GetThreadValue(threadIndex);
                else
                    return b.GetThreadValue(threadIndex);
            });
        }
        
        public static NumericValue Sqrt(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Sqrt(Convert.ToSingle(val)));
        }

        public static NumericValue Step(NumericValue y, NumericValue x)
        {
            return Select(x >= y, 1, 0);
        }
        
        public static NumericValue Ceil(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Ceiling(Convert.ToSingle(val)));
        }
        
        public static NumericValue Cos(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Cos(Convert.ToSingle(val)));
        }
        
        public static NumericValue Cosh(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Cosh(Convert.ToSingle(val)));
        }

        public static NumericValue Countbits(NumericValue x)
        {
            var u = x.Cast(ScalarType.Uint);
            return u.Map(y =>
            {
                uint bits = Convert.ToUInt32(y);
                uint count = 0;
                for (int i = 0; i < 32; i++)
                {
                    if ((bits & (1 << i)) != 0)
                        count++;
                }
                return count;
            });
        }

        public static NumericValue Cross(NumericValue a, NumericValue b)
        {
            var vecA = ToFloatLike(a).BroadcastToVector(3);
            var vecB = ToFloatLike(b).BroadcastToVector(3);
            return VectorValue.FromScalars(
                vecA.y * vecB.z - vecA.z * vecB.y,
                vecA.z * vecB.x - vecA.x * vecB.z,
                vecA.x * vecB.y - vecA.y * vecB.x
            );
        }

        public static NumericValue D3DCOLORtoUBYTE4(NumericValue x)
        {
            var vec = CastToVector(Trunc(x.Cast(ScalarType.Float) * 255.001953f));
            return VectorValue.FromScalars(vec.z, vec.y, vec.x, vec.w);
        }

        public static NumericValue Degrees(NumericValue x)
        {
            return x * (180f / MathF.PI);
        }

        public static NumericValue Determinant(NumericValue x)
        {
            x = ToFloatLike(x);
            var mat = CastToMatrix(x);

            if (mat.Rows == 2) return Det2x2(mat);
            if (mat.Rows == 3) return Det3x3(mat);
            if (mat.Rows == 4) return Det4x4(mat);
            return mat[0, 0];

            // Determinant of a 2x2 matrix
            NumericValue Det2x2(MatrixValue m)
            {
                return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            }

            // Determinant of a 3x3 matrix
            NumericValue Det3x3(MatrixValue m)
            {
                return
                    m[0, 0] * (m[1, 1] * m[2, 2] - m[1,2] * m[2,1]) -
                    m[0, 1] * (m[1, 0] * m[2, 2] - m[1,2] * m[2,0]) +
                    m[0, 2] * (m[1, 0] * m[2, 1] - m[1,1] * m[2,0]);
            }

            // Determinant of a 4x4 matrix
            NumericValue Det4x4(MatrixValue m)
            {
                return
                    m[0,0] * (
                        m[1,1] * (m[2,2] * m[3,3] - m[2,3] * m[3,2]) -
                        m[1,2] * (m[2,1] * m[3,3] - m[2,3] * m[3,1]) +
                        m[1,3] * (m[2,1] * m[3,2] - m[2,2] * m[3,1])
                    ) -
                    m[0,1] * (
                        m[1,0] * (m[2,2] * m[3,3] - m[2,3] * m[3,2]) -
                        m[1,2] * (m[2,0] * m[3,3] - m[2,3] * m[3,0]) +
                        m[1,3] * (m[2,0] * m[3,2] - m[2,2] * m[3,0])
                    ) +
                    m[0,2] * (
                        m[1,0] * (m[2,1] * m[3,3] - m[2,3] * m[3,1]) -
                        m[1,1] * (m[2,0] * m[3,3] - m[2,3] * m[3,0]) +
                        m[1,3] * (m[2,0] * m[3,1] - m[2,1] * m[3,0])
                    ) -
                    m[0,3] * (
                        m[1,0] * (m[2,1] * m[3,2] - m[2,2] * m[3,1]) -
                        m[1,1] * (m[2,0] * m[3,2] - m[2,2] * m[3,0]) +
                        m[1,2] * (m[2,0] * m[3,1] - m[2,1] * m[3,0])
                    );
            }
        }
        
        public static NumericValue Distance(NumericValue x, NumericValue y)
        {
            return Length(y - x);
        }
        
        public static NumericValue Sign(NumericValue x)
        {
            return Select(x == 0, 0, Select(x > 0, 1, -1)).Cast(ScalarType.Int);
        }

        public static NumericValue F32tof16(NumericValue x)
        {
            x = x.Cast(ScalarType.Float);
            object[] perThreadValue = new object[x.ThreadCount];
            for (int threadIndex = 0; threadIndex < x.ThreadCount; threadIndex++)
            {
                uint f = BitConverter.ToUInt32(BitConverter.GetBytes(Convert.ToSingle(x.GetThreadValue(threadIndex))));

                // Extract sign, exponent, and mantissa from float32
                uint sign = (f >> 31) & 0x1;
                uint exponent = (f >> 23) & 0xFF;
                uint mantissa = f & 0x7FFFFF;

                uint half;

                // Handle special cases
                if (exponent == 0xFF) // Inf or NaN
                {
                    half = (sign << 15) | 0x7C00 | (mantissa != 0 ? 0x200u : 0);
                }
                else if (exponent == 0) // Zero or denormalized
                {
                    half = sign << 15; // Just preserve sign, flush denorms to zero
                }
                else
                {
                    // Rebias exponent from float32 (bias 127) to float16 (bias 15)
                    int newExp = (int)(exponent) - 127 + 15;

                    if (newExp >= 31) // Overflow to infinity
                    {
                        half = (sign << 15) | 0x7C00;
                    }
                    else if (newExp <= 0) // Underflow to zero
                    {
                        half = sign << 15;
                    }
                    else
                    {
                        // Normal case: construct half float
                        half = (sign << 15) | ((uint)(newExp) << 10) | (mantissa >> 13);
                    }
                }

                perThreadValue[threadIndex] = half;
            }

            if (x.ThreadCount == 1)
                return new ScalarValue(ScalarType.Uint, HLSLValueUtils.MakeScalarSGPR(perThreadValue[0]));
            else
                return new ScalarValue(ScalarType.Uint, HLSLValueUtils.MakeScalarVGPR(perThreadValue));
        }


        public static NumericValue F16tof32(NumericValue x)
        {
            x = x.Cast(ScalarType.Uint);
            object[] perThreadValue = new object[x.ThreadCount];
            for (int threadIndex = 0; threadIndex < x.ThreadCount; threadIndex++)
            {
                uint half = Convert.ToUInt32(x.GetThreadValue(threadIndex));

                // Extract sign, exponent, and mantissa from float16
                uint sign = (half >> 15) & 0x1;
                uint exponent = (half >> 10) & 0x1F;
                uint mantissa = half & 0x3FF;

                uint f;

                if (exponent == 0x1F) // Inf or NaN
                {
                    // Preserve inf/nan, expand mantissa
                    f = (sign << 31) | 0x7F800000 | (mantissa << 13);
                }
                else if (exponent == 0) // Zero or denormalized
                {
                    if (mantissa == 0) // Zero
                    {
                        f = sign << 31;
                    }
                    else // Denormalized - convert to normalized float32
                    {
                        // Find the leading 1 bit
                        exponent = 1;
                        while ((mantissa & 0x400) == 0)
                        {
                            mantissa <<= 1;
                            exponent--;
                        }
                        mantissa &= 0x3FF; // Remove leading 1

                        // Rebias exponent from float16 (bias 15) to float32 (bias 127)
                        uint newExp = exponent + 127 - 15;
                        f = (sign << 31) | (newExp << 23) | (mantissa << 13);
                    }
                }
                else // Normal case
                {
                    // Rebias exponent from float16 (bias 15) to float32 (bias 127)
                    uint newExp = exponent + 127 - 15;
                    f = (sign << 31) | (newExp << 23) | (mantissa << 13);
                }

                perThreadValue[threadIndex] = BitConverter.ToSingle(BitConverter.GetBytes(f));
            }

            if (x.ThreadCount == 1)
                return new ScalarValue(ScalarType.Float, HLSLValueUtils.MakeScalarSGPR(perThreadValue[0]));
            else
                return new ScalarValue(ScalarType.Float, HLSLValueUtils.MakeScalarVGPR(perThreadValue));
        }

        public static NumericValue Faceforward(NumericValue n, NumericValue i, NumericValue ng)
        {
            return -n * Sign(Dot(i, ng));
        }

        public static NumericValue Firstbithigh(NumericValue x)
        {
            if (x.Type != ScalarType.Int && x.Type != ScalarType.Uint)
                x = x.Cast(ScalarType.Uint);

            return x.Map(y =>
            {
                // Gets the location of the first set bit starting from the highest order bit and working downward, per component.
                if (y is int signed)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        int bitPos = 1 << (31 - i);
                        // For a negative signed integer, firstbithigh returns the position of the first bit set to 0.
                        if (signed < 0)
                        {
                            if ((signed & bitPos) == 0)
                                return (31 - i);
                        }
                        else if ((signed & bitPos) != 0)
                            return (31 - i);
                    }
                    return -1;
                }
                else
                {
                    uint unsigned = (uint)y;
                    for (int i = 0; i < 32; i++)
                    {
                        uint bitPos = 1u << (31 - i);
                        if ((unsigned & bitPos) != 0)
                            return (31 - i);
                    }
                    return 0xFFFFFFFFu;
                }
            });
        }

        public static NumericValue Firstbitlow(NumericValue x)
        {
            x = x.Cast(ScalarType.Uint);

            return x.Map(y =>
            {
                // Returns the location of the first set bit starting from the lowest order bit and working upward, per component.
                uint unsigned = (uint)y;
                for (int i = 0; i < 32; i++)
                {
                    uint bitPos = 1u << i;
                    if ((unsigned & bitPos) != 0)
                        return i;
                }
                return 0xFFFFFFFFu;
            });
        }

        public static NumericValue Floor(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Floor(Convert.ToSingle(val)));
        }

        public static NumericValue Fma(NumericValue a, NumericValue b, NumericValue c)
        {
            return a * b + c;
        }
        
        public static NumericValue Fmod(NumericValue a, NumericValue b)
        {
            a = ToFloatLike(a);
            b = ToFloatLike(b);
            var c = Frac(Abs(a/b))*Abs(b);
            return Select(a < 0, -c, c);
        }

        public static NumericValue Frac(NumericValue x)
        {
            return Abs(ToFloatLike(x).Map(val => Convert.ToSingle(val) % 1.0f));
        }
        
        public static NumericValue Isnan(NumericValue x)
        {
            return ToFloatLike(x).Map(val => float.IsNaN(Convert.ToSingle(val))).Cast(ScalarType.Bool);
        }
        
        public static NumericValue Isfinite(NumericValue x)
        {
            return ToFloatLike(x).Map(val => float.IsFinite(Convert.ToSingle(val))).Cast(ScalarType.Bool);
        }
        
        public static NumericValue Isinf(NumericValue x)
        {
            return ToFloatLike(x).Map(val => float.IsInfinity(Convert.ToSingle(val))).Cast(ScalarType.Bool);
        }

        public static NumericValue Ldexp(NumericValue x, NumericValue exp)
        {
            return x * Exp2(exp);
        }

        public static NumericValue Lit(NumericValue nDotL, NumericValue nDotH, NumericValue m)
        {
            nDotL = ToFloatLike(nDotL);
            nDotH = ToFloatLike(nDotH);
            m = ToFloatLike(m);

            var diffuse = Max(nDotL, 0.0f);
            var specular = Select(nDotL > 0.0f, Pow(Max(nDotH, 0.0f), m), 0.0f);

            ScalarValue one = (ScalarValue)(NumericValue)1.0f;
            return VectorValue.FromScalars(one, (ScalarValue)diffuse, (ScalarValue)specular, one);
        }

        public static NumericValue Log(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Log(Convert.ToSingle(val)));
        }

        public static NumericValue Log10(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Log(Convert.ToSingle(val)) / MathF.Log(10));
        }

        public static NumericValue Log2(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Log(Convert.ToSingle(val)) / MathF.Log(2));
        }

        public static NumericValue Mad(NumericValue m, NumericValue a, NumericValue b)
        {
            return m * a + b;
        }

        public static NumericValue Noise(NumericValue x)
        {
            return 1; // Not supported since SM2.0
        }
        
        public static NumericValue Pow(NumericValue x, NumericValue y)
        {
            x = ToFloatLike(x);
            y = ToFloatLike(y);
            (x, y) = HLSLValueUtils.Promote(x, y, false);
            return HLSLValueUtils.Map2(x, y, (fx, fy) => MathF.Pow(Convert.ToSingle(fx), Convert.ToSingle(fy)));
        }

        public static NumericValue Radians(NumericValue x)
        {
            return x / (180f / MathF.PI);
        }
        
        public static NumericValue Rcp(NumericValue x)
        {
            return 1.0f / x;
        }
        
        public static NumericValue Reflect(NumericValue i, NumericValue n)
        {
            return i - 2.0f * n * Dot(n,i);
        }

        public static NumericValue Refract(NumericValue i, NumericValue n, NumericValue eta)
        {
            var cosi = Dot(-i, n);
            var cost2 = 1.0f - eta * eta * (1.0f - cosi*cosi);
            var t = eta*i + ((eta*cosi - Sqrt(Abs(cost2))) * n);
            return t * (cost2 > 0);
        }

        public static NumericValue Reversebits(NumericValue x)
        {
            return x.Cast(ScalarType.Uint).Map(v =>
            {
                uint a = Convert.ToUInt32(v);
                uint r = 0;
                for (int i = 0; i < 32; i++)
                {
                    r <<= 1;
                    r |= (a & 1);
                    a >>= 1;
                }
                return r;
            });
        }

        public static NumericValue Round(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Round(Convert.ToSingle(val), MidpointRounding.AwayFromZero));
        }
        
        public static NumericValue Rsqrt(NumericValue x)
        {
            return 1.0f / Sqrt(x);
        }
        
        public static NumericValue Sin(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Sin(Convert.ToSingle(val)));
        }
        
        public static NumericValue Sinh(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Sinh(Convert.ToSingle(val)));
        }
        
        // https://developer.download.nvidia.com/cg/smoothstep.html
        public static NumericValue Smoothstep(NumericValue a, NumericValue b, NumericValue x)
        {
            var t = Saturate((x - a)/(b - a));
            return t*t*(3.0f - (2.0f*t));
        }
        
        public static NumericValue Tan(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Tan(Convert.ToSingle(val)));
        }
        
        public static NumericValue Tanh(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Tanh(Convert.ToSingle(val)));
        }

        public static NumericValue Transpose(NumericValue m)
        {
            var mat = CastToMatrix(m);
            var reg = mat.Values.Map(x =>
            {
                object[] values = new object[x.Length];
                for (int col = 0; col < mat.Columns; col++)
                {
                    for (int row = 0; row < mat.Rows; row++)
                    {
                        values[row * mat.Columns + col] = x[col * mat.Rows + row];
                    }
                }
                return values;
            });
            return new MatrixValue(mat.Type, mat.Columns, mat.Rows, reg);
        }
        
        public static NumericValue Trunc(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Truncate(Convert.ToSingle(val)));
        }
        
        public static NumericValue Dot(NumericValue x, NumericValue y)
        {
            (x, y) = HLSLValueUtils.Promote(x, y, false);
            VectorValue vx = CastToVector(x);
            VectorValue vy = CastToVector(y);

            var scalarsX = vx.ToScalars();
            var scalarsY = vy.ToScalars();

            var accum = scalarsX[0] - scalarsX[0];
            for (int i = 0; i < scalarsX.Length; i++)
            {
                accum += scalarsX[i] * scalarsY[i];
            }
            return accum;
        }

        public static NumericValue Dst(NumericValue src0, NumericValue src1)
        {
            var src0vec = CastToVector(ToFloatLike(src0));
            var src1vec = CastToVector(ToFloatLike(src1));
            return VectorValue.FromScalars((ScalarValue)(NumericValue)1.0f, src0vec.y * src1vec.y, src0vec.z, src1vec.w);
        }

        public static NumericValue Length(NumericValue x)
        {
            return Sqrt(Dot(x, x));
        }

        public static NumericValue Normalize(NumericValue x)
        {
            return x / Length(x);
        }

        public static NumericValue Min(NumericValue x, NumericValue y)
        {
            (x, y) = HLSLValueUtils.Promote(x, y, false);
            return HLSLValueUtils.Map2(x, y, Min);
        }

        public static NumericValue Msad4(NumericValue reference, NumericValue source, NumericValue accum)
        {
            ScalarValue referenceU = (ScalarValue)reference.Cast(ScalarType.Uint); // uint
            VectorValue sourceU = CastToVector(source.Cast(ScalarType.Uint), 2); // uint2
            VectorValue accumU = CastToVector(accum.Cast(ScalarType.Uint), 4); // uint4

            // Unpack reference bytes
            var r0 = referenceU & 0xFF;
            var r1 = (HLSLOperators.BitSHR(referenceU, 8)) & 0xFF;
            var r2 = (HLSLOperators.BitSHR(referenceU, 16)) & 0xFF;
            var r3 = (HLSLOperators.BitSHR(referenceU, 24)) & 0xFF;

            // Unpack source.x bytes
            var s0 = sourceU.x & 0xFF;
            var s1 = (HLSLOperators.BitSHR(sourceU.x, 8)) & 0xFF;
            var s2 = (HLSLOperators.BitSHR(sourceU.x, 16)) & 0xFF;
            var s3 = (HLSLOperators.BitSHR(sourceU.x, 24)) & 0xFF;

            // Unpack source.y bytes
            var t0 = sourceU.y & 0xFF;
            var t1 = (HLSLOperators.BitSHR(sourceU.y, 8)) & 0xFF;
            var t2 = (HLSLOperators.BitSHR(sourceU.y, 16)) & 0xFF;
            var t3 = (HLSLOperators.BitSHR(sourceU.y, 24)) & 0xFF;

            ScalarValue x = (ScalarValue)(
                  Abs(r0.Cast(ScalarType.Int) - s0.Cast(ScalarType.Int))
                + Abs(r1.Cast(ScalarType.Int) - s1.Cast(ScalarType.Int))
                + Abs(r2.Cast(ScalarType.Int) - s2.Cast(ScalarType.Int))
                + Abs(r3.Cast(ScalarType.Int) - s3.Cast(ScalarType.Int)));
            ScalarValue y = (ScalarValue)(
                  Abs(r0.Cast(ScalarType.Int) - s1.Cast(ScalarType.Int))
                + Abs(r1.Cast(ScalarType.Int) - s2.Cast(ScalarType.Int))
                + Abs(r2.Cast(ScalarType.Int) - s3.Cast(ScalarType.Int))
                + Abs(r3.Cast(ScalarType.Int) - t0.Cast(ScalarType.Int)));
            ScalarValue z = (ScalarValue)(
                  Abs(r0.Cast(ScalarType.Int) - s2.Cast(ScalarType.Int))
                + Abs(r1.Cast(ScalarType.Int) - s3.Cast(ScalarType.Int))
                + Abs(r2.Cast(ScalarType.Int) - t0.Cast(ScalarType.Int))
                + Abs(r3.Cast(ScalarType.Int) - t1.Cast(ScalarType.Int)));
            ScalarValue w = (ScalarValue)(
                  Abs(r0.Cast(ScalarType.Int) - s3.Cast(ScalarType.Int))
                + Abs(r1.Cast(ScalarType.Int) - t0.Cast(ScalarType.Int))
                + Abs(r2.Cast(ScalarType.Int) - t1.Cast(ScalarType.Int))
                + Abs(r3.Cast(ScalarType.Int) - t2.Cast(ScalarType.Int)));

            return accum + VectorValue.FromScalars(x,y,z,w);
        }

        // https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-mul#type-description
        public static NumericValue Mul(NumericValue x, NumericValue y)
        {
            bool xIsScalar = x is ScalarValue;
            bool yIsScalar = y is ScalarValue;

            // Covers case 1, 2, 3, 4, 7 - scalar mul
            if (xIsScalar || yIsScalar) 
                return x * y;

            bool xIsVector = x is VectorValue;
            bool yIsVector = y is VectorValue;

            // Case 5 - dot
            if (xIsVector && yIsVector)
                return Dot(x, y);

            bool xIsMatrix = x is MatrixValue;
            bool yIsMatrix = y is MatrixValue;

            // Case 6 - row vector mul
            if (xIsVector && yIsMatrix)
            {
                var xVec = (VectorValue)x;
                var yMat = (MatrixValue)y;
                ScalarValue[] result = new ScalarValue[yMat.Columns];
                for (int col = 0; col < yMat.Columns; col++)
                {
                    ScalarValue[] colVec = new ScalarValue[yMat.Rows];
                    for (int row = 0; row < yMat.Rows; row++)
                        colVec[row] = yMat[row, col];
                    result[col] = (ScalarValue)Dot(VectorValue.FromScalars(colVec), xVec);
                }
                return VectorValue.FromScalars(result);
            }

            // Case 8 - column vector mul
            if (xIsMatrix && yIsVector)
            {
                var xMat = (MatrixValue)x;
                var yVec = (VectorValue)y;
                ScalarValue[] result = new ScalarValue[xMat.Rows];
                for (int row = 0; row < xMat.Rows; row++)
                {
                    ScalarValue[] rowVec = new ScalarValue[xMat.Columns];
                    for (int col = 0; col < xMat.Columns; col++)
                        rowVec[col] = xMat[row, col];
                    result[row] = (ScalarValue)Dot(VectorValue.FromScalars(rowVec), yVec);
                }
                return VectorValue.FromScalars(result);
            }

            // Case 9 - full matmul
            if (xIsMatrix && yIsMatrix)
            {
                var xMat = (MatrixValue)x;
                var yMat = (MatrixValue)y;
                ScalarValue[] result = new ScalarValue[xMat.Rows*yMat.Columns];
                for (int row = 0; row < xMat.Rows; row++)
                {
                    for (int col = 0; col < yMat.Columns; col++)
                    {
                        // get row 'row' from xMat
                        ScalarValue[] rowVec = new ScalarValue[xMat.Columns];
                        for (int i = 0; i < xMat.Columns; i++)
                            rowVec[i] = xMat[row, i];

                        // get col 'col' from yMat
                        ScalarValue[] colVec = new ScalarValue[yMat.Rows];
                        for (int i = 0; i < yMat.Rows; i++)
                            colVec[i] = yMat[i, col];

                        result[row * xMat.Rows + col] = (ScalarValue)Dot(VectorValue.FromScalars(rowVec), VectorValue.FromScalars(colVec));
                    }
                }
                return MatrixValue.FromScalars(xMat.Rows, yMat.Columns, result);
            }

            return x;
        }

        public static NumericValue Max(NumericValue x, NumericValue y)
        {
            (x, y) = HLSLValueUtils.Promote(x, y, false);
            return HLSLValueUtils.Map2(x, y, Max);
        }

        public static NumericValue Clamp(NumericValue x, NumericValue min, NumericValue max)
        {
            return Min(Max(x, min), max);
        }

        public static NumericValue Saturate(NumericValue x)
        {
            return Clamp(x, 0f, 1f);
        }
        #endregion

        #region Intrinsics with out parameters
        public static NumericValue Modf(NumericValue x, ReferenceValue i)
        {
            i.Set(Trunc(x));
            return x - (NumericValue)i.Get();
        }

        public static void Sincos(NumericValue a, ReferenceValue s, ReferenceValue c)
        {
            s.Set(Sin(a));
            c.Set(Cos(a));
        }

        public static NumericValue Frexp(NumericValue x, ReferenceValue e)
        {
            var bits = Asuint(x);
            var biased_exp = (HLSLOperators.BitSHR(bits, 23) & 0xFF).Cast(ScalarType.Int);
            var mantissa_bits = (bits & 0x807FFFFFu) | (126u << 23);
            e.Set(Select(x == 0, 0, biased_exp - 126));
            return Select(x == 0, x, Asfloat(mantissa_bits));
        }

        public static void Asuint(NumericValue value, ReferenceValue lowbits, ReferenceValue highbits)
        {
            var d = (ScalarValue)value.Cast(ScalarType.Double);
            if (d.IsUniform)
            {
                var bytes = BitConverter.GetBytes(Convert.ToDouble(d.Value.UniformValue));
                lowbits.Set((ScalarValue)BitConverter.ToUInt32(bytes, 0));
                highbits.Set((ScalarValue)BitConverter.ToUInt32(bytes, 4));
                return;
            }

            ScalarValue retLow = 0u;
            retLow = (ScalarValue)retLow.Vectorize(d.ThreadCount);
            ScalarValue retHigh = 0u;
            retHigh = (ScalarValue)retHigh.Vectorize(d.ThreadCount);
            for (int threadIndex = 0; threadIndex < d.ThreadCount; threadIndex++)
            {
                var bytes = BitConverter.GetBytes(Convert.ToDouble(d.Value.Get(threadIndex)));
                uint low = BitConverter.ToUInt32(bytes, 0);
                uint high = BitConverter.ToUInt32(bytes, 4);
                retLow = (ScalarValue)retLow.SetThreadValue(threadIndex, low);
                retHigh = (ScalarValue)retHigh.SetThreadValue(threadIndex, high);
            }
            lowbits.Set(retLow);
            highbits.Set(retHigh);
        }
        #endregion

        #region Special intrinsics that touch execution state
        public static void Printf(HLSLExecutionState executionState, HLSLValue[] args)
        {
            if (args.Length > 0)
            {
                int maxThreadCount = args.Max(x => x.ThreadCount);

                bool scalarizeLoop = executionState.IsVaryingExecution() || maxThreadCount > 1;
                int numThreads = scalarizeLoop ? Math.Max(maxThreadCount, executionState.GetThreadCount()) : 1;

                for (int threadIndex = 0; threadIndex < numThreads; threadIndex++)
                {
                    if (scalarizeLoop && !executionState.IsThreadActive(threadIndex))
                        continue;

                    string formatString = args[0].ToString();
                    StringBuilder sb = new StringBuilder();
                    if (scalarizeLoop)
                        sb.Append($"[Thread {threadIndex}] ");
                    int argCounter = 1;
                    for (int j = 0; j < formatString.Length; j++)
                    {
                        if (formatString[j] == '%')
                        {
                            j++;
                            var arg = args[argCounter++];
                            if (arg is NumericValue num)
                            {
                                sb.Append(Convert.ToString(num.Scalarize(threadIndex), CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                sb.Append(Convert.ToString(arg, CultureInfo.InvariantCulture));
                            }
                        }
                        else
                        {
                            sb.Append(formatString[j]);
                        }
                    }
                    Console.WriteLine(sb.ToString());
                }
            }
        }

        public static ScalarValue WaveGetLaneIndex(HLSLExecutionState executionState)
        {
            return new ScalarValue(ScalarType.Uint, HLSLValueUtils.MakeScalarVGPR(Enumerable.Range(0, executionState.GetThreadCount())));
        }

        public static ScalarValue WaveGetLaneCount(HLSLExecutionState executionState)
        {
            return (ScalarValue)executionState.GetThreadCount();
        }

        public static ScalarValue WaveIsFirstLane(HLSLExecutionState executionState)
        {
            var perLaneIsFirst = new bool[executionState.GetThreadCount()];
            for (int threadIdx = 0; threadIdx < executionState.GetThreadCount(); threadIdx++)
            {
                if (executionState.IsThreadActive(threadIdx))
                {
                    perLaneIsFirst[threadIdx] = true;
                    break;
                }
            }
            return new ScalarValue(ScalarType.Bool, HLSLValueUtils.MakeScalarVGPR(perLaneIsFirst));
        }

        public static NumericValue WaveReadLaneAt(HLSLExecutionState executionState, NumericValue expr, ScalarValue laneIndex)
        {
            if (laneIndex.IsUniform)
                return expr.Scalarize(Convert.ToInt32(laneIndex.Value.UniformValue));

            object[] perLaneValue = new object[laneIndex.ThreadCount];
            for (int threadIndex = 0; threadIndex < perLaneValue.Length; threadIndex++)
            {
                perLaneValue[threadIndex] = expr.Scalarize(Convert.ToInt32(laneIndex.GetThreadValue(threadIndex))).GetThreadValue(0);
            }
            if (expr is ScalarValue)
                return new ScalarValue(expr.Type, HLSLValueUtils.MakeScalarVGPR(perLaneValue));
            if (expr is VectorValue)
                return new VectorValue(expr.Type, HLSLValueUtils.MakeVectorVGPR((object[][])perLaneValue));
            if (expr is MatrixValue mat)
                return new MatrixValue(expr.Type, mat.Rows, mat.Columns, HLSLValueUtils.MakeVectorVGPR((object[][])perLaneValue));
            throw new InvalidOperationException();
        }

        public static NumericValue DdxFine(HLSLExecutionState executionState, NumericValue val)
        {
            if (val.IsUniform)
                return val - val;

            return val.MapThreads((_, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                int offset = (x % 2 == 0) ? 1 : -1;

                var me = val.Scalarize(threadIndex);
                var other = val.Scalarize(executionState.GetThreadIndex(x + offset, y));
                return ((other - me) * offset).GetThreadValue(0);
            });
        }

        public static NumericValue DdyFine(HLSLExecutionState executionState, NumericValue val)
        {
            if (val.IsUniform)
                return val - val;

            return val.MapThreads((_, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                int offset = (y % 2 == 0) ? 1 : -1;

                var me = val.Scalarize(threadIndex);
                var other = val.Scalarize(executionState.GetThreadIndex(x, y + offset));
                return ((other - me) * offset).GetThreadValue(0);
            });
        }

        public static NumericValue Ddx(HLSLExecutionState executionState, NumericValue val)
        {
            if (val.IsUniform)
                return val - val;

            return val.MapThreads((_, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                y -= (y & 1);
                threadIndex = executionState.GetThreadIndex(x, y);
                int offset = (x % 2 == 0) ? 1 : -1;

                var me = val.Scalarize(threadIndex);
                var other = val.Scalarize(executionState.GetThreadIndex(x + offset, y));
                return ((other - me) * offset).GetThreadValue(0);
            });
        }

        public static NumericValue Ddy(HLSLExecutionState executionState, NumericValue val)
        {
            if (val.IsUniform)
                return val - val;

            return val.MapThreads((_, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                x -= (x & 1);
                threadIndex = executionState.GetThreadIndex(x, y);
                int offset = (y % 2 == 0) ? 1 : -1;

                var me = val.Scalarize(threadIndex);
                var other = val.Scalarize(executionState.GetThreadIndex(x, y + offset));
                return ((other - me) * offset).GetThreadValue(0);
            });
        }

        public static NumericValue Fwidth(HLSLExecutionState executionState, NumericValue val) =>
            Abs(Ddx(executionState, val)) + Abs(Ddy(executionState, val));

        public static void Clip(HLSLExecutionState executionState, NumericValue x)
        {
            var cond = x < 0;
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
            {
                if (executionState.IsThreadActive(threadIndex))
                {
                    if (Convert.ToBoolean(cond.GetThreadValue(threadIndex)))
                        executionState.KillThreadGlobally(threadIndex);
                }
            }
        }

        public static void Abort(HLSLExecutionState executionState)
        {
            for (int threadIndex = 0; threadIndex < executionState.GetThreadCount(); threadIndex++)
                executionState.KillThreadGlobally(threadIndex);
        }
        #endregion
    }
}

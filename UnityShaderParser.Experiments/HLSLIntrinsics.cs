using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public static class HLSLIntrinsics
    {
        #region Helpers
        private static void CheckArity(string name, HLSLValue[] args, int arity)
        {
            if (args.Length != arity)
                throw new ArgumentException($"Expected {arity} arguments for builtin '{name}', but got '{args.Length}'.");
        }

        private static void CheckNumeric(string name, HLSLValue[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (!(args[i] is NumericValue))
                    throw new ArgumentException($"Expected argument in position '{i}' to builtin '{name}' to be a numeric value.");
            }
        }
        
        private static VectorValue CastToVector(NumericValue v)
        {
            if (v is VectorValue vec)
                return vec;
            else
                return v.BroadcastToVector(1);
        }

        private static MatrixValue CastToMatrix(NumericValue v)
        {
            if (v is MatrixValue mat)
                return mat;
            else
                return v.BroadcastToMatrix(1, 1);
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

        private static readonly Dictionary<string, (int arity, BasicIntrinsic fn)> basicIntrinsics = new Dictionary<string, (int arity, BasicIntrinsic fn)>()
        {
            //abort
            ["abs"] = N1(Abs),
            ["acos"] = N1(Acos),
            //all
            //AllMemoryBarrier
            //AllMemoryBarrierWithGroupSync
            //any
            //asdouble
            //asfloat
            ["asin"] = N1(Asin),
            //asint
            //asuint
            ["atan"] = N1(Atan),
            ["atan2"] = N2(Atan2),
            ["ceil"] = N1(Ceil),
            // CheckAccessFullyMapped
            ["clamp"] = N3(Clamp),
            //clip
            ["cos"] = N1(Cos),
            ["cosh"] = N1(Cosh),
            //countbits
            //cross
            //D3DCOLORtoUBYTE4
            ["degrees"] = N1(Degrees),
            //determinant
            //DeviceMemoryBarrier
            //DeviceMemoryBarrierWithGroupSync
            ["distance"] = N2(Distance),
            ["dot"] = N2(Dot),
            //dst
            //errorf
            //EvaluateAttributeCentroid
            //EvaluateAttributeAtSample
            //EvaluateAttributeSnapped
            ["exp"] = N1(Exp),
            ["exp2"] = N1(Exp2),
            //f16tof32
            //f32tof16
            ["faceforward"] = N3(Faceforward),
            //firstbithigh
            //firstbitlow
            ["floor"] = N1(Floor),
            ["fma"] = N3(Fma),
            ["fmod"] = N2(Fmod),
            ["frac"] = N1(Frac),
            //frexp
            //GetRenderTargetSampleCount
            //GetRenderTargetSamplePosition
            //GroupMemoryBarrier
            //GroupMemoryBarrierWithGroupSync
            //InterlockedAdd
            //InterlockedAnd
            //InterlockedCompareExchange
            //InterlockedCompareStore
            //InterlockedExchange
            //InterlockedMax
            //InterlockedMin
            //InterlockedOr
            //InterlockedXor
            ["isfinite"] = N1(Isfinite),
            ["isinf"] = N1(Isinf),
            ["isnan"] = N1(Isnan),
            ["ldexp"] = N2(Ldexp),
            ["length"] = N1(Length),
            ["lerp"] = N3(Lerp),
            //lit
            ["log"] = N1(Log),
            //log10
            ["log2"] = N1(Log2),
            ["mad"] = N3(Mad),
            ["max"] = N2(Max),
            ["min"] = N2(Min),
            //modf
            //msad4
            //mul
            ["noise"] = N1(Noise),
            ["normalize"] = N1(Normalize),
            ["pow"] = N2(Pow),
            //Process2DQuadTessFactorsAvg
            //Process2DQuadTessFactorsMax
            //Process2DQuadTessFactorsMin
            //ProcessIsolineTessFactors
            //ProcessQuadTessFactorsAvg
            //ProcessQuadTessFactorsMax
            //ProcessQuadTessFactorsMin
            //ProcessTriTessFactorsAvg
            //ProcessTriTessFactorsMax
            //ProcessTriTessFactorsMin
            ["radians"] = N1(Radians),
            ["rcp"] = N1(Rcp),
            ["reflect"] = N2(Reflect),
            ["refract"] = N3(Refract),
            //reversebits
            ["round"] = N1(Round),
            ["rsqrt"] = N1(Rsqrt),
            ["saturate"] = N1(Saturate),
            ["sign"] = N1(Sign),
            ["sin"] = N1(Sin),
            //sincos
            ["sinh"] = N1(Sinh),
            ["smoothstep"] = N3(Smoothstep),
            ["sqrt"] = N1(Sqrt),
            ["step"] = N2(Step),
            ["tan"] = N1(Tan),
            ["tanh"] = N1(Tanh),
            //tex1/2/3D/CUBE
            //tex1/2/3D/CUBEbias
            //tex1/2/3D/CUBEgrad
            //tex1/2/3D/CUBElod
            //tex1/2/3D/CUBEproj
            ["transpose"] = N1(Transpose),
            ["trunc"] = N1(Trunc),
        };

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
        
        public static NumericValue Asin(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Asin(Convert.ToSingle(val)));
        }

        public static NumericValue Atan(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Asin(Convert.ToSingle(val)));
        }
        
        public static NumericValue Atan2(NumericValue y, NumericValue x)
        {
            return Atan(ToFloatLike(y) / x);
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
            return ToFloatLike(x).Map(val => MathF.Sqrt(Convert.ToSingle(val)));
        }
        
        public static NumericValue Cos(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Cos(Convert.ToSingle(val)));
        }
        
        public static NumericValue Cosh(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Cosh(Convert.ToSingle(val)));
        }
        
        public static NumericValue Degrees(NumericValue x)
        {
            return x * (180f / MathF.PI);
        }
        
        public static NumericValue Distance(NumericValue x, NumericValue y)
        {
            return Length(y - x);
        }
        
        public static NumericValue Sign(NumericValue x)
        {
            return Lerp(-1, 1, Saturate(x)).Cast(ScalarType.Int);
        }

        public static NumericValue Faceforward(NumericValue n, NumericValue i, NumericValue ng)
        {
            return -n * Sign(Dot(i, ng));
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
            return ToFloatLike(x).Map(val => Convert.ToSingle(val) % 1.0f);
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
            return ToFloatLike(x).Map(val => !float.IsFinite(Convert.ToSingle(val))).Cast(ScalarType.Bool);
        }

        public static NumericValue Ldexp(NumericValue x, NumericValue exp)
        {
            return x * Exp2(exp);
        }
        
        public static NumericValue Log(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Log(Convert.ToSingle(val)));
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
        
        public static NumericValue Round(NumericValue x)
        {
            return ToFloatLike(x).Map(val => MathF.Round(Convert.ToSingle(val)));
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

            bool isFloat = HLSLValueUtils.IsFloat(vx.Type);

            int threadCount = Math.Max(vx.ThreadCount, vy.ThreadCount);
            if (threadCount == 1) // TODO: This can be refactored probably
            {
                float valueFloat = 0;
                int valueInt = 0;
                for (int channel = 0; channel < vx.Size; channel++)
                {
                    if (isFloat)
                        valueFloat += Convert.ToSingle(vx.Values.Get(0)[channel]) * Convert.ToSingle(vy.Values.Get(0)[channel]);
                    else
                        valueInt += Convert.ToInt32(vx.Values.Get(0)[channel]) * Convert.ToInt32(vy.Values.Get(0)[channel]);
                }
                return new ScalarValue(vx.Type, new HLSLRegister<object>(isFloat ? valueFloat : valueInt));
            }
            else
            {
                object[] results = new object[threadCount];
                for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
                {
                    float valueFloat = 0;
                    int valueInt = 0;
                    for (int channel = 0; channel < vx.Size; channel++)
                    {
                        if (isFloat)
                            valueFloat += Convert.ToSingle(vx.Values.Get(threadIndex)[channel]) * Convert.ToSingle(vy.Values.Get(threadIndex)[channel]);
                        else
                            valueInt += Convert.ToInt32(vx.Values.Get(threadIndex)[channel]) * Convert.ToInt32(vy.Values.Get(threadIndex)[channel]);
                    }
                    results[threadIndex] = isFloat ? valueFloat : valueInt;
                }
                return new ScalarValue(vx.Type, new HLSLRegister<object>(results));
            }
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

        #region Special intrinsics that touch execution state
        public static ScalarValue Printf(HLSLExecutionState executionState, HLSLValue[] args)
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
            return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
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

        #endregion
    }
}

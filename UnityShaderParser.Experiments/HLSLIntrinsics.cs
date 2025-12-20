using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public static class HLSLIntrinsics
    {
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

        public static bool TryInvokeIntrinsic(string name, HLSLValue[] args, out HLSLValue result)
        {
            switch (name)
            {
                case "lerp":
                    CheckArity(name, args, 3);
                    CheckNumeric(name, args);
                    result = Lerp((NumericValue)args[0], (NumericValue)args[1], (NumericValue)args[2]);
                    return true;
                case "exp":
                    CheckArity(name, args, 1);
                    CheckNumeric(name, args);
                    result = Exp((NumericValue)args[0]);
                    return true;
                default:
                    result = null;
                    return false;
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-lerp
        public static NumericValue Lerp(NumericValue x, NumericValue y, NumericValue s)
        {
            return x * (1 - s) + y * s;
        }

        public static NumericValue Exp(NumericValue x)
        {
            var casted = HLSLValueUtils.CastNumeric(ScalarType.Float, x);
            return HLSLValueUtils.Map(casted, val => Math.Exp((float)val));
        }

        #region Special intrinsics that touch execution state
        public static ScalarValue Printf(HLSLExecutionState executionState, HLSLValue[] args)
        {
            if (args.Length > 0)
            {
                int maxThreadCount = args.Max(x =>
                {
                    if (x is NumericValue num)
                        return HLSLValueUtils.GetThreadCount(num);
                    else
                        return 1;
                });

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
                                sb.Append(Convert.ToString(HLSLValueUtils.Scalarize(num, threadIndex), CultureInfo.InvariantCulture));
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
            if (HLSLValueUtils.IsUniform(val))
                return val - val;

            return HLSLValueUtils.MapThreads(val, (laneValue, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                int offset = (x % 2 == 0) ? 1 : -1;

                var me = HLSLValueUtils.Scalarize(val, threadIndex);
                var other = HLSLValueUtils.Scalarize(val, executionState.GetThreadIndex(x + offset, y));
                return HLSLValueUtils.ExtractThread((other - me) * offset, 0);
            });
        }

        public static NumericValue DdyFine(HLSLExecutionState executionState, NumericValue val)
        {
            if (HLSLValueUtils.IsUniform(val))
                return val - val;

            return HLSLValueUtils.MapThreads(val, (laneValue, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                int offset = (y % 2 == 0) ? 1 : -1;

                var me = HLSLValueUtils.Scalarize(val, threadIndex);
                var other = HLSLValueUtils.Scalarize(val, executionState.GetThreadIndex(x, y + offset));
                return HLSLValueUtils.ExtractThread((other - me) * offset, 0);
            });
        }

        public static NumericValue Ddx(HLSLExecutionState executionState, NumericValue val)
        {
            if (HLSLValueUtils.IsUniform(val))
                return val - val;

            return HLSLValueUtils.MapThreads(val, (laneValue, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                y -= (y & 1);
                threadIndex = executionState.GetThreadIndex(x, y);
                int offset = (x % 2 == 0) ? 1 : -1;

                var me = HLSLValueUtils.Scalarize(val, threadIndex);
                var other = HLSLValueUtils.Scalarize(val, executionState.GetThreadIndex(x + offset, y));
                return HLSLValueUtils.ExtractThread((other - me) * offset, 0);
            });
        }

        public static NumericValue Ddy(HLSLExecutionState executionState, NumericValue val)
        {
            if (HLSLValueUtils.IsUniform(val))
                return val - val;

            return HLSLValueUtils.MapThreads(val, (laneValue, threadIndex) =>
            {
                (int x, int y) = executionState.GetThreadPosition(threadIndex);
                x -= (x & 1);
                threadIndex = executionState.GetThreadIndex(x, y);
                int offset = (y % 2 == 0) ? 1 : -1;

                var me = HLSLValueUtils.Scalarize(val, threadIndex);
                var other = HLSLValueUtils.Scalarize(val, executionState.GetThreadIndex(x, y + offset));
                return HLSLValueUtils.ExtractThread((other - me) * offset, 0);
            });
        }
        #endregion
    }
}

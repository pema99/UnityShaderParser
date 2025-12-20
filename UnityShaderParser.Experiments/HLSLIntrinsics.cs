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
                case "printf":
                    result = Printf(args);
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

        public static ScalarValue Printf(HLSLValue[] args)
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

                for (int threadIndex = 0; threadIndex < maxThreadCount; threadIndex++)
                {
                    string formatString = args[0].ToString();
                    StringBuilder sb = new StringBuilder();
                    int argCounter = 1;
                    for (int j = 0; j < formatString.Length; j++)
                    {
                        if (formatString[j] == '%')
                        {
                            j++;
                            var arg = args[argCounter++];
                            if (arg is NumericValue num)
                            {
                                sb.Append($"[Thread {threadIndex}] ");
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;
using UnityShaderParser.HLSL.PreProcessor;

namespace UnityShaderParser.Test
{
    public class HLSLRunner
    {
        [Serializable]
        private class TestFailException : Exception
        {
            public TestFailException() { }
            public TestFailException(string message) : base(message) { }
            public TestFailException(string message, Exception innerException) : base(message, innerException) { }
        }

        [Serializable]
        private class TestPassException : Exception
        {
            public TestPassException() { }
            public TestPassException(string message) : base(message) { }
            public TestPassException(string message, Exception innerException) : base(message, innerException) { }
        }

        private struct TestRun
        {
            public string name;
            public bool customWarpSize;
            public int threadsX;
            public int threadsY;
        }


        protected HLSLInterpreter interpreter;

        public HLSLRunner(int defaultThreadsX = 2, int defaultThreadsY = 2)
        {
            interpreter = new HLSLInterpreter(defaultThreadsX, defaultThreadsY);

            // Add magic callbacks for test running
            interpreter.AddCallback("FORMAT", args =>
            {
                throw new NotImplementedException();
            });

            interpreter.AddCallback("ASSERT", args =>
            {
                if (args.Length > 0)
                {
                    HLSLValue val = interpreter.RunExpression(args[0]);
                    if (val is ScalarValue sv)
                    {
                        bool allPass = false;
                        for (int i = 0; i < sv.ThreadCount; i++)
                        {
                            if (!Convert.ToBoolean(sv.Value.Get(i)))
                            {
                                throw new TestFailException();
                            }
                        }
                        return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
                    }
                    else
                    {
                        throw new TestFailException("Argument to ASSERT should be a boolean value.");
                    }
                }
                return new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null));
            });

            interpreter.AddCallback("ASSERT_MSG", args =>
            {
                throw new NotImplementedException();
            });

            interpreter.AddCallback("PASS_TEST", args =>
            {
                throw new TestPassException();
            });

            interpreter.AddCallback("FAIL_TEST", args =>
            {
                throw new TestFailException();
            });

            interpreter.AddCallback("NAMEOF", args =>
            {
                throw new NotImplementedException();
            });
        }

        private HLSLParserConfig AddTestRunnerDefine(HLSLParserConfig config)
        {
            var newConfig = new HLSLParserConfig()
            {
                PreProcessorMode = config.PreProcessorMode,
                BasePath = config.BasePath,
                FileName = config.FileName,
                IncludeResolver = config.IncludeResolver,
                Defines = new Dictionary<string, string>(config.Defines),
                ThrowExceptionOnError = config.ThrowExceptionOnError,
                DiagnosticFilter = config.DiagnosticFilter,
            };
            newConfig.Defines.Add("__HLSL_TEST_RUNNER__", "1");
            return newConfig;
        }

        public void ProcessCode(string code) =>
            interpreter.VisitMany(ShaderParser.ParseTopLevelDeclarations(code, AddTestRunnerDefine(new HLSLParserConfig())));
        public void ProcessCode(string code, HLSLParserConfig config) =>
            interpreter.VisitMany(ShaderParser.ParseTopLevelDeclarations(code, AddTestRunnerDefine(config)));
        public void ProcessCode(string code, out List<Diagnostic> diagnostics, out List<string> pragmas) =>
            interpreter.VisitMany(ShaderParser.ParseTopLevelDeclarations(code, out diagnostics, out pragmas));
        public void ProcessCode(string code, HLSLParserConfig config, out List<Diagnostic> diagnostics, out List<string> pragmas) =>
            interpreter.VisitMany(ShaderParser.ParseTopLevelDeclarations(code, AddTestRunnerDefine(config), out diagnostics, out pragmas));

        public void Reset() => interpreter.Reset();

        public void CallFunction(string name, params object[] args) => interpreter.CallFunction(name, args);
        public void SetWarpSize(int threadsX, int threadsY) => interpreter.SetWarpSize(threadsX, threadsY);
        public void CallFunctionWithWarpSize(string name, int threadsX, int threadsY, params object[] args)
        {
            interpreter.SetWarpSize(threadsX, threadsY);
            interpreter.CallFunction(name, args);
        }

        public void RunTests() => RunTests(null);
        public void RunTests(string testFilter)
        {
            FunctionDefinitionNode[] functions = interpreter.GetFunctions();

            List<TestRun> testsToRun = new List<TestRun>();

            foreach (var func in functions.Where(x => string.IsNullOrEmpty(testFilter) || Regex.IsMatch(x.Name.GetName(), testFilter)))
            {
                bool hasTestAttribute = false;
                TestRun testRun = default;
                testRun.name = func.Name.GetName();
                foreach (var attribute in func.Attributes)
                {
                    string lexeme = attribute.Name.Identifier.ToLower();
                    switch (lexeme)
                    {
                        case "test":
                            hasTestAttribute = true;
                            break;
                        case "warpsize":
                            if (attribute.Arguments.Count > 1)
                            {
                                testRun.customWarpSize = true;
                                testRun.threadsX = Convert.ToInt32((interpreter.RunExpression(attribute.Arguments[0]) as ScalarValue).GetThreadValue(0));
                                testRun.threadsY = Convert.ToInt32((interpreter.RunExpression(attribute.Arguments[1]) as ScalarValue).GetThreadValue(0));
                            }
                            else if (attribute.Arguments.Count > 0)
                            {
                                testRun.customWarpSize = true;
                                testRun.threadsX = Convert.ToInt32((interpreter.RunExpression(attribute.Arguments[0]) as ScalarValue).GetThreadValue(0));
                                testRun.threadsY = 1;
                            }
                            break;
                        default: break;
                    }
                }
                
                if (hasTestAttribute)
                    testsToRun.Add(testRun);
            }

            int passCount = 0;
            for (int i = 0; i < testsToRun.Count; i++)
            {
                Console.WriteLine($"=== Running test ({i+1}/{testsToRun.Count}) ===");
                try
                {
                    interpreter.CallFunction(testsToRun[i].name);
                    passCount++;
                }
                catch (TestFailException ex)
                {
                    Console.WriteLine("Test failed.");
                }
                catch (TestPassException ex)
                {
                    passCount++;
                    Console.WriteLine("Test passed.");
                }
            }
            Console.WriteLine($"=== Tests passed: ({passCount}/{testsToRun.Count}) ===");
        }
    }
}

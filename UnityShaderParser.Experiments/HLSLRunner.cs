using System;
using System.Collections.Generic;
using System.IO;
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

        public struct TestRun
        {
            public string TestName;
            public bool UsesCustomWarpSize;
            public int WarpSizeX;
            public int WarpSizeY;
            public Func<List<HLSLValue>> GetInputs;
        }

        public struct TestResult
        {
            public string TestName;
            public bool Pass;
            public string Message;
            public string Log;
        }

        protected HLSLInterpreter interpreter;

        public HLSLRunner(int defaultThreadsX = 2, int defaultThreadsY = 2)
        {
            interpreter = new HLSLInterpreter(defaultThreadsX, defaultThreadsY);

            // Add magic callbacks for test running
            interpreter.AddCallback("FORMAT", (state, args) =>
            {
                throw new NotImplementedException();
            });

            ScalarValue Assert(HLSLExecutionState state, ExpressionNode[] args)
            {
                if (args.Length > 0)
                {
                    string message = null;
                    if (args.Length > 1)
                        message = (interpreter.EvaluateExpression(args[1]) as ScalarValue).Value.Get(0) as string;

                    HLSLValue val = interpreter.EvaluateExpression(args[0]);
                    if (val is ScalarValue sv)
                    {
                        for (int i = 0; i < sv.ThreadCount; i++)
                        {
                            if (state.IsThreadActive(i) && !Convert.ToBoolean(sv.Value.Get(i)))
                            {
                                throw new TestFailException(message ?? $"Assertion failed: {args[0].GetPrettyPrintedCode()}");
                            }
                        }
                    }
                    else if (val is VectorValue vv)
                    {
                        for (int i = 0; i < vv.ThreadCount; i++)
                        {
                            foreach (var b in vv.Values.Get(i))
                            {
                                if (state.IsThreadActive(i) && !Convert.ToBoolean(b))
                                {
                                    throw new TestFailException(message ?? $"Assertion failed: {args[0].GetPrettyPrintedCode()}");
                                }
                            }
                        }
                    }
                    else if (val is MatrixValue mv)
                    {
                        for (int i = 0; i < mv.ThreadCount; i++)
                        {
                            foreach (var b in mv.Values.Get(i))
                            {
                                if (state.IsThreadActive(i) && !Convert.ToBoolean(b))
                                {
                                    throw new TestFailException(message ?? $"Assertion failed: {args[0].GetPrettyPrintedCode()}");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new TestFailException("Argument to ASSERT should be a boolean value.");
                    }
                }
                return ScalarValue.Null;
            }

            interpreter.AddCallback("ASSERT", (state, args) =>
            {
                return Assert(state, args);
            });

            interpreter.AddCallback("ASSERT_MSG", (state, args) =>
            {
                return Assert(state, args);
            });

            interpreter.AddCallback("PASS_TEST", (state, args) =>
            {
                if (state.IsAnyThreadActive())
                {
                    if (args.Length > 0)
                        throw new TestPassException(interpreter.EvaluateExpression(args[0]).ToString());
                    else
                        throw new TestPassException();
                }
                return ScalarValue.Null;
            });

            interpreter.AddCallback("FAIL_TEST", (state, args) =>
            {
                if (state.IsAnyThreadActive())
                {
                    if (args.Length > 0)
                        throw new TestFailException(interpreter.EvaluateExpression(args[0]).ToString());
                    else
                        throw new TestFailException();
                }
                return ScalarValue.Null;
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
        public void ProcessCode(IEnumerable<HLSLSyntaxNode> nodes) =>
            interpreter.VisitMany(nodes);

        public void Reset() => interpreter.Reset();

        public void SetWarpSize(int threadsX, int threadsY) => interpreter.SetWarpSize(threadsX, threadsY);
        public void SetVariable(string name, HLSLValue value) => interpreter.SetVariable(name, value);
        public HLSLValue GetVariable(string name) => interpreter.GetVariable(name);
        public HLSLValue CallFunction(string name, params HLSLValue[] args) => interpreter.CallFunction(name, args);
        public HLSLValue CallFunctionWithWarpSize(string name, int threadsX, int threadsY, params HLSLValue[] args)
        {
            interpreter.SetWarpSize(threadsX, threadsY);
            return interpreter.CallFunction(name, args);
        }

        public TestResult[] RunTests(string testFilter = null, Action<TestRun> runBeforeTest = null, Action<TestRun, TestResult> runAfterTest = null)
        {
            FunctionDefinitionNode[] functions = interpreter.GetFunctions();

            List<TestRun> testsToRun = new List<TestRun>();

            foreach (var func in functions.Where(x => string.IsNullOrEmpty(testFilter) || Regex.IsMatch(x.Name.GetName(), testFilter)))
            {
                bool hasTestAttribute = false;
                List<Func<List<HLSLValue>>> testCases = new List<Func<List<HLSLValue>>>();
                TestRun testRun = default;
                testRun.TestName = func.Name.GetName();
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
                                testRun.UsesCustomWarpSize = true;
                                testRun.WarpSizeX = Convert.ToInt32((interpreter.EvaluateExpression(attribute.Arguments[0]) as ScalarValue).GetThreadValue(0));
                                testRun.WarpSizeY = Convert.ToInt32((interpreter.EvaluateExpression(attribute.Arguments[1]) as ScalarValue).GetThreadValue(0));
                            }
                            else if (attribute.Arguments.Count > 0)
                            {
                                testRun.UsesCustomWarpSize = true;
                                testRun.WarpSizeX = Convert.ToInt32((interpreter.EvaluateExpression(attribute.Arguments[0]) as ScalarValue).GetThreadValue(0));
                                testRun.WarpSizeY = 1;
                            }
                            break;
                        case "testcase":
                            if (attribute.Arguments.Count == func.Parameters.Count)
                            {
                                testCases.Add(() =>
                                {
                                    List<HLSLValue> inputs = new List<HLSLValue>();
                                    for (int i = 0; i < attribute.Arguments.Count; i++)
                                        inputs.Add(interpreter.EvaluateExpression(attribute.Arguments[i]));
                                    return inputs;
                                });
                            }
                            break;
                        default: break;
                    }
                }

                if (hasTestAttribute)
                {
                    if (testCases.Count == 0)
                    {
                        testsToRun.Add(testRun);
                    }
                    else
                    {
                        foreach (var testCase in testCases)
                        {
                            testRun.GetInputs = testCase;
                            testsToRun.Add(testRun);
                        }
                    }
                }
            }

            TestResult[] results = new TestResult[testsToRun.Count];
            var oldConsoleOut = Console.Out;
            for (int i = 0; i < testsToRun.Count; i++)
            {
                if (runBeforeTest != null)
                    runBeforeTest(testsToRun[i]);

                var sw = new StringWriter();
                Console.SetOut(sw);

                string formattedName = testsToRun[i].TestName;
                try
                {
                    if (testsToRun[i].GetInputs != null)
                    {
                        var inputs = testsToRun[i].GetInputs();
                        formattedName += $"({string.Join(", ", inputs)})";
                        interpreter.CallFunction(testsToRun[i].TestName, inputs.ToArray());
                    }
                    else
                    {
                        interpreter.CallFunction(testsToRun[i].TestName, Array.Empty<object>());
                    }
                    results[i] = new TestResult
                    {
                        TestName = formattedName,
                        Pass = true,
                        Log = sw.ToString(),
                    };
                }
                catch (TestFailException ex)
                {
                    results[i] = new TestResult
                    {
                        TestName = formattedName,
                        Pass = false,
                        Log = sw.ToString(),
                        Message = ex.Message
                    };
                }
                catch (TestPassException ex)
                {
                    results[i] = new TestResult
                    {
                        TestName = formattedName,
                        Pass = true,
                        Log = sw.ToString(),
                        Message = ex.Message
                    };
                }

                Console.SetOut(oldConsoleOut);

                if (runAfterTest != null)
                    runAfterTest(testsToRun[i], results[i]);
            }
            return results;
        }
    }
}

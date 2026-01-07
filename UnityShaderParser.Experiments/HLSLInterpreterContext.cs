using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLInterpreterContext
    {
        private Stack<Dictionary<string, HLSLValue>> environment = new Stack<Dictionary<string, HLSLValue>>(new[] { new Dictionary<string, HLSLValue>() });
        private Stack<string> namespaceStack = new Stack<string>();
        private Dictionary<string, List<FunctionDefinitionNode>> functions = new Dictionary<string, List<FunctionDefinitionNode>>();
        private Stack<HLSLValue> returnStack = new Stack<HLSLValue>();

        public void PushScope()
        {
            environment.Push(new Dictionary<string, HLSLValue>());
        }

        public void PopScope()
        {
            environment.Pop();
        }

        private bool TryFindVariable(string name, out Dictionary<string, HLSLValue> resolvedScope, out string resolvedName, out HLSLValue resolvedValue)
        {
            // Local scope
            var localScope = environment.Take(environment.Count - 1);
            foreach (var scope in localScope)
            {
                if (scope.TryGetValue(name, out var val))
                {
                    resolvedScope = scope;
                    resolvedName = name;
                    resolvedValue = val;
                    return true;
                }
            }

            // Not in local scope, try global scope
            var globalScope = environment.Last();
            if (namespaceStack.Count > 0)
            {
                // In a namespace, start with most specific prefix, and try each possible prefix
                var reverseNamespace = namespaceStack.Reverse().ToArray();
                for (int i = 0; i < namespaceStack.Count + 1; i++)
                {
                    int prefixLength = namespaceStack.Count - i;
                    string currPrefix = string.Join("::", reverseNamespace.Take(prefixLength));
                    string qualifiedName = string.IsNullOrEmpty(currPrefix) ? name : $"{currPrefix}::{name}";
                    if (globalScope.TryGetValue(qualifiedName, out var val))
                    {
                        resolvedScope = globalScope;
                        resolvedName = qualifiedName;
                        resolvedValue = val;
                        return true;
                    }
                }
            }
            else
            {
                // No namespace, resolve the name directly
                if (globalScope.TryGetValue(name, out var val))
                {
                    resolvedScope = globalScope;
                    resolvedName = name;
                    resolvedValue = val;
                    return true;
                }
            }

            resolvedScope = null;
            resolvedName = null;
            resolvedValue = null;
            return false;
        }
        
        public HLSLValue GetVariable(string name)
        {
            TryFindVariable(name, out _, out _, out HLSLValue value);
            return value;
        }

        public ReferenceValue GetReference(string name)
        {
            if (TryFindVariable(name, out var scope, out var resolvedName, out _))
                return new ReferenceValue(() => scope[resolvedName], val => scope[resolvedName] = val);
            return null;
        }

        public bool TryGetVariable(string name, out HLSLValue variable)
        {
            variable = GetVariable(name);
            return variable != null;
        }

        public bool HasVariable(string name)
        {
            return GetVariable(name) != null;
        }

        public void SetVariable(string name, HLSLValue val)
        {
            if (environment.Count <= 1)
            {
                SetGlobalVariable(name, val);
                return;
            }

            if (TryFindVariable(name, out var scope, out var resolvedName, out _))
            {
                scope[resolvedName] = val;
                return;
            }

            environment.Peek()[name] = val;
        }

        public void SetGlobalVariable(string name, HLSLValue type)
        {
            // If we are in a namespace (and in global scope), prepend the namespace to the name
            if (namespaceStack.Count > 0)
            {
                name = $"{string.Join("::", namespaceStack.Reverse())}::{name}";
            }

            environment.Peek()[name] = type;
        }

        public FunctionDefinitionNode GetFunction(string name, IEnumerable<HLSLValue> args)
        {
            if (namespaceStack.Count > 0)
            {
                // If we are in a namespace, try to resolve the name with the namespace prefix, starting from the most specific
                var revNamespace = namespaceStack.Reverse().ToArray();
                for (int i = 0; i < namespaceStack.Count + 1; i++)
                {
                    int prefixLen = namespaceStack.Count - i;
                    string prefix = string.Join("::", revNamespace.Take(prefixLen));
                    string fullName = string.IsNullOrEmpty(prefix) ? name : $"{prefix}::{name}";
                    if (functions.TryGetValue(fullName, out var funcs))
                        return funcs.First();
                    // TODO:
                    // return HLSLValueUtils.PickOverload(funcs, args);
                }
            }
            else
            {
                // If we are not in a namespace, just try to resolve the name directly
                if (functions.TryGetValue(name, out var funcs))
                    return funcs.First();
                // TODO:
                // return HLSLValueUtils.PickOverload(funcs, args);
            }

            return null;
        }

        public FunctionDefinitionNode[] GetFunctions()
        {
            return functions.Values.SelectMany(x => x).ToArray();
        }

        public void AddFunction(string name, FunctionDefinitionNode func)
        {
            if (namespaceStack.Count > 0)
            {
                name = $"{string.Join("::", namespaceStack.Reverse())}::{name}";
            }
            if (!functions.TryGetValue(name, out var overloads))
            {
                overloads = new List<FunctionDefinitionNode>();
                functions[name] = overloads;
            }
            overloads.Add(func);
        }

        public void PushReturn()
        {
            // We don't know the type yet, so just put a dummy object
            returnStack.Push(new ScalarValue(ScalarType.Void, new HLSLRegister<object>(null)));
        }

        public void SetReturn(int threadIndex, HLSLValue value)
        {
            var oldReturn = returnStack.Pop();
            // If this is the first return, just use it directly.
            if (oldReturn is ScalarValue sv && sv.Type == ScalarType.Void)
            {
                returnStack.Push(value);
            }
            // Otherwise splat the thread value
            else
            {
                var newReturn = HLSLValueUtils.SetThreadValue(oldReturn, threadIndex, value);
                returnStack.Push(newReturn);
            }
        }

        public HLSLValue PopReturn()
        {
            return returnStack.Pop();
        }
    }
}

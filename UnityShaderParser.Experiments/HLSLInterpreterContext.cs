using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public class HLSLInterpreterContext
    {
        private Stack<(bool isFunction, Dictionary<string, HLSLValue> table)> environment = new Stack<(bool, Dictionary<string, HLSLValue>)>(new[] { (false, new Dictionary<string, HLSLValue>()) });
        private Stack<string> namespaceStack = new Stack<string>();
        private Dictionary<string, List<FunctionDefinitionNode>> functions = new Dictionary<string, List<FunctionDefinitionNode>>();
        private Dictionary<string, StructTypeNode> structs = new Dictionary<string, StructTypeNode>();
        private Stack<HLSLValue> returnStack = new Stack<HLSLValue>();

        public void EnterNamespace(string name)
        {
            namespaceStack.Push(name);
        }

        public void ExitNamespace()
        {
            namespaceStack.Pop();
        }

        public void PushScope(bool isFunction = false)
        {
            environment.Push((isFunction, new Dictionary<string, HLSLValue>()));
        }

        public void PopScope()
        {
            environment.Pop();
        }

        private bool TryFindVariable(string name, out Dictionary<string, HLSLValue> resolvedScope, out string resolvedName, out HLSLValue resolvedValue)
        {
            // Local scope
            var localScope = environment.Take(environment.Count - 1);
            foreach (var (isFunction, scope) in localScope)
            {
                if (scope.TryGetValue(name, out var val))
                {
                    resolvedScope = scope;
                    resolvedName = name;
                    resolvedValue = val;
                    return true;
                }
                if (isFunction)
                    break;
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
                    if (globalScope.table.TryGetValue(qualifiedName, out var val))
                    {
                        resolvedScope = globalScope.table;
                        resolvedName = qualifiedName;
                        resolvedValue = val;
                        return true;
                    }
                }
            }
            else
            {
                // No namespace, resolve the name directly
                if (globalScope.table.TryGetValue(name, out var val))
                {
                    resolvedScope = globalScope.table;
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

            environment.Peek().table[name] = val;
        }

        public void SetGlobalVariable(string name, HLSLValue type)
        {
            // If we are in a namespace (and in global scope), prepend the namespace to the name
            if (namespaceStack.Count > 0)
            {
                name = $"{string.Join("::", namespaceStack.Reverse())}::{name}";
            }

            environment.Peek().table[name] = type;
        }

        public FunctionDefinitionNode GetFunction(string name, HLSLValue[] args)
        {
            FunctionDefinitionNode overload = null;
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
                    {
                        overload = HLSLValueUtils.PickOverload(funcs, args);
                    }
                }
            }
            else
            {
                // If we are not in a namespace, just try to resolve the name directly
                if (functions.TryGetValue(name, out var funcs))
                {
                    overload = HLSLValueUtils.PickOverload(funcs, args);
                }
            }

            return overload;
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
        
        public StructTypeNode GetStruct(string name)
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
                    if (structs.TryGetValue(fullName, out var structType))
                        return structType;
                }
            }
            else
            {
                // If we are not in a namespace, just try to resolve the name directly
                if (structs.TryGetValue(name, out var structType))
                    return structType;
            }

            return null;
        }

        public void AddStruct(string name, StructTypeNode structType)
        {
            if (namespaceStack.Count > 0)
            {
                name = $"{string.Join("::", namespaceStack.Reverse())}::{name}";
            }
            structs[name] = structType;
        }

        public void PushReturn()
        {
            // We don't know the type yet, so just put a dummy object
            returnStack.Push(ScalarValue.Null);
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

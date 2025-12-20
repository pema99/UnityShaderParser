using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace UnityShaderParser.Test
{
    public class HLSLInterpreterContext
    {
        private Stack<Dictionary<string, HLSLValue>> environment = new Stack<Dictionary<string, HLSLValue>>(new[] { new Dictionary<string, HLSLValue>() });
        private Stack<string> namespaceStack = new Stack<string>();
        //private Dictionary<string, List<FunctionKind>> functions = new();

        public void PushScope()
        {
            environment.Push(new Dictionary<string, HLSLValue>());
        }

        public void PopScope()
        {
            environment.Pop();
        }

        public HLSLValue GetVariable(string name)
        {
            // Local scope
            var localScope = environment.Take(environment.Count - 1);
            foreach (var scope in localScope)
            {
                if (scope.TryGetValue(name, out var type))
                {
                    return type;
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
                    if (globalScope.TryGetValue(qualifiedName, out var type))
                    {
                        return type;
                    }
                }
            }
            else
            {
                // No namespace, resolve the name directly
                if (globalScope.TryGetValue(name, out var type))
                {
                    return type;
                }
            }

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

        public void SetVariable(string name, HLSLValue type)
        {
            if (environment.Count <= 1)
                SetGlobalVariable(name, type);
            else
                environment.Peek()[name] = type;
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
    }
}

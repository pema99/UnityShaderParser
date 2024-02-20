using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityShaderParser.Common
{
    public enum ParserStage
    {
        HLSLLexing,
        HLSLPreProcessing,
        HLSLParsing,
        ShaderLabLexing,
        ShaderLabParsing,
    }

    public struct SourceLocation
    {
        public int Line;
        public int Column;

        public SourceLocation(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public override string ToString() => $"({Line}, {Column})";
    }

    public struct Diagnostic
    {
        public SourceLocation Location;
        public ParserStage Stage;
        public string Text;

        public override string ToString()
        {
            return $"Error during {Stage}, line {Location.Line}, col {Location.Column}: {Text}";
        }
    }

    // TODO: Filename
    public struct SourceSpan
    {
        public SourceLocation Start;
        public SourceLocation End;

        public override string ToString() => $"({Start.Line}:{Start.Column} - {End.Line}:{End.Column})";

        public static SourceSpan Union(SourceSpan start, SourceSpan end)
        {
            return new SourceSpan
            {
                Start = start.Start,
                End = end.End,
            };
        }
    }

    public struct Token<T>
        where T : struct
    {
        public T Kind;
        public string Identifier;   // Optional
        public SourceSpan Span;     // Location in source code
        public int Position;        // Location in token stream

        // TODO: Trivia
        public override string ToString()
        {
            if (Identifier == null)
                return Kind.ToString() ?? string.Empty;
            else
                return $"{Kind}({Identifier})";
        }
    }

    public abstract class SyntaxNode<TSelf>
        where TSelf : SyntaxNode<TSelf>
    {
        // Helpers
        protected static IEnumerable<TSelf> MergeChildren(params IEnumerable<TSelf>[] children)
            => children.SelectMany(x => x);
        protected static IEnumerable<TSelf> OptionalChild(TSelf child)
            => child == null ? Enumerable.Empty<TSelf>() : new[] { child };
        protected static IEnumerable<TSelf> Child(TSelf child)
            => new[] { child };
        protected abstract IEnumerable<TSelf> GetChildren { get; }

        private TSelf parent;
        internal void ComputeParents()
        {
            foreach (var child in GetChildren)
            {
                child.parent = (TSelf)this;
                child.ComputeParents();
            }
        }

        // Public API
        public List<TSelf> Children => GetChildren.ToList();
        public TSelf Parent => parent;
        public abstract SourceSpan Span { get; }
    }

    public enum PrettyEnumStyle
    {
        AllLowerCase,
        AllUpperCase,
        CamelCase,
        PascalCase,
    }

    public class PrettyEnumAttribute : Attribute
    {
        public PrettyEnumStyle Style { get; set; }
        public PrettyEnumAttribute(PrettyEnumStyle firstIsLowerCase)
        {
            Style = firstIsLowerCase;
        }
    }

    public class PrettyNameAttribute : Attribute
    {
        public string Name { get; set; }

        public PrettyNameAttribute(string name)
        {
            Name = name;
        }
    }

    public static class PrintingUtil
    {
        public static string GetEnumName<T>(T val)
            where T : Enum
        {
            string name;
            PrettyEnumAttribute[] enumAttrs = typeof(T).GetCustomAttributes<PrettyEnumAttribute>().ToArray();
            if (enumAttrs == null || enumAttrs.Length == 0)
            {
                name = Enum.GetName(typeof(T), val);
            }
            else
            {
                MemberInfo[] memberInfo = typeof(T).GetMember(val.ToString());
                if (memberInfo != null && memberInfo.Length > 0)
                {
                    foreach (MemberInfo member in memberInfo)
                    {
                        PrettyNameAttribute[] attrs = member.GetCustomAttributes<PrettyNameAttribute>().ToArray();

                        if (attrs != null && attrs.Length > 0)
                        {
                            //Pull out the description value
                            return attrs[0].Name;
                        }
                    }
                }
                name = Enum.GetName(typeof(T), val);
            }

            switch (enumAttrs[0].Style)
            {
                case PrettyEnumStyle.AllLowerCase: return name.ToLower();
                case PrettyEnumStyle.AllUpperCase: return name.ToUpper();
                case PrettyEnumStyle.CamelCase:
                    if (name.Length > 0)
                    {
                        name = $"{char.ToLower(name[0])}{name.Substring(1)}";
                    }
                    return name;
                default:
                    return name;
            }
        }
    }
}

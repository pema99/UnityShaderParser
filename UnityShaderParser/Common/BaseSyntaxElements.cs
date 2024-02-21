using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // For better diagnostics
        public int Line { get; }
        public int Column { get; }

        // For analysis
        public int Index { get; }

        public SourceLocation(int line, int column, int index)
        {
            Line = line;
            Column = column;
            Index = index;
        }

        public override string ToString() => $"({Line}, {Column})";
    }

    [Flags]
    public enum DiagnosticFlags
    {
        None = 0,
        SyntaxError = 1 << 0,       // Ill-formed source code
        SemanticError = 1 << 1,     // Well-formed source code, but incorrect meaning
        PreProcessorError = 1 << 2, // Error during preprocessing
        Warning = 1 << 3,           // Well-formed source code, but probably not what was intended

        OnlyErrors = SyntaxError | SemanticError | PreProcessorError,
        All = OnlyErrors | Warning
    }

    public struct Diagnostic
    {
        public SourceLocation Location { get; }
        public DiagnosticFlags Kind { get; }
        public ParserStage Stage { get; }
        public string Text { get; }

        public Diagnostic(SourceLocation location, DiagnosticFlags kind, ParserStage stage, string text)
        {
            Location = location;
            Kind = kind;
            Stage = stage;
            Text = text;
        }

        public override string ToString()
        {
            return $"Error during {Stage}, line {Location.Line}, col {Location.Column}: {Text}";
        }
    }

    // TODO: Filename
    public struct SourceSpan
    {
        public SourceLocation Start { get; }
        public SourceLocation End { get; }

        public int StartIndex => Start.Index;
        public int EndIndex => End.Index;
        public int Length => EndIndex - StartIndex;

        public SourceSpan(SourceLocation start, SourceLocation end)
        {
            Start = start;
            End = end;
        }

        public override string ToString() => $"({Start.Line}:{Start.Column} - {End.Line}:{End.Column})";

        public string GetCodeInSourceText(string sourceText) => sourceText.Substring(StartIndex, Length);

        public static SourceSpan FromTokens<T>(IEnumerable<Token<T>> tokens)
            where T : struct
        {
            if (tokens == null || !tokens.Any())
                throw new ArgumentException(nameof(tokens));
            var ordered = tokens.OrderBy(x => x.Span.StartIndex);
            var first = ordered.First();
            var last = ordered.Last();
            return BetweenTokens(first, last);
        }

        public static SourceSpan BetweenTokens<T>(Token<T> first, Token<T> last)
            where T : struct => new SourceSpan(first.Span.Start, last.Span.End);
    }

    public struct Token<T>
        where T : struct
    {
        public T Kind { get; }
        public string Identifier { get; } // Optional
        public SourceSpan Span { get; }   // Location in source code
        public int Position { get; }      // Location in token stream

        public Token(T kind, string identifier, SourceSpan span, int position)
        {
            Kind = kind;
            Identifier = identifier;
            Span = span;
            Position = position;
        }

        // TODO: Trivia
        public override string ToString()
        {
            if (Identifier == null)
                return Kind.ToString() ?? string.Empty;
            else
                return $"{Kind}({Identifier})";
        }

        public string GetCodeInSourceText(string sourceText) => Span.GetCodeInSourceText(sourceText);
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract IEnumerable<TSelf> GetChildren { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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

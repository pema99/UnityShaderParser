using System.Collections.Generic;
using System.Linq;

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

    public struct Diagnostic
    {
        public (int Line, int Column) Location;
        public ParserStage Stage;
        public string Text;
    }

    // TODO: Filename
    public struct SourceSpan
    {
        public (int Line, int Column) Start;
        public (int Line, int Column) End;

        public override string ToString() => $"({Start.Line}:{Start.Column} - {End.Line}:{End.Column})";
    }

    public struct Token<TTokenKind>
        where TTokenKind : struct
    {
        public TTokenKind Kind;
        public string Identifier; // Optional
        public SourceSpan Span;

        public override string ToString()
        {
            if (Identifier == null)
                return Kind.ToString() ?? string.Empty;
            else
                return $"{Kind}: {Identifier}";
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

        // Public API
        public List<TSelf> Children => GetChildren.ToList();

        // TODO: Store parent by making ctor's which the relevant parent on their child
        // TODO: Feed in span data
    }
}

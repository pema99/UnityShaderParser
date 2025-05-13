using System;
using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    public abstract class HLSLEditor : HLSLSyntaxVisitor
    {
        public string Source { get; private set; }
        public List<Token<TokenKind>> Tokens { get; private set; }
        public EditConflictResolutionMode ConflictResolutionMode { get; private set; }
        public EditConflictHandler<TokenKind, HLSLSyntaxNode> ConflictHandler { get; private set;}

        public HLSLEditor(
            string source,
            List<Token<TokenKind>> tokens,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenKind, HLSLSyntaxNode> conflictHandler)
        {
            Source = source;
            Tokens = tokens;
            ConflictResolutionMode = conflictResolutionMode;
            ConflictHandler = conflictHandler;
        }

        protected HashSet<EditInfo<TokenKind, HLSLSyntaxNode>> Edits = new HashSet<EditInfo<TokenKind, HLSLSyntaxNode>>();

        protected void Edit(SourceSpan span, string newText) => Edits.Add(new EditInfo<TokenKind, HLSLSyntaxNode>(span, newText));
        protected void Edit(Token<TokenKind> token, string newText) => Edits.Add(new EditInfo<TokenKind, HLSLSyntaxNode>(token, newText));
        protected void Edit(HLSLSyntaxNode node, string newText) => Edits.Add(new EditInfo<TokenKind, HLSLSyntaxNode>(node, newText));
        protected void AddBefore(SourceSpan span, string newText) => Edit(new SourceSpan(span.BasePath, span.FileName, span.Start, span.Start), newText);
        protected void AddBefore(Token<TokenKind> token, string newText) => Edit(new SourceSpan(token.Span.BasePath, token.Span.FileName, token.Span.Start, token.Span.Start), newText);
        protected void AddBefore(HLSLSyntaxNode node, string newText) => Edit(new SourceSpan(node.Span.BasePath, node.Span.FileName, node.Span.Start, node.Span.Start), newText);
        protected void AddAfter(SourceSpan span, string newText) => Edit(new SourceSpan(span.BasePath, span.FileName, span.End, span.End), newText);
        protected void AddAfter(Token<TokenKind> token, string newText) => Edit(new SourceSpan(token.Span.BasePath, token.Span.FileName, token.Span.End, token.Span.End), newText);
        protected void AddAfter(HLSLSyntaxNode node, string newText) => Edit(new SourceSpan(node.Span.BasePath, node.Span.FileName, node.Span.End, node.Span.End), newText);

        public string ApplyCurrentEdits() => EditorUtils.ApplyEdits(Edits, Source, ConflictResolutionMode, ConflictHandler);

        public string ApplyEdits(HLSLSyntaxNode node)
        {
            Visit(node);
            return ApplyCurrentEdits();
        }

        public string ApplyEdits(IEnumerable<HLSLSyntaxNode> nodes)
        {
            VisitMany(nodes);
            return ApplyCurrentEdits();
        }

        public static string RunEditor<T>(string source, HLSLSyntaxNode node)
            where T : HLSLEditor
        {
            return RunEditor<T>(source, node, default);
        }

        public static string RunEditor<T>(string source, IEnumerable<HLSLSyntaxNode> node)
            where T : HLSLEditor
        {
            return RunEditor<T>(source, node, default);
        }

        public static string RunEditor<T>(
            string source,
            HLSLSyntaxNode node,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenKind, HLSLSyntaxNode> conflictHandler = null)
            where T : HLSLEditor
        {
            var editor = (HLSLEditor)Activator.CreateInstance(typeof(T), source, node.Tokens, conflictResolutionMode, conflictHandler);
            return editor.ApplyEdits(node);
        }

        public static string RunEditor<T>(
            string source,
            IEnumerable<HLSLSyntaxNode> node,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenKind, HLSLSyntaxNode> conflictHandler = null)
            where T : HLSLEditor
        {
            var editor = (HLSLEditor)Activator.CreateInstance(typeof(T), source, node.SelectMany(x => x.Tokens).ToList(), conflictResolutionMode, conflictHandler);
            return editor.ApplyEdits(node);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityShaderParser.Common;

namespace UnityShaderParser.ShaderLab
{
    public abstract class ShaderLabEditor : ShaderLabSyntaxVisitor
    {
        public string Source { get; private set; }
        public List<Token<TokenKind>> Tokens { get; private set; }
        public EditConflictResolutionMode ConflictResolutionMode { get; private set; }
        public EditConflictHandler<TokenKind, ShaderLabSyntaxNode> ConflictHandler { get; private set; }

        public ShaderLabEditor(
            string source,
            List<Token<TokenKind>> tokens,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenKind, ShaderLabSyntaxNode> conflictHandler)
        {
            Source = source;
            Tokens = tokens;
            ConflictResolutionMode = conflictResolutionMode;
            ConflictHandler = conflictHandler;
        }

        protected HashSet<EditInfo<TokenKind, ShaderLabSyntaxNode>> Edits = new HashSet<EditInfo<TokenKind, ShaderLabSyntaxNode>>();

        protected void Edit(SourceSpan span, string newText) => Edits.Add(new EditInfo<TokenKind, ShaderLabSyntaxNode>(span, newText));
        protected void Edit(Token<TokenKind> token, string newText) => Edits.Add(new EditInfo<TokenKind, ShaderLabSyntaxNode>(token, newText));
        protected void Edit(ShaderLabSyntaxNode node, string newText) => Edits.Add(new EditInfo<TokenKind, ShaderLabSyntaxNode>(node, newText));
        protected void AddBefore(SourceSpan span, string newText) => Edit(new SourceSpan(span.BasePath, span.FileName, span.Start, span.Start), newText);
        protected void AddBefore(Token<TokenKind> token, string newText) => Edit(new SourceSpan(token.Span.BasePath, token.Span.FileName, token.Span.Start, token.Span.Start), newText);
        protected void AddBefore(ShaderLabSyntaxNode node, string newText) => Edit(new SourceSpan(node.Span.BasePath, node.Span.FileName, node.Span.Start, node.Span.Start), newText);
        protected void AddAfter(SourceSpan span, string newText) => Edit(new SourceSpan(span.BasePath, span.FileName, span.End, span.End), newText);
        protected void AddAfter(Token<TokenKind> token, string newText) => Edit(new SourceSpan(token.Span.BasePath, token.Span.FileName, token.Span.End, token.Span.End), newText);
        protected void AddAfter(ShaderLabSyntaxNode node, string newText) => Edit(new SourceSpan(node.Span.BasePath, node.Span.FileName, node.Span.End, node.Span.End), newText);

        public string ApplyCurrentEdits() => EditorUtils.ApplyEdits(Edits, Source, ConflictResolutionMode, ConflictHandler);

        public string ApplyEdits(ShaderLabSyntaxNode node)
        {
            Visit(node);
            return ApplyCurrentEdits();
        }

        public string ApplyEdits(IEnumerable<ShaderLabSyntaxNode> nodes)
        {
            VisitMany(nodes);
            return ApplyCurrentEdits();
        }

        public static string RunEditor<T>(string source, ShaderLabSyntaxNode node)
            where T : ShaderLabEditor
        {
            return RunEditor<T>(source, node, default);
        }

        public static string RunEditor<T>(string source, IEnumerable<ShaderLabSyntaxNode> node)
            where T : ShaderLabEditor
        {
            return RunEditor<T>(source, node, default);
        }

        public static string RunEditor<T>(
            string source,
            ShaderLabSyntaxNode node,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenKind, ShaderLabSyntaxNode> conflictHandler = null)
            where T : ShaderLabEditor
        {
            var editor = (ShaderLabEditor)Activator.CreateInstance(typeof(T), source, node.Tokens, conflictResolutionMode, conflictHandler);
            return editor.ApplyEdits(node);
        }

        public static string RunEditor<T>(
            string source,
            IEnumerable<ShaderLabSyntaxNode> node,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenKind, ShaderLabSyntaxNode> conflictHandler = null)
            where T : ShaderLabEditor
        {
            var editor = (ShaderLabEditor)Activator.CreateInstance(typeof(T), source, node.SelectMany(x => x.Tokens).ToList(), conflictResolutionMode, conflictHandler);
            return editor.ApplyEdits(node);
        }
    }
}

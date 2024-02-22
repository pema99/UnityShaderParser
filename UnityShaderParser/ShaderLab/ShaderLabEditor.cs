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

        public ShaderLabEditor(string source, List<Token<TokenKind>> tokens)
        {
            Source = source;
            Tokens = tokens;
        }

        protected HashSet<(SourceSpan span, string newText)> Edits = new HashSet<(SourceSpan, string)>();

        protected void Edit(SourceSpan span, string newText) => Edits.Add((span, newText));
        protected void Edit(Token<TokenKind> token, string newText) => Edit(token.Span, newText);
        protected void Edit(ShaderLabSyntaxNode node, string newText) => Edit(node.Span, newText);

        public string ApplyCurrentEdits() => PrintingUtil.ApplyEditsToSourceText(Edits, Source);

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
            var editor = (ShaderLabEditor)Activator.CreateInstance(typeof(T), source, node.Tokens);
            return editor.ApplyEdits(node);
        }

        public static string RunEditor<T>(string source, IEnumerable<ShaderLabSyntaxNode> node)
            where T : ShaderLabEditor
        {
            var editor = (ShaderLabEditor)Activator.CreateInstance(typeof(T), source, node.SelectMany(x => x.Tokens).ToList());
            return editor.ApplyEdits(node);
        }
    }
}

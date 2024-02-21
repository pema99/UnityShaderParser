using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    // TODO: Base Editor for ShaderLab as well (base Visitor too?)
    public abstract class HLSLEditor : HLSLSyntaxVisitor
    {
        public string Source { get; private set; }
        public List<Token<TokenKind>> Tokens { get; private set; }

        public HLSLEditor(string source, List<Token<TokenKind>> tokens)
        {
            Source = source;
            Tokens = tokens;
        }

        protected HashSet<(SourceSpan span, string newText)> Edits = new HashSet<(SourceSpan, string)>();

        protected void Edit(SourceSpan span, string newText) => Edits.Add((span, newText));
        protected void Edit(Token<TokenKind> token, string newText) => Edit(token.Span, newText);
        protected void Edit(HLSLSyntaxNode node, string newText) => Edit(node.Span, newText);

        public string ApplyCurrentEdits()
        {
            var orderedEdits = Edits.OrderBy(x => x.span.Start.Index);
            var editedSourced = new StringBuilder(Source);
            int offset = 0;
            foreach ((SourceSpan span, string newText) in orderedEdits)
            {
                editedSourced.Remove(span.Start.Index + offset, span.Length);
                editedSourced.Insert(span.Start.Index + offset, newText);
                offset -= span.Length;
                offset += newText.Length;
            }
            return editedSourced.ToString();
        }

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
            var editor = (HLSLEditor)Activator.CreateInstance(typeof(T), source, node.Tokens);
            return editor.ApplyEdits(node);
        }

        public static string RunEditor<T>(string source, IEnumerable<HLSLSyntaxNode> node)
            where T : HLSLEditor
        {
            var editor = (HLSLEditor)Activator.CreateInstance(typeof(T), source, node.SelectMany(x => x.Tokens).ToList());
            return editor.ApplyEdits(node);
        }
    }
}

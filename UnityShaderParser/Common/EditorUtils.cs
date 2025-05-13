

using System.Collections.Generic;
using System.Linq;

namespace UnityShaderParser.Common
{
    public enum EditConflictResolutionMode
    {
        FirstWins, // First edit wins
        LastWins,  // Last edit wins
        Custom,    // Delegate provided
        IgnoreConflicts, // Ignore all conflicts, may give unpredictable results, but faster
    }

    public enum EditConflictResolution
    {
        KeepFirst,
        KeepSecond,
        DiscardBoth,
    }

    public struct EditInfo<TokenType, NodeType>
        where TokenType : struct
        where NodeType : SyntaxNode<NodeType>
    {
        public SourceSpan Span;
        public string NewText;
        public Token<TokenType> Token;    // Optional, if the info is available
        public SyntaxNode<NodeType> Node; // Optional, if the info is available

        public EditInfo(SourceSpan span, string newText)
        {
            Span = span;
            NewText = newText;
            Token = null;
            Node = null;
        }

        public EditInfo(Token<TokenType> token, string newText)
        {
            Span = token.Span;
            NewText = newText;
            Token = token;
            Node = null;
        }

        public EditInfo(SyntaxNode<NodeType> node, string newText)
        {
            Span = node.Span;
            NewText = newText;
            Token = null;
            Node = node;
        }
    }

    public delegate EditConflictResolution EditConflictHandler<TokenType, NodeType>(
        EditInfo<TokenType, NodeType> firstEdit,
        EditInfo<TokenType, NodeType> secondEdit)
        where TokenType : struct
        where NodeType : SyntaxNode<NodeType>;

    public class EditorUtils
    {
        public static List<EditInfo<TokenType, NodeType>> HandleConflictingEdits<TokenType, NodeType>(
            IEnumerable<EditInfo<TokenType, NodeType>> edits,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenType, NodeType> conflictHandler)
            where TokenType : struct
            where NodeType : SyntaxNode<NodeType>
        {
            if (conflictResolutionMode == EditConflictResolutionMode.IgnoreConflicts)
                return edits.ToList();

            if (conflictResolutionMode == EditConflictResolutionMode.Custom && conflictHandler == null)
            {
                throw new System.InvalidOperationException(
                    "Edit conflict resolution mode was set to Custom, " +
                    "but no conflict handler was provided.");
            }

            var sortedEdits = edits
                .OrderBy(e => e.Span.Start.Index)
                .ToList();

            var filtered = new List<EditInfo<TokenType, NodeType>>();

            foreach (var edit in sortedEdits)
            {
                // Find all overlapping edits already in the list
                var overlappingIndices = new List<int>();

                for (int i = 0; i < filtered.Count; i++)
                {
                    var existing = filtered[i];
                    if (edit.Span.Start.Index < existing.Span.End.Index &&
                        existing.Span.Start.Index < edit.Span.End.Index)
                    {
                        overlappingIndices.Add(i);
                    }
                }

                if (overlappingIndices.Count == 0)
                {
                    // No conflict — just add it
                    filtered.Add(edit);
                }
                else if (conflictResolutionMode == EditConflictResolutionMode.LastWins)
                {
                    // Remove all overlapping edits and insert the current one
                    foreach (var idx in overlappingIndices.OrderByDescending(i => i))
                    {
                        filtered.RemoveAt(idx);
                    }

                    filtered.Add(edit);
                }
                else if (conflictResolutionMode == EditConflictResolutionMode.Custom)
                {
                    bool discard = false;
                    foreach (var idx in overlappingIndices.OrderByDescending(i => i))
                    {
                        var existing = filtered[idx];
                        var resolution = conflictHandler(existing, edit);
                        if (resolution == EditConflictResolution.KeepSecond)
                        {
                            filtered.RemoveAt(idx);
                        }
                        else if (resolution == EditConflictResolution.DiscardBoth)
                        {
                            filtered.RemoveAt(idx);
                            discard = true;
                        }
                        else if (resolution == EditConflictResolution.KeepFirst)
                        {
                            discard = true;
                        }
                    }
                    if (!discard)
                        filtered.Add(edit);
                }
                // Else: FirstWins — skip conflicting edit
            }

            return filtered;
        }

        public static string ApplyEdits<TokenType, NodeType>(
            IEnumerable<EditInfo<TokenType, NodeType>> edits,
            string source,
            EditConflictResolutionMode conflictResolutionMode,
            EditConflictHandler<TokenType, NodeType> conflictHandler)
            where TokenType : struct
            where NodeType : SyntaxNode<NodeType>
        {
            var filteredEdits = edits;
            if (conflictResolutionMode != EditConflictResolutionMode.IgnoreConflicts)
                filteredEdits = HandleConflictingEdits(edits, conflictResolutionMode, conflictHandler);

            return PrintingUtil.ApplyEditsToSourceText(filteredEdits.Select(edit => (edit.Span, edit.NewText)), source);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityShaderParser.Common
{
    public abstract class BaseParser<T>
        where T : struct, Enum
    {
        protected abstract class BaseTokenStream
        {
            public abstract bool IsAtEnd(int offsetFromCurrent);
            public abstract Token<T> GetAt(int offsetFromCurrent);
            public abstract void Advance(int offsetFromCurrent);
            public abstract List<Token<T>> ToList();
            public abstract void BeginChangeCheck();
            public abstract bool EndChangeCheck();
        }

        protected sealed class ListTokenStream : BaseTokenStream
        {
            public List<Token<T>> InnerList = new List<Token<T>>();
            public int Index = 0;
            private int LastIndex = 0;

            public ListTokenStream(List<Token<T>> list) { InnerList = new List<Token<T>>(list); }

            public override bool IsAtEnd(int offsetFromCurrent) => Index + offsetFromCurrent >= InnerList.Count;
            public override void Advance(int offsetFromCurrent) => Index += offsetFromCurrent;
            public override Token<T> GetAt(int offsetFromCurrent) => InnerList[Index + offsetFromCurrent];
            public override List<Token<T>> ToList() => InnerList;
            public override void BeginChangeCheck() => LastIndex = Index;
            public override bool EndChangeCheck() => Index > LastIndex;
        }

        protected sealed class LinkedListTokenStream : BaseTokenStream
        {
            public class Node
            {
                public Token<T> Value;
                public Node Next;
                public Node Prev;
            }
            public Node FirstNode;
            public Node CurrentNode;
            public Node ChangeCheckNode;

            public LinkedListTokenStream(List<Token<T>> list)
            {
                if (list.Count == 0)
                    return;

                FirstNode = new Node
                {
                    Value = list[0]
                };
                CurrentNode = FirstNode;

                var currNode = FirstNode;
                for (int i = 1; i < list.Count; i++)
                {
                    var newNode = new Node
                    {
                        Value = list[i],
                    };
                    currNode.Next = newNode;
                    newNode.Prev = currNode;
                    currNode = newNode;
                }
            }

            public override bool IsAtEnd(int offsetFromCurrent)
            {
                if (offsetFromCurrent > 0)
                    throw new Exception("Don't use random access with a linked list.");

                return CurrentNode == null;
            }

            public override void Advance(int offsetFromCurrent)
            {
                if (offsetFromCurrent > 1)
                    throw new Exception("Don't use random access with a linked list.");

                if (CurrentNode != null)
                {
                    CurrentNode = CurrentNode.Next;
                }
            }

            public override Token<T> GetAt(int offsetFromCurrent)
            {
                if (offsetFromCurrent > 0)
                    throw new Exception("Don't use random access with a linked list.");

                return CurrentNode.Value;
            }

            // vvvv <-- Range to replace
            // x---y
            // inclusiveStart = x, afterEnd = y
            public void Replace(Node inclusiveStart, Node afterEnd, List<Token<T>> replacement)
            {
                var currNode = inclusiveStart.Prev;
                bool isReplacingFromStart = currNode == null;
                if (isReplacingFromStart)
                {
                    // Create a sentinel
                    currNode = new Node();
                }
                var firstNode = currNode;
                foreach (var tok in replacement)
                {
                    currNode.Next = new Node { Value = tok };
                    currNode.Next.Prev = currNode;
                    currNode = currNode.Next;
                }
                currNode.Next = afterEnd;
                if (currNode.Next != null)
                {
                    currNode.Next.Prev = currNode;
                }
                if (isReplacingFromStart)
                {
                    FirstNode = firstNode.Next;
                }
                CurrentNode = firstNode.Next;
            }

            public override List<Token<T>> ToList()
            {
                List<Token<T>> result = new List<Token<T>>();
                var currNode = FirstNode;
                while (currNode != null)
                {
                    result.Add(currNode.Value);
                    currNode = currNode.Next;
                }
                return result;
            }

            public override void BeginChangeCheck()
            {
                ChangeCheckNode = CurrentNode;
            }

            public override bool EndChangeCheck()
            {
                return ChangeCheckNode != CurrentNode;
            }
        }

        // Require token kinds
        protected abstract T StringLiteralTokenKind { get; }
        protected abstract T IntegerLiteralTokenKind { get; }
        protected abstract T FloatLiteralTokenKind { get; }
        protected abstract T IdentifierTokenKind { get; }
        protected abstract ParserStage Stage { get; }

        protected BaseTokenStream tokens;
        protected SourceSpan anchorSpan = default;
        protected bool throwExceptionOnError = false;

        protected List<Diagnostic> diagnostics = new List<Diagnostic>();
        public List<Diagnostic> Diagnostics => diagnostics;

        public BaseParser(List<Token<T>> tokens, bool throwExceptionOnError)
        {
            this.tokens = new ListTokenStream(tokens);
            this.throwExceptionOnError = throwExceptionOnError;
        }

        protected BaseParser(BaseTokenStream stream, bool throwExceptionOnError)
        {
            this.tokens = stream;
            this.throwExceptionOnError = throwExceptionOnError;
        }

        private Stack<(int position, SourceSpan span, int diagnosticCount)> snapshots = new Stack<(int position, SourceSpan span, int diagnosticCount)>();

        private void SnapshotState()
        {
            if (tokens is ListTokenStream stream)
            {
                snapshots.Push((stream.Index, anchorSpan, diagnostics.Count));
            }
            else
            {
                throw new Exception("Don't use snapshots with a linked list.");
            }
        }

        private void RestoreState()
        {
            if (tokens is ListTokenStream stream)
            {
                var snapshot = snapshots.Pop();
                stream.Index = snapshot.position;
                anchorSpan = snapshot.span;
                diagnostics.RemoveRange(snapshot.diagnosticCount, diagnostics.Count - snapshot.diagnosticCount);
            }
            else
            {
                throw new Exception("Don't use snapshots with a linked list.");
            }
        }

        protected void BeginChangeCheck() => tokens.BeginChangeCheck();
        protected bool EndChangeCheck() => tokens.EndChangeCheck();

        protected bool Speculate(Func<bool> parser)
        {
            SnapshotState();

            try
            {
                // Try the parser
                bool result = parser();

                // If we encountered any errors, report false
                if (diagnostics.Count > snapshots.Peek().diagnosticCount)
                {
                    return false;
                }

                // Otherwise report whatever the parser got
                return result;
            }
            finally
            {
                RestoreState();   
            }
        }

        protected bool TryParse<P>(Func<P> parser, out P parsed)
        {
            SnapshotState();

            try
            {
                // Try the parser
                parsed = parser();

                // If we encountered any errors, report false
                if (diagnostics.Count > snapshots.Peek().diagnosticCount)
                {
                    RestoreState();
                    parsed = default;
                    return false;
                }

                // Otherwise return whatever the parser got
                return true;
            }
            catch
            {
                RestoreState();
                parsed = default;
                return false;
            }
        }

        protected Token<T> Peek() => IsAtEnd() ? default : tokens.GetAt(0);
        protected Token<T> LookAhead(int offset = 1) => IsAtEnd(offset) ? default : tokens.GetAt(offset);
        protected bool Match(Func<Token<T>, bool> predicate) => predicate(Peek());
        protected bool Match(Func<T, bool> predicate) => predicate(Peek().Kind);
        protected bool Match(T kind) => Match(tok => EqualityComparer<T>.Default.Equals(tok.Kind, kind));
        protected bool Match(params T[] alternatives) => Match(tok => alternatives.Contains(tok.Kind));
        protected bool IsAtEnd(int offset = 0) => tokens.IsAtEnd(offset);
        protected Token<T> Eat(Func<Token<T>, bool> predicate)
        {
            if (!Match(predicate))
                Error($"Unexpected token '{Peek()}'.");
            return Advance();
        }
        protected Token<T> Eat(Func<T, bool> predicate)
        {
            if (!Match(predicate))
                Error($"Unexpected token '{Peek()}'.");
            return Advance();
        }
        protected Token<T> Eat(T kind)
        {
            if (!Match(kind))
                Error($"Expected token type '{kind}', got '{Peek().Kind}'.");
            return Advance();
        }
        protected Token<T> Eat(params T[] alternatives)
        {
            if (!Match(alternatives))
            {
                string allowed = string.Join(", ", alternatives);
                Error($"Unexpected token '{Peek()}', expected one of the following token types: {allowed}.");
            }
            return Advance();
        }
        protected Token<T> Advance(int amount = 1)
        {
            if (IsAtEnd(amount - 1))
                return default;
            Token<T> result = tokens.GetAt(0);
            tokens.Advance(amount);
            anchorSpan = tokens.IsAtEnd(0) ? default : tokens.GetAt(0).Span;
            return result;
        }

        protected void Error(string msg)
        {
            if (throwExceptionOnError)
            {
                throw new Exception($"Error at line {anchorSpan.Start.Line}, column {anchorSpan.Start.Column} during {Stage}: {msg}");
            }
            diagnostics.Add(new Diagnostic { Location = anchorSpan.Start, Stage = this.Stage, Text = msg });
        }

        protected void Error(string msg, SourceSpan span)
        {
            anchorSpan = span;
            Error(msg);
        }

        protected void Error(string expected, Token<T> token)
        {
            Error($"Expected {expected}, got token ({token})", token.Span);
        }

        protected string ParseIdentifier()
        {
            Token<T> identifierToken = Eat(IdentifierTokenKind);
            string identifier = identifierToken.Identifier ?? string.Empty;
            if (string.IsNullOrEmpty(identifier))
                Error("a valid identifier", identifierToken);
            return identifier;
        }

        protected string ParseStringLiteral()
        {
            Token<T> literalToken = Eat(StringLiteralTokenKind);
            return literalToken.Identifier ?? string.Empty;
        }

        protected float ParseNumericLiteral()
        {
            Token<T> literalToken = Eat(FloatLiteralTokenKind, IntegerLiteralTokenKind);
            string literal = literalToken.Identifier ?? string.Empty;
            if (string.IsNullOrEmpty(literal))
                Error("a valid numeric literal", literalToken);
            return float.Parse(literal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
        }

        protected int ParseIntegerLiteral()
        {
            return (int)ParseNumericLiteral();
        }

        protected byte ParseByteLiteral()
        {
            return (byte)ParseNumericLiteral();
        }

        protected TEnum ParseEnum<TEnum>(string expected)
            where TEnum : struct
        {
            Token<T> next = Advance();
            // ShaderLab has a lot of ambiguous syntax, many keywords are reused in multiple places as regular identifiers.
            // If we fail to use the identifier directly, it might be an overlapping keyword, so try that instead.
            string identifier = next.Identifier ?? next.Kind.ToString()?.Replace("Keyword", "") ?? String.Empty;
            if (Enum.TryParse(identifier, true, out TEnum result))
            {
                return result;
            }
            else
            {
                Error(expected, next);
                return default;
            }
        }

        #region Parser combinators
        protected List<P> ParseSeparatedList0<P>(T end, T separator, Func<P> parser, bool allowTrailingSeparator = false)
        {
            if (Match(end))
                return new List<P>();

            List<P> result = new List<P>();

            result.Add(parser());
            while (Match(separator))
            {
                BeginChangeCheck();

                Advance();
                if (!allowTrailingSeparator || !Match(end))
                {
                    result.Add(parser());
                }

                if (!EndChangeCheck())
                {
#if DEBUG
                    throw new Exception($"Parser got stuck parsing {Peek()}. Please file a bug report.");
#else
                    return result;
#endif
                }
            }

            return result;
        }

        protected List<P> ParseSeparatedList1<P>(T seperator, Func<P> parser)
        {
            List<P> result = new List<P>();

            result.Add(parser());
            while (Match(seperator))
            {
                BeginChangeCheck();

                Eat(seperator);
                result.Add(parser());

                if (!EndChangeCheck())
                {
#if DEBUG
                    throw new Exception($"Parser got stuck parsing {Peek()}. Please file a bug report.");
#else
                    return result;
#endif
                }
            }

            return result;
        }

        protected List<P> ParseMany1<P>(T first, Func<P> parser)
        {
            List<P> result = new List<P>();

            result.Add(parser());

            while (Match(first))
            {
                BeginChangeCheck();

                result.Add(parser());

                if (!EndChangeCheck())
                {
#if DEBUG
                    throw new Exception($"Parser got stuck parsing {Peek()}. Please file a bug report.");
#else
                    return result;
#endif
                }
            }

            return result;
        }

        protected List<P> ParseMany0<P>(T first, Func<P> parser)
        {
            if (!Match(first))
                return new List<P>();

            return ParseMany1(first, parser);
        }

        protected List<P> ParseMany1<P>(Func<bool> first, Func<P> parser)
        {
            List<P> result = new List<P>();

            result.Add(parser());

            while (first())
            {
                BeginChangeCheck();

                result.Add(parser());

                if (!EndChangeCheck())
                {
#if DEBUG
                    throw new Exception($"Parser got stuck parsing {Peek()}. Please file a bug report.");
#else
                    return result;
#endif
                }
            }

            return result;
        }

        protected List<P> ParseMany0<P>(Func<bool> first, Func<P> parser)
        {
            if (!first())
                return new List<P>();

            return ParseMany1(first, parser);
        }

        protected P ParseOptional<P>(T first, Func<P> parser)
        {
            if (Match(first))
                return parser();
            return default;
        }

        protected P ParseOptional<P>(Func<bool> first, Func<P> parser)
        {
            if (first())
                return parser();
            return default;
        }
#endregion
    }
}

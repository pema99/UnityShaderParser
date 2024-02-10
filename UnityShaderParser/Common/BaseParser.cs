using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityShaderParser.Common
{
    public abstract class BaseParser<T>
        where T : struct, Enum
    {
        // Require token kinds
        protected abstract T StringLiteralTokenKind { get; }
        protected abstract T IntegerLiteralTokenKind { get; }
        protected abstract T FloatLiteralTokenKind { get; }
        protected abstract T IdentifierTokenKind { get; }
        protected abstract ParserStage Stage { get; }

        protected List<Token<T>> tokens = new List<Token<T>>();
        protected int position = 0;
        protected SourceSpan anchorSpan = default;
        protected bool throwExceptionOnError = false;

        protected List<Diagnostic> diagnostics = new List<Diagnostic>();
        public List<Diagnostic> Diagnostics => diagnostics;

        public BaseParser(List<Token<T>> tokens, bool throwExceptionOnError)
        {
            // Need to copy since the parser might want to modify tokens in place
            this.tokens = new List<Token<T>>(tokens);
            this.throwExceptionOnError = throwExceptionOnError;
        }

        private Stack<(int position, SourceSpan span, int diagnosticCount)> snapshots = new Stack<(int position, SourceSpan span, int diagnosticCount)>();

        private void SnapshotState()
        {
            snapshots.Push((position, anchorSpan, diagnostics.Count));
        }

        private void RestoreState()
        {
            var snapshot = snapshots.Pop();
            position = snapshot.position;
            anchorSpan = snapshot.span;
            diagnostics.RemoveRange(snapshot.diagnosticCount, diagnostics.Count - snapshot.diagnosticCount);
        }

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

        protected Token<T> Peek() => IsAtEnd() ? default : tokens[position];
        protected Token<T> LookAhead(int offset = 1) => IsAtEnd(offset) ? default : tokens[position + offset];
        protected bool Match(Func<Token<T>, bool> predicate) => predicate(Peek());
        protected bool Match(Func<T, bool> predicate) => predicate(Peek().Kind);
        protected bool Match(T kind) => Match(tok => EqualityComparer<T>.Default.Equals(tok.Kind, kind));
        protected bool Match(params T[] alternatives) => Match(tok => alternatives.Contains(tok.Kind));
        protected bool IsAtEnd(int offset = 0) => position + offset >= tokens.Count;
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
            Token<T> result = tokens[position];
            position += amount;
            anchorSpan = Peek().Span;
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
                int lastPosition = position;

                Advance();
                if (!allowTrailingSeparator || !Match(end))
                {
                    result.Add(parser());
                }

                if (lastPosition == position)
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
                int lastPosition = position;

                Eat(seperator);
                result.Add(parser());

                if (lastPosition == position)
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
                int lastPosition = position;

                result.Add(parser());

                if (lastPosition == position)
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
                int lastPosition = position;

                result.Add(parser());

                if (lastPosition == position)
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

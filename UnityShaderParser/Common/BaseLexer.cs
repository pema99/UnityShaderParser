using System.Collections.Generic;
using System.Text;

namespace UnityShaderParser.Common
{
    public abstract class BaseLexer<T>
        where T : struct
    {
        protected abstract ParserStage Stage { get; }

        protected string source = string.Empty;
        protected int position = 0;
        protected int line = 1;
        protected int column = 1;
        protected int anchorLine = 1;
        protected int anchorColumn = 1;

        protected List<Token<T>> tokens = new List<Token<T>>();
        protected List<Diagnostic> diagnostics = new List<Diagnostic>();

        public BaseLexer(string source)
        {
            this.source = source;
        }

        protected char Peek() => IsAtEnd() ? '\0' : source[position];
        protected char LookAhead(int offset = 1) => IsAtEnd(offset) ? '\0' : source[position + offset];
        protected bool LookAhead(char c, int offset = 1) => LookAhead(offset) == c;
        protected bool Match(char tok) => Peek() == tok;
        protected bool IsAtEnd(int offset = 0) => position + offset >= source.Length;
        protected void Add(string identifier, T kind) => tokens.Add(new Token<T> { Identifier = identifier, Kind = kind, Span = GetCurrentSpan() });
        protected void Add(T kind) => tokens.Add(new Token<T>() { Kind = kind, Span = GetCurrentSpan() });
        protected void Eat(char tok)
        {
            if (!Match(tok))
                Error($"Expected token '{tok}', got '{Peek()}'.");
            Advance();
        }
        protected char Advance(int amount = 1)
        {
            if (IsAtEnd(amount - 1))
                return '\0';
            column++;
            if (Peek() == '\n')
            {
                column = 1;
                line++;
            }
            char result = source[position];
            position += amount;
            return result;
        }
        protected void Error(string err)
        {
            diagnostics.Add(new Diagnostic { Location = (line, column), Stage = this.Stage, Text = err });
        }

        protected void StartCurrentSpan()
        {
            anchorLine = line;
            anchorColumn = column;
        }

        protected SourceSpan GetCurrentSpan()
        {
            return new SourceSpan
            {
                Start = (anchorLine, anchorColumn),
                End = (line, column)
            };
        }

        protected string EatStringLiteral(char start, char end)
        {
            StringBuilder builder = new StringBuilder();
            Eat(start);
            while (Peek() != end)
            {
                builder.Append(Advance());
            }
            Eat(end);
            return builder.ToString();
        }

        protected string EatIdentifier()
        {
            StringBuilder builder = new StringBuilder();
            while (IsAlphaNumericOrUnderscore(Peek()))
            {
                builder.Append(Advance());
            }
            return builder.ToString();
        }

        protected string EatNumber(out bool isFloat)
        {
            StringBuilder builder = new StringBuilder();
            if (Match('-'))
            {
                builder.Append(Advance());
            }
            while (true)
            {
                char c = Peek();
                if (char.IsDigit(c) || c == '.' || c == 'f' || c == 'F')
                {
                    builder.Append(Advance());
                }
                // Scientific notation
                else if (c == 'e' || c == 'E')
                {
                    builder.Append(Advance());
                    if (Peek() == '-')
                        builder.Append(Advance());
                }
                else
                {
                    break;
                }
            }
            string number = builder.ToString();
            isFloat = number.Contains('.') || number.Contains('f') || number.Contains('F');
            return number;
        }

        protected void SkipWhitespace(bool skipNewLines = false)
        {
            while (Peek() == ' ' || Peek() == '\t' || Peek() == '\r' || (skipNewLines && Peek() == '\n'))
            {
                Advance();
            }
        }

        protected static bool IsAlphaNumericOrUnderscore(char c) => c == '_' || char.IsLetterOrDigit(c);

        protected abstract void ProcessChar(char nextChar);

        public void Lex()
        {
            while (!IsAtEnd())
            {
                StartCurrentSpan();
                ProcessChar(Peek());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UnityShaderParser.Common
{
    public abstract class BaseLexer<T>
        where T : struct
    {
        protected abstract ParserStage Stage { get; }

        protected bool throwExceptionOnError = false;
        protected string source = string.Empty;
        protected int position = 0;
        protected int line = 1;
        protected int column = 1;
        protected int anchorLine = 1;
        protected int anchorColumn = 1;
        protected int anchorPosition = 0;
        protected string basePath;
        protected string fileName;
        protected DiagnosticFlags diagnosticFilter = DiagnosticFlags.All;

        protected List<Token<T>> tokens = new List<Token<T>>();
        protected List<Diagnostic> diagnostics = new List<Diagnostic>();

        public BaseLexer(string source, string basePath, string fileName, bool throwExceptionOnError, SourceLocation offset)
        {
            this.source = source;
            this.throwExceptionOnError = throwExceptionOnError;
            this.line = offset.Line;
            this.column = offset.Column;
            this.basePath = basePath;
            this.fileName = fileName;
        }

        protected char Peek() => IsAtEnd() ? '\0' : source[position];
        protected char LookAhead(int offset = 1) => IsAtEnd(offset) ? '\0' : source[position + offset];
        protected bool LookAhead(char c, int offset = 1) => LookAhead(offset) == c;
        protected bool Match(char tok) => Peek() == tok;
        protected bool IsAtEnd(int offset = 0) => position + offset >= source.Length;
        protected void Add(string identifier, T kind) => tokens.Add(new Token<T>(kind, identifier, GetCurrentSpan(), tokens.Count));
        protected void Add(T kind) => tokens.Add(new Token<T>(kind, null, GetCurrentSpan(), tokens.Count));
        protected void Eat(char tok)
        {
            if (!Match(tok))
                Error(DiagnosticFlags.SyntaxError, $"Expected token '{tok}', got '{Peek()}'.");
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
        protected void Error(DiagnosticFlags kind, string err)
        {
            if (!diagnosticFilter.HasFlag(kind))
                return;

            if (throwExceptionOnError && kind != DiagnosticFlags.Warning)
            {
                throw new Exception($"Error at line {line}, column {column} during {Stage}: {err}");
            }
            diagnostics.Add(new Diagnostic(GetCurrentSpan(), kind, this.Stage, err));
        }

        protected void StartCurrentSpan()
        {
            anchorLine = line;
            anchorColumn = column;
            anchorPosition = position;
        }

        protected SourceSpan GetCurrentSpan()
        {
            return new SourceSpan(basePath, fileName, new SourceLocation(anchorLine, anchorColumn, anchorPosition), new SourceLocation(line, column, position));
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
                if (char.IsDigit(c) || c == '.')
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
            if (Match('f') || Match('F') || Match('h') || Match('H') || Match('u') || Match('U'))
            {
                builder.Append(Advance());
            }
            string number = builder.ToString();
            isFloat = number.Contains(".") ||
                number.EndsWith("f") ||
                number.EndsWith("F") ||
                number.EndsWith("h") ||
                number.EndsWith("H");
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

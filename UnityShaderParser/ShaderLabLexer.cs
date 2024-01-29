using System.Text;

namespace UnityShaderParser.ShaderLab
{
    public class ShaderLabLexer
    {
        private string source = string.Empty;
        private int position = 0;
        private int line = 1;
        private int column = 1;
        private int anchorLine = 1;
        private int anchorColumn = 1;

        private List<Token> tokens = new();
        private List<string> diagnostics = new();

        private ShaderLabLexer(string source)
        {
            this.source = source;
        }

        private char Peek() => IsAtEnd() ? '\0' : source[position];
        private char LookAhead(int offset = 1) => IsAtEnd(offset) ? '\0' : source[position + offset];
        private bool LookAhead(char c, int offset = 1) => LookAhead(offset) == c;
        private bool Match(char tok) => Peek() == tok;
        private bool IsAtEnd(int offset = 0) => position + offset >= source.Length;
        private void Add(string identifier, TokenKind kind) => tokens.Add(new Token { Identifier = identifier, Kind = kind, Span = GetCurrentSpan() });
        private void Add(TokenKind kind) => tokens.Add(new() { Kind = kind, Span = GetCurrentSpan() });
        private void Eat(char tok)
        {
            if (!Match(tok))
                diagnostics.Add($"Error at line {line} column {column}: Expected token '{tok}', got '{Peek()}'.");
            Advance();
        }
        private char Advance(int amount = 1)
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

        private void StartCurrentSpan()
        {
            anchorLine = line;
            anchorColumn = column;
        }

        private SourceSpan GetCurrentSpan()
        {
            return new SourceSpan
            {
                Start = (anchorLine, anchorColumn),
                End = (line, column)
            };
        }

        public static void Lex(string source, out List<Token> tokens, out List<string> diagnostics)
        {
            ShaderLabLexer lexer = new(source);

            lexer.Lex();

            tokens = lexer.tokens;
            diagnostics = lexer.diagnostics;
        }

        private void Lex()
        {
            while (!IsAtEnd())
            {
                StartCurrentSpan();

                switch (Peek())
                {
                    case char c when char.IsLetter(c) || c == '_':
                        LexIdentifier();
                        break;

                    case '2' when LookAhead('D') || LookAhead('d'):
                    case '3' when LookAhead('D') || LookAhead('d'):
                        LexDimensionalTextureType();
                        break;

                    case char c when char.IsDigit(c) || c == '.' || c == '-':
                        LexNumber();
                        break;

                    case '"':
                        LexString('"', '"', TokenKind.StringLiteralToken);
                        break;

                    case '[' when SyntaxFacts.IsAlphaNumericOrUnderscore(LookAhead()):
                        LexString('[', ']', TokenKind.BracketedStringLiteralToken);
                        break;

                    case ' ' or '\t' or '\r' or '\n':
                        Advance();
                        break;

                    case '/' when LookAhead('/'):
                        Advance(2);
                        while (!Match('\n'))
                        {
                            Advance();
                        }
                        Advance();
                        break;

                    case '/' when LookAhead('*'):
                        Advance(2);
                        while (!(Match('*') && LookAhead('/')))
                        {
                            Advance();
                        }
                        Advance(2);
                        break;

                    case '(': Advance(); Add(TokenKind.OpenParenToken); break;
                    case ')': Advance(); Add(TokenKind.CloseParenToken); break;
                    case '[': Advance(); Add(TokenKind.OpenBracketToken); break;
                    case ']': Advance(); Add(TokenKind.CloseBracketToken); break;
                    case '{': Advance(); Add(TokenKind.OpenBraceToken); break;
                    case '}': Advance(); Add(TokenKind.CloseBraceToken); break;
                    case ';': Advance(); Add(TokenKind.SemiToken); break;
                    case ',': Advance(); Add(TokenKind.CommaToken); break;
                    case '.': Advance(); Add(TokenKind.DotToken); break;
                    case '~': Advance(); Add(TokenKind.TildeToken); break;
                    case '?': Advance(); Add(TokenKind.QuestionToken); break;

                    case '<' when LookAhead('='): Advance(2); Add(TokenKind.LessThanEqualsToken); break;
                    case '<' when LookAhead('<') && LookAhead('=', 2): Advance(3); Add(TokenKind.LessThanLessThanEqualsToken); break;
                    case '<' when LookAhead('<'): Advance(2); Add(TokenKind.LessThanLessThanToken); break;
                    case '<': Advance(); Add(TokenKind.LessThanToken); break;

                    case '>' when LookAhead('='): Advance(2); Add(TokenKind.GreaterThanEqualsToken); break;
                    case '>' when LookAhead('>') && LookAhead('=', 2): Advance(3); Add(TokenKind.GreaterThanGreaterThanEqualsToken); break;
                    case '>' when LookAhead('>'): Advance(2); Add(TokenKind.GreaterThanGreaterThanToken); break;
                    case '>': Advance(); Add(TokenKind.GreaterThanToken); break;

                    case '+' when LookAhead('+'): Advance(2); Add(TokenKind.PlusPlusToken); break;
                    case '+' when LookAhead('='): Advance(2); Add(TokenKind.PlusEqualsToken); break;
                    case '+': Advance(); Add(TokenKind.PlusToken); break;

                    case '-' when LookAhead('+'): Advance(2); Add(TokenKind.MinusMinusToken); break;
                    case '-' when LookAhead('='): Advance(2); Add(TokenKind.MinusEqualsToken); break;
                    case '-': Advance(); Add(TokenKind.MinusToken); break;

                    case '*' when LookAhead('='): Advance(2); Add(TokenKind.AsteriskEqualsToken); break;
                    case '*': Advance(); Add(TokenKind.AsteriskToken); break;

                    case '/' when LookAhead('='): Advance(2); Add(TokenKind.SlashEqualsToken); break;
                    case '/': Advance(); Add(TokenKind.SlashToken); break;

                    case '%' when LookAhead('='): Advance(2); Add(TokenKind.PercentEqualsToken); break;
                    case '%': Advance(); Add(TokenKind.PercentToken); break;

                    case '&' when LookAhead('&'): Advance(2); Add(TokenKind.AmpersandAmpersandToken); break;
                    case '&' when LookAhead('='): Advance(2); Add(TokenKind.AmpersandEqualsToken); break;
                    case '&': Advance(); Add(TokenKind.AmpersandToken); break;

                    case '|' when LookAhead('|'): Advance(2); Add(TokenKind.BarBarToken); break;
                    case '|' when LookAhead('='): Advance(2); Add(TokenKind.BarEqualsToken); break;
                    case '|': Advance(); Add(TokenKind.BarToken); break;

                    case '^' when LookAhead('='): Advance(2); Add(TokenKind.CaretEqualsToken); break;
                    case '^': Advance(); Add(TokenKind.CaretToken); break;

                    case ':' when LookAhead(':'): Advance(2); Add(TokenKind.ColonColonToken); break;
                    case ':': Advance(); Add(TokenKind.ColonToken); break;

                    case '=' when LookAhead('='): Advance(2); Add(TokenKind.EqualsEqualsToken); break;
                    case '=': Advance(); Add(TokenKind.EqualsToken); break;

                    case '!' when LookAhead('='): Advance(2); Add(TokenKind.ExclamationEqualsToken); break;
                    case '!': Advance(); Add(TokenKind.NotToken); break;

                    case char c:
                        Advance();
                        diagnostics.Add($"Error at line {line} column {column}: Unexpected token '{c}'.");
                        break;
                }
            }
        }

        private string SkipProgramBody(string expectedEnd)
        {
            StringBuilder builder = new();
            while (true)
            {
                // If there is still space for the terminator
                if (!IsAtEnd(expectedEnd.Length))
                {
                    // And we have reached the terminator, stop
                    if (source.Substring(position, expectedEnd.Length) == expectedEnd)
                    {
                        Advance(expectedEnd.Length);
                        break;
                    }

                    // Otherwise advance
                    builder.Append(Advance());
                }
                // No space for terminator, error
                else
                {
                    diagnostics.Add($"Error at line {line} column {column}: Unterminated program block.");
                    break;
                }
            }

            return builder.ToString();
        }

        private void LexDimensionalTextureType()
        {
            StringBuilder builder = new();
            builder.Append(Advance());
            while (char.IsLetter(Peek()))
            {
                builder.Append(Advance());
            }

            switch (builder.ToString().ToLower())
            {
                case "2darray": Add(TokenKind._2DArrayKeyword); break;
                case "3darray": Add(TokenKind._3DArrayKeyword); break;
                case "2d": Add(TokenKind._2DKeyword); break;
                case "3d": Add(TokenKind._3DKeyword); break;
            }
        }

        private void LexIdentifier()
        {
            StringBuilder builder = new();
            while (SyntaxFacts.IsAlphaNumericOrUnderscore(Peek()))
            {
                builder.Append(Advance());
            }
            string identifier = builder.ToString();
            if (SyntaxFacts.TryParseShaderLabKeyword(identifier, out TokenKind token))
            {
                if (token == TokenKind.CgProgramKeyword)
                {
                    string body = SkipProgramBody("ENDCG");
                    Add(body, TokenKind.ProgramBlock);
                }
                else if (token == TokenKind.CgIncludeKeyword)
                {
                    string body = SkipProgramBody("ENDCG");
                    Add(body, TokenKind.IncludeBlock);
                }
                else if (token == TokenKind.HlslProgramKeyword)
                {
                    string body = SkipProgramBody("ENDHLSL");
                    Add(body, TokenKind.ProgramBlock);
                }
                else if (token == TokenKind.HlslIncludeKeyword)
                {
                    string body = SkipProgramBody("ENDHLSL");
                    Add(body, TokenKind.IncludeBlock);
                }
                else if (token == TokenKind.GlslProgramKeyword)
                {
                    string body = SkipProgramBody("ENDGLSL");
                    Add(body, TokenKind.ProgramBlock);
                }
                else if (token == TokenKind.GlslIncludeKeyword)
                {
                    string body = SkipProgramBody("ENDGLSL");
                    Add(body, TokenKind.IncludeBlock);
                }
                else
                {
                    Add(token);
                }
            }
            else
            {
                Add(identifier, TokenKind.IdentifierToken);
            }
        }

        private void LexNumber()
        {
            StringBuilder builder = new();
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
                else
                {
                    break;
                }
            }
            string number = builder.ToString();
            TokenKind kind = TokenKind.IntegerLiteralToken;
            if (number.Contains('.') || number.Contains('f') || number.Contains('F'))
            {
                kind = TokenKind.FloatLiteralToken;
            }
            Add(number, kind);
        }

        private void LexString(char start, char end, TokenKind kind)
        {
            StringBuilder builder = new();
            Eat(start);
            while (Peek() != end)
            {
                builder.Append(Advance());
            }
            Eat(end);
            Add(builder.ToString(), kind);
        }
    }
}

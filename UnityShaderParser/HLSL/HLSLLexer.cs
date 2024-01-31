﻿using System.Text;
using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    using Token = Token<TokenKind>;

    public class HLSLLexer : BaseLexer<TokenKind>
    {
        public HLSLLexer(string source)
            : base(source) { }

        public static void Lex(string source, out List<Token> tokens, out List<string> diagnostics)
        {
            HLSLLexer lexer = new(source);

            lexer.Lex();

            tokens = lexer.tokens;
            diagnostics = lexer.diagnostics;
        }

        protected override void ProcessChar(char nextChar)
        {
            switch (nextChar)
            {
                case char c when char.IsLetter(c) || c == '_':
                    LexIdentifier();
                    break;

                case char c when char.IsDigit(c) || c == '.' || c == '-':
                    string num = EatNumber(out bool isFloat);
                    TokenKind kind = isFloat ? TokenKind.FloatLiteralToken : TokenKind.IntegerLiteralToken;
                    Add(num, kind);
                    break;

                case '\'':
                    Add(EatStringLiteral('\'', '\''), TokenKind.CharacterLiteralToken);
                    break;

                case '"':
                    Add(EatStringLiteral('"', '"'), TokenKind.StringLiteralToken);
                    break;

                case '[' when IsAlphaNumericOrUnderscore(LookAhead()):
                    Add(EatStringLiteral('[', ']'), TokenKind.BracketedStringLiteralToken);
                    break;

                case ' ' or '\t' or '\r' or '\n':
                    Advance();
                    break;

                case '/' when LookAhead('/'):
                    Advance(2);
                    while (!Match('\n'))
                    {
                        Advance();
                        if (IsAtEnd())
                            break;
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

                case '#' when LookAhead('#'): Advance(2); Add(TokenKind.HashHashToken); break;
                case '#': Advance(); Add(TokenKind.HashToken); break;

                case char c:
                    Advance();
                    diagnostics.Add($"Error at line {line} column {column}: Unexpected token '{c}'.");
                    break;
            }
        }

        private void LexIdentifier()
        {
            string identifier = EatIdentifier();
            if (HLSLSyntaxFacts.TryParseHLSLKeyword(identifier, out TokenKind token))
            {
                Add(token);
            }
            else
            {
                Add(identifier, TokenKind.IdentifierToken);
            }
        }
    }
}
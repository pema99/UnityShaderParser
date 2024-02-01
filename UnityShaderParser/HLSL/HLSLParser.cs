using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    using Token = Token<TokenKind>;

    public class HLSLParser : BaseParser<TokenKind>
    {
        public HLSLParser(List<Token> tokens)
            : base(tokens) { }

        protected override TokenKind StringLiteralTokenKind => TokenKind.StringLiteralToken;
        protected override TokenKind BracketedStringLiteralTokenKind => TokenKind.BracketedStringLiteralToken;
        protected override TokenKind IntegerLiteralTokenKind => TokenKind.IntegerLiteralToken;
        protected override TokenKind FloatLiteralTokenKind => TokenKind.FloatLiteralToken;
        protected override TokenKind IdentifierTokenKind => TokenKind.IdentifierToken;

        public static void Parse(List<Token> tokens, out List<HLSLSyntaxNode> rootNodes, out List<string> diagnostics)
        {
            HLSLParser parser = new(tokens);

            rootNodes = parser.ParseTopLevelDeclarations();
            diagnostics = parser.diagnostics;
        }

        private List<HLSLSyntaxNode> ParseTopLevelDeclarations()
        {
            List<HLSLSyntaxNode> result = new();

            while (!IsAtEnd())
            {
                switch (Peek().Kind)
                {
                    // TODO: class definition
                    // TODO: intereface definition
                    // TODO: variable declaration statement
                    // TODO: struct definition
                    // TODO: constant buffer
                    // TODO: function definition
                    // TODO: function prototype (declaration)
                    case TokenKind.ClassKeyword:
                    case TokenKind.InterfaceKeyword:
                    case TokenKind.StructKeyword:
                    case TokenKind.ConstantBufferKeyword:
                        throw new NotImplementedException();

                    default:
                        // TODO: Disambguiate function prototypes and variable declarations (fuck C syntax)
                        result.Add(ParseFunction());
                        break;
                }
            }

            return result;
        }

        enum PrecedenceLevel
        {                 // Associativity:
            Compound,     // left
            Assignment,   // right
            Ternary,      // left
            LogicalOr,    // left
            LogicalAnd,   // left
            BitwiseOr,    // left
            BitwiseXor,   // left
            BitwiseAnd,   // left
            Equality,     // left
            Comparison,   // left
            BitShift,     // left
            AddSub,       // left
            MulDivMod,    // left
            PrefixUnary,  // right
            PostFixUnary, // left
        }

        // https://en.cppreference.com/w/c/language/operator_precedence
        List<(HashSet<TokenKind> operators, bool rightAssociative, Func<ExpressionNode, TokenKind, ExpressionNode, ExpressionNode> ctor)> operatorGroups = new ()
        {
            // TODO: Compound expression

            // Assignment
            (new() {
                TokenKind.EqualsToken, TokenKind.PlusEqualsToken, TokenKind.MinusEqualsToken,
                TokenKind.AsteriskEqualsToken, TokenKind.SlashEqualsToken, TokenKind.PercentEqualsToken,
                TokenKind.LessThanLessThanEqualsToken, TokenKind.GreaterThanGreaterThanEqualsToken,
                TokenKind.AmpersandEqualsToken, TokenKind.CaretEqualsToken, TokenKind.BarEqualsToken },
            true,
            (l, op, r) => new AssignmentExpressionNode { Left = l, Operator = op, Right = r }),

            // TODO: Ternary

            // LogicalOr
            (new() { TokenKind.BarBarToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // LogicalAnd
            (new() { TokenKind.AmpersandAmpersandToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // BitwiseOr
            (new() { TokenKind.BarToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // BitwiseXor
            (new() { TokenKind.CaretToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // BitwiseAnd
            (new() { TokenKind.AmpersandToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // Equality
            (new() { TokenKind.EqualsEqualsToken, TokenKind.ExclamationEqualsToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // Comparison
            (new() { TokenKind.LessThanToken, TokenKind.LessThanEqualsToken, TokenKind.GreaterThanToken, TokenKind.GreaterThanEqualsToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // BitShift
            (new() { TokenKind.LessThanLessThanToken, TokenKind.GreaterThanGreaterThanToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // AddSub
            (new() { TokenKind.PlusToken, TokenKind.MinusToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // MulDivMod
            (new() { TokenKind.AsteriskToken, TokenKind.SlashToken, TokenKind.PercentToken },
            false,
            (l, op, r) => new BinaryExpressionNode { Left = l, Operator = op, Right = r }),

            // TODO: Prefix unary
            // TODO: Postfix unary

            // Binds most tightly
        };

        private LiteralExpressionNode ParseLiteralExpression()
        {
            return new LiteralExpressionNode { Lexeme = ParseNumericLiteral().ToString() };
        }

        private ExpressionNode ParseTermExpression()
        {
            return ParseLiteralExpression();
        }

        private ExpressionNode ParseBinaryExpression(int level = 0)
        {
            if (level >= operatorGroups.Count)
            {
                return ParseTermExpression();
            }

            ExpressionNode higher = ParseBinaryExpression(level + 1);

            var group = operatorGroups[level];
            while (Match(tok => group.operators.Contains(tok.Kind)))
            {
                Token next = Advance();

                higher = group.ctor(
                    higher,
                    next.Kind,
                    ParseBinaryExpression(group.rightAssociative ? level : level + 1));

                if (IsAtEnd())
                {
                    return higher;
                }
            }

            return higher;
        }

        private ExpressionNode ParseExpression()
        {
            // TODO:
            // - Function invocation            \
            // - Method invocation              | - Group these 3
            // - Numeric constructor invocation /
            // - Array initializer
            // - Assignment
            // - Binary op
            // - Cast
            // - Compound
            // - Ternary
            // - Element access
            // - Field access
            // - Literal (incl strings)
            // - Paranthesized expression
            // - Prefix unary
            // - Postfix unary?
            // - Compile (wtf is this?)

            // Ambiguity groupings:
            // - Function invocation i(

            // - Literal (incl strings) num|"|keyword

            // - Array initializer {e

            // - Prefix unary _e

            // - Paranthesized expression (e
            // - Cast (i

            // - Numeric constructor invocation k(
            // - Compile (wtf is this?)

            // - Method invocation e.
            // - Ternary e?
            // - Binary op e_
            // - Compound e,
            // - Element access e[
            // - Field access e.
            // - Postfix unary? e_
            // - Assignment e=

            return ParseBinaryExpression();
        }

        private AttributeNode ParseAttribute()
        {
            Eat(TokenKind.OpenBracketToken);

            string identifier = ParseIdentifier();

            List<LiteralExpressionNode> args = new();
            if (Match(TokenKind.OpenParenToken))
            {
                Eat(TokenKind.OpenParenToken);

                args = ParseSeparatedList1(TokenKind.CommaToken, ParseLiteralExpression);

                Eat(TokenKind.CloseParenToken);
            }

            Eat(TokenKind.CloseBracketToken);

            return new AttributeNode
            {
                Name = identifier,
                Arguments = args
            };
        }

        private FunctionNode ParseFunction()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);

            TypeNode returnType = ParseType(true);

            NameNode name = ParseName();

            Eat(TokenKind.OpenParenToken);
            List<ParameterNode> parameters = ParseMany0(() => !Match(TokenKind.CloseParenToken), ParseParameter);
            Eat(TokenKind.CloseParenToken);

            SemanticNode? semantic = ParseOptional(TokenKind.ColonToken, ParseSemantic);

            // Function prototype
            if (Match(TokenKind.SemiToken))
            {
                return new FunctionDeclarationNode
                {
                    Attributes = attributes,
                    ReturnType = returnType,
                    Name = name,
                    Parameters = parameters,
                    Semantic = semantic
                };
            }

            // Otherwise, full function
            BlockNode body  = ParseBlock();
            return new FunctionDefinitionNode
            {
                Attributes = attributes,
                ReturnType = returnType,
                Name = name,
                Parameters = parameters,
                Semantic = semantic,
                Body = body
            };
        }

        private TypeNode ParseType(bool allowVoid = false)
        {
            if (Match(TokenKind.IdentifierToken))
            {
                // TODO: Predefined names !
                return ParseName();
            }

            Token typeToken = Advance();
            if (HLSLSyntaxFacts.TryConvertToScalarType(typeToken.Kind, out ScalarType scalarType))
            {
                if (scalarType == ScalarType.Void)
                    Error("a type that isn't 'void'", typeToken);
                return new ScalarTypeNode { Kind = scalarType };
            }

            throw new NotImplementedException();
        }

        private NameNode ParseName()
        {
            string identifier = ParseIdentifier();
            var name = new IdentifierNameNode { Name = identifier };

            if (Match(TokenKind.ColonColonToken))
            {
                Eat(TokenKind.ColonColonToken);

                return new QualifiedNameNode { Left = name, Right = ParseName() };
            }
            else
            {
                return name;
            }
        }

        private ParameterNode ParseParameter()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);
            TypeNode type = ParseType();
            VariableDeclaratorNode declarator = ParseVariableDeclarator();

            return new ParameterNode
            {
                Attributes = attributes,
                ParamType = type,
                Declarator = declarator
            };
        }

        private VariableDeclaratorNode ParseVariableDeclarator()
        {
            string identifier = ParseIdentifier();

            List<VariableDeclaratorQualifierNode> qualifiers = ParseMany0(TokenKind.ColonToken, ParseVariableDeclaratorQualifierNode);
            
            ExpressionNode? initializer = null;
            if (Match(TokenKind.EqualsToken))
            {
                Eat(TokenKind.SemiToken);
                initializer = ParseExpression();
            }

            return new VariableDeclaratorNode
            {
                Name = identifier,
                Qualifiers = qualifiers,
                Initializer = initializer,
            };
        }

        private VariableDeclaratorQualifierNode ParseVariableDeclaratorQualifierNode()
        {
            return ParseSemantic();
        }

        private SemanticNode ParseSemantic()
        {
            Eat(TokenKind.ColonToken);
            string identifier = ParseIdentifier();
            return new SemanticNode { Name = identifier };
        }

        private BlockNode ParseBlock()
        {
            Eat(TokenKind.OpenBraceToken);
            List<StatementNode> statements = ParseMany0(() => !Match(TokenKind.CloseBraceToken), ParseStatement);
            Eat(TokenKind.CloseBraceToken);

            return new BlockNode
            {
                Statements = statements,
            };
        }

        private bool IsVariableDeclaration(TokenKind nextKind)
        {
            if (HLSLSyntaxFacts.IsModifier(nextKind))
                return true;
            if ((HLSLSyntaxFacts.IsBuiltinType(nextKind) || nextKind == TokenKind.IdentifierToken) && LookAhead().Kind == TokenKind.IdentifierToken)
                return true;
            return false;
        }

        private StatementNode ParseStatement()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);

            Token next = Peek();
            switch (next.Kind)
            {
                case TokenKind.OpenBraceToken:
                case TokenKind.SemiToken:
                case TokenKind.BreakKeyword:
                case TokenKind.ContinueKeyword:
                case TokenKind.DiscardKeyword:
                case TokenKind.DoKeyword:
                case TokenKind.ForKeyword:
                case TokenKind.IfKeyword:
                case TokenKind.ReturnKeyword:
                case TokenKind.SwitchKeyword:
                case TokenKind.WhileKeyword:
                case TokenKind.TypedefKeyword:
                    throw new NotImplementedException($"{next.Kind}");
                    break;

                case TokenKind.InterfaceKeyword:
                case TokenKind.StructKeyword:
                    throw new NotImplementedException();
                    break;

                case TokenKind kind when IsVariableDeclaration(kind):
                    return ParseVariableDeclarationStatement(attributes);

                default:
                    ExpressionNode expr = ParseExpression();
                    Eat(TokenKind.SemiToken);
                    return new ExpressionStatementNode { Attributes = attributes, Expression = expr };
            }
        }

        private VariableDeclarationStatementNode ParseVariableDeclarationStatement(List<AttributeNode> attributes)
        {
            TypeNode kind = ParseType();
            List<VariableDeclaratorNode> variables = ParseSeparatedList1(TokenKind.CommaToken, ParseVariableDeclarator);
            Eat(TokenKind.SemiToken);

            return new VariableDeclarationStatementNode
            {
                Kind = kind,
                Declarators = variables,
                Attributes = attributes,
            };
        }
    }
}

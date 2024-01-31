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

        private ExpressionNode ParseExpression()
        {
            // TODO:
            return ParseLiteralExpression();
        }

        private LiteralExpressionNode ParseLiteralExpression()
        {
            throw new NotImplementedException();
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
            List<StatementNode> statements = ParseMany0(() => !Match(TokenKind.CloseParenToken), ParseStatement);
            Eat(TokenKind.CloseBraceToken);

            return new BlockNode
            {
                Statements = statements,
            };
        }

        private StatementNode ParseStatement()
        {
            throw new NotImplementedException();
            return null;
        }
    }
}

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
                    case TokenKind.ClassKeyword:
                    case TokenKind.InterfaceKeyword:
                        throw new NotImplementedException(anchorSpan + ": " + Peek().ToString());

                    case TokenKind.CBufferKeyword:
                        result.Add(ParseConstantBuffer());
                        break;

                    case TokenKind.StructKeyword:
                        result.Add(ParseStructDefinition());
                        break;

                    case TokenKind kind when IsTopLevelVariableDeclaration(kind):
                        result.Add(ParseVariableDeclarationStatement(new()));
                        break;

                    default:
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
            //Ternary,      // left
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
            // TODO: Compound expression (comma)
            (new() { }, false, (l, op, r) => throw new NotImplementedException()),

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

            // Binds most tightly
        };

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

        private ExpressionNode ParseExpression(int level = 0)
        {
            if (level >= operatorGroups.Count)
            {
                return ParsePrefixOrPostFixExpression();
            }

            ExpressionNode higher = ParseExpression(level + 1);

            var group = operatorGroups[level];
            while (Match(tok => group.operators.Contains(tok.Kind)))
            {
                Token next = Advance();

                higher = group.ctor(
                    higher,
                    next.Kind,
                    ParseExpression(group.rightAssociative ? level : level + 1));

                if (IsAtEnd())
                {
                    return higher;
                }
            }

            return higher;
        }

        private ExpressionNode ParsePrefixOrPostFixExpression()
        {
            switch (Peek().Kind)
            {
                case TokenKind.PlusPlusToken:
                case TokenKind.MinusMinusToken:
                case TokenKind.PlusToken:
                case TokenKind.MinusToken:
                case TokenKind.NotToken:
                case TokenKind.TildeToken:
                    TokenKind op = Eat(HLSLSyntaxFacts.IsPrefixUnaryToken).Kind;
                    return new PrefixUnaryExpressionNode { Operator = op, Expression = ParsePrefixOrPostFixExpression() };

                case TokenKind.OpenParenToken: // TODO: Conflicts with paranthesized expressions
                    Eat(TokenKind.OpenParenToken);
                    var type = ParseType();
                    Eat(TokenKind.CloseParenToken);
                    return new CastExpressionNode { Kind = type, Expression = ParsePrefixOrPostFixExpression() };

                default:
                    // Special case for constructors of built-in types. Their target is not an expression, but a keyword.
                    if (Match(HLSLSyntaxFacts.IsNumericConstructor))
                    {
                        var kind = ParseNumericType();
                        var ctorArgs = ParseParameterList();
                        return new NumericConstructorCallExpressionNode { Kind = kind, Arguments = ctorArgs };
                    }

                    var higher = ParseTerminalExpression();
                    while (true)
                    {
                        switch (Peek().Kind)
                        {
                            case TokenKind.PlusPlusToken:
                            case TokenKind.MinusMinusToken:
                                throw new NotImplementedException(anchorSpan + ": " + Peek().ToString());

                            case TokenKind.OpenParenToken when higher is NamedExpressionNode target:
                                var funcArgs = ParseParameterList();
                                higher = new FunctionCallExpressionNode { Name = target, Arguments = funcArgs };
                                break;

                            case TokenKind.OpenBracketToken:
                                throw new NotImplementedException(anchorSpan + ": " + Peek().ToString());

                            case TokenKind.DotToken:
                                Eat(TokenKind.DotToken);
                                string identifier = ParseIdentifier();

                                if (Match(TokenKind.OpenParenToken))
                                {
                                    var methodArgs = ParseParameterList();
                                    higher = new MethodCallExpressionNode { Target = higher, Name = identifier, Arguments = methodArgs };
                                }
                                else
                                {
                                    higher = new FieldAccessExpressionNode { Target = higher, Name = identifier };
                                }
                                break;

                            default:
                                return higher;
                        }
                    }
            }
        }

        private LiteralExpressionNode ParseLiteralExpression()
        {
            Token next = Advance();
            string lexeme = next.Identifier ?? string.Empty;

            if (!HLSLSyntaxFacts.TryConvertLiteralKind(next.Kind, out var literalKind))
            {
                Error("a valid literal expression", next);
            }

            return new LiteralExpressionNode { Lexeme = lexeme, Kind = literalKind };
        }

        private ExpressionNode ParseTerminalExpression()
        {
            if (Match(TokenKind.IdentifierToken))
            {
                string identifier = ParseIdentifier();
                return new IdentifierExpressionNode { Name = identifier };
            }

            return ParseLiteralExpression();
        }

        private List<ExpressionNode> ParseParameterList()
        {
            Eat(TokenKind.OpenParenToken);
            List<ExpressionNode> exprs = ParseSeparatedList0(
                () => !Match(TokenKind.CloseParenToken),
                TokenKind.CommaToken,
                () => ParseExpression((int)PrecedenceLevel.Compound + 1));
            Eat(TokenKind.CloseParenToken);
            return exprs;
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

            UserDefinedTypeNode name = ParseUserDefinedTypeName();

            Eat(TokenKind.OpenParenToken);
            List<FormalParameterNode> parameters = ParseSeparatedList0(() => !Match(TokenKind.CloseParenToken), TokenKind.CommaToken, ParseFormalParameter);
            Eat(TokenKind.CloseParenToken);

            SemanticNode? semantic = ParseOptional(TokenKind.ColonToken, ParseSemantic);

            // Function prototype
            if (Match(TokenKind.SemiToken))
            {
                Eat(TokenKind.SemiToken);
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

        private StructDefinitionNode ParseStructDefinition()
        {
            Eat(TokenKind.StructKeyword);
            var name = ParseUserDefinedTypeName();

            // base list
            List<UserDefinedTypeNode> baseList = new();
            if (Match(TokenKind.ColonToken))
            {
                Eat(TokenKind.ColonToken);

                baseList = ParseSeparatedList1(TokenKind.CommaToken, ParseUserDefinedTypeName);
            }

            Eat(TokenKind.OpenBraceToken);

            List<VariableDeclarationStatementNode> decls = ParseMany0(
                () => !Match(TokenKind.CloseBraceToken),
                () => ParseVariableDeclarationStatement(new()));

            Eat(TokenKind.CloseBraceToken);
            Eat(TokenKind.SemiToken);

            return new StructDefinitionNode
            {
                Name = name,
                Inherits = baseList,
                Declarations = decls
            };
        }

        private ConstantBufferNode ParseConstantBuffer()
        {
            Eat(TokenKind.CBufferKeyword);
            var name = ParseUserDefinedTypeName();

            RegisterLocationNode? reg = null;
            if (Match(TokenKind.ColonToken))
            {
                reg = ParseRegisterLocation();
            }

            Eat(TokenKind.OpenBraceToken);

            List<VariableDeclarationStatementNode> decls = ParseMany0(
                () => !Match(TokenKind.CloseBraceToken),
                () => ParseVariableDeclarationStatement(new()));

            Eat(TokenKind.CloseBraceToken);
            Eat(TokenKind.SemiToken);

            return new ConstantBufferNode
            {
                Name = name,
                RegisterLocation = reg,
                Declarations = decls
            };
        }

        private TypeNode ParseType(bool allowVoid = false)
        {
            if (Match(TokenKind.IdentifierToken))
            {
                // TODO: Predefined names !
                return ParseUserDefinedTypeName();
            }

            // TODO: Modifiers

            return ParseNumericType();
        }

        private NumericTypeNode ParseNumericType(/* List<Modifier> Modifiers */)
        {
            Token typeToken = Advance();
            if (HLSLSyntaxFacts.TryConvertToScalarType(typeToken.Kind, out ScalarType scalarType))
            {
                if (scalarType == ScalarType.Void)
                    Error("a type that isn't 'void'", typeToken);
                return new ScalarTypeNode { Kind = scalarType };
            }

            if (HLSLSyntaxFacts.TryConvertToMonomorphicVectorType(typeToken.Kind, out ScalarType vectorType, out int dimension))
            {
                return new VectorTypeNode { Kind = vectorType, Dimension = dimension };
            }

            if (HLSLSyntaxFacts.TryConvertToMonomorphicMatrixType(typeToken.Kind, out ScalarType matrixType, out int dimX, out int dimY))
            {
                return new MatrixTypeNode { Kind = matrixType, FirstDimension = dimX, SecondDimension = dimY };
            }
            // TODO: Generic vector and matrix types

            throw new NotImplementedException(anchorSpan + ": " + typeToken.ToString());
        }

        private UserDefinedTypeNode ParseUserDefinedTypeName()
        {
            string identifier = ParseIdentifier();
            var name = new NamedTypeNode { Name = identifier };

            if (Match(TokenKind.ColonColonToken))
            {
                Eat(TokenKind.ColonColonToken);

                return new QualifiedNamedTypeNode { Left = name, Right = ParseUserDefinedTypeName() };
            }
            else
            {
                return name;
            }
        }

        private FormalParameterNode ParseFormalParameter()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);
            TypeNode type = ParseType();
            VariableDeclaratorNode declarator = ParseVariableDeclarator();

            return new FormalParameterNode
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
                Eat(TokenKind.EqualsToken);
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
            switch (LookAhead().Kind)
            {
                case TokenKind.IdentifierToken: return ParseSemantic();
                case TokenKind.RegisterKeyword: return ParseRegisterLocation();
                default: throw new NotImplementedException(anchorSpan + ": " + Peek().ToString());
            }
        }

        private SemanticNode ParseSemantic()
        {
            Eat(TokenKind.ColonToken);
            string identifier = ParseIdentifier();
            return new SemanticNode { Name = identifier };
        }

        private RegisterLocationNode ParseRegisterLocation()
        {
            Eat(TokenKind.ColonToken);
            Eat(TokenKind.RegisterKeyword);
            Eat(TokenKind.OpenParenToken);

            string location = ParseIdentifier();
            RegisterKind kind = default;
            int index = 0;
            switch (location.ToLower().FirstOrDefault())
            {
                case 't': kind = RegisterKind.Texture; break;
                case 'b': kind = RegisterKind.Buffer; break;
                case 'u': kind = RegisterKind.UAV; break;
                case 's': kind = RegisterKind.Sampler; break;
                default: break;
            }
            string indexLexeme = string.Concat(location.SkipWhile(x => !char.IsNumber(x)));
            if (!int.TryParse(indexLexeme, out index))
            {
                Error($"Expected a valid register location, but got '{location}'.");
            }

            int? spaceIndex = null;
            if (Match(TokenKind.CommaToken))
            {
                Eat(TokenKind.CommaToken);

                string space = ParseIdentifier();
                string spaceLexeme = string.Concat(space.SkipWhile(x => !char.IsNumber(x)));
                if (int.TryParse(spaceLexeme, out int parsedIndex))
                {
                    spaceIndex = parsedIndex;
                }
                else
                {
                    Error($"Expected a valid register location, but got '{location}'.");
                }
            }

            Eat(TokenKind.CloseParenToken);

            return new RegisterLocationNode
            {
                Kind = kind,
                Location = index,
                Space = spaceIndex,
            };
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

        private bool IsVariableDeclarationStatement(TokenKind nextKind)
        {
            if (HLSLSyntaxFacts.IsModifier(nextKind))
                return true;
            if ((HLSLSyntaxFacts.IsBuiltinType(nextKind) || nextKind == TokenKind.IdentifierToken) && LookAhead().Kind == TokenKind.IdentifierToken)
                return true;
            return false;
        }

        private bool IsTopLevelVariableDeclaration(TokenKind nextKind)
        {
            // Skip modifiers (they don't disambguiate top level declarations)
            int offset = 0;
            if (HLSLSyntaxFacts.IsModifier(nextKind))
            {
                offset = 1;
                while (HLSLSyntaxFacts.IsModifier(LookAhead(offset).Kind))
                {
                    offset++;
                }
            }
            nextKind = LookAhead(offset).Kind;

            // TODO: This will break for qualified return types
            if ((HLSLSyntaxFacts.IsBuiltinType(nextKind) || nextKind == TokenKind.IdentifierToken) &&
                LookAhead(offset + 1).Kind == TokenKind.IdentifierToken &&
                LookAhead(offset + 2).Kind != TokenKind.OpenParenToken)
                return true;

            return false;
        }

        private StatementNode ParseStatement()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);

            Token next = Peek();
            switch (next.Kind)
            {
                case TokenKind.ReturnKeyword:
                    Advance();
                    ExpressionNode returnExpr = ParseExpression();
                    Eat(TokenKind.SemiToken);
                    return new ReturnStatementNode { Attributes = attributes, Expression = returnExpr };

                case TokenKind.OpenBraceToken:
                case TokenKind.SemiToken:
                case TokenKind.BreakKeyword:
                case TokenKind.ContinueKeyword:
                case TokenKind.DiscardKeyword:
                case TokenKind.DoKeyword:
                case TokenKind.ForKeyword:
                case TokenKind.IfKeyword:
                case TokenKind.SwitchKeyword:
                case TokenKind.WhileKeyword:
                case TokenKind.TypedefKeyword:
                    throw new NotImplementedException(anchorSpan + ": " + Peek().ToString());
                    break;

                case TokenKind.InterfaceKeyword:
                case TokenKind.StructKeyword:
                    throw new NotImplementedException(anchorSpan + ": " + Peek().ToString());
                    break;

                case TokenKind kind when IsVariableDeclarationStatement(kind):
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

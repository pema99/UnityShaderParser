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
                    case TokenKind.ClassKeyword:
                    case TokenKind.InterfaceKeyword:
                        throw new NotImplementedException(Peek().Span + ": " + Peek().ToString());

                    case TokenKind.CBufferKeyword:
                    case TokenKind.TBufferKeyword:
                        result.Add(ParseConstantBuffer());
                        break;

                    case TokenKind.StructKeyword:
                        result.Add(ParseStructDefinition());
                        break;

                    default:
                        if (IsNextPossiblyFunctionDeclaration())
                        {
                            result.Add(ParseFunction());
                        }
                        else
                        {
                            result.Add(ParseVariableDeclarationStatement(new()));
                        }
                        break;
                }
            }

            return result;
        }

        private bool IsNextCast()
        {
            int offset = 0;

            // Must have initial paren
            if (LookAhead(offset).Kind != TokenKind.OpenParenToken)
                return false;
            offset++;

            // If we mention a builtin type - it's a cast
            if (HLSLSyntaxFacts.IsBuiltinType(LookAhead(offset).Kind))
            {
                return true;
            }
            // If we have a user defined type keyword, it's a cast
            else if (LookAhead(offset).Kind is TokenKind.ClassKeyword or TokenKind.StructKeyword or TokenKind.InterfaceKeyword)
            {
                return true;
            }
            // If there is an identifier
            else if (LookAhead(offset).Kind == TokenKind.IdentifierToken)
            {
                // Take as many qualifier sections as possible
                offset++;
                while (LookAhead(offset).Kind == TokenKind.ColonColonToken)
                {
                    offset++;
                    if (LookAhead(offset).Kind != TokenKind.IdentifierToken)
                    {
                        return false;
                    }
                    offset++;
                }
            }
            // If none of the above are true, can't be a type
            else
            {
                return false;
            }

            // If we had an identifier, check if it is followed by an array type
            while (LookAhead(offset).Kind == TokenKind.OpenBracketToken)
            {
                // All arguments must be constants or identifiers
                offset++;
                if (LookAhead(offset).Kind is not TokenKind.IntegerLiteralToken or TokenKind.IdentifierToken)
                {
                    return false;
                }
                offset++;
                if (LookAhead(offset).Kind != TokenKind.CloseBracketToken)
                {
                    return false;
                }
                offset++;
            }

            // If we've reached this point, make sure the cast is closed
            if (LookAhead(offset).Kind != TokenKind.CloseParenToken)
                return false;

            // It might still be ambiguous, so check if the next token is allowed to follow a cast
            offset++;
            return HLSLSyntaxFacts.CanTokenComeAfterCast(LookAhead(offset).Kind);
        }

        private bool IsNextPossiblyFunctionDeclaration()
        {
            return Try(() =>
            {
                ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);
                ParseDeclarationModifiers();
                ParseType(true);
                ParseUserDefinedTypeName();
                return Match(TokenKind.OpenParenToken);
            });
        }

        private StatePropertyNode ParseStatePropertyNode()
        {
            string key = ParseIdentifier();
            ArrayRankNode? rank = null;
            if (Match(TokenKind.OpenBracketToken))
            {
                rank = ParseArrayRank();
            }
            Eat(TokenKind.EqualsToken);
            var expr = ParseExpression();
            Eat(TokenKind.SemiToken);

            return new StatePropertyNode
            {
                Name = key,
                ArrayRank = rank,
                Value = expr
            };
        }

        private SamplerStateLiteralExpressionNode ParseSamplerStateLiteral()
        {
            Eat(TokenKind.SamplerStateLegacyKeyword);
            Eat(TokenKind.OpenBraceToken);

            if (Match(TokenKind.TextureKeyword))
            {
                Advance();
            }
            else
            {
                string textureKeyword = ParseIdentifier();
                if (textureKeyword.ToLower() != "Texture")
                    Error($"Expected to find the keyword 'Texture' in sampler state object, got '{textureKeyword}'.");
            }

            Eat(TokenKind.EqualsToken);
            bool hasBrackets = Match(TokenKind.LessThanToken);
            if (hasBrackets) Eat(TokenKind.LessThanToken);
            NamedExpressionNode textureName = ParseNamedExpression();
            if (hasBrackets) Eat(TokenKind.GreaterThanToken);
            Eat(TokenKind.SemiToken);

            List<StatePropertyNode> states = new();
            while (Match(TokenKind.IdentifierToken))
            {
                states.Add(ParseStatePropertyNode());
            }

            Eat(TokenKind.CloseBraceToken);

            return new SamplerStateLiteralExpressionNode
            {
                TextureName = textureName,
                States = states
            };
        }

        private ExpressionNode ParseExpression(int level = 0)
        {
            if (Match(TokenKind.SamplerStateLegacyKeyword))
            {
                return ParseSamplerStateLiteral();
            }

            return ParseBinaryExpression(level);
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

        private ExpressionNode ParseBinaryExpression(int level = 0)
        {
            if (level >= operatorGroups.Count)
            {
                return ParsePrefixOrPostFixExpression();
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

                case TokenKind.OpenParenToken when IsNextCast():
                    Eat(TokenKind.OpenParenToken);
                    var type = ParseType();
                    Eat(TokenKind.CloseParenToken);
                    return new CastExpressionNode { Kind = type, Expression = ParsePrefixOrPostFixExpression() };

                case TokenKind.OpenParenToken:
                    Eat(TokenKind.OpenParenToken);
                    var expr = ParseExpression();
                    Eat(TokenKind.CloseParenToken);
                    return expr;

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
                                var incrOp = Advance().Kind;
                                higher = new PostfixUnaryExpressionNode { Expression = higher, Operator = incrOp };
                                break;

                            case TokenKind.OpenParenToken when higher is NamedExpressionNode target:
                                var funcArgs = ParseParameterList();
                                higher = new FunctionCallExpressionNode { Name = target, Arguments = funcArgs };
                                break;

                            case TokenKind.OpenBracketToken:
                                Eat(TokenKind.OpenBracketToken);
                                var indexArg = ParseExpression();
                                Eat(TokenKind.CloseBracketToken);
                                higher = new ElementAccessExpressionNode { Target = higher, Index = indexArg };
                                break;

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

        // TODO: Qualified names
        private NamedExpressionNode ParseNamedExpression()
        {
            string identifier = ParseIdentifier();
            return new IdentifierExpressionNode { Name = identifier };
        }

        private ArrayInitializerExpressionNode ParseArrayInitializer()
        {
            Eat(TokenKind.OpenBraceToken);
            var exprs = ParseSeparatedList0(
                TokenKind.CloseBraceToken,
                TokenKind.CommaToken,
                () => ParseExpression(),
                true);
            Eat(TokenKind.CloseBraceToken);
            return new ArrayInitializerExpressionNode { Elements = exprs };
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
                return ParseNamedExpression();
            }

            if (Match(TokenKind.OpenBraceToken))
            {
                return ParseArrayInitializer();
            }

            return ParseLiteralExpression();
        }

        private List<ExpressionNode> ParseParameterList()
        {
            Eat(TokenKind.OpenParenToken);
            List<ExpressionNode> exprs = ParseSeparatedList0(
                TokenKind.CloseParenToken,
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

            var modifiers = ParseDeclarationModifiers();
            TypeNode returnType = ParseType(true);

            UserDefinedTypeNode name = ParseUserDefinedTypeName();

            Eat(TokenKind.OpenParenToken);
            List<FormalParameterNode> parameters = ParseSeparatedList0(TokenKind.CloseParenToken, TokenKind.CommaToken, ParseFormalParameter);
            Eat(TokenKind.CloseParenToken);

            SemanticNode? semantic = ParseOptional(TokenKind.ColonToken, ParseSemantic);

            // Function prototype
            if (Match(TokenKind.SemiToken))
            {
                Eat(TokenKind.SemiToken);
                return new FunctionDeclarationNode
                {
                    Attributes = attributes,
                    Modifiers = modifiers,
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
                Modifiers = modifiers,
                ReturnType = returnType,
                Name = name,
                Parameters = parameters,
                Semantic = semantic,
                Body = body
            };
        }

        private StructDefinitionNode ParseStructDefinition()
        {
            var modifiers = ParseDeclarationModifiers();

            Eat(TokenKind.StructKeyword);
            var name = ParseOptional(TokenKind.IdentifierToken, ParseUserDefinedTypeName);

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
                Modifiers = modifiers,
                Name = name,
                Inherits = baseList,
                Declarations = decls
            };
        }

        private ConstantBufferNode ParseConstantBuffer()
        {
            var buffer = Eat(TokenKind.CBufferKeyword, TokenKind.TBufferKeyword);
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
            if (Match(TokenKind.SemiToken))
            {
                Eat(TokenKind.SemiToken);
            }

            return new ConstantBufferNode
            {
                Name = name,
                RegisterLocation = reg,
                Declarations = decls,
                IsTextureBuffer = buffer.Kind == TokenKind.TBufferKeyword
            };
        }

        private TypeNode ParseType(bool allowVoid = false)
        {
            if (Match(TokenKind.IdentifierToken))
            {
                return ParseUserDefinedTypeName();
            }

            if (HLSLSyntaxFacts.TryConvertToPredefinedObjectType(Peek().Kind, out PredefinedObjectType predefinedType))
            {
                Advance();

                List<TypeNode> args = new();
                if (Match(TokenKind.LessThanToken))
                {
                    Eat(TokenKind.LessThanToken);
                    args = ParseSeparatedList0(
                        TokenKind.GreaterThanToken,
                        TokenKind.CommaToken,
                        ParseTemplateArgumentType);
                    Eat(TokenKind.GreaterThanToken);
                }

                return new PredefinedObjectTypeNode
                {
                    Kind = predefinedType,
                    TemplateArguments = args,
                };
            }

            // TODO: Modifiers

            return ParseNumericType(allowVoid);
        }

        private NumericTypeNode ParseNumericType(bool allowVoid = false/* List<Modifier> Modifiers */)
        {
            Token typeToken = Advance();
            if (HLSLSyntaxFacts.TryConvertToScalarType(typeToken.Kind, out ScalarType scalarType))
            {
                if (scalarType == ScalarType.Void && !allowVoid)
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

            throw new NotImplementedException(typeToken.Span.ToString() + ": " + typeToken.ToString());
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

        private TypeNode ParseTemplateArgumentType()
        {
            if (Match(TokenKind.CharacterLiteralToken, TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken, TokenKind.StringLiteralToken))
            {
                var expression = ParseLiteralExpression();
                return new LiteralTemplateArgumentType { Literal = expression };
            }

            return ParseType();
        }

        private FormalParameterNode ParseFormalParameter()
        {

            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);
            var modifiers = ParseParameterModifiers();
            TypeNode type = ParseType();
            VariableDeclaratorNode declarator = ParseVariableDeclarator();

            return new FormalParameterNode
            {
                Attributes = attributes,
                Modifiers = modifiers,
                ParamType = type,
                Declarator = declarator
            };
        }

        private ArrayRankNode ParseArrayRank()
        {
            Eat(TokenKind.OpenBracketToken);
            ExpressionNode? expr = null;
            if (!Match(TokenKind.CloseBracketToken))
            {
                expr = ParseExpression();
            }
            Eat(TokenKind.CloseBracketToken);
            return new ArrayRankNode { Dimension = expr };
        }

        private VariableDeclaratorNode ParseVariableDeclarator()
        {
            string identifier = ParseIdentifier();

            List<ArrayRankNode> arrayRanks = new();
            while (Match(TokenKind.OpenBracketToken))
            {
                arrayRanks.Add(ParseArrayRank());
            }

            List<VariableDeclaratorQualifierNode> qualifiers = ParseMany0(TokenKind.ColonToken, ParseVariableDeclaratorQualifierNode);

            List<VariableDeclarationStatementNode> annotations = new();
            if (Match(TokenKind.LessThanToken))
            {
                Eat(TokenKind.LessThanToken);
                annotations = ParseMany0(() => !Match(TokenKind.GreaterThanToken), () => ParseVariableDeclarationStatement(new()));
                Eat(TokenKind.GreaterThanToken);
            }

            InitializerNode? initializer = null;
            if (Match(TokenKind.EqualsToken))
            {
                initializer = ParseValueInitializer();
            }
            else if (Match(TokenKind.OpenBraceToken))
            {
                initializer = ParseStateInitializer();
            }

            return new VariableDeclaratorNode
            {
                Name = identifier,
                ArrayRanks = arrayRanks,
                Qualifiers = qualifiers,
                Annotations = annotations,
                Initializer = initializer,
            };
        }

        private ValueInitializerNode ParseValueInitializer()
        {
            Eat(TokenKind.EqualsToken);
            var expr = ParseExpression();
            return new ValueInitializerNode { Expression = expr };
        }

        private StateInitializerNode ParseStateInitializer()
        {
            Eat(TokenKind.OpenBraceToken);
            List<StatePropertyNode> states = new();
            while (Match(TokenKind.IdentifierToken))
            {
                states.Add(ParseStatePropertyNode());
            }
            Eat(TokenKind.CloseBraceToken);
            return new StateInitializerNode { States = states };
        }

        private VariableDeclaratorQualifierNode ParseVariableDeclaratorQualifierNode()
        {
            switch (LookAhead().Kind)
            {
                case TokenKind.IdentifierToken: return ParseSemantic();
                case TokenKind.RegisterKeyword: return ParseRegisterLocation();
                case TokenKind.PackoffsetKeyword: return ParsePackoffsetNode();
                default: return ParseSemantic();
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

        private PackoffsetNode ParsePackoffsetNode()
        {
            Eat(TokenKind.ColonToken);
            Eat(TokenKind.PackoffsetKeyword);
            Eat(TokenKind.OpenParenToken);

            string location = ParseIdentifier();
            int index = 0;
            string indexLexeme = string.Concat(location.SkipWhile(x => !char.IsNumber(x)));
            if (!int.TryParse(indexLexeme, out index))
            {
                Error($"Expected a valid packoffset location, but got '{location}'.");
            }

            string? swizzle = null;
            if (Match(TokenKind.DotToken))
            {
                Eat(TokenKind.DotToken);
                swizzle = ParseIdentifier();
            }

            Eat(TokenKind.CloseParenToken);

            return new PackoffsetNode
            {
                Location = index,
                Swizzle = swizzle,
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

        private StatementNode ParseStatement()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);

            Token next = Peek();
            switch (next.Kind)
            {
                case TokenKind.SemiToken:
                    Advance();
                    return new EmptyStatementNode { };

                case TokenKind.OpenBraceToken:
                    return ParseBlock();

                case TokenKind.ReturnKeyword:
                    Advance();
                    ExpressionNode returnExpr = ParseExpression();
                    Eat(TokenKind.SemiToken);
                    return new ReturnStatementNode { Attributes = attributes, Expression = returnExpr };

                case TokenKind.ForKeyword:
                    return ParseForStatement(attributes);

                case TokenKind.IfKeyword:
                    return ParseIfStatement(attributes);

                case TokenKind.BreakKeyword:
                    Advance();
                    return new BreakStatementNode { Attributes = attributes };

                case TokenKind.ContinueKeyword:
                    Advance();
                    return new ContinueStatementNode { Attributes = attributes };

                case TokenKind.DiscardKeyword:
                    Advance();
                    return new DiscardStatementNode { Attributes = attributes };

                case TokenKind.DoKeyword:
                case TokenKind.SwitchKeyword:
                case TokenKind.WhileKeyword:
                case TokenKind.TypedefKeyword:
                    throw new NotImplementedException(next.Span + ": " + next.Kind.ToString());

                case TokenKind.InterfaceKeyword:
                case TokenKind.ClassKeyword:
                    throw new NotImplementedException(next.Span + ": " + next.Kind.ToString());

                case TokenKind.StructKeyword:
                    return ParseStructDefinition();

                case TokenKind kind when IsVariableDeclarationStatement(kind):
                    return ParseVariableDeclarationStatement(attributes);

                default:
                    ExpressionNode expr = ParseExpression();
                    Eat(TokenKind.SemiToken);
                    return new ExpressionStatementNode { Attributes = attributes, Expression = expr };
            }
        }

        private List<BindingModifier> ParseParameterModifiers()
        {
            List<BindingModifier> modifiers = new();
            while (HLSLSyntaxFacts.TryConvertToParameterModifier(Peek().Kind, out var modifier))
            {
                Advance();
                modifiers.Add(modifier);
            }
            return modifiers;
        }

        private List<BindingModifier> ParseDeclarationModifiers()
        {
            List<BindingModifier> modifiers = new();
            while (HLSLSyntaxFacts.TryConvertToDeclarationModifier(Peek().Kind, out var modifier))
            {
                Advance();
                modifiers.Add(modifier);
            }
            return modifiers;
        }

        private VariableDeclarationStatementNode ParseVariableDeclarationStatement(List<AttributeNode> attributes)
        {
            var modifiers = ParseDeclarationModifiers();
            TypeNode kind = ParseType();
            List<VariableDeclaratorNode> variables = ParseSeparatedList1(TokenKind.CommaToken, ParseVariableDeclarator);
            Eat(TokenKind.SemiToken);

            return new VariableDeclarationStatementNode
            {
                Modifiers = modifiers,
                Kind = kind,
                Declarators = variables,
                Attributes = attributes,
            };
        }

        private ForStatementNode ParseForStatement(List<AttributeNode> attributes)
        {
            Eat(TokenKind.ForKeyword);
            Eat(TokenKind.OpenParenToken);

            VariableDeclarationStatementNode? decl = null;
            if (!Match(TokenKind.SemiToken))
            {
                decl = ParseVariableDeclarationStatement(new());
            }

            ExpressionNode? cond = null;
            if (!Match(TokenKind.SemiToken))
            {
                cond = ParseExpression();
            }
            Eat(TokenKind.SemiToken);

            ExpressionNode? incrementor = null;
            if (!Match(TokenKind.SemiToken))
            {
                incrementor = ParseExpression();
            }
            Eat(TokenKind.CloseParenToken);

            var body = ParseStatement();

            return new ForStatementNode
            {
                Declaration = decl,
                Condition = cond,
                Increment = incrementor,
                Body = body,
                Attributes = attributes,
            };
        }

        private IfStatementNode ParseIfStatement(List<AttributeNode> attributes)
        {
            Eat(TokenKind.IfKeyword);
            Eat(TokenKind.OpenParenToken);

            var cond = ParseExpression();

            Eat(TokenKind.CloseParenToken);

            var body = ParseStatement();

            StatementNode? elseClause = null;
            if (Match(TokenKind.ElseKeyword))
            {
                Eat(TokenKind.ElseKeyword);
                elseClause = ParseStatement();
            }

            return new IfStatementNode
            {
                Attributes = attributes,
                Condition = cond,
                Body = body,
                ElseClause = elseClause,
            };
        }
    }
}

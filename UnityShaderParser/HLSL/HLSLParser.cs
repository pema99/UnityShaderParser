using UnityShaderParser.Common;

namespace UnityShaderParser.HLSL
{
    using HLSLToken = Token<TokenKind>;

    public class HLSLParser : BaseParser<TokenKind>
    {
        public HLSLParser(List<HLSLToken> tokens)
            : base(tokens) { }

        protected override TokenKind StringLiteralTokenKind => TokenKind.StringLiteralToken;
        protected override TokenKind IntegerLiteralTokenKind => TokenKind.IntegerLiteralToken;
        protected override TokenKind FloatLiteralTokenKind => TokenKind.FloatLiteralToken;
        protected override TokenKind IdentifierTokenKind => TokenKind.IdentifierToken;

        public static void ParseTopLevelDeclarations(List<HLSLToken> tokens, out List<HLSLSyntaxNode> rootNodes, out List<string> diagnostics)
        {
            HLSLParser parser = new(tokens);
            rootNodes = parser.ParseTopLevelDeclarations();
            diagnostics = parser.diagnostics;
        }

        public static void ParseTopLevelDeclaration(List<HLSLToken> tokens, out HLSLSyntaxNode rootNode, out List<string> diagnostics)
        {
            HLSLParser parser = new(tokens);
            rootNode = parser.ParseTopLevelDeclaration();
            diagnostics = parser.diagnostics;
        }

        public static void ParseStatement(List<HLSLToken> tokens, out StatementNode statement, out List<string> diagnostics)
        {
            HLSLParser parser = new(tokens);
            statement = parser.ParseStatement();
            diagnostics = parser.diagnostics;
        }

        public static void ParseExpression(List<HLSLToken> tokens, out ExpressionNode statement, out List<string> diagnostics)
        {
            HLSLParser parser = new(tokens);
            statement = parser.ParseExpression();
            diagnostics = parser.diagnostics;
        }

        internal List<HLSLSyntaxNode> ParseTopLevelDeclarations()
        {
            List<HLSLSyntaxNode> result = new();

            while (!IsAtEnd())
            {
                result.Add(ParseTopLevelDeclaration());
            }

            return result;
        }

        internal HLSLSyntaxNode ParseTopLevelDeclaration()
        {
            switch (Peek().Kind)
            {
                case TokenKind.CBufferKeyword:
                case TokenKind.TBufferKeyword:
                    return ParseConstantBuffer();

                case TokenKind.StructKeyword:
                case TokenKind.ClassKeyword:
                    return ParseStructDefinition(new());

                case TokenKind.InterfaceKeyword:
                  return ParseInterfaceDefinition(new());

                case TokenKind.TypedefKeyword:
                    return ParseTypedef(new());

                case TokenKind.Technique10Keyword:
                case TokenKind.Technique11Keyword:
                case TokenKind.TechniqueKeyword:
                    return ParseTechnique();

                default:
                    if (IsNextPossiblyFunctionDeclaration())
                    {
                        return ParseFunction();
                    }
                    else
                    {
                        return ParseVariableDeclarationStatement(new());
                    }
            }
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
            UserDefinedTypeNode name;
            if (Match(TokenKind.TextureKeyword))
            {
                Advance();
                name = new NamedTypeNode { Name = "texture" };
            }
            else
            {
                name = ParseUserDefinedTypeName();
            }
            ArrayRankNode? rank = null;
            if (Match(TokenKind.OpenBracketToken))
            {
                rank = ParseArrayRank();
            }

            ExpressionNode expr;
            Eat(TokenKind.EqualsToken);
            bool isReference = Match(TokenKind.LessThanToken);
            if (isReference)
            {
                Eat(TokenKind.LessThanToken);
                expr = ParseNamedExpression();
                Eat(TokenKind.GreaterThanToken);
            }
            else
            {
                expr = ParseExpression();
            }
            Eat(TokenKind.SemiToken);

            return new StatePropertyNode
            {
                Name = name,
                ArrayRank = rank,
                Value = expr,
                IsReference = isReference,
            };
        }

        private SamplerStateLiteralExpressionNode ParseSamplerStateLiteral()
        {
            Eat(TokenKind.SamplerStateLegacyKeyword);
            Eat(TokenKind.OpenBraceToken);

            List<StatePropertyNode> states = new();
            while (Match(TokenKind.IdentifierToken, TokenKind.TextureKeyword))
            {
                states.Add(ParseStatePropertyNode());
            }

            Eat(TokenKind.CloseBraceToken);

            return new SamplerStateLiteralExpressionNode
            {
                States = states
            };
        }

        private CompileExpressionNode ParseCompileExpression()
        {
            Eat(TokenKind.CompileKeyword);
            string target = ParseIdentifier();

            var name = ParseNamedExpression();
            var param = ParseParameterList();
            var expr = new FunctionCallExpressionNode { Name = name, Arguments = param };

            return new CompileExpressionNode
            {
                Target = target,
                Invocation = expr
            };
        }

        internal ExpressionNode ParseExpression(int level = 0)
        {
            if (Match(TokenKind.SamplerStateLegacyKeyword))
            {
                return ParseSamplerStateLiteral();
            }

            return ParseBinaryExpression(level);
        }

        private enum PrecedenceLevel
        {                 // Associativity:
            Compound,     // left
            Assignment,   // right
            Ternary,      // right
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
        private List<(
            HashSet<TokenKind> operators,
            bool rightAssociative,
            Func<ExpressionNode, OperatorKind, ExpressionNode, ExpressionNode> ctor
        )> operatorGroups = new ()
        {
            // Compound expression
            (new() { TokenKind.CommaToken },
            false,
            (l, op, r) => new CompoundExpressionNode { Left = l, Right = r }),

            // Assignment
            (new() {
                TokenKind.EqualsToken, TokenKind.PlusEqualsToken, TokenKind.MinusEqualsToken,
                TokenKind.AsteriskEqualsToken, TokenKind.SlashEqualsToken, TokenKind.PercentEqualsToken,
                TokenKind.LessThanLessThanEqualsToken, TokenKind.GreaterThanGreaterThanEqualsToken,
                TokenKind.AmpersandEqualsToken, TokenKind.CaretEqualsToken, TokenKind.BarEqualsToken },
            true,
            (l, op, r) => new AssignmentExpressionNode { Left = l, Operator = op, Right = r }),

            // Ternary
            (new() { TokenKind.QuestionToken },
            true,
            (l, op, r) => throw new Exception("This should never happen. Please file a bug report.")),

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

        private ExpressionNode ParseBinaryExpression(int level = 0)
        {
            if (level >= operatorGroups.Count)
            {
                return ParsePrefixOrPostFixExpression();
            }

            ExpressionNode higher = ParseBinaryExpression(level + 1);

            // Ternary is a special case
            if (level == (int)PrecedenceLevel.Ternary)
            {
                if (Match(TokenKind.QuestionToken))
                {
                    Eat(TokenKind.QuestionToken);
                    var left = ParseExpression();
                    Eat(TokenKind.ColonToken);
                    var right = ParseExpression();
                    return new TernaryExpressionNode { Condition = higher, TrueCase = left, FalseCase = right };
                }
            }

            var group = operatorGroups[level];
            while (Match(tok => group.operators.Contains(tok.Kind)))
            {
                HLSLToken next = Advance();
                if (!HLSLSyntaxFacts.TryConvertToOperator(next.Kind, out OperatorKind op))
                {
                    Error("a valid operator", next);
                }

                higher = group.ctor(
                    higher,
                    op,
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
                    TokenKind opKind = Eat(HLSLSyntaxFacts.IsPrefixUnaryToken).Kind;
                    HLSLSyntaxFacts.TryConvertToOperator(opKind, out var op);
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
                                HLSLSyntaxFacts.TryConvertToOperator(Advance().Kind, out var incrOp);
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

        private NamedExpressionNode ParseNamedExpression()
        {
            string identifier = ParseIdentifier();
            
            var name = new IdentifierExpressionNode { Name = identifier };

            if (Match(TokenKind.ColonColonToken))
            {
                Eat(TokenKind.ColonColonToken);

                return new QualifiedIdentifierExpressionNode { Left = name, Right = ParseNamedExpression() };
            }
            else
            {
                return name;
            }
        }

        private ArrayInitializerExpressionNode ParseArrayInitializer()
        {
            Eat(TokenKind.OpenBraceToken);
            var exprs = ParseSeparatedList0(
                TokenKind.CloseBraceToken,
                TokenKind.CommaToken,
                () => ParseExpression((int)PrecedenceLevel.Compound + 1),
                true);
            Eat(TokenKind.CloseBraceToken);
            return new ArrayInitializerExpressionNode { Elements = exprs };
        }

        private LiteralExpressionNode ParseLiteralExpression()
        {
            HLSLToken next = Advance();
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

            else if (Match(TokenKind.CompileKeyword))
            {
                return ParseCompileExpression();
            }

            else if (Match(TokenKind.OpenBraceToken))
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

        private StructDefinitionNode ParseStructDefinition(List<AttributeNode> attributes)
        {
            bool isClass = Eat(TokenKind.StructKeyword, TokenKind.ClassKeyword).Kind == TokenKind.ClassKeyword;
            var name = ParseOptional(TokenKind.IdentifierToken, ParseUserDefinedTypeName);

            // base list
            List<UserDefinedTypeNode> baseList = new();
            if (Match(TokenKind.ColonToken))
            {
                Eat(TokenKind.ColonToken);

                baseList = ParseSeparatedList1(TokenKind.CommaToken, ParseUserDefinedTypeName);
            }

            Eat(TokenKind.OpenBraceToken);

            List<VariableDeclarationStatementNode> decls = new();
            List<FunctionNode> methods = new();
            while (!IsAtEnd() && !Match(TokenKind.CloseBraceToken))
            {
                if (IsNextPossiblyFunctionDeclaration())
                {
                    methods.Add(ParseFunction());
                }
                else
                {
                    decls.Add(ParseVariableDeclarationStatement(new()));
                }
            }

            Eat(TokenKind.CloseBraceToken);
            Eat(TokenKind.SemiToken);

            return new StructDefinitionNode
            {
                Attributes = attributes,
                Name = name,
                Inherits = baseList,
                Fields = decls,
                Methods = methods,
                IsClass = isClass,
            };
        }

        private InterfaceDefinitionNode ParseInterfaceDefinition(List<AttributeNode> attributes)
        {
            Eat(TokenKind.InterfaceKeyword);
            var name = ParseUserDefinedTypeName();

            Eat(TokenKind.OpenBraceToken);

            List<FunctionNode> funs = ParseMany0(
                () => !Match(TokenKind.CloseBraceToken),
                ParseFunction);

            List<FunctionDeclarationNode> decls = new();
            foreach (var function in funs)
            {
                if (function is FunctionDeclarationNode decl)
                {
                    decls.Add(decl);
                }
                else
                {
                    Error("Expected only function declarations/prototypes in interface type, but found a function body.");
                }
            }

            Eat(TokenKind.CloseBraceToken);
            Eat(TokenKind.SemiToken);

            return new InterfaceDefinitionNode
            {
                Attributes = attributes,
                Name = name,
                Functions = decls,
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

        private TypedefNode ParseTypedef(List<AttributeNode> attributes)
        {
            Eat(TokenKind.TypedefKeyword);

            bool isConst = false;
            if (Match(TokenKind.ConstKeyword))
            {
                Eat(TokenKind.ConstKeyword);
                isConst = true;
            }

            var type = ParseType();

            var names = ParseSeparatedList1(TokenKind.CommaToken, ParseUserDefinedTypeName);

            Eat(TokenKind.SemiToken);

            return new TypedefNode
            {
                Attributes = attributes,
                FromType = type,
                ToNames = names,
                IsConst = isConst,
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

            return ParseNumericType(allowVoid);
        }

        private NumericTypeNode ParseNumericType(bool allowVoid = false/* List<Modifier> Modifiers */)
        {
            HLSLToken typeToken = Advance();
            if (HLSLSyntaxFacts.TryConvertToScalarType(typeToken.Kind, out ScalarType scalarType))
            {
                if (scalarType == ScalarType.Void && !allowVoid)
                    Error("a type that isn't 'void'", typeToken);
                return new ScalarTypeNode { Kind = scalarType };
            }

            if (HLSLSyntaxFacts.TryConvertToMonomorphicVectorType(typeToken.Kind, out ScalarType vectorType, out int dimension))
            {
                if (typeToken.Kind == TokenKind.VectorKeyword && Match(TokenKind.LessThanToken))
                {
                    Eat(TokenKind.LessThanToken);
                    var genVectorType = ParseNumericType().Kind;
                    Eat(TokenKind.CommaToken);
                    int genDim = ParseIntegerLiteral();
                    Eat(TokenKind.GreaterThanToken);
                    return new VectorTypeNode { Kind = genVectorType, Dimension = genDim };
                }

                return new VectorTypeNode { Kind = vectorType, Dimension = dimension };
            }

            if (HLSLSyntaxFacts.TryConvertToMonomorphicMatrixType(typeToken.Kind, out ScalarType matrixType, out int dimX, out int dimY))
            {
                if (typeToken.Kind == TokenKind.MatrixKeyword && Match(TokenKind.LessThanToken))
                {
                    Eat(TokenKind.LessThanToken);
                    var genMatrixType = ParseNumericType().Kind;
                    Eat(TokenKind.CommaToken);
                    int genDimX = ParseIntegerLiteral();
                    Eat(TokenKind.CommaToken);
                    int genDimY = ParseIntegerLiteral();
                    Eat(TokenKind.GreaterThanToken);
                    return new MatrixTypeNode { Kind = genMatrixType, FirstDimension = genDimX, SecondDimension = genDimY };
                }

                return new MatrixTypeNode { Kind = matrixType, FirstDimension = dimX, SecondDimension = dimY };
            }

            if (typeToken.Kind == TokenKind.UnsignedKeyword)
            {
                var type = ParseNumericType();
                type.Kind = HLSLSyntaxFacts.MakeUnsigned(type.Kind);
                return type;
            }

            throw new NotImplementedException(typeToken.Span.ToString() + ": " + typeToken.ToString()); // TODO
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
            if ((HLSLSyntaxFacts.IsBuiltinType(nextKind) || nextKind == TokenKind.IdentifierToken))
            {
                return Try(() =>
                {
                    ParseType();
                    return Match(TokenKind.IdentifierToken);
                });
            }
            return false;
        }

        internal StatementNode ParseStatement()
        {
            List<AttributeNode> attributes = ParseMany0(TokenKind.OpenBracketToken, ParseAttribute);

            HLSLToken next = Peek();
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

                case TokenKind.WhileKeyword:
                    return ParseWhileStatement(attributes);

                case TokenKind.DoKeyword:
                    return ParseDoWhileStatement(attributes);

                case TokenKind.IfKeyword:
                    return ParseIfStatement(attributes);

                case TokenKind.SwitchKeyword:
                    return ParseSwitchStatement(attributes);

                case TokenKind.TypedefKeyword:
                    return ParseTypedef(attributes);

                case TokenKind.BreakKeyword:
                    Advance();
                    return new BreakStatementNode { Attributes = attributes };

                case TokenKind.ContinueKeyword:
                    Advance();
                    return new ContinueStatementNode { Attributes = attributes };

                case TokenKind.DiscardKeyword:
                    Advance();
                    return new DiscardStatementNode { Attributes = attributes };

                case TokenKind.InterfaceKeyword:
                    return ParseInterfaceDefinition(attributes);

                case TokenKind.StructKeyword:
                case TokenKind.ClassKeyword:
                    return ParseStructDefinition(attributes);

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

        private WhileStatementNode ParseWhileStatement(List<AttributeNode> attributes)
        {
            Eat(TokenKind.WhileKeyword);
            Eat(TokenKind.OpenParenToken);

            var cond = ParseExpression();

            Eat(TokenKind.CloseParenToken);

            var body = ParseStatement();

            return new WhileStatementNode
            {
                Attributes = attributes,
                Condition = cond,
                Body = body,
            };
        }

        private DoWhileStatementNode ParseDoWhileStatement(List<AttributeNode> attributes)
        {
            Eat(TokenKind.DoKeyword);
            var body = ParseStatement();

            Eat(TokenKind.WhileKeyword);
            Eat(TokenKind.OpenParenToken);

            var cond = ParseExpression();

            Eat(TokenKind.CloseParenToken);

            return new DoWhileStatementNode
            {
                Attributes = attributes,
                Body = body,
                Condition = cond,
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

        private SwitchStatementNode ParseSwitchStatement(List<AttributeNode> attributes)
        {
            Eat(TokenKind.SwitchKeyword);
            Eat(TokenKind.OpenParenToken);
            var expr = ParseExpression();
            Eat(TokenKind.CloseParenToken);
            Eat(TokenKind.OpenBraceToken);

            List<SwitchClauseNode> switchClauses = new();
            while (Match(TokenKind.CaseKeyword, TokenKind.DefaultKeyword))
            {
                List<SwitchLabelNode> switchLabels = new();
                while (Match(TokenKind.CaseKeyword, TokenKind.DefaultKeyword))
                {
                    if (Match(TokenKind.CaseKeyword))
                    {
                        Eat(TokenKind.CaseKeyword);
                        var caseExpr = ParseExpression();
                        switchLabels.Add(new SwitchCaseLabelNode { Value = caseExpr });
                    }
                    else
                    {
                        Eat(TokenKind.DefaultKeyword);
                        switchLabels.Add(new SwitchDefaultLabelNode { });
                    }
                    Eat(TokenKind.ColonToken);
                }

                List<StatementNode> statements = ParseMany0(
                    () => !Match(TokenKind.CloseBraceToken, TokenKind.CaseKeyword),
                    ParseStatement);
                switchClauses.Add(new SwitchClauseNode { Labels = switchLabels, Statements = statements });
            }

            Eat(TokenKind.CloseBraceToken);

            return new SwitchStatementNode
            {
                Attributes = attributes,
                Expression = expr,
                Clauses = switchClauses,
            };
        }

        private TechniqueNode ParseTechnique()
        {
            int version =
                Eat(TokenKind.TechniqueKeyword, TokenKind.Technique10Keyword, TokenKind.Technique11Keyword).Kind
                    == TokenKind.Technique10Keyword ? 10 : 11;

            var name = ParseUserDefinedTypeName();

            List<VariableDeclarationStatementNode> annotations = new();
            if (Match(TokenKind.LessThanToken))
            {
                Eat(TokenKind.LessThanToken);
                annotations = ParseMany0(() => !Match(TokenKind.GreaterThanToken), () => ParseVariableDeclarationStatement(new()));
                Eat(TokenKind.GreaterThanToken);
            }

            Eat(TokenKind.OpenBraceToken);
            var passes = ParseMany0(TokenKind.PassKeyword, ParsePass);
            Eat(TokenKind.CloseBraceToken);

            if (Match(TokenKind.SemiToken))
            {
                Eat(TokenKind.SemiToken);
            }

            return new TechniqueNode
            {
                Name = name,
                Annotations = annotations,
                Version = version,
                Passes = passes
            };
        }

        private PassNode ParsePass()
        {
            Eat(TokenKind.PassKeyword);
            var name = ParseUserDefinedTypeName();

            List<VariableDeclarationStatementNode> annotations = new();
            if (Match(TokenKind.LessThanToken))
            {
                Eat(TokenKind.LessThanToken);
                annotations = ParseMany0(() => !Match(TokenKind.GreaterThanToken), () => ParseVariableDeclarationStatement(new()));
                Eat(TokenKind.GreaterThanToken);
            }

            Eat(TokenKind.OpenBraceToken);
            List<StatementNode> statements = ParseMany0(() => !Match(TokenKind.CloseBraceToken), ParseStatement);
            Eat(TokenKind.CloseBraceToken);

            return new PassNode
            {
                Name = name,
                Annotations = annotations,
                Statements = statements
            };
        }
    }
}

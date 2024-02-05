using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.PreProcessor
{
    using HLSLToken = Token<TokenKind>;

    public class PreProcessor : BaseParser<TokenKind>
    {
        protected override TokenKind StringLiteralTokenKind => TokenKind.StringLiteralToken;
        protected override TokenKind IntegerLiteralTokenKind => TokenKind.IntegerLiteralToken;
        protected override TokenKind FloatLiteralTokenKind => TokenKind.FloatLiteralToken;
        protected override TokenKind IdentifierTokenKind => TokenKind.IdentifierToken;

        protected struct Macro
        {
            public bool FunctionLike;
            public string Name;
            public List<string> Parameters;
            public List<HLSLToken> Tokens;
        }

        protected string basePath;
        protected IPreProcessorIncludeResolver includeResolver;

        protected int lineOffset = 0;
        protected Dictionary<string, Macro> defines = new();

        public List<HLSLToken> outputTokens = new(); // TODO
        public List<string> outputPragmas = new();

        protected void Add(HLSLToken token)
        {
            token.Span.Start.Line += lineOffset;
            token.Span.End.Line += lineOffset;
            outputTokens.Add(token);
        }
        protected void Add(IEnumerable<HLSLToken> tokens)
        {
            foreach (var token in tokens)
            {
                Add(token);
            }
        }

        // TODO: Pre-defines
        public PreProcessor(List<HLSLToken> tokens, string basePath, IPreProcessorIncludeResolver includeResolver)
            : base(tokens)
        {
            this.basePath = basePath;
            this.includeResolver = includeResolver;
        }

        public PreProcessor(List<HLSLToken> tokens, string basePath)
            : base(tokens)
        {
            this.basePath = basePath;
            this.includeResolver = new DefaultPreProcessorIncludeResolver();
        }

        private void ExpandInclude()
        {
            Eat(TokenKind.IncludeDirectiveKeyword);
            var pathToken = Eat(TokenKind.SystemIncludeLiteralToken, TokenKind.StringLiteralToken);
            Eat(TokenKind.EndDirectiveToken);
            string filePath = pathToken.Identifier ?? string.Empty;
            string source = includeResolver.ReadFile(basePath, filePath);
            HLSLLexer.Lex(source, out var tokensToAdd, out var diagnosticsToAdd);
            diagnostics.AddRange(diagnosticsToAdd);
            Add(tokensToAdd);
        }

        private bool TryParseFunctionLikeMacroInvocationParameters(List<HLSLToken> tokenStream, ref int streamOffset, out List<List<HLSLToken>> parameters)
        {
            int localOffset = streamOffset + 1;

            // Setup local parser functionality (we want to parse on a secondary token stream)
            bool LocalIsAtEnd() => localOffset >= tokenStream.Count;
            HLSLToken LocalAdvance() => LocalIsAtEnd() ? default : tokenStream[localOffset++];
            HLSLToken LocalPeek() => LocalIsAtEnd() ? default : tokenStream[localOffset];
            bool LocalMatch(TokenKind kind) => LocalIsAtEnd() ? false : kind == tokenStream[localOffset].Kind;
            HLSLToken LocalEat(TokenKind kind)
            {
                if (!LocalMatch(kind))
                    Error($"Expected token type '{kind}', got '{LocalPeek().Kind}'.");
                return LocalAdvance();
            }

            parameters = new List<List<HLSLToken>>();

            // Eat arguments if they are available
            if (LocalMatch(TokenKind.OpenParenToken))
            {
                // Always eat open paren
                LocalEat(TokenKind.OpenParenToken);

                // Check for special case of 0 args
                if (LocalMatch(TokenKind.CloseParenToken))
                {
                    LocalEat(TokenKind.CloseParenToken);
                    streamOffset = localOffset - 1;
                    return true;
                }

                parameters.Add(new());

                // Parse until we have match parens, they might be nested
                int numParens = 1;
                while (numParens > 0)
                {
                    var next = LocalAdvance();
                    switch (next.Kind)
                    {
                        case TokenKind.OpenParenToken:
                            numParens++;
                            if (numParens > 1) parameters.Last().Add(next);
                            break;
                        case TokenKind.CloseParenToken:
                            if (numParens > 1) parameters.Last().Add(next);
                            numParens--;
                            break;
                        case TokenKind.CommaToken when numParens == 1:
                            parameters.Add(new());
                            break;
                        default:
                            parameters.Last().Add(next);
                            break;
                    }
                }
                streamOffset = localOffset - 1;
                return true;
            }
            // If no args, it must be a regular identifier
            return false;
        }

        private List<HLSLToken> ApplyMacros()
        {
            // First, get the entire macro identifier
            List<HLSLToken> expanded = new();
            var identifierTok = Eat(TokenKind.IdentifierToken);
            expanded.Add(identifierTok);
            string identifier = identifierTok.Identifier ?? string.Empty;

            // Check if it is a functionlike macro
            bool isFunctionLike = defines.ContainsKey(identifier) && defines[identifier].FunctionLike;
            if (isFunctionLike)
            {
                // If so, eat arguments if they are available
                if (Match(TokenKind.OpenParenToken))
                {
                    expanded.Add(Eat(TokenKind.OpenParenToken));
                    int numParens = 1;
                    while (numParens > 0) // Might have nested parens
                    {
                        var next = Advance();
                        if (next.Kind == TokenKind.OpenParenToken)
                            numParens++;
                        else if (next.Kind == TokenKind.CloseParenToken)
                            numParens--;
                        expanded.Add(next);
                    }
                }
                // Otherwise, it must be a regular identifier
                else
                {
                    return expanded;
                }
            }
            

            HashSet<string> hideSet = new();
            
            // Loop until we can't apply macros anymore
            while (true)
            {
                // Take note of hideset count so we know if anything was applied
                int hideSetSize = hideSet.Count;

                // Try to apply each macro
                foreach (var macro in defines)
                {
                    // ... unless it was already applied
                    if (hideSet.Contains(macro.Key))
                        continue;

                    List<HLSLToken> next = new();

                    for (int i = 0; i < expanded.Count; i++)
                    {
                        HLSLToken token = expanded[i];

                        string lexeme = HLSLSyntaxFacts.IdentifierOrKeywordToString(token);
                        // If the macro matches
                        if (macro.Key == lexeme)
                        {
                            // Add it to the hideset
                            if (!hideSet.Contains(macro.Key))
                            {
                                hideSet.Add(macro.Key);
                            }

                            // We need to replace tokens.
                            // First, check if we have a functionlike macro
                            if (macro.Value.FunctionLike)
                            {
                                // Try to parase parameters. If they aren't there, it's just an identifier.
                                if (!TryParseFunctionLikeMacroInvocationParameters(expanded, ref i, out var parameters))
                                    next.Add(token);

                                if (parameters.Count != macro.Value.Parameters.Count)
                                    Error($"Incorrect number of arguments passed to macro '{macro.Value.Name}', expected {macro.Value.Parameters.Count}, got {parameters.Count}.");

                                // If they are there, substitute them
                                foreach (var macroToken in macro.Value.Tokens)
                                {
                                    string macroTokenLexeme = HLSLSyntaxFacts.IdentifierOrKeywordToString(macroToken);
                                    int paramIndex = macro.Value.Parameters.IndexOf(macroTokenLexeme);
                                    if (paramIndex >= 0 && paramIndex < parameters.Count)
                                    {
                                        var parameter = parameters[paramIndex];
                                        next.AddRange(parameter);
                                    }
                                    else
                                    {
                                        next.Add(macroToken);
                                    }
                                }
                            }
                            // If not, we can just substitute tokens directly
                            else
                            {
                                next.AddRange(macro.Value.Tokens);
                            }
                        }
                        // Otherwise just pass the token through
                        else
                        {
                            next.Add(token);
                        }
                    }

                    expanded = next;
                }

                // If nothing was applied, stop
                if (hideSet.Count == hideSetSize)
                {
                    break;
                }
            }

            return expanded;
        }

        private List<HLSLToken> SkipUntilEndOfConditional()
        {
            List<HLSLToken> skipped = new();
            while (true)
            {
                if (IsAtEnd())
                {
                    Error("Unterminated conditional directive.");
                    break;
                }

                switch (Peek().Kind)
                {
                    case TokenKind.ElifDirectiveKeyword:
                    case TokenKind.ElseDirectiveKeyword:
                    case TokenKind.EndifDirectiveKeyword:
                        return skipped;

                    default:
                        skipped.Add(Advance());
                        break;
                }
            }
            return skipped;
        }

        private bool EvaluateConstExpr(List<HLSLToken> exprTokens)
        {
            bool result = ConstExpressionEvaluator.EvaluateConstExprTokens(exprTokens, out var evalDiags);
            if (evalDiags.Count > 0)
            {
                foreach (string diag in evalDiags)
                {
                    Error(diag);
                }
                return false;
            }
            return result;
        }

        private bool EvaluateCondition(bool continued)
        {
            HLSLToken conditional = Advance();
            switch (conditional.Kind)
            {
                case TokenKind.IfdefDirectiveKeyword:
                    string ifdefName = ParseIdentifier();
                    Eat(TokenKind.EndDirectiveToken);
                    return defines.ContainsKey(ifdefName);

                case TokenKind.IfndefDirectiveKeyword:
                    string ifndefName = ParseIdentifier();
                    Eat(TokenKind.EndDirectiveToken);
                    return !defines.ContainsKey(ifndefName);

                case TokenKind.ElseDirectiveKeyword:
                    Eat(TokenKind.EndDirectiveToken);
                    if (!continued)
                    {
                        Error("Unexpected #else directive - there is no conditional directive preceding it.");
                    }
                    return true;

                case TokenKind.IfDirectiveKeyword:
                case TokenKind.ElifDirectiveKeyword:
                    if (!continued && conditional.Kind == TokenKind.ElifDirectiveKeyword)
                    {
                        Error("Unexpected #elif directive - there is no conditional directive preceding it.");
                    }
                    // Get the expanded tokens for the condition expression
                    List<HLSLToken> expandedConditionTokens = new();
                    while (!IsAtEnd() && !Match(TokenKind.EndDirectiveToken))
                    {
                        // If we find an identifier, eagerly expand (https://www.math.utah.edu/docs/info/cpp_1.html)
                        if (Match(TokenKind.IdentifierToken))
                        {
                            expandedConditionTokens.AddRange(ApplyMacros());
                        }
                        else
                        {
                            expandedConditionTokens.Add(Advance());
                        }
                    }
                    // The C spec says we should replace any identifiers remaining after expansion with the literal 0
                    for (int i = 0; i < expandedConditionTokens.Count; i++)
                    {
                        var token = expandedConditionTokens[i];
                        if (token.Kind == TokenKind.IdentifierToken)
                        {
                            token.Kind = TokenKind.IntegerLiteralToken;
                            token.Identifier = "0";
                            expandedConditionTokens[i] = token;
                        }
                    }
                    Eat(TokenKind.EndDirectiveToken);
                    // Finally evaluate the expression
                    return EvaluateConstExpr(expandedConditionTokens);
                default:
                    Error($"Unexpected token '{conditional.Kind}', expected preprocessor directive.");
                    return false;
            }
        }

        private void ExpandConditional()
        {
            List<HLSLToken> takenTokens = new();
            bool branchTaken = false;

            bool condEvaluation = EvaluateCondition(false);

            while (true)
            {
                if (IsAtEnd())
                {
                    Error("Unterminated conditional directive.");
                    break;
                }

                // Eat the body
                var skipped = SkipUntilEndOfConditional();

                // If we haven't already taken a branch, and this one is true, take it
                if (!branchTaken && condEvaluation)
                {
                    branchTaken = true;
                    takenTokens = skipped;
                }

                // If we have reached the end, stop
                if (Match(TokenKind.EndifDirectiveKeyword))
                {
                    Eat(TokenKind.EndifDirectiveKeyword);
                    Eat(TokenKind.EndDirectiveToken);
                    break;
                }

                condEvaluation = EvaluateCondition(true);
            }

            Add(takenTokens);
        }

        public void ExpandMacros()
        {
            while (!IsAtEnd())
            {
                HLSLToken next = Peek();
                switch (next.Kind)
                {
                    case TokenKind.IncludeDirectiveKeyword:
                        ExpandInclude();
                        break;

                    case TokenKind.LineDirectiveKeyword:
                        int tokenLine = next.Span.Start.Line; // where we actually are
                        Eat(TokenKind.LineDirectiveKeyword);
                        int targetLine = ParseIntegerLiteral(); // where we want to be
                        lineOffset = targetLine - tokenLine; // calculate the offset
                        if (Match(TokenKind.StringLiteralToken))
                        {
                            Advance();
                        }
                        Eat(TokenKind.EndDirectiveToken);
                        break;

                    case TokenKind.DefineDirectiveKeyword:
                        Eat(TokenKind.DefineDirectiveKeyword);
                        string from = ParseIdentifier();
                        List<string> args = new();
                        bool functionLike = false;
                        if (Match(TokenKind.OpenParenToken))
                        {
                            functionLike = true;
                            Eat(TokenKind.OpenParenToken);
                            args = ParseSeparatedList0(TokenKind.CloseParenToken, TokenKind.CommaToken, ParseIdentifier);
                            Eat(TokenKind.CloseParenToken);
                        }
                        List<HLSLToken> toks = ParseMany0(() => !Match(TokenKind.EndDirectiveToken), () => Advance());
                        Eat(TokenKind.EndDirectiveToken);
                        defines[from] = new Macro
                        {
                            Name = from,
                            FunctionLike = functionLike,
                            Parameters = args,
                            Tokens = toks
                        };
                        break;

                    case TokenKind.UndefDirectiveKeyword:
                        Eat(TokenKind.UndefDirectiveKeyword);
                        string undef = ParseIdentifier();
                        Eat(TokenKind.EndDirectiveToken);
                        defines.Remove(undef);
                        break;

                    case TokenKind.ErrorDirectiveKeyword:
                        Eat(TokenKind.ErrorDirectiveKeyword);
                        var errorToks = ParseMany0(() => !Match(TokenKind.EndDirectiveToken), () => Advance())
                            .Select(x => HLSLSyntaxFacts.TokenToString(x));
                        Eat(TokenKind.EndDirectiveToken);
                        string error = string.Join(" ", errorToks);
                        diagnostics.Add(error);
                        break;

                    case TokenKind.PragmaDirectiveKeyword:
                        Eat(TokenKind.PragmaDirectiveKeyword);
                        var pragmaToks = ParseMany0(() => !Match(TokenKind.EndDirectiveToken), () => Advance())
                            .Select(x => HLSLSyntaxFacts.TokenToString(x));
                        Eat(TokenKind.EndDirectiveToken);
                        string pragma = string.Join(" ", pragmaToks);
                        outputPragmas.Add(pragma);
                        break;

                    case TokenKind.IfdefDirectiveKeyword:
                    case TokenKind.IfndefDirectiveKeyword:
                    case TokenKind.IfDirectiveKeyword:
                    case TokenKind.ElifDirectiveKeyword:
                    case TokenKind.ElseDirectiveKeyword:
                    case TokenKind.EndifDirectiveKeyword:
                        ExpandConditional();
                        break;

                    // TODO: Glue identifiers, glue adjacent string literals
                    case TokenKind.HashHashToken:

                    case TokenKind.IdentifierToken:
                        Add(ApplyMacros());
                        break;

                    default:
                        Add(Advance());
                        break;
                }
            }
        }

        public void ExpandIncludesOnly()
        {
            while (!IsAtEnd())
            {
                HLSLToken next = Peek();
                if (next.Kind == TokenKind.IncludeDirectiveKeyword)
                {
                    ExpandInclude();
                }
                else
                {
                    Add(Advance());
                }
            }
        }
    }
}

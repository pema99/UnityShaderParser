using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.PreProcessor
{
    using HLSLToken = Token<TokenKind>;

    public enum PreProcessorMode
    {
        ExpandAll,
        ExpandIncludesOnly,
        ExpandAllExceptIncludes,
        StripDirectives,
        DoNothing,
        // TODO: Option to embed directives into tokens
    }

    internal struct Macro
    {
        public bool FunctionLike;
        public string Name;
        public List<string> Parameters;
        public List<HLSLToken> Tokens;
    }

    public class HLSLPreProcessor : BaseParser<TokenKind>
    {
        protected override TokenKind StringLiteralTokenKind => TokenKind.StringLiteralToken;
        protected override TokenKind IntegerLiteralTokenKind => TokenKind.IntegerLiteralToken;
        protected override TokenKind FloatLiteralTokenKind => TokenKind.FloatLiteralToken;
        protected override TokenKind IdentifierTokenKind => TokenKind.IdentifierToken;
        protected override ParserStage Stage => ParserStage.HLSLPreProcessing;

        protected string basePath;
        protected IPreProcessorIncludeResolver includeResolver;

        protected int lineOffset = 0;
        internal Dictionary<string, Macro> defines = new Dictionary<string, Macro>();

        protected List<HLSLToken> outputTokens = new List<HLSLToken>();
        protected List<string> outputPragmas = new List<string>();

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

        public HLSLPreProcessor(List<HLSLToken> tokens, bool throwExceptionOnError, string basePath, IPreProcessorIncludeResolver includeResolver, Dictionary<string, string> defines)
            : base(tokens, throwExceptionOnError)
        {
            this.basePath = basePath;
            this.includeResolver = includeResolver;

            foreach (var kvp in defines)
            {
                var localTokens = HLSLLexer.Lex(kvp.Value, false, out var localLexerDiags);
                if (localLexerDiags.Count > 0)
                {
                    Error($"Invalid define '{kvp.Key}' passed.");
                }
                this.defines.Add(kvp.Key, new Macro
                {
                    FunctionLike = false,
                    Name = kvp.Key,
                    Parameters = new List<string>(),
                    Tokens = localTokens
                });
            }
        }

        public static List<HLSLToken> PreProcess(
            List<HLSLToken> tokens,
            bool throwExceptionOnError,
            PreProcessorMode mode,
            string basePath,
            IPreProcessorIncludeResolver includeResolver,
            Dictionary<string, string> defines,
            out List<string> pragmas,
            out List<Diagnostic> diagnostics)
        {
            HLSLPreProcessor preProcessor = new HLSLPreProcessor(tokens, throwExceptionOnError, basePath, includeResolver, defines);
            switch (mode)
            {
                case PreProcessorMode.ExpandAll:
                    preProcessor.ExpandDirectives(true);
                    break;
                case PreProcessorMode.ExpandIncludesOnly:
                    preProcessor.ExpandIncludesOnly();
                    break;
                case PreProcessorMode.ExpandAllExceptIncludes:
                    preProcessor.ExpandDirectives(false);
                    break;
                case PreProcessorMode.StripDirectives:
                    preProcessor.StripDirectives();
                    break;
                case PreProcessorMode.DoNothing:
                    preProcessor.outputTokens = tokens;
                    break;
            }
            pragmas = preProcessor.outputPragmas;
            diagnostics = preProcessor.diagnostics;
            return preProcessor.outputTokens;
        }

        private new string ParseIdentifier()
        {
            if (Match(TokenKind.IdentifierToken))
            {
                return base.ParseIdentifier();
            }
            else
            {
                var identifierToken = Advance();
                if (HLSLSyntaxFacts.TryConvertKeywordToString(identifierToken.Kind, out string result))
                {
                    return result;
                }
                Error("a valid identifier", identifierToken);
                return string.Empty;
            }
        }

        private void ExpandInclude()
        {
            Eat(TokenKind.IncludeDirectiveKeyword);
            var pathToken = Eat(TokenKind.SystemIncludeLiteralToken, TokenKind.StringLiteralToken);
            Eat(TokenKind.EndDirectiveToken);
            string filePath = pathToken.Identifier ?? string.Empty;
            string source = includeResolver.ReadFile(basePath, filePath);
            var tokensToAdd = HLSLLexer.Lex(source, throwExceptionOnError, out var diagnosticsToAdd);
            diagnostics.AddRange(diagnosticsToAdd);
            tokens.InsertRange(position, tokensToAdd);
        }

        // Glues tokens together with ## and evaluates defined(x) between each expansion
        private void ReplaceBetweenExpansions(List<HLSLToken> tokens)
        {
            HLSLToken LocalPeek(int i) => i < tokens.Count ? tokens[i] : default;

            List<HLSLToken> result = new List<HLSLToken>();
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (HLSLSyntaxFacts.TryConvertIdentifierOrKeywordToString(token, out string gluedIdentifier) && LocalPeek(i + 1).Kind == TokenKind.HashHashToken)
                {
                    SourceSpan startSpan = token.Span;
                    SourceSpan endSpan = token.Span;

                    i++; // identifier
                    while (LocalPeek(i).Kind == TokenKind.HashHashToken &&
                        HLSLSyntaxFacts.TryConvertIdentifierOrKeywordToString(LocalPeek(i + 1), out string nextIdentifier))
                    {
                        i++; // ##
                        var nextToken = LocalPeek(i++); // identifier
                        gluedIdentifier += nextIdentifier;
                        endSpan = nextToken.Span;
                    }

                    var gluedToken = new HLSLToken
                    {
                        Identifier = gluedIdentifier,
                        Kind = TokenKind.IdentifierToken,
                        Span = new SourceSpan { Start = startSpan.Start, End = endSpan.End },
                    };
                    i--; // For loop continues

                    result.Add(gluedToken);
                }
                else if (token.Kind == TokenKind.IdentifierToken && token.Identifier == "defined")
                {
                    SourceSpan startSpan = token.Span;

                    i++; // defined
                    bool hasParen = LocalPeek(i).Kind == TokenKind.OpenParenToken;
                    if (hasParen) i++;
                    HLSLToken identifier = LocalPeek(i++);
                    SourceSpan endSpan = identifier.Span;
                    if (hasParen) endSpan = LocalPeek(i++).Span;

                    var replacedToken = new HLSLToken
                    {
                        Identifier = defines.ContainsKey(HLSLSyntaxFacts.IdentifierOrKeywordToString(identifier)) ? "1" : "0",
                        Kind = TokenKind.IntegerLiteralToken,
                        Span = new SourceSpan { Start = startSpan.Start, End = endSpan.End },
                    };
                    i--; // For loop continues

                    result.Add(replacedToken);
                }
                else
                {
                    result.Add(token);
                }
            }

            tokens.Clear();
            tokens.AddRange(result);
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

                parameters.Add(new List<HLSLToken>());

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
                            parameters.Add(new List<HLSLToken>());
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
            List<HLSLToken> expanded = new List<HLSLToken>();
            var identifierTok = Eat(TokenKind.IdentifierToken);
            expanded.Add(identifierTok);
            string identifier = identifierTok.Identifier ?? string.Empty;

            // Check if it is a functionlike macro
            bool isFunctionLike = (defines.ContainsKey(identifier) && defines[identifier].FunctionLike) || identifier == "defined";
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

            // Optimization: Only do this if necessary
            bool hasGlueTokenOrDefined = expanded.Any(x => x.Kind == TokenKind.HashHashToken || x.Identifier == "defined");
            if (hasGlueTokenOrDefined)
            {
                ReplaceBetweenExpansions(expanded);
            }
            
            HashSet<string> hideSet = new HashSet<string>();
            
            // Loop until we can't apply macros anymore
            while (true)
            {
                List<HLSLToken> next = new List<HLSLToken>();
                HashSet<string> nextHideSet = new HashSet<string>();

                // Go over each token and try to apply, adding to the hideset as we go
                bool anyThingApplied = false;
                for (int i = 0; i < expanded.Count; i++)
                {
                    HLSLToken token = expanded[i];

                    string lexeme = HLSLSyntaxFacts.IdentifierOrKeywordToString(token);
                    // If the macro matches
                    if (!hideSet.Contains(lexeme) && defines.TryGetValue(lexeme, out Macro macro))
                    {
                        // Add it to the hideset
                        if (!nextHideSet.Contains(lexeme))
                        {
                            nextHideSet.Add(lexeme);
                        }

                        anyThingApplied = true;

                        // We need to replace tokens.
                        // First, check if we have a functionlike macro
                        if (macro.FunctionLike)
                        {
                            // Try to parase parameters. If they aren't there, it's just an identifier.
                            if (!TryParseFunctionLikeMacroInvocationParameters(expanded, ref i, out var parameters))
                                next.Add(token);

                            if (parameters.Count != macro.Parameters.Count)
                                Error($"Incorrect number of arguments passed to macro '{macro.Name}', expected {macro.Parameters.Count}, got {parameters.Count}.");

                            // If they are there, substitute them
                            foreach (var macroToken in macro.Tokens)
                            {
                                string macroTokenLexeme = HLSLSyntaxFacts.IdentifierOrKeywordToString(macroToken);
                                int paramIndex = macro.Parameters.IndexOf(macroTokenLexeme);
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
                            next.AddRange(macro.Tokens);
                        }
                    }
                    // Otherwise just pass the token through
                    else
                    {
                        next.Add(token);
                    }
                }

                // Optimization: Check if anything changed - costly to replace
                if (anyThingApplied)
                {
                    ReplaceBetweenExpansions(next);
                }

                hideSet = nextHideSet;
                expanded = next;

                // If nothing was applied, stop
                if (!anyThingApplied)
                {
                    break;
                }
            }

            return expanded;
        }

        private List<HLSLToken> SkipUntilEndOfConditional()
        {
            List<HLSLToken> skipped = new List<HLSLToken>();
            int depth = 0;
            while (true)
            {
                if (IsAtEnd())
                {
                    Error("Unterminated conditional directive.");
                    break;
                }

                switch (Peek().Kind)
                {
                    case TokenKind.IfdefDirectiveKeyword:
                    case TokenKind.IfndefDirectiveKeyword:
                    case TokenKind.IfDirectiveKeyword:
                        depth++;
                        skipped.Add(Advance());
                        break;

                    case TokenKind.ElseDirectiveKeyword:
                    case TokenKind.ElifDirectiveKeyword:
                        if (depth == 0)
                        {
                            return skipped;
                        }
                        else
                        {
                            skipped.Add(Advance());
                        }
                        break;

                    case TokenKind.EndifDirectiveKeyword:
                        if (depth == 0)
                        {
                            return skipped;
                        }
                        else
                        {
                            depth--;
                            skipped.Add(Advance());
                        }
                        break;

                    default:
                        skipped.Add(Advance());
                        break;
                }
            }
            return skipped;
        }

        private bool EvaluateConstExpr(List<HLSLToken> exprTokens)
        {
            bool result = ConstExpressionEvaluator.EvaluateConstExprTokens(exprTokens, throwExceptionOnError, out var evalDiags);
            if (evalDiags.Count > 0)
            {
                foreach (var diag in evalDiags)
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
                    List<HLSLToken> expandedConditionTokens = new List<HLSLToken>();
                    while (!IsAtEnd() && !Match(TokenKind.EndDirectiveToken))
                    {
                        // If we find an identifier, eagerly expand (https://www.math.utah.edu/docs/info/cpp_1.html)
                        var next = Peek();
                        if (next.Kind == TokenKind.IdentifierToken)
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
            int startPosition = position;
            List<HLSLToken> takenTokens = new List<HLSLToken>();
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

            // Substitution. First take away the tokens we just evaluated, then insert the substitution,
            // and rewind to the start of it
            int numTokensInDirective = position - startPosition;
            position = startPosition;
            tokens.RemoveRange(position, numTokensInDirective);
            tokens.InsertRange(position, takenTokens);
        }

        private void GlueStringLiteralsPass()
        {
            position = 0;
            tokens = new List<HLSLToken>(outputTokens);
            outputTokens.Clear();
            while (!IsAtEnd())
            {
                if (Match(TokenKind.StringLiteralToken))
                {
                    var strTok = Eat(TokenKind.StringLiteralToken);
                    string glued = strTok.Identifier ?? string.Empty;
                    SourceSpan spanStart = strTok.Span;
                    SourceSpan spanEnd = strTok.Span;
                    while (Match(TokenKind.StringLiteralToken))
                    {
                        var nextStrTok = Eat(TokenKind.StringLiteralToken);
                        glued += nextStrTok.Identifier ?? string.Empty;
                        spanEnd = strTok.Span;
                    }
                    var gluedToken = new HLSLToken
                    {
                        Kind = TokenKind.StringLiteralToken,
                        Identifier = glued,
                        Span = new SourceSpan { Start = spanStart.Start, End = spanEnd.End },
                    };
                    Add(gluedToken);
                }
                else
                {
                    Add(Advance());
                }
            }
        }

        public void ExpandDirectives(bool expandIncludes = true)
        {
            while (!IsAtEnd())
            {
                HLSLToken next = Peek();
                switch (next.Kind)
                {
                    case TokenKind.IncludeDirectiveKeyword:
                        if (expandIncludes)
                        {
                            ExpandInclude();
                        }
                        else
                        {
                            // Skip the include
                            Eat(TokenKind.IncludeDirectiveKeyword);
                            var pathToken = Eat(TokenKind.SystemIncludeLiteralToken, TokenKind.StringLiteralToken);
                            Eat(TokenKind.EndDirectiveToken);
                        }
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
                        List<string> args = new List<string>();
                        bool functionLike = false;
                        if (Match(TokenKind.OpenFunctionLikeMacroParenToken))
                        {
                            functionLike = true;
                            Eat(TokenKind.OpenFunctionLikeMacroParenToken);
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
                        Error(error);
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

                    case TokenKind.IdentifierToken:
                        Add(ApplyMacros());
                        break;

                    default:
                        Add(Advance());
                        break;
                }
            }

            // C spec says we need to glue adjacent string literals
            GlueStringLiteralsPass();
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

        public void StripDirectives(bool expandIncludes = true)
        {
            while (!IsAtEnd())
            {
                HLSLToken next = Peek();
                switch (next.Kind)
                {
                    case TokenKind.IncludeDirectiveKeyword:
                    case TokenKind.LineDirectiveKeyword:
                    case TokenKind.DefineDirectiveKeyword:
                    case TokenKind.UndefDirectiveKeyword:
                    case TokenKind.ErrorDirectiveKeyword:
                    case TokenKind.PragmaDirectiveKeyword:
                    case TokenKind.IfdefDirectiveKeyword:
                    case TokenKind.IfndefDirectiveKeyword:
                    case TokenKind.IfDirectiveKeyword:
                    case TokenKind.ElifDirectiveKeyword:
                    case TokenKind.ElseDirectiveKeyword:
                    case TokenKind.EndifDirectiveKeyword:
                        while (!IsAtEnd() && !Match(TokenKind.EndDirectiveToken))
                        {
                            Advance();
                        }
                        if (Match(TokenKind.EndDirectiveToken))
                        {
                            Advance();
                        }
                        break;

                    default:
                        Add(Advance());
                        break;
                }
            }

            // C spec says we need to glue adjacent string literals
            GlueStringLiteralsPass();
        }
    }
}

﻿using UnityShaderParser.Common;

namespace UnityShaderParser.ShaderLab
{
    using Token = Token<TokenKind>;

    public class ShaderLabParser : BaseParser<TokenKind>
    {
        public ShaderLabParser(List<Token> tokens)
            : base(tokens) { }

        protected override TokenKind StringLiteralTokenKind => TokenKind.StringLiteralToken;
        protected override TokenKind BracketedStringLiteralTokenKind => TokenKind.BracketedStringLiteralToken;
        protected override TokenKind IntegerLiteralTokenKind => TokenKind.IntegerLiteralToken;
        protected override TokenKind FloatLiteralTokenKind => TokenKind.FloatLiteralToken;
        protected override TokenKind IdentifierTokenKind => TokenKind.IdentifierToken;

        public static void Parse(List<Token> tokens, out ShaderNode rootNode, out List<string> diagnostics)
        {
            ShaderLabParser parser = new(tokens);

            rootNode = parser.ParseShader();
            diagnostics = parser.diagnostics;
        }

        private ShaderNode ParseShader()
        {
            Eat(TokenKind.ShaderKeyword);
            string name = Eat(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
            Eat(TokenKind.OpenBraceToken);

            List<string> includeBlocks = new List<string>();

            ParseIncludeBlocksIfPresent(includeBlocks);

            List<ShaderPropertyNode> properties = new();
            if (Match(TokenKind.PropertiesKeyword))
            {
                ParsePropertySection(properties);
            }

            List<SubShaderNode> subshaders = new();
            string? fallback = null;
            bool fallbackDisabledExplicitly = false;
            string? customEditor = null;
            Dictionary<string, string> dependencies = new();

            // Keep track of commands inherited by categories as we parse.
            // We essentially pretend categories don't exist, since they are a niche feature.
            Stack<List<ShaderLabCommandNode>> categoryCommands = new();

            while (!IsAtEnd())
            {
                ParseIncludeBlocksIfPresent(includeBlocks);

                // If we are in a category, put the commands there
                if (categoryCommands.Count > 0)
                    ParseCommandsIfPresent(categoryCommands.Peek());

                Token next = Peek();
                if (next.Kind == TokenKind.CloseBraceToken)
                    break;

                switch (next.Kind)
                {
                    case TokenKind.SubShaderKeyword:
                        var subShader = ParseSubShader();
                        subShader.Commands.AddRange(categoryCommands.SelectMany(x => x));
                        subshaders.Add(subShader);
                        break;
                    case TokenKind.FallbackKeyword:
                        Advance();
                        if (Match(TokenKind.OffKeyword, TokenKind.FalseKeyword))
                        {
                            fallbackDisabledExplicitly = true;
                            Advance();
                        }
                        else
                        {
                            fallback = Eat(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
                        }
                        break;
                    case TokenKind.DependencyKeyword:
                        Advance();
                        string key = ParseStringLiteral();
                        Eat(TokenKind.EqualsToken);
                        string val = ParseStringLiteral();
                        dependencies[key] = val;
                        break;
                    case TokenKind.CustomEditorKeyword:
                        Advance();
                        customEditor = Eat(TokenKind.StringLiteralToken).Identifier ?? string.Empty;
                        break;
                    case TokenKind.CategoryKeyword:
                        Advance();
                        Eat(TokenKind.OpenBraceToken);
                        categoryCommands.Push(new());
                        break;
                    case TokenKind.CloseBraceToken when categoryCommands.Count > 0:
                        Advance();
                        categoryCommands.Pop();
                        break;
                    default:
                        Advance();
                        Error($"SubShader, Fallback, Dependency or CustomEditor", next);
                        break;
                }
            }

            ParseIncludeBlocksIfPresent(includeBlocks);

            Eat(TokenKind.CloseBraceToken);

            return new ShaderNode
            {
                Name = name,
                Properties = properties,
                SubShaders = subshaders,
                Fallback = fallback,
                FallbackDisabledExplicitly = fallbackDisabledExplicitly,
                CustomEditor = customEditor,
                Dependencies = dependencies,
                IncludeBlocks = includeBlocks,
            };
        }

        private void ParseIncludeBlocksIfPresent(List<string> outIncludeBlocks)
        {
            while (true)
            {
                Token next = Peek();
                if (next.Kind == TokenKind.IncludeBlock && !string.IsNullOrEmpty(next.Identifier))
                {
                    outIncludeBlocks.Add(next.Identifier);
                    Advance();
                }
                else
                {
                    break;
                }
            }
        }

        private void ParsePropertySection(List<ShaderPropertyNode> outProperties)
        {
            Eat(TokenKind.PropertiesKeyword);
            Eat(TokenKind.OpenBraceToken);

            while (Match(TokenKind.IdentifierToken, TokenKind.BracketedStringLiteralToken))
            {
                outProperties.Add(ParseProperty());
            }

            Eat(TokenKind.CloseBraceToken);
        }

        private ShaderPropertyNode ParseProperty()
        {
            List<string> attributes = new();
            while (Match(TokenKind.BracketedStringLiteralToken))
            {
                attributes.Add(ParseBracketedStringLiteral());
            }

            string uniform = ParseIdentifier();

            Eat(TokenKind.OpenParenToken);

            string name = ParseStringLiteral();
            Eat(TokenKind.CommaToken);

            ShaderPropertyKind kind = ShaderPropertyKind.None;
            (float Min, float Max)? rangeMinMax = null;
            Token typeToken = Advance();
            switch (typeToken.Kind)
            {
                case TokenKind.FloatKeyword: kind = ShaderPropertyKind.Float; break;
                case TokenKind.IntegerKeyword: kind = ShaderPropertyKind.Integer; break;
                case TokenKind.IntKeyword: kind = ShaderPropertyKind.Int; break;
                case TokenKind.ColorKeyword: kind = ShaderPropertyKind.Color; break;
                case TokenKind.VectorKeyword: kind = ShaderPropertyKind.Vector; break;
                case TokenKind._2DKeyword or TokenKind.RectKeyword: kind = ShaderPropertyKind.Texture2D; break;
                case TokenKind._3DKeyword: kind = ShaderPropertyKind.Texture3D; break;
                case TokenKind.CubeKeyword: kind = ShaderPropertyKind.TextureCube; break;
                case TokenKind._2DArrayKeyword: kind = ShaderPropertyKind.Texture2DArray; break;
                case TokenKind._3DArrayKeyword: kind = ShaderPropertyKind.Texture3DArray; break;
                case TokenKind.CubeArrayKeyword: kind = ShaderPropertyKind.TextureCubeArray; break;
                case TokenKind.AnyKeyword: kind = ShaderPropertyKind.TextureAny; break;
                case TokenKind.RangeKeyword:
                    kind = ShaderPropertyKind.Range;
                    Eat(TokenKind.OpenParenToken);
                    float min = ParseNumericLiteral();
                    Eat(TokenKind.CommaToken);
                    float max = ParseNumericLiteral();
                    Eat(TokenKind.CloseParenToken);
                    rangeMinMax = (min, max);
                    break;
                default:
                    Error("a valid type", typeToken);
                    break;
            }

            Eat(TokenKind.CloseParenToken);

            Eat(TokenKind.EqualsToken);

            ShaderPropertyValueNode valueNode = new();
            switch (kind)
            {
                case ShaderPropertyKind.Color or ShaderPropertyKind.Vector:
                    Eat(TokenKind.OpenParenToken);
                    float x = ParseNumericLiteral();
                    Eat(TokenKind.CommaToken);
                    float y = ParseNumericLiteral();
                    Eat(TokenKind.CommaToken);
                    float z = ParseNumericLiteral();
                    float w = 1;
                    bool hasLastChannel = false;
                    if (Match(TokenKind.CommaToken))
                    {
                        Eat(TokenKind.CommaToken);
                        w = ParseNumericLiteral();
                        hasLastChannel = true;
                    }
                    Eat(TokenKind.CloseParenToken);
                    if (kind == ShaderPropertyKind.Color)
                        valueNode = new ShaderPropertyValueColorNode { HasAlphaChannel = hasLastChannel, Color = (x, y, z, w) };
                    else
                        valueNode = new ShaderPropertyValueVectorNode { HasWChannel = hasLastChannel, Vector = (x, y, z, w) };
                    break;

                case ShaderPropertyKind.TextureCube or ShaderPropertyKind.Texture2D or ShaderPropertyKind.Texture3D or ShaderPropertyKind.TextureAny or
                     ShaderPropertyKind.TextureCubeArray or ShaderPropertyKind.Texture2DArray or ShaderPropertyKind.Texture3DArray:
                    valueNode = new ShaderPropertyValueTextureNode { Kind = ShaderLabSyntaxFacts.ShaderPropertyTypeToTextureType(kind), TextureName = ParseStringLiteral() };
                    break;

                case ShaderPropertyKind.Integer or ShaderPropertyKind.Int:
                    valueNode = new ShaderPropertyValueIntegerNode { Number = ParseIntegerLiteral() };
                    break;

                case ShaderPropertyKind.Float or ShaderPropertyKind.Range:
                    valueNode = new ShaderPropertyValueFloatNode { Number = ParseNumericLiteral() };
                    break;

                default:
                    break;
            }

            if (Match(TokenKind.OpenBraceToken))
            {
                Eat(TokenKind.OpenBraceToken);
                while (Peek().Kind != TokenKind.CloseBraceToken)
                    Advance();
                Eat(TokenKind.CloseBraceToken);
            }

            return new ShaderPropertyNode
            {
                Attributes = attributes,
                Uniform = uniform,
                Name = name,
                Kind = kind,
                RangeMinMax = rangeMinMax,
                Value = valueNode,
            };
        }

        private SubShaderNode ParseSubShader()
        {
            Eat(TokenKind.SubShaderKeyword);
            Eat(TokenKind.OpenBraceToken);

            List<ShaderPassNode> passes = new();
            List<ShaderLabCommandNode> commands = new();
            List<string> programBlocks = new();
            List<string> includeBlocks = new();

            while (!IsAtEnd())
            {
                Token next = Peek();
                if (next.Kind == TokenKind.CloseBraceToken)
                    break;

                switch (next.Kind)
                {
                    case TokenKind.PassKeyword: passes.Add(ParseCodePass()); break;
                    case TokenKind.GrabPassKeyword: passes.Add(ParseGrabPass()); break;
                    case TokenKind.UsePassKeyword: passes.Add(ParseUsePass()); break;
                    case TokenKind.ProgramBlock: programBlocks.Add(Eat(TokenKind.ProgramBlock).Identifier ?? string.Empty); break;
                    default:
                        ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);
                        break;
                }
            }

            ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);

            Eat(TokenKind.CloseBraceToken);

            return new SubShaderNode
            {
                Passes = passes,
                Commands = commands,
                IncludeBlocks = includeBlocks
            };
        }

        private ShaderCodePassNode ParseCodePass()
        {
            Eat(TokenKind.PassKeyword);
            Eat(TokenKind.OpenBraceToken);

            List<ShaderLabCommandNode> commands = new();
            List<string> programBlocks = new();
            List<string> includeBlocks = new();

            ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);

            while (Match(TokenKind.ProgramBlock))
            {
                programBlocks.Add(Eat(TokenKind.ProgramBlock).Identifier ?? string.Empty);
                ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);
            }

            Eat(TokenKind.CloseBraceToken);

            return new ShaderCodePassNode
            {
                ProgramBlocks = programBlocks,
                Commands = commands,
                IncludeBlocks = includeBlocks
            };
        }

        private ShaderGrabPassNode ParseGrabPass()
        {
            Eat(TokenKind.GrabPassKeyword);
            Eat(TokenKind.OpenBraceToken);

            List<ShaderLabCommandNode> commands = new();
            List<string> includeBlocks = new();

            ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);

            string? name = null;
            if (Peek().Kind != TokenKind.CloseBraceToken)
            {
                name = ParseStringLiteral();

                ParseCommandsAndIncludeBlocksIfPresent(commands, includeBlocks);
            }

            Eat(TokenKind.CloseBraceToken);

            return new ShaderGrabPassNode
            {
                TextureName = name,
                Commands = commands,
                IncludeBlocks = includeBlocks
            };
        }

        private ShaderUsePassNode ParseUsePass()
        {
            Eat(TokenKind.UsePassKeyword);
            return new ShaderUsePassNode
            {
                PassName = ParseStringLiteral()
            };
        }

        private void ParseCommandsAndIncludeBlocksIfPresent(List<ShaderLabCommandNode> outCommands, List<string> outIncludeBlocks)
        {
            while (true)
            {
                int lastPosition = position;

                ParseCommandsIfPresent(outCommands);
                ParseIncludeBlocksIfPresent(outIncludeBlocks);

                if (lastPosition == position)
                    break;
            }
        }

        private void ParseCommandsIfPresent(List<ShaderLabCommandNode> outCommands)
        {
            bool run = true;
            while (run)
            {
                Token next = Peek();
                switch (next.Kind)
                {
                    case TokenKind.LightingKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandLightingNode>(next.Kind)); break;
                    case TokenKind.SeparateSpecularKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandSeparateSpecularNode>(next.Kind)); break;
                    case TokenKind.ZWriteKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandZWriteNode>(next.Kind)); break;
                    case TokenKind.AlphaToMaskKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandAlphaToMaskNode>(next.Kind)); break;
                    case TokenKind.ZClipKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandZClipNode>(next.Kind)); break;
                    case TokenKind.ConservativeKeyword: outCommands.Add(ParseBasicToggleCommand<ShaderLabCommandConservativeNode>(next.Kind)); break;
                    case TokenKind.TagsKeyword: outCommands.Add(ParseTagsCommand()); break;
                    case TokenKind.LodKeyword: outCommands.Add(ParseLodCommand()); break;
                    case TokenKind.CullKeyword: outCommands.Add(ParseCullCommand()); break;
                    case TokenKind.ZTestKeyword: outCommands.Add(ParseZTestCommand()); break;
                    case TokenKind.BlendKeyword: outCommands.Add(ParseBlendCommand()); break;
                    case TokenKind.OffsetKeyword: outCommands.Add(ParseOffsetCommand()); break;
                    case TokenKind.ColorMaskKeyword: outCommands.Add(ParseColorMaskCommand()); break;
                    case TokenKind.AlphaTestKeyword: outCommands.Add(ParseAlphaTestCommand()); break;
                    case TokenKind.FogKeyword: outCommands.Add(ParseFogCommand()); break;
                    case TokenKind.NameKeyword: outCommands.Add(ParseNameCommand()); break;
                    case TokenKind.BindChannelsKeyword: outCommands.Add(ParseBindChannelsCommand()); break;
                    case TokenKind.ColorKeyword: outCommands.Add(ParseColorCommand()); break;
                    case TokenKind.BlendOpKeyword: outCommands.Add(ParseBlendOpCommand()); break;
                    case TokenKind.MaterialKeyword: outCommands.Add(ParseMaterialCommand()); break;
                    case TokenKind.SetTextureKeyword: outCommands.Add(ParseSetTextureCommand()); break;
                    case TokenKind.ColorMaterialKeyword: outCommands.Add(ParseColorMaterialNode()); break;
                    case TokenKind.StencilKeyword: outCommands.Add(ParseStencilNode()); break;

                    default:
                        run = false;
                        break;
                }
            }
        }

        private ShaderLabCommandTagsNode ParseTagsCommand()
        {
            Eat(TokenKind.TagsKeyword);
            Eat(TokenKind.OpenBraceToken);

            Dictionary<string, string> tags = new();
            while (Peek().Kind != TokenKind.CloseBraceToken)
            {
                string key = ParseStringLiteral();
                Eat(TokenKind.EqualsToken);
                string val = ParseStringLiteral();

                tags.Add(key, val);
            }

            Eat(TokenKind.CloseBraceToken);

            return new ShaderLabCommandTagsNode
            {
                Tags = tags
            };
        }

        private ShaderLabCommandLodNode ParseLodCommand()
        {
            Eat(TokenKind.LodKeyword);
            int level = ParseIntegerLiteral();
            return new ShaderLabCommandLodNode
            {
                LodLevel = level,
            };
        }

        private PropertyReferenceOr<TOther> ParsePropertyReferenceOr<TOther>(Func<TOther> otherParser)
        {
            if (Match(TokenKind.BracketedStringLiteralToken))
            {
                return new PropertyReferenceOr<TOther> { Property = ParseBracketedStringLiteral() };
            }
            else
            {
                return new PropertyReferenceOr<TOther> { Value = otherParser() };
            }
        }

        private T ParseBasicToggleCommand<T>(TokenKind keyword)
            where T : ShaderLabBasicToggleCommandNode, new()
        {
            Eat(keyword);
            var prop = ParsePropertyReferenceOr(() =>
            {
                var kind = Eat(TokenKind.OnKeyword, TokenKind.OffKeyword, TokenKind.TrueKeyword, TokenKind.FalseKeyword).Kind;
                return kind is TokenKind.OnKeyword or TokenKind.TrueKeyword;
            });
            return new T { Enabled = prop };
        }

        private ShaderLabCommandCullNode ParseCullCommand()
        {
            Eat(TokenKind.CullKeyword);
            var prop = ParsePropertyReferenceOr(() =>
            {
                var kind = Eat(TokenKind.OffKeyword, TokenKind.FrontKeyword, TokenKind.BackKeyword, TokenKind.FalseKeyword).Kind;
                CullMode mode = default;
                if (kind is TokenKind.OffKeyword or TokenKind.FalseKeyword)
                    mode = CullMode.Off;
                else if (kind == TokenKind.FrontKeyword)
                    mode = CullMode.Front;
                else if (kind == TokenKind.BackKeyword)
                    mode = CullMode.Back;
                return mode;
            });
            return new ShaderLabCommandCullNode { Mode = prop };
        }

        private ShaderLabCommandZTestNode ParseZTestCommand()
        {
            Eat(TokenKind.ZTestKeyword);
            var prop = ParsePropertyReferenceOr(() => ParseEnum<ComparisonMode>("a valid comparison operator"));
            return new ShaderLabCommandZTestNode { Mode = prop };
        }

        private static readonly Dictionary<TokenKind, BlendFactor> blendFactors = new()
        {
            { TokenKind.OneKeyword, BlendFactor.One },
            { TokenKind.ZeroKeyword, BlendFactor.Zero },
            { TokenKind.SrcColorKeyword, BlendFactor.SrcColor },
            { TokenKind.SrcAlphaKeyword, BlendFactor.SrcAlpha },
            { TokenKind.SrcAlphaSaturateKeyword, BlendFactor.SrcAlphaSaturate },
            { TokenKind.DstColorKeyword, BlendFactor.DstColor },
            { TokenKind.DstAlphaKeyword, BlendFactor.DstAlpha },
            { TokenKind.OneMinusSrcColorKeyword, BlendFactor.OneMinusSrcColor },
            { TokenKind.OneMinusSrcAlphaKeyword, BlendFactor.OneMinusSrcAlpha },
            { TokenKind.OneMinusDstColorKeyword, BlendFactor.OneMinusDstColor },
            { TokenKind.OneMinusDstAlphaKeyword, BlendFactor.OneMinusDstAlpha }
        };
        private static readonly TokenKind[] blendFactorsKeys = blendFactors.Keys.ToArray();

        private ShaderLabCommandBlendNode ParseBlendCommand()
        {
            Eat(TokenKind.BlendKeyword);

            int renderTarget = 0;
            if (Match(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken))
            {
                renderTarget = ParseIntegerLiteral();
            }

            if (Match(TokenKind.OffKeyword, TokenKind.FalseKeyword))
            {
                Advance();
                return new ShaderLabCommandBlendNode { RenderTarget = renderTarget, Enabled = false };
            }

            var srcRGB = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));
            var dstRGB = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));

            var srcAlpha = srcRGB;
            var dstAlpha = dstRGB;
            if (Match(TokenKind.CommaToken))
            {
                Eat(TokenKind.CommaToken);
                srcAlpha = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));
                dstAlpha = ParsePropertyReferenceOr(() => blendFactors.GetValueOrDefault(Eat(blendFactorsKeys).Kind));
            }

            return new ShaderLabCommandBlendNode
            {
                RenderTarget = renderTarget,
                Enabled = true,
                SourceFactorRGB = srcRGB,
                DestinationFactorRGB = dstRGB,
                SourceFactorAlpha = srcAlpha,
                DestinationFactorAlpha = dstAlpha
            };
        }

        private ShaderLabCommandOffsetNode ParseOffsetCommand()
        {
            Eat(TokenKind.OffsetKeyword);
            var factor = ParsePropertyReferenceOr(ParseNumericLiteral);
            Eat(TokenKind.CommaToken);
            var units = ParsePropertyReferenceOr(ParseNumericLiteral);
            return new ShaderLabCommandOffsetNode { Factor = factor, Units = units };
        }

        private ShaderLabCommandColorMaskNode ParseColorMaskCommand()
        {
            Eat(TokenKind.ColorMaskKeyword);
            var mask = ParsePropertyReferenceOr(() =>
            {
                Token next = Peek();
                if (next.Kind is TokenKind.FloatLiteralToken or TokenKind.IntegerLiteralToken)
                {
                    string result = ParseNumericLiteral().ToString();
                    if (result != "0")
                        Error("the numeric literal 0", next);
                    return result;
                }
                else
                {
                    string result = ParseIdentifier();
                    if (!result.ToLower().All(x => x is 'r' or 'g' or 'b' or 'a'))
                        Error("a valid mask containing only the letter 'r', 'g', 'b', 'a'", next);
                    return result;
                }
            });
            int renderTarget = 0;
            if (Match(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken))
            {
                renderTarget = ParseIntegerLiteral();
            }
            return new ShaderLabCommandColorMaskNode { RenderTarget = renderTarget, Mask = mask };
        }

        private ShaderLabCommandAlphaTestNode ParseAlphaTestCommand()
        {
            Eat(TokenKind.AlphaTestKeyword);
            var prop = ParsePropertyReferenceOr(() => ParseEnum<ComparisonMode>("a valid comparison operator"));
            PropertyReferenceOr<float>? alpha = null;
            if (Match(TokenKind.FloatLiteralToken, TokenKind.IntegerLiteralToken, TokenKind.BracketedStringLiteralToken))
            {
                alpha = ParsePropertyReferenceOr(ParseNumericLiteral);
            }
            return new ShaderLabCommandAlphaTestNode { Mode = prop, AlphaValue = alpha };
        }

        private void ParseColor(out (float r, float g, float b, float a) color, out bool hasAlphaChannel)
        {
            hasAlphaChannel = false;
            float r, g, b, a = 1;
            Eat(TokenKind.OpenParenToken);
            r = ParseNumericLiteral();
            Eat(TokenKind.CommaToken);
            g = ParseNumericLiteral();
            Eat(TokenKind.CommaToken);
            b = ParseNumericLiteral();
            if (Match(TokenKind.CommaToken))
            {
                Eat(TokenKind.CommaToken);
                a = ParseNumericLiteral();
                hasAlphaChannel = true;
            }
            Eat(TokenKind.CloseParenToken);
            color = (r, g, b, a);
        }

        private ShaderLabCommandFogNode ParseFogCommand()
        {
            Eat(TokenKind.FogKeyword);
            Eat(TokenKind.OpenBraceToken);

            (float, float, float, float)? color = null;
            bool isEnabled;
            if (Match(TokenKind.ColorKeyword))
            {
                isEnabled = true;

                Eat(TokenKind.ColorKeyword);
                ParseColor(out var parsedColor, out _);
                color = parsedColor;
            }
            else
            {
                Eat(TokenKind.ModeKeyword);

                TokenKind modeKind = Eat(TokenKind.OffKeyword, TokenKind.GlobalKeyword).Kind;
                isEnabled = modeKind == TokenKind.GlobalKeyword;
            }

            Eat(TokenKind.CloseBraceToken);
            return new ShaderLabCommandFogNode { Enabled = isEnabled, Color = color };
        }

        private ShaderLabCommandNameNode ParseNameCommand()
        {
            Eat(TokenKind.NameKeyword);
            string name = ParseStringLiteral();
            return new ShaderLabCommandNameNode { Name = name };
        }

        private ShaderLabCommandBindChannelsNode ParseBindChannelsCommand()
        {
            Eat(TokenKind.BindChannelsKeyword);
            Eat(TokenKind.OpenBraceToken);

            Dictionary<BindChannel, BindChannel> bindings = new();
            while (Peek().Kind != TokenKind.CloseBraceToken)
            {
                Eat(TokenKind.BindKeyword);
                string source = ParseStringLiteral();
                Eat(TokenKind.CommaToken);
                Token targetToken = Advance();
                // Handle ShaderLab's ambiguous syntax: Could be a keyword or an identifier here, in the case of color.
                string target = targetToken.Kind == TokenKind.ColorKeyword ? "color" : targetToken.Identifier ?? String.Empty;
                if (ShaderLabSyntaxFacts.TryParseBindChannelName(source, out BindChannel sourceChannel) &&
                    ShaderLabSyntaxFacts.TryParseBindChannelName(target, out BindChannel targetChannel))
                {
                    bindings[sourceChannel] = targetChannel;
                }
                else
                {
                    Error($"Failed to parse channel binding from '{source}' to '{target}'.");
                }
            }

            Eat(TokenKind.CloseBraceToken);

            return new ShaderLabCommandBindChannelsNode { Bindings = bindings };
        }

        private ShaderLabCommandColorNode ParseColorCommand()
        {
            Eat(TokenKind.ColorKeyword);
            bool hasAlphaChannel = false;
            var prop = ParsePropertyReferenceOr(() =>
            {
                ParseColor(out var color, out hasAlphaChannel);
                return color;
            });
            return new ShaderLabCommandColorNode { Color = prop, HasAlphaChannel = hasAlphaChannel };
        }

        private static readonly Dictionary<TokenKind, BlendOp> blendOps = new()
        {
            { TokenKind.AddKeyword, BlendOp.Add },
            { TokenKind.SubKeyword, BlendOp.Sub },
            { TokenKind.RevSubKeyword, BlendOp.RevSub },
            { TokenKind.MinKeyword, BlendOp.Min },
            { TokenKind.MaxKeyword, BlendOp.Max },
            { TokenKind.LogicalClearKeyword, BlendOp.LogicalClear },
            { TokenKind.LogicalSetKeyword, BlendOp.LogicalSet },
            { TokenKind.LogicalCopyKeyword, BlendOp.LogicalCopy },
            { TokenKind.LogicalCopyInvertedKeyword, BlendOp.LogicalCopyInverted },
            { TokenKind.LogicalNoopKeyword, BlendOp.LogicalNoop },
            { TokenKind.LogicalInvertKeyword, BlendOp.LogicalInvert },
            { TokenKind.LogicalAndKeyword, BlendOp.LogicalAnd },
            { TokenKind.LogicalNandKeyword, BlendOp.LogicalNand },
            { TokenKind.LogicalOrKeyword, BlendOp.LogicalOr },
            { TokenKind.LogicalNorKeyword, BlendOp.LogicalNor },
            { TokenKind.LogicalXorKeyword, BlendOp.LogicalXor },
            { TokenKind.LogicalEquivKeyword, BlendOp.LogicalEquiv },
            { TokenKind.LogicalAndReverseKeyword, BlendOp.LogicalAndReverse },
            { TokenKind.LogicalOrReverseKeyword, BlendOp.LogicalOrReverse },
            { TokenKind.LogicalOrInvertedKeyword, BlendOp.LogicalOrInverted },
            { TokenKind.MultiplyKeyword, BlendOp.Multiply },
            { TokenKind.ScreenKeyword, BlendOp.Screen },
            { TokenKind.OverlayKeyword, BlendOp.Overlay },
            { TokenKind.DarkenKeyword, BlendOp.Darken },
            { TokenKind.LightenKeyword, BlendOp.Lighten },
            { TokenKind.ColorDodgeKeyword, BlendOp.ColorDodge },
            { TokenKind.ColorBurnKeyword, BlendOp.ColorBurn },
            { TokenKind.HardLightKeyword, BlendOp.HardLight },
            { TokenKind.SoftLightKeyword, BlendOp.SoftLight },
            { TokenKind.DifferenceKeyword, BlendOp.Difference },
            { TokenKind.ExclusionKeyword, BlendOp.Exclusion },
            { TokenKind.HSLHueKeyword, BlendOp.HSLHue },
            { TokenKind.HSLSaturationKeyword, BlendOp.HSLSaturation },
            { TokenKind.HSLColorKeyword, BlendOp.HSLColor },
            { TokenKind.HSLLuminosityKeyword, BlendOp.HSLLuminosity },
        };
        private static readonly TokenKind[] blendOpsKeys = blendOps.Keys.ToArray();
        private ShaderLabCommandBlendOpNode ParseBlendOpCommand()
        {
            Eat(TokenKind.BlendOpKeyword);
            var op = ParsePropertyReferenceOr(() => blendOps.GetValueOrDefault(Eat(blendOpsKeys).Kind));
            return new ShaderLabCommandBlendOpNode { BlendOp = op };
        }

        private static readonly Dictionary<TokenKind, FixedFunctionMaterialProperty> fixedFunctionsMatProps = new()
        {
            { TokenKind.DiffuseKeyword, FixedFunctionMaterialProperty.Diffuse },
            { TokenKind.SpecularKeyword, FixedFunctionMaterialProperty.Specular },
            { TokenKind.AmbientKeyword, FixedFunctionMaterialProperty.Ambient },
            { TokenKind.EmissionKeyword, FixedFunctionMaterialProperty.Emission },
            { TokenKind.ShininessKeyword, FixedFunctionMaterialProperty.Shininess },
        };
        private static readonly TokenKind[] fixedFunctionsMatPropsKeys = fixedFunctionsMatProps.Keys.ToArray();

        private ShaderLabCommandMaterialNode ParseMaterialCommand()
        {
            Eat(TokenKind.MaterialKeyword);
            Eat(TokenKind.OpenBraceToken);

            Dictionary<FixedFunctionMaterialProperty, PropertyReferenceOr<(float, float, float, float)>> props = new();
            while (!Match(TokenKind.CloseBraceToken))
            {
                var prop = fixedFunctionsMatProps.GetValueOrDefault(Eat(fixedFunctionsMatPropsKeys).Kind);
                var val = ParsePropertyReferenceOr(() =>
                {
                    ParseColor(out var color, out _);
                    return color;
                });

            }

            Eat(TokenKind.CloseBraceToken);

            return new ShaderLabCommandMaterialNode { Properties = props };
        }

        private ShaderLabCommandSetTextureNode ParseSetTextureCommand()
        {
            Eat(TokenKind.SetTextureKeyword);
            string name = ParseBracketedStringLiteral();
            Eat(TokenKind.OpenBraceToken);

            List<Token> tokens = new();
            while (!Match(TokenKind.CloseBraceToken))
            {
                tokens.Add(Advance());
            }

            Eat(TokenKind.CloseBraceToken);

            return new ShaderLabCommandSetTextureNode { TextureName = name, Body = tokens };
        }

        private ShaderLabCommandColorMaterialNode ParseColorMaterialNode()
        {
            Eat(TokenKind.ColorMaterialKeyword);
            bool ambient = Eat(TokenKind.EmissionKeyword, TokenKind.AmbientAndDiffuseKeyword).Kind == TokenKind.AmbientAndDiffuseKeyword;
            return new ShaderLabCommandColorMaterialNode { AmbientAndDiffuse = ambient };
        }

        private ShaderLabCommandStencilNode ParseStencilNode()
        {
            Eat(TokenKind.StencilKeyword);
            Eat(TokenKind.OpenBraceToken);

            // Set defaults
            var result = new ShaderLabCommandStencilNode
            {
                Ref = new PropertyReferenceOr<byte> { Value = 0 },
                ReadMask = new PropertyReferenceOr<byte> { Value = 255 },
                WriteMask = new PropertyReferenceOr<byte> { Value = 255 },
                ComparisonOperationBack = new PropertyReferenceOr<ComparisonMode> { Value = ComparisonMode.Always },
                PassOperationBack = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
                FailOperationBack = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
                ZFailOperationBack = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
                ComparisonOperationFront = new PropertyReferenceOr<ComparisonMode> { Value = ComparisonMode.Always },
                PassOperationFront = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
                FailOperationFront = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
                ZFailOperationFront = new PropertyReferenceOr<StencilOp> { Value = StencilOp.Keep },
            };

            StencilOp ParseStencilOp() => ParseEnum<StencilOp>("a valid stencil operator");
            ComparisonMode ParseComparisonMode() => ParseEnum<ComparisonMode>("a valid stencil comparison operator");

            while (!Match(TokenKind.CloseBraceToken))
            {
                Token next = Advance();
                switch (next.Kind)
                {
                    case TokenKind.RefKeyword: result.Ref = ParsePropertyReferenceOr(ParseByteLiteral); break;
                    case TokenKind.ReadMaskKeyword: result.ReadMask = ParsePropertyReferenceOr(ParseByteLiteral); break;
                    case TokenKind.WriteMaskKeyword: result.WriteMask = ParsePropertyReferenceOr(ParseByteLiteral); break;
                    case TokenKind.CompKeyword: result.ComparisonOperationBack = result.ComparisonOperationFront = ParsePropertyReferenceOr(ParseComparisonMode); break;
                    case TokenKind.PassKeyword: result.PassOperationBack = result.PassOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.FailKeyword: result.FailOperationBack = result.FailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.ZFailKeyword: result.ZFailOperationBack = result.ZFailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.CompBackKeyword: result.ComparisonOperationBack = ParsePropertyReferenceOr(ParseComparisonMode); break;
                    case TokenKind.PassBackKeyword: result.PassOperationBack = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.FailBackKeyword: result.FailOperationBack = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.ZFailBackKeyword: result.ZFailOperationBack = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.CompFrontKeyword: result.ComparisonOperationFront = ParsePropertyReferenceOr(ParseComparisonMode); break;
                    case TokenKind.PassFrontKeyword: result.PassOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.FailFrontKeyword: result.FailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;
                    case TokenKind.ZFailFrontKeyword: result.ZFailOperationFront = ParsePropertyReferenceOr(ParseStencilOp); break;

                    default:
                        Error("a valid stencil operation", next);
                        break;
                }
            }

            Eat(TokenKind.CloseBraceToken);

            return result;
        }
    }
}
namespace UnityShaderParser.ShaderLab
{
    public static class ShaderLabSyntaxFacts
    {
        public static bool TryParseShaderLabKeyword(string keyword, out TokenKind token)
        {
            token = default;

            switch (keyword.ToLower())
            {
                case "shader": token = TokenKind.ShaderKeyword; return true;
                case "properties": token = TokenKind.PropertiesKeyword; return true;
                case "range": token = TokenKind.RangeKeyword; return true;
                case "float": token = TokenKind.FloatKeyword; return true;
                case "integer": token = TokenKind.IntegerKeyword; return true;
                case "int": token = TokenKind.IntKeyword; return true;
                case "color": token = TokenKind.ColorKeyword; return true;
                case "vector": token = TokenKind.VectorKeyword; return true;
                case "2d": token = TokenKind._2DKeyword; return true;
                case "3d": token = TokenKind._3DKeyword; return true;
                case "cube": token = TokenKind.CubeKeyword; return true;
                case "2darray": token = TokenKind._2DArrayKeyword; return true;
                case "3darray": token = TokenKind._3DArrayKeyword; return true;
                case "cubearray": token = TokenKind.CubeArrayKeyword; return true;
                case "any": token = TokenKind.AnyKeyword; return true;
                case "rect": token = TokenKind.RectKeyword; return true;
                case "category": token = TokenKind.CategoryKeyword; return true;
                case "subshader": token = TokenKind.SubShaderKeyword; return true;
                case "tags": token = TokenKind.TagsKeyword; return true;
                case "pass": token = TokenKind.PassKeyword; return true;
                case "cgprogram": token = TokenKind.CgProgramKeyword; return true;
                case "cginclude": token = TokenKind.CgIncludeKeyword; return true;
                case "endcg": token = TokenKind.EndCgKeyword; return true;
                case "hlslprogram": token = TokenKind.HlslProgramKeyword; return true;
                case "hlslinclude": token = TokenKind.HlslIncludeKeyword; return true;
                case "endhlsl": token = TokenKind.EndHlslKeyword; return true;
                case "glslprogram": token = TokenKind.GlslProgramKeyword; return true;
                case "glslinclude": token = TokenKind.GlslIncludeKeyword; return true;
                case "endglsl": token = TokenKind.EndGlslKeyword; return true;
                case "fallback": token = TokenKind.FallbackKeyword; return true;
                case "customeditor": token = TokenKind.CustomEditorKeyword; return true;
                case "cull": token = TokenKind.CullKeyword; return true;
                case "zwrite": token = TokenKind.ZWriteKeyword; return true;
                case "ztest": token = TokenKind.ZTestKeyword; return true;
                case "offset": token = TokenKind.OffsetKeyword; return true;
                case "blend": token = TokenKind.BlendKeyword; return true;
                case "blendop": token = TokenKind.BlendOpKeyword; return true;
                case "colormask": token = TokenKind.ColorMaskKeyword; return true;
                case "alphatomask": token = TokenKind.AlphaToMaskKeyword; return true;
                case "zclip": token = TokenKind.ZClipKeyword; return true;
                case "conservative": token = TokenKind.ConservativeKeyword; return true;
                case "lod": token = TokenKind.LodKeyword; return true;
                case "name": token = TokenKind.NameKeyword; return true;
                case "lighting": token = TokenKind.LightingKeyword; return true;
                case "stencil": token = TokenKind.StencilKeyword; return true;
                case "ref": token = TokenKind.RefKeyword; return true;
                case "readmask": token = TokenKind.ReadMaskKeyword; return true;
                case "writemask": token = TokenKind.WriteMaskKeyword; return true;
                case "comp": token = TokenKind.CompKeyword; return true;
                case "compback": token = TokenKind.CompBackKeyword; return true;
                case "compfront": token = TokenKind.CompFrontKeyword; return true;
                case "fail": token = TokenKind.FailKeyword; return true;
                case "zfail": token = TokenKind.ZFailKeyword; return true;
                case "failback": token = TokenKind.FailBackKeyword; return true;
                case "failfront": token = TokenKind.FailFrontKeyword; return true;
                case "zfailback": token = TokenKind.ZFailBackKeyword; return true;
                case "zfailfront": token = TokenKind.ZFailFrontKeyword; return true;
                case "passfront": token = TokenKind.PassFrontKeyword; return true;
                case "passback": token = TokenKind.PassBackKeyword; return true;
                case "usepass": token = TokenKind.UsePassKeyword; return true;
                case "grabpass": token = TokenKind.GrabPassKeyword; return true;
                case "dependency": token = TokenKind.DependencyKeyword; return true;
                case "material": token = TokenKind.MaterialKeyword; return true;
                case "diffuse": token = TokenKind.DiffuseKeyword; return true;
                case "ambient": token = TokenKind.AmbientKeyword; return true;
                case "shininess": token = TokenKind.ShininessKeyword; return true;
                case "specular": token = TokenKind.SpecularKeyword; return true;
                case "emission": token = TokenKind.EmissionKeyword; return true;
                case "ambientanddiffuse": token = TokenKind.AmbientAndDiffuseKeyword; return true;
                case "fog": token = TokenKind.FogKeyword; return true;
                case "mode": token = TokenKind.ModeKeyword; return true;
                case "density": token = TokenKind.DensityKeyword; return true;
                case "separatespecular": token = TokenKind.SeparateSpecularKeyword; return true;
                case "settexture": token = TokenKind.SetTextureKeyword; return true;
                case "combine": token = TokenKind.CombineKeyword; return true;
                case "alpha": token = TokenKind.AlphaKeyword; return true;
                case "lerp": token = TokenKind.LerpKeyword; return true;
                case "double": token = TokenKind.DoubleKeyword; return true;
                case "quad": token = TokenKind.QuadKeyword; return true;
                case "constantcolor": token = TokenKind.ConstantColorKeyword; return true;
                case "matrix": token = TokenKind.MatrixKeyword; return true;
                case "alphatest": token = TokenKind.AlphaTestKeyword; return true;
                case "colormaterial": token = TokenKind.ColorMaterialKeyword; return true;
                case "bindchannels": token = TokenKind.BindChannelsKeyword; return true;
                case "bind": token = TokenKind.BindKeyword; return true;
                case "true": token = TokenKind.TrueKeyword; return true;
                case "false": token = TokenKind.FalseKeyword; return true;
                case "off": token = TokenKind.OffKeyword; return true;
                case "on": token = TokenKind.OnKeyword; return true;
                case "front": token = TokenKind.FrontKeyword; return true;
                case "back": token = TokenKind.BackKeyword; return true;
                case "one": token = TokenKind.OneKeyword; return true;
                case "zero": token = TokenKind.ZeroKeyword; return true;
                case "srccolor": token = TokenKind.SrcColorKeyword; return true;
                case "srcalpha": token = TokenKind.SrcAlphaKeyword; return true;
                case "srcalphasaturate": token = TokenKind.SrcAlphaSaturateKeyword; return true;
                case "dstcolor": token = TokenKind.DstColorKeyword; return true;
                case "dstalpha": token = TokenKind.DstAlphaKeyword; return true;
                case "oneminussrccolor": token = TokenKind.OneMinusSrcColorKeyword; return true;
                case "oneminussrcalpha": token = TokenKind.OneMinusSrcAlphaKeyword; return true;
                case "oneminusdstcolor": token = TokenKind.OneMinusDstColorKeyword; return true;
                case "oneminusdstalpha": token = TokenKind.OneMinusDstAlphaKeyword; return true;
                case "global": token = TokenKind.GlobalKeyword; return true;
                case "add": token = TokenKind.AddKeyword; return true;
                case "sub": token = TokenKind.SubKeyword; return true;
                case "revsub": token = TokenKind.RevSubKeyword; return true;
                case "min": token = TokenKind.MinKeyword; return true;
                case "max": token = TokenKind.MaxKeyword; return true;
                case "logicalclear": token = TokenKind.LogicalClearKeyword; return true;
                case "logicalset": token = TokenKind.LogicalSetKeyword; return true;
                case "logicalcopy": token = TokenKind.LogicalCopyKeyword; return true;
                case "logicalcopyinverted": token = TokenKind.LogicalCopyInvertedKeyword; return true;
                case "logicalnoop": token = TokenKind.LogicalNoopKeyword; return true;
                case "logicalinvert": token = TokenKind.LogicalInvertKeyword; return true;
                case "logicaland": token = TokenKind.LogicalAndKeyword; return true;
                case "logicalnand": token = TokenKind.LogicalNandKeyword; return true;
                case "logicalor": token = TokenKind.LogicalOrKeyword; return true;
                case "logicalnor": token = TokenKind.LogicalNorKeyword; return true;
                case "logicalxor": token = TokenKind.LogicalXorKeyword; return true;
                case "logicalequiv": token = TokenKind.LogicalEquivKeyword; return true;
                case "logicalandreverse": token = TokenKind.LogicalAndReverseKeyword; return true;
                case "logicalorreverse": token = TokenKind.LogicalOrReverseKeyword; return true;
                case "logicalorinverted": token = TokenKind.LogicalOrInvertedKeyword; return true;
                case "multiply": token = TokenKind.MultiplyKeyword; return true;
                case "screen": token = TokenKind.ScreenKeyword; return true;
                case "overlay": token = TokenKind.OverlayKeyword; return true;
                case "darken": token = TokenKind.DarkenKeyword; return true;
                case "lighten": token = TokenKind.LightenKeyword; return true;
                case "colordodge": token = TokenKind.ColorDodgeKeyword; return true;
                case "colorburn": token = TokenKind.ColorBurnKeyword; return true;
                case "hardlight": token = TokenKind.HardLightKeyword; return true;
                case "softlight": token = TokenKind.SoftLightKeyword; return true;
                case "difference": token = TokenKind.DifferenceKeyword; return true;
                case "exclusion": token = TokenKind.ExclusionKeyword; return true;
                case "hslhue": token = TokenKind.HSLHueKeyword; return true;
                case "hslsaturation": token = TokenKind.HSLSaturationKeyword; return true;
                case "hslcolor": token = TokenKind.HSLColorKeyword; return true;
                case "hslluminosity": token = TokenKind.HSLLuminosityKeyword; return true;
                default: return false;
            }
        }

        public static bool TryParseBindChannelName(string name, out BindChannel bindChannel)
        {
            bindChannel = default;

            switch (name.ToLower())
            {
                case "vertex": bindChannel = BindChannel.Vertex; return true;
                case "normal": bindChannel = BindChannel.Normal; return true;
                case "tangent": bindChannel = BindChannel.Tangent; return true;
                case "texcoord0": case "texcoord": bindChannel = BindChannel.TexCoord0; return true;
                case "texcoord1": bindChannel = BindChannel.TexCoord1; return true;
                case "texcoord2": bindChannel = BindChannel.TexCoord2; return true;
                case "texcoord3": bindChannel = BindChannel.TexCoord3; return true;
                case "texcoord4": bindChannel = BindChannel.TexCoord4; return true;
                case "texcoord5": bindChannel = BindChannel.TexCoord5; return true;
                case "texcoord6": bindChannel = BindChannel.TexCoord6; return true;
                case "texcoord7": bindChannel = BindChannel.TexCoord7; return true;
                case "color": bindChannel = BindChannel.Color; return true;
                default: return false;
            }
        }

        public static TextureType ShaderPropertyTypeToTextureType(ShaderPropertyKind kind)
        {
            switch (kind)
            {
                case ShaderPropertyKind.Texture2D: return TextureType.Texture2D; 
                case ShaderPropertyKind.Texture3D: return TextureType.Texture3D; 
                case ShaderPropertyKind.TextureCube: return TextureType.TextureCube; 
                case ShaderPropertyKind.TextureAny: return TextureType.TextureAny; 
                case ShaderPropertyKind.Texture2DArray: return TextureType.Texture2DArray; 
                case ShaderPropertyKind.Texture3DArray: return TextureType.Texture3DArray; 
                case ShaderPropertyKind.TextureCubeArray: return TextureType.TextureCubeArray;
                default: return default;
            }
        }
    }

}

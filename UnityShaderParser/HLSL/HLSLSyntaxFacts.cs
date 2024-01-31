﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityShaderParser.HLSL
{
    public static class HLSLSyntaxFacts
    {
        public static bool TryParseHLSLKeyword(string keyword, out TokenKind token)
        {
            token = default;

            switch (keyword)
            {
                case "AppendStructuredBuffer": token = TokenKind.AppendStructuredBufferKeyword; return true;
                case "BlendState": token = TokenKind.BlendStateKeyword; return true;
                case "bool": token = TokenKind.BoolKeyword; return true;
                case "bool1": token = TokenKind.Bool1Keyword; return true;
                case "bool2": token = TokenKind.Bool2Keyword; return true;
                case "bool3": token = TokenKind.Bool3Keyword; return true;
                case "bool4": token = TokenKind.Bool4Keyword; return true;
                case "bool1x1": token = TokenKind.Bool1x1Keyword; return true;
                case "bool1x2": token = TokenKind.Bool1x2Keyword; return true;
                case "bool1x3": token = TokenKind.Bool1x3Keyword; return true;
                case "bool1x4": token = TokenKind.Bool1x4Keyword; return true;
                case "bool2x1": token = TokenKind.Bool2x1Keyword; return true;
                case "bool2x2": token = TokenKind.Bool2x2Keyword; return true;
                case "bool2x3": token = TokenKind.Bool2x3Keyword; return true;
                case "bool2x4": token = TokenKind.Bool2x4Keyword; return true;
                case "bool3x1": token = TokenKind.Bool3x1Keyword; return true;
                case "bool3x2": token = TokenKind.Bool3x2Keyword; return true;
                case "bool3x3": token = TokenKind.Bool3x3Keyword; return true;
                case "bool3x4": token = TokenKind.Bool3x4Keyword; return true;
                case "bool4x1": token = TokenKind.Bool4x1Keyword; return true;
                case "bool4x2": token = TokenKind.Bool4x2Keyword; return true;
                case "bool4x3": token = TokenKind.Bool4x3Keyword; return true;
                case "bool4x4": token = TokenKind.Bool4x4Keyword; return true;
                case "Buffer": token = TokenKind.BufferKeyword; return true;
                case "ByteAddressBuffer": token = TokenKind.ByteAddressBufferKeyword; return true;
                case "break": token = TokenKind.BreakKeyword; return true;
                case "case": token = TokenKind.CaseKeyword; return true;
                case "cbuffer": token = TokenKind.CBufferKeyword; return true;
                case "centroid": token = TokenKind.CentroidKeyword; return true;
                case "class": token = TokenKind.ClassKeyword; return true;
                case "column_major": token = TokenKind.ColumnMajorKeyword; return true;
                case "compile": token = TokenKind.CompileKeyword; return true;
                case "CompileShader": token = TokenKind.CompileShaderKeyword; return true;
                case "constantbuffer": token = TokenKind.ConstantBufferKeyword; return true;
                case "const": token = TokenKind.ConstKeyword; return true;
                case "ConsumeStructuredBuffer": token = TokenKind.ConsumeStructuredBufferKeyword; return true;
                case "continue": token = TokenKind.ContinueKeyword; return true;
                case "default": token = TokenKind.DefaultKeyword; return true;
                case "def": token = TokenKind.DefKeyword; return true;
                case "DepthStencilState": token = TokenKind.DepthStencilStateKeyword; return true;
                case "discard": token = TokenKind.DiscardKeyword; return true;
                case "do": token = TokenKind.DoKeyword; return true;
                case "double": token = TokenKind.DoubleKeyword; return true;
                case "double1": token = TokenKind.Double1Keyword; return true;
                case "double2": token = TokenKind.Double2Keyword; return true;
                case "double3": token = TokenKind.Double3Keyword; return true;
                case "double4": token = TokenKind.Double4Keyword; return true;
                case "double1x1": token = TokenKind.Double1x1Keyword; return true;
                case "double1x2": token = TokenKind.Double1x2Keyword; return true;
                case "double1x3": token = TokenKind.Double1x3Keyword; return true;
                case "double1x4": token = TokenKind.Double1x4Keyword; return true;
                case "double2x1": token = TokenKind.Double2x1Keyword; return true;
                case "double2x2": token = TokenKind.Double2x2Keyword; return true;
                case "double2x3": token = TokenKind.Double2x3Keyword; return true;
                case "double2x4": token = TokenKind.Double2x4Keyword; return true;
                case "double3x1": token = TokenKind.Double3x1Keyword; return true;
                case "double3x2": token = TokenKind.Double3x2Keyword; return true;
                case "double3x3": token = TokenKind.Double3x3Keyword; return true;
                case "double3x4": token = TokenKind.Double3x4Keyword; return true;
                case "double4x1": token = TokenKind.Double4x1Keyword; return true;
                case "double4x2": token = TokenKind.Double4x2Keyword; return true;
                case "double4x3": token = TokenKind.Double4x3Keyword; return true;
                case "double4x4": token = TokenKind.Double4x4Keyword; return true;
                case "else": token = TokenKind.ElseKeyword; return true;
                case "error": token = TokenKind.ErrorKeyword; return true;
                case "export": token = TokenKind.ExportKeyword; return true;
                case "extern": token = TokenKind.ExternKeyword; return true;
                case "float": token = TokenKind.FloatKeyword; return true;
                case "float1": token = TokenKind.Float1Keyword; return true;
                case "float2": token = TokenKind.Float2Keyword; return true;
                case "float3": token = TokenKind.Float3Keyword; return true;
                case "float4": token = TokenKind.Float4Keyword; return true;
                case "float1x1": token = TokenKind.Float1x1Keyword; return true;
                case "float1x2": token = TokenKind.Float1x2Keyword; return true;
                case "float1x3": token = TokenKind.Float1x3Keyword; return true;
                case "float1x4": token = TokenKind.Float1x4Keyword; return true;
                case "float2x1": token = TokenKind.Float2x1Keyword; return true;
                case "float2x2": token = TokenKind.Float2x2Keyword; return true;
                case "float2x3": token = TokenKind.Float2x3Keyword; return true;
                case "float2x4": token = TokenKind.Float2x4Keyword; return true;
                case "float3x1": token = TokenKind.Float3x1Keyword; return true;
                case "float3x2": token = TokenKind.Float3x2Keyword; return true;
                case "float3x3": token = TokenKind.Float3x3Keyword; return true;
                case "float3x4": token = TokenKind.Float3x4Keyword; return true;
                case "float4x1": token = TokenKind.Float4x1Keyword; return true;
                case "float4x2": token = TokenKind.Float4x2Keyword; return true;
                case "float4x3": token = TokenKind.Float4x3Keyword; return true;
                case "float4x4": token = TokenKind.Float4x4Keyword; return true;
                case "for": token = TokenKind.ForKeyword; return true;
                case "GeometryShader": token = TokenKind.GeometryShaderKeyword; return true;
                case "globallycoherent": token = TokenKind.GloballycoherentKeyword; return true;
                case "groupshared": token = TokenKind.GroupsharedKeyword; return true;
                case "half": token = TokenKind.HalfKeyword; return true;
                case "half1": token = TokenKind.Half1Keyword; return true;
                case "half2": token = TokenKind.Half2Keyword; return true;
                case "half3": token = TokenKind.Half3Keyword; return true;
                case "half4": token = TokenKind.Half4Keyword; return true;
                case "half1x1": token = TokenKind.Half1x1Keyword; return true;
                case "half1x2": token = TokenKind.Half1x2Keyword; return true;
                case "half1x3": token = TokenKind.Half1x3Keyword; return true;
                case "half1x4": token = TokenKind.Half1x4Keyword; return true;
                case "half2x1": token = TokenKind.Half2x1Keyword; return true;
                case "half2x2": token = TokenKind.Half2x2Keyword; return true;
                case "half2x3": token = TokenKind.Half2x3Keyword; return true;
                case "half2x4": token = TokenKind.Half2x4Keyword; return true;
                case "half3x1": token = TokenKind.Half3x1Keyword; return true;
                case "half3x2": token = TokenKind.Half3x2Keyword; return true;
                case "half3x3": token = TokenKind.Half3x3Keyword; return true;
                case "half3x4": token = TokenKind.Half3x4Keyword; return true;
                case "half4x1": token = TokenKind.Half4x1Keyword; return true;
                case "half4x2": token = TokenKind.Half4x2Keyword; return true;
                case "half4x3": token = TokenKind.Half4x3Keyword; return true;
                case "half4x4": token = TokenKind.Half4x4Keyword; return true;
                case "if": token = TokenKind.IfKeyword; return true;
                case "indices": token = TokenKind.IndicesKeyword; return true;
                case "in": token = TokenKind.InKeyword; return true;
                case "inline": token = TokenKind.InlineKeyword; return true;
                case "inout": token = TokenKind.InoutKeyword; return true;
                case "InputPatch": token = TokenKind.InputPatchKeyword; return true;
                case "int": token = TokenKind.IntKeyword; return true;
                case "int1": token = TokenKind.Int1Keyword; return true;
                case "int2": token = TokenKind.Int2Keyword; return true;
                case "int3": token = TokenKind.Int3Keyword; return true;
                case "int4": token = TokenKind.Int4Keyword; return true;
                case "int1x1": token = TokenKind.Int1x1Keyword; return true;
                case "int1x2": token = TokenKind.Int1x2Keyword; return true;
                case "int1x3": token = TokenKind.Int1x3Keyword; return true;
                case "int1x4": token = TokenKind.Int1x4Keyword; return true;
                case "int2x1": token = TokenKind.Int2x1Keyword; return true;
                case "int2x2": token = TokenKind.Int2x2Keyword; return true;
                case "int2x3": token = TokenKind.Int2x3Keyword; return true;
                case "int2x4": token = TokenKind.Int2x4Keyword; return true;
                case "int3x1": token = TokenKind.Int3x1Keyword; return true;
                case "int3x2": token = TokenKind.Int3x2Keyword; return true;
                case "int3x3": token = TokenKind.Int3x3Keyword; return true;
                case "int3x4": token = TokenKind.Int3x4Keyword; return true;
                case "int4x1": token = TokenKind.Int4x1Keyword; return true;
                case "int4x2": token = TokenKind.Int4x2Keyword; return true;
                case "int4x3": token = TokenKind.Int4x3Keyword; return true;
                case "int4x4": token = TokenKind.Int4x4Keyword; return true;
                case "interface": token = TokenKind.InterfaceKeyword; return true;
                case "line": token = TokenKind.LineKeyword; return true;
                case "lineadj": token = TokenKind.LineAdjKeyword; return true;
                case "linear": token = TokenKind.LinearKeyword; return true;
                case "LineStream": token = TokenKind.LineStreamKeyword; return true;
                case "matrix": token = TokenKind.MatrixKeyword; return true;
                case "message": token = TokenKind.MessageKeyword; return true;
                case "min10float": token = TokenKind.Min10FloatKeyword; return true;
                case "min10float1": token = TokenKind.Min10Float1Keyword; return true;
                case "min10float2": token = TokenKind.Min10Float2Keyword; return true;
                case "min10float3": token = TokenKind.Min10Float3Keyword; return true;
                case "min10float4": token = TokenKind.Min10Float4Keyword; return true;
                case "min10float1x1": token = TokenKind.Min10Float1x1Keyword; return true;
                case "min10float1x2": token = TokenKind.Min10Float1x2Keyword; return true;
                case "min10float1x3": token = TokenKind.Min10Float1x3Keyword; return true;
                case "min10float1x4": token = TokenKind.Min10Float1x4Keyword; return true;
                case "min10float2x1": token = TokenKind.Min10Float2x1Keyword; return true;
                case "min10float2x2": token = TokenKind.Min10Float2x2Keyword; return true;
                case "min10float2x3": token = TokenKind.Min10Float2x3Keyword; return true;
                case "min10float2x4": token = TokenKind.Min10Float2x4Keyword; return true;
                case "min10float3x1": token = TokenKind.Min10Float3x1Keyword; return true;
                case "min10float3x2": token = TokenKind.Min10Float3x2Keyword; return true;
                case "min10float3x3": token = TokenKind.Min10Float3x3Keyword; return true;
                case "min10float3x4": token = TokenKind.Min10Float3x4Keyword; return true;
                case "min10float4x1": token = TokenKind.Min10Float4x1Keyword; return true;
                case "min10float4x2": token = TokenKind.Min10Float4x2Keyword; return true;
                case "min10float4x3": token = TokenKind.Min10Float4x3Keyword; return true;
                case "min10float4x4": token = TokenKind.Min10Float4x4Keyword; return true;
                case "min12int": token = TokenKind.Min12IntKeyword; return true;
                case "min12int1": token = TokenKind.Min12Int1Keyword; return true;
                case "min12int2": token = TokenKind.Min12Int2Keyword; return true;
                case "min12int3": token = TokenKind.Min12Int3Keyword; return true;
                case "min12int4": token = TokenKind.Min12Int4Keyword; return true;
                case "min12int1x1": token = TokenKind.Min12Int1x1Keyword; return true;
                case "min12int1x2": token = TokenKind.Min12Int1x2Keyword; return true;
                case "min12int1x3": token = TokenKind.Min12Int1x3Keyword; return true;
                case "min12int1x4": token = TokenKind.Min12Int1x4Keyword; return true;
                case "min12int2x1": token = TokenKind.Min12Int2x1Keyword; return true;
                case "min12int2x2": token = TokenKind.Min12Int2x2Keyword; return true;
                case "min12int2x3": token = TokenKind.Min12Int2x3Keyword; return true;
                case "min12int2x4": token = TokenKind.Min12Int2x4Keyword; return true;
                case "min12int3x1": token = TokenKind.Min12Int3x1Keyword; return true;
                case "min12int3x2": token = TokenKind.Min12Int3x2Keyword; return true;
                case "min12int3x3": token = TokenKind.Min12Int3x3Keyword; return true;
                case "min12int3x4": token = TokenKind.Min12Int3x4Keyword; return true;
                case "min12int4x1": token = TokenKind.Min12Int4x1Keyword; return true;
                case "min12int4x2": token = TokenKind.Min12Int4x2Keyword; return true;
                case "min12int4x3": token = TokenKind.Min12Int4x3Keyword; return true;
                case "min12int4x4": token = TokenKind.Min12Int4x4Keyword; return true;
                case "min16float": token = TokenKind.Min16FloatKeyword; return true;
                case "min16float1": token = TokenKind.Min16Float1Keyword; return true;
                case "min16float2": token = TokenKind.Min16Float2Keyword; return true;
                case "min16float3": token = TokenKind.Min16Float3Keyword; return true;
                case "min16float4": token = TokenKind.Min16Float4Keyword; return true;
                case "min16float1x1": token = TokenKind.Min16Float1x1Keyword; return true;
                case "min16float1x2": token = TokenKind.Min16Float1x2Keyword; return true;
                case "min16float1x3": token = TokenKind.Min16Float1x3Keyword; return true;
                case "min16float1x4": token = TokenKind.Min16Float1x4Keyword; return true;
                case "min16float2x1": token = TokenKind.Min16Float2x1Keyword; return true;
                case "min16float2x2": token = TokenKind.Min16Float2x2Keyword; return true;
                case "min16float2x3": token = TokenKind.Min16Float2x3Keyword; return true;
                case "min16float2x4": token = TokenKind.Min16Float2x4Keyword; return true;
                case "min16float3x1": token = TokenKind.Min16Float3x1Keyword; return true;
                case "min16float3x2": token = TokenKind.Min16Float3x2Keyword; return true;
                case "min16float3x3": token = TokenKind.Min16Float3x3Keyword; return true;
                case "min16float3x4": token = TokenKind.Min16Float3x4Keyword; return true;
                case "min16float4x1": token = TokenKind.Min16Float4x1Keyword; return true;
                case "min16float4x2": token = TokenKind.Min16Float4x2Keyword; return true;
                case "min16float4x3": token = TokenKind.Min16Float4x3Keyword; return true;
                case "min16float4x4": token = TokenKind.Min16Float4x4Keyword; return true;
                case "min16int": token = TokenKind.Min16IntKeyword; return true;
                case "min16int1": token = TokenKind.Min16Int1Keyword; return true;
                case "min16int2": token = TokenKind.Min16Int2Keyword; return true;
                case "min16int3": token = TokenKind.Min16Int3Keyword; return true;
                case "min16int4": token = TokenKind.Min16Int4Keyword; return true;
                case "min16int1x1": token = TokenKind.Min16Int1x1Keyword; return true;
                case "min16int1x2": token = TokenKind.Min16Int1x2Keyword; return true;
                case "min16int1x3": token = TokenKind.Min16Int1x3Keyword; return true;
                case "min16int1x4": token = TokenKind.Min16Int1x4Keyword; return true;
                case "min16int2x1": token = TokenKind.Min16Int2x1Keyword; return true;
                case "min16int2x2": token = TokenKind.Min16Int2x2Keyword; return true;
                case "min16int2x3": token = TokenKind.Min16Int2x3Keyword; return true;
                case "min16int2x4": token = TokenKind.Min16Int2x4Keyword; return true;
                case "min16int3x1": token = TokenKind.Min16Int3x1Keyword; return true;
                case "min16int3x2": token = TokenKind.Min16Int3x2Keyword; return true;
                case "min16int3x3": token = TokenKind.Min16Int3x3Keyword; return true;
                case "min16int3x4": token = TokenKind.Min16Int3x4Keyword; return true;
                case "min16int4x1": token = TokenKind.Min16Int4x1Keyword; return true;
                case "min16int4x2": token = TokenKind.Min16Int4x2Keyword; return true;
                case "min16int4x3": token = TokenKind.Min16Int4x3Keyword; return true;
                case "min16int4x4": token = TokenKind.Min16Int4x4Keyword; return true;
                case "min16uint": token = TokenKind.Min16UintKeyword; return true;
                case "min16uint1": token = TokenKind.Min16Uint1Keyword; return true;
                case "min16uint2": token = TokenKind.Min16Uint2Keyword; return true;
                case "min16uint3": token = TokenKind.Min16Uint3Keyword; return true;
                case "min16uint4": token = TokenKind.Min16Uint4Keyword; return true;
                case "min16uint1x1": token = TokenKind.Min16Uint1x1Keyword; return true;
                case "min16uint1x2": token = TokenKind.Min16Uint1x2Keyword; return true;
                case "min16uint1x3": token = TokenKind.Min16Uint1x3Keyword; return true;
                case "min16uint1x4": token = TokenKind.Min16Uint1x4Keyword; return true;
                case "min16uint2x1": token = TokenKind.Min16Uint2x1Keyword; return true;
                case "min16uint2x2": token = TokenKind.Min16Uint2x2Keyword; return true;
                case "min16uint2x3": token = TokenKind.Min16Uint2x3Keyword; return true;
                case "min16uint2x4": token = TokenKind.Min16Uint2x4Keyword; return true;
                case "min16uint3x1": token = TokenKind.Min16Uint3x1Keyword; return true;
                case "min16uint3x2": token = TokenKind.Min16Uint3x2Keyword; return true;
                case "min16uint3x3": token = TokenKind.Min16Uint3x3Keyword; return true;
                case "min16uint3x4": token = TokenKind.Min16Uint3x4Keyword; return true;
                case "min16uint4x1": token = TokenKind.Min16Uint4x1Keyword; return true;
                case "min16uint4x2": token = TokenKind.Min16Uint4x2Keyword; return true;
                case "min16uint4x3": token = TokenKind.Min16Uint4x3Keyword; return true;
                case "min16uint4x4": token = TokenKind.Min16Uint4x4Keyword; return true;
                case "namespace": token = TokenKind.NamespaceKeyword; return true;
                case "nointerpolation": token = TokenKind.NointerpolationKeyword; return true;
                case "noperspective": token = TokenKind.NoperspectiveKeyword; return true;
                case "NULL": token = TokenKind.NullKeyword; return true;
                case "out": token = TokenKind.OutKeyword; return true;
                case "OutputPatch": token = TokenKind.OutputPatchKeyword; return true;
                case "packmatrix": token = TokenKind.PackMatrixKeyword; return true;
                case "packoffset": token = TokenKind.PackoffsetKeyword; return true;
                case "pass": token = TokenKind.PassKeyword; return true;
                case "payload": token = TokenKind.PayloadKeyword; return true;
                case "PixelShader": token = TokenKind.PixelShaderKeyword; return true;
                case "point": token = TokenKind.PointKeyword; return true;
                case "PointStream": token = TokenKind.PointStreamKeyword; return true;
                case "pragma": token = TokenKind.PragmaKeyword; return true;
                case "precise": token = TokenKind.PreciseKeyword; return true;
                case "primitives": token = TokenKind.PrimitivesKeyword; return true;
                case "rasterizerorderedbuffer": token = TokenKind.RasterizerOrderedBufferKeyword; return true;
                case "rasterizerorderedbyteaddressbuffer": token = TokenKind.RasterizerOrderedByteAddressBufferKeyword; return true;
                case "rasterizerorderedstructuredbuffer": token = TokenKind.RasterizerOrderedStructuredBufferKeyword; return true;
                case "rasterizerorderedtexture1d": token = TokenKind.RasterizerOrderedTexture1DKeyword; return true;
                case "rasterizerorderedtexture1darray": token = TokenKind.RasterizerOrderedTexture1DArrayKeyword; return true;
                case "rasterizerorderedtexture2d": token = TokenKind.RasterizerOrderedTexture2DKeyword; return true;
                case "rasterizerorderedtexture2darray": token = TokenKind.RasterizerOrderedTexture2DArrayKeyword; return true;
                case "rasterizerorderedtexture3d": token = TokenKind.RasterizerOrderedTexture3DKeyword; return true;
                case "RasterizerState": token = TokenKind.RasterizerStateKeyword; return true;
                case "register": token = TokenKind.RegisterKeyword; return true;
                case "return": token = TokenKind.ReturnKeyword; return true;
                case "row_major": token = TokenKind.RowMajorKeyword; return true;
                case "RWBuffer": token = TokenKind.RWBufferKeyword; return true;
                case "RWByteAddressBuffer": token = TokenKind.RWByteAddressBufferKeyword; return true;
                case "RWStructuredBuffer": token = TokenKind.RWStructuredBufferKeyword; return true;
                case "RWTexture1D": token = TokenKind.RWTexture1DKeyword; return true;
                case "RWTexture1DArray": token = TokenKind.RWTexture1DArrayKeyword; return true;
                case "RWTexture2D": token = TokenKind.RWTexture2DKeyword; return true;
                case "RWTexture2DArray": token = TokenKind.RWTexture2DArrayKeyword; return true;
                case "RWTexture3D": token = TokenKind.RWTexture3DKeyword; return true;
                case "sample": token = TokenKind.SampleKeyword; return true;
                case "sampler": token = TokenKind.SamplerKeyword; return true;
                case "sampler1d": token = TokenKind.Sampler1DKeyword; return true;
                case "sampler2d": token = TokenKind.Sampler2DKeyword; return true;
                case "sampler3d": token = TokenKind.Sampler3DKeyword; return true;
                case "samplercube": token = TokenKind.SamplerCubeKeyword; return true;
                case "SamplerComparisonState": token = TokenKind.SamplerComparisonStateKeyword; return true;
                case "SamplerState": token = TokenKind.SamplerStateKeyword; return true;
                case "SamplerStateLegacy": token = TokenKind.SamplerStateLegacyKeyword; return true;
                case "shared": token = TokenKind.SharedKeyword; return true;
                case "snorm": token = TokenKind.SNormKeyword; return true;
                case "static": token = TokenKind.StaticKeyword; return true;
                case "string": token = TokenKind.StringKeyword; return true;
                case "struct": token = TokenKind.StructKeyword; return true;
                case "StructuredBuffer": token = TokenKind.StructuredBufferKeyword; return true;
                case "switch": token = TokenKind.SwitchKeyword; return true;
                case "tbuffer": token = TokenKind.TBufferKeyword; return true;
                case "technique": token = TokenKind.TechniqueKeyword; return true;
                case "technique10": token = TokenKind.Technique10Keyword; return true;
                case "technique11": token = TokenKind.Technique11Keyword; return true;
                case "texture": token = TokenKind.TextureKeyword; return true;
                case "Texture2DLegacy": token = TokenKind.Texture2DLegacyKeyword; return true;
                case "TextureCubeLegacy": token = TokenKind.TextureCubeLegacyKeyword; return true;
                case "Texture1D": token = TokenKind.Texture1DKeyword; return true;
                case "Texture1DArray": token = TokenKind.Texture1DArrayKeyword; return true;
                case "Texture2D": token = TokenKind.Texture2DKeyword; return true;
                case "Texture2DArray": token = TokenKind.Texture2DArrayKeyword; return true;
                case "Texture2DMS": token = TokenKind.Texture2DMSKeyword; return true;
                case "Texture2DMSArray": token = TokenKind.Texture2DMSArrayKeyword; return true;
                case "Texture3D": token = TokenKind.Texture3DKeyword; return true;
                case "TextureCube": token = TokenKind.TextureCubeKeyword; return true;
                case "TextureCubeArray": token = TokenKind.TextureCubeArrayKeyword; return true;
                case "triangle": token = TokenKind.TriangleKeyword; return true;
                case "triangleadj": token = TokenKind.TriangleAdjKeyword; return true;
                case "TriangleStream": token = TokenKind.TriangleStreamKeyword; return true;
                case "typedef": token = TokenKind.TypedefKeyword; return true;
                case "uniform": token = TokenKind.UniformKeyword; return true;
                case "unorm": token = TokenKind.UNormKeyword; return true;
                case "uint": token = TokenKind.UintKeyword; return true;
                case "uint1": token = TokenKind.Uint1Keyword; return true;
                case "uint2": token = TokenKind.Uint2Keyword; return true;
                case "uint3": token = TokenKind.Uint3Keyword; return true;
                case "uint4": token = TokenKind.Uint4Keyword; return true;
                case "uint1x1": token = TokenKind.Uint1x1Keyword; return true;
                case "uint1x2": token = TokenKind.Uint1x2Keyword; return true;
                case "uint1x3": token = TokenKind.Uint1x3Keyword; return true;
                case "uint1x4": token = TokenKind.Uint1x4Keyword; return true;
                case "uint2x1": token = TokenKind.Uint2x1Keyword; return true;
                case "uint2x2": token = TokenKind.Uint2x2Keyword; return true;
                case "uint2x3": token = TokenKind.Uint2x3Keyword; return true;
                case "uint2x4": token = TokenKind.Uint2x4Keyword; return true;
                case "uint3x1": token = TokenKind.Uint3x1Keyword; return true;
                case "uint3x2": token = TokenKind.Uint3x2Keyword; return true;
                case "uint3x3": token = TokenKind.Uint3x3Keyword; return true;
                case "uint3x4": token = TokenKind.Uint3x4Keyword; return true;
                case "uint4x1": token = TokenKind.Uint4x1Keyword; return true;
                case "uint4x2": token = TokenKind.Uint4x2Keyword; return true;
                case "uint4x3": token = TokenKind.Uint4x3Keyword; return true;
                case "uint4x4": token = TokenKind.Uint4x4Keyword; return true;
                case "vector": token = TokenKind.VectorKeyword; return true;
                case "VertexShader": token = TokenKind.VertexShaderKeyword; return true;
                case "vertices": token = TokenKind.VerticesKeyword; return true;
                case "volatile": token = TokenKind.VolatileKeyword; return true;
                case "void": token = TokenKind.VoidKeyword; return true;
                case "warning": token = TokenKind.WarningKeyword; return true;
                case "while": token = TokenKind.WhileKeyword; return true;
                case "true": token = TokenKind.TrueKeyword; return true;
                case "false": token = TokenKind.FalseKeyword; return true;
                case "unsigned": token = TokenKind.UnsignedKeyword; return true;
                case "dword": token = TokenKind.DwordKeyword; return true;
                case "compile_fragment": token = TokenKind.CompileFragmentKeyword; return true;
                case "DepthStencilView": token = TokenKind.DepthStencilViewKeyword; return true;
                case "ComputeShader": token = TokenKind.ComputeShaderKeyword; return true;
                case "DomainShader": token = TokenKind.DomainShaderKeyword; return true;
                case "HullShader": token = TokenKind.HullShaderKeyword; return true;
                case "pixelfragment": token = TokenKind.PixelfragmentKeyword; return true;
                case "RenderTargetView": token = TokenKind.RenderTargetViewKeyword; return true;
                case "stateblock_state": token = TokenKind.StateblockStateKeyword; return true;
                case "stateblock": token = TokenKind.StateblockKeyword; return true;
                default: token = TokenKind.None; return false;
            }
        }

        public static bool TryConvertToScalarType(TokenKind kind, out ScalarType type)
        {
            switch (kind)
            {
                case TokenKind.VoidKeyword: type = ScalarType.Void; return true;
                case TokenKind.BoolKeyword: type = ScalarType.Bool; return true;
                case TokenKind.IntKeyword: type = ScalarType.Int; return true;
                case TokenKind.UintKeyword: type = ScalarType.Uint; return true;
                case TokenKind.HalfKeyword: type = ScalarType.Half; return true;
                case TokenKind.FloatKeyword: type = ScalarType.Float; return true;
                case TokenKind.DoubleKeyword: type = ScalarType.Double; return true;
                case TokenKind.Min16FloatKeyword: type = ScalarType.Min16Float; return true;
                case TokenKind.Min10FloatKeyword: type = ScalarType.Min10Float; return true;
                case TokenKind.Min16IntKeyword: type = ScalarType.Min16Int; return true;
                case TokenKind.Min12IntKeyword: type = ScalarType.Min12Int; return true;
                case TokenKind.Min16UintKeyword: type = ScalarType.Min16Uint; return true;
                case TokenKind.StringKeyword: type = ScalarType.String; return true;
                default: type = ScalarType.Void; return false;
            }
        }

        public static bool IsBuiltinType(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.BoolKeyword:
                case TokenKind.IntKeyword:
                case TokenKind.UnsignedKeyword:
                case TokenKind.DwordKeyword:
                case TokenKind.UintKeyword:
                case TokenKind.HalfKeyword:
                case TokenKind.FloatKeyword:
                case TokenKind.DoubleKeyword:
                case TokenKind.Min16FloatKeyword:
                case TokenKind.Min10FloatKeyword:
                case TokenKind.Min16IntKeyword:
                case TokenKind.Min12IntKeyword:
                case TokenKind.Min16UintKeyword:
                case TokenKind.VoidKeyword:
                case TokenKind.StringKeyword:
                case TokenKind.SNormKeyword:
                case TokenKind.UNormKeyword:

                case TokenKind.VectorKeyword:
                case TokenKind.Bool1Keyword:
                case TokenKind.Bool2Keyword:
                case TokenKind.Bool3Keyword:
                case TokenKind.Bool4Keyword:
                case TokenKind.Half1Keyword:
                case TokenKind.Half2Keyword:
                case TokenKind.Half3Keyword:
                case TokenKind.Half4Keyword:
                case TokenKind.Int1Keyword:
                case TokenKind.Int2Keyword:
                case TokenKind.Int3Keyword:
                case TokenKind.Int4Keyword:
                case TokenKind.Uint1Keyword:
                case TokenKind.Uint2Keyword:
                case TokenKind.Uint3Keyword:
                case TokenKind.Uint4Keyword:
                case TokenKind.Float1Keyword:
                case TokenKind.Float2Keyword:
                case TokenKind.Float3Keyword:
                case TokenKind.Float4Keyword:
                case TokenKind.Double1Keyword:
                case TokenKind.Double2Keyword:
                case TokenKind.Double3Keyword:
                case TokenKind.Double4Keyword:
                case TokenKind.Min16Float1Keyword:
                case TokenKind.Min16Float2Keyword:
                case TokenKind.Min16Float3Keyword:
                case TokenKind.Min16Float4Keyword:
                case TokenKind.Min10Float1Keyword:
                case TokenKind.Min10Float2Keyword:
                case TokenKind.Min10Float3Keyword:
                case TokenKind.Min10Float4Keyword:
                case TokenKind.Min16Int1Keyword:
                case TokenKind.Min16Int2Keyword:
                case TokenKind.Min16Int3Keyword:
                case TokenKind.Min16Int4Keyword:
                case TokenKind.Min12Int1Keyword:
                case TokenKind.Min12Int2Keyword:
                case TokenKind.Min12Int3Keyword:
                case TokenKind.Min12Int4Keyword:
                case TokenKind.Min16Uint1Keyword:
                case TokenKind.Min16Uint2Keyword:
                case TokenKind.Min16Uint3Keyword:
                case TokenKind.Min16Uint4Keyword:
                //case TokenKind.SNormKeyword:
                //case TokenKind.UNormKeyword:

                case TokenKind.MatrixKeyword:
                case TokenKind.Bool1x1Keyword:
                case TokenKind.Bool1x2Keyword:
                case TokenKind.Bool1x3Keyword:
                case TokenKind.Bool1x4Keyword:
                case TokenKind.Bool2x1Keyword:
                case TokenKind.Bool2x2Keyword:
                case TokenKind.Bool2x3Keyword:
                case TokenKind.Bool2x4Keyword:
                case TokenKind.Bool3x1Keyword:
                case TokenKind.Bool3x2Keyword:
                case TokenKind.Bool3x3Keyword:
                case TokenKind.Bool3x4Keyword:
                case TokenKind.Bool4x1Keyword:
                case TokenKind.Bool4x2Keyword:
                case TokenKind.Bool4x3Keyword:
                case TokenKind.Bool4x4Keyword:
                case TokenKind.Double1x1Keyword:
                case TokenKind.Double1x2Keyword:
                case TokenKind.Double1x3Keyword:
                case TokenKind.Double1x4Keyword:
                case TokenKind.Double2x1Keyword:
                case TokenKind.Double2x2Keyword:
                case TokenKind.Double2x3Keyword:
                case TokenKind.Double2x4Keyword:
                case TokenKind.Double3x1Keyword:
                case TokenKind.Double3x2Keyword:
                case TokenKind.Double3x3Keyword:
                case TokenKind.Double3x4Keyword:
                case TokenKind.Double4x1Keyword:
                case TokenKind.Double4x2Keyword:
                case TokenKind.Double4x3Keyword:
                case TokenKind.Double4x4Keyword:
                case TokenKind.Float1x1Keyword:
                case TokenKind.Float1x2Keyword:
                case TokenKind.Float1x3Keyword:
                case TokenKind.Float1x4Keyword:
                case TokenKind.Float2x1Keyword:
                case TokenKind.Float2x2Keyword:
                case TokenKind.Float2x3Keyword:
                case TokenKind.Float2x4Keyword:
                case TokenKind.Float3x1Keyword:
                case TokenKind.Float3x2Keyword:
                case TokenKind.Float3x3Keyword:
                case TokenKind.Float3x4Keyword:
                case TokenKind.Float4x1Keyword:
                case TokenKind.Float4x2Keyword:
                case TokenKind.Float4x3Keyword:
                case TokenKind.Float4x4Keyword:
                case TokenKind.Half1x1Keyword:
                case TokenKind.Half1x2Keyword:
                case TokenKind.Half1x3Keyword:
                case TokenKind.Half1x4Keyword:
                case TokenKind.Half2x1Keyword:
                case TokenKind.Half2x2Keyword:
                case TokenKind.Half2x3Keyword:
                case TokenKind.Half2x4Keyword:
                case TokenKind.Half3x1Keyword:
                case TokenKind.Half3x2Keyword:
                case TokenKind.Half3x3Keyword:
                case TokenKind.Half3x4Keyword:
                case TokenKind.Half4x1Keyword:
                case TokenKind.Half4x2Keyword:
                case TokenKind.Half4x3Keyword:
                case TokenKind.Half4x4Keyword:
                case TokenKind.Int1x1Keyword:
                case TokenKind.Int1x2Keyword:
                case TokenKind.Int1x3Keyword:
                case TokenKind.Int1x4Keyword:
                case TokenKind.Int2x1Keyword:
                case TokenKind.Int2x2Keyword:
                case TokenKind.Int2x3Keyword:
                case TokenKind.Int2x4Keyword:
                case TokenKind.Int3x1Keyword:
                case TokenKind.Int3x2Keyword:
                case TokenKind.Int3x3Keyword:
                case TokenKind.Int3x4Keyword:
                case TokenKind.Int4x1Keyword:
                case TokenKind.Int4x2Keyword:
                case TokenKind.Int4x3Keyword:
                case TokenKind.Int4x4Keyword:
                case TokenKind.Min10Float1x1Keyword:
                case TokenKind.Min10Float1x2Keyword:
                case TokenKind.Min10Float1x3Keyword:
                case TokenKind.Min10Float1x4Keyword:
                case TokenKind.Min10Float2x1Keyword:
                case TokenKind.Min10Float2x2Keyword:
                case TokenKind.Min10Float2x3Keyword:
                case TokenKind.Min10Float2x4Keyword:
                case TokenKind.Min10Float3x1Keyword:
                case TokenKind.Min10Float3x2Keyword:
                case TokenKind.Min10Float3x3Keyword:
                case TokenKind.Min10Float3x4Keyword:
                case TokenKind.Min10Float4x1Keyword:
                case TokenKind.Min10Float4x2Keyword:
                case TokenKind.Min10Float4x3Keyword:
                case TokenKind.Min10Float4x4Keyword:
                case TokenKind.Min12Int1x1Keyword:
                case TokenKind.Min12Int1x2Keyword:
                case TokenKind.Min12Int1x3Keyword:
                case TokenKind.Min12Int1x4Keyword:
                case TokenKind.Min12Int2x1Keyword:
                case TokenKind.Min12Int2x2Keyword:
                case TokenKind.Min12Int2x3Keyword:
                case TokenKind.Min12Int2x4Keyword:
                case TokenKind.Min12Int3x1Keyword:
                case TokenKind.Min12Int3x2Keyword:
                case TokenKind.Min12Int3x3Keyword:
                case TokenKind.Min12Int3x4Keyword:
                case TokenKind.Min12Int4x1Keyword:
                case TokenKind.Min12Int4x2Keyword:
                case TokenKind.Min12Int4x3Keyword:
                case TokenKind.Min12Int4x4Keyword:
                case TokenKind.Min16Float1x1Keyword:
                case TokenKind.Min16Float1x2Keyword:
                case TokenKind.Min16Float1x3Keyword:
                case TokenKind.Min16Float1x4Keyword:
                case TokenKind.Min16Float2x1Keyword:
                case TokenKind.Min16Float2x2Keyword:
                case TokenKind.Min16Float2x3Keyword:
                case TokenKind.Min16Float2x4Keyword:
                case TokenKind.Min16Float3x1Keyword:
                case TokenKind.Min16Float3x2Keyword:
                case TokenKind.Min16Float3x3Keyword:
                case TokenKind.Min16Float3x4Keyword:
                case TokenKind.Min16Float4x1Keyword:
                case TokenKind.Min16Float4x2Keyword:
                case TokenKind.Min16Float4x3Keyword:
                case TokenKind.Min16Float4x4Keyword:
                case TokenKind.Min16Int1x1Keyword:
                case TokenKind.Min16Int1x2Keyword:
                case TokenKind.Min16Int1x3Keyword:
                case TokenKind.Min16Int1x4Keyword:
                case TokenKind.Min16Int2x1Keyword:
                case TokenKind.Min16Int2x2Keyword:
                case TokenKind.Min16Int2x3Keyword:
                case TokenKind.Min16Int2x4Keyword:
                case TokenKind.Min16Int3x1Keyword:
                case TokenKind.Min16Int3x2Keyword:
                case TokenKind.Min16Int3x3Keyword:
                case TokenKind.Min16Int3x4Keyword:
                case TokenKind.Min16Int4x1Keyword:
                case TokenKind.Min16Int4x2Keyword:
                case TokenKind.Min16Int4x3Keyword:
                case TokenKind.Min16Int4x4Keyword:
                case TokenKind.Min16Uint1x1Keyword:
                case TokenKind.Min16Uint1x2Keyword:
                case TokenKind.Min16Uint1x3Keyword:
                case TokenKind.Min16Uint1x4Keyword:
                case TokenKind.Min16Uint2x1Keyword:
                case TokenKind.Min16Uint2x2Keyword:
                case TokenKind.Min16Uint2x3Keyword:
                case TokenKind.Min16Uint2x4Keyword:
                case TokenKind.Min16Uint3x1Keyword:
                case TokenKind.Min16Uint3x2Keyword:
                case TokenKind.Min16Uint3x3Keyword:
                case TokenKind.Min16Uint3x4Keyword:
                case TokenKind.Min16Uint4x1Keyword:
                case TokenKind.Min16Uint4x2Keyword:
                case TokenKind.Min16Uint4x3Keyword:
                case TokenKind.Min16Uint4x4Keyword:
                case TokenKind.Uint1x1Keyword:
                case TokenKind.Uint1x2Keyword:
                case TokenKind.Uint1x3Keyword:
                case TokenKind.Uint1x4Keyword:
                case TokenKind.Uint2x1Keyword:
                case TokenKind.Uint2x2Keyword:
                case TokenKind.Uint2x3Keyword:
                case TokenKind.Uint2x4Keyword:
                case TokenKind.Uint3x1Keyword:
                case TokenKind.Uint3x2Keyword:
                case TokenKind.Uint3x3Keyword:
                case TokenKind.Uint3x4Keyword:
                case TokenKind.Uint4x1Keyword:
                case TokenKind.Uint4x2Keyword:
                case TokenKind.Uint4x3Keyword:
                case TokenKind.Uint4x4Keyword:
                //case TokenKind.SNormKeyword:
                //case TokenKind.UNormKeyword:

                case TokenKind.AppendStructuredBufferKeyword:
                case TokenKind.BlendStateKeyword:
                case TokenKind.BufferKeyword:
                case TokenKind.ByteAddressBufferKeyword:
                case TokenKind.ConsumeStructuredBufferKeyword:
                case TokenKind.DepthStencilStateKeyword:
                case TokenKind.InputPatchKeyword:
                case TokenKind.LineStreamKeyword:
                case TokenKind.OutputPatchKeyword:
                case TokenKind.PointStreamKeyword:
                case TokenKind.RasterizerOrderedBufferKeyword:
                case TokenKind.RasterizerOrderedByteAddressBufferKeyword:
                case TokenKind.RasterizerOrderedStructuredBufferKeyword:
                case TokenKind.RasterizerOrderedTexture1DKeyword:
                case TokenKind.RasterizerOrderedTexture1DArrayKeyword:
                case TokenKind.RasterizerOrderedTexture2DKeyword:
                case TokenKind.RasterizerOrderedTexture2DArrayKeyword:
                case TokenKind.RasterizerOrderedTexture3DKeyword:
                case TokenKind.RasterizerStateKeyword:
                case TokenKind.RWBufferKeyword:
                case TokenKind.RWByteAddressBufferKeyword:
                case TokenKind.RWStructuredBufferKeyword:
                case TokenKind.RWTexture1DKeyword:
                case TokenKind.RWTexture1DArrayKeyword:
                case TokenKind.RWTexture2DKeyword:
                case TokenKind.RWTexture2DArrayKeyword:
                case TokenKind.RWTexture3DKeyword:
                case TokenKind.SamplerKeyword:
                case TokenKind.Sampler1DKeyword:
                case TokenKind.Sampler2DKeyword:
                case TokenKind.Sampler3DKeyword:
                case TokenKind.SamplerCubeKeyword:
                case TokenKind.SamplerStateKeyword:
                case TokenKind.SamplerComparisonStateKeyword:
                case TokenKind.StructuredBufferKeyword:
                case TokenKind.Texture2DLegacyKeyword:
                case TokenKind.TextureCubeLegacyKeyword:
                case TokenKind.Texture1DKeyword:
                case TokenKind.Texture1DArrayKeyword:
                case TokenKind.Texture2DKeyword:
                case TokenKind.Texture2DArrayKeyword:
                case TokenKind.Texture2DMSKeyword:
                case TokenKind.Texture2DMSArrayKeyword:
                case TokenKind.Texture3DKeyword:
                case TokenKind.TextureCubeKeyword:
                case TokenKind.TextureCubeArrayKeyword:
                case TokenKind.TriangleStreamKeyword:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsModifier(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.ConstKeyword:
                case TokenKind.RowMajorKeyword:
                case TokenKind.ColumnMajorKeyword:
                    return true;

                case TokenKind.ExportKeyword:
                case TokenKind.ExternKeyword:
                case TokenKind.InlineKeyword:
                case TokenKind.PreciseKeyword:
                case TokenKind.SharedKeyword:
                case TokenKind.GloballycoherentKeyword:
                case TokenKind.GroupsharedKeyword:
                case TokenKind.StaticKeyword:
                case TokenKind.UniformKeyword:
                case TokenKind.VolatileKeyword:
                    return true;

                case TokenKind.SNormKeyword:
                case TokenKind.UNormKeyword:
                    return true;

                case TokenKind.LinearKeyword:
                case TokenKind.CentroidKeyword:
                case TokenKind.NointerpolationKeyword:
                case TokenKind.NoperspectiveKeyword:
                    return true;

                default:
                    return false;
            }
        }
    }
}
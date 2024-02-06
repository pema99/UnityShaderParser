using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityShaderParser.Common;

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
                case "min12uint": token = TokenKind.Min12UintKeyword; return true;
                case "min12uint1": token = TokenKind.Min12Uint1Keyword; return true;
                case "min12uint2": token = TokenKind.Min12Uint2Keyword; return true;
                case "min12uint3": token = TokenKind.Min12Uint3Keyword; return true;
                case "min12uint4": token = TokenKind.Min12Uint4Keyword; return true;
                case "min12uint1x1": token = TokenKind.Min12Uint1x1Keyword; return true;
                case "min12uint1x2": token = TokenKind.Min12Uint1x2Keyword; return true;
                case "min12uint1x3": token = TokenKind.Min12Uint1x3Keyword; return true;
                case "min12uint1x4": token = TokenKind.Min12Uint1x4Keyword; return true;
                case "min12uint2x1": token = TokenKind.Min12Uint2x1Keyword; return true;
                case "min12uint2x2": token = TokenKind.Min12Uint2x2Keyword; return true;
                case "min12uint2x3": token = TokenKind.Min12Uint2x3Keyword; return true;
                case "min12uint2x4": token = TokenKind.Min12Uint2x4Keyword; return true;
                case "min12uint3x1": token = TokenKind.Min12Uint3x1Keyword; return true;
                case "min12uint3x2": token = TokenKind.Min12Uint3x2Keyword; return true;
                case "min12uint3x3": token = TokenKind.Min12Uint3x3Keyword; return true;
                case "min12uint3x4": token = TokenKind.Min12Uint3x4Keyword; return true;
                case "min12uint4x1": token = TokenKind.Min12Uint4x1Keyword; return true;
                case "min12uint4x2": token = TokenKind.Min12Uint4x2Keyword; return true;
                case "min12uint4x3": token = TokenKind.Min12Uint4x3Keyword; return true;
                case "min12uint4x4": token = TokenKind.Min12Uint4x4Keyword; return true;
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
                case "sampler": token = TokenKind.SamplerKeyword; return true;
                case "sampler1d": token = TokenKind.Sampler1DKeyword; return true;
                case "sampler2d": token = TokenKind.Sampler2DKeyword; return true;
                case "sampler3d": token = TokenKind.Sampler3DKeyword; return true;
                case "samplercube": token = TokenKind.SamplerCubeKeyword; return true;
                case "SamplerComparisonState": token = TokenKind.SamplerComparisonStateKeyword; return true;
                case "SamplerState": token = TokenKind.SamplerStateKeyword; return true;
                case "sampler_state": token = TokenKind.SamplerStateLegacyKeyword; return true;
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
                case TokenKind.Min12UintKeyword: type = ScalarType.Min12Uint; return true;
                case TokenKind.StringKeyword: type = ScalarType.String; return true;
                default: type = ScalarType.Void; return false;
            }
        }

        public static bool TryConvertToMonomorphicVectorType(TokenKind kind, out ScalarType type, out int dimension)
        {
            switch (kind)
            {
                case TokenKind.Bool1Keyword: type = ScalarType.Bool; dimension = 1; return true;
                case TokenKind.Bool2Keyword: type = ScalarType.Bool; dimension = 2; return true;
                case TokenKind.Bool3Keyword: type = ScalarType.Bool; dimension = 3; return true;
                case TokenKind.Bool4Keyword: type = ScalarType.Bool; dimension = 4; return true;
                case TokenKind.Half1Keyword: type = ScalarType.Half; dimension = 1; return true;
                case TokenKind.Half2Keyword: type = ScalarType.Half; dimension = 2; return true;
                case TokenKind.Half3Keyword: type = ScalarType.Half; dimension = 3; return true;
                case TokenKind.Half4Keyword: type = ScalarType.Half; dimension = 4; return true;
                case TokenKind.Int1Keyword: type = ScalarType.Int; dimension = 1; return true;
                case TokenKind.Int2Keyword: type = ScalarType.Int; dimension = 2; return true;
                case TokenKind.Int3Keyword: type = ScalarType.Int; dimension = 3; return true;
                case TokenKind.Int4Keyword: type = ScalarType.Int; dimension = 4; return true;
                case TokenKind.Uint1Keyword: type = ScalarType.Uint; dimension = 1; return true;
                case TokenKind.Uint2Keyword: type = ScalarType.Uint; dimension = 2; return true;
                case TokenKind.Uint3Keyword: type = ScalarType.Uint; dimension = 3; return true;
                case TokenKind.Uint4Keyword: type = ScalarType.Uint; dimension = 4; return true;
                case TokenKind.Float1Keyword: type = ScalarType.Float; dimension = 1; return true;
                case TokenKind.Float2Keyword: type = ScalarType.Float; dimension = 2; return true;
                case TokenKind.Float3Keyword: type = ScalarType.Float; dimension = 3; return true;
                case TokenKind.Float4Keyword: type = ScalarType.Float; dimension = 4; return true;
                case TokenKind.Double1Keyword: type = ScalarType.Double; dimension = 1; return true;
                case TokenKind.Double2Keyword: type = ScalarType.Double; dimension = 2; return true;
                case TokenKind.Double3Keyword: type = ScalarType.Double; dimension = 3; return true;
                case TokenKind.Double4Keyword: type = ScalarType.Double; dimension = 4; return true;
                case TokenKind.Min16Float1Keyword: type = ScalarType.Min16Float; dimension = 1; return true;
                case TokenKind.Min16Float2Keyword: type = ScalarType.Min16Float; dimension = 2; return true;
                case TokenKind.Min16Float3Keyword: type = ScalarType.Min16Float; dimension = 3; return true;
                case TokenKind.Min16Float4Keyword: type = ScalarType.Min16Float; dimension = 4; return true;
                case TokenKind.Min10Float1Keyword: type = ScalarType.Min10Float; dimension = 1; return true;
                case TokenKind.Min10Float2Keyword: type = ScalarType.Min10Float; dimension = 2; return true;
                case TokenKind.Min10Float3Keyword: type = ScalarType.Min10Float; dimension = 3; return true;
                case TokenKind.Min10Float4Keyword: type = ScalarType.Min10Float; dimension = 4; return true;
                case TokenKind.Min16Int1Keyword: type = ScalarType.Min16Int; dimension = 1; return true;
                case TokenKind.Min16Int2Keyword: type = ScalarType.Min16Int; dimension = 2; return true;
                case TokenKind.Min16Int3Keyword: type = ScalarType.Min16Int; dimension = 3; return true;
                case TokenKind.Min16Int4Keyword: type = ScalarType.Min16Int; dimension = 4; return true;
                case TokenKind.Min12Int1Keyword: type = ScalarType.Min12Int; dimension = 1; return true;
                case TokenKind.Min12Int2Keyword: type = ScalarType.Min12Int; dimension = 2; return true;
                case TokenKind.Min12Int3Keyword: type = ScalarType.Min12Int; dimension = 3; return true;
                case TokenKind.Min12Int4Keyword: type = ScalarType.Min12Int; dimension = 4; return true;
                case TokenKind.Min16Uint1Keyword: type = ScalarType.Min16Uint; dimension = 1; return true;
                case TokenKind.Min16Uint2Keyword: type = ScalarType.Min16Uint; dimension = 2; return true;
                case TokenKind.Min16Uint3Keyword: type = ScalarType.Min16Uint; dimension = 3; return true;
                case TokenKind.Min16Uint4Keyword: type = ScalarType.Min16Uint; dimension = 4; return true;
                case TokenKind.Min12Uint1Keyword: type = ScalarType.Min12Uint; dimension = 1; return true;
                case TokenKind.Min12Uint2Keyword: type = ScalarType.Min12Uint; dimension = 2; return true;
                case TokenKind.Min12Uint3Keyword: type = ScalarType.Min12Uint; dimension = 3; return true;
                case TokenKind.Min12Uint4Keyword: type = ScalarType.Min12Uint; dimension = 4; return true;
                case TokenKind.VectorKeyword: type = ScalarType.Float; dimension = 4; return true;
                default: type = default; dimension = 0; return false;
            }
        }

        public static bool TryConvertToPredefinedObjectType(Token<TokenKind> token, out PredefinedObjectType type)
        {
            switch (token.Kind)
            {
                case TokenKind.AppendStructuredBufferKeyword: type = PredefinedObjectType.AppendStructuredBuffer; return true;
                case TokenKind.BlendStateKeyword: type = PredefinedObjectType.BlendState; return true;
                case TokenKind.BufferKeyword: type = PredefinedObjectType.Buffer; return true;
                case TokenKind.ByteAddressBufferKeyword: type = PredefinedObjectType.ByteAddressBuffer; return true;
                case TokenKind.ConsumeStructuredBufferKeyword: type = PredefinedObjectType.ConsumeStructuredBuffer; return true;
                case TokenKind.DepthStencilStateKeyword: type = PredefinedObjectType.DepthStencilState; return true;
                case TokenKind.InputPatchKeyword: type = PredefinedObjectType.InputPatch; return true;
                case TokenKind.LineStreamKeyword: type = PredefinedObjectType.LineStream; return true;
                case TokenKind.OutputPatchKeyword: type = PredefinedObjectType.OutputPatch; return true;
                case TokenKind.PointStreamKeyword: type = PredefinedObjectType.PointStream; return true;
                case TokenKind.RasterizerStateKeyword: type = PredefinedObjectType.RasterizerState; return true;
                case TokenKind.RWBufferKeyword: type = PredefinedObjectType.RWBuffer; return true;
                case TokenKind.RWByteAddressBufferKeyword: type = PredefinedObjectType.RWByteAddressBuffer; return true;
                case TokenKind.RWStructuredBufferKeyword: type = PredefinedObjectType.RWStructuredBuffer; return true;
                case TokenKind.RWTexture1DKeyword: type = PredefinedObjectType.RWTexture1D; return true;
                case TokenKind.RWTexture1DArrayKeyword: type = PredefinedObjectType.RWTexture1DArray; return true;
                case TokenKind.RWTexture2DKeyword: type = PredefinedObjectType.RWTexture2D; return true;
                case TokenKind.RWTexture2DArrayKeyword: type = PredefinedObjectType.RWTexture2DArray; return true;
                case TokenKind.RWTexture3DKeyword: type = PredefinedObjectType.RWTexture3D; return true;
                case TokenKind.Sampler1DKeyword: type = PredefinedObjectType.Sampler1D; return true;
                case TokenKind.SamplerKeyword: type = PredefinedObjectType.Sampler; return true;
                case TokenKind.Sampler2DKeyword: type = PredefinedObjectType.Sampler2D; return true;
                case TokenKind.Sampler3DKeyword: type = PredefinedObjectType.Sampler3D; return true;
                case TokenKind.SamplerCubeKeyword: type = PredefinedObjectType.SamplerCube; return true;
                case TokenKind.SamplerStateKeyword: type = PredefinedObjectType.SamplerState; return true;
                case TokenKind.SamplerComparisonStateKeyword: type = PredefinedObjectType.SamplerComparisonState; return true;
                case TokenKind.StructuredBufferKeyword: type = PredefinedObjectType.StructuredBuffer; return true;
                case TokenKind.TextureKeyword: type = PredefinedObjectType.Texture; return true;
                case TokenKind.Texture2DLegacyKeyword: type = PredefinedObjectType.Texture; return true;
                case TokenKind.TextureCubeLegacyKeyword: type = PredefinedObjectType.Texture; return true;
                case TokenKind.Texture1DKeyword: type = PredefinedObjectType.Texture1D; return true;
                case TokenKind.Texture1DArrayKeyword: type = PredefinedObjectType.Texture1DArray; return true;
                case TokenKind.Texture2DKeyword: type = PredefinedObjectType.Texture2D; return true;
                case TokenKind.Texture2DArrayKeyword: type = PredefinedObjectType.Texture2DArray; return true;
                case TokenKind.Texture2DMSKeyword: type = PredefinedObjectType.Texture2DMS; return true;
                case TokenKind.Texture2DMSArrayKeyword: type = PredefinedObjectType.Texture2DMSArray; return true;
                case TokenKind.Texture3DKeyword: type = PredefinedObjectType.Texture3D; return true;
                case TokenKind.TextureCubeKeyword: type = PredefinedObjectType.TextureCube; return true;
                case TokenKind.TextureCubeArrayKeyword: type = PredefinedObjectType.TextureCubeArray; return true;
                case TokenKind.TriangleStreamKeyword: type = PredefinedObjectType.TriangleStream; return true;
                case TokenKind.RasterizerOrderedBufferKeyword: type = PredefinedObjectType.RasterizerOrderedBuffer; return true;
                case TokenKind.RasterizerOrderedByteAddressBufferKeyword: type = PredefinedObjectType.RasterizerOrderedByteAddressBuffer; return true;
                case TokenKind.RasterizerOrderedStructuredBufferKeyword: type = PredefinedObjectType.RasterizerOrderedStructuredBuffer; return true;
                case TokenKind.RasterizerOrderedTexture1DArrayKeyword: type = PredefinedObjectType.RasterizerOrderedTexture1DArray; return true;
                case TokenKind.RasterizerOrderedTexture1DKeyword: type = PredefinedObjectType.RasterizerOrderedTexture1D; return true;
                case TokenKind.RasterizerOrderedTexture2DArrayKeyword: type = PredefinedObjectType.RasterizerOrderedTexture2DArray; return true;
                case TokenKind.RasterizerOrderedTexture2DKeyword: type = PredefinedObjectType.RasterizerOrderedTexture2D; return true;
                case TokenKind.RasterizerOrderedTexture3DKeyword: type = PredefinedObjectType.RasterizerOrderedTexture3D; return true;
                // Weird edge case of HLSL grammar - 'ConstantBuffer' is not a real keyword, but is allowed as a generic type.
                case TokenKind.IdentifierToken when token.Identifier == "ConstantBuffer": type = PredefinedObjectType.ConstantBuffer; return true;
                default: type = default; return false;
            }
        }

        public static bool TryConvertToMonomorphicMatrixType(TokenKind kind, out ScalarType type, out int dimensionX, out int dimensionY)
        {
            switch (kind)
            {
                case TokenKind.Bool1x1Keyword: type = ScalarType.Bool; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Bool1x2Keyword: type = ScalarType.Bool; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Bool1x3Keyword: type = ScalarType.Bool; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Bool1x4Keyword: type = ScalarType.Bool; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Bool2x1Keyword: type = ScalarType.Bool; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Bool2x2Keyword: type = ScalarType.Bool; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Bool2x3Keyword: type = ScalarType.Bool; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Bool2x4Keyword: type = ScalarType.Bool; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Bool3x1Keyword: type = ScalarType.Bool; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Bool3x2Keyword: type = ScalarType.Bool; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Bool3x3Keyword: type = ScalarType.Bool; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Bool3x4Keyword: type = ScalarType.Bool; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Bool4x1Keyword: type = ScalarType.Bool; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Bool4x2Keyword: type = ScalarType.Bool; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Bool4x3Keyword: type = ScalarType.Bool; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Bool4x4Keyword: type = ScalarType.Bool; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Double1x1Keyword: type = ScalarType.Double; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Double1x2Keyword: type = ScalarType.Double; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Double1x3Keyword: type = ScalarType.Double; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Double1x4Keyword: type = ScalarType.Double; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Double2x1Keyword: type = ScalarType.Double; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Double2x2Keyword: type = ScalarType.Double; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Double2x3Keyword: type = ScalarType.Double; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Double2x4Keyword: type = ScalarType.Double; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Double3x1Keyword: type = ScalarType.Double; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Double3x2Keyword: type = ScalarType.Double; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Double3x3Keyword: type = ScalarType.Double; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Double3x4Keyword: type = ScalarType.Double; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Double4x1Keyword: type = ScalarType.Double; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Double4x2Keyword: type = ScalarType.Double; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Double4x3Keyword: type = ScalarType.Double; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Double4x4Keyword: type = ScalarType.Double; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Float1x1Keyword: type = ScalarType.Float; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Float1x2Keyword: type = ScalarType.Float; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Float1x3Keyword: type = ScalarType.Float; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Float1x4Keyword: type = ScalarType.Float; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Float2x1Keyword: type = ScalarType.Float; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Float2x2Keyword: type = ScalarType.Float; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Float2x3Keyword: type = ScalarType.Float; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Float2x4Keyword: type = ScalarType.Float; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Float3x1Keyword: type = ScalarType.Float; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Float3x2Keyword: type = ScalarType.Float; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Float3x3Keyword: type = ScalarType.Float; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Float3x4Keyword: type = ScalarType.Float; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Float4x1Keyword: type = ScalarType.Float; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Float4x2Keyword: type = ScalarType.Float; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Float4x3Keyword: type = ScalarType.Float; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Float4x4Keyword: type = ScalarType.Float; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Half1x1Keyword: type = ScalarType.Half; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Half1x2Keyword: type = ScalarType.Half; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Half1x3Keyword: type = ScalarType.Half; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Half1x4Keyword: type = ScalarType.Half; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Half2x1Keyword: type = ScalarType.Half; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Half2x2Keyword: type = ScalarType.Half; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Half2x3Keyword: type = ScalarType.Half; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Half2x4Keyword: type = ScalarType.Half; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Half3x1Keyword: type = ScalarType.Half; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Half3x2Keyword: type = ScalarType.Half; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Half3x3Keyword: type = ScalarType.Half; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Half3x4Keyword: type = ScalarType.Half; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Half4x1Keyword: type = ScalarType.Half; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Half4x2Keyword: type = ScalarType.Half; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Half4x3Keyword: type = ScalarType.Half; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Half4x4Keyword: type = ScalarType.Half; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Int1x1Keyword: type = ScalarType.Int; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Int1x2Keyword: type = ScalarType.Int; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Int1x3Keyword: type = ScalarType.Int; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Int1x4Keyword: type = ScalarType.Int; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Int2x1Keyword: type = ScalarType.Int; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Int2x2Keyword: type = ScalarType.Int; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Int2x3Keyword: type = ScalarType.Int; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Int2x4Keyword: type = ScalarType.Int; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Int3x1Keyword: type = ScalarType.Int; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Int3x2Keyword: type = ScalarType.Int; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Int3x3Keyword: type = ScalarType.Int; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Int3x4Keyword: type = ScalarType.Int; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Int4x1Keyword: type = ScalarType.Int; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Int4x2Keyword: type = ScalarType.Int; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Int4x3Keyword: type = ScalarType.Int; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Int4x4Keyword: type = ScalarType.Int; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Min10Float1x1Keyword: type = ScalarType.Min10Float; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Min10Float1x2Keyword: type = ScalarType.Min10Float; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Min10Float1x3Keyword: type = ScalarType.Min10Float; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Min10Float1x4Keyword: type = ScalarType.Min10Float; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Min10Float2x1Keyword: type = ScalarType.Min10Float; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Min10Float2x2Keyword: type = ScalarType.Min10Float; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Min10Float2x3Keyword: type = ScalarType.Min10Float; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Min10Float2x4Keyword: type = ScalarType.Min10Float; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Min10Float3x1Keyword: type = ScalarType.Min10Float; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Min10Float3x2Keyword: type = ScalarType.Min10Float; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Min10Float3x3Keyword: type = ScalarType.Min10Float; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Min10Float3x4Keyword: type = ScalarType.Min10Float; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Min10Float4x1Keyword: type = ScalarType.Min10Float; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Min10Float4x2Keyword: type = ScalarType.Min10Float; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Min10Float4x3Keyword: type = ScalarType.Min10Float; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Min10Float4x4Keyword: type = ScalarType.Min10Float; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Min12Int1x1Keyword: type = ScalarType.Min12Int; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Min12Int1x2Keyword: type = ScalarType.Min12Int; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Min12Int1x3Keyword: type = ScalarType.Min12Int; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Min12Int1x4Keyword: type = ScalarType.Min12Int; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Min12Int2x1Keyword: type = ScalarType.Min12Int; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Min12Int2x2Keyword: type = ScalarType.Min12Int; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Min12Int2x3Keyword: type = ScalarType.Min12Int; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Min12Int2x4Keyword: type = ScalarType.Min12Int; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Min12Int3x1Keyword: type = ScalarType.Min12Int; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Min12Int3x2Keyword: type = ScalarType.Min12Int; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Min12Int3x3Keyword: type = ScalarType.Min12Int; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Min12Int3x4Keyword: type = ScalarType.Min12Int; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Min12Int4x1Keyword: type = ScalarType.Min12Int; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Min12Int4x2Keyword: type = ScalarType.Min12Int; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Min12Int4x3Keyword: type = ScalarType.Min12Int; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Min12Int4x4Keyword: type = ScalarType.Min12Int; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Min16Float1x1Keyword: type = ScalarType.Min16Float; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Min16Float1x2Keyword: type = ScalarType.Min16Float; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Min16Float1x3Keyword: type = ScalarType.Min16Float; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Min16Float1x4Keyword: type = ScalarType.Min16Float; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Min16Float2x1Keyword: type = ScalarType.Min16Float; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Min16Float2x2Keyword: type = ScalarType.Min16Float; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Min16Float2x3Keyword: type = ScalarType.Min16Float; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Min16Float2x4Keyword: type = ScalarType.Min16Float; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Min16Float3x1Keyword: type = ScalarType.Min16Float; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Min16Float3x2Keyword: type = ScalarType.Min16Float; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Min16Float3x3Keyword: type = ScalarType.Min16Float; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Min16Float3x4Keyword: type = ScalarType.Min16Float; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Min16Float4x1Keyword: type = ScalarType.Min16Float; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Min16Float4x2Keyword: type = ScalarType.Min16Float; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Min16Float4x3Keyword: type = ScalarType.Min16Float; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Min16Float4x4Keyword: type = ScalarType.Min16Float; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Min16Int1x1Keyword: type = ScalarType.Min16Int; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Min16Int1x2Keyword: type = ScalarType.Min16Int; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Min16Int1x3Keyword: type = ScalarType.Min16Int; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Min16Int1x4Keyword: type = ScalarType.Min16Int; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Min16Int2x1Keyword: type = ScalarType.Min16Int; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Min16Int2x2Keyword: type = ScalarType.Min16Int; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Min16Int2x3Keyword: type = ScalarType.Min16Int; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Min16Int2x4Keyword: type = ScalarType.Min16Int; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Min16Int3x1Keyword: type = ScalarType.Min16Int; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Min16Int3x2Keyword: type = ScalarType.Min16Int; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Min16Int3x3Keyword: type = ScalarType.Min16Int; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Min16Int3x4Keyword: type = ScalarType.Min16Int; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Min16Int4x1Keyword: type = ScalarType.Min16Int; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Min16Int4x2Keyword: type = ScalarType.Min16Int; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Min16Int4x3Keyword: type = ScalarType.Min16Int; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Min16Int4x4Keyword: type = ScalarType.Min16Int; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Min16Uint1x1Keyword: type = ScalarType.Min16Uint; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Min16Uint1x2Keyword: type = ScalarType.Min16Uint; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Min16Uint1x3Keyword: type = ScalarType.Min16Uint; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Min16Uint1x4Keyword: type = ScalarType.Min16Uint; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Min16Uint2x1Keyword: type = ScalarType.Min16Uint; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Min16Uint2x2Keyword: type = ScalarType.Min16Uint; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Min16Uint2x3Keyword: type = ScalarType.Min16Uint; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Min16Uint2x4Keyword: type = ScalarType.Min16Uint; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Min16Uint3x1Keyword: type = ScalarType.Min16Uint; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Min16Uint3x2Keyword: type = ScalarType.Min16Uint; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Min16Uint3x3Keyword: type = ScalarType.Min16Uint; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Min16Uint3x4Keyword: type = ScalarType.Min16Uint; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Min16Uint4x1Keyword: type = ScalarType.Min16Uint; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Min16Uint4x2Keyword: type = ScalarType.Min16Uint; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Min16Uint4x3Keyword: type = ScalarType.Min16Uint; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Min16Uint4x4Keyword: type = ScalarType.Min16Uint; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Uint1x1Keyword: type = ScalarType.Uint; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Uint1x2Keyword: type = ScalarType.Uint; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Uint1x3Keyword: type = ScalarType.Uint; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Uint1x4Keyword: type = ScalarType.Uint; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Uint2x1Keyword: type = ScalarType.Uint; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Uint2x2Keyword: type = ScalarType.Uint; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Uint2x3Keyword: type = ScalarType.Uint; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Uint2x4Keyword: type = ScalarType.Uint; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Uint3x1Keyword: type = ScalarType.Uint; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Uint3x2Keyword: type = ScalarType.Uint; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Uint3x3Keyword: type = ScalarType.Uint; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Uint3x4Keyword: type = ScalarType.Uint; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Uint4x1Keyword: type = ScalarType.Uint; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Uint4x2Keyword: type = ScalarType.Uint; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Uint4x3Keyword: type = ScalarType.Uint; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Uint4x4Keyword: type = ScalarType.Uint; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.MatrixKeyword: type = ScalarType.Float; dimensionX = 4; dimensionY = 4; return true;
                case TokenKind.Min12Uint1x1Keyword: type = ScalarType.Min12Uint; dimensionX = 1; dimensionY = 1; return true;
                case TokenKind.Min12Uint1x2Keyword: type = ScalarType.Min12Uint; dimensionX = 1; dimensionY = 2; return true;
                case TokenKind.Min12Uint1x3Keyword: type = ScalarType.Min12Uint; dimensionX = 1; dimensionY = 3; return true;
                case TokenKind.Min12Uint1x4Keyword: type = ScalarType.Min12Uint; dimensionX = 1; dimensionY = 4; return true;
                case TokenKind.Min12Uint2x1Keyword: type = ScalarType.Min12Uint; dimensionX = 2; dimensionY = 1; return true;
                case TokenKind.Min12Uint2x2Keyword: type = ScalarType.Min12Uint; dimensionX = 2; dimensionY = 2; return true;
                case TokenKind.Min12Uint2x3Keyword: type = ScalarType.Min12Uint; dimensionX = 2; dimensionY = 3; return true;
                case TokenKind.Min12Uint2x4Keyword: type = ScalarType.Min12Uint; dimensionX = 2; dimensionY = 4; return true;
                case TokenKind.Min12Uint3x1Keyword: type = ScalarType.Min12Uint; dimensionX = 3; dimensionY = 1; return true;
                case TokenKind.Min12Uint3x2Keyword: type = ScalarType.Min12Uint; dimensionX = 3; dimensionY = 2; return true;
                case TokenKind.Min12Uint3x3Keyword: type = ScalarType.Min12Uint; dimensionX = 3; dimensionY = 3; return true;
                case TokenKind.Min12Uint3x4Keyword: type = ScalarType.Min12Uint; dimensionX = 3; dimensionY = 4; return true;
                case TokenKind.Min12Uint4x1Keyword: type = ScalarType.Min12Uint; dimensionX = 4; dimensionY = 1; return true;
                case TokenKind.Min12Uint4x2Keyword: type = ScalarType.Min12Uint; dimensionX = 4; dimensionY = 2; return true;
                case TokenKind.Min12Uint4x3Keyword: type = ScalarType.Min12Uint; dimensionX = 4; dimensionY = 3; return true;
                case TokenKind.Min12Uint4x4Keyword: type = ScalarType.Min12Uint; dimensionX = 4; dimensionY = 4; return true;
                default: type = default; dimensionX = 0; dimensionY = 0; return false;
            }
        }

        public static bool IsMultiArityNumericConstructor(TokenKind kind)
        {
            switch (kind)
            {
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
                case TokenKind.Min12Uint1Keyword:
                case TokenKind.Min12Uint2Keyword:
                case TokenKind.Min12Uint3Keyword:
                case TokenKind.Min12Uint4Keyword:
                case TokenKind.SNormKeyword:
                case TokenKind.UNormKeyword:

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
                case TokenKind.Min12Uint1x1Keyword:
                case TokenKind.Min12Uint1x2Keyword:
                case TokenKind.Min12Uint1x3Keyword:
                case TokenKind.Min12Uint1x4Keyword:
                case TokenKind.Min12Uint2x1Keyword:
                case TokenKind.Min12Uint2x2Keyword:
                case TokenKind.Min12Uint2x3Keyword:
                case TokenKind.Min12Uint2x4Keyword:
                case TokenKind.Min12Uint3x1Keyword:
                case TokenKind.Min12Uint3x2Keyword:
                case TokenKind.Min12Uint3x3Keyword:
                case TokenKind.Min12Uint3x4Keyword:
                case TokenKind.Min12Uint4x1Keyword:
                case TokenKind.Min12Uint4x2Keyword:
                case TokenKind.Min12Uint4x3Keyword:
                case TokenKind.Min12Uint4x4Keyword:
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
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsSingleArityNumericConstructor(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.BoolKeyword:
                case TokenKind.HalfKeyword:
                case TokenKind.IntKeyword:
                case TokenKind.UintKeyword:
                case TokenKind.FloatKeyword:
                case TokenKind.DoubleKeyword:
                case TokenKind.Min16FloatKeyword:
                case TokenKind.Min16IntKeyword:
                case TokenKind.Min16UintKeyword:
                case TokenKind.StringKeyword:
                    return true;

                default:
                    return false;
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
                case TokenKind.Min12UintKeyword:
                case TokenKind.VoidKeyword:
                case TokenKind.StringKeyword:
                case TokenKind.SNormKeyword:
                case TokenKind.UNormKeyword:

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
                    return IsMultiArityNumericConstructor(kind);
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

        public static bool IsPrefixUnaryToken(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.PlusPlusToken:
                case TokenKind.MinusMinusToken:
                case TokenKind.PlusToken:
                case TokenKind.MinusToken:
                case TokenKind.NotToken:
                case TokenKind.TildeToken:
                    return true;

                default:
                    return false;
            }
        }

        public static bool TryConvertLiteralKind(TokenKind kind, out LiteralKind outKind)
        {
            switch (kind)
            {
                case TokenKind.StringLiteralToken: outKind = LiteralKind.String; return true;
                case TokenKind.FloatLiteralToken: outKind = LiteralKind.Float; return true;
                case TokenKind.IntegerLiteralToken: outKind = LiteralKind.Integer; return true;
                case TokenKind.CharacterLiteralToken: outKind = LiteralKind.Character; return true;
                case TokenKind.TrueKeyword: outKind = LiteralKind.Boolean; return true;
                case TokenKind.FalseKeyword: outKind = LiteralKind.Boolean; return true;
                case TokenKind.NullKeyword: outKind = LiteralKind.Null; return true;
                default: outKind = default; return false;
            }
        }

        public static bool CanTokenComeAfterCast(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.SemiToken:
                case TokenKind.CloseParenToken:
                case TokenKind.CloseBracketToken:
                case TokenKind.OpenBraceToken:
                case TokenKind.CloseBraceToken:
                case TokenKind.CommaToken:
                case TokenKind.EqualsToken:
                case TokenKind.PlusEqualsToken:
                case TokenKind.MinusEqualsToken:
                case TokenKind.AsteriskEqualsToken:
                case TokenKind.SlashEqualsToken:
                case TokenKind.PercentEqualsToken:
                case TokenKind.AmpersandEqualsToken:
                case TokenKind.CaretEqualsToken:
                case TokenKind.BarEqualsToken:
                case TokenKind.LessThanLessThanEqualsToken:
                case TokenKind.GreaterThanGreaterThanEqualsToken:
                case TokenKind.QuestionToken:
                case TokenKind.ColonToken:
                case TokenKind.BarBarToken:
                case TokenKind.AmpersandAmpersandToken:
                case TokenKind.BarToken:
                case TokenKind.CaretToken:
                case TokenKind.AmpersandToken:
                case TokenKind.EqualsEqualsToken:
                case TokenKind.ExclamationEqualsToken:
                case TokenKind.LessThanToken:
                case TokenKind.LessThanEqualsToken:
                case TokenKind.GreaterThanToken:
                case TokenKind.GreaterThanEqualsToken:
                case TokenKind.LessThanLessThanToken:
                case TokenKind.GreaterThanGreaterThanToken:
                case TokenKind.PlusToken:
                case TokenKind.MinusToken:
                case TokenKind.AsteriskToken:
                case TokenKind.SlashToken:
                case TokenKind.PercentToken:
                case TokenKind.PlusPlusToken:
                case TokenKind.MinusMinusToken:
                case TokenKind.OpenBracketToken:
                case TokenKind.DotToken:
                    return false;

                default:
                    return true;
            }
        }

        public static bool TryConvertToDeclarationModifier(Token<TokenKind> token, out BindingModifier modifier)
        {
            switch (token.Kind)
            {
                case TokenKind.ConstKeyword: modifier = BindingModifier.Const; return true;
                case TokenKind.RowMajorKeyword: modifier = BindingModifier.RowMajor; return true;
                case TokenKind.ColumnMajorKeyword: modifier = BindingModifier.ColumnMajor; return true;
                case TokenKind.ExportKeyword: modifier = BindingModifier.Export; return true;
                case TokenKind.ExternKeyword: modifier = BindingModifier.Extern; return true;
                case TokenKind.InlineKeyword: modifier = BindingModifier.Inline; return true;
                case TokenKind.PreciseKeyword: modifier = BindingModifier.Precise; return true;
                case TokenKind.SharedKeyword: modifier = BindingModifier.Shared; return true;
                case TokenKind.GloballycoherentKeyword: modifier = BindingModifier.Globallycoherent; return true;
                case TokenKind.GroupsharedKeyword: modifier = BindingModifier.Groupshared; return true;
                case TokenKind.StaticKeyword: modifier = BindingModifier.Static; return true;
                case TokenKind.UniformKeyword: modifier = BindingModifier.Uniform; return true;
                case TokenKind.VolatileKeyword: modifier = BindingModifier.Volatile; return true;
                case TokenKind.SNormKeyword: modifier = BindingModifier.SNorm; return true;
                case TokenKind.UNormKeyword: modifier = BindingModifier.UNorm; return true;
                case TokenKind.LinearKeyword: modifier = BindingModifier.Linear; return true;
                case TokenKind.CentroidKeyword: modifier = BindingModifier.Centroid; return true;
                case TokenKind.NointerpolationKeyword: modifier = BindingModifier.Nointerpolation; return true;
                case TokenKind.NoperspectiveKeyword: modifier = BindingModifier.Noperspective; return true;
                // Weird edge case of HLSL grammar - 'sample' is not a real keyword.
                case TokenKind.IdentifierToken when token.Identifier == "sample": modifier = BindingModifier.Sample; return true;
                default: modifier = default; return false;
            }
        }

        public static bool TryConvertToParameterModifier(Token<TokenKind> token, out BindingModifier modifier)
        {
            if (TryConvertToDeclarationModifier(token, out modifier))
                return true;

            switch (token.Kind)
            {
                case TokenKind.InKeyword: modifier = BindingModifier.In; return true;
                case TokenKind.OutKeyword: modifier = BindingModifier.Out; return true;
                case TokenKind.InoutKeyword: modifier = BindingModifier.Inout; return true;
                case TokenKind.PointKeyword: modifier = BindingModifier.Point; return true;
                case TokenKind.TriangleKeyword: modifier = BindingModifier.Triangle; return true;
                case TokenKind.TriangleAdjKeyword: modifier = BindingModifier.TriangleAdj; return true;
                case TokenKind.LineKeyword: modifier = BindingModifier.Line; return true;
                case TokenKind.LineAdjKeyword: modifier = BindingModifier.LineAdj; return true;
                default: modifier = default; return false;
            }
        }

        public static ScalarType MakeUnsigned(ScalarType type)
        {
            switch (type)
            {
                case ScalarType.Int: return ScalarType.Uint;
                case ScalarType.Min12Int: return ScalarType.Min12Uint;
                case ScalarType.Min16Int: return ScalarType.Min16Uint;
                default: return type;
            }
        }

        public static bool TryConvertToOperator(TokenKind kind, out OperatorKind op)
        {
            switch (kind)
            {
                case TokenKind.EqualsToken: op =  OperatorKind.Assignment; return true;
                case TokenKind.PlusEqualsToken: op =  OperatorKind.PlusAssignment; return true;
                case TokenKind.MinusEqualsToken: op =  OperatorKind.MinusAssignment; return true;
                case TokenKind.AsteriskEqualsToken: op =  OperatorKind.MulAssignment; return true;
                case TokenKind.SlashEqualsToken: op =  OperatorKind.DivAssignment; return true;
                case TokenKind.PercentEqualsToken: op =  OperatorKind.ModAssignment; return true;
                case TokenKind.LessThanLessThanEqualsToken: op =  OperatorKind.ShiftLeftAssignment; return true;
                case TokenKind.GreaterThanGreaterThanEqualsToken: op =  OperatorKind.ShiftRightAssignment; return true;
                case TokenKind.AmpersandEqualsToken: op =  OperatorKind.BitwiseAndAssignment; return true;
                case TokenKind.CaretEqualsToken: op =  OperatorKind.BitwiseXorAssignment; return true;
                case TokenKind.BarEqualsToken: op =  OperatorKind.BitwiseOrAssignment; return true;
                case TokenKind.BarBarToken: op =  OperatorKind.LogicalOr; return true;
                case TokenKind.AmpersandAmpersandToken: op =  OperatorKind.LogicalAnd; return true;
                case TokenKind.BarToken: op =  OperatorKind.BitwiseOr; return true;
                case TokenKind.AmpersandToken: op =  OperatorKind.BitwiseAnd; return true;
                case TokenKind.CaretToken: op =  OperatorKind.BitwiseXor; return true;
                case TokenKind.CommaToken: op =  OperatorKind.Compound; return true;
                case TokenKind.QuestionToken: op =  OperatorKind.Ternary; return true;
                case TokenKind.EqualsEqualsToken: op =  OperatorKind.Equals; return true;
                case TokenKind.ExclamationEqualsToken: op =  OperatorKind.NotEquals; return true;
                case TokenKind.LessThanToken: op =  OperatorKind.LessThan; return true;
                case TokenKind.LessThanEqualsToken: op =  OperatorKind.LessThanOrEquals; return true;
                case TokenKind.GreaterThanToken: op =  OperatorKind.GreaterThan; return true;
                case TokenKind.GreaterThanEqualsToken: op =  OperatorKind.GreaterThanOrEquals; return true;
                case TokenKind.LessThanLessThanToken: op =  OperatorKind.ShiftLeft; return true;
                case TokenKind.GreaterThanGreaterThanToken: op =  OperatorKind.ShiftRight; return true;
                case TokenKind.PlusToken: op =  OperatorKind.Plus; return true;
                case TokenKind.MinusToken: op =  OperatorKind.Minus; return true;
                case TokenKind.AsteriskToken: op =  OperatorKind.Mul; return true;
                case TokenKind.SlashToken: op =  OperatorKind.Div; return true;
                case TokenKind.PercentToken: op =  OperatorKind.Mod; return true;
                case TokenKind.PlusPlusToken: op = OperatorKind.Increment; return true;
                case TokenKind.MinusMinusToken: op = OperatorKind.Decrement; return true;
                case TokenKind.NotToken: op = OperatorKind.Not; return true;
                case TokenKind.TildeToken: op = OperatorKind.BitFlip; return true;
                default: op = default; 
                    return false;
            }
        }

        public static string IdentifierOrKeywordToString(Token<TokenKind> token)
        {
            if (token.Identifier != null)
                return token.Identifier;

            switch (token.Kind)
            {
                case TokenKind.AppendStructuredBufferKeyword: return "AppendStructuredBuffer";
                case TokenKind.BlendStateKeyword: return "BlendState";
                case TokenKind.BoolKeyword: return "bool";
                case TokenKind.Bool1Keyword: return "bool1";
                case TokenKind.Bool2Keyword: return "bool2";
                case TokenKind.Bool3Keyword: return "bool3";
                case TokenKind.Bool4Keyword: return "bool4";
                case TokenKind.Bool1x1Keyword: return "bool1x1";
                case TokenKind.Bool1x2Keyword: return "bool1x2";
                case TokenKind.Bool1x3Keyword: return "bool1x3";
                case TokenKind.Bool1x4Keyword: return "bool1x4";
                case TokenKind.Bool2x1Keyword: return "bool2x1";
                case TokenKind.Bool2x2Keyword: return "bool2x2";
                case TokenKind.Bool2x3Keyword: return "bool2x3";
                case TokenKind.Bool2x4Keyword: return "bool2x4";
                case TokenKind.Bool3x1Keyword: return "bool3x1";
                case TokenKind.Bool3x2Keyword: return "bool3x2";
                case TokenKind.Bool3x3Keyword: return "bool3x3";
                case TokenKind.Bool3x4Keyword: return "bool3x4";
                case TokenKind.Bool4x1Keyword: return "bool4x1";
                case TokenKind.Bool4x2Keyword: return "bool4x2";
                case TokenKind.Bool4x3Keyword: return "bool4x3";
                case TokenKind.Bool4x4Keyword: return "bool4x4";
                case TokenKind.BufferKeyword: return "Buffer";
                case TokenKind.ByteAddressBufferKeyword: return "ByteAddressBuffer";
                case TokenKind.BreakKeyword: return "break";
                case TokenKind.CaseKeyword: return "case";
                case TokenKind.CBufferKeyword: return "cbuffer";
                case TokenKind.CentroidKeyword: return "centroid";
                case TokenKind.ClassKeyword: return "class";
                case TokenKind.ColumnMajorKeyword: return "column_major";
                case TokenKind.CompileKeyword: return "compile";
                case TokenKind.ConstKeyword: return "const";
                case TokenKind.ConsumeStructuredBufferKeyword: return "ConsumeStructuredBuffer";
                case TokenKind.ContinueKeyword: return "continue";
                case TokenKind.DefaultKeyword: return "default";
                case TokenKind.DefKeyword: return "def";
                case TokenKind.DepthStencilStateKeyword: return "DepthStencilState";
                case TokenKind.DiscardKeyword: return "discard";
                case TokenKind.DoKeyword: return "do";
                case TokenKind.DoubleKeyword: return "double";
                case TokenKind.Double1Keyword: return "double1";
                case TokenKind.Double2Keyword: return "double2";
                case TokenKind.Double3Keyword: return "double3";
                case TokenKind.Double4Keyword: return "double4";
                case TokenKind.Double1x1Keyword: return "double1x1";
                case TokenKind.Double1x2Keyword: return "double1x2";
                case TokenKind.Double1x3Keyword: return "double1x3";
                case TokenKind.Double1x4Keyword: return "double1x4";
                case TokenKind.Double2x1Keyword: return "double2x1";
                case TokenKind.Double2x2Keyword: return "double2x2";
                case TokenKind.Double2x3Keyword: return "double2x3";
                case TokenKind.Double2x4Keyword: return "double2x4";
                case TokenKind.Double3x1Keyword: return "double3x1";
                case TokenKind.Double3x2Keyword: return "double3x2";
                case TokenKind.Double3x3Keyword: return "double3x3";
                case TokenKind.Double3x4Keyword: return "double3x4";
                case TokenKind.Double4x1Keyword: return "double4x1";
                case TokenKind.Double4x2Keyword: return "double4x2";
                case TokenKind.Double4x3Keyword: return "double4x3";
                case TokenKind.Double4x4Keyword: return "double4x4";
                case TokenKind.ElseKeyword: return "else";
                case TokenKind.ErrorKeyword: return "error";
                case TokenKind.ExportKeyword: return "export";
                case TokenKind.ExternKeyword: return "extern";
                case TokenKind.FloatKeyword: return "float";
                case TokenKind.Float1Keyword: return "float1";
                case TokenKind.Float2Keyword: return "float2";
                case TokenKind.Float3Keyword: return "float3";
                case TokenKind.Float4Keyword: return "float4";
                case TokenKind.Float1x1Keyword: return "float1x1";
                case TokenKind.Float1x2Keyword: return "float1x2";
                case TokenKind.Float1x3Keyword: return "float1x3";
                case TokenKind.Float1x4Keyword: return "float1x4";
                case TokenKind.Float2x1Keyword: return "float2x1";
                case TokenKind.Float2x2Keyword: return "float2x2";
                case TokenKind.Float2x3Keyword: return "float2x3";
                case TokenKind.Float2x4Keyword: return "float2x4";
                case TokenKind.Float3x1Keyword: return "float3x1";
                case TokenKind.Float3x2Keyword: return "float3x2";
                case TokenKind.Float3x3Keyword: return "float3x3";
                case TokenKind.Float3x4Keyword: return "float3x4";
                case TokenKind.Float4x1Keyword: return "float4x1";
                case TokenKind.Float4x2Keyword: return "float4x2";
                case TokenKind.Float4x3Keyword: return "float4x3";
                case TokenKind.Float4x4Keyword: return "float4x4";
                case TokenKind.ForKeyword: return "for";
                case TokenKind.GloballycoherentKeyword: return "globallycoherent";
                case TokenKind.GroupsharedKeyword: return "groupshared";
                case TokenKind.HalfKeyword: return "half";
                case TokenKind.Half1Keyword: return "half1";
                case TokenKind.Half2Keyword: return "half2";
                case TokenKind.Half3Keyword: return "half3";
                case TokenKind.Half4Keyword: return "half4";
                case TokenKind.Half1x1Keyword: return "half1x1";
                case TokenKind.Half1x2Keyword: return "half1x2";
                case TokenKind.Half1x3Keyword: return "half1x3";
                case TokenKind.Half1x4Keyword: return "half1x4";
                case TokenKind.Half2x1Keyword: return "half2x1";
                case TokenKind.Half2x2Keyword: return "half2x2";
                case TokenKind.Half2x3Keyword: return "half2x3";
                case TokenKind.Half2x4Keyword: return "half2x4";
                case TokenKind.Half3x1Keyword: return "half3x1";
                case TokenKind.Half3x2Keyword: return "half3x2";
                case TokenKind.Half3x3Keyword: return "half3x3";
                case TokenKind.Half3x4Keyword: return "half3x4";
                case TokenKind.Half4x1Keyword: return "half4x1";
                case TokenKind.Half4x2Keyword: return "half4x2";
                case TokenKind.Half4x3Keyword: return "half4x3";
                case TokenKind.Half4x4Keyword: return "half4x4";
                case TokenKind.IfKeyword: return "if";
                case TokenKind.IndicesKeyword: return "indices";
                case TokenKind.InKeyword: return "in";
                case TokenKind.InlineKeyword: return "inline";
                case TokenKind.InoutKeyword: return "inout";
                case TokenKind.InputPatchKeyword: return "InputPatch";
                case TokenKind.IntKeyword: return "int";
                case TokenKind.Int1Keyword: return "int1";
                case TokenKind.Int2Keyword: return "int2";
                case TokenKind.Int3Keyword: return "int3";
                case TokenKind.Int4Keyword: return "int4";
                case TokenKind.Int1x1Keyword: return "int1x1";
                case TokenKind.Int1x2Keyword: return "int1x2";
                case TokenKind.Int1x3Keyword: return "int1x3";
                case TokenKind.Int1x4Keyword: return "int1x4";
                case TokenKind.Int2x1Keyword: return "int2x1";
                case TokenKind.Int2x2Keyword: return "int2x2";
                case TokenKind.Int2x3Keyword: return "int2x3";
                case TokenKind.Int2x4Keyword: return "int2x4";
                case TokenKind.Int3x1Keyword: return "int3x1";
                case TokenKind.Int3x2Keyword: return "int3x2";
                case TokenKind.Int3x3Keyword: return "int3x3";
                case TokenKind.Int3x4Keyword: return "int3x4";
                case TokenKind.Int4x1Keyword: return "int4x1";
                case TokenKind.Int4x2Keyword: return "int4x2";
                case TokenKind.Int4x3Keyword: return "int4x3";
                case TokenKind.Int4x4Keyword: return "int4x4";
                case TokenKind.InterfaceKeyword: return "interface";
                case TokenKind.LineKeyword: return "line";
                case TokenKind.LineAdjKeyword: return "lineadj";
                case TokenKind.LinearKeyword: return "linear";
                case TokenKind.LineStreamKeyword: return "LineStream";
                case TokenKind.MatrixKeyword: return "matrix";
                case TokenKind.MessageKeyword: return "message";
                case TokenKind.Min10FloatKeyword: return "min10float";
                case TokenKind.Min10Float1Keyword: return "min10float1";
                case TokenKind.Min10Float2Keyword: return "min10float2";
                case TokenKind.Min10Float3Keyword: return "min10float3";
                case TokenKind.Min10Float4Keyword: return "min10float4";
                case TokenKind.Min10Float1x1Keyword: return "min10float1x1";
                case TokenKind.Min10Float1x2Keyword: return "min10float1x2";
                case TokenKind.Min10Float1x3Keyword: return "min10float1x3";
                case TokenKind.Min10Float1x4Keyword: return "min10float1x4";
                case TokenKind.Min10Float2x1Keyword: return "min10float2x1";
                case TokenKind.Min10Float2x2Keyword: return "min10float2x2";
                case TokenKind.Min10Float2x3Keyword: return "min10float2x3";
                case TokenKind.Min10Float2x4Keyword: return "min10float2x4";
                case TokenKind.Min10Float3x1Keyword: return "min10float3x1";
                case TokenKind.Min10Float3x2Keyword: return "min10float3x2";
                case TokenKind.Min10Float3x3Keyword: return "min10float3x3";
                case TokenKind.Min10Float3x4Keyword: return "min10float3x4";
                case TokenKind.Min10Float4x1Keyword: return "min10float4x1";
                case TokenKind.Min10Float4x2Keyword: return "min10float4x2";
                case TokenKind.Min10Float4x3Keyword: return "min10float4x3";
                case TokenKind.Min10Float4x4Keyword: return "min10float4x4";
                case TokenKind.Min12IntKeyword: return "min12int";
                case TokenKind.Min12Int1Keyword: return "min12int1";
                case TokenKind.Min12Int2Keyword: return "min12int2";
                case TokenKind.Min12Int3Keyword: return "min12int3";
                case TokenKind.Min12Int4Keyword: return "min12int4";
                case TokenKind.Min12Int1x1Keyword: return "min12int1x1";
                case TokenKind.Min12Int1x2Keyword: return "min12int1x2";
                case TokenKind.Min12Int1x3Keyword: return "min12int1x3";
                case TokenKind.Min12Int1x4Keyword: return "min12int1x4";
                case TokenKind.Min12Int2x1Keyword: return "min12int2x1";
                case TokenKind.Min12Int2x2Keyword: return "min12int2x2";
                case TokenKind.Min12Int2x3Keyword: return "min12int2x3";
                case TokenKind.Min12Int2x4Keyword: return "min12int2x4";
                case TokenKind.Min12Int3x1Keyword: return "min12int3x1";
                case TokenKind.Min12Int3x2Keyword: return "min12int3x2";
                case TokenKind.Min12Int3x3Keyword: return "min12int3x3";
                case TokenKind.Min12Int3x4Keyword: return "min12int3x4";
                case TokenKind.Min12Int4x1Keyword: return "min12int4x1";
                case TokenKind.Min12Int4x2Keyword: return "min12int4x2";
                case TokenKind.Min12Int4x3Keyword: return "min12int4x3";
                case TokenKind.Min12Int4x4Keyword: return "min12int4x4";
                case TokenKind.Min12UintKeyword: return "min12uint";
                case TokenKind.Min12Uint1Keyword: return "min12uint1";
                case TokenKind.Min12Uint2Keyword: return "min12uint2";
                case TokenKind.Min12Uint3Keyword: return "min12uint3";
                case TokenKind.Min12Uint4Keyword: return "min12uint4";
                case TokenKind.Min12Uint1x1Keyword: return "min12uint1x1";
                case TokenKind.Min12Uint1x2Keyword: return "min12uint1x2";
                case TokenKind.Min12Uint1x3Keyword: return "min12uint1x3";
                case TokenKind.Min12Uint1x4Keyword: return "min12uint1x4";
                case TokenKind.Min12Uint2x1Keyword: return "min12uint2x1";
                case TokenKind.Min12Uint2x2Keyword: return "min12uint2x2";
                case TokenKind.Min12Uint2x3Keyword: return "min12uint2x3";
                case TokenKind.Min12Uint2x4Keyword: return "min12uint2x4";
                case TokenKind.Min12Uint3x1Keyword: return "min12uint3x1";
                case TokenKind.Min12Uint3x2Keyword: return "min12uint3x2";
                case TokenKind.Min12Uint3x3Keyword: return "min12uint3x3";
                case TokenKind.Min12Uint3x4Keyword: return "min12uint3x4";
                case TokenKind.Min12Uint4x1Keyword: return "min12uint4x1";
                case TokenKind.Min12Uint4x2Keyword: return "min12uint4x2";
                case TokenKind.Min12Uint4x3Keyword: return "min12uint4x3";
                case TokenKind.Min12Uint4x4Keyword: return "min12uint4x4";
                case TokenKind.Min16FloatKeyword: return "min16float";
                case TokenKind.Min16Float1Keyword: return "min16float1";
                case TokenKind.Min16Float2Keyword: return "min16float2";
                case TokenKind.Min16Float3Keyword: return "min16float3";
                case TokenKind.Min16Float4Keyword: return "min16float4";
                case TokenKind.Min16Float1x1Keyword: return "min16float1x1";
                case TokenKind.Min16Float1x2Keyword: return "min16float1x2";
                case TokenKind.Min16Float1x3Keyword: return "min16float1x3";
                case TokenKind.Min16Float1x4Keyword: return "min16float1x4";
                case TokenKind.Min16Float2x1Keyword: return "min16float2x1";
                case TokenKind.Min16Float2x2Keyword: return "min16float2x2";
                case TokenKind.Min16Float2x3Keyword: return "min16float2x3";
                case TokenKind.Min16Float2x4Keyword: return "min16float2x4";
                case TokenKind.Min16Float3x1Keyword: return "min16float3x1";
                case TokenKind.Min16Float3x2Keyword: return "min16float3x2";
                case TokenKind.Min16Float3x3Keyword: return "min16float3x3";
                case TokenKind.Min16Float3x4Keyword: return "min16float3x4";
                case TokenKind.Min16Float4x1Keyword: return "min16float4x1";
                case TokenKind.Min16Float4x2Keyword: return "min16float4x2";
                case TokenKind.Min16Float4x3Keyword: return "min16float4x3";
                case TokenKind.Min16Float4x4Keyword: return "min16float4x4";
                case TokenKind.Min16IntKeyword: return "min16int";
                case TokenKind.Min16Int1Keyword: return "min16int1";
                case TokenKind.Min16Int2Keyword: return "min16int2";
                case TokenKind.Min16Int3Keyword: return "min16int3";
                case TokenKind.Min16Int4Keyword: return "min16int4";
                case TokenKind.Min16Int1x1Keyword: return "min16int1x1";
                case TokenKind.Min16Int1x2Keyword: return "min16int1x2";
                case TokenKind.Min16Int1x3Keyword: return "min16int1x3";
                case TokenKind.Min16Int1x4Keyword: return "min16int1x4";
                case TokenKind.Min16Int2x1Keyword: return "min16int2x1";
                case TokenKind.Min16Int2x2Keyword: return "min16int2x2";
                case TokenKind.Min16Int2x3Keyword: return "min16int2x3";
                case TokenKind.Min16Int2x4Keyword: return "min16int2x4";
                case TokenKind.Min16Int3x1Keyword: return "min16int3x1";
                case TokenKind.Min16Int3x2Keyword: return "min16int3x2";
                case TokenKind.Min16Int3x3Keyword: return "min16int3x3";
                case TokenKind.Min16Int3x4Keyword: return "min16int3x4";
                case TokenKind.Min16Int4x1Keyword: return "min16int4x1";
                case TokenKind.Min16Int4x2Keyword: return "min16int4x2";
                case TokenKind.Min16Int4x3Keyword: return "min16int4x3";
                case TokenKind.Min16Int4x4Keyword: return "min16int4x4";
                case TokenKind.Min16UintKeyword: return "min16uint";
                case TokenKind.Min16Uint1Keyword: return "min16uint1";
                case TokenKind.Min16Uint2Keyword: return "min16uint2";
                case TokenKind.Min16Uint3Keyword: return "min16uint3";
                case TokenKind.Min16Uint4Keyword: return "min16uint4";
                case TokenKind.Min16Uint1x1Keyword: return "min16uint1x1";
                case TokenKind.Min16Uint1x2Keyword: return "min16uint1x2";
                case TokenKind.Min16Uint1x3Keyword: return "min16uint1x3";
                case TokenKind.Min16Uint1x4Keyword: return "min16uint1x4";
                case TokenKind.Min16Uint2x1Keyword: return "min16uint2x1";
                case TokenKind.Min16Uint2x2Keyword: return "min16uint2x2";
                case TokenKind.Min16Uint2x3Keyword: return "min16uint2x3";
                case TokenKind.Min16Uint2x4Keyword: return "min16uint2x4";
                case TokenKind.Min16Uint3x1Keyword: return "min16uint3x1";
                case TokenKind.Min16Uint3x2Keyword: return "min16uint3x2";
                case TokenKind.Min16Uint3x3Keyword: return "min16uint3x3";
                case TokenKind.Min16Uint3x4Keyword: return "min16uint3x4";
                case TokenKind.Min16Uint4x1Keyword: return "min16uint4x1";
                case TokenKind.Min16Uint4x2Keyword: return "min16uint4x2";
                case TokenKind.Min16Uint4x3Keyword: return "min16uint4x3";
                case TokenKind.Min16Uint4x4Keyword: return "min16uint4x4";
                case TokenKind.NamespaceKeyword: return "namespace";
                case TokenKind.NointerpolationKeyword: return "nointerpolation";
                case TokenKind.NoperspectiveKeyword: return "noperspective";
                case TokenKind.NullKeyword: return "NULL";
                case TokenKind.OutKeyword: return "out";
                case TokenKind.OutputPatchKeyword: return "OutputPatch";
                case TokenKind.PackMatrixKeyword: return "packmatrix";
                case TokenKind.PackoffsetKeyword: return "packoffset";
                case TokenKind.PassKeyword: return "pass";
                case TokenKind.PayloadKeyword: return "payload";
                case TokenKind.PointKeyword: return "point";
                case TokenKind.PointStreamKeyword: return "PointStream";
                case TokenKind.PragmaKeyword: return "pragma";
                case TokenKind.PreciseKeyword: return "precise";
                case TokenKind.PrimitivesKeyword: return "primitives";
                case TokenKind.RasterizerOrderedBufferKeyword: return "rasterizerorderedbuffer";
                case TokenKind.RasterizerOrderedByteAddressBufferKeyword: return "rasterizerorderedbyteaddressbuffer";
                case TokenKind.RasterizerOrderedStructuredBufferKeyword: return "rasterizerorderedstructuredbuffer";
                case TokenKind.RasterizerOrderedTexture1DKeyword: return "rasterizerorderedtexture1d";
                case TokenKind.RasterizerOrderedTexture1DArrayKeyword: return "rasterizerorderedtexture1darray";
                case TokenKind.RasterizerOrderedTexture2DKeyword: return "rasterizerorderedtexture2d";
                case TokenKind.RasterizerOrderedTexture2DArrayKeyword: return "rasterizerorderedtexture2darray";
                case TokenKind.RasterizerOrderedTexture3DKeyword: return "rasterizerorderedtexture3d";
                case TokenKind.RasterizerStateKeyword: return "RasterizerState";
                case TokenKind.RegisterKeyword: return "register";
                case TokenKind.ReturnKeyword: return "return";
                case TokenKind.RowMajorKeyword: return "row_major";
                case TokenKind.RWBufferKeyword: return "RWBuffer";
                case TokenKind.RWByteAddressBufferKeyword: return "RWByteAddressBuffer";
                case TokenKind.RWStructuredBufferKeyword: return "RWStructuredBuffer";
                case TokenKind.RWTexture1DKeyword: return "RWTexture1D";
                case TokenKind.RWTexture1DArrayKeyword: return "RWTexture1DArray";
                case TokenKind.RWTexture2DKeyword: return "RWTexture2D";
                case TokenKind.RWTexture2DArrayKeyword: return "RWTexture2DArray";
                case TokenKind.RWTexture3DKeyword: return "RWTexture3D";
                case TokenKind.SamplerKeyword: return "sampler";
                case TokenKind.Sampler1DKeyword: return "sampler1d";
                case TokenKind.Sampler2DKeyword: return "sampler2d";
                case TokenKind.Sampler3DKeyword: return "sampler3d";
                case TokenKind.SamplerCubeKeyword: return "samplercube";
                case TokenKind.SamplerComparisonStateKeyword: return "SamplerComparisonState";
                case TokenKind.SamplerStateKeyword: return "SamplerState";
                case TokenKind.SamplerStateLegacyKeyword: return "sampler_state";
                case TokenKind.SharedKeyword: return "shared";
                case TokenKind.SNormKeyword: return "snorm";
                case TokenKind.StaticKeyword: return "static";
                case TokenKind.StringKeyword: return "string";
                case TokenKind.StructKeyword: return "struct";
                case TokenKind.StructuredBufferKeyword: return "StructuredBuffer";
                case TokenKind.SwitchKeyword: return "switch";
                case TokenKind.TBufferKeyword: return "tbuffer";
                case TokenKind.TechniqueKeyword: return "technique";
                case TokenKind.Technique10Keyword: return "technique10";
                case TokenKind.Technique11Keyword: return "technique11";
                case TokenKind.TextureKeyword: return "texture";
                case TokenKind.Texture2DLegacyKeyword: return "Texture2DLegacy";
                case TokenKind.TextureCubeLegacyKeyword: return "TextureCubeLegacy";
                case TokenKind.Texture1DKeyword: return "Texture1D";
                case TokenKind.Texture1DArrayKeyword: return "Texture1DArray";
                case TokenKind.Texture2DKeyword: return "Texture2D";
                case TokenKind.Texture2DArrayKeyword: return "Texture2DArray";
                case TokenKind.Texture2DMSKeyword: return "Texture2DMS";
                case TokenKind.Texture2DMSArrayKeyword: return "Texture2DMSArray";
                case TokenKind.Texture3DKeyword: return "Texture3D";
                case TokenKind.TextureCubeKeyword: return "TextureCube";
                case TokenKind.TextureCubeArrayKeyword: return "TextureCubeArray";
                case TokenKind.TriangleKeyword: return "triangle";
                case TokenKind.TriangleAdjKeyword: return "triangleadj";
                case TokenKind.TriangleStreamKeyword: return "TriangleStream";
                case TokenKind.TypedefKeyword: return "typedef";
                case TokenKind.UniformKeyword: return "uniform";
                case TokenKind.UNormKeyword: return "unorm";
                case TokenKind.UintKeyword: return "uint";
                case TokenKind.Uint1Keyword: return "uint1";
                case TokenKind.Uint2Keyword: return "uint2";
                case TokenKind.Uint3Keyword: return "uint3";
                case TokenKind.Uint4Keyword: return "uint4";
                case TokenKind.Uint1x1Keyword: return "uint1x1";
                case TokenKind.Uint1x2Keyword: return "uint1x2";
                case TokenKind.Uint1x3Keyword: return "uint1x3";
                case TokenKind.Uint1x4Keyword: return "uint1x4";
                case TokenKind.Uint2x1Keyword: return "uint2x1";
                case TokenKind.Uint2x2Keyword: return "uint2x2";
                case TokenKind.Uint2x3Keyword: return "uint2x3";
                case TokenKind.Uint2x4Keyword: return "uint2x4";
                case TokenKind.Uint3x1Keyword: return "uint3x1";
                case TokenKind.Uint3x2Keyword: return "uint3x2";
                case TokenKind.Uint3x3Keyword: return "uint3x3";
                case TokenKind.Uint3x4Keyword: return "uint3x4";
                case TokenKind.Uint4x1Keyword: return "uint4x1";
                case TokenKind.Uint4x2Keyword: return "uint4x2";
                case TokenKind.Uint4x3Keyword: return "uint4x3";
                case TokenKind.Uint4x4Keyword: return "uint4x4";
                case TokenKind.VectorKeyword: return "vector";
                case TokenKind.VerticesKeyword: return "vertices";
                case TokenKind.VolatileKeyword: return "volatile";
                case TokenKind.VoidKeyword: return "void";
                case TokenKind.WarningKeyword: return "warning";
                case TokenKind.WhileKeyword: return "while";
                case TokenKind.TrueKeyword: return "true";
                case TokenKind.FalseKeyword: return "false";
                case TokenKind.UnsignedKeyword: return "unsigned";
                case TokenKind.DwordKeyword: return "dword";
                case TokenKind.CompileFragmentKeyword: return "compile_fragment";
                case TokenKind.DepthStencilViewKeyword: return "DepthStencilView";
                case TokenKind.PixelfragmentKeyword: return "pixelfragment";
                case TokenKind.RenderTargetViewKeyword: return "RenderTargetView";
                case TokenKind.StateblockStateKeyword: return "stateblock_state";
                case TokenKind.StateblockKeyword: return "stateblock";

                default: return "__INVALID";
            }
        }

        public static string TokenToString(Token<TokenKind> token)
        {
            switch (token.Kind)
            {
                case TokenKind.OpenParenToken: return "(";
                case TokenKind.CloseParenToken: return ")";
                case TokenKind.OpenBracketToken: return "[";
                case TokenKind.CloseBracketToken: return "]";
                case TokenKind.OpenBraceToken: return "{";
                case TokenKind.CloseBraceToken: return "}";
                case TokenKind.SemiToken: return ";";
                case TokenKind.CommaToken: return ",";
                case TokenKind.LessThanToken: return "<";
                case TokenKind.LessThanEqualsToken: return "<=";
                case TokenKind.GreaterThanToken: return ">";
                case TokenKind.GreaterThanEqualsToken: return ">=";
                case TokenKind.LessThanLessThanToken: return "<<";
                case TokenKind.GreaterThanGreaterThanToken: return ">>";
                case TokenKind.PlusToken: return "+";
                case TokenKind.PlusPlusToken: return "++";
                case TokenKind.MinusToken: return "-";
                case TokenKind.MinusMinusToken: return "--";
                case TokenKind.AsteriskToken: return "*";
                case TokenKind.SlashToken: return "/";
                case TokenKind.PercentToken: return "%";
                case TokenKind.AmpersandToken: return "&";
                case TokenKind.BarToken: return "|";
                case TokenKind.AmpersandAmpersandToken: return "&&";
                case TokenKind.BarBarToken: return "||";
                case TokenKind.CaretToken: return "^";
                case TokenKind.NotToken: return "!";
                case TokenKind.TildeToken: return "~";
                case TokenKind.QuestionToken: return "?";
                case TokenKind.ColonToken: return ":";
                case TokenKind.ColonColonToken: return "::";
                case TokenKind.EqualsToken: return "=";
                case TokenKind.AsteriskEqualsToken: return "*";
                case TokenKind.SlashEqualsToken: return "/=";
                case TokenKind.PercentEqualsToken: return "%=";
                case TokenKind.PlusEqualsToken: return "+=";
                case TokenKind.MinusEqualsToken: return "-=";
                case TokenKind.LessThanLessThanEqualsToken: return "<<=";
                case TokenKind.GreaterThanGreaterThanEqualsToken: return ">>=";
                case TokenKind.AmpersandEqualsToken: return "&=";
                case TokenKind.CaretEqualsToken: return "^=";
                case TokenKind.BarEqualsToken: return "|=";
                case TokenKind.EqualsEqualsToken: return "==";
                case TokenKind.ExclamationEqualsToken: return "!=";
                case TokenKind.DotToken: return ".";
                case TokenKind.HashToken: return "#";
                case TokenKind.HashHashToken: return "##";

                case TokenKind.DefineDirectiveKeyword: return "#define";
                case TokenKind.IncludeDirectiveKeyword: return "#include";
                case TokenKind.LineDirectiveKeyword: return "#line";
                case TokenKind.UndefDirectiveKeyword: return "#undef";
                case TokenKind.ErrorDirectiveKeyword: return "#error";
                case TokenKind.PragmaDirectiveKeyword: return "#pragma";
                case TokenKind.IfDirectiveKeyword: return "#if";
                case TokenKind.IfdefDirectiveKeyword: return "#ifdef";
                case TokenKind.IfndefDirectiveKeyword: return "#ifndef";
                case TokenKind.ElifDirectiveKeyword: return "#elif";
                case TokenKind.ElseDirectiveKeyword: return "#else";
                case TokenKind.EndifDirectiveKeyword: return "#endif";

                default: return IdentifierOrKeywordToString(token);
            }
        }
    }
}

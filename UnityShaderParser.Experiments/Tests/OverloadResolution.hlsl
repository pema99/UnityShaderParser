//=============================================================================
// BASIC OVERLOADING BY PARAMETER COUNT
//=============================================================================

int Foo() { return 0; }
int Foo(int a) { return 1; }
int Foo(int a, int b) { return 2; }
int Foo(int a, int b, int c) { return 3; }

[Test]
void Overload_ByParameterCount_SelectsCorrectOverload()
{
    ASSERT(Foo() == 0);
    ASSERT(Foo(10) == 1);
    ASSERT(Foo(10, 20) == 2);
    ASSERT(Foo(10, 20, 30) == 3);
}

//=============================================================================
// BASIC OVERLOADING BY PARAMETER TYPE
//=============================================================================

int TypeOverload(int x) { return 1; }
int TypeOverload(float x) { return 2; }
int TypeOverload(bool x) { return 3; }
int TypeOverload(uint x) { return 4; }

[Test]
void Overload_ByParameterType_SelectsCorrectOverload()
{
    int i = 5;
    float f = 5.0f;
    bool b = true;
    uint u = 5u;
    
    ASSERT(TypeOverload(i) == 1);
    ASSERT(TypeOverload(f) == 2);
    ASSERT(TypeOverload(b) == 3);
    ASSERT(TypeOverload(u) == 4);
}

//=============================================================================
// OVERLOADING WITH IMPLICIT CONVERSIONS
//=============================================================================

int ImplicitConv(float x) { return 1; }
int ImplicitConv(double x) { return 2; }

[Test]
void Overload_IntArgument_PrefersFloatOverDouble()
{
    int i = 5;
    ASSERT(ImplicitConv(i) == 1);  // int -> float preferred over int -> double
}

int ImplicitConv2(int x) { return 1; }
int ImplicitConv2(float x) { return 2; }

[Test]
void Overload_IntArgument_PrefersExactMatch()
{
    int i = 5;
    float f = 5.0f;
    ASSERT(ImplicitConv2(i) == 1);  // Exact match
    ASSERT(ImplicitConv2(f) == 2);  // Exact match
}

//=============================================================================
// OVERLOADING WITH VECTOR TYPES
//=============================================================================

int VecOverload(float x) { return 1; }
int VecOverload(float2 x) { return 2; }
int VecOverload(float3 x) { return 3; }
int VecOverload(float4 x) { return 4; }

[Test]
void Overload_VectorTypes_SelectsCorrectDimension()
{
    ASSERT(VecOverload(1.0f) == 1);
    ASSERT(VecOverload(float2(1, 2)) == 2);
    ASSERT(VecOverload(float3(1, 2, 3)) == 3);
    ASSERT(VecOverload(float4(1, 2, 3, 4)) == 4);
}

int VecTypeOverload(int3 x) { return 1; }
int VecTypeOverload(float3 x) { return 2; }
int VecTypeOverload(uint3 x) { return 3; }

[Test]
void Overload_VectorElementTypes_SelectsCorrectType()
{
    ASSERT(VecTypeOverload(int3(1, 2, 3)) == 1);
    ASSERT(VecTypeOverload(float3(1, 2, 3)) == 2);
    ASSERT(VecTypeOverload(uint3(1, 2, 3)) == 3);
}

//=============================================================================
// OVERLOADING WITH MATRIX TYPES
//=============================================================================

int MatOverload(float2x2 x) { return 1; }
int MatOverload(float3x3 x) { return 2; }
int MatOverload(float4x4 x) { return 3; }

[Test]
void Overload_MatrixDimensions_SelectsCorrectSize()
{
    float2x2 m2 = float2x2(1, 0, 0, 1);
    float3x3 m3 = float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
    float4x4 m4 = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
    
    ASSERT(MatOverload(m2) == 1);
    ASSERT(MatOverload(m3) == 2);
    ASSERT(MatOverload(m4) == 3);
}

int MatOverload2(float2x3 x) { return 1; }
int MatOverload2(float3x2 x) { return 2; }

[Test]
void Overload_NonSquareMatrices_DistinguishesDimensions()
{
    float2x3 m23 = (float2x3)0;
    float3x2 m32 = (float3x2)0;
    
    ASSERT(MatOverload2(m23) == 1);
    ASSERT(MatOverload2(m32) == 2);
}

//=============================================================================
// OVERLOADING WITH MIXED SCALAR/VECTOR TYPES
//=============================================================================

int MixedOverload(float x, float y) { return 1; }
int MixedOverload(float2 xy) { return 2; }

[Test]
void Overload_ScalarVsVector_DistinguishesCorrectly()
{
    ASSERT(MixedOverload(1.0f, 2.0f) == 1);
    ASSERT(MixedOverload(float2(1, 2)) == 2);
}

//=============================================================================
// OVERLOADING WITH OUTPUT PARAMETERS
//=============================================================================

int OutParamOverload(int x, out int y) { y = x * 2; return 1; }
int OutParamOverload(int x, out float y) { y = x * 3.0f; return 2; }

[Test]
void Overload_OutParameterTypes_SelectsBasedOnOutType()
{
    int oi;
    float of;
    
    ASSERT(OutParamOverload(5, oi) == 1);
    ASSERT(oi == 10);
    
    ASSERT(OutParamOverload(5, of) == 2);
    ASSERT(of == 15.0f);
}

//=============================================================================
// OVERLOADING WITH INOUT PARAMETERS
//=============================================================================

int InOutOverload(inout int x) { x *= 2; return 1; }
int InOutOverload(inout float x) { x *= 3.0f; return 2; }

[Test]
void Overload_InOutParameterTypes_SelectsBasedOnType()
{
    int i = 5;
    float f = 5.0f;
    
    ASSERT(InOutOverload(i) == 1);
    
    ASSERT(InOutOverload(f) == 2);
}

//=============================================================================
// OVERLOADING WITH ARRAYS
//=============================================================================

int ArrayOverload(int x[2]) { return 1; }
int ArrayOverload(int x[3]) { return 2; }
int ArrayOverload(int x[4]) { return 3; }

[Test]
void Overload_ArraySizes_SelectsCorrectSize()
{
    int a2[2] = { 1, 2 };
    int a3[3] = { 1, 2, 3 };
    int a4[4] = { 1, 2, 3, 4 };
    
    ASSERT(ArrayOverload(a2) == 1);
    ASSERT(ArrayOverload(a3) == 2);
    ASSERT(ArrayOverload(a4) == 3);
}

int ArrayTypeOverload(int x[2]) { return 1; }
int ArrayTypeOverload(float x[2]) { return 2; }

[Test]
void Overload_ArrayElementTypes_SelectsCorrectType()
{
    int ai[2] = { 1, 2 };
    float af[2] = { 1.0f, 2.0f };
    
    ASSERT(ArrayTypeOverload(ai) == 1);
    ASSERT(ArrayTypeOverload(af) == 2);
}

//=============================================================================
// OVERLOADING WITH STRUCTS
//=============================================================================

struct StructA { int x; };
struct StructB { int x; };
struct StructC { float x; };

int StructOverload(StructA s) { return 1; }
int StructOverload(StructB s) { return 2; }
int StructOverload(StructC s) { return 3; }

[Test]
void Overload_StructTypes_DistinguishesByStructType()
{
    StructA a; a.x = 1;
    StructB b; b.x = 1;
    StructC c; c.x = 1.0f;
    
    ASSERT(StructOverload(a) == 1);
    ASSERT(StructOverload(b) == 2);
    ASSERT(StructOverload(c) == 3);
}

//=============================================================================
// OVERLOADING RETURN TYPE DOES NOT AFFECT RESOLUTION
//=============================================================================

// Note: HLSL does not allow overloading by return type alone
// These must have different parameter signatures

int ReturnTest(int x) { return x; }
float ReturnTest(float x) { return x * 2.0f; }

[Test]
void Overload_DifferentReturnTypes_ResolvesByParameters()
{
    int ri = ReturnTest(5);
    float rf = ReturnTest(5.0f);
    
    ASSERT(ri == 5);
    ASSERT(rf == 10.0f);
}

//=============================================================================
// OVERLOADING WITH LITERALS
//=============================================================================

int LiteralOverload(int x) { return 1; }
int LiteralOverload(uint x) { return 2; }
int LiteralOverload(float x) { return 3; }

[Test]
void Overload_IntegerLiteral_PrefersInt()
{
    ASSERT(LiteralOverload(5) == 1);  // Integer literal -> int
}

[Test]
void Overload_UnsignedLiteral_PrefersUint()
{
    ASSERT(LiteralOverload(5u) == 2);  // Unsigned literal -> uint
}

[Test]
void Overload_FloatLiteral_PrefersFloat()
{
    ASSERT(LiteralOverload(5.0f) == 3);  // Float literal -> float
    ASSERT(LiteralOverload(5.0) == 3);   // Double literal -> float (if no double overload)
}

//=============================================================================
// OVERLOADING WITH MULTIPLE PARAMETERS - BEST MATCH
//=============================================================================

int MultiParam(int a, int b) { return 1; }
int MultiParam(float a, float b) { return 2; }
int MultiParam(int a, float b) { return 3; }
int MultiParam(float a, int b) { return 4; }

[Test]
void Overload_MultipleParameters_SelectsBestMatch()
{
    int i = 1;
    float f = 1.0f;
    
    ASSERT(MultiParam(i, i) == 1);
    ASSERT(MultiParam(f, f) == 2);
    ASSERT(MultiParam(i, f) == 3);
    ASSERT(MultiParam(f, i) == 4);
}

//=============================================================================
// OVERLOADING WITH PROMOTION CHAINS
//=============================================================================

// Testing promotion: bool -> int -> uint -> float -> double

int PromotionTest(float x) { return 1; }

[Test]
void Overload_BoolPromotion_PromotesToFloat()
{
    bool b = true;
    ASSERT(PromotionTest(b) == 1);  // bool -> float
}

int PromotionTest2(int x) { return 1; }
int PromotionTest2(float x) { return 2; }

[Test]
void Overload_BoolWithIntAndFloat_PrefersInt()
{
    bool b = true;
    ASSERT(PromotionTest2(b) == 1);  // bool -> int preferred over bool -> float
}

//=============================================================================
// OVERLOADING WITH VECTOR TRUNCATION/EXPANSION (IF SUPPORTED)
//=============================================================================

int VecConversion(float3 x) { return 1; }

[Test]
void Overload_VectorConstruction_AcceptsExactMatch()
{
    float3 v = float3(1, 2, 3);
    ASSERT(VecConversion(v) == 1);
}

//=============================================================================
// OVERLOADING WITH TEXTURE/SAMPLER TYPES
//=============================================================================

Texture2D<float4> tex2D;
Texture3D<float4> tex3D;
TextureCube<float4> texCube;

int TexOverload(Texture2D<float4> t) { return 1; }
int TexOverload(Texture3D<float4> t) { return 2; }
int TexOverload(TextureCube<float4> t) { return 3; }

[Test]
void Overload_TextureTypes_DistinguishesByTextureType()
{
    ASSERT(TexOverload(tex2D) == 1);
    ASSERT(TexOverload(tex3D) == 2);
    ASSERT(TexOverload(texCube) == 3);
}

//=============================================================================
// OVERLOADING WITH BUFFER TYPES
//=============================================================================

StructuredBuffer<int> sbInt;
StructuredBuffer<float> sbFloat;

int BufferOverload(StructuredBuffer<int> b) { return 1; }
int BufferOverload(StructuredBuffer<float> b) { return 2; }

[Test]
void Overload_BufferTypes_DistinguishesByElementType()
{
    ASSERT(BufferOverload(sbInt) == 1);
    ASSERT(BufferOverload(sbFloat) == 2);
}

//=============================================================================
// RECURSIVE OVERLOAD RESOLUTION
//=============================================================================

int RecursiveHelper(int x) { return x * 2; }
float RecursiveHelper(float x) { return x * 3.0f; }

int RecursiveOuter(int x) { return RecursiveHelper(x); }
float RecursiveOuter(float x) { return RecursiveHelper(x); }

[Test]
void Overload_NestedCalls_ResolvesCorrectlyAtEachLevel()
{
    ASSERT(RecursiveOuter(5) == 10);      // int -> int -> *2
    ASSERT(RecursiveOuter(5.0f) == 15.0f); // float -> float -> *3
}

//=============================================================================
// OVERLOADING IN EXPRESSIONS
//=============================================================================

int ExprOverload(int x) { return x + 1; }
float ExprOverload(float x) { return x + 0.5f; }

[Test]
void Overload_InExpression_ResolvesBasedOnOperandTypes()
{
    int i = 10;
    float f = 10.0f;
    
    int ri = ExprOverload(i) + ExprOverload(i);
    float rf = ExprOverload(f) + ExprOverload(f);
    
    ASSERT(ri == 22);      // (10+1) + (10+1)
    ASSERT(rf == 21.0f);   // (10+0.5) + (10+0.5)
}

//=============================================================================
// AMBIGUOUS OVERLOAD DETECTION (SHOULD FAIL TO COMPILE)
//=============================================================================

// Uncomment to test that compiler correctly identifies ambiguous calls:
//
// int AmbiguousTest(int a, float b) { return 1; }
// int AmbiguousTest(float a, int b) { return 2; }
//
// [Test]
// void Overload_Ambiguous_ShouldNotCompile()
// {
//     // This should be a compile error - ambiguous call
//     int result = AmbiguousTest(1, 1);
// }

//=============================================================================
// OVERLOADING WITH CONST QUALIFIERS (IF SUPPORTED)
//=============================================================================

int ConstOverload(int x) { return 1; }
// int ConstOverload(const int x) { return 2; }  // May not be allowed as distinct overload

[Test]
void Overload_ConstParameter_BehavesAsExpected()
{
    int i = 5;
    const int ci = 5;
    
    ASSERT(ConstOverload(i) == 1);
    ASSERT(ConstOverload(ci) == 1);  // const doesn't affect overload resolution for values
}

//=============================================================================
// EDGE CASE: ZERO AND NULLPTR
//=============================================================================

int ZeroOverload(int x) { return 1; }
int ZeroOverload(float x) { return 2; }
int ZeroOverload(uint x) { return 3; }

[Test]
void Overload_ZeroLiteral_ResolvesToInt()
{
    ASSERT(ZeroOverload(0) == 1);  // 0 as int literal
}

//=============================================================================
// OVERLOADING WITH HALF PRECISION (IF SUPPORTED)
//=============================================================================

int HalfOverload(half x) { return 1; }
int HalfOverload(float x) { return 2; }

[Test]
void Overload_HalfVsFloat_SelectsCorrectPrecision()
{
    half h = (half)1.0;
    float f = 1.0f;
    
    ASSERT(HalfOverload(h) == 1);
    ASSERT(HalfOverload(f) == 2);
}

//=============================================================================
// COMPLEX SCENARIO: MULTIPLE OVERLOADS WITH CONVERSIONS
//=============================================================================

int ComplexOverload(float a, float b, float c) { return 1; }
int ComplexOverload(int a, int b, int c) { return 2; }
int ComplexOverload(float a, int b, float c) { return 3; }

[Test]
void Overload_ComplexMixedTypes_SelectsBestMatch()
{
    ASSERT(ComplexOverload(1.0f, 2.0f, 3.0f) == 1);
    ASSERT(ComplexOverload(1, 2, 3) == 2);
    ASSERT(ComplexOverload(1.0f, 2, 3.0f) == 3);
}

[Test]
void Overload_ComplexWithConversions_SelectsFewestConversions()
{
    // When exact match not available, prefer fewer conversions
    int i = 1;
    float f = 1.0f;
    
    // (float, float, int) - needs 1 conversion to match overload 1
    // Could also convert to overload 2 with 2 conversions
    ASSERT(ComplexOverload(f, f, i) == 1);
}

//=============================================================================
// WARP-DIVERGENT OVERLOAD CALLS
//=============================================================================

int WarpOverload(int x) { return x * 2; }
int WarpOverload(float x) { return (int)(x * 3.0f); }

[Test]
[WarpSize(4, 1)]
void Overload_InDivergentFlow_CallsCorrectOverload()
{
    uint lane = WaveGetLaneIndex();
    int result;
    
    if (lane < 2)
    {
        int i = (int)lane + 1;
        result = WarpOverload(i);  // int overload: *2
    }
    else
    {
        float f = (float)lane + 1.0f;
        result = WarpOverload(f);  // float overload: *3
    }
    
    ASSERT(WaveReadLaneAt(result, 0) == 2);   // 1 * 2
    ASSERT(WaveReadLaneAt(result, 1) == 4);   // 2 * 2
    ASSERT(WaveReadLaneAt(result, 2) == 9);   // 3.0 * 3
    ASSERT(WaveReadLaneAt(result, 3) == 12);  // 4.0 * 3
}
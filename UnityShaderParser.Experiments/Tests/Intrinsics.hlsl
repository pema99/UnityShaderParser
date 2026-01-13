[Test]
void Intrinsic_Abs()
{
    // Positive, negative, zero
    ASSERT(abs(-5.0) == 5.0);
    ASSERT(abs(5.0) == 5.0);
    ASSERT(abs(0.0) == 0.0);
    ASSERT(abs(-0.5) == 0.5);
    
    // Vector with mixed signs
    float3 v = abs(float3(-1.0, 2.0, -3.0));
    ASSERT(v.x == 1.0 && v.y == 2.0 && v.z == 3.0);
    
    // All negative vector
    float3 v2 = abs(float3(-1.0, -2.0, -3.0));
    ASSERT(v2.x == 1.0 && v2.y == 2.0 && v2.z == 3.0);
}

[Test]
void Intrinsic_Acos()
{
    // Boundary values
    ASSERT(abs(acos(1.0) - 0.0) < 0.001);
    ASSERT(abs(acos(-1.0) - 3.1415926) < 0.001);
    
    // Middle values
    ASSERT(abs(acos(0.0) - 1.5707963) < 0.001);
    ASSERT(abs(acos(0.5) - 1.0471975) < 0.001);
    ASSERT(abs(acos(-0.5) - 2.0943951) < 0.001);
    
    // Near boundaries
    ASSERT(abs(acos(0.9) - 0.4510268) < 0.001);
    ASSERT(abs(acos(-0.9) - 2.6905658) < 0.001);
}

[Test]
void Intrinsic_Asin()
{
    // Boundary values
    ASSERT(abs(asin(0.0) - 0.0) < 0.001);
    ASSERT(abs(asin(1.0) - 1.5707963) < 0.001);
    ASSERT(abs(asin(-1.0) - (-1.5707963)) < 0.001);
    
    // Middle values
    ASSERT(abs(asin(0.5) - 0.5235987) < 0.001);
    ASSERT(abs(asin(-0.5) - (-0.5235987)) < 0.001);
    
    // Near boundaries
    ASSERT(abs(asin(0.9) - 1.1197695) < 0.001);
    ASSERT(abs(asin(-0.9) - (-1.1197695)) < 0.001);
}

[Test]
void Intrinsic_Atan()
{
    // Zero
    ASSERT(abs(atan(0.0) - 0.0) < 0.001);
    
    // Positive values
    ASSERT(abs(atan(1.0) - 0.7853981) < 0.001);
    ASSERT(abs(atan(1000.0) - 1.5707963) < 0.001);
    
    // Negative values
    ASSERT(abs(atan(-1.0) - (-0.7853981)) < 0.001);
    ASSERT(abs(atan(-1000.0) - (-1.5707963)) < 0.001);
    
    // Small values
    ASSERT(abs(atan(0.1) - 0.0996686) < 0.001);
}

[Test]
void Intrinsic_Atan2()
{
    // Four quadrants
    ASSERT(abs(atan2(1.0, 1.0) - 0.7853981) < 0.001);     // Q1: +x, +y
    ASSERT(abs(atan2(1.0, -1.0) - 2.3561944) < 0.001);    // Q2: -x, +y
    ASSERT(abs(atan2(-1.0, -1.0) - (-2.3561944)) < 0.001); // Q3: -x, -y
    ASSERT(abs(atan2(-1.0, 1.0) - (-0.7853981)) < 0.001);  // Q4: +x, -y
    
    // Axes
    ASSERT(abs(atan2(1.0, 0.0) - 1.5707963) < 0.001);      // +y axis
    ASSERT(abs(atan2(-1.0, 0.0) - (-1.5707963)) < 0.001);  // -y axis
    ASSERT(abs(atan2(0.0, 1.0) - 0.0) < 0.001);            // +x axis
    ASSERT(abs(atan2(0.0, -1.0) - 3.1415926) < 0.001);     // -x axis
    
    // Zero
    ASSERT(abs(atan2(0.0, 0.0) - 0.0) < 0.001);
}

[Test]
void Intrinsic_Ceil()
{
    // Positive values
    ASSERT(ceil(1.5) == 2.0);
    ASSERT(ceil(1.1) == 2.0);
    ASSERT(ceil(0.1) == 1.0);
    
    // Negative values
    ASSERT(ceil(-1.5) == -1.0);
    ASSERT(ceil(-1.1) == -1.0);
    ASSERT(ceil(-0.1) == 0.0);
    
    // Exact values
    ASSERT(ceil(2.0) == 2.0);
    ASSERT(ceil(-2.0) == -2.0);
    ASSERT(ceil(0.0) == 0.0);
}

[Test]
void Intrinsic_Clamp()
{
    // Within range
    ASSERT(clamp(5.0, 0.0, 10.0) == 5.0);
    
    // Below range
    ASSERT(clamp(-5.0, 0.0, 10.0) == 0.0);
    ASSERT(clamp(-100.0, 0.0, 10.0) == 0.0);
    
    // Above range
    ASSERT(clamp(15.0, 0.0, 10.0) == 10.0);
    ASSERT(clamp(100.0, 0.0, 10.0) == 10.0);
    
    // At boundaries
    ASSERT(clamp(0.0, 0.0, 10.0) == 0.0);
    ASSERT(clamp(10.0, 0.0, 10.0) == 10.0);
    
    // Negative range
    ASSERT(clamp(-5.0, -10.0, -1.0) == -5.0);
    ASSERT(clamp(0.0, -10.0, -1.0) == -1.0);
    ASSERT(clamp(-15.0, -10.0, -1.0) == -10.0);
    
    // Vector clamping
    float3 v = clamp(float3(-1.0, 0.5, 2.0), 0.0, 1.0);
    ASSERT(v.x == 0.0 && v.y == 0.5 && v.z == 1.0);
}

[Test]
void Intrinsic_Cos()
{
    // Key angles
    ASSERT(abs(cos(0.0) - 1.0) < 0.001);
    ASSERT(abs(cos(3.1415926) - (-1.0)) < 0.001);
    ASSERT(abs(cos(1.5707963) - 0.0) < 0.001);
    
    // Negative angles
    ASSERT(abs(cos(-1.5707963) - 0.0) < 0.001);
    ASSERT(abs(cos(-3.1415926) - (-1.0)) < 0.001);
    
    // Quarter angles
    ASSERT(abs(cos(0.7853981) - 0.7071067) < 0.001);
    ASSERT(abs(cos(2.3561944) - (-0.7071067)) < 0.001);
}

[Test]
void Intrinsic_Cosh()
{
    // Zero
    ASSERT(abs(cosh(0.0) - 1.0) < 0.001);
    
    // Positive and negative (should be same - even function)
    ASSERT(abs(cosh(1.0) - 1.5430806) < 0.001);
    ASSERT(abs(cosh(-1.0) - 1.5430806) < 0.001);
    ASSERT(abs(cosh(2.0) - 3.7621956) < 0.001);
    ASSERT(abs(cosh(-2.0) - 3.7621956) < 0.001);
}

[Test]
void Intrinsic_Cross()
{
    // Standard basis vectors
    float3 a = float3(1.0, 0.0, 0.0);
    float3 b = float3(0.0, 1.0, 0.0);
    float3 c = cross(a, b);
    ASSERT(c.x == 0.0 && c.y == 0.0 && c.z == 1.0);
    
    // Reverse order (anti-commutative)
    c = cross(b, a);
    ASSERT(c.x == 0.0 && c.y == 0.0 && c.z == -1.0);
    
    // General vectors
    float3 d = cross(float3(1.0, 2.0, 3.0), float3(4.0, 5.0, 6.0));
    ASSERT(d.x == -3.0 && d.y == 6.0 && d.z == -3.0);
    
    // Parallel vectors (should be zero)
    float3 e = cross(float3(2.0, 4.0, 6.0), float3(1.0, 2.0, 3.0));
    ASSERT(abs(e.x) < 0.001 && abs(e.y) < 0.001 && abs(e.z) < 0.001);
    
    // With negative components
    float3 f = cross(float3(-1.0, 0.0, 0.0), float3(0.0, 1.0, 0.0));
    ASSERT(f.x == 0.0 && f.y == 0.0 && f.z == -1.0);
}

[Test]
void Intrinsic_Degrees()
{
    // Zero
    ASSERT(abs(degrees(0.0) - 0.0) < 0.001);
    
    // Common angles
    ASSERT(abs(degrees(3.1415926) - 180.0) < 0.1);
    ASSERT(abs(degrees(1.5707963) - 90.0) < 0.1);
    ASSERT(abs(degrees(6.2831853) - 360.0) < 0.1);
    
    // Negative angles
    ASSERT(abs(degrees(-1.5707963) - (-90.0)) < 0.1);
    ASSERT(abs(degrees(-3.1415926) - (-180.0)) < 0.1);
    
    // Small angle
    ASSERT(abs(degrees(0.1) - 5.729577) < 0.1);
}

[Test]
void Intrinsic_Distance()
{
    // Scalar distance
    ASSERT(abs(distance(0.0, 5.0) - 5.0) < 0.001);
    ASSERT(abs(distance(5.0, 0.0) - 5.0) < 0.001);
    ASSERT(abs(distance(-3.0, 2.0) - 5.0) < 0.001);
    
    // 2D distance
    ASSERT(abs(distance(float2(0.0, 0.0), float2(3.0, 4.0)) - 5.0) < 0.001);
    ASSERT(abs(distance(float2(1.0, 1.0), float2(4.0, 5.0)) - 5.0) < 0.001);
    
    // 3D distance
    ASSERT(abs(distance(float3(0.0, 0.0, 0.0), float3(3.0, 4.0, 0.0)) - 5.0) < 0.001);
    ASSERT(abs(distance(float3(1.0, 2.0, 3.0), float3(1.0, 2.0, 3.0)) - 0.0) < 0.001);
    
    // Negative coordinates
    ASSERT(abs(distance(float3(-1.0, -1.0, -1.0), float3(1.0, 1.0, 1.0)) - 3.464101) < 0.001);
}

[Test]
void Intrinsic_Dot()
{
    // Scalar
    ASSERT(dot(2.0, 3.0) == 6.0);
    ASSERT(dot(-2.0, 3.0) == -6.0);
    
    // Orthogonal vectors (dot product = 0)
    ASSERT(dot(float2(1.0, 0.0), float2(0.0, 1.0)) == 0.0);
    ASSERT(dot(float3(1.0, 0.0, 0.0), float3(0.0, 1.0, 0.0)) == 0.0);
    
    // Parallel vectors
    ASSERT(dot(float3(1.0, 2.0, 3.0), float3(2.0, 4.0, 6.0)) == 28.0);
    
    // Mixed signs
    ASSERT(dot(float3(1.0, 2.0, 3.0), float3(4.0, 5.0, 6.0)) == 32.0);
    ASSERT(dot(float3(-1.0, 2.0, -3.0), float3(2.0, -1.0, 1.0)) == -7.0);
    ASSERT(dot(float3(-1.0, -2.0, -3.0), float3(1.0, 2.0, 3.0)) == -14.0);
    
    // Same vector (dot with itself = squared length)
    ASSERT(dot(float3(3.0, 4.0, 0.0), float3(3.0, 4.0, 0.0)) == 25.0);
}

[Test]
void Intrinsic_Exp()
{
    // Zero
    ASSERT(abs(exp(0.0) - 1.0) < 0.001);
    
    // Positive values
    ASSERT(abs(exp(1.0) - 2.7182818) < 0.001);
    ASSERT(abs(exp(2.0) - 7.3890560) < 0.001);
    
    // Negative values
    ASSERT(abs(exp(-1.0) - 0.3678794) < 0.001);
    ASSERT(abs(exp(-2.0) - 0.1353352) < 0.001);
    
    // Small values
    ASSERT(abs(exp(0.1) - 1.1051709) < 0.001);
    ASSERT(abs(exp(-0.1) - 0.9048374) < 0.001);
}

[Test]
void Intrinsic_Exp2()
{
    // Zero
    ASSERT(abs(exp2(0.0) - 1.0) < 0.001);
    
    // Positive integer powers
    ASSERT(abs(exp2(1.0) - 2.0) < 0.001);
    ASSERT(abs(exp2(2.0) - 4.0) < 0.001);
    ASSERT(abs(exp2(10.0) - 1024.0) < 0.1);
    
    // Negative powers
    ASSERT(abs(exp2(-1.0) - 0.5) < 0.001);
    ASSERT(abs(exp2(-2.0) - 0.25) < 0.001);
    
    // Fractional powers
    ASSERT(abs(exp2(0.5) - 1.414213) < 0.001);
    ASSERT(abs(exp2(1.5) - 2.828427) < 0.001);
}

[Test]
void Intrinsic_Faceforward()
{
    float3 n = float3(0.0, 1.0, 0.0);
    float3 i = float3(0.0, -1.0, 0.0);
    float3 ng = float3(0.0, 1.0, 0.0);
    float3 result = faceforward(n, i, ng);
    ASSERT(result.x == 0.0 && result.y == 1.0 && result.z == 0.0);
    
    i = float3(0.0, 1.0, 0.0);
    result = faceforward(n, i, ng);
    ASSERT(result.x == 0.0 && result.y == -1.0 && result.z == 0.0);
    
    n = float3(1.0, 0.0, 0.0);
    i = float3(-1.0, 0.0, 0.0);
    ng = float3(1.0, 0.0, 0.0);
    result = faceforward(n, i, ng);
    ASSERT(result.x == 1.0 && result.y == 0.0 && result.z == 0.0);
}


[Test]
void Intrinsic_Floor()
{
    // Positive values
    ASSERT(floor(1.5) == 1.0);
    ASSERT(floor(1.1) == 1.0);
    ASSERT(floor(0.9) == 0.0);
    
    // Negative values
    ASSERT(floor(-1.5) == -2.0);
    ASSERT(floor(-1.1) == -2.0);
    ASSERT(floor(-0.1) == -1.0);
    
    // Exact values
    ASSERT(floor(2.0) == 2.0);
    ASSERT(floor(-2.0) == -2.0);
    ASSERT(floor(0.0) == 0.0);
}

[Test]
void Intrinsic_Fma()
{
    // Basic operation
    ASSERT(fma(2.0, 3.0, 4.0) == 10.0);
    ASSERT(fma(0.5, 2.0, 1.0) == 2.0);
    
    // With negative multiply
    ASSERT(fma(-1.0, 5.0, 10.0) == 5.0);
    ASSERT(fma(-2.0, 3.0, 1.0) == -5.0);
    
    // With negative add
    ASSERT(fma(2.0, 3.0, -4.0) == 2.0);
    
    // All negative
    ASSERT(fma(-2.0, -3.0, -4.0) == 2.0);
    
    // Zero cases
    ASSERT(fma(0.0, 5.0, 10.0) == 10.0);
    ASSERT(fma(5.0, 0.0, 10.0) == 10.0);
}

[Test]
void Intrinsic_Fmod()
{
    // Positive operands
    ASSERT(abs(fmod(5.0, 2.0) - 1.0) < 0.001);
    ASSERT(abs(fmod(7.5, 2.5) - 0.0) < 0.001);
    ASSERT(abs(fmod(3.5, 2.0) - 1.5) < 0.001);
    
    // Negative dividend, positive divisor
    ASSERT(abs(fmod(-5.0, 2.0) - (-1.0)) < 0.001);
    ASSERT(abs(fmod(-7.5, 2.5) - 0.0) < 0.001);
    
    // Positive dividend, negative divisor
    ASSERT(abs(fmod(5.0, -2.0) - 1.0) < 0.001);
    
    // Both negative
    ASSERT(abs(fmod(-5.0, -2.0) - (-1.0)) < 0.001);
    
    // Exact division
    ASSERT(abs(fmod(10.0, 5.0) - 0.0) < 0.001);
}

[Test]
void Intrinsic_Frac()
{
    // Positive values
    ASSERT(abs(frac(1.5) - 0.5) < 0.001);
    ASSERT(abs(frac(2.75) - 0.75) < 0.001);
    ASSERT(abs(frac(0.25) - 0.25) < 0.001);
    
    // Negative values (frac returns positive fractional part)
    ASSERT(abs(frac(-1.5) - 0.5) < 0.001);
    ASSERT(abs(frac(-2.75) - 0.75) < 0.001);
    
    // Whole numbers
    ASSERT(abs(frac(5.0) - 0.0) < 0.001);
    ASSERT(abs(frac(-3.0) - 0.0) < 0.001);
    ASSERT(abs(frac(0.0) - 0.0) < 0.001);
}

[Test]
void Intrinsic_Isfinite()
{
    // Finite values
    ASSERT(isfinite(0.0));
    ASSERT(isfinite(1.0));
    ASSERT(isfinite(-1.0));
    ASSERT(isfinite(-1000.0));
    ASSERT(isfinite(0.00001));
    ASSERT(isfinite(1000000.0));
    
    // Infinity should return false
    ASSERT(!isfinite(1.0 / 0.0));
    ASSERT(!isfinite(-1.0 / 0.0));
    
    // NaN should return false
    ASSERT(!isfinite(0.0 / 0.0));
}

[Test]
void Intrinsic_Isinf()
{
    // Non-infinite values
    ASSERT(!isinf(0.0));
    ASSERT(!isinf(1.0));
    ASSERT(!isinf(-1.0));
    ASSERT(!isinf(1000000.0));
    ASSERT(!isinf(-1000000.0));
    
    // Positive infinity
    ASSERT(isinf(1.0 / 0.0));
    
    // Negative infinity
    ASSERT(isinf(-1.0 / 0.0));
    
    // NaN is not infinity
    ASSERT(!isinf(0.0 / 0.0));
}

[Test]
void Intrinsic_Isnan()
{
    // Non-NaN values
    ASSERT(!isnan(0.0));
    ASSERT(!isnan(1.0));
    ASSERT(!isnan(-1.0));
    ASSERT(!isnan(1000000.0));
    
    // Infinity is not NaN
    ASSERT(!isnan(1.0 / 0.0));
    ASSERT(!isnan(-1.0 / 0.0));
    
    // NaN values
    ASSERT(isnan(0.0 / 0.0));
    ASSERT(isnan(sqrt(-1.0)));
}

[Test]
void Intrinsic_Ldexp()
{
    // Zero exponent
    ASSERT(abs(ldexp(1.0, 0) - 1.0) < 0.001);
    ASSERT(abs(ldexp(5.0, 0) - 5.0) < 0.001);
    
    // Positive exponent
    ASSERT(abs(ldexp(1.0, 3) - 8.0) < 0.001);
    ASSERT(abs(ldexp(2.0, 2) - 8.0) < 0.001);
    ASSERT(abs(ldexp(1.5, 4) - 24.0) < 0.001);
    
    // Negative exponent
    ASSERT(abs(ldexp(1.0, -2) - 0.25) < 0.001);
    ASSERT(abs(ldexp(8.0, -3) - 1.0) < 0.001);
    
    // Negative mantissa
    ASSERT(abs(ldexp(-1.0, 3) - (-8.0)) < 0.001);
    ASSERT(abs(ldexp(-2.0, -2) - (-0.5)) < 0.001);
}

[Test]
void Intrinsic_Length()
{
    // Scalar
    ASSERT(abs(length(5.0) - 5.0) < 0.001);
    ASSERT(abs(length(-5.0) - 5.0) < 0.001);
    ASSERT(abs(length(0.0) - 0.0) < 0.001);
    
    // 2D vectors
    ASSERT(abs(length(float2(3.0, 4.0)) - 5.0) < 0.001);
    ASSERT(abs(length(float2(1.0, 1.0)) - 1.414213) < 0.001);
    ASSERT(abs(length(float2(-3.0, 4.0)) - 5.0) < 0.001);
    
    // 3D vectors
    ASSERT(abs(length(float3(3.0, 4.0, 0.0)) - 5.0) < 0.001);
    ASSERT(abs(length(float3(-1.0, -2.0, -2.0)) - 3.0) < 0.001);
    ASSERT(abs(length(float3(1.0, 0.0, 0.0)) - 1.0) < 0.001);
    
    // All negative components
    ASSERT(abs(length(float3(-3.0, -4.0, 0.0)) - 5.0) < 0.001);
}

[Test]
void Intrinsic_Lerp()
{
    // Scalar interpolation
    ASSERT(abs(lerp(0.0, 10.0, 0.5) - 5.0) < 0.001);
    ASSERT(abs(lerp(0.0, 10.0, 0.0) - 0.0) < 0.001);
    ASSERT(abs(lerp(0.0, 10.0, 1.0) - 10.0) < 0.001);
    ASSERT(abs(lerp(0.0, 10.0, 0.25) - 2.5) < 0.001);
    
    // Extrapolation (t outside [0,1])
    ASSERT(abs(lerp(0.0, 10.0, 1.5) - 15.0) < 0.001);
    ASSERT(abs(lerp(0.0, 10.0, -0.5) - (-5.0)) < 0.001);
    
    // Negative range
    ASSERT(abs(lerp(-10.0, 10.0, 0.5) - 0.0) < 0.001);
    ASSERT(abs(lerp(10.0, 0.0, 0.5) - 5.0) < 0.001);
    
    // Vector interpolation
    float3 v = lerp(float3(0.0, 0.0, 0.0), float3(10.0, 20.0, 30.0), 0.5);
    ASSERT(v.x == 5.0 && v.y == 10.0 && v.z == 15.0);
    
    // Vector with negative values
    float3 v2 = lerp(float3(-5.0, 10.0, 0.0), float3(5.0, 0.0, 10.0), 0.5);
    ASSERT(v2.x == 0.0 && v2.y == 5.0 && v2.z == 5.0);
}

[Test]
void Intrinsic_Log()
{
    // e^x values
    ASSERT(abs(log(1.0) - 0.0) < 0.001);
    ASSERT(abs(log(2.7182818) - 1.0) < 0.001);
    ASSERT(abs(log(7.389056) - 2.0) < 0.001);
    
    // Other values
    ASSERT(abs(log(2.0) - 0.693147) < 0.001);
    ASSERT(abs(log(10.0) - 2.302585) < 0.001);
    ASSERT(abs(log(0.5) - (-0.693147)) < 0.001);
}

[Test]
void Intrinsic_Log2()
{
    // Powers of 2
    ASSERT(abs(log2(1.0) - 0.0) < 0.001);
    ASSERT(abs(log2(2.0) - 1.0) < 0.001);
    ASSERT(abs(log2(4.0) - 2.0) < 0.001);
    ASSERT(abs(log2(8.0) - 3.0) < 0.001);
    ASSERT(abs(log2(1024.0) - 10.0) < 0.001);
    
    // Non-powers of 2
    ASSERT(abs(log2(3.0) - 1.584962) < 0.001);
    ASSERT(abs(log2(10.0) - 3.321928) < 0.001);
    
    // Fractional values (negative log)
    ASSERT(abs(log2(0.5) - (-1.0)) < 0.001);
    ASSERT(abs(log2(0.25) - (-2.0)) < 0.001);
}

[Test]
void Intrinsic_Mad()
{
    // Basic operation
    ASSERT(mad(2.0, 3.0, 4.0) == 10.0);
    ASSERT(mad(0.5, 2.0, 1.0) == 2.0);
    
    // With negative multiply
    ASSERT(mad(-1.0, 5.0, 10.0) == 5.0);
    ASSERT(mad(-2.0, 3.0, 1.0) == -5.0);
    
    // With negative add
    ASSERT(mad(2.0, 3.0, -4.0) == 2.0);
    
    // All negative
    ASSERT(mad(-2.0, -3.0, -4.0) == 2.0);
    
    // Zero cases
    ASSERT(mad(0.0, 5.0, 10.0) == 10.0);
    ASSERT(mad(5.0, 0.0, 10.0) == 10.0);
}

[Test]
void Intrinsic_Max()
{
    // Both positive
    ASSERT(max(5.0, 3.0) == 5.0);
    ASSERT(max(3.0, 5.0) == 5.0);
    
    // Both negative
    ASSERT(max(-5.0, -3.0) == -3.0);
    ASSERT(max(-3.0, -5.0) == -3.0);
    
    // Mixed signs
    ASSERT(max(-5.0, 3.0) == 3.0);
    ASSERT(max(3.0, -5.0) == 3.0);
    
    // With zero
    ASSERT(max(0.0, 5.0) == 5.0);
    ASSERT(max(-5.0, 0.0) == 0.0);
    ASSERT(max(0.0, 0.0) == 0.0);
    
    // Equal values
    ASSERT(max(5.0, 5.0) == 5.0);
    
    // Vector max
    float3 v = max(float3(1.0, 5.0, 3.0), float3(2.0, 4.0, 6.0));
    ASSERT(v.x == 2.0 && v.y == 5.0 && v.z == 6.0);
    
    // Vector with negative values
    float3 v2 = max(float3(-1.0, 2.0, -3.0), float3(-2.0, -4.0, 1.0));
    ASSERT(v2.x == -1.0 && v2.y == 2.0 && v2.z == 1.0);
}

[Test]
void Intrinsic_Min()
{
    // Both positive
    ASSERT(min(5.0, 3.0) == 3.0);
    ASSERT(min(3.0, 5.0) == 3.0);
    
    // Both negative
    ASSERT(min(-5.0, -3.0) == -5.0);
    ASSERT(min(-3.0, -5.0) == -5.0);
    
    // Mixed signs
    ASSERT(min(-5.0, 3.0) == -5.0);
    ASSERT(min(3.0, -5.0) == -5.0);
    
    // With zero
    ASSERT(min(0.0, 5.0) == 0.0);
    ASSERT(min(-5.0, 0.0) == -5.0);
    ASSERT(min(0.0, 0.0) == 0.0);
    
    // Equal values
    ASSERT(min(5.0, 5.0) == 5.0);
    
    // Vector min
    float3 v = min(float3(1.0, 5.0, 3.0), float3(2.0, 4.0, 6.0));
    ASSERT(v.x == 1.0 && v.y == 4.0 && v.z == 3.0);
    
    // Vector with negative values
    float3 v2 = min(float3(-1.0, 2.0, -3.0), float3(-2.0, -4.0, 1.0));
    ASSERT(v2.x == -2.0 && v2.y == -4.0 && v2.z == -3.0);
}

[Test]
void Intrinsic_Noise()
{
    // Test that noise returns values in [0,1] range
    float n1 = noise(float3(0.0, 0.0, 0.0));
    float n2 = noise(float3(1.0, 2.0, 3.0));
    float n3 = noise(float3(-1.0, -2.0, -3.0));
    float n4 = noise(float3(100.0, 200.0, 300.0));
    
    ASSERT(n1 >= 0.0 && n1 <= 1.0);
    ASSERT(n2 >= 0.0 && n2 <= 1.0);
    ASSERT(n3 >= 0.0 && n3 <= 1.0);
    ASSERT(n4 >= 0.0 && n4 <= 1.0);
}

[Test]
void Intrinsic_Normalize()
{
    // Standard vector (longer than 1)
    float3 v1 = normalize(float3(3.0, 4.0, 0.0));
    ASSERT(abs(v1.x - 0.6) < 0.001 && abs(v1.y - 0.8) < 0.001 && abs(v1.z - 0.0) < 0.001);
    ASSERT(abs(length(v1) - 1.0) < 0.001);
    
    // All negative components
    float3 v2 = normalize(float3(-1.0, -1.0, -1.0));
    float expected = -0.57735;
    ASSERT(abs(v2.x - expected) < 0.001 && abs(v2.y - expected) < 0.001 && abs(v2.z - expected) < 0.001);
    ASSERT(abs(length(v2) - 1.0) < 0.001);
    
    // Axis-aligned vector (longer than 1)
    float3 v3 = normalize(float3(10.0, 0.0, 0.0));
    ASSERT(abs(v3.x - 1.0) < 0.001 && abs(v3.y - 0.0) < 0.001 && abs(v3.z - 0.0) < 0.001);
    
    // Mixed positive and negative (longer than 1)
    float3 v4 = normalize(float3(2.0, -2.0, 1.0));
    ASSERT(abs(length(v4) - 1.0) < 0.001);
    
    // Already normalized vector
    float3 v5 = normalize(float3(1.0, 0.0, 0.0));
    ASSERT(abs(v5.x - 1.0) < 0.001 && abs(v5.y - 0.0) < 0.001 && abs(v5.z - 0.0) < 0.001);
    
    // Short vector (less than 1)
    float3 v6 = normalize(float3(0.3, 0.4, 0.0));
    ASSERT(abs(v6.x - 0.6) < 0.001 && abs(v6.y - 0.8) < 0.001 && abs(v6.z - 0.0) < 0.001);
    ASSERT(abs(length(v6) - 1.0) < 0.001);
}

[Test]
void Intrinsic_Pow()
{
    // Positive base, positive exponent
    ASSERT(abs(pow(2.0, 3.0) - 8.0) < 0.001);
    ASSERT(abs(pow(3.0, 2.0) - 9.0) < 0.001);
    
    // Zero exponent
    ASSERT(abs(pow(5.0, 0.0) - 1.0) < 0.001);
    ASSERT(abs(pow(0.0, 0.0) - 1.0) < 0.001);
    
    // Fractional exponent (roots)
    ASSERT(abs(pow(4.0, 0.5) - 2.0) < 0.001);
    ASSERT(abs(pow(27.0, 0.333333) - 3.0) < 0.01);
    
    // Negative exponent
    ASSERT(abs(pow(2.0, -1.0) - 0.5) < 0.001);
    ASSERT(abs(pow(4.0, -2.0) - 0.0625) < 0.001);
    
    // Base of 1
    ASSERT(abs(pow(1.0, 5.0) - 1.0) < 0.001);
    ASSERT(abs(pow(1.0, -5.0) - 1.0) < 0.001);
    
    // Large exponent
    ASSERT(abs(pow(2.0, 10.0) - 1024.0) < 0.1);
}

[Test]
void Intrinsic_Radians()
{
    // Zero
    ASSERT(abs(radians(0.0) - 0.0) < 0.001);
    
    // Common angles
    ASSERT(abs(radians(180.0) - 3.1415926) < 0.001);
    ASSERT(abs(radians(90.0) - 1.5707963) < 0.001);
    ASSERT(abs(radians(360.0) - 6.2831853) < 0.001);
    ASSERT(abs(radians(45.0) - 0.7853981) < 0.001);
    
    // Negative angles
    ASSERT(abs(radians(-90.0) - (-1.5707963)) < 0.001);
    ASSERT(abs(radians(-180.0) - (-3.1415926)) < 0.001);
}

[Test]
void Intrinsic_Rcp()
{
    // Positive values
    ASSERT(abs(rcp(2.0) - 0.5) < 0.001);
    ASSERT(abs(rcp(0.5) - 2.0) < 0.001);
    ASSERT(abs(rcp(1.0) - 1.0) < 0.001);
    ASSERT(abs(rcp(10.0) - 0.1) < 0.001);
    
    // Negative values
    ASSERT(abs(rcp(-4.0) - (-0.25)) < 0.001);
    ASSERT(abs(rcp(-0.5) - (-2.0)) < 0.001);
    ASSERT(abs(rcp(-1.0) - (-1.0)) < 0.001);
    
    // Fractional values
    ASSERT(abs(rcp(0.25) - 4.0) < 0.001);
    ASSERT(abs(rcp(0.1) - 10.0) < 0.01);
}

[Test]
void Intrinsic_Reflect()
{
    // Incident at 45 degrees to normal
    float3 i = normalize(float3(1.0, -1.0, 0.0));
    float3 n = float3(0.0, 1.0, 0.0);
    float3 r = reflect(i, n);
    ASSERT(abs(r.x - 0.7071067) < 0.001 && abs(r.y - 0.7071067) < 0.001 && abs(r.z - 0.0) < 0.001);
    
    // Head-on collision (should reflect back)
    i = float3(1.0, 0.0, 0.0);
    n = float3(-1.0, 0.0, 0.0);
    r = reflect(i, n);
    ASSERT(abs(r.x - (-1.0)) < 0.001 && abs(r.y - 0.0) < 0.001 && abs(r.z - 0.0) < 0.001);
    
    // Different axis
    i = float3(0.0, 0.0, -1.0);
    n = float3(0.0, 0.0, 1.0);
    r = reflect(i, n);
    ASSERT(abs(r.x - 0.0) < 0.001 && abs(r.y - 0.0) < 0.001 && abs(r.z - 1.0) < 0.001);
}

[Test]
void Intrinsic_Refract()
{
    // Basic refraction entering denser medium
    float3 i = normalize(float3(1.0, -1.0, 0.0));
    float3 n = float3(0.0, 1.0, 0.0);
    float eta = 0.66; // air to glass
    float3 r = refract(i, n, eta);
    ASSERT(r.x != 0.0 || r.y != 0.0 || r.z != 0.0);
    
    // Different incident angle
    i = normalize(float3(0.5, -1.0, 0.0));
    r = refract(i, n, eta);
    ASSERT(r.x != 0.0 || r.y != 0.0 || r.z != 0.0);
   
    
    // Normal incidence
    i = float3(0.0, -1.0, 0.0);
    r = refract(i, n, 0.66);
    ASSERT(abs(r.x) < 0.001 && r.y < 0.0 && abs(r.z) < 0.001);
}

[Test]
void Intrinsic_Round()
{
    // Positive values
    ASSERT(round(1.5) == 2.0);
    ASSERT(round(1.4) == 1.0);
    ASSERT(round(1.6) == 2.0);
    ASSERT(round(0.5) == 0.0 || round(0.5) == 1.0); // Banker's rounding may vary
    
    // Negative values
    ASSERT(round(-1.5) == -2.0);
    ASSERT(round(-1.4) == -1.0);
    ASSERT(round(-1.6) == -2.0);
    
    // Exact values
    ASSERT(round(2.0) == 2.0);
    ASSERT(round(-2.0) == -2.0);
    ASSERT(round(0.0) == 0.0);
}

[Test]
void Intrinsic_Rsqrt()
{
    // Perfect squares
    ASSERT(abs(rsqrt(4.0) - 0.5) < 0.001);
    ASSERT(abs(rsqrt(1.0) - 1.0) < 0.001);
    ASSERT(abs(rsqrt(9.0) - 0.333333) < 0.001);
    ASSERT(abs(rsqrt(16.0) - 0.25) < 0.001);
    
    // Non-perfect squares
    ASSERT(abs(rsqrt(2.0) - 0.7071067) < 0.001);
    ASSERT(abs(rsqrt(3.0) - 0.5773502) < 0.001);
    
    // Fractional values
    ASSERT(abs(rsqrt(0.25) - 2.0) < 0.001);
    ASSERT(abs(rsqrt(0.5) - 1.414213) < 0.001);
    
    // Verify rsqrt(x) * sqrt(x) = 1
    ASSERT(abs(rsqrt(5.0) * sqrt(5.0) - 1.0) < 0.001);
}

[Test]
void Intrinsic_Saturate()
{
    // Values within [0,1]
    ASSERT(saturate(0.5) == 0.5);
    ASSERT(saturate(0.0) == 0.0);
    ASSERT(saturate(1.0) == 1.0);
    ASSERT(saturate(0.25) == 0.25);
    
    // Values below 0
    ASSERT(saturate(-0.5) == 0.0);
    ASSERT(saturate(-1.0) == 0.0);
    ASSERT(saturate(-100.0) == 0.0);
    
    // Values above 1
    ASSERT(saturate(1.5) == 1.0);
    ASSERT(saturate(2.0) == 1.0);
    ASSERT(saturate(100.0) == 1.0);
    
    // Vector saturate
    float3 v = saturate(float3(-0.5, 0.5, 1.5));
    ASSERT(v.x == 0.0 && v.y == 0.5 && v.z == 1.0);
}

[Test]
void Intrinsic_Sign()
{
    // Positive values
    ASSERT(sign(5.0) == 1.0);
    ASSERT(sign(0.1) == 1.0);
    
    // Negative values
    ASSERT(sign(-5.0) == -1.0);
    ASSERT(sign(-0.1) == -1.0);
    
    // Zero
    ASSERT(sign(0.0) == 0.0);
    
    // Vector with mixed signs
    float3 v = sign(float3(-1.0, 0.0, 2.0));
    ASSERT(v.x == -1.0 && v.y == 0.0 && v.z == 1.0);
    
    // All negative
    float3 v2 = sign(float3(-1.0, -5.0, -0.1));
    ASSERT(v2.x == -1.0 && v2.y == -1.0 && v2.z == -1.0);
}

[Test]
void Intrinsic_Sin()
{
    // Key angles
    ASSERT(abs(sin(0.0) - 0.0) < 0.001);
    ASSERT(abs(sin(1.5707963) - 1.0) < 0.001);
    ASSERT(abs(sin(3.1415926) - 0.0) < 0.001);
    
    // Negative angles
    ASSERT(abs(sin(-1.5707963) - (-1.0)) < 0.001);
    ASSERT(abs(sin(-3.1415926) - 0.0) < 0.001);
    
    // Quarter angles
    ASSERT(abs(sin(0.7853981) - 0.7071067) < 0.001);
    ASSERT(abs(sin(2.3561944) - 0.7071067) < 0.001);
    
    // Full period
    ASSERT(abs(sin(6.2831853) - 0.0) < 0.001);
}

[Test]
void Intrinsic_Sinh()
{
    // Zero
    ASSERT(abs(sinh(0.0) - 0.0) < 0.001);
    
    // Positive values
    ASSERT(abs(sinh(1.0) - 1.1752011) < 0.001);
    ASSERT(abs(sinh(2.0) - 3.6268604) < 0.001);
    
    // Negative values (sinh is odd function)
    ASSERT(abs(sinh(-1.0) - (-1.1752011)) < 0.001);
    ASSERT(abs(sinh(-2.0) - (-3.6268604)) < 0.001);
    
    // Small values
    ASSERT(abs(sinh(0.1) - 0.1001668) < 0.001);
}

[Test]
void Intrinsic_Smoothstep()
{
    // Standard range [0,1]
    ASSERT(abs(smoothstep(0.0, 1.0, 0.0) - 0.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 1.0) - 1.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 0.5) - 0.5) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 0.25) - 0.15625) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 0.75) - 0.84375) < 0.001);
    
    // Below range (should clamp to 0)
    ASSERT(abs(smoothstep(0.0, 1.0, -0.5) - 0.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, -1.0) - 0.0) < 0.001);
    
    // Above range (should clamp to 1)
    ASSERT(abs(smoothstep(0.0, 1.0, 1.5) - 1.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 2.0) - 1.0) < 0.001);
    
    // Different range
    ASSERT(abs(smoothstep(0.0, 10.0, 5.0) - 0.5) < 0.001);
    ASSERT(abs(smoothstep(-1.0, 1.0, 0.0) - 0.5) < 0.001);
}

[Test]
void Intrinsic_Sqrt()
{
    // Perfect squares
    ASSERT(abs(sqrt(4.0) - 2.0) < 0.001);
    ASSERT(abs(sqrt(9.0) - 3.0) < 0.001);
    ASSERT(abs(sqrt(16.0) - 4.0) < 0.001);
    ASSERT(abs(sqrt(1.0) - 1.0) < 0.001);
    
    // Zero
    ASSERT(abs(sqrt(0.0) - 0.0) < 0.001);
    
    // Non-perfect squares
    ASSERT(abs(sqrt(2.0) - 1.414213) < 0.001);
    ASSERT(abs(sqrt(3.0) - 1.732050) < 0.001);
    
    // Fractional values
    ASSERT(abs(sqrt(0.25) - 0.5) < 0.001);
    ASSERT(abs(sqrt(0.5) - 0.7071067) < 0.001);
    
    // Large values
    ASSERT(abs(sqrt(100.0) - 10.0) < 0.001);
}

[Test]
void Intrinsic_Step()
{
    // Value greater than edge
    ASSERT(step(0.5, 1.0) == 1.0);
    ASSERT(step(0.0, 1.0) == 1.0);
    
    // Value equal to edge
    ASSERT(step(0.5, 0.5) == 1.0);
    
    // Value less than edge
    ASSERT(step(0.5, 0.3) == 0.0);
    ASSERT(step(0.0, -1.0) == 0.0);
    
    // Negative values
    ASSERT(step(-0.5, 0.0) == 1.0);
    ASSERT(step(0.0, -0.5) == 0.0);
    ASSERT(step(-1.0, -0.5) == 1.0);
    ASSERT(step(-0.5, -1.0) == 0.0);
    
    // Vector step
    float2 v = step(float2(0.5, 0.5), float2(0.3, 0.7));
    ASSERT(v.x == 0.0 && v.y == 1.0);
    
    // Vector with negative values
    float3 v2 = step(float3(0.0, -1.0, 1.0), float3(-0.5, -0.5, 1.5));
    ASSERT(v2.x == 0.0 && v2.y == 1.0 && v2.z == 1.0);
}

[Test]
void Intrinsic_Tan()
{
    // Zero
    ASSERT(abs(tan(0.0) - 0.0) < 0.001);
    
    // 45 degrees
    ASSERT(abs(tan(0.7853981) - 1.0) < 0.001);
    ASSERT(abs(tan(-0.7853981) - (-1.0)) < 0.001);
    
    // Small angles
    ASSERT(abs(tan(0.1) - 0.1003346) < 0.001);
    ASSERT(abs(tan(-0.1) - (-0.1003346)) < 0.001);
    
    // Other angles
    ASSERT(abs(tan(1.0) - 1.5574077) < 0.001);
}

[Test]
void Intrinsic_Tanh()
{
    // Zero
    ASSERT(abs(tanh(0.0) - 0.0) < 0.001);
    
    // Positive values
    ASSERT(abs(tanh(1.0) - 0.7615941) < 0.001);
    ASSERT(abs(tanh(2.0) - 0.9640275) < 0.001);
    
    // Negative values (tanh is odd function)
    ASSERT(abs(tanh(-1.0) - (-0.7615941)) < 0.001);
    ASSERT(abs(tanh(-2.0) - (-0.9640275)) < 0.001);
    
    // Small values
    ASSERT(abs(tanh(0.1) - 0.0996679) < 0.001);
    
    // Large values (should approach ±1)
    ASSERT(abs(tanh(5.0) - 0.9999092) < 0.001);
    ASSERT(abs(tanh(-5.0) - (-0.9999092)) < 0.001);
}


[Test]
void Intrinsic_Transpose()
{
    float2x2 m = float2x2(1.0, 2.0, 3.0, 4.0);
    float2x2 t = transpose(m);
    ASSERT(t == float2x2(1.0, 3.0, 2.0, 4.0));
    ASSERT(transpose(t) == m);
}

[Test]
void Intrinsic_Trunc()
{
    // Positive values
    ASSERT(trunc(1.5) == 1.0);
    ASSERT(trunc(1.9) == 1.0);
    ASSERT(trunc(2.1) == 2.0);
    
    // Negative values
    ASSERT(trunc(-1.5) == -1.0);
    ASSERT(trunc(-1.9) == -1.0);
    ASSERT(trunc(-2.1) == -2.0);
    
    // Exact values
    ASSERT(trunc(2.0) == 2.0);
    ASSERT(trunc(-2.0) == -2.0);
    ASSERT(trunc(0.0) == 0.0);
    
    // Near zero
    ASSERT(trunc(0.9) == 0.0);
    ASSERT(trunc(-0.9) == 0.0);
}
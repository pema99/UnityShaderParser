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

[Test]
void Intrinsic_Asfloat()
{
    // Convert int to float (bit pattern reinterpretation)
    // 0x3F800000 is the bit pattern for 1.0f
    int i1 = 0x3F800000;
    ASSERT(asfloat(i1) == 1.0);
    
    // 0x40000000 is the bit pattern for 2.0f
    int i2 = 0x40000000;
    ASSERT(asfloat(i2) == 2.0);
    
    // 0xBF800000 is the bit pattern for -1.0f
    int i3 = 0xBF800000;
    ASSERT(asfloat(i3) == -1.0);
    
    // 0x00000000 is the bit pattern for 0.0f
    int i4 = 0x00000000;
    ASSERT(asfloat(i4) == 0.0);
    
    // Convert uint to float
    uint u1 = 0x3F800000u;
    ASSERT(asfloat(u1) == 1.0);
    
    uint u2 = 0x40490FDBu; // Approximately pi
    ASSERT(abs(asfloat(u2) - 3.1415926) < 0.001);
    
    // Test with float input (should return same value)
    ASSERT(asfloat(5.5) == 5.5);
    ASSERT(asfloat(-3.25) == -3.25);
    
    // Vector conversion
    int3 iv = int3(0x3F800000, 0x40000000, 0xBF800000);
    float3 fv = asfloat(iv);
    ASSERT(fv.x == 1.0 && fv.y == 2.0 && fv.z == -1.0);
    
    // Special bit patterns
    // 0x7F800000 is positive infinity
    int inf_bits = 0x7F800000;
    ASSERT(isinf(asfloat(inf_bits)));
    
    // 0xFF800000 is negative infinity
    int neg_inf_bits = 0xFF800000;
    ASSERT(isinf(asfloat(neg_inf_bits)));
    
    // 0x7FC00000 is a NaN pattern
    int nan_bits = 0x7FC00000;
    ASSERT(isnan(asfloat(nan_bits)));
}

[Test]
void Intrinsic_Asint()
{
    // Convert float to int (bit pattern reinterpretation)
    float f1 = 1.0;
    ASSERT(asint(f1) == 0x3F800000);
    
    float f2 = 2.0;
    ASSERT(asint(f2) == 0x40000000);
    
    float f3 = -1.0;
    ASSERT(asint(f3) == 0xBF800000);
    
    float f4 = 0.0;
    ASSERT(asint(f4) == 0x00000000);
    
    // Convert uint to int (preserves bit pattern)
    uint u1 = 0x7FFFFFFF;
    ASSERT(asint(u1) == 0x7FFFFFFF);
    
    uint u2 = 0x80000000u;
    ASSERT(asint(u2) == -2147483648); // Sign bit set
    
    uint u3 = 0x00000000u;
    ASSERT(asint(u3) == 0);
    
    uint u4 = 0xFFFFFFFFu;
    ASSERT(asint(u4) == -1);
    
    // Test with int input (should return same value)
    ASSERT(asint(42) == 42);
    ASSERT(asint(-42) == -42);
    ASSERT(asint(0) == 0);
    
    // Vector conversion
    float3 fv = float3(1.0, 2.0, -1.0);
    int3 iv = asint(fv);
    ASSERT(iv.x == 0x3F800000 && iv.y == 0x40000000 && iv.z == 0xBF800000);
    
    // Mixed uint vector
    uint3 uv = uint3(0x00000001u, 0x7FFFFFFFu, 0x80000000u);
    int3 iv2 = asint(uv);
    ASSERT(iv2.x == 1 && iv2.y == 2147483647 && iv2.z == -2147483648);
}

[Test]
void Intrinsic_Asuint()
{
    // Convert float to uint (bit pattern reinterpretation)
    float f1 = 1.0;
    ASSERT(asuint(f1) == 0x3F800000u);
    
    float f2 = 2.0;
    ASSERT(asuint(f2) == 0x40000000u);
    
    float f3 = -1.0;
    ASSERT(asuint(f3) == 0xBF800000u);
    
    float f4 = 0.0;
    ASSERT(asuint(f4) == 0x00000000u);
    
    // Convert int to uint (preserves bit pattern)
    int i1 = -1;
    ASSERT(asuint(i1) == 0xFFFFFFFFu);
    
    int i2 = -2147483648;
    ASSERT(asuint(i2) == 0x80000000u);
    
    int i3 = 2147483647;
    ASSERT(asuint(i3) == 0x7FFFFFFFu);
    
    int i4 = 0;
    ASSERT(asuint(i4) == 0u);
    
    // Positive int
    int i5 = 42;
    ASSERT(asuint(i5) == 42u);
    
    // Test with uint input (should return same value)
    ASSERT(asuint(42u) == 42u);
    ASSERT(asuint(0u) == 0u);
    ASSERT(asuint(0xFFFFFFFFu) == 0xFFFFFFFFu);
    
    // Vector conversion
    float3 fv = float3(1.0, 2.0, -1.0);
    uint3 uv = asuint(fv);
    ASSERT(uv.x == 0x3F800000u && uv.y == 0x40000000u && uv.z == 0xBF800000u);
    
    // Mixed int vector
    int3 iv = int3(1, -1, 0);
    uint3 uv2 = asuint(iv);
    ASSERT(uv2.x == 1u && uv2.y == 0xFFFFFFFFu && uv2.z == 0u);
    
    // Verify round-trip conversion
    float original = 3.14159;
    uint bits = asuint(original);
    float restored = asfloat(bits);
    ASSERT(original == restored);
}

[Test]
void Intrinsic_Asdouble()
{
    // Convert two uints to double (low bits, high bits)
    // Double representation of 1.0: 0x3FF0000000000000
    uint low1 = 0x00000000u;
    uint high1 = 0x3FF00000u;
    ASSERT(asdouble(low1, high1) == 1.0);
    
    // Double representation of 2.0: 0x4000000000000000
    uint low2 = 0x00000000u;
    uint high2 = 0x40000000u;
    ASSERT(asdouble(low2, high2) == 2.0);
    
    // Double representation of -1.0: 0xBFF0000000000000
    uint low3 = 0x00000000u;
    uint high3 = 0xBFF00000u;
    ASSERT(asdouble(low3, high3) == -1.0);
    
    // Double representation of 0.0: 0x0000000000000000
    uint low4 = 0x00000000u;
    uint high4 = 0x00000000u;
    ASSERT(asdouble(low4, high4) == 0.0);
    
    // Double representation of 0.5: 0x3FE0000000000000
    uint low5 = 0x00000000u;
    uint high5 = 0x3FE00000u;
    ASSERT(asdouble(low5, high5) == 0.5);
    
    // Double representation of -0.5: 0xBFE0000000000000
    uint low6 = 0x00000000u;
    uint high6 = 0xBFE00000u;
    ASSERT(asdouble(low6, high6) == -0.5);
    
    // Test with non-zero low bits
    // 1.5: 0x3FF8000000000000
    uint low7 = 0x00000000u;
    uint high7 = 0x3FF80000u;
    ASSERT(asdouble(low7, high7) == 1.5);
    
    // 3.0: 0x4008000000000000
    uint low8 = 0x00000000u;
    uint high8 = 0x40080000u;
    ASSERT(asdouble(low8, high8) == 3.0);
    
    // Special values - positive infinity: 0x7FF0000000000000
    uint low_inf = 0x00000000u;
    uint high_inf = 0x7FF00000u;
    ASSERT(isinf(asdouble(low_inf, high_inf)));
    
    // Negative infinity: 0xFFF0000000000000
    uint low_neginf = 0x00000000u;
    uint high_neginf = 0xFFF00000u;
    ASSERT(isinf(asdouble(low_neginf, high_neginf)));
    
    // NaN: 0x7FF8000000000000
    uint low_nan = 0x00000000u;
    uint high_nan = 0x7FF80000u;
    ASSERT(isnan(asdouble(low_nan, high_nan)));
    
    // Small positive number
    uint low9 = 0x00000000u;
    uint high9 = 0x3F800000u;
    double small = asdouble(low9, high9);
    ASSERT(small > 0.0 && small < 1.0);
}

[Test]
void Intrinsic_Msad4()
{
    // Test 1: Zero accumulator, identical values (should produce all zeros)
    uint reference = 0x01020304u;
    uint2 source = uint2(0x01020304u, 0x05060708u);
    uint4 accum = uint4(0, 0, 0, 0);
    uint4 result = msad4(reference, source, accum);
    // First position: bytes match exactly
    ASSERT(result.x == 0);
    
    // Test 2: Zero accumulator, different values
    reference = 0x00000000u;
    source = uint2(0x01020304u, 0x05060708u);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // First position: |0-1| + |0-2| + |0-3| + |0-4| = 10
    ASSERT(result.x == 10);
    // Second position: |0-2| + |0-3| + |0-4| + |0-5| = 14
    ASSERT(result.y == 14);
    // Third position: |0-3| + |0-4| + |0-5| + |0-6| = 18
    ASSERT(result.z == 18);
    // Fourth position: |0-4| + |0-5| + |0-6| + |0-7| = 22
    ASSERT(result.w == 22);
    
    // Test 3: Non-zero accumulator
    reference = 0x00000000u;
    source = uint2(0x01010101u, 0x01010101u);
    accum = uint4(10, 20, 30, 40);
    result = msad4(reference, source, accum);
    // Each position: |0-1| + |0-1| + |0-1| + |0-1| = 4, plus accumulator
    ASSERT(result.x == 14);
    ASSERT(result.y == 24);
    ASSERT(result.z == 34);
    ASSERT(result.w == 44);
    
    // Test 4: All bytes same in reference and source
    reference = 0x05050505u;
    source = uint2(0x05050505u, 0x05050505u);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    ASSERT(result.x == 0);
    ASSERT(result.y == 0);
    ASSERT(result.z == 0);
    ASSERT(result.w == 0);
    
    // Test 5: Maximum difference per byte (255)
    reference = 0x00000000u;
    source = uint2(0xFFFFFFFFu, 0xFFFFFFFFu);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // Each position: |0-255| * 4 = 1020
    ASSERT(result.x == 1020);
    ASSERT(result.y == 1020);
    ASSERT(result.z == 1020);
    ASSERT(result.w == 1020);
    
    // Test 6: Mixed byte values - gradient pattern
    // Note: 0x00204060 unpacks as bytes [0x60, 0x40, 0x20, 0x00] (little-endian)
    reference = 0x80808080u; // All bytes are 128
    source = uint2(0x00204060u, 0x80A0C0E0u);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // source bytes: [0x60, 0x40, 0x20, 0x00, 0xE0, 0xC0, 0xA0, 0x80]
    //                 96    64    32     0   224   192   160   128
    // Position 0: |128-96| + |128-64| + |128-32| + |128-0| = 32 + 64 + 96 + 128 = 320
    ASSERT(result.x == 320);
    // Position 1: |128-64| + |128-32| + |128-0| + |128-224| = 64 + 96 + 128 + 96 = 384
    ASSERT(result.y == 384);
    // Position 2: |128-32| + |128-0| + |128-224| + |128-192| = 96 + 128 + 96 + 64 = 384
    ASSERT(result.z == 384);
    // Position 3: |128-0| + |128-224| + |128-192| + |128-160| = 128 + 96 + 64 + 32 = 320
    ASSERT(result.w == 320);
    
    // Test 7: Alternating high/low bytes
    reference = 0xFF00FF00u;
    source = uint2(0x00FF00FFu, 0xFF00FF00u);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // Position 0: |0-255| + |255-0| + |0-255| + |255-0| = 255 + 255 + 255 + 255 = 1020
    ASSERT(result.x == 1020);
    // Position 1: |0-0| + |255-255| + |0-0| + |255-0| = 0 + 0 + 0 + 255 = 255
    ASSERT(result.y == 255);
    // Position 2: |0-255| + |255-0| + |0-0| + |255-255| = 255 + 255 + 0 + 0 = 510
    ASSERT(result.z == 510);
    // Position 3: |0-0| + |255-0| + |0-255| + |255-0| = 0 + 255 + 255 + 255 = 765
    ASSERT(result.w == 765);
    
    // Test 8: Single byte differences
    reference = 0x01010101u;
    source = uint2(0x02020202u, 0x02020202u);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // Each position: |1-2| * 4 = 4
    ASSERT(result.x == 4);
    ASSERT(result.y == 4);
    ASSERT(result.z == 4);
    ASSERT(result.w == 4);
    
    // Test 9: Asymmetric source pattern
    // reference = 0x10203040 unpacks as [0x40, 0x30, 0x20, 0x10] = [64, 48, 32, 16]
    // source.x  = 0x10203040 unpacks as [0x40, 0x30, 0x20, 0x10] = [64, 48, 32, 16]
    // source.y  = 0x50607080 unpacks as [0x80, 0x70, 0x60, 0x50] = [128, 112, 96, 80]
    reference = 0x10203040u;
    source = uint2(0x10203040u, 0x50607080u);
    accum = uint4(5, 10, 15, 20);
    result = msad4(reference, source, accum);
    // Position 0: |64-64| + |48-48| + |32-32| + |16-16| + 5 = 0 + 5 = 5
    ASSERT(result.x == 5);
    // Position 1: |64-48| + |48-32| + |32-16| + |16-128| + 10 = 16+16+16+112 + 10 = 170
    ASSERT(result.y == 170);
    // Position 2: |64-32| + |48-16| + |32-128| + |16-112| + 15 = 32+32+96+96 + 15 = 271
    ASSERT(result.z == 271);
    // Position 3: |64-16| + |48-128| + |32-112| + |16-96| + 20 = 48+80+80+80 + 20 = 308
    ASSERT(result.w == 308);
    
    // Test 10: Large accumulator values
    reference = 0x00000000u;
    source = uint2(0x01010101u, 0x01010101u);
    accum = uint4(1000000, 2000000, 3000000, 4000000);
    result = msad4(reference, source, accum);
    // Each position: 4 + large accumulator
    ASSERT(result.x == 1000004);
    ASSERT(result.y == 2000004);
    ASSERT(result.z == 3000004);
    ASSERT(result.w == 4000004);
    
    // Test 11: Byte boundary wrapping behavior
    reference = 0xFEFEFEFEu;
    source = uint2(0xFFFFFFFEu, 0xFEFEFEFEu);
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // Position 0: |254-255| + |254-255| + |254-255| + |254-254| = 1+1+1+0 = 3
    ASSERT(result.x == 3);
    
    // Test 12: All different patterns across positions
    // reference = 0xAAAAAAAA unpacks as [0xAA, 0xAA, 0xAA, 0xAA] = all 170
    // source.x  = 0x55555555 unpacks as [0x55, 0x55, 0x55, 0x55] = all 85
    // source.y  = 0xAAAAAAAA unpacks as [0xAA, 0xAA, 0xAA, 0xAA] = all 170
    reference = 0xAAAAAAAAu; // 10101010 pattern
    source = uint2(0x55555555u, 0xAAAAAAAAu); // 01010101 and 10101010 patterns
    accum = uint4(0, 0, 0, 0);
    result = msad4(reference, source, accum);
    // 0xAA = 170, 0x55 = 85, difference = 85
    // Position 0: |170-85| + |170-85| + |170-85| + |170-85| = 340
    ASSERT(result.x == 340);
    // Position 1: |170-85| + |170-85| + |170-85| + |170-170| = 255
    ASSERT(result.y == 255);
    // Position 2: |170-85| + |170-85| + |170-170| + |170-170| = 170
    ASSERT(result.z == 170);
    // Position 3: |170-85| + |170-170| + |170-170| + |170-170| = 85
    ASSERT(result.w == 85);
}




///////
[Test]
void Intrinsic_All()
{
    // All true (non-zero) - scalar
    ASSERT(all(1.0) == true);
    ASSERT(all(5.0) == true);
    ASSERT(all(-1.0) == true);
    
    // Zero - scalar
    ASSERT(all(0.0) == false);
    
    // All components non-zero - vector
    ASSERT(all(float2(1.0, 2.0)) == true);
    ASSERT(all(float3(1.0, 2.0, 3.0)) == true);
    ASSERT(all(float4(-1.0, -2.0, -3.0, -4.0)) == true);
    
    // Some components zero - vector
    ASSERT(all(float2(1.0, 0.0)) == false);
    ASSERT(all(float2(0.0, 1.0)) == false);
    ASSERT(all(float3(1.0, 2.0, 0.0)) == false);
    ASSERT(all(float3(0.0, 1.0, 2.0)) == false);
    ASSERT(all(float4(1.0, 0.0, 2.0, 3.0)) == false);
    
    // All components zero - vector
    ASSERT(all(float2(0.0, 0.0)) == false);
    ASSERT(all(float3(0.0, 0.0, 0.0)) == false);
    ASSERT(all(float4(0.0, 0.0, 0.0, 0.0)) == false);
    
    // Mixed positive and negative (all non-zero)
    ASSERT(all(float3(-1.0, 2.0, -3.0)) == true);
    ASSERT(all(float4(1.0, -1.0, 1.0, -1.0)) == true);
    
    // Boolean vectors
    ASSERT(all(bool2(true, true)) == true);
    ASSERT(all(bool2(true, false)) == false);
    ASSERT(all(bool3(true, true, true)) == true);
    ASSERT(all(bool3(true, true, false)) == false);
}

[Test]
void Intrinsic_Any()
{
    // Any non-zero - scalar
    ASSERT(any(1.0) == true);
    ASSERT(any(-1.0) == true);
    
    // Zero - scalar
    ASSERT(any(0.0) == false);
    
    // At least one component non-zero - vector
    ASSERT(any(float2(1.0, 0.0)) == true);
    ASSERT(any(float2(0.0, 1.0)) == true);
    ASSERT(any(float3(0.0, 0.0, 1.0)) == true);
    ASSERT(any(float3(1.0, 0.0, 0.0)) == true);
    ASSERT(any(float4(0.0, 0.0, 0.0, 1.0)) == true);
    
    // All components non-zero - vector
    ASSERT(any(float2(1.0, 2.0)) == true);
    ASSERT(any(float3(1.0, 2.0, 3.0)) == true);
    ASSERT(any(float4(-1.0, -2.0, -3.0, -4.0)) == true);
    
    // All components zero - vector
    ASSERT(any(float2(0.0, 0.0)) == false);
    ASSERT(any(float3(0.0, 0.0, 0.0)) == false);
    ASSERT(any(float4(0.0, 0.0, 0.0, 0.0)) == false);
    
    // Mixed signs with at least one non-zero
    ASSERT(any(float3(-1.0, 0.0, 0.0)) == true);
    ASSERT(any(float4(0.0, -1.0, 0.0, 0.0)) == true);
    
    // Boolean vectors
    ASSERT(any(bool2(false, false)) == false);
    ASSERT(any(bool2(true, false)) == true);
    ASSERT(any(bool2(false, true)) == true);
    ASSERT(any(bool3(false, false, true)) == true);
}

[Test]
void Intrinsic_Log10()
{
    // Powers of 10
    ASSERT(abs(log10(1.0) - 0.0) < 0.001);
    ASSERT(abs(log10(10.0) - 1.0) < 0.001);
    ASSERT(abs(log10(100.0) - 2.0) < 0.001);
    ASSERT(abs(log10(1000.0) - 3.0) < 0.001);
    
    // Fractional powers of 10
    ASSERT(abs(log10(0.1) - (-1.0)) < 0.001);
    ASSERT(abs(log10(0.01) - (-2.0)) < 0.001);
    ASSERT(abs(log10(0.001) - (-3.0)) < 0.001);
    
    // Non-powers of 10
    ASSERT(abs(log10(2.0) - 0.301029) < 0.001);
    ASSERT(abs(log10(5.0) - 0.698970) < 0.001);
    ASSERT(abs(log10(50.0) - 1.698970) < 0.001);
    
    // Special values
    ASSERT(abs(log10(2.7182818) - 0.4342944) < 0.001);
}

[Test]
void Intrinsic_Reversebits()
{
    // Simple patterns
    ASSERT(reversebits(0u) == 0u);
    ASSERT(reversebits(0xFFFFFFFFu) == 0xFFFFFFFFu);
    
    // Single bit set
    ASSERT(reversebits(0x00000001u) == 0x80000000u);
    ASSERT(reversebits(0x80000000u) == 0x00000001u);
    ASSERT(reversebits(0x00000002u) == 0x40000000u);
    ASSERT(reversebits(0x00000004u) == 0x20000000u);
    
    // Multiple bits
    ASSERT(reversebits(0x0000000Fu) == 0xF0000000u);
    ASSERT(reversebits(0x000000FFu) == 0xFF000000u);
    ASSERT(reversebits(0x0000FFFFu) == 0xFFFF0000u);
    
    // Patterns
    ASSERT(reversebits(0x12345678u) == 0x1E6A2C48u);
    ASSERT(reversebits(0xAAAAAAAAu) == 0x55555555u);
    ASSERT(reversebits(0x55555555u) == 0xAAAAAAAAu);
    
    // Verify double reverse returns original
    uint original = 0x12345678u;
    ASSERT(reversebits(reversebits(original)) == original);
}

[Test]
void Intrinsic_Lit()
{
    // Standard case: positive n·l and n·h
    float4 result = lit(0.5, 0.8, 32.0);
    ASSERT(result.x == 1.0); // ambient is always 1
    ASSERT(result.y == 0.5); // diffuse = n·l
    ASSERT(result.z > 0.0);  // specular = (n·h)^m when n·l > 0
    ASSERT(result.w == 1.0); // w is always 1
    
    // n·l is zero
    result = lit(0.0, 0.8, 32.0);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 0.0);
    ASSERT(result.z == 0.0); // specular is 0 when n·l <= 0
    ASSERT(result.w == 1.0);
    
    // n·l is negative (backfacing)
    result = lit(-0.5, 0.8, 32.0);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 0.0); // diffuse clamped to 0
    ASSERT(result.z == 0.0); // specular is 0 when n·l <= 0
    ASSERT(result.w == 1.0);
    
    // n·h is zero (no specular highlight)
    result = lit(0.5, 0.0, 32.0);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 0.5);
    ASSERT(result.z == 0.0); // (n·h)^m = 0^32 = 0
    ASSERT(result.w == 1.0);
    
    // n·h is negative
    result = lit(0.5, -0.5, 32.0);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 0.5);
    ASSERT(result.z == 0.0); // negative n·h clamped
    ASSERT(result.w == 1.0);
    
    // Maximum values
    result = lit(1.0, 1.0, 1.0);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 1.0);
    ASSERT(result.z == 1.0); // 1^1 = 1
    ASSERT(result.w == 1.0);
    
    // Different exponents
    result = lit(1.0, 0.5, 2.0);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 1.0);
    ASSERT(abs(result.z - 0.25) < 0.001); // 0.5^2 = 0.25
    ASSERT(result.w == 1.0);
}

[Test]
void Intrinsic_Countbits()
{
    // Zero
    ASSERT(countbits(0u) == 0);
    
    // Single bit
    ASSERT(countbits(1u) == 1);
    ASSERT(countbits(2u) == 1);
    ASSERT(countbits(4u) == 1);
    ASSERT(countbits(0x80000000u) == 1);
    
    // Multiple bits
    ASSERT(countbits(3u) == 2);        // 0b11
    ASSERT(countbits(7u) == 3);        // 0b111
    ASSERT(countbits(15u) == 4);       // 0b1111
    ASSERT(countbits(0xFFu) == 8);     // 8 bits
    ASSERT(countbits(0xFFFFu) == 16);  // 16 bits
    
    // All bits set
    ASSERT(countbits(0xFFFFFFFFu) == 32);
    
    // Patterns
    ASSERT(countbits(0xAAAAAAAAu) == 16); // alternating bits
    ASSERT(countbits(0x55555555u) == 16); // alternating bits
    ASSERT(countbits(0x0F0F0F0Fu) == 16); // pattern
    
    // Specific values
    ASSERT(countbits(0x12345678u) == 13);
    
    // Powers of 2 minus 1
    ASSERT(countbits(0x7FFFFFFFu) == 31);
}

[Test]
void Intrinsic_D3DCOLORtoUBYTE4()
{
    // r0.xyzw = float4(255.001953,255.001953,255.001953,255.001953) * r0.zyxw;
    // o0.xyzw = (int4)r0.xyzw;
    
    float4 result = D3DCOLORtoUBYTE4(float4(0.0, 0.0, 0.0, 0.0));
    ASSERT(result.x == 0.0 && result.y == 0.0 && result.z == 0.0 && result.w == 0.0);
    
    result = D3DCOLORtoUBYTE4(float4(0.25, 0.5, 0.75, 1.0));
    ASSERT(result.x == 191.0 && result.y == 127.0 && result.z == 63.0 && result.w == 255.0);

    result = D3DCOLORtoUBYTE4(float4(-0.25, 0.5, 0.75, 1.0));
    ASSERT(result.x == 191.0 && result.y == 127.0 && result.z == -63.0 && result.w == 255.0);
}

[Test]
void Intrinsic_Dst()
{
    // Standard distance vector calculation
    // dst(d1, d2) = (1, d1.y * d2.y, d1.z, d2.w)
    
    // Basic case
    float4 result = dst(float4(1.0, 2.0, 3.0, 4.0), float4(5.0, 6.0, 7.0, 8.0));
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 12.0); // 2.0 * 6.0
    ASSERT(result.z == 3.0);
    ASSERT(result.w == 8.0);
    
    // With zeros
    result = dst(float4(0.0, 0.0, 0.0, 0.0), float4(1.0, 2.0, 3.0, 4.0));
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 0.0);
    ASSERT(result.z == 0.0);
    ASSERT(result.w == 4.0);
    
    // With negative values
    result = dst(float4(1.0, -2.0, 3.0, -4.0), float4(5.0, -6.0, 7.0, -8.0));
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 12.0); // -2.0 * -6.0 = 12.0
    ASSERT(result.z == 3.0);
    ASSERT(result.w == -8.0);
    
    // With ones
    result = dst(float4(1.0, 1.0, 1.0, 1.0), float4(1.0, 1.0, 1.0, 1.0));
    ASSERT(result.x == 1.0);
    ASSERT(result.y == 1.0);
    ASSERT(result.z == 1.0);
    ASSERT(result.w == 1.0);
    
    // Verify first component is always 1
    result = dst(float4(99.0, 2.0, 3.0, 4.0), float4(88.0, 6.0, 7.0, 8.0));
    ASSERT(result.x == 1.0);
}

[Test]
void Intrinsic_F16tof32()
{
    // Zero
    ASSERT(abs(f16tof32(0x0000) - 0.0) < 0.001);
    
    // One
    ASSERT(abs(f16tof32(0x3C00) - 1.0) < 0.001);
    
    // Negative one
    ASSERT(abs(f16tof32(0xBC00) - (-1.0)) < 0.001);
    
    // Two
    ASSERT(abs(f16tof32(0x4000) - 2.0) < 0.001);
    
    // Half
    ASSERT(abs(f16tof32(0x3800) - 0.5) < 0.001);
    
    // Small positive value
    ASSERT(abs(f16tof32(0x3400) - 0.25) < 0.001);
    
    // Negative values
    ASSERT(abs(f16tof32(0xC000) - (-2.0)) < 0.001);
    ASSERT(abs(f16tof32(0xB800) - (-0.5)) < 0.001);
    
    // Larger values
    ASSERT(abs(f16tof32(0x7BFF) - 65504.0) < 1.0); // max half float
    
    // Special values: infinity
    float inf = f16tof32(0x7C00);
    ASSERT(isinf(inf));
    
    // Negative infinity
    float ninf = f16tof32(0xFC00);
    ASSERT(isinf(ninf));
}

[Test]
void Intrinsic_F32tof16()
{
    // Zero
    ASSERT(f32tof16(0.0) == 0x0000);
    
    // One
    ASSERT(f32tof16(1.0) == 0x3C00);
    
    // Negative one
    ASSERT(f32tof16(-1.0) == 0xBC00);
    
    // Two
    ASSERT(f32tof16(2.0) == 0x4000);
    
    // Half
    ASSERT(f32tof16(0.5) == 0x3800);
    
    // Quarter
    ASSERT(f32tof16(0.25) == 0x3400);
    
    // Negative values
    ASSERT(f32tof16(-2.0) == 0xC000);
    ASSERT(f32tof16(-0.5) == 0xB800);
    
    // Round-trip conversion
    float original = 1.5;
    uint halff = f32tof16(original);
    float restored = f16tof32(halff);
    ASSERT(abs(restored - original) < 0.001);
    
    // Another round-trip
    original = -3.75;
    halff = f32tof16(original);
    restored = f16tof32(halff);
    ASSERT(abs(restored - original) < 0.001);
}

[Test]
void Intrinsic_Firstbithigh()
{
    // Zero has no bits set - typically returns -1
    ASSERT(firstbithigh(0u) == -1);
    
    // Single bit at MSB
    ASSERT(firstbithigh(0x80000000u) == 31);
    
    // Single bit at LSB
    ASSERT(firstbithigh(0x00000001u) == 0);
    
    // Multiple bits - should return highest
    ASSERT(firstbithigh(0xFFFFFFFFu) == 31);
    ASSERT(firstbithigh(0x0FFFFFFFu) == 27);
    ASSERT(firstbithigh(0x00FFFFFFu) == 23);
    ASSERT(firstbithigh(0x000FFFFFu) == 19);
    
    // Specific patterns
    ASSERT(firstbithigh(0x00000003u) == 1);  // 0b11
    ASSERT(firstbithigh(0x00000007u) == 2);  // 0b111
    ASSERT(firstbithigh(0x0000000Fu) == 3);  // 0b1111
    
    // Powers of 2
    ASSERT(firstbithigh(0x00000001u) == 0);
    ASSERT(firstbithigh(0x00000002u) == 1);
    ASSERT(firstbithigh(0x00000004u) == 2);
    ASSERT(firstbithigh(0x00000008u) == 3);
    ASSERT(firstbithigh(0x00000010u) == 4);
    ASSERT(firstbithigh(0x00000100u) == 8);
    ASSERT(firstbithigh(0x00010000u) == 16);
    
    // Mixed patterns
    ASSERT(firstbithigh(0x12345678u) == 28); // highest bit is bit 28
    ASSERT(firstbithigh(0x0000FFFFu) == 15);
}

[Test]
void Intrinsic_Firstbitlow()
{
    // Zero has no bits set - typically returns -1
    ASSERT(firstbitlow(0u) == -1);
    
    // Single bit at LSB
    ASSERT(firstbitlow(0x00000001u) == 0);
    
    // Single bit at MSB
    ASSERT(firstbitlow(0x80000000u) == 31);
    
    // Multiple bits - should return lowest
    ASSERT(firstbitlow(0xFFFFFFFFu) == 0);
    ASSERT(firstbitlow(0xFFFFFFFEu) == 1);
    ASSERT(firstbitlow(0xFFFFFFFCu) == 2);
    ASSERT(firstbitlow(0xFFFFFFF8u) == 3);
    
    // Powers of 2
    ASSERT(firstbitlow(0x00000001u) == 0);
    ASSERT(firstbitlow(0x00000002u) == 1);
    ASSERT(firstbitlow(0x00000004u) == 2);
    ASSERT(firstbitlow(0x00000008u) == 3);
    ASSERT(firstbitlow(0x00000010u) == 4);
    ASSERT(firstbitlow(0x00000100u) == 8);
    ASSERT(firstbitlow(0x00010000u) == 16);
    
    // Specific patterns - lowest set bit wins
    ASSERT(firstbitlow(0x00000003u) == 0);  // 0b11, lowest is bit 0
    ASSERT(firstbitlow(0x00000006u) == 1);  // 0b110, lowest is bit 1
    ASSERT(firstbitlow(0x0000000Cu) == 2);  // 0b1100, lowest is bit 2
    
    // Mixed patterns
    ASSERT(firstbitlow(0x12345678u) == 3);  // 0x78 ends in 0b1000, lowest bit is 3
    ASSERT(firstbitlow(0xFFFF0000u) == 16);
    ASSERT(firstbitlow(0x80000000u) == 31);
    
    // Even numbers have bit 0 clear
    ASSERT(firstbitlow(0x00000002u) == 1);
    ASSERT(firstbitlow(0x00000004u) == 2);
    ASSERT(firstbitlow(0x00000008u) == 3);
}

[Test]
void Intrinsic_Determinant()
{
    // 2x2 Identity matrix
    float2x2 identity2 = float2x2(1.0, 0.0, 0.0, 1.0);
    ASSERT(abs(determinant(identity2) - 1.0) < 0.001);
    
    // 2x2 matrix with determinant 0 (singular)
    float2x2 singular2 = float2x2(1.0, 2.0, 2.0, 4.0);
    ASSERT(abs(determinant(singular2) - 0.0) < 0.001);
    
    // 2x2 matrix with negative determinant
    float2x2 neg2 = float2x2(1.0, 2.0, 3.0, 4.0);
    ASSERT(abs(determinant(neg2) - (-2.0)) < 0.001); // 1*4 - 2*3 = -2
    
    // 2x2 matrix with positive determinant
    float2x2 pos2 = float2x2(4.0, 3.0, 2.0, 1.0);
    ASSERT(abs(determinant(pos2) - (-2.0)) < 0.001); // 4*1 - 3*2 = -2
    
    // 3x3 Identity matrix
    float3x3 identity3 = float3x3(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);
    ASSERT(abs(determinant(identity3) - 1.0) < 0.001);
    
    // 3x3 matrix
    float3x3 mat3 = float3x3(1.0, 2.0, 3.0, 0.0, 1.0, 4.0, 5.0, 6.0, 0.0);
    // det = 1*(1*0 - 4*6) - 2*(0*0 - 4*5) + 3*(0*6 - 1*5)
    //     = 1*(-24) - 2*(-20) + 3*(-5)
    //     = -24 + 40 - 15 = 1
    ASSERT(abs(determinant(mat3) - 1.0) < 0.001);
    
    // 3x3 singular matrix (determinant = 0)
    float3x3 singular3 = float3x3(1.0, 2.0, 3.0, 2.0, 4.0, 6.0, 1.0, 2.0, 3.0);
    ASSERT(abs(determinant(singular3) - 0.0) < 0.001);
    
    // 4x4 Identity matrix
    float4x4 identity4 = float4x4(
        1.0, 0.0, 0.0, 0.0,
        0.0, 1.0, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0
    );
    ASSERT(abs(determinant(identity4) - 1.0) < 0.001);
    
    // 4x4 diagonal matrix
    float4x4 diag4 = float4x4(
        2.0, 0.0, 0.0, 0.0,
        0.0, 3.0, 0.0, 0.0,
        0.0, 0.0, 4.0, 0.0,
        0.0, 0.0, 0.0, 5.0
    );
    ASSERT(abs(determinant(diag4) - 120.0) < 0.001); // 2*3*4*5 = 120
    
    // Scale matrix
    float2x2 scale2 = float2x2(2.0, 0.0, 0.0, 3.0);
    ASSERT(abs(determinant(scale2) - 6.0) < 0.001);
    
    // Negative scale
    float2x2 negScale = float2x2(-1.0, 0.0, 0.0, 1.0);
    ASSERT(abs(determinant(negScale) - (-1.0)) < 0.001);
}
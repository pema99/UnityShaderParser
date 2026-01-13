[Test]
void Intrinsic_Abs()
{
    ASSERT(abs(-5.0) == 5.0);
    ASSERT(abs(5.0) == 5.0);
    ASSERT(abs(0.0) == 0.0);
    ASSERT(abs(-0.5) == 0.5);
    
    float3 v = abs(float3(-1.0, 2.0, -3.0));
    ASSERT(v.x == 1.0 && v.y == 2.0 && v.z == 3.0);
}

[Test]
void Intrinsic_Acos()
{
    ASSERT(abs(acos(1.0) - 0.0) < 0.001);
    ASSERT(abs(acos(0.0) - 1.5707963) < 0.001);
    ASSERT(abs(acos(-1.0) - 3.1415926) < 0.001);
    ASSERT(abs(acos(0.5) - 1.0471975) < 0.001);
}

[Test]
void Intrinsic_Asin()
{
    ASSERT(abs(asin(0.0) - 0.0) < 0.001);
    ASSERT(abs(asin(1.0) - 1.5707963) < 0.001);
    ASSERT(abs(asin(-1.0) - (-1.5707963)) < 0.001);
    ASSERT(abs(asin(0.5) - 0.5235987) < 0.001);
}

[Test]
void Intrinsic_Atan()
{
    ASSERT(abs(atan(0.0) - 0.0) < 0.001);
    ASSERT(abs(atan(1.0) - 0.7853981) < 0.001);
    ASSERT(abs(atan(-1.0) - (-0.7853981)) < 0.001);
    ASSERT(abs(atan(1000.0) - 1.5707963) < 0.001);
}

[Test]
void Intrinsic_Atan2()
{
    ASSERT(abs(atan2(1.0, 1.0) - 0.7853981) < 0.001);
    ASSERT(abs(atan2(1.0, 0.0) - 1.5707963) < 0.001);
    ASSERT(abs(atan2(0.0, 1.0) - 0.0) < 0.001);
    ASSERT(abs(atan2(-1.0, -1.0) - (-2.3561944)) < 0.001);
}

[Test]
void Intrinsic_Ceil()
{
    ASSERT(ceil(1.5) == 2.0);
    ASSERT(ceil(-1.5) == -1.0);
    ASSERT(ceil(2.0) == 2.0);
    ASSERT(ceil(0.1) == 1.0);
    ASSERT(ceil(-0.1) == 0.0);
}

[Test]
void Intrinsic_Clamp()
{
    ASSERT(clamp(5.0, 0.0, 10.0) == 5.0);
    ASSERT(clamp(-5.0, 0.0, 10.0) == 0.0);
    ASSERT(clamp(15.0, 0.0, 10.0) == 10.0);
    ASSERT(clamp(0.5, 0.0, 1.0) == 0.5);
    
    float3 v = clamp(float3(-1.0, 0.5, 2.0), 0.0, 1.0);
    ASSERT(v.x == 0.0 && v.y == 0.5 && v.z == 1.0);
}

[Test]
void Intrinsic_Cos()
{
    ASSERT(abs(cos(0.0) - 1.0) < 0.001);
    ASSERT(abs(cos(3.1415926) - (-1.0)) < 0.001);
    ASSERT(abs(cos(1.5707963) - 0.0) < 0.001);
    ASSERT(abs(cos(-1.5707963) - 0.0) < 0.001);
}

[Test]
void Intrinsic_Cosh()
{
    ASSERT(abs(cosh(0.0) - 1.0) < 0.001);
    ASSERT(abs(cosh(1.0) - 1.5430806) < 0.001);
    ASSERT(abs(cosh(-1.0) - 1.5430806) < 0.001);
}

[Test]
void Intrinsic_Cross()
{
    float3 a = float3(1.0, 0.0, 0.0);
    float3 b = float3(0.0, 1.0, 0.0);
    float3 c = cross(a, b);
    ASSERT(c.x == 0.0 && c.y == 0.0 && c.z == 1.0);
    
    float3 d = cross(float3(1.0, 2.0, 3.0), float3(4.0, 5.0, 6.0));
    ASSERT(d.x == -3.0 && d.y == 6.0 && d.z == -3.0);
}

[Test]
void Intrinsic_Degrees()
{
    ASSERT(abs(degrees(0.0) - 0.0) < 0.001);
    ASSERT(abs(degrees(3.1415926) - 180.0) < 0.1);
    ASSERT(abs(degrees(1.5707963) - 90.0) < 0.1);
    ASSERT(abs(degrees(6.2831853) - 360.0) < 0.1);
}

[Test]
void Intrinsic_Distance()
{
    ASSERT(abs(distance(0.0, 5.0) - 5.0) < 0.001);
    ASSERT(abs(distance(float3(0.0, 0.0, 0.0), float3(3.0, 4.0, 0.0)) - 5.0) < 0.001);
    ASSERT(abs(distance(float2(1.0, 1.0), float2(4.0, 5.0)) - 5.0) < 0.001);
}

[Test]
void Intrinsic_Dot()
{
    ASSERT(dot(2.0, 3.0) == 6.0);
    ASSERT(dot(float3(1.0, 2.0, 3.0), float3(4.0, 5.0, 6.0)) == 32.0);
    ASSERT(dot(float2(1.0, 0.0), float2(0.0, 1.0)) == 0.0);
    ASSERT(dot(float3(-1.0, 2.0, -3.0), float3(2.0, -1.0, 1.0)) == -7.0);
}

[Test]
void Intrinsic_Exp()
{
    ASSERT(abs(exp(0.0) - 1.0) < 0.001);
    ASSERT(abs(exp(1.0) - 2.7182818) < 0.001);
    ASSERT(abs(exp(-1.0) - 0.3678794) < 0.001);
    ASSERT(abs(exp(2.0) - 7.3890560) < 0.001);
}

[Test]
void Intrinsic_Exp2()
{
    ASSERT(abs(exp2(0.0) - 1.0) < 0.001);
    ASSERT(abs(exp2(1.0) - 2.0) < 0.001);
    ASSERT(abs(exp2(2.0) - 4.0) < 0.001);
    ASSERT(abs(exp2(-1.0) - 0.5) < 0.001);
    ASSERT(abs(exp2(10.0) - 1024.0) < 0.1);
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
    ASSERT(floor(1.5) == 1.0);
    ASSERT(floor(-1.5) == -2.0);
    ASSERT(floor(2.0) == 2.0);
    ASSERT(floor(0.9) == 0.0);
    ASSERT(floor(-0.1) == -1.0);
}

[Test]
void Intrinsic_Fma()
{
    ASSERT(fma(2.0, 3.0, 4.0) == 10.0);
    ASSERT(fma(0.5, 2.0, 1.0) == 2.0);
    ASSERT(fma(-1.0, 5.0, 10.0) == 5.0);
}

[Test]
void Intrinsic_Fmod()
{
    ASSERT(abs(fmod(5.0, 2.0) - 1.0) < 0.001);
    ASSERT(abs(fmod(7.5, 2.5) - 0.0) < 0.001);
    ASSERT(abs(fmod(-5.0, 2.0) - (-1.0)) < 0.001);
    ASSERT(abs(fmod(5.0, -2.0) - 1.0) < 0.001);
}

[Test]
void Intrinsic_Frac()
{
    ASSERT(abs(frac(1.5) - 0.5) < 0.001);
    ASSERT(abs(frac(2.75) - 0.75) < 0.001);
    ASSERT(abs(frac(-1.5) - 0.5) < 0.001);
    ASSERT(abs(frac(5.0) - 0.0) < 0.001);
}

[Test]
void Intrinsic_Isfinite()
{
    ASSERT(isfinite(0.0));
    ASSERT(isfinite(1.0));
    ASSERT(isfinite(-1000.0));
    ASSERT(isfinite(0.00001));
}

[Test]
void Intrinsic_Isinf()
{
    ASSERT(!isinf(0.0));
    ASSERT(!isinf(1.0));
    ASSERT(!isinf(1000000.0));
    ASSERT(isinf(1.0 / 0.0));
}

[Test]
void Intrinsic_Isnan()
{
    ASSERT(!isnan(0.0));
    ASSERT(!isnan(1.0));
    ASSERT(!isnan(-1.0));
    ASSERT(isnan(0.0 / 0.0));
}

[Test]
void Intrinsic_Ldexp()
{
    ASSERT(abs(ldexp(1.0, 0) - 1.0) < 0.001);
    ASSERT(abs(ldexp(1.0, 3) - 8.0) < 0.001);
    ASSERT(abs(ldexp(2.0, 2) - 8.0) < 0.001);
    ASSERT(abs(ldexp(1.0, -2) - 0.25) < 0.001);
}

[Test]
void Intrinsic_Length()
{
    ASSERT(abs(length(5.0) - 5.0) < 0.001);
    ASSERT(abs(length(float3(3.0, 4.0, 0.0)) - 5.0) < 0.001);
    ASSERT(abs(length(float2(1.0, 1.0)) - 1.414213) < 0.001);
    ASSERT(abs(length(float3(-1.0, -2.0, -2.0)) - 3.0) < 0.001);
}

[Test]
void Intrinsic_Lerp()
{
    ASSERT(abs(lerp(0.0, 10.0, 0.5) - 5.0) < 0.001);
    ASSERT(abs(lerp(0.0, 10.0, 0.0) - 0.0) < 0.001);
    ASSERT(abs(lerp(0.0, 10.0, 1.0) - 10.0) < 0.001);
    
    float3 v = lerp(float3(0.0, 0.0, 0.0), float3(10.0, 20.0, 30.0), 0.5);
    ASSERT(v.x == 5.0 && v.y == 10.0 && v.z == 15.0);
}

[Test]
void Intrinsic_Log()
{
    ASSERT(abs(log(1.0) - 0.0) < 0.001);
    ASSERT(abs(log(2.7182818) - 1.0) < 0.001);
    ASSERT(abs(log(7.389056) - 2.0) < 0.001);
}

[Test]
void Intrinsic_Log2()
{
    ASSERT(abs(log2(1.0) - 0.0) < 0.001);
    ASSERT(abs(log2(2.0) - 1.0) < 0.001);
    ASSERT(abs(log2(8.0) - 3.0) < 0.001);
    ASSERT(abs(log2(1024.0) - 10.0) < 0.001);
}

[Test]
void Intrinsic_Mad()
{
    ASSERT(mad(2.0, 3.0, 4.0) == 10.0);
    ASSERT(mad(0.5, 2.0, 1.0) == 2.0);
    ASSERT(mad(-1.0, 5.0, 10.0) == 5.0);
}

[Test]
void Intrinsic_Max()
{
    ASSERT(max(5.0, 3.0) == 5.0);
    ASSERT(max(-5.0, -3.0) == -3.0);
    ASSERT(max(0.0, 0.0) == 0.0);
    
    float3 v = max(float3(1.0, 5.0, 3.0), float3(2.0, 4.0, 6.0));
    ASSERT(v.x == 2.0 && v.y == 5.0 && v.z == 6.0);
}

[Test]
void Intrinsic_Min()
{
    ASSERT(min(5.0, 3.0) == 3.0);
    ASSERT(min(-5.0, -3.0) == -5.0);
    ASSERT(min(0.0, 0.0) == 0.0);
    
    float3 v = min(float3(1.0, 5.0, 3.0), float3(2.0, 4.0, 6.0));
    ASSERT(v.x == 1.0 && v.y == 4.0 && v.z == 3.0);
}

[Test]
void Intrinsic_Noise()
{
    float n1 = noise(float3(0.0, 0.0, 0.0));
    float n2 = noise(float3(1.0, 2.0, 3.0));
    ASSERT(n1 >= 0.0 && n1 <= 1.0);
    ASSERT(n2 >= 0.0 && n2 <= 1.0);
}

[Test]
void Intrinsic_Normalize()
{
    float3 v1 = normalize(float3(3.0, 4.0, 0.0));
    ASSERT(abs(v1.x - 0.6) < 0.001 && abs(v1.y - 0.8) < 0.001 && abs(v1.z - 0.0) < 0.001);
    
    float3 v2 = normalize(float3(-1.0, -1.0, -1.0));
    float expected = -0.57735;
    ASSERT(abs(v2.x - expected) < 0.001 && abs(v2.y - expected) < 0.001 && abs(v2.z - expected) < 0.001);
    
    float3 v3 = normalize(float3(10.0, 0.0, 0.0));
    ASSERT(abs(v3.x - 1.0) < 0.001 && abs(v3.y - 0.0) < 0.001 && abs(v3.z - 0.0) < 0.001);
}

[Test]
void Intrinsic_Pow()
{
    ASSERT(abs(pow(2.0, 3.0) - 8.0) < 0.001);
    ASSERT(abs(pow(5.0, 0.0) - 1.0) < 0.001);
    ASSERT(abs(pow(4.0, 0.5) - 2.0) < 0.001);
    ASSERT(abs(pow(2.0, -1.0) - 0.5) < 0.001);
}

[Test]
void Intrinsic_Radians()
{
    ASSERT(abs(radians(0.0) - 0.0) < 0.001);
    ASSERT(abs(radians(180.0) - 3.1415926) < 0.001);
    ASSERT(abs(radians(90.0) - 1.5707963) < 0.001);
    ASSERT(abs(radians(360.0) - 6.2831853) < 0.001);
}

[Test]
void Intrinsic_Rcp()
{
    ASSERT(abs(rcp(2.0) - 0.5) < 0.001);
    ASSERT(abs(rcp(0.5) - 2.0) < 0.001);
    ASSERT(abs(rcp(1.0) - 1.0) < 0.001);
    ASSERT(abs(rcp(-4.0) - (-0.25)) < 0.001);
}

[Test]
void Intrinsic_Reflect()
{
    float3 i = float3(1.0, -1.0, 0.0);
    float3 n = float3(0.0, 1.0, 0.0);
    float3 r = reflect(i, n);
    ASSERT(abs(r.x - 1.0) < 0.001 && abs(r.y - 1.0) < 0.001 && abs(r.z - 0.0) < 0.001);
    
    i = float3(1.0, 0.0, 0.0);
    n = float3(-1.0, 0.0, 0.0);
    r = reflect(i, n);
    ASSERT(abs(r.x - (-1.0)) < 0.001 && abs(r.y - 0.0) < 0.001 && abs(r.z - 0.0) < 0.001);
}

[Test]
void Intrinsic_Refract()
{
    float3 i = normalize(float3(1.0, -1.0, 0.0));
    float3 n = float3(0.0, 1.0, 0.0);
    float eta = 0.66;
    float3 r = refract(i, n, eta);
    ASSERT(r.x != 0.0 || r.y != 0.0 || r.z != 0.0);
}

[Test]
void Intrinsic_Round()
{
    ASSERT(round(1.5) == 2.0);
    ASSERT(round(1.4) == 1.0);
    ASSERT(round(-1.5) == -2.0);
    ASSERT(round(2.0) == 2.0);
}

[Test]
void Intrinsic_Rsqrt()
{
    ASSERT(abs(rsqrt(4.0) - 0.5) < 0.001);
    ASSERT(abs(rsqrt(1.0) - 1.0) < 0.001);
    ASSERT(abs(rsqrt(9.0) - 0.333333) < 0.001);
    ASSERT(abs(rsqrt(0.25) - 2.0) < 0.001);
}

[Test]
void Intrinsic_Saturate()
{
    ASSERT(saturate(0.5) == 0.5);
    ASSERT(saturate(-0.5) == 0.0);
    ASSERT(saturate(1.5) == 1.0);
    ASSERT(saturate(0.0) == 0.0);
    ASSERT(saturate(1.0) == 1.0);
}

[Test]
void Intrinsic_Sign()
{
    ASSERT(sign(5.0) == 1.0);
    ASSERT(sign(-5.0) == -1.0);
    ASSERT(sign(0.0) == 0.0);
    
    float3 v = sign(float3(-1.0, 0.0, 2.0));
    ASSERT(v.x == -1.0 && v.y == 0.0 && v.z == 1.0);
}

[Test]
void Intrinsic_Sin()
{
    ASSERT(abs(sin(0.0) - 0.0) < 0.001);
    ASSERT(abs(sin(1.5707963) - 1.0) < 0.001);
    ASSERT(abs(sin(3.1415926) - 0.0) < 0.001);
    ASSERT(abs(sin(-1.5707963) - (-1.0)) < 0.001);
}

[Test]
void Intrinsic_Sinh()
{
    ASSERT(abs(sinh(0.0) - 0.0) < 0.001);
    ASSERT(abs(sinh(1.0) - 1.1752011) < 0.001);
    ASSERT(abs(sinh(-1.0) - (-1.1752011)) < 0.001);
}

[Test]
void Intrinsic_Smoothstep()
{
    ASSERT(abs(smoothstep(0.0, 1.0, 0.5) - 0.5) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 0.0) - 0.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 1.0) - 1.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, -0.5) - 0.0) < 0.001);
    ASSERT(abs(smoothstep(0.0, 1.0, 1.5) - 1.0) < 0.001);
}

[Test]
void Intrinsic_Sqrt()
{
    ASSERT(abs(sqrt(4.0) - 2.0) < 0.001);
    ASSERT(abs(sqrt(9.0) - 3.0) < 0.001);
    ASSERT(abs(sqrt(0.0) - 0.0) < 0.001);
    ASSERT(abs(sqrt(0.25) - 0.5) < 0.001);
}

[Test]
void Intrinsic_Step()
{
    ASSERT(step(0.5, 1.0) == 1.0);
    ASSERT(step(0.5, 0.5) == 1.0);
    ASSERT(step(0.5, 0.3) == 0.0);
    
    float2 v = step(float2(0.5, 0.5), float2(0.3, 0.7));
    ASSERT(v.x == 0.0 && v.y == 1.0);
}

[Test]
void Intrinsic_Tan()
{
    ASSERT(abs(tan(0.0) - 0.0) < 0.001);
    ASSERT(abs(tan(0.7853981) - 1.0) < 0.001);
    ASSERT(abs(tan(-0.7853981) - (-1.0)) < 0.001);
}

[Test]
void Intrinsic_Tanh()
{
    ASSERT(abs(tanh(0.0) - 0.0) < 0.001);
    ASSERT(abs(tanh(1.0) - 0.7615941) < 0.001);
    ASSERT(abs(tanh(-1.0) - (-0.7615941)) < 0.001);
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
    ASSERT(trunc(1.5) == 1.0);
    ASSERT(trunc(-1.5) == -1.0);
    ASSERT(trunc(2.9) == 2.0);
    ASSERT(trunc(-2.9) == -2.0);
    ASSERT(trunc(0.0) == 0.0);
}
// Shader: https://www.shadertoy.com/view/Wt33Wf
// Converted using https://pema.dev/glsl2hlsl

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

// Built-in properties
sampler2D _MainTex;   float4 _MainTex_TexelSize;
sampler2D _SecondTex; float4 _SecondTex_TexelSize;
sampler2D _ThirdTex;  float4 _ThirdTex_TexelSize;
sampler2D _FourthTex; float4 _FourthTex_TexelSize;
float4 _Mouse;
float _GammaCorrect;
float _Resolution;

// GLSL Compatability macros
#define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))
#define texelFetch(ch, uv, lod) tex2Dlod(ch, float4((uv).xy * ch##_TexelSize.xy + ch##_TexelSize.xy * 0.5, 0, lod))
#define textureLod(ch, uv, lod) tex2Dlod(ch, float4(uv, 0, lod))
#define iResolution float3(_Resolution, _Resolution, _Resolution)
#define iFrame (floor(_Time.y / 60))
#define iChannelTime float4(_Time.y, _Time.y, _Time.y, _Time.y)
#define iDate float4(2020, 6, 18, 30)
#define iSampleRate (44100)
#define iChannelResolution float4x4(                      \
    _MainTex_TexelSize.z,   _MainTex_TexelSize.w,   0, 0, \
    _SecondTex_TexelSize.z, _SecondTex_TexelSize.w, 0, 0, \
    _ThirdTex_TexelSize.z,  _ThirdTex_TexelSize.w,  0, 0, \
    _FourthTex_TexelSize.z, _FourthTex_TexelSize.w, 0, 0)

// Global access to uv data
static v2f vertex_output;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv =  v.uv;
    return o;
}

float sun(float2 uv, float battery)
{
    float val = smoothstep(0.3, 0.29, length(uv));
    float bloom = smoothstep(0.7, 0., length(uv));
    float cut = 3.*sin((uv.y+_Time.y*0.2*(battery+0.02))*100.)+clamp(uv.y*14.+1., -6., 6.);
    cut = clamp(cut, 0., 1.);
    return clamp(val*cut, 0., 1.)+bloom*0.6;
}

float grid(float2 uv, float battery)
{
    float2 size = float2(uv.y, uv.y*uv.y*0.2)*0.01;
    uv += float2(0., _Time.y*4.*(battery+0.05));
    uv = abs(frac(uv)-0.5);
    float2 lines = smoothstep(size, ((float2)0.), uv);
    lines += smoothstep(size*5., ((float2)0.), uv)*0.4*battery;
    return clamp(lines.x+lines.y, 0., 3.);
}

float dot2(in float2 v)
{
    return dot(v, v);
}

float sdTrapezoid(in float2 p, in float r1, float r2, float he)
{
    float2 k1 = float2(r2, he);
    float2 k2 = float2(r2-r1, 2.*he);
    p.x = abs(p.x);
    float2 ca = float2(p.x-min(p.x, p.y<0. ? r1 : r2), abs(p.y)-he);
    float2 cb = p-k1+k2*clamp(dot(k1-p, k2)/dot2(k2), 0., 1.);
    float s = cb.x<0.&&ca.y<0. ? -1. : 1.;
    return s*sqrt(min(dot2(ca), dot2(cb)));
}

float sdLine(in float2 p, in float2 a, in float2 b)
{
    float2 pa = p-a, ba = b-a;
    float h = clamp(dot(pa, ba)/dot(ba, ba), 0., 1.);
    return length(pa-ba*h);
}

float sdBox(in float2 p, in float2 b)
{
    float2 d = abs(p)-b;
    return length(max(d, ((float2)0)))+min(max(d.x, d.y), 0.);
}

float opSmoothUnion(float d1, float d2, float k)
{
    float h = clamp(0.5+0.5*(d2-d1)/k, 0., 1.);
    return lerp(d2, d1, h)-k*h*(1.-h);
}

float sdCloud(in float2 p, in float2 a1, in float2 b1, in float2 a2, in float2 b2, float w)
{
    float lineVal1 = sdLine(p, a1, b1);
    float lineVal2 = sdLine(p, a2, b2);
    float2 ww = float2(w*1.5, 0.);
    float2 left = max(a1+ww, a2+ww);
    float2 right = min(b1-ww, b2-ww);
    float2 boxCenter = (left+right)*0.5;
    float boxH = abs(a2.y-a1.y)*0.5;
    float boxVal = sdBox(p-boxCenter, float2(0.04, boxH))+w;
    float uniVal1 = opSmoothUnion(lineVal1, boxVal, 0.05);
    float uniVal2 = opSmoothUnion(lineVal2, boxVal, 0.05);
    return min(uniVal1, uniVal2);
}

float4 frag (v2f __vertex_output) : SV_Target
{
    vertex_output = __vertex_output;
    float4 fragColor = 0;
    float2 fragCoord = vertex_output.uv * _Resolution;
    float2 uv = (2.*fragCoord.xy-iResolution.xy)/iResolution.y;
    float battery = 1.;
    {
        float fog = smoothstep(0.1, -0.02, abs(uv.y+0.2));
        float3 col = float3(0., 0.1, 0.2);
        if (uv.y<-0.2)
        {
            uv.y = 3./(abs(uv.y+0.2)+0.05);
            uv.x *= uv.y*1.;
            float gridVal = grid(uv, battery);
            col = lerp(col, float3(1., 0.5, 1.), gridVal);
        }
        else 
        {
            float fujiD = min(uv.y*4.5-0.5, 1.);
            uv.y -= battery*1.1-0.51;
            float2 sunUV = uv;
            float2 fujiUV = uv;
            sunUV += float2(0.75, 0.2);
            col = float3(1., 0.2, 1.);
            float sunVal = sun(sunUV, battery);
            col = lerp(col, float3(1., 0.4, 0.1), sunUV.y*2.+0.2);
            col = lerp(float3(0., 0., 0.), col, sunVal);
            float fujiVal = sdTrapezoid(uv+float2(-0.75+sunUV.y*0., 0.5), 1.75+pow(uv.y*uv.y, 2.1), 0.2, 0.5);
            float waveVal = uv.y+sin(uv.x*20.+_Time.y*2.)*0.05+0.2;
            float wave_width = smoothstep(0., 0.01, waveVal);
            col = lerp(col, lerp(float3(0., 0., 0.25), float3(1., 0., 0.5), fujiD), step(fujiVal, 0.));
            col = lerp(col, float3(1., 0.5, 1.), wave_width*step(fujiVal, 0.));
            col = lerp(col, float3(1., 0.5, 1.), 1.-smoothstep(0., 0.01, abs(fujiVal)));
            col += lerp(col, lerp(float3(1., 0.12, 0.8), float3(0., 0., 0.2), clamp(uv.y*3.5+3., 0., 1.)), step(0., fujiVal));
            float2 cloudUV = uv;
            cloudUV.x = glsl_mod(cloudUV.x+_Time.y*0.1, 4.)-2.;
            float cloudTime = _Time.y*0.5;
            float cloudY = -0.5;
            float cloudVal1 = sdCloud(cloudUV, float2(0.1+sin(cloudTime+140.5)*0.1, cloudY), float2(1.05+cos(cloudTime*0.9-36.56)*0.1, cloudY), float2(0.2+cos(cloudTime*0.867+387.165)*0.1, 0.25+cloudY), float2(0.5+cos(cloudTime*0.9675-15.162)*0.09, 0.25+cloudY), 0.075);
            cloudY = -0.6;
            float cloudVal2 = sdCloud(cloudUV, float2(-0.9+cos(cloudTime*1.02+541.75)*0.1, cloudY), float2(-0.5+sin(cloudTime*0.9-316.56)*0.1, cloudY), float2(-1.5+cos(cloudTime*0.867+37.165)*0.1, 0.25+cloudY), float2(-0.6+sin(cloudTime*0.9675+665.162)*0.09, 0.25+cloudY), 0.075);
            float cloudVal = min(cloudVal1, cloudVal2);
            col = lerp(col, float3(0., 0., 0.2), 1.-smoothstep(0.075-0.0001, 0.075, cloudVal));
            col += float3(1., 1., 1.)*(1.-smoothstep(0., 0.01, abs(cloudVal-0.075)));
        }
        col += fog*fog*fog;
        col = lerp(float3(col.r, col.r, col.r)*0.5, col, battery*0.7);
        fragColor = float4(col, 1.);
    }
    if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
    return fragColor;
}
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

float map(float3 p)
{
    float3 n = float3(0, 1, 0);
    float k1 = 1.9;
    float k2 = (sin(p.x*k1)+sin(p.z*k1))*0.8;
    float k3 = (sin(p.y*k1)+sin(p.z*k1))*0.8;
    float w1 = 4.-dot(abs(p), normalize(n))+k2;
    float w2 = 4.-dot(abs(p), normalize(n.yzx))+k3;
    float s1 = length(glsl_mod(p.xy+float2(sin((p.z+p.x)*2.)*0.3, cos((p.z+p.x)*1.)*0.5), 2.)-1.)-0.2;
    float s2 = length(glsl_mod(0.5+p.yz+float2(sin((p.z+p.x)*2.)*0.3, cos((p.z+p.x)*1.)*0.3), 2.)-1.)-0.2;
    return min(w1, min(w2, min(s1, s2)));
}

float2 rot(float2 p, float a)
{
    return float2(p.x*cos(a)-p.y*sin(a), p.x*sin(a)+p.y*cos(a));
}

float4 frag (v2f __vertex_output) : SV_Target
{
    vertex_output = __vertex_output;
    float4 fragColor = 0;
    float2 fragCoord = vertex_output.uv * _Resolution;
    float time = 0.;
    float2 uv = fragCoord.xy/iResolution.xy*2.-1.;
    uv.x *= iResolution.x/iResolution.y;
    float3 dir = normalize(float3(uv, 1.));
    float3 pos = float3(0, 0, time);
    float3 col = ((float3)0.);
    float t = 0.;
    float tt = 0.;
    for (int i = 0;i<100; i++)
    {
        tt = map(pos+dir*t);
        if (tt<0.001)
            break;
                        
        t += tt*0.45;
    }
    float3 ip = pos+dir*t;
    col = ((float3)t*0.1);
    col = sqrt(col);
    fragColor = float4(0.05*t+abs(dir)*col+max(0., map(ip-0.1)-tt), 1.);
    fragColor.a = 1./(t*t*t*t);
    if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
    return fragColor;
}
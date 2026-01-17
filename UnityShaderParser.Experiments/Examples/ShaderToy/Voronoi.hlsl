
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

#define ANIMATE 
float2 hash2(float2 p)
{
    return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3))))*43758.547);
}

float3 voronoi(in float2 x)
{
    float2 ip = floor(x);
    float2 fp = frac(x);
    float2 mg, mr;
    float md = 8.;
    for (int j = -1;j<=1; j++)
    for (int i = -1;i<=1; i++)
    {
        float2 g = float2(float(i), float(j));
        float2 o = hash2(ip+g);
#ifdef ANIMATE
        o = 0.5+0.5*sin(_Time.y+6.2831*o);
#endif
        float2 r = g+o-fp;
        float d = dot(r, r);
        if (d<md)
        {
            md = d;
            mr = r;
            mg = g;
        }
                    
    }
    md = 8.;
    for (int j = -2;j<=2; j++)
    for (int i = -2;i<=2; i++)
    {
        float2 g = mg+float2(float(i), float(j));
        float2 o = hash2(ip+g);
#ifdef ANIMATE
        o = 0.5+0.5*sin(_Time.y+6.2831*o);
#endif
        float2 r = g+o-fp;
        if (dot(mr-r, mr-r)>0.00001)
            md = min(md, dot(0.5*(mr+r), normalize(r-mr)));
                        
    }
    return float3(md, mr);
}

float4 frag (v2f __vertex_output) : SV_Target
{
    vertex_output = __vertex_output;
    float4 fragColor = 0;
    float2 fragCoord = vertex_output.uv * _Resolution;
    float2 p = fragCoord/iResolution.xx;
    float3 c = voronoi(8.*p);
    float3 col = c.x*(0.5+0.5*sin(64.*c.x))*((float3)1.);
    col = lerp(float3(1., 0.6, 0.), col, smoothstep(0.04, 0.07, c.x));
    float dd = length(c.yz);
    col = lerp(float3(1., 0.6, 0.1), col, smoothstep(0., 0.12, dd));
    col += float3(1., 0.6, 0.1)*(1.-smoothstep(0., 0.04, dd));
    fragColor = float4(col, 1.);
    if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
    return fragColor;
}
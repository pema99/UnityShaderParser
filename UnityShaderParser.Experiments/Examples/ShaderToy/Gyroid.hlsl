// Shader: https://www.shadertoy.com/view/tXtyW8
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

#define FAR 30.
#define PI 3.1415
static int m = 0;
float2x2 rot(float a)
{
    float c = cos(a), s = sin(a);
    return transpose(float2x2(c, -s, s, c));
}

float3x3 lookAt(float3 dir)
{
    float3 up = float3(0., 1., 0.);
    float3 rt = normalize(cross(dir, up));
    return transpose(float3x3(rt, cross(rt, dir), dir));
}

float gyroid(float3 p)
{
    return dot(cos(p), sin(p.zxy))+1.;
}

float map(float3 p)
{
    float r = 100000., d;
    d = gyroid(p);
    if (d<r)
    {
        r = d;
        m = 1;
    }
                
    d = gyroid(p-float3(0, 0, PI));
    if (d<r)
    {
        r = d;
        m = 2;
    }
                
    return r;
}

float raymarch(float3 ro, float3 rd)
{
    float t = 0.;
    for (int i = 0;i<150; i++)
    {
        float d = map(ro+rd*t);
        if (abs(d)<0.001)
            break;
                        
        t += d;
        if (t>FAR)
            break;
                        
    }
    return t;
}

float getAO(float3 p, float3 sn)
{
    float occ = 0.;
    for (float i = 0.;i<4.; i++)
    {
        float t = i*0.08;
        float d = map(p+sn*t);
        occ += t-d;
    }
    return clamp(1.-occ, 0., 1.);
}

float3 getNormal(float3 p)
{
    float2 e = float2(0.5773, -0.5773)*0.001;
    return normalize(e.xyy*map(p+e.xyy)+e.yyx*map(p+e.yyx)+e.yxy*map(p+e.yxy)+e.xxx*map(p+e.xxx));
}

float3 trace(float3 ro, float3 rd)
{
    float3 C = ((float3)0);
    float3 throughput = ((float3)1);
    for (int bounce = 0;bounce<2; bounce++)
    {
        float d = raymarch(ro, rd);
        if (d>FAR)
        {
            break;
        }
                    
        float fog = 1.-exp(-0.008*d*d);
        C += throughput*fog*((float3)0);
        throughput *= 1.-fog;
        float3 p = ro+rd*d;
        float3 sn = normalize(getNormal(p)+pow(abs(cos(p*64.)), ((float3)16))*0.1);
        float3 lp = float3(10., -10., -10.+ro.z);
        float3 ld = normalize(lp-p);
        float diff = max(0., 0.5+2.*dot(sn, ld));
        float diff2 = pow(length(sin(sn*2.)*0.5+0.5), 2.);
        float diff3 = max(0., 0.5+0.5*dot(sn, float2(1, 0).yyx));
        float spec = max(0., dot(reflect(-ld, sn), -rd));
        float fres = 1.-max(0., dot(-rd, sn));
        float3 col = ((float3)0), alb = ((float3)0);
        col += float3(0.4, 0.6, 0.9)*diff;
        col += float3(0.5, 0.1, 0.1)*diff2;
        col += float3(0.9, 0.1, 0.4)*diff3;
        col += float3(0.3, 0.25, 0.25)*pow(spec, 4.)*8.;
        float freck = dot(cos(p*23.), ((float3)1));
        if (m==1)
        {
            alb = float3(0.2, 0.1, 0.9);
            alb *= max(0.6, step(2.5, freck));
        }
                    
        if (m==2)
        {
            alb = float3(0.6, 0.3, 0.1);
            alb *= max(0.8, step(-2.5, freck));
        }
                    
        col *= alb;
        col *= getAO(p, sn);
        C += throughput*col;
        rd = reflect(rd, sn);
        ro = p+sn*0.01;
        throughput *= 0.9*pow(fres, 1.);
    }
    return C;
}

float4 frag (v2f __vertex_output) : SV_Target
{
    vertex_output = __vertex_output;
    float4 fragColor = 0;
    float2 fragCoord = vertex_output.uv * _Resolution;
    float2 uv = (fragCoord.xy-iResolution.xy*0.5)/iResolution.y;
    float2 mo = (_Mouse.xy-iResolution.xy*0.5)/iResolution.y;
    float3 ro = float3(PI/2., 0, -_Time.y*0.5);
    float3 rd = normalize(float3(uv, -0.5));
    if (_Mouse.z>0.)
    {
        rd.zy = mul(rot(mo.y*PI),rd.zy);
        rd.xz = mul(rot(-mo.x*PI),rd.xz);
    }
    else 
    {
        rd.xy = mul(rot(sin(_Time.y*0.2)),rd.xy);
        float3 ta = float3(cos(_Time.y*0.4), sin(_Time.y*0.4), 4.);
        rd = mul(lookAt(normalize(ta)),rd);
    }
    float3 col = trace(ro, rd);
    col *= smoothstep(0., 1., 1.2-length(uv*0.9));
    col = pow(col, ((float3)0.4545));
    fragColor = float4(col, 1.);
    if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
    return fragColor;
}

struct PS_INPUT
{
    float4 TexC : TEXCOORD0;
};
SamplerState TextureSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D TextureBase;
Texture2D TextureDetail;

float4 main( PS_INPUT input ) : SV_Target
{
    float4 base = TextureBase.Sample(TextureSampler, input.TexC);
    float4 detail = TextureDetail.Sample(TextureSampler, input.TexC);
    return base * detail;
}



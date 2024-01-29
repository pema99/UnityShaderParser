// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Silent/FakeGlass"
{
	Properties
	{
		[HeaderEx(Glass Colour)] _Color("Tint Color", Color) = (1,1,1,0)
		_MainTex("Tint Texture", 2D) = "white" {}
		[Normal][HeaderEx(Material Properties)][SetKeyword(_NORMALMAP)]_BumpMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 1
		_Smoothness("Smoothness", Range(0 , 1)) = 1
		_Metallic("Metalness", Range(0 , 1)) = 0
		[HeaderEx(Reflection Properties)][Toggle(BLOOM)] _UseColourShift("Use Colour Shift", Float) = 0
		_IOR("IOR", Range(0 , 2)) = 1
		[Gamma]_Refraction("Refraction Power", Range(0 , 1)) = 0.1
		_InteriorDiffuseStrength("Interior Diffuse Strength", Range(0 , 1)) = 0.1
		[HeaderEx(Additional Properties)]_SurfaceMask("Surface Mask", 2D) = "black" {}
		_SurfaceSmoothness("Surface Smoothness ", Range(0 , 1)) = 0
		_SurfaceLevelTweak("Surface Level Tweak", Range(-1 , 1)) = 0
		_SurfaceSmoothnessTweak("Surface Smoothness Tweak", Range(-1 , 1)) = 0
		_OcclusionMap("Occlusion Map", 2D) = "white" {}
		_ShadowTransparency("Shadow Transparency", Range(0 , 1)) = 1
		[HDR][HeaderEx(Emission Properties)]_Glow("Glow Color", Color) = (0,0,0,0)
		_AlbedoToEmission("Tint Texture Emission Strength", Range(0 , 1)) = 1
		[SetKeyword(_EMISSION)]_EmissionMap("Emission Map (parallax)", 2D) = "white" {}
		_EmissionParallaxDepth("Emission Parallax Depth", Range(0 , 1)) = 0.1
		_EmissionRimPower("Emission Rim Power", Float) = 1
		[HideInInspector][NonModifiableTextureData]_DFG("DFG", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CullMode)][HeaderEx(System Options)]_CullMode("Cull Mode", Int) = 2
		[ToggleUI]_ZWrite("ZWrite (for solid glass)", Int) = 0
		[LTCGI(_LTCGI)][Toggle(_LTCGI)] _LTCGI("LTCGI", Float) = 0
		[HideInInspector] _texcoord2("", 2D) = "white" {}
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

		SubShader
		{
			Tags{ "RenderType" = "Custom"  "Queue" = "Transparent-6" "IsEmissive" = "true"  "LTCGI" = "_LTCGI" }
			Cull[_CullMode]
			ZWrite[_ZWrite]
			Blend One OneMinusSrcAlpha

			ColorMask RGB
			CGINCLUDE
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "Lighting.cginc"
			#pragma target 3.0
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature _EMISSION
			#pragma shader_feature_local BLOOM
			#pragma shader_feature_local _LTCGI
			#if defined(UNITY_PASS_FORWARDBASE)
			#include "FakeGlass_LTCGI.cginc"
			#endif
			#define ASE_USING_SAMPLING_MACROS 1
			#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
			#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
			#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex.SampleLevel(samplerTex,coord, lod)
			#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex.SampleBias(samplerTex,coord,bias)
			#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex.SampleGrad(samplerTex,coord,ddx,ddy)
			#else//ASE Sampling Macros
			#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
			#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex2Dlod(tex,float4(coord,0,lod))
			#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex2Dbias(tex,float4(coord,0,bias))
			#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex2Dgrad(tex,coord,ddx,ddy)
			#endif//ASE Sampling Macros

			#ifdef UNITY_PASS_SHADOWCASTER
				#undef INTERNAL_DATA
				#undef WorldReflectionVector
				#undef WorldNormalVector
				#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
				#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
				#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
			#endif
			struct Input
			{
				float2 uv_texcoord;
				float3 worldNormal;
				INTERNAL_DATA
				half ASEVFace : VFACE;
				float3 worldPos;
				float3 viewDir;
				float2 uv2_texcoord2;
			};

			struct SurfaceOutputCustomLightingCustom
			{
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half Metallic;
				half Smoothness;
				half Occlusion;
				half Alpha;
				Input SurfInput;
				UnityGIInput GIData;
			};

			uniform int _ZWrite;
			uniform int _CullMode;
			UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex);
			uniform float4 _MainTex_ST;
			SamplerState sampler_MainTex;
			uniform float4 _Color;
			uniform float _Metallic;
			UNITY_DECLARE_TEX2D_NOSAMPLER(_SurfaceMask);
			uniform float4 _SurfaceMask_ST;
			SamplerState sampler_SurfaceMask;
			uniform float _SurfaceLevelTweak;
			UNITY_DECLARE_TEX2D_NOSAMPLER(_DFG);
			UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap);
			uniform float4 _BumpMap_ST;
			SamplerState sampler_BumpMap;
			uniform float _NormalScale;
			uniform float _Smoothness;
			SamplerState sampler_DFG;
			uniform float _ShadowTransparency;
			uniform float _SurfaceSmoothness;
			uniform float _SurfaceSmoothnessTweak;
			uniform float4 _Glow;
			uniform float _AlbedoToEmission;
			uniform float _EmissionRimPower;
			UNITY_DECLARE_TEX2D_NOSAMPLER(_EmissionMap);
			uniform float _EmissionParallaxDepth;
			uniform float4 _EmissionMap_ST;
			SamplerState sampler_EmissionMap;
			uniform float _IOR;
			UNITY_DECLARE_TEX2D_NOSAMPLER(_OcclusionMap);
			uniform float4 _OcclusionMap_ST;
			SamplerState sampler_OcclusionMap;
			uniform float _Refraction;
			uniform float _InteriorDiffuseStrength;


			float SmoothnesstoPerceptualRoughness309(float smoothness)
			{
				return SmoothnessToPerceptualRoughness(smoothness);
			}


			float IsotropicNDFFiltering403(float3 normal, float roughness2)
			{
				// Tokuyoshi and Kaplanyan 2021, "Stable Geometric Specular Antialiasing with
				// Projected-Space NDF Filtering"
				float SIGMA2 = 0.15915494;
				float KAPPA = 0.18;
				float3 dndu = ddx(normal);
				float3 dndv = ddy(normal);
				float kernelRoughness2 = 2.0 * SIGMA2 * (dot(dndu, dndu) + dot(dndv, dndv));
				float clampedKernelRoughness2 = min(kernelRoughness2, KAPPA);
				float filteredRoughness2 = saturate(roughness2 + clampedKernelRoughness2);
				return filteredRoughness2;
		}


		float3 ApplySpecularDFG358(float oneMinusReflectivity, float3 specColor, float3 dfg)
		{
			half3 f0 = 0.16 * (1 - oneMinusReflectivity) + specColor;
			return lerp(dfg.xxx, dfg.yyy, f0);
		}


		float OneMinusYIfXIsNegative370(float x, float y)
		{
			return x > 0 ? y : 1 - y;
		}


		float PerceptualRoughnesstoRoughness56(float smoothness)
		{
			return PerceptualRoughnessToRoughness(smoothness);
		}


		float SpecularAO_Lagarde(float NoV, float visibility, float roughness)
		{
			return saturate(pow(NoV + visibility, exp2(-16.0 * roughness - 1.0)) - 1.0 + visibility);
		}


		inline half4 LightingStandardCustomLighting(inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi)
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 mainTex121 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_MainTex);
			float4 temp_output_123_0 = (mainTex121 * _Color);
			float4 mainTint174 = temp_output_123_0;
			half3 specColor42 = (0).xxx;
			half oneMinusReflectivity42 = 0;
			half3 diffuseAndSpecularFromMetallic42 = DiffuseAndSpecularFromMetallic(mainTint174.rgb,_Metallic,specColor42,oneMinusReflectivity42);
			float oneMinusReflectivity44 = oneMinusReflectivity42;
			float alpha37 = (temp_output_123_0).a;
			float premultipliedAlpha212 = ((1.0 - oneMinusReflectivity44) + (alpha37 * oneMinusReflectivity44));
			float2 uv_SurfaceMask = i.uv_texcoord * _SurfaceMask_ST.xy + _SurfaceMask_ST.zw;
			float4 tex2DNode96 = SAMPLE_TEXTURE2D(_SurfaceMask, sampler_SurfaceMask, uv_SurfaceMask);
			float surfaceLevel165 = saturate((tex2DNode96.r + _SurfaceLevelTweak));
			float oneMinusReflectivity358 = oneMinusReflectivity44;
			float3 specColor133 = specColor42;
			float3 specColor358 = specColor133;
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			#ifdef _NORMALMAP
				float3 staticSwitch401 = UnpackScaleNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv_BumpMap), _NormalScale);
			#else
				float3 staticSwitch401 = float3(0,0,1);
			#endif
			float3 newWorldNormal414 = (WorldNormalVector(i , staticSwitch401));
			float3 switchResult137 = (((i.ASEVFace > 0) ? (newWorldNormal414) : (-newWorldNormal414)));
			float3 worldNormal206 = switchResult137;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize(UnityWorldSpaceViewDir(ase_worldPos));
			float dotResult54 = dot(worldNormal206 , ase_worldViewDir);
			float NdotV181 = max(dotResult54 , 0.0001);
			float3 ase_worldNormal = WorldNormalVector(i, float3(0, 0, 1));
			float3 ase_vertexNormal = mul(unity_WorldToObject, float4(ase_worldNormal, 0));
			float3 normal403 = ase_vertexNormal;
			float smoothness309 = _Smoothness;
			float localSmoothnesstoPerceptualRoughness309 = SmoothnesstoPerceptualRoughness309(smoothness309);
			float roughness2403 = localSmoothnesstoPerceptualRoughness309;
			float localIsotropicNDFFiltering403 = IsotropicNDFFiltering403(normal403 , roughness2403);
			float perceptualRoughness310 = localIsotropicNDFFiltering403;
			float2 appendResult313 = (float2(NdotV181 , perceptualRoughness310));
			float4 DFG303 = SAMPLE_TEXTURE2D_LOD(_DFG, sampler_DFG, appendResult313, 0.0);
			float3 dfg358 = DFG303.rgb;
			float3 localApplySpecularDFG358 = ApplySpecularDFG358(oneMinusReflectivity358 , specColor358 , dfg358);
			float3 temp_cast_12 = (0.3333333).xxx;
			float dotResult279 = dot(localApplySpecularDFG358 , temp_cast_12);
			float fresnelStrengthGreyscale397 = dotResult279;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize(UnityWorldSpaceLightDir(ase_worldPos));
			#endif //aseld
			float dotResult203 = dot(ase_worldlightDir , worldNormal206);
			float temp_output_225_0 = (1.0 - _ShadowTransparency);
			float surfaceSmoothness166 = (_SurfaceSmoothness * saturate((_SurfaceSmoothnessTweak + tex2DNode96.r)));
			float lerpResult209 = lerp(premultipliedAlpha212 , (1.0 - surfaceSmoothness166) , surfaceLevel165);
			#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch201 = ((((lerpResult209 + (dotResult203 - -1.0) * (1.0 - lerpResult209) / (temp_output_225_0 - -1.0)) * _ShadowTransparency) + (1.0 - _ShadowTransparency)) * (1.0 - saturate(temp_output_225_0)));
			#else
				float staticSwitch201 = saturate((premultipliedAlpha212 + surfaceLevel165 + fresnelStrengthGreyscale397));
			#endif
			float finalOpacity215 = staticSwitch201;
			float temp_output_191_0 = (_IOR - 1.0);
			float3 indirectNormal266 = refract(ase_worldViewDir , worldNormal206 , temp_output_191_0);
			float smoothness60 = (1.0 - localIsotropicNDFFiltering403);
			float NoV323 = NdotV181;
			float2 uv_OcclusionMap = i.uv_texcoord * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw;
			float visibility323 = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv_OcclusionMap).g;
			float smoothness56 = perceptualRoughness310;
			float localPerceptualRoughnesstoRoughness56 = PerceptualRoughnesstoRoughness56(smoothness56);
			float roughness307 = localPerceptualRoughnesstoRoughness56;
			float roughness323 = roughness307;
			float localSpecularAO_Lagarde323 = SpecularAO_Lagarde(NoV323 , visibility323 , roughness323);
			float occlusion116 = localSpecularAO_Lagarde323;
			Unity_GlossyEnvironmentData g266 = UnityGlossyEnvironmentSetup(smoothness60, data.worldViewDir, indirectNormal266, float3(0,0,0));
			float3 indirectSpecular266 = UnityGI_IndirectSpecular(data, occlusion116, indirectNormal266, g266);
			float temp_output_241_0 = (_Refraction + 0.0);
			float3 _WavelengthRatios = float3(1,0.8,0.7);
			float3 indirectNormal1 = refract(ase_worldViewDir , worldNormal206 , (temp_output_191_0 + (temp_output_241_0 * _WavelengthRatios.x)));
			Unity_GlossyEnvironmentData g1 = UnityGlossyEnvironmentSetup(smoothness60, data.worldViewDir, indirectNormal1, float3(0,0,0));
			float3 indirectSpecular1 = UnityGI_IndirectSpecular(data, occlusion116, indirectNormal1, g1);
			float3 indirectNormal5 = refract(ase_worldViewDir , worldNormal206 , (temp_output_191_0 + (temp_output_241_0 * _WavelengthRatios.y)));
			Unity_GlossyEnvironmentData g5 = UnityGlossyEnvironmentSetup(smoothness60, data.worldViewDir, indirectNormal5, float3(0,0,0));
			float3 indirectSpecular5 = UnityGI_IndirectSpecular(data, occlusion116, indirectNormal5, g5);
			float3 indirectNormal7 = refract(ase_worldViewDir , worldNormal206 , (temp_output_191_0 + (temp_output_241_0 * _WavelengthRatios.z)));
			Unity_GlossyEnvironmentData g7 = UnityGlossyEnvironmentSetup(smoothness60, data.worldViewDir, indirectNormal7, float3(0,0,0));
			float3 indirectSpecular7 = UnityGI_IndirectSpecular(data, occlusion116, indirectNormal7, g7);
			float3 appendResult4 = (float3((indirectSpecular1).x , (indirectSpecular5).y , (indirectSpecular7).z));
			#ifdef BLOOM
				float3 staticSwitch292 = appendResult4;
			#else
				float3 staticSwitch292 = indirectSpecular266;
			#endif
			float localLTCGI15_g17 = (0.0);
			float3 worldPos15_g17 = ase_worldPos;
			float3 worldNorm15_g17 = worldNormal206;
			float3 normalizeResult12_g17 = normalize((_WorldSpaceCameraPos - ase_worldPos));
			float3 cameraDir15_g17 = normalizeResult12_g17;
			float roughness15_g17 = perceptualRoughness310;
			float2 lightmapUV15_g17 = i.uv2_texcoord2;
			float3 diffuse15_g17 = float3(0,0,0);
			float3 specular15_g17 = float3(0,0,0);
			float specularIntensity15_g17 = 0;
			{
			#if (defined(_LTCGI) && defined(UNITY_PASS_FORWARDBASE))
			LTCGI_Contribution(worldPos15_g17, worldNorm15_g17, cameraDir15_g17, roughness15_g17, lightmapUV15_g17, diffuse15_g17, specular15_g17, specularIntensity15_g17);
			#endif
			}
			#ifdef _LTCGI
				float3 staticSwitch385 = specular15_g17;
			#else
				float3 staticSwitch385 = float3(0,0,0);
			#endif
			float3 ltcgi_specular386 = staticSwitch385;
			#ifdef _LTCGI
				float staticSwitch388 = specularIntensity15_g17;
			#else
				float staticSwitch388 = 0.0;
			#endif
			float ltcgi_specularMask387 = staticSwitch388;
			float3 lerpResult395 = lerp(staticSwitch292 , ltcgi_specular386 , ltcgi_specularMask387);
			float3 specularLight261 = lerpResult395;
			float3 finalAlbedo104 = diffuseAndSpecularFromMetallic42;
			float3 normalMap87 = staticSwitch401;
			float3 indirectNormal66 = (WorldNormalVector(i , float3((normalMap87).xy ,  0.0)));
			Unity_GlossyEnvironmentData g66 = UnityGlossyEnvironmentSetup(surfaceSmoothness166, data.worldViewDir, indirectNormal66, float3(0,0,0));
			float3 indirectSpecular66 = UnityGI_IndirectSpecular(data, occlusion116, indirectNormal66, g66);
			float3 fakeTransparency301 = indirectSpecular66;
			#ifdef _LTCGI
				float3 staticSwitch379 = diffuse15_g17;
			#else
				float3 staticSwitch379 = float3(0,0,0);
			#endif
			float3 ltcgi_diffuse384 = staticSwitch379;
			UnityGI gi331 = gi;
			float3 diffNorm331 = worldNormal206;
			gi331 = UnityGI_Base(data, 1, diffNorm331);
			float3 indirectDiffuse331 = gi331.indirect.diffuse + diffNorm331 * 0.0001;
			float3 diffuseLight382 = (ltcgi_diffuse384 + indirectDiffuse331);
			float3 multipliedAlbedo343 = (finalAlbedo104 * finalOpacity215);
			c.rgb = max(((specularLight261 * localApplySpecularDFG358) + ((specColor133 + finalAlbedo104) * fakeTransparency301 * (1.0 - fresnelStrengthGreyscale397) * surfaceSmoothness166) + ((_InteriorDiffuseStrength * diffuseLight382) * fresnelStrengthGreyscale397) + (diffuseLight382 * multipliedAlbedo343)) , float3(0,0,0));
			c.a = finalOpacity215;
			return c;
		}

		inline void LightingStandardCustomLighting_GI(inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi)
		{
			s.GIData = data;
		}

		void surf(Input i , inout SurfaceOutputCustomLightingCustom o)
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 mainTex121 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_MainTex);
			float4 temp_output_123_0 = (mainTex121 * _Color);
			float4 mainTint174 = temp_output_123_0;
			half3 specColor42 = (0).xxx;
			half oneMinusReflectivity42 = 0;
			half3 diffuseAndSpecularFromMetallic42 = DiffuseAndSpecularFromMetallic(mainTint174.rgb,_Metallic,specColor42,oneMinusReflectivity42);
			float3 finalAlbedo104 = diffuseAndSpecularFromMetallic42;
			float oneMinusReflectivity44 = oneMinusReflectivity42;
			float alpha37 = (temp_output_123_0).a;
			float premultipliedAlpha212 = ((1.0 - oneMinusReflectivity44) + (alpha37 * oneMinusReflectivity44));
			float2 uv_SurfaceMask = i.uv_texcoord * _SurfaceMask_ST.xy + _SurfaceMask_ST.zw;
			float4 tex2DNode96 = SAMPLE_TEXTURE2D(_SurfaceMask, sampler_SurfaceMask, uv_SurfaceMask);
			float surfaceLevel165 = saturate((tex2DNode96.r + _SurfaceLevelTweak));
			float oneMinusReflectivity358 = oneMinusReflectivity44;
			float3 specColor133 = specColor42;
			float3 specColor358 = specColor133;
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			#ifdef _NORMALMAP
				float3 staticSwitch401 = UnpackScaleNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv_BumpMap), _NormalScale);
			#else
				float3 staticSwitch401 = float3(0,0,1);
			#endif
			float3 newWorldNormal414 = (WorldNormalVector(i , staticSwitch401));
			float3 switchResult137 = (((i.ASEVFace > 0) ? (newWorldNormal414) : (-newWorldNormal414)));
			float3 worldNormal206 = switchResult137;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize(UnityWorldSpaceViewDir(ase_worldPos));
			float dotResult54 = dot(worldNormal206 , ase_worldViewDir);
			float NdotV181 = max(dotResult54 , 0.0001);
			float3 ase_worldNormal = WorldNormalVector(i, float3(0, 0, 1));
			float3 ase_vertexNormal = mul(unity_WorldToObject, float4(ase_worldNormal, 0));
			float3 normal403 = ase_vertexNormal;
			float smoothness309 = _Smoothness;
			float localSmoothnesstoPerceptualRoughness309 = SmoothnesstoPerceptualRoughness309(smoothness309);
			float roughness2403 = localSmoothnesstoPerceptualRoughness309;
			float localIsotropicNDFFiltering403 = IsotropicNDFFiltering403(normal403 , roughness2403);
			float perceptualRoughness310 = localIsotropicNDFFiltering403;
			float2 appendResult313 = (float2(NdotV181 , perceptualRoughness310));
			float4 DFG303 = SAMPLE_TEXTURE2D_LOD(_DFG, sampler_DFG, appendResult313, 0.0);
			float3 dfg358 = DFG303.rgb;
			float3 localApplySpecularDFG358 = ApplySpecularDFG358(oneMinusReflectivity358 , specColor358 , dfg358);
			float3 temp_cast_2 = (0.3333333).xxx;
			float dotResult279 = dot(localApplySpecularDFG358 , temp_cast_2);
			float fresnelStrengthGreyscale397 = dotResult279;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize(UnityWorldSpaceLightDir(ase_worldPos));
			#endif //aseld
			float dotResult203 = dot(ase_worldlightDir , worldNormal206);
			float temp_output_225_0 = (1.0 - _ShadowTransparency);
			float surfaceSmoothness166 = (_SurfaceSmoothness * saturate((_SurfaceSmoothnessTweak + tex2DNode96.r)));
			float lerpResult209 = lerp(premultipliedAlpha212 , (1.0 - surfaceSmoothness166) , surfaceLevel165);
			#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch201 = ((((lerpResult209 + (dotResult203 - -1.0) * (1.0 - lerpResult209) / (temp_output_225_0 - -1.0)) * _ShadowTransparency) + (1.0 - _ShadowTransparency)) * (1.0 - saturate(temp_output_225_0)));
			#else
				float staticSwitch201 = saturate((premultipliedAlpha212 + surfaceLevel165 + fresnelStrengthGreyscale397));
			#endif
			float finalOpacity215 = staticSwitch201;
			float3 multipliedAlbedo343 = (finalAlbedo104 * finalOpacity215);
			o.Albedo = multipliedAlbedo343;
			float temp_output_2_0_g18 = _AlbedoToEmission;
			float temp_output_3_0_g18 = (1.0 - temp_output_2_0_g18);
			float3 appendResult7_g18 = (float3(temp_output_3_0_g18 , temp_output_3_0_g18 , temp_output_3_0_g18));
			float x370 = _EmissionRimPower;
			float y370 = NdotV181;
			float localOneMinusYIfXIsNegative370 = OneMinusYIfXIsNegative370(x370 , y370);
			float4 temp_cast_5 = (1.0).xxxx;
			float3 normalMap87 = staticSwitch401;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			float3 switchResult461 = (((i.ASEVFace > 0) ? (i.viewDir) : (-i.viewDir)));
			#ifdef _EMISSION
				float4 staticSwitch457 = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, (((normalMap87 * _EmissionParallaxDepth) + float3(uv_EmissionMap ,  0.0)) + float3((-_EmissionParallaxDepth * ((switchResult461).xy / max((switchResult461).z , 1E-05))) ,  0.0)).xy);
			#else
				float4 staticSwitch457 = temp_cast_5;
			#endif
			float4 parallaxEmission456 = staticSwitch457;
			float4 finalEmission350 = (_Glow * float4(((mainTex121.rgb * temp_output_2_0_g18) + appendResult7_g18) , 0.0) * pow(localOneMinusYIfXIsNegative370 , abs(_EmissionRimPower)) * parallaxEmission456);
			o.Emission = finalEmission350.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert(appdata_full v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				Input customInputData;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}
			half4 frag(v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT(Input, surfIN);
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3(IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z);
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT(SurfaceOutputCustomLightingCustom, o)
				surf(surfIN, o);
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				o.Alpha = LightingStandardCustomLighting(o, worldViewDir, gi).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D(_DitherMaskLOD, float3(vpos.xy * 0.25, o.Alpha * 0.9375)).a;
				clip(alphaRef - 0.01);
				SHADOW_CASTER_FRAGMENT(IN)
			}
			ENDCG
		}
		}
			Fallback "Diffuse"
					CustomEditor "SilentFakeGlass.Unity.FakeGlassInspector"
}
/*ASEBEGIN
Version=18909
421;-12;1713;2134;2385.729;1469.646;2.996125;True;False
Node;AmplifyShaderEditor.CommentaryNode;103;-4625.976,-676.8352;Inherit;False;2091.764;516.8967;Normal Map;11;28;87;206;137;415;414;401;9;402;30;153;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-4456.613,-631.2725;Inherit;False;Property;_NormalScale;Normal Scale;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-4367.977,-524.912;Inherit;False;0;9;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;402;-4003.619,-356.3694;Inherit;False;Constant;_NoNormal;No Normal;25;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;9;-4143.661,-548.9387;Inherit;True;Property;_BumpMap;Normal Map;2;1;[Normal];Create;False;0;0;0;False;2;HeaderEx(Material Properties);SetKeyword(_NORMALMAP);False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;401;-3829.989,-413.5486;Inherit;False;Property;_NormalMap;Normal Map;25;0;Create;True;0;0;0;True;0;False;0;0;0;False;_NORMALMAP;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;414;-3609.699,-408.8025;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;347;-1668.795,-1347.225;Inherit;False;1312.01;437.493;Tint texture;7;122;121;34;123;125;37;174;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.NegateNode;415;-3365.456,-326.257;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;122;-1618.795,-1208.182;Inherit;True;Property;_MainTex;Tint Texture;1;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;137;-3223.574,-411.0783;Inherit;False;2;0;FLOAT3;1,1,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;206;-3032.26,-413.6584;Inherit;False;worldNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;260;-2788.41,1596.545;Inherit;False;737.8884;309;NdotV for fresnel;7;54;57;181;258;259;315;316;;0.3960785,0.2627451,0.4862745,1;0;0
Node;AmplifyShaderEditor.ColorNode;34;-1283.203,-1121.732;Float;False;Property;_Color;Tint Color;0;0;Create;False;0;0;0;False;1;HeaderEx(Glass Colour);False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;305;-3303.035,-1352.565;Inherit;False;1590.342;443.3043;Smoothness;9;60;405;307;56;310;403;309;404;35;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-1280.01,-1211.173;Float;False;mainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;259;-2738.41,1646.545;Inherit;False;206;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;258;-2719.41,1721.545;Float;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;35;-3279.938,-1137.034;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;0;False;0;False;1;0.888;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-964.7959,-1144.182;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;404;-2911.177,-1282.805;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;54;-2520.472,1696.565;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;349;-1306.765,-501.0977;Inherit;False;949.0176;337.1766;Specular colour;6;104;133;44;42;43;346;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.CustomExpressionNode;309;-3010.547,-1133.954;Float;False;return SmoothnessToPerceptualRoughness(smoothness)@$;1;Create;1;True;smoothness;FLOAT;0;In;;Float;False;Smoothness to Perceptual Roughness;True;False;0;;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;174;-781.8152,-1066.183;Float;False;mainTint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;316;-2523.927,1809.919;Inherit;False;Constant;_1e4;1e-4;21;0;Create;True;0;0;0;False;0;False;0.0001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;403;-2666.178,-1161.805;Inherit;False;    // Tokuyoshi and Kaplanyan 2021, "Stable Geometric Specular Antialiasing with$	// Projected-Space NDF Filtering"$	float SIGMA2 = 0.15915494@$	float KAPPA = 0.18@$	float3 dndu = ddx(normal)@$	float3 dndv = ddy(normal)@$	float kernelRoughness2 = 2.0 * SIGMA2 * (dot(dndu, dndu) + dot(dndv, dndv))@$	float clampedKernelRoughness2 = min(kernelRoughness2, KAPPA)@$	float filteredRoughness2 = saturate(roughness2 + clampedKernelRoughness2)@$	return filteredRoughness2@$;1;Create;2;True;normal;FLOAT3;0,0,0;In;;Inherit;False;True;roughness2;FLOAT;0;In;;Inherit;False;IsotropicNDFFiltering;True;False;0;;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;346;-1162.329,-392.2816;Inherit;False;174;mainTint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1267.765,-309.8975;Float;False;Property;_Metallic;Metalness;6;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;257;-1707.561,1725.809;Inherit;False;1290.449;661.2965;Surface Mask;11;165;146;166;149;151;148;150;143;152;144;96;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;315;-2382.927,1789.919;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-2257.521,1787.588;Inherit;False;NdotV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DiffuseAndSpecularFromMetallicNode;42;-963.0646,-363.1975;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;3;FLOAT3;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;96;-1651.061,2166.61;Inherit;True;Property;_SurfaceMask;Surface Mask;11;0;Create;True;0;0;0;False;1;HeaderEx(Additional Properties);False;-1;None;45a4a1afa17f0cc45b970e994139c968;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;144;-1642.561,1984.809;Inherit;False;Property;_SurfaceSmoothnessTweak;Surface Smoothness Tweak;14;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;317;-1650.745,2480.897;Inherit;False;951.6936;282;Sample DFG;5;311;312;313;304;303;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.ComponentMaskNode;125;-778.7959,-1154.182;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;310;-2442.374,-1086.671;Float;False;perceptualRoughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;312;-1527.745,2553.967;Inherit;False;181;NdotV;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-580.7856,-1144.755;Float;False;alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;33;-193.1943,-599.1953;Inherit;False;829;302;Premultiplied Alpha;6;212;41;40;39;38;46;;0.3960785,0.2627451,0.4862745,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;152;-1255.061,2278.611;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-596.7476,-271.9213;Float;False;oneMinusReflectivity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;311;-1600.745,2637.967;Inherit;False;310;perceptualRoughness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-1641.561,1897.809;Inherit;False;Property;_SurfaceLevelTweak;Surface Level Tweak;13;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-158.7221,-395.8339;Inherit;False;44;oneMinusReflectivity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;313;-1345.745,2560.967;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;150;-1115.945,2286.177;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-1638.561,2072.81;Inherit;False;Property;_SurfaceSmoothness;Surface Smoothness ;12;0;Create;True;0;0;0;False;0;False;0;0.3333333;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-143.7766,-506.2184;Inherit;False;37;alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;304;-1224.051,2532.897;Inherit;True;Property;_DFG;DFG;23;1;[HideInInspector];Create;True;0;0;0;False;1;NonModifiableTextureData;False;-1;6489e9ca11fba6e4c8825c44b14636e3;6489e9ca11fba6e4c8825c44b14636e3;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;151;-1257.561,1897.809;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;116.2234,-417.2185;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-864.9457,2260.177;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;39;100.2235,-525.2184;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;-923.0514,2530.897;Inherit;False;DFG;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;-576.0316,-351.5977;Float;False;specColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;146;-1125.445,1899.375;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-691.0894,2262.734;Inherit;False;surfaceSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;412;-1369.276,0.5420251;Inherit;False;1263.235;363.6942;Specular reflection from surface;9;354;360;359;358;280;279;397;262;361;;1,0.7384022,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;268;-14.57957,1552.712;Inherit;False;1323.339;645.322;The lower surface smoothness is, the darker the shadow. The higher base opacity is, the darker the shadow.;17;208;203;209;225;217;213;214;205;207;210;211;287;288;290;328;329;327;;0.5058824,0.2705882,0.4270897,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;271.2234,-442.2184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;165;-674.2083,1914.29;Inherit;False;surfaceLevel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;41.96495,2018.231;Inherit;False;166;surfaceSmoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-1824.652,-887.5549;Inherit;False;1470.444;362.8022;Occlusion;7;117;116;115;118;323;324;325;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;269;-4645.663,-25.61247;Inherit;False;2643.855;894.5969;Refracted reflection;37;252;31;255;233;27;241;102;191;239;238;244;243;53;242;240;180;190;119;195;186;5;7;1;8;267;6;3;266;4;261;270;292;293;395;394;396;418;;0.5058824,0.2705882,0.4270897,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;410.049,-510.5813;Inherit;False;premultipliedAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;-1319.276,82.57922;Inherit;False;44;oneMinusReflectivity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;56;-2213.155,-1084.019;Float;False;return PerceptualRoughnessToRoughness(smoothness)@$;1;Create;1;True;smoothness;FLOAT;0;In;;Float;False;Perceptual Roughness to Roughness;True;False;0;;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;360;-1263.107,155.184;Inherit;False;133;specColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;354;-1255.579,232.1413;Inherit;False;303;DFG;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;458;-1019.266,-2021.799;Inherit;False;2057.626;629.4854;Parallax Emission;19;461;451;452;450;455;448;449;442;447;453;454;443;456;457;440;460;445;462;464;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;210;305.8498,2023.04;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;205;41.9798,1607.05;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;31;-4563.663,218.1876;Float;False;Property;_Refraction;Refraction Power;9;1;[Gamma];Create;False;0;0;0;False;0;False;0.1;0.09999982;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;445;-995.0829,-1554.13;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;307;-1941.982,-1090.736;Float;False;roughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;217;19.29852,1846.283;Inherit;False;Property;_ShadowTransparency;Shadow Transparency;16;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;207;72.59271,1753.017;Inherit;False;206;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;263.9944,2095.555;Inherit;False;165;surfaceLevel;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;118;-1541.891,-769.3548;Inherit;False;0;115;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;213;41.96499,1938.698;Inherit;False;212;premultipliedAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;358;-999.8654,164.957;Inherit;False;half3 f0 = 0.16 * (1-oneMinusReflectivity) + specColor@$return lerp(dfg.xxx, dfg.yyy, f0)@;3;Create;3;True;oneMinusReflectivity;FLOAT;0;In;;Inherit;False;True;specColor;FLOAT3;0,0,0;In;;Inherit;False;True;dfg;FLOAT3;0,0,0;In;;Inherit;False;Apply Specular DFG;True;False;0;;False;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;280;-722.5356,248.2356;Inherit;False;Constant;_Float1;Float 1;18;0;Create;True;0;0;0;False;0;False;0.3333333;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;209;491.5468,1958.582;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;115;-1286.299,-798.6492;Inherit;True;Property;_OcclusionMap;Occlusion Map;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;324;-978.2835,-809.9243;Inherit;False;181;NdotV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;325;-979.3165,-639.6006;Inherit;False;307;roughness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;270;-4197.476,692.0044;Inherit;False;Constant;_WavelengthRatios;WavelengthRatios;18;0;Create;True;0;0;0;False;0;False;1,0.8,0.7;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;279;-522.1447,180.8219;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.3333333;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;462;-810.9409,-1486.75;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;241;-4106.664,220.1876;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-4560.663,308.1876;Float;False;Property;_IOR;IOR;8;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;225;485.3874,1727.378;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;203;376.3474,1660.134;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;397;-388.041,173.5137;Inherit;False;fresnelStrengthGreyscale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;208;689.5927,1656.017;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0.5;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-3916.664,296.1876;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-3921.664,408.1873;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;323;-772.8943,-756.2788;Inherit;False;return saturate(pow(NoV + visibility, exp2(-16.0 * roughness - 1.0)) - 1.0 + visibility)@$;1;Create;3;True;NoV;FLOAT;0;In;;Inherit;False;True;visibility;FLOAT;0;In;;Inherit;False;True;roughness;FLOAT;0;In;;Inherit;False;SpecularAO_Lagarde;False;False;0;;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;400;552.1541,1196.416;Inherit;False;1353.029;320.3065;Opacity or shadow strength;7;168;362;399;99;106;201;215;;0.5058824,0.2705882,0.4270897,1;0;0
Node;AmplifyShaderEditor.SwitchByFaceNode;461;-688.0378,-1540.776;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;383;-4644.942,944.0049;Inherit;False;1485;289;LTCGI;9;379;377;375;384;385;386;387;388;407;;0.5058824,0.2705882,0.4270897,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;405;-2449.178,-1164.805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-3918.664,519.1873;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;191;-4275.664,298.1876;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;-2277.806,-1173.322;Float;False;smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;242;-3636.939,594.7253;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;362;641.9305,1246.416;Inherit;False;212;premultipliedAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;-3613.169,-507.9276;Float;False;normalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;464;-508.941,-1480.75;Inherit;False;False;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-561.3196,-751.5065;Float;False;occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;327;848.3892,1847.572;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;240;-3637.939,479.7252;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;53;-4371.663,586.1875;Float;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;287;750.098,2073.83;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;328;886.3892,1662.572;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;377;-4583.942,1078.005;Inherit;False;310;perceptualRoughness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;243;-3630.938,350.7253;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;669.0315,1316.751;Inherit;False;165;surfaceLevel;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;102;-3924.464,113.6872;Inherit;False;206;worldNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;375;-4594.942,994.0048;Inherit;False;206;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;399;603.1541,1402.723;Inherit;False;397;fresnelStrengthGreyscale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RefractOpVec;180;-3479.418,428.9757;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;119;-3424.168,672.4438;Inherit;False;116;occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;450;-305.5962,-1503.314;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1E-05;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;329;1031.389,1667.572;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;407;-4346.942,1053.005;Inherit;False;LTCGI_Contribution_FakeGlass;-1;;17;cb25e0c87ca877a4fa69aa7835e847e7;0;2;23;FLOAT3;0,0,0;False;21;FLOAT;0;False;3;FLOAT3;16;FLOAT;17;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;452;-515.5959,-1596.314;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RefractOpVec;190;-3484.865,545.7395;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-3427.94,753.9844;Inherit;False;60;smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RefractOpVec;186;-3472.865,313.7394;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;290;924.098,2075.83;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;453;-529.2662,-1952.799;Inherit;False;Property;_EmissionParallaxDepth;Emission Parallax Depth;20;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;447;-438.0831,-1748.13;Inherit;False;87;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;919.6127,1310.034;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;1;-3106.264,359.3875;Inherit;False;World;3;0;FLOAT3;0,0,1;False;1;FLOAT;1;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;1156.098,1686.83;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;106;1182.073,1329.703;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;393;-2862.964,970.7455;Inherit;False;845.1094;247.8146;Diffuse light;5;363;331;389;390;382;;0.5058824,0.2705882,0.4270897,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;318;-1774.851,838.4247;Inherit;False;1305.628;406.85;Fake transparency, visible through glass when surface is blurry;7;91;164;157;167;120;66;301;;0.5058824,0.2705882,0.4270897,1;0;0
Node;AmplifyShaderEditor.IndirectSpecularLight;5;-3105.607,475.3742;Inherit;False;World;3;0;FLOAT3;0,0,1;False;1;FLOAT;1;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;451;-157.5961,-1574.314;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;442;-472.2312,-1870.861;Inherit;False;0;440;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;455;-184.603,-1735.17;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;449;-172.5961,-1912.314;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;379;-3980.941,1097.005;Inherit;False;Property;_LTCGI;LTCGI;26;0;Create;True;0;0;0;True;1;LTCGI(_LTCGI);False;0;0;0;True;_LTCGI;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IndirectSpecularLight;7;-3108.607,589.3744;Inherit;False;World;3;0;FLOAT3;0,0,1;False;1;FLOAT;1;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;3;-2799.948,427.3608;Inherit;False;True;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;448;-9.083044,-1787.13;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;6;-2795.291,500.3476;Inherit;False;False;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RefractOpVec;267;-3480.094,128.9204;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;385;-3989.573,991.288;Inherit;False;Property;_LTCGI;LTCGI;24;0;Create;True;0;0;0;True;0;False;0;0;0;False;_LTCGI;Toggle;2;Key0;Key1;Fetch;False;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;454;-18.71591,-1587.501;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0.1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;384;-3790.573,1107.288;Inherit;False;ltcgi_diffuse;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;363;-2812.964,1099.753;Inherit;False;206;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;8;-2790.291,581.3478;Inherit;False;False;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;201;1379.25,1324.551;Inherit;False;Property;_Keyword0;Keyword 0;16;0;Create;True;0;0;0;False;0;False;0;0;0;False;UNITY_PASS_SHADOWCASTER;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;388;-3560.573,1053.288;Inherit;False;Property;_LTCGI;LTCGI;24;0;Create;True;0;0;0;True;0;False;0;0;0;False;_LTCGI;Toggle;2;Key0;Key1;Fetch;False;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-1726.851,990.4538;Inherit;False;87;normalMap;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;443;160.9219,-1609.609;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;342;-191.7212,-875.5432;Inherit;False;575.0001;261;Albedo with intensity;4;338;337;336;343;;0.3960785,0.2627451,0.4862745,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;-571.8936,-434.0977;Float;False;finalAlbedo;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;387;-3384.573,1062.288;Inherit;False;ltcgi_specularMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;215;1681.183,1323.831;Inherit;False;finalOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;266;-3099.447,122.2613;Inherit;False;World;3;0;FLOAT3;0,0,1;False;1;FLOAT;1;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;389;-2562.853,1020.746;Inherit;False;384;ltcgi_diffuse;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;331;-2612.516,1107.56;Inherit;False;World;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;164;-1547.209,989.3688;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;4;-2569.948,443.3608;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;386;-3805.573,1023.288;Inherit;False;ltcgi_specular;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;390;-2362.558,1080.834;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;352;-312.2547,-1357.876;Inherit;False;1352.67;453.2692;Emission;12;109;350;112;291;126;364;365;366;367;368;370;459;;0.1215686,0.3254902,0.6509804,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;394;-2551.279,616.7842;Inherit;False;386;ltcgi_specular;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;338;-150.7212,-807.5432;Inherit;False;104;finalAlbedo;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;-1267.228,1168.274;Inherit;False;116;occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;-154.7212,-726.5432;Inherit;False;215;finalOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;396;-2555.279,693.7842;Inherit;False;387;ltcgi_specularMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;292;-2439.319,267.5704;Inherit;False;Property;_UseColourShift;Use Colour Shift;7;0;Create;True;0;0;0;False;1;HeaderEx(Reflection Properties);False;0;0;0;True;BLOOM;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;-1333.629,897.4247;Inherit;False;166;surfaceSmoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;157;-1290.159,993.2687;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;440;292.5657,-1631.419;Inherit;True;Property;_EmissionMap;Emission Map (parallax);19;0;Create;False;0;0;0;False;1;SetKeyword(_EMISSION);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;460;354.6465,-1784.829;Inherit;False;Constant;_1;1;28;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;382;-2241.856,1076.565;Inherit;False;diffuseLight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;291;-276.2546,-1001.749;Inherit;False;181;NdotV;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;457;605.6846,-1687.088;Inherit;False;Property;_Emission;Emission;25;0;Create;True;0;0;0;True;0;False;0;0;0;False;_EMISSION;Toggle;2;Key0;Key1;Create;False;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;367;-278.3361,-1100.892;Inherit;False;Property;_EmissionRimPower;Emission Rim Power;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;66;-981.1762,1107.397;Inherit;False;World;3;0;FLOAT3;0,0,1;False;1;FLOAT;1;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;411;-436.7503,539.3481;Inherit;False;809.3116;293.9438;Scattered light from edge thickness;5;300;392;294;398;283;;1,0.7384022,0,1;0;0
Node;AmplifyShaderEditor.LerpOp;395;-2293.279,528.7842;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;413;-1368.7,382.762;Inherit;False;900.9224;417.212;Light refraction from interior;8;333;409;335;299;334;302;408;169;;1,0.7384022,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;336;27.27888,-774.5432;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;368;49.66408,-1100.892;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;456;802.3596,-1692.864;Inherit;False;parallaxEmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;409;-1131.537,598.6288;Inherit;False;397;fresnelStrengthGreyscale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;301;-715.2238,1147.319;Inherit;False;fakeTransparency;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;333;-1316.7,432.7619;Inherit;False;133;specColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;300;-386.7503,589.3482;Inherit;False;Property;_InteriorDiffuseStrength;Interior Diffuse Strength;10;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;392;-292.3046,673.3177;Inherit;False;382;diffuseLight;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;370;-25.33594,-1021.892;Inherit;False;return x > 0? y : 1-y@;1;Create;2;True;x;FLOAT;0;In;;Inherit;False;True;y;FLOAT;0;In;;Inherit;False;One Minus Y If X Is Negative;True;False;0;;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;335;-1318.7,525.762;Inherit;False;104;finalAlbedo;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;366;-276.3359,-1292.892;Inherit;False;Property;_AlbedoToEmission;Tint Texture Emission Strength;18;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;343;164.9631,-778.8165;Inherit;False;multipliedAlbedo;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;410;-82.57747,849.8297;Inherit;False;453.9615;270.3449;Albedo (scattered) light;3;345;391;332;;1,0.7384022,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;-276.9727,-1213.589;Inherit;False;121;mainTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;261;-2238.508,746.7654;Inherit;False;specularLight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;408;-884.5368,602.6288;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;345;-32.5775,899.8296;Inherit;False;343;multipliedAlbedo;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;459;444.4185,-1008.217;Inherit;False;456;parallaxEmission;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;262;-1056.761,50.54206;Inherit;False;261;specularLight;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;294;16.37198,600.653;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;391;-10.8349,976.3716;Inherit;False;382;diffuseLight;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;302;-943.8731,522.7171;Inherit;False;301;fakeTransparency;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;334;-1071.7,436.7619;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;109;311.5753,-1313.876;Float;False;Property;_Glow;Glow Color;17;1;[HDR];Create;False;1;;0;0;False;1;HeaderEx(Emission Properties);False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;398;-91.86502,717.2917;Inherit;False;397;fresnelStrengthGreyscale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;365;300.6643,-1134.892;Inherit;False;Lerp White To;-1;;18;047d7c189c36a62438973bad9d37b1c2;0;2;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;299;-941.2825,683.9738;Inherit;False;166;surfaceSmoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;364;215.664,-1017.892;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;664.9402,-1146.225;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-629.7783,438.0589;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;332;209.3838,985.1747;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;283;210.5614,669.3989;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;361;-540.5935,69.54175;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;372;1003.374,-267.5427;Inherit;False;400.6284;249.953;System Options;2;113;371;;0.3176471,0.5333334,0.1647059,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;256;-2793.684,1923.6;Inherit;False;1049.774;632.4728;Dithering;10;254;235;228;247;250;249;245;232;230;231;;0.3960785,0.2627451,0.4862745,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;821.6912,-1151.004;Inherit;False;finalEmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;854.1592,397.6246;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureTransformNode;117;-1775.891,-742.3548;Inherit;False;115;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.AbsOpNode;57;-2399.975,1699.125;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;-1971.666,2439.074;Inherit;False;dither;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;228;-2178.909,2189.943;Inherit;False;const float a1 = 0.75487766624669276@$const float a2 = 0.569840290998@$return frac(a1 * float(pixel.x) + a2 * float(pixel.y))@;1;Create;1;True;pixel;FLOAT2;0,0;In;;Inherit;False;R2noise;True;False;0;;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;249;-2594.538,1998.039;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;247;-2309.684,2188.6;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureTransformNode;28;-4575.976,-508.912;Inherit;False;9;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.IntNode;113;1210.002,-130.5897;Float;False;Property;_CullMode;Cull Mode;24;1;[Enum];Create;True;1;;0;1;UnityEngine.Rendering.CullMode;True;1;HeaderEx(System Options);False;2;2;False;0;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;235;-2014.452,2188.104;Inherit;False;return z >= 0.5 ? 2.-2.*z : 2.*z@;1;Create;1;True;z;FLOAT;0;In;;Inherit;False;T;True;False;0;;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;255;-4499.663,122.1875;Inherit;False;254;dither;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;418;-4289.064,438.8524;Inherit;False;IOR;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;232;-2448.91,2189.416;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;216;923.9498,278.3799;Inherit;False;215;finalOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;351;969.962,187.5759;Inherit;False;350;finalEmission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenParams;231;-2671.91,2349.416;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;230;-2711.91,2183.416;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;252;-4528.265,24.38753;Inherit;False;Property;_DitheredRefraction;Dithered Refraction;22;0;Create;True;0;0;0;False;1;ToggleUI;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;250;-2442.538,2050.039;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;-4265.664,78.18761;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;344;945.1749,103.3817;Inherit;False;343;multipliedAlbedo;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;371;1132.374,-211.5427;Float;False;Property;_ZWrite;ZWrite (for solid glass);25;0;Create;False;0;0;1;UnityEngine.Rendering.CullMode;True;1;ToggleUI;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.TimeNode;245;-2768.684,1976.6;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;170;990.654,398.5679;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1197.285,109.4671;Float;False;True;-1;2;SilentFakeGlass.Unity.FakeGlassInspector;0;0;CustomLighting;Silent/FakeGlass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;True;371;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;-6;True;Custom;;Transparent;All;16;all;True;True;True;False;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;1;1;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;1;LTCGI=_LTCGI;False;0;0;True;113;-1;0;False;-1;3;Custom;#if defined(UNITY_PASS_FORWARDBASE);False;;Custom;Include;FakeGlass_LTCGI.cginc;False;;Custom;Custom;#endif;False;;Custom;0;0;False;0.1;False;-1;0;False;-1;True;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;465;-602.0333,2480.101;Inherit;False;992.9531;102.4863;TODO: It might be nice to have inner surfaces get rougher/blurrier at grazing angles, to contrast the reflection over them. ;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;293;-2467.636,42.50848;Inherit;False;429;100;Keyword used is BLOOM to conserve keywords;0;;1,1,1,1;0;0
WireConnection;30;0;28;0
WireConnection;30;1;28;1
WireConnection;9;1;30;0
WireConnection;9;5;153;0
WireConnection;401;1;402;0
WireConnection;401;0;9;0
WireConnection;414;0;401;0
WireConnection;415;0;414;0
WireConnection;137;0;414;0
WireConnection;137;1;415;0
WireConnection;206;0;137;0
WireConnection;121;0;122;0
WireConnection;123;0;121;0
WireConnection;123;1;34;0
WireConnection;54;0;259;0
WireConnection;54;1;258;0
WireConnection;309;0;35;0
WireConnection;174;0;123;0
WireConnection;403;0;404;0
WireConnection;403;1;309;0
WireConnection;315;0;54;0
WireConnection;315;1;316;0
WireConnection;181;0;315;0
WireConnection;42;0;346;0
WireConnection;42;1;43;0
WireConnection;125;0;123;0
WireConnection;310;0;403;0
WireConnection;37;0;125;0
WireConnection;152;0;144;0
WireConnection;152;1;96;1
WireConnection;44;0;42;2
WireConnection;313;0;312;0
WireConnection;313;1;311;0
WireConnection;150;0;152;0
WireConnection;304;1;313;0
WireConnection;151;0;96;1
WireConnection;151;1;143;0
WireConnection;40;0;38;0
WireConnection;40;1;46;0
WireConnection;149;0;148;0
WireConnection;149;1;150;0
WireConnection;39;0;46;0
WireConnection;303;0;304;0
WireConnection;133;0;42;1
WireConnection;146;0;151;0
WireConnection;166;0;149;0
WireConnection;41;0;39;0
WireConnection;41;1;40;0
WireConnection;165;0;146;0
WireConnection;212;0;41;0
WireConnection;56;0;310;0
WireConnection;210;0;211;0
WireConnection;307;0;56;0
WireConnection;118;0;117;0
WireConnection;118;1;117;1
WireConnection;358;0;359;0
WireConnection;358;1;360;0
WireConnection;358;2;354;0
WireConnection;209;0;213;0
WireConnection;209;1;210;0
WireConnection;209;2;214;0
WireConnection;115;1;118;0
WireConnection;279;0;358;0
WireConnection;279;1;280;0
WireConnection;462;0;445;0
WireConnection;241;0;31;0
WireConnection;225;1;217;0
WireConnection;203;0;205;0
WireConnection;203;1;207;0
WireConnection;397;0;279;0
WireConnection;208;0;203;0
WireConnection;208;2;225;0
WireConnection;208;3;209;0
WireConnection;244;0;241;0
WireConnection;244;1;270;1
WireConnection;238;0;241;0
WireConnection;238;1;270;2
WireConnection;323;0;324;0
WireConnection;323;1;115;2
WireConnection;323;2;325;0
WireConnection;461;0;445;0
WireConnection;461;1;462;0
WireConnection;405;0;403;0
WireConnection;239;0;241;0
WireConnection;239;1;270;3
WireConnection;191;0;27;0
WireConnection;60;0;405;0
WireConnection;242;0;191;0
WireConnection;242;1;239;0
WireConnection;87;0;401;0
WireConnection;464;0;461;0
WireConnection;116;0;323;0
WireConnection;327;0;217;0
WireConnection;240;0;191;0
WireConnection;240;1;238;0
WireConnection;287;0;225;0
WireConnection;328;0;208;0
WireConnection;328;1;217;0
WireConnection;243;0;191;0
WireConnection;243;1;244;0
WireConnection;180;0;53;0
WireConnection;180;1;102;0
WireConnection;180;2;240;0
WireConnection;450;0;464;0
WireConnection;329;0;328;0
WireConnection;329;1;327;0
WireConnection;407;23;375;0
WireConnection;407;21;377;0
WireConnection;452;0;461;0
WireConnection;190;0;53;0
WireConnection;190;1;102;0
WireConnection;190;2;242;0
WireConnection;186;0;53;0
WireConnection;186;1;102;0
WireConnection;186;2;243;0
WireConnection;290;0;287;0
WireConnection;99;0;362;0
WireConnection;99;1;168;0
WireConnection;99;2;399;0
WireConnection;1;0;186;0
WireConnection;1;1;195;0
WireConnection;1;2;119;0
WireConnection;288;0;329;0
WireConnection;288;1;290;0
WireConnection;106;0;99;0
WireConnection;5;0;180;0
WireConnection;5;1;195;0
WireConnection;5;2;119;0
WireConnection;451;0;452;0
WireConnection;451;1;450;0
WireConnection;455;0;453;0
WireConnection;449;0;447;0
WireConnection;449;1;453;0
WireConnection;379;0;407;0
WireConnection;7;0;190;0
WireConnection;7;1;195;0
WireConnection;7;2;119;0
WireConnection;3;0;1;0
WireConnection;448;0;449;0
WireConnection;448;1;442;0
WireConnection;6;0;5;0
WireConnection;267;0;53;0
WireConnection;267;1;102;0
WireConnection;267;2;191;0
WireConnection;385;0;407;16
WireConnection;454;0;455;0
WireConnection;454;1;451;0
WireConnection;384;0;379;0
WireConnection;8;0;7;0
WireConnection;201;1;106;0
WireConnection;201;0;288;0
WireConnection;388;0;407;17
WireConnection;443;0;448;0
WireConnection;443;1;454;0
WireConnection;104;0;42;0
WireConnection;387;0;388;0
WireConnection;215;0;201;0
WireConnection;266;0;267;0
WireConnection;266;1;195;0
WireConnection;266;2;119;0
WireConnection;331;0;363;0
WireConnection;164;0;91;0
WireConnection;4;0;3;0
WireConnection;4;1;6;0
WireConnection;4;2;8;0
WireConnection;386;0;385;0
WireConnection;390;0;389;0
WireConnection;390;1;331;0
WireConnection;292;1;266;0
WireConnection;292;0;4;0
WireConnection;157;0;164;0
WireConnection;440;1;443;0
WireConnection;382;0;390;0
WireConnection;457;1;460;0
WireConnection;457;0;440;0
WireConnection;66;0;157;0
WireConnection;66;1;167;0
WireConnection;66;2;120;0
WireConnection;395;0;292;0
WireConnection;395;1;394;0
WireConnection;395;2;396;0
WireConnection;336;0;338;0
WireConnection;336;1;337;0
WireConnection;368;0;367;0
WireConnection;456;0;457;0
WireConnection;301;0;66;0
WireConnection;370;0;367;0
WireConnection;370;1;291;0
WireConnection;343;0;336;0
WireConnection;261;0;395;0
WireConnection;408;0;409;0
WireConnection;294;0;300;0
WireConnection;294;1;392;0
WireConnection;334;0;333;0
WireConnection;334;1;335;0
WireConnection;365;1;126;0
WireConnection;365;2;366;0
WireConnection;364;0;370;0
WireConnection;364;1;368;0
WireConnection;112;0;109;0
WireConnection;112;1;365;0
WireConnection;112;2;364;0
WireConnection;112;3;459;0
WireConnection;169;0;334;0
WireConnection;169;1;302;0
WireConnection;169;2;408;0
WireConnection;169;3;299;0
WireConnection;332;0;391;0
WireConnection;332;1;345;0
WireConnection;283;0;294;0
WireConnection;283;1;398;0
WireConnection;361;0;262;0
WireConnection;361;1;358;0
WireConnection;350;0;112;0
WireConnection;100;0;361;0
WireConnection;100;1;169;0
WireConnection;100;2;283;0
WireConnection;100;3;332;0
WireConnection;57;0;54;0
WireConnection;254;0;235;0
WireConnection;228;0;247;0
WireConnection;249;0;245;2
WireConnection;247;0;232;0
WireConnection;247;1;250;0
WireConnection;235;0;228;0
WireConnection;418;0;191;0
WireConnection;232;0;230;0
WireConnection;232;1;231;0
WireConnection;250;0;249;0
WireConnection;233;0;255;0
WireConnection;233;1;31;0
WireConnection;233;2;252;0
WireConnection;170;0;100;0
WireConnection;0;0;344;0
WireConnection;0;2;351;0
WireConnection;0;9;216;0
WireConnection;0;13;170;0
ASEEND*/
//CHKSM=65D7E6F9C4B29073FDE09DC0FCC749C7A2E881CA

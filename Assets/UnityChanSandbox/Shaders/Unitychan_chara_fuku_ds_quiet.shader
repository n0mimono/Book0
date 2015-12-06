Shader "UnityChan/Clothing - Double-sided-quiet"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_ShadowColor ("Shadow Color", Color) = (0.8, 0.8, 1, 1)
		_SpecularPower ("Specular Power", Float) = 20
		_EdgeThickness ("Outline Thickness", Float) = 1

		_MainTex ("Diffuse", 2D) = "white" {}
		_FalloffSampler ("Falloff Control", 2D) = "white" {}
		_RimLightSampler ("RimLight Control", 2D) = "black" {}
		_SpecularReflectionSampler ("Specular / Reflection Mask", 2D) = "white" {}
		_EnvMapSampler ("Environment Map", 2D) = "" {} 
		_NormalMapSampler ("Normal Map", 2D) = "" {} 
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
			"LightMode"="ForwardBase"
		}		

		Pass
		{
			Cull Off
			ZTest LEqual
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag_nonspec
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#define ENABLE_NORMAL_MAP
			#include "Assets/UnityChan/Models/UnityChanShader/Shader/CharaMain.cg"

	        // Fragment shader
	        float4 frag_nonspec( v2f i ) : COLOR
	        {
	            float4_t diffSamplerColor = tex2D( _MainTex, i.uv.xy );
	            #ifdef ENABLE_NORMAL_MAP
	            	float3_t normalVec = GetNormalFromMap( i );
	            #else
	            	float3_t normalVec = i.normal;
	           	#endif

	          	// Falloff. Convert the angle between the normal and the camera direction into a lookup for the gradient
	          	float_t normalDotEye = dot( normalVec, i.eyeDir.xyz );
				float_t falloffU = clamp( 1.0 - abs( normalDotEye ), 0.02, 0.98 );
				float4_t falloffSamplerColor = FALLOFF_POWER * tex2D( _FalloffSampler, float2( falloffU, 0.25f ) );
				float3_t shadowColor = diffSamplerColor.rgb * diffSamplerColor.rgb;
				float3_t combinedColor = lerp( diffSamplerColor.rgb, shadowColor, falloffSamplerColor.r );
				combinedColor *= ( 1.0 + falloffSamplerColor.rgb * falloffSamplerColor.a );

				// Specular
				// Use the eye vector as the light vector
				float4_t reflectionMaskColor = tex2D( _SpecularReflectionSampler, i.uv.xy );
				float_t specularDot = dot( normalVec, i.eyeDir.xyz );
				float4_t lighting = lit( normalDotEye, specularDot, _SpecularPower );
				float3_t specularColor = saturate( lighting.z ) * reflectionMaskColor.rgb * diffSamplerColor.rgb;
				combinedColor += specularColor;

				// Reflection
				float3_t reflectVector = reflect( -i.eyeDir.xyz, normalVec ).xzy;
				float2_t sphereMapCoords = 0.5 * ( float2_t( 1.0, 1.0 ) + reflectVector.xy );
				float3_t reflectColor = tex2D( _EnvMapSampler, sphereMapCoords ).rgb;
				reflectColor = GetOverlayColor( reflectColor, combinedColor );

				combinedColor = lerp( combinedColor, reflectColor, reflectionMaskColor.a );
				combinedColor *= _Color.rgb * _LightColor0.rgb;
				float opacity = diffSamplerColor.a * _Color.a * _LightColor0.a;

                #ifdef ENABLE_CAST_SHADOWS
				    // Cast shadows
				    shadowColor = _ShadowColor.rgb * combinedColor;
				    float_t attenuation = saturate( 2.0 * LIGHT_ATTENUATION( i ) - 1.0 );
				    combinedColor = lerp( shadowColor, combinedColor, attenuation );
				#endif

				// Rimlight
				float_t rimlightDot = saturate( 0.5 * ( dot( normalVec, i.lightDir ) + 1.0 ) );
				falloffU = saturate( rimlightDot * falloffU );
				falloffU = tex2D( _RimLightSampler, float2( falloffU, 0.25f ) ).r;
				float3_t lightColor = diffSamplerColor.rgb; // * 2.0;
				combinedColor += falloffU * lightColor;

				return float4( combinedColor, opacity );
			}
			ENDCG
		}

		Pass
		{
			Cull Front
			ZTest Less
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets/UnityChan/Models/UnityChanShader/Shader/CharaOutline.cg"
			ENDCG
		}

	}

	FallBack "Transparent/Cutout/Diffuse"
}

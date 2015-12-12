Shader "Standard/Water" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}

    _Metallic ("Metallic", Range(0, 1)) = 0
    _Gloss ("Gloss", Range(0, 1)) = 1

	[NoScaleOffset] _BumpMap0 ("Normal Map 0", 2D) = "bump" {}
	[NoScaleOffset] _BumpMap1 ("Normal Map 1", 2D) = "bump" {}
    _WaveSpeed4 ("Wave Speed 4", Vector) = (1,1,-1,-1)
    _WaveScale4 ("Wave Scale 4", Vector) = (1,1,1,1)

    _CubeMipResolution("Cupe Mip Resolution", Range(0.01, 10)) = 7
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "UnityPBSLighting.cginc"
    #include "UnityStandardBRDF.cginc"

    half4 _Color;
    sampler2D _MainTex; half4 _MainTex_ST;

    uniform half _Metallic;
    uniform half _Gloss;

    uniform sampler2D _BumpMap0;
    uniform sampler2D _BumpMap1;
    uniform half4 _WaveSpeed4;
    uniform half4 _WaveScale4;
    uniform half _CubeMipResolution;

    struct v2f {
      float4 pos      : SV_POSITION;
      float2 uv       : TEXCOORD0;
      float3 worldPos : TEXCOORD1;
      float3 normal   : TEXCOORD2;
      float2 bumpuv0  : TEXCOORD3;
      float2 bumpuv1  : TEXCOORD4;
      LIGHTING_COORDS(5,6)
      UNITY_FOG_COORDS(7)
    };

    v2f vert(appdata_full v) {
      v2f o;

      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
      o.worldPos = mul(_Object2World, v.vertex).xyz;
      o.normal = v.normal;

      float4 temp = (o.worldPos.xzxz + _WaveSpeed4 * _Time.x) * _WaveScale4;
      o.bumpuv0 = temp.xy;
      o.bumpuv1 = temp.wz;

      TRANSFER_VERTEX_TO_FRAGMENT(o);
      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    half4 frag(v2f i) : SV_Target {

      half atten = LIGHT_ATTENUATION(i);

      half3 normalDir  = half3(0,1,0);
      half3 viewDir    = normalize(_WorldSpaceCameraPos - i.worldPos);
      half3 lightDir   = normalize(_WorldSpaceLightPos0.xyz);
      half3 halfDir    = normalize(viewDir + lightDir);
      half3 modDir     = half3(1,1,1) * _Color; // viewDir

      // normal direction
      half3 bump0 = UnpackNormal(tex2D( _BumpMap0, i.bumpuv0 )).rgb;
      half3 bump1 = UnpackNormal(tex2D( _BumpMap1, i.bumpuv1 )).rgb;
      half3 bump = normalize(bump0 + bump1);
      half3 reflectDir = reflect( -viewDir, bump );

      // gi set
      UnityGI gi;
      half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir, _CubeMipResolution);
      gi.light.dir = lightDir;
      gi.light.color = _LightColor0.rgb;
      gi.light.ndotl = LambertTerm (bump, gi.light.dir);
      gi.indirect.specular = 2.0 * rgbm.rgb;
      gi.indirect.diffuse  = half3(1,1,1);

      // diffuse basis
      half NdotL = gi.light.ndotl;

      // calc diffuse and specular
      half oneMinusReflectivity;
      half3 specColor;
      DiffuseAndSpecularFromMetallic(half3(1,1,1), _Metallic, specColor, oneMinusReflectivity);

      // calc PBS
      half4 brdf = BRDF3_Unity_PBS(half3(0,0,0), specColor, oneMinusReflectivity, _Gloss, bump, modDir, gi.light, gi.indirect);

      fixed4 col = brdf;
      //col.rgb = rgbm.rgb * 2;
      //col.rgb = reflectDir * 0.5 + 0.5;
      //col.rgb = float3(1,1,1) * gi.light.ndotl;
      //col.rgb = lightDir;
      //col.rgb = float3(1,1,1) * oneMinusReflectivity;

      UNITY_APPLY_FOG(i.fogCoord, col);
      return col;
    }

    ENDCG

    Pass {
      Tags { "LightMode" = "ForwardBase" }
      CGPROGRAM
      #pragma multi_compile_fwdbase
      #pragma multi_compile_fog
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
    }
  }
  FallBack "Mobile/Diffuse"
}
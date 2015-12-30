Shader "Standard/Simple" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}

    _Metallic ("Metallic", Range(0, 1)) = 0
    _Gloss ("Gloss", Range(0, 1)) = 1

    _Mip("Cupe Mip Resolution", Range(1, 10)) = 7
    [KeywordEnum(BRDF1, BRDF2, BRDF3)] _PBS_QUALITY("PBR Quality", Float) = 0
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "UnityPBSLighting.cginc"
    #include "UnityStandardBRDF.cginc"
    #pragma multi_compile _PBS_QUALITY_BRDF1 _PBS_QUALITY_BRDF2 _PBS_QUALITY_BRDF3
    #if defined(_PBS_QUALITY_BRDF1)
       #define STANDARD_PBS BRDF1_Unity_PBS
    #elif defined(_PBS_QUALITY_BRDF2)
       #define STANDARD_PBS BRDF2_Unity_PBS
    #elif defined(_PBS_QUALITY_BRDF3)
       #define STANDARD_PBS BRDF3_Unity_PBS
    #else
       #define STANDARD_PBS BRDF1_Unity_PBS
    #endif

    half4 _Color;
    sampler2D _MainTex; half4 _MainTex_ST;

    uniform half _Metallic;
    uniform half _Gloss;

    uniform half _Mip;

    struct v2f {
      float4 pos      : SV_POSITION;
      float2 uv       : TEXCOORD0;
      float3 worldPos : TEXCOORD1;
      float3 normal   : TEXCOORD2;
      LIGHTING_COORDS(3,4)
      UNITY_FOG_COORDS(5)
    };

    v2f vert(appdata_full v) {
      v2f o;

      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
      o.worldPos = mul(_Object2World, v.vertex).xyz;
      o.normal = v.normal;

      TRANSFER_VERTEX_TO_FRAGMENT(o);
      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    half4 frag(v2f i) : SV_Target {
      half atten = LIGHT_ATTENUATION(i);

      // directions
      half3 normalDir  = normalize(i.normal);
      half3 viewDir    = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
      half3 lightDir   = normalize(_WorldSpaceLightPos0.xyz);
      half3 reflectDir = reflect( -viewDir, normalDir );

      // gi set
      UnityGI gi;
      half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir, _Mip);
      gi.light.dir = lightDir;
      gi.light.color = _LightColor0.rgb * atten;
      gi.light.ndotl = LambertTerm (normalDir, gi.light.dir);
      gi.indirect.specular = 2.0 * rgbm.rgb;
      gi.indirect.diffuse  = half3(1,1,1);

      // main albedo
      half3 albedo = tex2D(_MainTex, i.uv) * _Color;

      // calc diffuse and specular
      half oneMinusReflectivity;
      half3 specColor;
      half3 diffColor = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specColor, oneMinusReflectivity);

      // calc PBS
      fixed4 col = STANDARD_PBS(diffColor, specColor, oneMinusReflectivity, _Gloss, normalDir, viewDir, gi.light, gi.indirect);

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
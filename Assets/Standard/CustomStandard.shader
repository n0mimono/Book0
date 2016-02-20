Shader "Standard/Custom" {
  Properties {
    [Header(Albedo Properteis)]
    _Color("Color", Color) = (1,1,1,1)
    _MainTex("Albedo", 2D) = "white" {}
    
    //_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

    // Specular
    [Header(Specular Properties)]
    [KeywordEnum(METALLIC, SPECULAR)] _BRDF_INPUT("BRDR Input", Float) = 0
    _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5

    [Header(Metalic Mode Options)]
    [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
    [Toggle(_METALLICGLOSSMAP)] _UseMetallicGlossMap ("Use Specular Map", Float) = 0
    _MetallicGlossMap("Specular Map", 2D) = "white" {}

    [Header(Specular Mode Options)]
    _SpecColor("Specular Color", Color) = (0.2,0.2,0.2)
    [Toggle(_SPECGLOSSMAP)] _UseSpecGlossMap ("Use Specular Map", Float) = 0
    _SpecGlossMap("Specular Map (Specualr)", 2D) = "white" {}

    // Normal
    [Header(Normal Properties)]
    [Toggle(_NORMALMAP)] _UseNormalMap ("Use Normal Map", Float) = 0
    _BumpScale("Scale", Float) = 1.0
    _BumpMap("Normal Map", 2D) = "bump" {}

    //_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
    //_ParallaxMap ("Height Map", 2D) = "black" {}

    // AO
    //[Header(Occlusion Properties)]
    //_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
    //_OcclusionMap("Occlusion", 2D) = "white" {}

    [Header(Emission Properties)]
    [Toggle(_EMISSION)] _UseEmissionMap ("Use Emission", Float) = 0
    _EmissionColor("Color", Color) = (0,0,0)
    //_EmissionMap("Emission", 2D) = "white" {}

    [Header(Rim Properties)]
    [Toggle(_RIM)] _UseRim ("Use Rim", Float) = 0
    _RimScale("Scale", Float) = 1
    _RimColor("Color", Color) = (0,0,0)
    _RimPower("Power", Float) = 1

    //_DetailMask("Detail Mask", 2D) = "white" {}

    //_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
    //_DetailNormalMapScale("Scale", Float) = 1.0
    //_DetailNormalMap("Normal Map", 2D) = "bump" {}

    //[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

    // Blending state
    //[HideInInspector] _Mode ("__mode", Float) = 0.0
    //[HideInInspector] _SrcBlend ("__src", Float) = 1.0
    //[HideInInspector] _DstBlend ("__dst", Float) = 0.0
    //[HideInInspector] _ZWrite ("__zw", Float) = 1.0

    [Header(BPR Quality)]
    [KeywordEnum(BRDF1, BRDF2, BRDF3)] _PBS_QUALITY("PBR Quality", Float) = 0
  }

  SubShader {
    Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
    LOD 300

    Pass {
      Name "FORWARD" 
      Tags { "LightMode" = "ForwardBase" }
      //Blend [_SrcBlend] [_DstBlend]
      //ZWrite [_ZWrite]

      CGPROGRAM
      #pragma target 3.0

      #pragma shader_feature __ _NORMALMAP
      //#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
      #pragma shader_feature __ _EMISSION
      #pragma shader_feature __ _METALLICGLOSSMAP
      #pragma shader_feature __ _SPECGLOSSMAP
      //#pragma shader_feature ___ _DETAIL_MULX2
      //#pragma shader_feature _PARALLAXMAP
      #pragma shader_feature __ _RIM
      #pragma shader_feature _PBS_QUALITY_BRDF1 _PBS_QUALITY_BRDF2 _PBS_QUALITY_BRDF3
      #pragma shader_feature _BRDF_INPUT_METALLIC _BRDF_INPUT_SPECULAR

      #pragma multi_compile_fwdbase
      #pragma multi_compile_fog

      #pragma vertex vertForwardBase
      #pragma fragment fragForwardCustom

      // multicompile optons
      #if defined(_PBS_QUALITY_BRDF1)
       #define UNITY_BRDF_PBS BRDF1_Unity_PBS
      #elif defined(_PBS_QUALITY_BRDF2)
       #define UNITY_BRDF_PBS BRDF2_Unity_PBS
      #elif defined(_PBS_QUALITY_BRDF3)
       #define UNITY_BRDF_PBS BRDF3_Unity_PBS
      #else
       #define UNITY_BRDF_PBS BRDF3_Unity_PBS
      #endif

      #if defined(_BRDF_INPUT_METALLIC)
       #define UNITY_SETUP_BRDF_INPUT MetallicSetup
      #elif defined(_BRDF_INPUT_SPECULAR)
       #define UNITY_SETUP_BRDF_INPUT SpecularSetup
      #else
       #define UNITY_SETUP_BRDF_INPUT MetallicSetup
      #endif

      uniform float _RimScale;
      uniform float4 _RimColor;
      uniform float _RimPower;

      #include "UnityStandardCoreForward.cginc"
      #include "UnityCG.cginc"
      #include "UnityShaderVariables.cginc"
      #include "UnityStandardConfig.cginc"
      #include "UnityStandardInput.cginc"
      #include "UnityPBSLighting.cginc"
      #include "UnityStandardUtils.cginc"
      #include "UnityStandardBRDF.cginc"
      #include "AutoLight.cginc"

      half4 fragForwardCustom (VertexOutputForwardBase i) : SV_Target {
        FRAGMENT_SETUP(s)
        #if UNITY_OPTIMIZE_TEXCUBELOD
          s.reflUVW		= i.reflUVW;
        #endif

        UnityLight mainLight = MainLight (s.normalWorld);
        half atten = SHADOW_ATTENUATION(i);

        half occlusion = Occlusion(i.tex.xy);
        UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

        half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
        c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
        c.rgb += Emission(i.tex.xy);

        #if defined(_RIM)
          half NNdotV = 1-abs(dot(s.normalWorld, s.eyeVec));
          c.rgb += _RimScale * _RimColor.rgb * pow(NNdotV, _RimPower);
        #endif

        UNITY_APPLY_FOG(i.fogCoord, c.rgb);
        return OutputForward (c, s.alpha);
      }

      ENDCG
    }

  }
  FallBack "Mobile/Diffuse"
}
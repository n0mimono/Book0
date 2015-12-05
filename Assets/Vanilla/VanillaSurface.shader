Shader "Custom/VanillaSurface" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Albedo (RGB)", 2D) = "white" {}
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    CGPROGRAM
    #pragma surface surf Vanilla vertex:vert
    #pragma target 3.0

    sampler2D _MainTex;
    half4 _Color;

    struct Input {
      half2 uv_MainTex;
    };

    struct SurfaceOutputVanilla {
      half3 Albedo;
      half3 Normal;
      half Alpha;
      half3 Emission;
    };

    inline half4 LightingVanilla(SurfaceOutputVanilla s, half3 lightDir, half3 viewDir, half atten) {
      half4 c;

      half3 normalDir = s.Normal;
      half3 halfDir = normalize(viewDir + lightDir);

      half NdotL = max(0.0, dot(normalDir, lightDir));
      half NdotH = max(0.0, dot(normalDir, halfDir ));

      c.rgb = saturate(s.Albedo * NdotL * NdotH * atten * 2.0 + s.Emission);
      c.a = s.Alpha;

      return c;
    }

    void vert(inout appdata_full v, out Input o) {
      UNITY_INITIALIZE_OUTPUT(Input, o);
    }

    void surf (Input IN, inout SurfaceOutputVanilla o) {
      half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      o.Alpha = c.a;
    }
    ENDCG

  } 
  FallBack "Mobile/Diffuse"
}
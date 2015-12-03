Shader "UI/Emissive-Alpha-Diffuse" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Emission ("Emission", Range(0,2)) = 0
    _Frequency ("Frequency", Float) = 1
  }

  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      sampler2D _MainTex; float4 _MainTex_ST;
      float4 _Color;
      float _Emission;
      float _Frequency;

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      v2f vert (appdata_full v) {
        v2f o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 c = tex2D(_MainTex, i.uv) * _Color;
        c.rgb = c.rgb * (1 + _Emission * (1 + cos(_Time.y * _Frequency)) * 0.5);
        return c;
      }

      ENDCG
    }
  }

}
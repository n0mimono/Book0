Shader "Custom/VanillaUnlit" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Albedo (RGB)", 2D) = "white" {}
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    CGINCLUDE
    #include "UnityCG.cginc"

    half4 _Color;
    sampler2D _MainTex; half4 _MainTex_ST;

    struct v2f {
      half2 uv     : TEXCOORD0;
      half4 vertex : SV_POSITION;
    };

    v2f vert(appdata_full v) {
      v2f o;
      o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
      return o;
    }

    half4 frag(v2f i) : SV_Target {
      half4 col = tex2D(_MainTex, i.uv) * _Color;
      return col;
    }

    ENDCG

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
    }
  }
  FallBack "Mobile/Diffuse"
}
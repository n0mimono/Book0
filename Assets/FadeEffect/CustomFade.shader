Shader "Fade/Custom" {
  Properties {
    _Color ("Tint", Color) = (1,1,1,1)
    [HideInInspector] _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _BumpTex ("Bump Tex", 2D) = "bump" {}
    _Open ("Open", Range(-1,1)) = -1
  }
  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha

    CGINCLUDE
    #include "UnityCG.cginc"

    uniform half4 _Color;
    uniform sampler2D _MainTex; half4 _MainTex_ST;
    uniform sampler2D _BumpTex; half4 _BumpTex_ST;
    uniform half _Open;

    struct v2f {
      half4 pos      : SV_POSITION;
      half2 uv       : TEXCOORD0;
    };

    v2f vert(appdata_full v) {
      v2f o;
      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      o.uv  = v.texcoord;
      return o;
    }

    half4 frag(v2f i) : SV_Target {
      half2 uv = TRANSFORM_TEX(i.uv, _MainTex);
      half4 col = tex2D(_MainTex, uv) * _Color;

      half2 bumpuv = TRANSFORM_TEX(i.uv, _BumpTex);
      half3 bump = 2 * tex2D(_BumpTex, bumpuv).rgb - 1;

      half open = abs(_Open);
      half osgn = sign(_Open);
      half x = uv.x + bump * (0.5 - abs(open - 0.5));
      half alpha = osgn > 0 ? x - open : 1 - open - x;
      col.a = alpha > 0 ? 1 : 0;

      return col;
    }

    ENDCG

    Pass {
      Cull Back
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
    }
  }
  FallBack "Mobile/Diffuse"
}
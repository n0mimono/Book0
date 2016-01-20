Shader "UI/Cutin" {
  Properties {
    [HideInInspector] _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _MainWidthScale ("Main Scale", Range(0,1)) = 1
    _BackTex0 ("Background 0", 2D) = "white" {}
    _BackTex1 ("Background 1", 2D) = "white" {}
    _BackTex2 ("Background 2", 2D) = "white" {}
    _BackColor0 ("Back Color 0", Color) = (1,1,1,1)
    _BackColor1 ("Back Color 1", Color) = (1,1,1,1)
    _BackColor2 ("Back Color 2", Color) = (1,1,1,1)
    _BackLerp1 ("Back Lerp Rate 1", Range(0,1)) = 0.5
    _BackLerp2 ("Back Lerp Rate 2", Range(0,1)) = 0.5
    _AnimTime ("Anime Time", Range(0,3)) = 0
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
      float _MainWidthScale;

      sampler2D _BackTex0; float4 _BackTex0_ST;
      sampler2D _BackTex1; float4 _BackTex1_ST;
      sampler2D _BackTex2; float4 _BackTex2_ST;
      float4 _BackColor0;
      float4 _BackColor1;
      float4 _BackColor2;
      float _BackLerp1;
      float _BackLerp2;

      float _AnimTime;

      struct v2f {
        float2 uv     : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 color  : TEXCOORD1;
      };

      v2f vert (appdata_full v) {
        v2f o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv     =v.texcoord;// TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color  = v.color;
        return o;
      }

      half4 clerp(half4 col, half4 src, half t) {
        if (col.a < 0.1) {
          return src * t;
        } else {
          return lerp(col, src, t);
        }
      }

      half4 cback(half4 col, half4 src) {
        if (col.a < 0.1) {
          return src;
        } else {
          return col;
        }
      }

      fixed4 frag (v2f i) : SV_Target {
        half t = _AnimTime;
        half t1 = saturate(t);
        half t1n = t1 * 3 - 2;
        half t2 = saturate(t - 1);
        half t3 = saturate(3 - t);

        half muvx = (i.uv.x - 0.5) / _MainWidthScale + 0.5;
        muvx += t * 0.02;
        fixed2 uv_main = fixed2(muvx, i.uv.y);

        fixed4 c = tex2D(_MainTex, uv_main) * i.color;
        fixed4 b0 = tex2D(_BackTex0, i.uv);
        fixed4 b1 = tex2D(_BackTex1, i.uv + t2 * 0.1);
        fixed4 b2 = tex2D(_BackTex2, i.uv - t2 * 0.1);

        half2 uv = i.uv * 2 - 1;
        half x = uv.x;
        half y = uv.y;

        half x2 = (x - t2 * 0.05) * t1n;
        half x3 = (x + t2 * 0.1);
        bool m1 = y >= 1.5 * x2 - 0.5 && t1 * y <= 4 * x3 + 2 && i.uv.x + i.uv.y <= t1 * t3 * 2;
        bool m2 = t1 * y >= -0.5 * x3 - 0.5 && y <= -1.5 * x2 + 1 &&  i.uv.x - i.uv.y + 1 <= t1 * t3 * 2;
        c.a = c.a * t1;
        c.rgb = c.rgb;
        
        if (m1 && m2) {
          c = cback(c, b0 * _BackColor0);
        } else if (m1) {
          c = clerp(c, b1 * _BackColor1, _BackLerp1);
        } else if (m2) {
          c = clerp(c, b2 * _BackColor2, _BackLerp2);
        } else {
        }
        c.a *= t3;

        return c;
      }

      ENDCG
    }
  }

}

Shader "Custom/Sensu" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Width ("Width", Range(0,1)) = 1
    _Warp ("Warp", Range(0,1)) = 0
    _Center ("Center", Range(-1,1)) = 0
  }
  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha

    CGINCLUDE
    #include "UnityCG.cginc"

    float4 _Color;
    sampler2D _MainTex; float4 _MainTex_ST;
    float _Width;
    float _Warp;
    float _Center;

    struct v2f {
      float4 pos   : SV_POSITION;
      float2 uv    : TEXCOORD0;
      float4 color : TEXCOORD1;
      UNITY_FOG_COORDS(2)
    };

    float4 sensu_vert(float4 v, float2 uv) {
      float4 u = v;
      float y = uv.y;
      float x = abs(uv.x * 2 - 1);
      u.x *= 1 - y * _Width;
      u.z *= 1 - y * (-x*x + 1) * _Warp;
      u.z *= 1 + (1-y) * (-x*x+ 1) * _Warp;

      return u;
    }

    float4 sensu_color(float2 uv) {
      float4 col = float4(1,1,1,1);
      col.a = saturate(uv.y + _Center);
      return col;
    }

    v2f vert(appdata_full v) {
      v2f o;

      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
      v.vertex = sensu_vert(v.vertex, o.uv);

      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      o.color = sensu_color(o.uv);

      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    fixed4 frag(v2f i) : SV_Target {
      fixed4 col = tex2D(_MainTex, i.uv) * _Color;
      col *= i.color;

      UNITY_APPLY_FOG(i.fogCoord, col);
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
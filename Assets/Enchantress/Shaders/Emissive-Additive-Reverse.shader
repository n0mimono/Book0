Shader "Enchant/Emissive-Additive-Reverse" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Emission ("Emission", Range(-1,2)) = 0
    _Frequency ("Frequency", Float) = 1
    _ColorBase ("Color Base", Color) = (1,1,1,1)
  }

  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200
    Cull Off
    ZWrite Off
    Blend SrcAlpha One

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fog
      #include "UnityCG.cginc"

      sampler2D _MainTex; float4 _MainTex_ST;
      float4 _Color;
      float _Emission;
      float _Frequency;
      float4 _ColorBase;

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 color : TEXCOORD1;
        UNITY_FOG_COORDS(2)
      };

      v2f vert (appdata_full v) {
        v2f o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color = v.color;
        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 t = _ColorBase - tex2D(_MainTex, i.uv);

        fixed4 c = t * _Color * i.color;
        c.rgb = c.rgb * (1 + _Emission * (1 + cos(_Time.y * _Frequency)) * 0.5);
        UNITY_APPLY_FOG_COLOR(i.fogCoord, c, fixed4(0,0,0,0));
        return c;
      }

      ENDCG
    }
  }

}

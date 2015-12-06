Shader "Elastic/ElasticUnlit" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _StretchStrength ("Stretch Strength", Range(0,100)) = 1
    _StretchDirection ("Stretch Direction", Vector) = (0,0,1,1)
    _ForwardStretch ("Forward Stretch", Range(0,3)) = 0
    _LerpFactor ("Lerp Factor", Range(0,1)) = 0.5
  }

  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha

    CGINCLUDE
      float _StretchStrength;
      float4 _StretchDirection;
      float _ForwardStretch;
      float _LerpFactor;

      inline float4 stretch(float4 v) {
        float3 u = v.xyz;
        float3 d = normalize(_StretchDirection).xyz;
        float s = _StretchStrength;

        // decomposition
        float3 fwd = max(0,dot(u, d)) * d;
        float3 exc = u - fwd;

        // reconstruction
        float fwdScale = 1 + s;
        float excScale = 1 / (s + 1) - (1 - exp(-s)) * length(fwd) * _ForwardStretch;
        float3 r = fwd * fwdScale + exc *excScale;
        //u = r;
        u = lerp(u, r, _LerpFactor);

        return float4(u, v.w);
      }
    ENDCG

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      sampler2D _MainTex; float4 _MainTex_ST;
      float4 _Color;

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      v2f vert (appdata_full v) {
        v2f o;

        v.vertex = stretch(v.vertex);

        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 c = tex2D(_MainTex, i.uv) * _Color;
        return c;
      }

      ENDCG
    }
  }

}
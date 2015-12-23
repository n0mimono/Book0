Shader "Particle/Papillon" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Main Tex", 2D) = "white" {}
    _YColor ("Y Color", Color) = (1,1,1,1)
    _XColor ("X Color", Color) = (1,1,1,1)
    _Amplitude ("Amplitude", Float) = 1
    _Frequency ("Frequency", Float) = 1
    _Scale ("Scale", Float) = 1
  }
  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "UnityGlobalIllumination.cginc"

    float4 _Color;
    sampler2D _MainTex; float4 _MainTex_ST;
    float4 _YColor;
    float4 _XColor;
    float _Amplitude;
    float _Frequency;
    float _Scale;

    struct v2f {
      float4 pos      : SV_POSITION;
      float2 uv       : TEXCOORD0;
      float3 worldPos : TEXCOORD1;
      float4 color    : TEXCOORD2;
      UNITY_FOG_COORDS(3)
    };

    float4 papillon(float4 v, float2 uv) {
      float4 x = abs(uv.x * 2 - 1);
      float4 y = uv.y;

      float4 u = v;
      u.w = u.w * _Scale;

      u.y = (x * y) * _Amplitude * cos(_Frequency * _Time.y);

      return u;
    }

    v2f vert(appdata_full v) {
      v2f o;

      v.vertex = papillon(v.vertex, v.texcoord);
      v.color = lerp(lerp(v.color, _YColor, v.texcoord), _XColor, abs(v.texcoord * 2 - 1));

      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      o.uv = v.texcoord;
      o.worldPos = mul(_Object2World, v.vertex).xyz;
      o.color = v.color;

      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    fixed4 frag(v2f i) : SV_Target {
      fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;

      UNITY_APPLY_FOG(i.fogCoord, col);
      return col;
    }

    ENDCG

    Pass {
      Cull Off
      ZWrite Off
      CGPROGRAM
      #pragma multi_compile_fog
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
    }

  }
  FallBack "Mobile/Diffuse"
}

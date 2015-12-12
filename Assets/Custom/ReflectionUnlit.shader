﻿Shader "Custom/ReflectionUnlit" {
  Properties {
    _Mip ("Mip", Range(1, 5)) = 1
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fog
      #include "UnityCG.cginc"

      struct v2f {
        float4 pos      : SV_POSITION;
        float2 uv       : TEXCOORD0;
        float3 normal   : TEXCOORD1;
        float4 worldPos : TEXCOORD2;
        float3 view     : TEXCOORD3;
        float3 reflec   : TEXCOORD4;
        UNITY_FOG_COORDS(5)
      };

      float _Mip;

      v2f vert (appdata_full v) {
        v2f o;

        o.uv       = v.texcoord;
        o.pos      = mul(UNITY_MATRIX_MVP, v.vertex);
        o.normal   = v.normal;
        o.worldPos = mul(_Object2World, v.vertex);
        o.view     = normalize(o.worldPos - _WorldSpaceCameraPos.xyz);
        o.reflec   = reflect(o.view, o.normal);

        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 col = fixed4(1,1,1,1);

        //col.rgb *= texCUBElod(unity_SpecCube0, half4(i.reflec, _Mip)) * 2;
        col.rgb *= UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, i.reflec, _Mip) * 2;

        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;        
      }
      ENDCG
    }
  }
}
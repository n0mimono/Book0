Shader "Custom/VanillaLight" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Albedo (RGB)", 2D) = "white" {}
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"

    half4 _Color;
    sampler2D _MainTex; half4 _MainTex_ST;

    uniform half4 _LightColor0;

    struct v2f {
      half4 pos      : SV_POSITION;
      half2 uv       : TEXCOORD0;
      half3 worldPos : TEXCOORD1;
      half3 normal   : TEXCOORD2;
      LIGHTING_COORDS(3,4)
      UNITY_FOG_COORDS(5)
    };

    v2f vert(appdata_full v) {
      v2f o;

      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
      o.worldPos = mul(_Object2World, v.vertex).xyz;
      o.normal = v.normal;

      TRANSFER_VERTEX_TO_FRAGMENT(o);
      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    half4 frag(v2f i) : SV_Target {
      half atten = LIGHT_ATTENUATION(i);
      half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
      half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
      half3 normalDir = i.normal;
      half3 halfDir = normalize(viewDir + lightDir);
      half NdotL = max(0.0, dot(normalDir, lightDir));
      half NdotH = max(0.0, dot(normalDir, halfDir ));

      half4 col = tex2D(_MainTex, i.uv) * _Color;
      col *= NdotL * NdotH;
      col *= _LightColor0 * atten * 2.0;
      col += UNITY_LIGHTMODEL_AMBIENT * _Color;

      UNITY_APPLY_FOG(i.fogCoord, col);
      return col;
    }

    ENDCG

    Pass {
      Tags { "LightMode" = "ForwardBase" }
      CGPROGRAM
      #pragma multi_compile_fwdbase
      #pragma multi_compile_fog
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
    }
  }
  FallBack "Mobile/Diffuse"
}
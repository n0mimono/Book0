Shader "Custom/Bubble" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _Reflection ("Reflection", Range(0,1)) = 1
    _Mip ("Cube Mip", Range(1, 10)) = 1
    _Specular ("Specular", Range(0,1)) = 1
    _Gloss ("Gloss", Range(1,10)) = 3
    _Rim ("Rim", Range(0,1)) = 1
    _RimPower ("Rim Power", Range(1,10)) = 3
    _FrontAlpha ("Front Alpha", Range(0,1)) = 1
    _BackAlpha ("Back Alpha", Range(0,1)) = 1
  }
  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "UnityGlobalIllumination.cginc"

    half4 _Color;
    half _Reflection;
    half _Mip;
    half _Specular;
    half _Gloss;
    half _Rim;
    half _RimPower;
    half _FrontAlpha;
    half _BackAlpha;

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
      o.uv = v.texcoord;
      o.worldPos = mul(_Object2World, v.vertex).xyz;
      o.normal = v.normal;

      TRANSFER_VERTEX_TO_FRAGMENT(o);
      UNITY_TRANSFER_FOG(o,o.pos);
      return o;
    }

    half4 frag(v2f i, bool is_front) {
      half atten = LIGHT_ATTENUATION(i);

      half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
      half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
      half3 normalDir = i.normal;
      half3 halfDir = normalize(viewDir + lightDir);
      half3 reflectDir = reflect(-viewDir, normalDir);

      if (!is_front) {
        normalDir *= -1;
      }

      half NdotL = max(0.0, dot(normalDir, lightDir));
      half NdotH = max(0.0, dot(normalDir, halfDir ));
      half NdotV = max(0.0, dot(normalDir, viewDir ));

      half4 rgbm = 2.0 * texCUBElod(unity_SpecCube0, half4(reflectDir, _Mip));
      half rim = _Rim * pow(1 - NdotV, _RimPower);
      half spec = _Specular * pow(NdotH, _Gloss);

      half4 refl = lerp(half4(0,0,0,0), rgbm, _Reflection);

      half4 col = refl * _Color + spec + rim;
      col.rgb = (col.rgb * _LightColor0.rgb) + UNITY_LIGHTMODEL_AMBIENT.rgb;
      col.a *= is_front ? _FrontAlpha : _BackAlpha;

      UNITY_APPLY_FOG(i.fogCoord, col);

      return col;
    }

    half4 frag_front(v2f i) : SV_Target {
      return frag(i, true);
    }
    half4 frag_back(v2f i) : SV_Target {
      return frag(i, false);
    }

    ENDCG

    Pass {
      //Tags { "LightMode" = "ForwardAdd" }
      Cull Front
      ZWrite Off
      CGPROGRAM
      #pragma multi_compile_fog
      #pragma vertex vert
      #pragma fragment frag_back
      #pragma target 3.0
      ENDCG
    }

    Pass {
      Tags { "LightMode" = "ForwardBase" }
      Cull Back
      ZWrite Off
      CGPROGRAM
      #pragma multi_compile_fwdbase
      #pragma multi_compile_fog
      #pragma vertex vert
      #pragma fragment frag_front
      #pragma target 3.0
      ENDCG
    }

  }
  FallBack "Mobile/Diffuse"
}

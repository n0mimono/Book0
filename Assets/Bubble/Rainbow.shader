Shader "Bubble/Rainbow" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _Reflection ("Reflection", Range(0,1)) = 1
    _Mip ("Cube Mip", Range(1, 10)) = 1
    _Specular ("Specular", Range(0,1)) = 1
    _Gloss ("Gloss", Range(1,10)) = 3
    _Rim ("Rim", Range(0,1)) = 1
    _RimPower ("Rim Power", Range(1,10)) = 3

    _Rainbow ("Rainbow", Float) = 3
    _RainbowScale ("Rainbow Scale", Float) = 1
    _RainbowOffset ("Rainbow Offset", Float) = 0
  }

  SubShader {
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

    half _Rainbow;
    half _RainbowScale;
    half _RainbowOffset;

    struct v2f {
      half4 pos      : SV_POSITION;
      half2 uv       : TEXCOORD0;
      half3 worldPos : TEXCOORD1;
      half3 normal   : TEXCOORD2;
      LIGHTING_COORDS(3,4)
      UNITY_FOG_COORDS(5)
    };

    half3 hue2rgb(half h) {
      half p = h * 3;
      half u = pow(p - floor(p), 1);
      if (h <     0.0) return half3(  1,  0,  0);
      if (h < 1.0/3.0) return half3(1-u,  u,  0);
      if (h < 2.0/3.0) return half3(  0,1-u,  u);
      if (h <=3.0/3.0) return half3(  u,  0,1-u);
      else             return half3(  1,  0,  0);
    }
    half gauss(half x) {
      return exp(-pow(x - 0.5,2)/sqrt(2));
    }
    half3 rainbow(half spec) {
      half3 r = hue2rgb(_RainbowScale * spec + _RainbowOffset);
      return (r + 0.5) * gauss(spec);
    }

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

    half4 frag(v2f i) : SV_Target {
      half atten = LIGHT_ATTENUATION(i);

      half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
      half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
      half3 normalDir = i.normal;
      half3 halfDir = normalize(viewDir + lightDir);
      half3 reflectDir = reflect(-viewDir, normalDir);

      half NdotL = max(0.0, dot(normalDir, lightDir));
      half NdotH = max(0.0, dot(normalDir, halfDir ));
      half NdotV = max(0.0, dot(normalDir, viewDir ));

      half4 rgbm = 2.0 * UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir, _Mip);
      half rim = _Rim * pow(1 - NdotV, _RimPower);
      half spec = _Specular * pow(NdotH, _Gloss);

      half4 rain = _Rainbow * half4(rainbow(NdotH), 1) * (1 - NdotH);
      half4 refl = lerp(half4(0,0,0,0), rgbm, _Reflection);

      half4 col = refl * _Color + spec * rain + rim;
      col.rgb = (col.rgb * _LightColor0.rgb) + UNITY_LIGHTMODEL_AMBIENT.rgb;

      UNITY_APPLY_FOG(i.fogCoord, col);

      return col;
    }

    ENDCG

    Pass {
      Tags { "LightMode" = "ForwardBase" }
      Cull Back
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

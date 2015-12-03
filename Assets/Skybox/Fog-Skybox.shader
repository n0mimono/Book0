Shader "Skybox/Fog-Skybox" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
    _FogFactor ("Fog Factor", Float) = 0
    [NoScaleOffset] _FrontTex ("Front [+Z]", 2D) = "grey" {}
    [NoScaleOffset] _BackTex ("Back [-Z]", 2D) = "grey" {}
    [NoScaleOffset] _LeftTex ("Left [+X]", 2D) = "grey" {}
    [NoScaleOffset] _RightTex ("Right [-X]", 2D) = "grey" {}
    [NoScaleOffset] _UpTex ("Up [+Y]", 2D) = "grey" {}
    [NoScaleOffset] _DownTex ("Down [-Y]", 2D) = "grey" {}
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    CGINCLUDE
    #include "UnityCG.cginc" 

    #define DIR_FORWARD half4( 0, 0, 1,1)
    #define DIR_BACK    half4( 0, 0,-1,1)
    #define DIR_LEFT    half4(-1, 0, 0,1)
    #define DIR_RIGHT   half4( 1, 0, 0,1)
    #define DIR_UP      half4( 0, 1, 0,1)
    #define DIR_DOWN    half4( 0,-1, 0,1)
	uniform float global_LightFactor;
	uniform float4 global_LightPos;
	uniform float4 global_LightColor;

    half4 _Tint;
    half _Exposure;
    float _Rotation;
    float _FogFactor;

    inline float4 RotateAroundYInDegrees (float4 vertex, float degrees) {
        float alpha = degrees * UNITY_PI / 180.0;
        float sina, cosa;
        sincos(alpha, sina, cosa);
        float2x2 m = float2x2(cosa, -sina, sina, cosa);
        return float4(mul(m, vertex.xz), vertex.yw).xzyw;
    }

    struct appdata_t {
        float4 vertex   : POSITION;
        float2 texcoord : TEXCOORD0;
    };
    struct v2f {
        float4 vertex    : SV_POSITION;
        float2 texcoord  : TEXCOORD0;
        float4 posWorld  : TEXCOORD1;
    };

    inline half3 add_fog(half3 c, v2f i, half fogBase) {
    #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
        half coord = saturate(2 * i.texcoord.y - 1);

        // fog factor
        #if defined(FOG_LINEAR)
            half fogFactor = -coord * _FogFactor * fogBase;
        #elif defined(FOG_EXP)
            half fogFactor = exp2(-coord * _FogFactor) * fogBase;
        #elif defined(FOG_EXP2)
            half fac = coord*_FogFactor;
            half fogFactor = exp2(-fac * fac) * fogBase;
        #else
            half fogFactor = fogBase;
        #endif

        // fog color
        half3 lightDir = normalize(global_LightPos.xyz);
        half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);
        half LdotNV = max(0, dot(lightDir, -viewDir));
        half lightFactor = LdotNV * global_LightFactor;
        half3 fogColor = saturate(unity_FogColor.rgb + global_LightColor.rgb * lightFactor);

        return lerp(c, fogColor, fogFactor);
    #else
        return c;
    #endif
    }

    v2f skybox_vert (appdata_t v, half4 dir) {
        v2f o;
        float4 vertex = RotateAroundYInDegrees(v.vertex, _Rotation);
        o.vertex = mul(UNITY_MATRIX_MVP, vertex);
        o.posWorld = mul(_Object2World, vertex);
        o.texcoord = v.texcoord;

        return o;
    }

    half4 skybox_frag (v2f i, sampler2D smp, half4 smpDecode, half fogBase)
    {
        half4 tex = tex2D (smp, i.texcoord);
        half3 c = tex.rgb;

        c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
        c *= _Exposure;

        c = add_fog(c, i, fogBase);

        return half4(c, 1);
    }
    ENDCG

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _FrontTex;
        half4 _FrontTex_HDR;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_FORWARD); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex_HDR, 1); }
        ENDCG 
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _BackTex;
        half4 _BackTex_HDR;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_BACK); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex_HDR, 1); }
        ENDCG 
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _LeftTex;
        half4 _LeftTex_HDR;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_LEFT); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex_HDR, 1); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _RightTex;
        half4 _RightTex_HDR;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_RIGHT); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex_HDR, 1); }
        ENDCG
    }    
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _UpTex;
        half4 _UpTex_HDR;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_UP); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex_HDR, 0); }
        ENDCG
    }    
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _DownTex;
        half4 _DownTex_HDR;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_DOWN); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex_HDR, 0); }
        ENDCG
    }
}
}

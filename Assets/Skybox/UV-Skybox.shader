Shader "Skybox/UV-Skybox" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    _Rotation ("Rotation", Range(0, 360)) = 0
    [KeywordEnum(Uv, View, Light)] _UV_SKY ("Skybox Mode", Float) = 0
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    CGINCLUDE
    #include "UnityCG.cginc" 
    #pragma multi_compile _UV_SKY_UV _UV_SKY_VIEW _UV_SKY_LIGHT

    #define DIR_FORWARD half4( 0, 0, 1,1)
    #define DIR_BACK    half4( 0, 0,-1,1)
    #define DIR_LEFT    half4( 1, 0, 0,1)
    #define DIR_RIGHT   half4(-1, 0, 0,1)
    #define DIR_UP      half4( 0, 1, 0,1)
    #define DIR_DOWN    half4( 0,-1, 0,1)

    half4 _Tint;
    float _Rotation;

    uniform float global_LightFactor;
	uniform float4 global_LightPos;
	uniform float4 global_LightColor;

    float4 RotateAroundYInDegrees (float4 vertex, float degrees) {
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
        float4 vertex   : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float4 posWorld : TEXCOORD1;
    };
    v2f skybox_vert (appdata_t v, half4 dir) {
        v2f o;
        float4 vertex = RotateAroundYInDegrees(v.vertex, _Rotation);
        o.vertex = mul(UNITY_MATRIX_MVP, vertex);
        o.posWorld = mul(_Object2World, vertex);
        o.texcoord = v.texcoord;

        return o;
    }

    half4 skybox_frag (v2f i, sampler2D smp, half4 smpDecode) {        
        half3 lightDir = normalize(global_LightPos.xyz);
        half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);
        half LdotNV = max(0, dot(lightDir, -viewDir));
        half lightFactor = LdotNV * global_LightFactor;

        #if defined(_UV_SKY_UV)
        return half4(i.texcoord.x, 0, i.texcoord.y, 1);
        #elif defined(_UV_SKY_VIEW)
        return float4(viewDir * 0.5 + 0.5,1);
        #elif defined(_UV_SKY_LIGHT)
        return lightFactor * global_LightColor;
        #else
        return float4(1,1,1,1);
        #endif
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
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex_HDR); }
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
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex_HDR); }
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
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex_HDR); }
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
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex_HDR); }
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
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex_HDR); }
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
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex_HDR); }
        ENDCG
    }
}
}

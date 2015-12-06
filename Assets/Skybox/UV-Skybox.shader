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
    inline v2f skybox_vert (appdata_t v) {
        v2f o;
        float4 vertex = RotateAroundYInDegrees(v.vertex, _Rotation);
        o.vertex = mul(UNITY_MATRIX_MVP, vertex);
        o.posWorld = mul(_Object2World, vertex);
        o.texcoord = v.texcoord;

        return o;
    }

    inline half4 skybox_frag (v2f i, sampler2D smp) {        
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
        v2f vert (appdata_t v) { return skybox_vert(v); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex); }
        ENDCG 
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _BackTex;
        v2f vert (appdata_t v) { return skybox_vert(v); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex); }
        ENDCG 
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _LeftTex;
        v2f vert (appdata_t v) { return skybox_vert(v); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _RightTex;
        v2f vert (appdata_t v) { return skybox_vert(v); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex); }
        ENDCG
    }    
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _UpTex;
        v2f vert (appdata_t v) { return skybox_vert(v); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex); }
        ENDCG
    }    
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _DownTex;
        v2f vert (appdata_t v) { return skybox_vert(v); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex); }
        ENDCG
    }
}
}

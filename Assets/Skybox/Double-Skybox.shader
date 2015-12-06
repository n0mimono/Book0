Shader "Skybox/Double-Skybox" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
    [NoScaleOffset] _FrontTex ("Front [+Z]", 2D) = "white" {}
    [NoScaleOffset] _BackTex ("Back [-Z]", 2D) = "white" {}
    [NoScaleOffset] _LeftTex ("Left [+X]", 2D) = "white" {}
    [NoScaleOffset] _RightTex ("Right [-X]", 2D) = "white" {}
    [NoScaleOffset] _UpTex ("Up [+Y]", 2D) = "white" {}
    [NoScaleOffset] _DownTex ("Down [-Y]", 2D) = "white" {}

    _SecondTint ("[Second] Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _SecondExposure ("[Second] Exposure", Range(0, 8)) = 1.0
    [NoScaleOffset] _SecondFrontTex ("[Second] Front [+Z]", 2D) = "black" {}
    [NoScaleOffset] _SecondBackTex ("[Second] Back [-Z]", 2D) = "black" {}
    [NoScaleOffset] _SecondLeftTex ("[Second] Left [+X]", 2D) = "black" {}
    [NoScaleOffset] _SecondRightTex ("[Second] Right [-X]", 2D) = "black" {}
    [NoScaleOffset] _SecondUpTex ("[Second] Up [+Y]", 2D) = "black" {}
    [NoScaleOffset] _SecondDownTex ("[Second] Down [-Y]", 2D) = "black" {}

    _LerpBorder ("Lerp Border", Range(-1,1)) = 0
    _BorderWidth ("Border Width", Float) = 1
    _BorderMask ("Border Mask", 2D) = "gray" {}
    _MaskScale ("Mask Scale", Range(0,1)) = 0
    _Origin ("Origin", Vector) = (0,0,1,1)
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

    half4 _Tint;
    half _Exposure;
    float _Rotation;
    half4 _SecondTint;
    half _SecondExposure;
    half _LerpBorder;
    half _BorderWidth;
    sampler2D _BorderMask;
    half _MaskScale;
    half4 _Origin;

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

    v2f skybox_vert (appdata_t v, half4 dir) {
        v2f o;
        float4 vertex = RotateAroundYInDegrees(v.vertex, _Rotation);
        o.vertex = mul(UNITY_MATRIX_MVP, vertex);
        o.posWorld = mul(_Object2World, vertex);
        o.texcoord = v.texcoord;

        return o;
    }

    half4 skybox_frag (v2f i, sampler2D smp, sampler2D secondSmp) {
        half3 orgDir = normalize(_Origin.xyz);
        half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);
        half OdotNV = dot(orgDir, -viewDir);

        half3 firstCol = tex2D (smp, i.texcoord).rgb;
        firstCol *= _Tint.rgb * unity_ColorSpaceDouble.rgb;
        firstCol *= _Exposure;

        half3 secondCol = tex2D (secondSmp, i.texcoord).rgb;
        firstCol *= _SecondTint.rgb * unity_ColorSpaceDouble.rgb;
        firstCol *= _SecondExposure;

        half3 maskCol = tex2D (_BorderMask, i.texcoord).rgb;
        half mask = (2 * length(maskCol) - 1) * _MaskScale;

        half lerpFactor = (OdotNV + _LerpBorder * 2 + mask) * _BorderWidth;
        lerpFactor = saturate((lerpFactor + 1) * 0.5);

        half3 col = lerp(firstCol, secondCol, lerpFactor);
        col = saturate(col);

        return half4(col, 1);
    }
    ENDCG

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _FrontTex;
        sampler2D _SecondFrontTex;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_FORWARD); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _SecondFrontTex); }
        ENDCG 
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _BackTex;
        sampler2D _SecondBackTex;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_BACK); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _SecondBackTex); }
        ENDCG 
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _LeftTex;
        sampler2D _SecondLeftTex;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_LEFT); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _SecondLeftTex); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _RightTex;
        sampler2D _SecondRightTex;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_RIGHT); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _SecondRightTex); }
        ENDCG
    }    
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _UpTex;
        sampler2D _SecondUpTex;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_UP); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _SecondUpTex); }
        ENDCG
    }    
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_fog
        sampler2D _DownTex;
        sampler2D _SecondDownTex;
        v2f vert (appdata_t v) { return skybox_vert(v, DIR_DOWN); }
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _SecondDownTex); }
        ENDCG
    }
}
}

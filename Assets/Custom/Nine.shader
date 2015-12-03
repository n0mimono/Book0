Shader "Custom/Nine" {
    Properties {
        _Tex01 ("01", 2D) = "white" {}
        _Tex02 ("02", 2D) = "white" {}
        _Tex03 ("03", 2D) = "white" {}
        _Tex04 ("04", 2D) = "white" {}
        _Tex05 ("05", 2D) = "white" {}
        _Tex06 ("06", 2D) = "white" {}
        _Tex07 ("07", 2D) = "white" {}
        _Tex08 ("08", 2D) = "white" {}
        _Tex09 ("09", 2D) = "white" {}
        }
    SubShader {
        CGINCLUDE
            #include "UnityCG.cginc"

            sampler2D _Tex01; float4 _Tex01_ST;
            sampler2D _Tex02; float4 _Tex02_ST;
            sampler2D _Tex03; float4 _Tex03_ST;
            sampler2D _Tex04; float4 _Tex04_ST;
            sampler2D _Tex05; float4 _Tex05_ST;
            sampler2D _Tex06; float4 _Tex06_ST;
            sampler2D _Tex07; float4 _Tex07_ST;
            sampler2D _Tex08; float4 _Tex08_ST;
            sampler2D _Tex09; float4 _Tex09_ST;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_full v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _Tex01);
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float4 col = tex2D(_Tex01, i.uv)
                    + tex2D(_Tex02, i.uv)
                    + tex2D(_Tex03, i.uv)
                    + tex2D(_Tex04, i.uv)
                    + tex2D(_Tex05, i.uv)
                    + tex2D(_Tex06, i.uv)
                    + tex2D(_Tex07, i.uv) // comment out me!
                    + tex2D(_Tex08, i.uv)
                    + tex2D(_Tex09, i.uv);
                col *= (1.0 / 9.0);
                return col;
            }

        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
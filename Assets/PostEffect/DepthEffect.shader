Shader "ImageEffect/DepthEffect" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_TexFactor ("Texture Factor", Range(0,1)) = 1
		_DepthScale ("Depth Scale", Range(0,10)) = 1
		_DepthPower ("Depth Power", Range(0, 2)) = 1
		[Toggle(_RED_LINE)] _RedLine ("Red Line", Float) = 0
		_RedLineTimeScale ("Red Line Time Scale", Range(0,1)) = 0.1
		_RedLineWidth ("Red Line Width", Range(0, 1)) = 0.02
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _RED_LINE

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float _TexFactor;
			float _DepthScale;
			float _DepthPower;
			float _RedLineTimeScale;
			float _RedLineWidth;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			float mod(float x) {
			    return x - floor(x);
			}
			
			fixed4 frag (v2f i) : SV_Target {
		        fixed4 col = lerp(fixed4(1,1,1,1), tex2D(_MainTex, i.uv), _TexFactor);;

				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
				float depth01 = Linear01Depth(depth);
				float dlum = pow(depth01 * _DepthScale, _DepthPower);
				col *= dlum;

				#ifdef _RED_LINE
				float time = 1 - mod(_Time.y * _RedLineTimeScale);
				float4 red_line = abs(time - dlum) < _RedLineWidth ? float4(1,0,0,0) : float4(1,1,1,1);
				col *= red_line;
				#endif

				return col;
			}
			ENDCG
		}
	}
}

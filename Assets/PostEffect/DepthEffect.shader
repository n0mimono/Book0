Shader "ImageEffect/DepthEffect" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_DepthScale ("Depth Scale", Range(0,10)) = 1
		_DepthPower ("Depth Power", Range(0, 2)) = 1

	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float _DepthScale;
			float _DepthPower;

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
			
			fixed4 frag (v2f i) : SV_Target {
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
				float depth01 = Linear01Depth(depth);
				float dlum = pow(depth01 * _DepthScale, _DepthPower);

				fixed4 col = fixed4(1,1,1,1);
				col *= dlum;
				return col;
			}
			ENDCG
		}
	}
}

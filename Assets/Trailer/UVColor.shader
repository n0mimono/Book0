Shader "Custom/UVColor" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex; float4 _MainTex_ST;
			
			v2f vert (appdata_full v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				//fixed4 col = tex2D(_MainTex, i.uv);

				fixed4 col = fixed4(i.uv.x, 0, i.uv.y, 1);

				return col;
			}
			ENDCG
		}
	}
}

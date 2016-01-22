Shader "ImageEffect/FlashFade" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_AnimTime ("Animation Time", Range(0, 1)) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _AnimTime;

			fixed4 frag (v2f_img i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed gray = (col.r + col.g + col.b) / 3;
				col = saturate(lerp(col, gray, _AnimTime) + _AnimTime);

				return col;
			}
			ENDCG
		}
	}
}

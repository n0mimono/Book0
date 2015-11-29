Shader "Trail/ProcedualTrail" {
	Properties {
	  _Color ("Color", Color) = (1,1,1,1)
	  _MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct v2f {
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			sampler2D _MainTex; float4 _MainTex_ST;

			half mod(half x) {
			    return saturate(x - floor(x));
			}

			v2f vert (appdata_full v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;

				//o.uv.x = mod(o.uv.x - _Time.y);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)) * _Color;
			    half2 p = i.uv * 2 - 1;
			    half r = length(p);
			    half a = 1 - r;

			    col.a = a;

				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));		
				return col;
			}
			ENDCG
		}
	}
}

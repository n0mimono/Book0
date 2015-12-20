Shader "Hidden/ToonEffect" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex; float4 _MainTex_ST;
			uniform sampler2D _TmpTex;
			uniform float4    _MainTex_TexelSize;
			uniform sampler2D _CameraDepthNormalsTexture;
			uniform float4    _EdgeParams;
			uniform float4    _EdgePowers;

			struct v2f {
				float4 pos    : SV_POSITION;
				float2 uv[3]  : TEXCOORD0;
			};

			v2f vert (appdata_img v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv[0] = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv[1] = o.uv[0] + float2( _MainTex_TexelSize.x, 0) * _EdgeParams.w;
				o.uv[2] = o.uv[0] + float2(0, -_MainTex_TexelSize.y) * _EdgeParams.w;
				return o;
			}
		ENDCG

		// 0: edge
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 frag (v2f i) : SV_Target {
				fixed4 color   = tex2D(_MainTex, i.uv[0]);
				half4  dnormal = tex2D(_CameraDepthNormalsTexture, i.uv[0]);
				half   depth   = DecodeFloatRG(dnormal.zw);
				half4  edge = 0;

				for (int j = 0; j < 2; j++) {
					half4 col = tex2D(_MainTex, i.uv[j]);
					half4 dn  = tex2D(_CameraDepthNormalsTexture, i.uv[j]);

					half2 n = abs(dnormal.xy - dn.xy);
					edge.a += (n.x + n.y) > _EdgeParams.x ? _EdgePowers.x : 0;

					half d = depth - DecodeFloatRG(dn.zw);
					edge.a += d * _EdgeParams.y > 0.09 * depth ? _EdgePowers.y : 0;

					half c = length(color.rgb - col.rgb);
					edge.a += c*c > _EdgeParams.z ? _EdgePowers.z : 0;
				}

				return edge;
			}
			ENDCG
		}

		// 1: composite
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 frag (v2f i) : SV_Target {
			    fixed4 color = tex2D(_MainTex, i.uv[0]);
			    fixed4 edge  = tex2D(_TmpTex, i.uv[0]);

			    color.rgb = lerp(color.rgb, pow(color.rgb*0.95, 1 + 5 * edge.a), edge.a);
			    return color;
			}
			ENDCG
		}

		// 2: color
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 frag (v2f i) : SV_Target {
			    float2 uv = i.uv[0];
				fixed4 layer = tex2D(_MainTex, uv);
				fixed4 color = layer;
				float centerDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, uv).zw);
				
				// shadow by hair
				fixed4 hair = tex2D(_MainTex, uv + _MainTex_TexelSize * half2(-1,1) * 15);
				half4 hairD = tex2D(_CameraDepthNormalsTexture, uv + _MainTex_TexelSize * half2(-1, 1) * 15);
				float hairDepth = DecodeFloatRG(hairD.zw);

				float3 diff = (hair.rgb - float3(230, 211, 158) / 255.0);
				float3 diff2 = (layer.rgb - float3(255, 235, 202) / 255.0);
				if (dot(diff, diff) < 0.01 && dot(diff2, diff2) < 0.01 && hairDepth < centerDepth)
					color.rgb *= float3(1, 0.8, 0.8);

				// hair 
				diff2 = (layer.rgb - float3(230, 211, 158) / 255.0);
				if (dot(diff2, diff2) < 0.01)
					color.rgb *= lerp(float3(1,1,1),float3(1, 0.5, 0.7),1 - uv.y);

				// shadow with neck
				hair = tex2D(_MainTex, uv + _MainTex_TexelSize * half2(-0.5, 1) * 55);
				hairD = tex2D(_CameraDepthNormalsTexture, uv + _MainTex_TexelSize * half2(-0.5, 1) * 55);
				hairDepth = DecodeFloatRG(hairD.zw);
				diff = (hair.rgb - float3(255, 235, 202) / 255.0);
				diff2 = (layer.rgb - float3(253, 222, 197) / 255.0);
				if (dot(diff, diff) < 0.01 && dot(diff2, diff2) < 0.0001 && hairDepth < centerDepth)
					color.rgb *= float3(.95, 0.8, 0.7);

				// hair 1
				diff = (hair.rgb - float3(230, 211, 158) / 255.0);
				diff2 = (layer.rgb - float3(253, 222, 197) / 255.0);
				if (dot(diff, diff) < 0.01 && dot(diff2, diff2) < 0.0001 && hairDepth < centerDepth)
					color.rgb *= float3(.95, 0.8, 0.7);

				// hair 2
				diff = (hair.rgb - float3(230, 211, 158) / 255.0);
				diff2 = (layer.rgb - float3(230, 211, 158) / 255.0);
				if (dot(diff, diff) < 0.01 && dot(diff2, diff2) < 0.01 && hairDepth + 0.0002 < centerDepth)
					color.rgb *= float3(.95, 0.8, 0.7);

				// hair 3
				diff = (hair.rgb - float3(255, 235, 202) / 255.0);
				diff2 = (layer.rgb - float3(230, 211, 158) / 255.0);
				if (dot(diff, diff) < 0.01 && dot(diff2, diff2) < 0.0001 && hairDepth < centerDepth)
					color.rgb *= float3(.95, 0.8, 0.7);

				return color;
			}
			ENDCG
		}


	}
}

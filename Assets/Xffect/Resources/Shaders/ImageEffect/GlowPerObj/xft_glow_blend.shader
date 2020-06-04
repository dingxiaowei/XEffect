// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PP/Xffect/glow_per_obj/blend" {
Properties {
	_MainTex ("Main Texture", 2D) = "white" {}
	_GlowTex ("Glow Texture", 2D) = "white" {}
}

Category {
	Tags { "XftEffect"="GlowPerObj" }
	ZTest Always
	Cull Off Lighting Off ZWrite Off Fog { Mode Off }
	

	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _GlowTex;
			sampler2D _CameraDepthTexture;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
			};
			
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord2 = o.texcoord;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
				o.texcoord2.y = 1-o.texcoord2.y;
				#endif

				return o;
			}

			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mcolor = tex2D(_MainTex, i.texcoord);
				fixed4 gcolor = tex2D(_GlowTex, i.texcoord2);
				
				fixed4 ret = mcolor + gcolor;

				return ret;
			}
			ENDCG 
		}
	} 	
}
}

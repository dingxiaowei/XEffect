// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Xffect/PP/glitch" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Mask ("Mask Texture (RG)", 2D) = "white" {}
}

SubShader
{
 Pass
 {
     ZTest Always Cull Off ZWrite Off
     Blend Off
     Fog { Mode off }

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _Mask;
uniform fixed4 _displace;


struct v2f {
 float4 pos : POSITION;
 float2 uv : TEXCOORD0;
};

v2f vert (appdata_img v)
{
 v2f o;
 o.pos = UnityObjectToClipPos(v.vertex);
 o.uv = v.texcoord.xy;
 return o;
}


fixed4 frag (v2f i) : COLOR
{
    fixed4 mask = tex2D(_Mask, i.uv);
    fixed4 c = tex2D(_MainTex, i.uv);
    fixed strength = mask.r;
    half2 ouv = i.uv;
    
    
    //rgb displacement
    ouv.x = i.uv.x + _displace[0] * strength;
    c.r = tex2D(_MainTex, ouv).r;
    ouv.x = i.uv.x + _displace[1] * strength;
    c.g = tex2D(_MainTex, ouv).g;
    ouv.x = i.uv.x + _displace[2] * strength;
    c.b = tex2D(_MainTex, ouv).b;
	return c;
}
ENDCG

 }
}

Fallback off

}
Shader "Xffect/Object/spherical_billboard_add" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Main Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off
	ZTest Always

BindChannels {
     Bind "Color", color
     Bind "Vertex", vertex
     Bind "TexCoord", texcoord
 }

SubShader {
    Pass {

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"


sampler2D _MainTex;
fixed4 _TintColor;
sampler2D _CameraDepthTexture;

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
	float2 param : TEXCOORD1;
};

struct v2f {
    float4 pos : SV_POSITION;
	fixed4 color : COLOR;
	half2 texcoord : TEXCOORD0;
	float4 screenPos : TEXCOORD1;
	float3 P : TEXCOORD2;
	float3 Q : TEXCOORD3;
	float2 param : TEXCOORD4;
};


v2f vert (appdata_t v)
{
    v2f o;
	o.color = v.color;
	o.texcoord = v.texcoord;

	//retrieve radius and rotation from node.
	o.param = v.param;
	float angle = v.param.y;
	//rotate around z axis.
	float4x4 rotMat = float4x4(
	cos(angle),-sin(angle),0,0,
	sin(angle),cos(angle),0,0,
	0,0,1,0,
	0,0,0,1);

	//get camera-space center pos. NOTE ALL THE FOUR VERTEX ARE CENTER_POS, APPLIE IN NODE.
	float3 P = mul (UNITY_MATRIX_MV, v.vertex);
	o.P = P;

	//then get spreaded-sphere pos by texcoord.
	float3 Q = P;
	float3 dirP = normalize(P);
	//float3 up = float3(0, 1, 0);
	float3 up = mul (rotMat, float4(0,1,0,1));
	float3 right = normalize(cross(up, dirP));
	up = normalize(cross(dirP, right));
	Q += (v.texcoord.x-0.5) * right * v.param.x + (v.texcoord.y-0.5) * up * v.param.x;
	o.Q = Q;

	//float3 dirQ = normalize(Q);
	//float x = (v.param.x + _ProjectionParams.y) * length(Q) / length(P);
	//Q += dirQ * v.param.x;

	//then get the actual projected pos
	o.pos = mul (UNITY_MATRIX_P, float4(Q,1));
	
    // move projected z to near plane if point is behind near plane
	float inFrontOf = ( o.pos.z / o.pos.w ) > 0;
	o.pos.z *= inFrontOf; 

	//get screen pos.
	o.screenPos = ComputeScreenPos (o.pos);
    return o;
}


inline float Opacity(float3 P, float3 Q, float r, float4 screenCoord, float tau)
{
	float alpha = 0.0;
	
	float frontPlane = _ProjectionParams.y;
	float farPlane = _ProjectionParams.z;

	float d = length(Q - P);
	float Ql = length(Q);
	float fMin = frontPlane * Ql / Q.z;
	if(d < r)
	{		
		float w = sqrt(r * r - d * d);
		float Ds = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(screenCoord))));
		if(Ds == 0) Ds = farPlane;//farplane (needed because the distance map was cleared with 0 and not with farplane distance)
		float F = max(Ql - w, fMin);
		float B = min(Ql + w, Ds);	
		float ds = B - F;
		if(ds < 0) ds = 0;
		alpha = 1 - exp(-tau * pow(1.0 - d / r, 2) * ds);
		//alpha = 1 - exp(-tau * (1.0 - d / r) * ds);		
	}
	return alpha;
}

half4 frag (v2f i) : COLOR
{
	fixed dens = length(i.color.rgb);

	fixed tau = i.color.a * dens;

	float alpha = Opacity(i.P, i.Q, i.param.x, i.screenPos, tau);

	fixed4 mainColor = tex2D(_MainTex, i.texcoord);

	return 2.0f * i.color * _TintColor * mainColor * alpha;
	//return fixed4(0.5,0.5,0.5,0.5);
}
ENDCG

		}
	}
}
Fallback "VertexLit"
} 

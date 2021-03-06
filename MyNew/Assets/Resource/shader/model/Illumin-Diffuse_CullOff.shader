Shader "Roma/Model/Self-Illumin_Diffuse_CullOff" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Illum ("Illumin (A)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	Cull off
CGPROGRAM

#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
sampler2D _Illum;
fixed4 _Color;

struct Input {
	fixed2 uv_MainTex;
	fixed2 uv_Illum;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
	o.Alpha = c.a;
}
ENDCG
} 
//FallBack "Legacy Shaders/Self-Illumin/VertexLit"
//CustomEditor "LegacyIlluminShaderGUI"
}

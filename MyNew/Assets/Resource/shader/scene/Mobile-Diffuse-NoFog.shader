Shader "Roma/Scene/Diffuse_NoFog_NoLight" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				fixed2 texcoord : TEXCOORD0;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			fixed4 _MainTex_ST;


			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				return col * _Color;
			}
		ENDCG
	}
}
Fallback "Mobile/VertexLit"
}

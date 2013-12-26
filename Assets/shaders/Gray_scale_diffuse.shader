Shader "Grey Scale/diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		float4 _Color;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_GreyMask;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex)*_Color;
			o.Albedo = (c.r + c.b + c.g) / 3;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

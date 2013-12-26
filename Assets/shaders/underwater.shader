Shader "Custom/underwater" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DeepColor ("Deep Color", Color) = (1,1,1,1)
		_MiddleColor ("Middle Color", Color) = (1,1,1,1)
		_ShallowColor ("Shallow Color", Color) = (1,1,1,1)
		_DeepestPoint("deep point", float) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Causticsurf finalcolor:causColor vertex:causVert

		sampler2D _MainTex;
		float _DeepestPoint;
	
		float4 _DeepColor;
		float4 _MiddleColor;
		float4 _ShallowColor;
		struct Input {
			half2 fog;
			float2 uv_MainTex;
		};

		void causVert (inout appdata_full v, out Input data)
	    {
	          float4 wp = mul(_Object2World,v.vertex);
	          data.fog.y = clamp(1+wp.y/_DeepestPoint,0,1);
	    }
	    
	    half4 LightingCausticsurf (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten){
	     	half4 c;
	     	c.rgb = s.Albedo;
	     	return c;
	    }
	    
	    void causColor (Input IN, SurfaceOutput o, inout fixed4 color){
     	
	     	half4 waterColor = lerp(_DeepColor,_ShallowColor,2 * IN.fog.y);
	        //waterColor = lerp(waterColor,_ShallowColor,-1 + IN.fog.y*2);
	        //waterColor = clamp(waterColor,0,1);
	        
	        half4 outCol = waterColor;
	        //outCol.a = 1-IN.fog.x;
	     	color =  waterColor;
	     }
   
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

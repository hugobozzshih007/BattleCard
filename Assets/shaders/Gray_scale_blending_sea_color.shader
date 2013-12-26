Shader "Grey Scale/Terrain_BlendingTextures_island" {
Properties {
	_Control ("Control (RGBA)", 2D) = "red" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
	// used in fallback on old cards
	_MainTex ("BaseMap (RGB)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	_RedColor ("Red Color", Color) = (1,1,1,1)
	_GreenColor ("Green Color", Color) = (1,1,1,1)
	_BlueColor ("Blue Color", Color) = (1,1,1,1)
	
	_DeepColor ("Deep Color", Color) = (1,1,1,1)
	_ShallowColor ("Shallow Color", Color) = (0,0,0,0)
	
	_DeepestPoint("deep point", float) = 10
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf Lambert

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_MainTex : TEXCOORD4;
};

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_MainTex ;
float4 _Color, _RedColor, _GreenColor, _BlueColor;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	fixed3 col;
	//col = splat_control.a * tex2D (_MainTex, IN.uv_MainTex).rgb* _Color;
	col = splat_control.r * tex2D (_Splat0, IN.uv_Splat0).rgb* _RedColor ;
	col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1).rgb* _GreenColor;
	col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2).rgb* _BlueColor;
	o.Albedo = (col.r+col.g+col.b)/3;
	o.Alpha = 1.0;
}
ENDCG 

 Blend One OneMinusSrcAlpha
	//ZWrite Off
	CGPROGRAM
	#pragma surface surf Causticsurf finalcolor:causColor vertex:causVert
	
	float _DeepestPoint;
	
	float4 _DeepColor;
	float4 _MiddleColor;
	float4 _ShallowColor;
	
	struct Input {
		half2 fog;
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
     	
     	half4 waterColor = lerp(_DeepColor,_ShallowColor,1*IN.fog.y);
       	//waterColor = lerp(waterColor,_ShallowColor, IN.fog.y);
        waterColor = clamp(waterColor,0,1);
        
        half4 outCol = waterColor;
        outCol.a = 0.0;
     	color =  waterColor;
     }
     
     void surf (Input IN, inout SurfaceOutput o) {
     	//half noise=0.9 + 0.5 * sin(_Time*_CausticSpeed*100 +IN.causUV.x*150 * IN.causUV.y*65 * _causticsTileMult);
     	//half texcaustic = tex2D (_CausticTex, IN.causUV.xy).g;
     	//o.Albedo = texcaustic;
     }
     
     ENDCG
   
}

// Fallback to Diffuse
Fallback "Diffuse"
}


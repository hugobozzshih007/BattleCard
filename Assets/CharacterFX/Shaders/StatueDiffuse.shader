Shader "Character/StatueDiffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Diffuse (RGB)", 2D) = "white" {}
		_GrungeTex ("Grunge (RGB)", 2D) = "white" {}
		_BaseTex ("Base (RGB)", 2D) = "white" {}
		_Scale("Scale", Float) = 1
		_Tighten("Tighten",Range(0.1,0.45))=0.3
		_DiffuseAmount("Diffuse Amount", Range(0,1)) = 0.2
		_GrungeAmount("Grunge Amount", Range(0.01,2)) = 1
	}
	
	SubShader
	{   
			Tags { "RenderType"="Opaque" }
			LOD 200
 
		CGPROGRAM
		#pragma surface surf Lambert nolightmap  
		
		sampler2D _MainTex; 
		sampler2D _GrungeTex;
		sampler2D _BaseTex;
		float4 _Color;
		float  _GrungeAmount;
		float  _DiffuseAmount;
		float  _Scale;
        float  _Tighten; 
 
		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		} ;
 
		void surf (Input IN, inout SurfaceOutput o)
		{
		    // tighten the blending zone
   			float3 blend_weights = abs(IN.worldNormal)- _Tighten;
		    // 
		    blend_weights = max(blend_weights, 0);
   			blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z).xxx;

            float4 d = tex2D(_MainTex, IN.uv_MainTex);
            float4 grunge = tex2D(_GrungeTex, IN.worldPos.xz/_Scale);
			float4 cy =  lerp(tex2D(_BaseTex, IN.worldPos.xz/_Scale),grunge,_GrungeAmount);
			float4 cz = tex2D(_BaseTex, IN.worldPos.xy/_Scale);
			float4 cx = tex2D(_BaseTex, IN.worldPos.zy/_Scale);
            float4 result = cx.xyzw * blend_weights.xxxx +  
                            cy.xyzw * blend_weights.yyyy +  
                            cz.xyzw * blend_weights.zzzz;

            o.Albedo = lerp(result,d,_DiffuseAmount) * _Color;
		}
		ENDCG
	}
 
	Fallback "VertexLit"
}
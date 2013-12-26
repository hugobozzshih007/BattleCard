Shader "Character/StatueBmp" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Diffuse (RGB)", 2D) = "white" {}
		_GrungeTex ("Grunge (RGB)", 2D) = "white" {}
		_BumpMap ("Bump (RGB) Illumin (A)", 2D) = "bump" {}
		_BaseTex ("Base (RGB)", 2D) = "white" {}
		_Scale("Scale", Float) = 1
		_Tighten("Tighten",Range(0.1,0.45))=0.3		
		_DiffuseAmount("Diffuse Amount", Range(0,1)) = 0.2
		_GrungeAmount("Grunge Amount", Range(0,2)) = 1
	}
	
	SubShader
	{  
			Tags { "RenderType"="Opaque" }
			LOD 200
 
		CGPROGRAM
		#pragma debug
		#pragma surface surf Lambert nolightmap
        #pragma target 3.0
		
		sampler2D _MainTex; 
		sampler2D _GrungeTex;
		sampler2D _BumpMap;
		sampler2D _BaseTex;
		fixed4 _Color;
		fixed _GrungeAmount;
		fixed _DiffuseAmount;
		half  _Scale;
		half _Tighten;
 
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			half3 worldPos;
			fixed3 worldNormal;	INTERNAL_DATA		
		};
 
		void surf (Input IN, inout SurfaceOutput o)
		{
		    // tighten the blending zone
		    float4 nrm =  tex2D (_BumpMap,IN.uv_BumpMap); 
		    o.Normal = UnpackNormal(nrm); 
		    float3 realNormal = WorldNormalVector(IN,o.Normal);
		    
   			float3 blend_weights = abs(realNormal)- _Tighten;
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

            result = lerp(result,d,_DiffuseAmount);	
            
            o.Albedo = result.rgb * _Color.rgb;
            o.Alpha = result.a * _Color.a;

		}
		ENDCG
	}
 
	Fallback "VertexLit"
}
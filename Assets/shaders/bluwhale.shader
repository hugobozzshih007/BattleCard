Shader "Underwater/Blue_whale" {
    Properties {
	_Shininess ("Shininess", float) = 0.078125
	_SpecularMult ("Specular Intensity", float) = 1.0
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_SpecTex ("Specular (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_CausticTex ("Caustic Texture", 2D) = "black" {}
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 400
		
	CGPROGRAM
	//#pragma surface surf BlinnPhong
	#pragma surface surf WemoWhale
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	
	float4 _ambLightColor;
	float _ambLightMult;
	
	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
	};
    half4 LightingWemoWhale (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten ){
    	//#ifndef USING_DIRECTIONAL_LIGHT
    	lightDir = normalize(lightDir);
	    //#endif
	    half4 ambV = mul(_World2Object, float4(-1,-5,-1,1));
	    half3 ambDir = normalize(ambV.rgb);
	    viewDir = normalize(viewDir);
	    half diff = dot(s.Normal, lightDir );
	    half amb = max(dot(s.Normal, ambDir ),0);
	    half4 ambColor =  _ambLightMult*_ambLightColor*amb; 
	    half4 c;
	    c.rgb = (s.Albedo * _LightColor0.rgb * diff +ambColor ) * (atten * 2);
	    c.a = s.Alpha; 
	    return c*0.5;
    }
    
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = tex.rgb;
		o.Alpha = tex.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	
	
	Blend One OneMinusSrcAlpha
	//ZWrite Off
	CGPROGRAM
	#pragma surface surf Specsurf
	sampler2D _SpecTex;
	sampler2D _BumpMap;
	half _Shininess;
	half _SpecularMult;

	struct Input {
		float2 uv_BumpMap;
		float2 uv_SpecTex;
	};
     half4 LightingSpecsurf (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten ){
    	#ifndef USING_DIRECTIONAL_LIGHT
    	lightDir = normalize(lightDir);
	    #endif
	    viewDir = normalize(viewDir);
	    half3 h = normalize( lightDir + viewDir );
	    //half diff = dot(s.Normal, lightDir );
	    float nh =  dot( h, s.Normal ) ;
	    float spec = pow( nh, s.Specular*100 );
	    half4 c;
	    c.rgb = (s.Albedo * spec*_SpecularMult) * (atten * 2);
	    // specular passes by default put highlights to overbright
	    c.a = max(spec,0.1); 
	    return c;
    }
     void surf (Input IN, inout SurfaceOutput o) {
		fixed3 specTex = tex2D(_SpecTex, IN.uv_SpecTex).rgb;
		o.Albedo = specTex;
		o.Specular = _Shininess;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	
	Blend One OneMinusSrcAlpha
	//ZWrite Off
	CGPROGRAM
	#pragma surface surf Causticsurf finalcolor:causColor vertex:causVert
	sampler2D _CausticTex;
	
	float4x4 _lightMatrix;
	float _DepthMax;
	float _DeepestPoint;
	float _causticsTileMult;
	float _CausticSpeed;
	float _causticsMult;
	float _CausticMult;
	float _keyLightMult;
	float _DepthPower;
	float4 _lightDir;
	float4 _DeepColor;
	float4 _MiddleColor;
	float4 _ShallowColor;
	
	struct Input {
		float2 causUV;
		float2 uv_CausticTex;
		float3 normal;
		half2 fog;
	};
	
	void causVert (inout appdata_full v, out Input data)
    {
          float4 wp = mul(_Object2World,v.vertex);
          
          float dis = length(_WorldSpaceCameraPos - wp.xyz);
          
         
          data.fog.x = pow(clamp(1-dis/_DepthMax,0,1),_DepthPower);
          data.fog.y = clamp(1+wp.y/_DeepestPoint,0,1);
          
          data.normal = v.normal;
          float2 causUV = mul (_lightMatrix, wp);
          data.causUV.x = _causticsTileMult * causUV.x+ _Time + 0.02*sin(_Time*_CausticSpeed*15 +causUV.y*40 * _causticsTileMult);
          data.causUV.y = _causticsTileMult * causUV.y+ 0.02*cos(_Time*_CausticSpeed*15 +causUV.x*40 * _causticsTileMult);
     }
     
     half4 LightingCausticsurf (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten){
     	half4 c;
     	c.rgb = s.Albedo;
   
     	return c;
     }
     
     void causColor (Input IN, SurfaceOutput o, inout fixed4 color){
     	
     	half4 waterColor = lerp(_DeepColor,_MiddleColor,2 * IN.fog.y);
        waterColor = lerp(waterColor,_ShallowColor,-1 + IN.fog.y*2);
        waterColor = clamp(waterColor,0,1);
        float3 dir = mul(_World2Object, -_lightDir);
        float diff2 = saturate(dot(normalize(IN.normal),normalize(dir)));
        half4 srfCol =diff2*color*_CausticMult;
        half4 outCol = waterColor*(1-IN.fog.x) + IN.fog.x *srfCol*(_keyLightMult*IN.fog.x);
        outCol.a = 1-IN.fog.x;
     	color =  outCol;
     }
     
     void surf (Input IN, inout SurfaceOutput o) {
     	//half noise=0.9 + 0.5 * sin(_Time*_CausticSpeed*100 +IN.causUV.x*150 * IN.causUV.y*65 * _causticsTileMult);
     	half texcaustic = tex2D (_CausticTex, IN.causUV.xy).g;
     	o.Albedo = texcaustic;
     }
     
     ENDCG
	
	
	}
	
	FallBack "Specular"
}
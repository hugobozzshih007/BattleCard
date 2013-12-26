  Shader "Character/CharShader-lambertwrap" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _MaskTex ("MaskTex (RGBA)", 2D) = "black" {}
      _EyeColor   ("Eye Color", Color) = (0.5,0.5,0.5,1)
      _SkinColor  ("Skin Color", Color) = (0.5,0.5,0.5,1)
      _HairColor  ("Hair Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM

      #pragma surface surf WrapLambert nolightmap
      
      half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
          half NdotL = dot (s.Normal, lightDir);
          half diff = NdotL * 0.5 + 0.5;
          half4 c;
          c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
          c.a = s.Alpha;
          return c;
      }      
          
      struct Input {
        float2 uv_MainTex;
      };
      
      sampler2D _MainTex;
      sampler2D _MaskTex;
      
      float4    _EyeColor;
      float4    _SkinColor;
      float4    _HairColor;
      float4    _AccentColor;
      
      void surf (Input IN, inout SurfaceOutput o) 
      {
      	float4 basecol = tex2D (_MainTex, IN.uv_MainTex);
      	float4 maskcol = tex2D (_MaskTex, IN.uv_MainTex);
      	float3 newcol;
      
      	if (maskcol.r > 0)
      	{
      		float3 graycol = dot(basecol.rgb,float3(0.3,0.59,0.11));
      		newcol = graycol * 2 * _EyeColor.rgb;
      		basecol.rgb = lerp(basecol,newcol,maskcol.r); 
      	}
      	
      	if (maskcol.g > 0)
      	{
      	    newcol = basecol * 2 * _SkinColor.rgb;
      		basecol.rgb = lerp(basecol,newcol,maskcol.g);
      	}
      	if (maskcol.b > 0)
      	{
      		newcol = basecol * 2 * _HairColor.rgb;
      		basecol.rgb = lerp(basecol,newcol,maskcol.b);
      	}
      	      	
        o.Albedo = basecol;
  		o.Gloss = basecol.a;
		o.Specular =  basecol.a;        
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
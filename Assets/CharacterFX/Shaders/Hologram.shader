  Shader "Character/Hologram" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _RimColor ("Rim Color", Color) = (0.26,0.7,1.0,0.0)
      _RimPower ("Rim Power", Range(0.1,8.0)) = 3.0
      _ClipPower("Clip Power", Range(0.0,301.0)) = 200.0
      _Brightness ("Brightness",Range(0.0,3.0)) = 1.5   
      _DiffuseAmount("Diffuse Amount", Range(0.0,1.0)) = 0.0   
    }
    SubShader {
      Tags { "RenderType" = "Transparent"  "Queue"="Transparent" "IgnoreProjector"="True"}
      
    // extra pass that renders to depth buffer only
     Pass {
        ZWrite On
        ColorMask 0
       }
          
      CGPROGRAM
      #pragma surface surf Lambert alpha noambient nolightmap nodirlightmap  novertexlights
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
          float3 worldPos;    
          float4 screenPos;      
      };
      sampler2D _MainTex;
      sampler2D _BumpMap;
      float4 _RimColor;
      float _RimPower;
      float _ClipPower;
      float _Brightness;
      float _DiffuseAmount;

      void surf (Input IN, inout SurfaceOutput o) {
          float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
          
          if (_ClipPower <= 300.0f)
             clip (frac(screenUV.y * _ClipPower) - 0.5);
      	  half4 basecol = tex2D (_MainTex, IN.uv_MainTex);
     	  half3 graycol = dot(basecol.rgb,float3(0.3,0.59,0.11));
          o.Albedo = basecol;//lerp(graycol,basecol,_DiffuseAmount);
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Emission = lerp(_RimColor.rgb * pow (rim, _RimPower) * _Brightness,basecol,_DiffuseAmount);
          o.Alpha = o.Emission;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
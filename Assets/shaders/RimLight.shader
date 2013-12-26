Shader "Custom/Rim" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _RimTex ("Fresenel", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader {
      Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      ZWrite Off
      Blend One OneMinusSrcColor
      //Cull off
      CGPROGRAM
      #pragma surface surf Lambert alpha
      struct Input {
          float2 uv_MainTex;
          float2 uv_RimTex;
          float3 viewDir;
      };
      sampler2D _MainTex;
      sampler2D _RimTex;
      float4 _RimColor;
      float _RimPower;
      void surf (Input IN, inout SurfaceOutput o) {
          half rim_inside = saturate(dot (normalize(IN.viewDir), o.Normal));
          half rim = 1-rim_inside;
          half rim_pow = pow (rim_inside, _RimPower);
          half4 rimTex = tex2D(_RimTex, rim.xx);
          half4 rimTex_inside = tex2D(_MainTex, rim_pow.xx);
          o.Alpha = rim + rim_pow;
          o.Albedo = rimTex_inside+rimTex;
          o.Emission = _RimColor.rgb * pow (rim, _RimPower);
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }

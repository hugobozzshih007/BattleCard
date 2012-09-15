Shader "map/illuminate" {
    Properties
	{
      _MainTex ("Texture", 2D) = "white" {}
      _MainTexColor ("Texture Mult", Color) = (1.0,1.0,1.0,1.0)
      _exl ("Brightness", float) = 1.0
      _alphaTest ("Alpha cutoff", Range (0,1)) = 0.5
    }
    SubShader 
	{
		pass{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater [_alphaTest]
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				float4 _MainTexColor;
				float _exl;
			  
				struct v2f 
				{
					float4  pos : SV_POSITION;
					float2  uv : TEXCOORD0;
				};
			  
				float4 _MainTex_ST;

				v2f vert (appdata_base v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
					float4 wp = mul(_Object2World,v.vertex);				
					return o;
				}

				half4 frag (v2f i) : COLOR
				{
					half4 texCol = tex2D (_MainTex, i.uv) * _MainTexColor;	
					half4 outCol = texCol*(1.0+texCol.a*_exl);
					outCol.a = texCol.a;
					
					return  outCol;
				}
				
			ENDCG
		}
    }
    Fallback "Diffuse"
  }
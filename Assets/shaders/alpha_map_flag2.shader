Shader "map/mixed_Flag2" {
    Properties
	{
      _MainTex ("Texture", 2D) = "white" {}
      _MainTex2 ("Texture", 2D) = "white" {}
      _MainTexColor ("Texture Mult", Color) = (1.0,1.0,1.0,1.0)
      _alpha ("Alpha", float) = 1.0
    }
    SubShader 
	{
		pass{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				sampler2D _MainTex2;
				float4 _MainTexColor;
				float _alpha;
			  
				struct v2f 
				{
					float4  pos : SV_POSITION;
					float4  uv : TEXCOORD0;
				};
			  
				float4 _MainTex_ST;
				float4 _MainTex2_ST;

				v2f vert (appdata_base v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv.xy = TRANSFORM_TEX (v.texcoord, _MainTex);
					o.uv.zw = TRANSFORM_TEX (v.texcoord, _MainTex2);
					float4 wp = mul(_Object2World,v.vertex);				
					return o;
				}

				half4 frag (v2f i) : COLOR
				{
					half4 texCol = tex2D (_MainTex, i.uv.xy);
					half4 texCol2 = tex2D (_MainTex2, i.uv.zw) * _MainTexColor;	
					half4 outCol = (texCol.a+texCol2)*texCol;
					outCol.a = (texCol2.a+texCol.a)*_alpha;
					//outCol.rgb += texCol.rgb;
					return outCol;
				}
				
			ENDCG
		}
    }
    Fallback "Diffuse"
  }
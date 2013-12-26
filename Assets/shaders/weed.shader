Shader "Underwater/VegetationDispTex_IOS" {
    Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _MainTexColor ("Texture Mult", Color) = (1.0,1.0,1.0,1.0)
		_NoiseAmp ("Noise Amplitude", float) = 6.0
		_NoiseFreq ("Noise Frequency", float) = 0.04
		_NoiseSpeed ("Noise Speed", float) = 22
		_NoiseYMax ("Noise Y Max", float) = 100
		_TilingX ("Tiling X", float) = 1
		_TilingY ("Tiling Y", float) = 1
	    _keyLightMult ("key Light Mult", float)  = 1.0
    }
    SubShader 
	{
		pass{
			Tags { "Queue" = "Opaque"}
			Cull OFF
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTexColor;
				float4 _DeepColor;
				float4 _MiddleColor;
				float4 _ShallowColor;
				float _DepthMax;
				float _DeepestPoint;
				float _DeepestPointBrightness;
				float _DepthPower;
				float _NoiseAmp;
				float _NoiseFreq;
				float _NoiseSpeed;
				float _NoiseYMax;
				float _keyLightIntensity;
				float4 _keyLightColor;
				float _keyLightMult;
			
				float _TilingX;
				float _TilingY;
				
				struct appdata_uv2 {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
					float4 color : COLOR;
					float4 uvrand : TEXCOORD1;
				};
				struct v2f 
				{
					float4  pos : SV_POSITION;
					float2  uv : TEXCOORD0;
					float4  util : TEXCOORD1;
					float4  uv2 : TEXCOORD2;
					float4  vertcol : TEXCOORD3;
				};
			  

				v2f vert (appdata_uv2 v)
				{
					v2f o;
					float4 os = v.vertex; 
					float zRamp = smoothstep(0,_NoiseYMax,os.z);
					os.x += _NoiseAmp*sin(os.z*_NoiseFreq + _Time*_NoiseSpeed) * zRamp;
					os.y += _NoiseAmp*sin(os.z*_NoiseFreq*1.2 + _Time*_NoiseSpeed) * zRamp;
				
					o.uv2 = v.color;
					o.pos = mul (UNITY_MATRIX_MVP, os);
					o.uv = v.texcoord;
					
					float4 wp = mul(_Object2World,os);
					float dis = length(_WorldSpaceCameraPos - wp.xyz);
					o.util.x = pow(clamp(1-dis/_DepthMax,0,1),_DepthPower); //depth
					o.util.y = clamp(1+wp.y/_DeepestPoint,0,1); //deep
					o.util.z = 0.2+0.8*zRamp;
					o.vertcol = v.color;

					return o;
				}

				half4 frag (v2f i) : COLOR
				{
				    float uShadingFactor = sin(2*(i.uv.y)) ;
					i.uv.x*= _TilingX;
					i.uv.y*= _TilingY;
					half4 diffcol = tex2D (_MainTex, (i.uv)) * _MainTexColor * i.vertcol;
					
					half4 outCol = diffcol;  					
					half4 waterColor = lerp(_DeepColor,_MiddleColor,2 * i.util.y);
					waterColor = lerp(waterColor,_ShallowColor,-1 + i.util.y*2);
					waterColor = clamp(waterColor,0,1);
					half4 col = max(i.util.y,_DeepestPointBrightness) * diffcol* uShadingFactor * i.util.z * _keyLightIntensity * _keyLightColor * _keyLightMult * i.vertcol;
					outCol.rgb = waterColor.rgb*(1-i.util.x) + i.util.x * col;
					return outCol;
				}
				
			ENDCG
		}
    }
    Fallback "Diffuse"
  }
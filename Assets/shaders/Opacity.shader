Shader "Underwater/MoonJellyOuter" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
    _OpacMult ("Opacity Mult", float) = 0.5
	_Color ("Main Color", Color) = (1,1,1,1)
	_strength("rim strength", float) = 20
	_thick("rim thick", float) = 5
}


Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	//Tags { "RenderType" = "Opaque" }
		//Blend One One
		Blend SrcAlpha One
		//Cull Off	
		ZWrite off
		//ZTest LEqual
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			//float4 _lightDir;
				
			sampler2D _MainTex;
			float4 _TintColor;
			float _OpacMult;
			float4 _Color;
			float _thick;
			float _strength;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4  wp : TEXCOORD1;
				float3  wn : TEXCOORD2;
				//float4  tan : TEXCOORD3;
			};

			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.wp = mul(_Object2World,v.vertex);
				o.wn = mul((float3x3)_Object2World,v.normal.xyz);
				//o.wn = v.normal.xyz;
				
				//float NdotL = max(0.2,(dot(-normalize(float3(_lightDir.x,_lightDir.y,_lightDir.z)),normalize(o.wn.xyz))+0.8)/1.8);
				//float NdotL = abs((dot(-normalize(float3(_lightDir.x,_lightDir.y,_lightDir.z)),normalize(o.wn.xyz))));
				o.wp.w = 1;//NdotL;
				//o.tan = v.tangent;
				return o;
			}

			
			half4 frag (v2f i) : COLOR
			{
				float3  eyeDirection = _WorldSpaceCameraPos - i.wp.xyz;
				float dis = length(eyeDirection);
				float f = dot(normalize(i.wn),normalize(eyeDirection));
				float fresnel =  1 - _strength* pow(f,_thick)  ;
				half4 prev = tex2D(_MainTex, fresnel.xx)* _Color  *fresnel * i.wp.w;
				//prev.a = fresnel * _OpacMult  ;
				//prev.rgb *= prev.a;
				prev.a = fresnel * _OpacMult;
				return prev;
			}
			ENDCG 
		}
	} 	

	
}
}
Shader "Underwater/SoftGlow" {
Properties {
	//_MainTex ("Particle Texture", 2D) = "white" {}
    _OpacMult ("Opacity Mult", float) = 0.5
	_Color ("Main Color", Color) = (1,1,1,1)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend One OneMinusSrcColor
	//Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	//Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
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

			//sampler2D _MainTex;
			float4 _TintColor;
			float _DepthMax;
			float _DepthPower;
			float _OpacMult;
			float4 _Color;
			
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
				float4  tan : TEXCOORD3;
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				//o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.wp = mul(_Object2World,v.vertex);
				o.wn = mul((float3x3)_Object2World,v.normal.xyz);
				//o.wn = v.normal.xyz;
				o.tan = v.tangent;
				return o;
			}

			
			half4 frag (v2f i) : COLOR
			{
				float3  eyeDirection = _WorldSpaceCameraPos - i.wp.xyz;
				float dis = length(eyeDirection);
				float fade = pow(clamp(1-dis/_DepthMax,0,1),_DepthPower); //depth	
				float fresnel =  0.6 *  pow(clamp(-.05 + dot(normalize(i.wn),normalize(eyeDirection)),0,1),2);
				half4 prev = fade * _OpacMult  * fresnel * _Color; //tex2D(_MainTex, i.texcoord)
				prev.rgb *= prev.a;
				return prev;
			}
			ENDCG 
		}
	} 	

	
}
}
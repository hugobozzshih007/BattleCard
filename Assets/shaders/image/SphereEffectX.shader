// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'

Shader "Hidden/SphereEffectX" {

Properties {
    _MainTex ("Base (RGB)", RECT) = "white" {}
	TopTexture ("Base (RGBA)", 2D) = "white" {}
    rampTexture ("Base (RGB)", 2D) = "rampTexture" {}
}

 

SubShader{
    Pass{
        ZTest Always Cull Off ZWrite Off
        Fog { Mode off }

		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#include "UnityCG.cginc"
		uniform sampler2D _MainTex;
		uniform sampler2D TopTexture;
		uniform sampler2D rampTexture;
		uniform float _RampOffset;
		uniform float m_angle = 0;
		uniform float m_radius = .5;
		uniform float4 m_colorGood = float4(1,1,0,1);
		uniform float4 m_colorBad   = float4(1,0,0,1);
		
		float4 frag (v2f_img i) : COLOR
		{
		    float4 rc = float4(1,0,0,0);
		    float2 distance = i.uv - float2(0.5,0.5);
		    float d = length(distance);
		    //get start angle
		    float rad = atan2(distance.x,distance.y);
		    
		    if(rad < 0){
		        rad += (3.14 * 2);
		    }
		
		    float angle = (rad * 180) / 3.14;
		
		    if(d < m_radius){
		        rc = m_colorGood;
		        if(angle > m_angle){
		            rc = m_colorBad;
		        }
		    }
			float4 rcColor = rc *tex2D(rampTexture, i.uv);
			float4 trueColor = tex2D(TopTexture, i.uv);
			float4 finalColor = trueColor + rcColor; 
			finalColor.a = trueColor.a; 
		    return finalColor;
		}
		ENDCG
    }
}
Fallback off
}
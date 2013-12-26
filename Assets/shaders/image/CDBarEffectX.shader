// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'

Shader "Hidden/CDBarEffectX" {

Properties {
    _MainTex ("Base (RGB)", RECT) = "white" {}
	TopTexture ("Base (RGBA)", 2D) = "white" {}
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
		uniform float m_angle = 0;
		uniform float m_radius = .5;
		uniform float4 m_colorGood = float4(1,1,1,1);
		uniform float4 m_colorBad   = float4(0,0,0,1);
		uniform float4 m_colorSide = float4(1,0,0,1);
		
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
		    
			float4 trueColor = tex2D(TopTexture, i.uv)*float4(m_colorSide.rgb,1);
			float4 GrayColor = dot(trueColor.rgb, float3(0.3, 0.59, 0.11))*float4(0.35,0.35,0.35,1);
			GrayColor.a = trueColor.a;
			GrayColor*=(1-rc.b);
			float4 finalColor = float4(trueColor.rgb, rc.b*trueColor.a);
			finalColor*=rc.b;
		    return finalColor+GrayColor;
		}
		ENDCG
    }
}
Fallback off
}
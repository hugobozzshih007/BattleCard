Shader "PlaneShadows" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (1,1,1,1)
        _planeNormal ("Plane Normal", Vector) = (0,0,0,0)
        _shadowBias ("Bias", Float) = 0.01
    }
    SubShader {
        Pass {
            Tags{ "RenderType" = "Opaque" "Queue" = "Opaque" }          
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma exclude_renderers xbox360
                #pragma exclude_renderers ps3
                #pragma target 3.0
 
                float4 _planeNormal;
                float4 _ShadowColor;
                float _shadowBias;
 
                float4x4 _projectionMatrix;
                float4x4 _viewInv;
                float4x4 _view;
 
                // vertex input: position, UV
                struct appdata {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };
                struct v2f {
                    float4 pos : SV_POSITION;
                    float4 uv : TEXCOORD0;
                };              
                v2f vert (appdata v) {
                    v2f o;
 
                    // Get model (world) matrix
                    float4x4 modelMatrix = mul(_viewInv, UNITY_MATRIX_MV);
 
                    // Transform to world
                    float4 vertex = mul (modelMatrix, v.vertex);
                    // Project to plane
                    float4 pos = mul (_projectionMatrix, vertex);
                    // Add shadow bias
                    pos += _planeNormal * _shadowBias;
                    // Transform to view
                    pos = mul (_view, pos);
                    // Project to screen
                    o.pos = mul (UNITY_MATRIX_P, pos);
 
                    return o;
                }               
                half4 frag( v2f i ) : COLOR {
                    return _ShadowColor;
                }               
            ENDCG
        }
    }
}

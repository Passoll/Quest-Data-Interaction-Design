Shader "Universal Render Pipeline/M_Skyshader"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1, 1, 1, 0)
        _HorizonColor ("Horizon Color", Color) = (1, 1, 1, 0)
        _BottomColor ("Bottom Color", Color) = (1, 1, 1, 0)
        _TopExponent ("Top Exponent", Float) = 0.5
        _BottomExponent ("Bottom Exponent", Float) = 0.5
        _AmplFactor ("Amplification", Float) = 1.0
    }

    SubShader
    {
        Tags {"RenderType" = "Background" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="4.5"}
        LOD 100

        ZWrite Off Cull Off 
        Fog { Mode Off }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            half _TopExponent;
            half _BottomExponent;
            float4 _TopColor;
            float4 _HorizonColor;
            float4 _BottomColor;
            half _AmplFactor;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float interpUv = normalize (IN.uv).y;
                // top goes from 0->1 going down toward horizon
                float topLerp = 1.0f - pow (min (1.0f, 1.0f - interpUv), _TopExponent);
                // bottom goes from 0->1 going up toward horizon
                float bottomLerp = 1.0f - pow (min (1.0f, 1.0f + interpUv), _BottomExponent);
                // last lerp param is horizon. all must add up to 1.0
                float horizonLerp = 1.0f - topLerp - bottomLerp;
                return (_TopColor * topLerp + _HorizonColor * horizonLerp + _BottomColor * bottomLerp) *
                  _AmplFactor;
            }
            ENDHLSL
        }
    }

}

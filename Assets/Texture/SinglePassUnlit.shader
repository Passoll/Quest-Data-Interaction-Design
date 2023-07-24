// This shader fills the mesh shape with a color predefined in the code.
Shader "Myshader/SinglePassUnlit"
{
   Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }
        SubShader{
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off
        // extra pass that renders to depth buffer only
        Pass{
            ZWrite On
            ColorMask 0
        }

        CGPROGRAM
        #pragma surface surf Lambert keepalpha
            fixed4 _Color;
        struct Input {
            float4 color : COLOR;
        };
        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
    }


        Fallback "Transparent/VertexLit"
}
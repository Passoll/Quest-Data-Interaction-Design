
Shader "Custom/ColorButton"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _BorderColor("BorderColor", Color) = (0, 0, 0, 1)
        
        _EdgeColor("EdgeColor", Color) = (0, 0, 0, 1)
        _IconColor("IconColor", Color) = (0, 0, 0, 1)
        // width, height, border radius, unused
        _Dimensions("Dimensions", Vector) = (0, 0, 0, 0)

        // radius corners
        _Radii("Radii", Vector) = (0, 0, 0, 0)

        // defaults to LEqual
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest [_ZTest]
        LOD 100
        Cull off 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_VERTEX_OUTPUT_STEREO
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                fixed4 borderColor : TEXCOORD1;
                fixed4 dimensions : TEXCOORD2;
                fixed4 radii : TEXCOORD3;
                fixed4 ecolor : TEXCOORD4;
                fixed4 icolor : TEXCOORD5;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _BorderColor)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Dimensions)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Radii)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _EdgeColor)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _IconColor)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.radii = UNITY_ACCESS_INSTANCED_PROP(Props, _Radii);
                o.dimensions = UNITY_ACCESS_INSTANCED_PROP(Props, _Dimensions);
                o.borderColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BorderColor);
                o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                o.ecolor = UNITY_ACCESS_INSTANCED_PROP(Props, _EdgeColor);
                o.icolor = UNITY_ACCESS_INSTANCED_PROP(Props, _IconColor);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv-float2(.5f,.5f))*2.0f*o.dimensions.xy;

                return o;
            }

             float sdRoundBox( in float2 p, in float2 b, in float4 r )
            {
	            // We choose the radius based on the quadrant we're in
                // We cap the radius based on the minimum of the box half width/height
                r.xy = (p.x>0.0)?r.xy : r.zw;
                r.x = (p.y>0.0)?r.x : r.y;
                r.x = min(2.0f*r.x, min(b.x, b.y));

                float2 q = abs(p)-b+r.x;
                return min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = sdRoundBox(i.uv, i.dimensions.xy - i.dimensions.ww * 2.0f, i.radii);
                float2 ddDist = float2(ddx(dist), ddy(dist));
                float ddDistLen = length(ddDist);

                float outerRadius = i.dimensions.w;
                float innerRadius = i.dimensions.z;

                float borderMask = (outerRadius > 0.0f | innerRadius > 0.0f)? 1.0 : 0.0;

                float outerDist = dist - outerRadius * 2.0;
                float outerDistOverLen = outerDist / ddDistLen;
                clip(1.0 - outerDistOverLen < 0.1f ? -1:1);

                float innerDist = dist + innerRadius * 2.0;
                float innerDistOverLen = innerDist / ddDistLen;

                float colorLerpParam = saturate(innerDistOverLen) * borderMask;
                float4 fragColor = lerp(i.color, i.borderColor, colorLerpParam);
                fragColor.a *= (1.0 - saturate(outerDistOverLen));
                
                fixed4 col = tex2D(_MainTex, i.uv / 2 + 0.5 );
                fixed4 outcolor = (1 - col.r) * i.icolor + (col.a - 1 + col.r) * i.ecolor + fragColor * (1 - col.a);
                return outcolor;
            }
            
            ENDCG
        }
    }
}



Shader "Custom/TilemapOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.1
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        // Outline Pass
        Pass
        {
            Name"Outline"
            ZWrite On
            ZTest LEqual
            Cull Front

            // Apply the offset to create the outline effect
            Offset 10, 10

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            // Outline color and width
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            // Vertex Shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Apply the outline offset
                o.pos.xy += float2(_OutlineWidth, _OutlineWidth);
                o.color = _OutlineColor;

                return o;
            }

            // Fragment Shader
            half4 frag(v2f i) : SV_Target
            {
                return i.color; // Output the outline color
            }
            ENDCG
        }

        // Main Pass
        Pass
        {
            Name"Main"
            ZWrite On
            ZTest LEqual
            Cull Front

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
                o.Alpha = 1.0;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}

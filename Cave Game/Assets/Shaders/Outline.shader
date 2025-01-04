Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 1.0
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            ZWrite On
            Cull Front
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset 5, 5

            CGPROGRAM
            #pragma surface surf Lambert

            struct Input
            {
                float2 uv_MainTex;
            };

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            void vert(inout appdata_full v)
            {
                // Scale the vertex positions along their normals to create an outline effect
                v.vertex.xyz += v.normal * _OutlineWidth;
            }

            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = _OutlineColor.rgb;
                o.Alpha = _OutlineColor.a;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}

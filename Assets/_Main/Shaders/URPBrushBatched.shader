Shader "Custom/URPBrushBatched"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _BrushTex("Brush Tex", 2D) = "white" {}
        _BrushColor("Brush Color", Color) = (0,0,0,1)
        _BrushSize("Brush Size", Float) = 0.1
        _PointCount("Point Count", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Name "BrushPass"
            Tags { "LightMode" = "UniversalForward" }

            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define MAX_POINTS 256

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _BrushTex;
            half4 _BrushColor;
            half _BrushSize;
            int _PointCount;
            float4 _UVPositions[MAX_POINTS];

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = tex2D(_MainTex, IN.uv);

                // Loop over brush points in this batch
                [loop]
                for (int i = 0; i < _PointCount; i++)
                {
                    half2 diff = IN.uv - _UVPositions[i].xy;
                    half2 brushUV = diff / (_BrushSize + 1.0e-6h) + 0.5h;

                    // Fast in-bounds check — avoids sampling brush if outside
                    if (brushUV.x < 0.0h || brushUV.x > 1.0h ||
                        brushUV.y < 0.0h || brushUV.y > 1.0h)
                        continue;

                    half brushAlpha = tex2D(_BrushTex, brushUV).a;

                    // Skip blending if alpha is zero
                    if (brushAlpha <= 0.0h)
                        continue;

                    col = lerp(col, _BrushColor, brushAlpha);
                }

                return col;
            }
            ENDHLSL
        }
    }
}

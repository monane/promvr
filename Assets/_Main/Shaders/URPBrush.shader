Shader "Custom/URPBrush"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _BrushTex("Brush Tex", 2D) = "white" {}
        _BrushColor("Brush Color", Color) = (0,0,0,1)
        _UVPosition("UV Position", Vector) = (0,0,0,0)
        _BrushSize("Brush Size", Float) = 0.1
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

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _BrushTex;
            half4 _BrushColor;
            float2 _UVPosition;
            float _BrushSize;

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

                float2 diff = IN.uv - _UVPosition;
                float2 brushUV = diff / (_BrushSize + 1.0e-6) + 0.5;

                half2 inBounds = step(half2(0,0), brushUV) * step(brushUV, half2(1,1));
                half mask = inBounds.x * inBounds.y;

                half4 brushSample = tex2D(_BrushTex, brushUV);
                
                half blendFactor = brushSample.a * mask;
                col = lerp(col, _BrushColor, blendFactor);
                
                return col;
            }
            ENDHLSL
        }
    }
}
// Sprite shader for the 2D Renderer that can flash a sprite in one colour.
//
// It is the URP "Sprite-Unlit-Default" shader plus two properties:
//   _FlashColor  the colour the sprite is pushed towards.
//   _FlashAmount 0 = the sprite as drawn, 1 = the whole sprite silhouette in _FlashColor (a white flash).
// A plain SpriteRenderer.color can only darken or tint a sprite, never turn it white, which is why a
// dedicated shader is needed. The amount is driven from a script (PlayerFeedbackController) through a
// MaterialPropertyBlock, so the material asset itself is never modified at run time.
Shader "Ashar/SpriteFlash"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _FlashColor ("Flash Colour", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0, 1)) = 0
        [MaterialToggle] _ZWrite("ZWrite", Float) = 0

        // Legacy properties, kept so the material can fall back to the standard sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            // NOTE: the properties are not inside an #ifdef, because the SRP batcher needs the same layout everywhere.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _FlashColor;
                half _FlashAmount;
            CBUFFER_END

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                half4 pixel = CommonUnlitFragment(input, input.color);
                // Push the colour towards the flash colour; the alpha is kept, so the sprite silhouette does not change.
                pixel.rgb = lerp(pixel.rgb, _FlashColor.rgb, _FlashAmount);
                return pixel;
            }
            ENDHLSL
        }
    }
}


Shader "Custom/Pixelate"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        // [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        _PixelsPerUnit("Pixels Per Unit", Float) = 16

        // [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        // [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        // [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

    }

    SubShader
    {

        Tags
		{ 
			// "Queue"="Transparent" 
			// "IgnoreProjector"="True" 
			// "RenderType"="Transparent" 
			// "PreviewType"="Plane"
			// "CanUseSpriteAtlas"="True"
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"

		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {

            // HLSLINCLUDE
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // TEXTURE2D(_MainTex)
            sampler2D _MainTex;
            SamplerState sampler_point_clamp;

            float4 _MainTex_ST;
            float _PixelsPerUnit;
            float _ScreenWidth;
            float _ScreenHeight;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float x = floor(i.uv.x * _PixelsPerUnit * _ScreenWidth) / (_PixelsPerUnit * _ScreenWidth);
                float y = floor(i.uv.y * _PixelsPerUnit * _ScreenHeight) / (_PixelsPerUnit * _ScreenHeight);
                // float y = floor(i.uv.y * _MainTex_ST.y * _PixelsPerUnit) / (_MainTex_ST.x * _PixelsPerUnit);
                // fixed4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, float2(x, y)); // tex2D(_MainTex, float2(x, y));
                fixed4 col = tex2D(_MainTex, float2(x, y));
                return col;
            }

            ENDCG
            // ENDHLSL
        }
    }
}

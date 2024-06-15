Shader "Custom/PosterizeValue"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        // [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            // Standard luminance.
            // float4 Luminance = (0.2125, 0.7154, 0.0721, 0);
            float Epsilon = 1e-10;

            float3 rgb2hsv(float3 c) {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                // Using the ternary to force a cmov in the compilation.
                float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
                float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);
                float d = q.x - min(q.w, q.y);
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + Epsilon)), d / (q.x + Epsilon), q.x);
            }

            float3 hsv2rgb(float3 c) {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 adjustHSV(float3 c, float s, float v) {
                float3 hsv = rgb2hsv(c.rgb);
                hsv.y *= s;
                hsv.z = saturate(hsv.z + v);
                return hsv2rgb(hsv);
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Value;


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 hsv = rgb2hsv(col.rgb);

                // Flatten the values.
                hsv.z = _Value;
                if (hsv.z < 0) {
                    hsv.z = 0;
                }
                col.rgb = hsv2rgb(hsv);

                // Keep only the values.
                // hsv.z *= 0.8;
                // col.rgb = pow(hsv.z, _Value);

                return col;
            }

            ENDCG
        }
    }
}

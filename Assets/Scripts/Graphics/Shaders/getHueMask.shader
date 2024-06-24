// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GetHueMask"
{
    Properties
    {
        
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        // [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        
        _HueLeft("Hue Left", float) = 0
        _HueRight("Hue Right", float) = 0
        _SaturationThreshold("Saturation Threshold", float) = 0
        _ValueThreshold("Value Threshold", float) = 0
        _RemoveColor("Remove Color", float) = 0

    }

    SubShader
    {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _HueLeft, _HueRight, _SaturationThreshold, _ValueThreshold;
            float _RemoveColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                // Note this interesting effect.
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 hsv = rgb2hsv(col);

                if (hsv.x > _HueLeft & hsv.x < _HueRight & hsv.y > _SaturationThreshold) {
                    col.rgb = 1 * _RemoveColor + col.rgb * (1 - _RemoveColor);
                }else {
                    col.rgb = 0;
                }
            
                return col;

            }
            ENDCG
        }
    }
}

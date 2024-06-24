// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GradientMap"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        
        _GradientMap("Gradient Map", 2D) = "white" {}
        // _TargetPalette("Target Palette", 2D) = "white" {}
        _ColorCount("Color Count", float) = 0
        // _PrecisionThreshold("Precision Threshold", float) = 0.05
        _TwoTone("_TwoTone", float) = 0
        _KeepBlack("_KeepBlack", float) = 0
        
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

            sampler2D _GradientMap;
            SamplerState sampler_point_clamp; // sampler_state poin
            float _ColorCount;
            float _TwoTone;

            float _KeepBlack;

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

                float black = col.r + col.g + col.b;

                if (_TwoTone > 0) {
                    fixed4 _col = tex2D(_GradientMap, float2(0.3, 0.5));

                    float3 hsvA = rgb2hsv(col);
                    float3 hsvB = rgb2hsv(_col);

                    float3 hsvC = float3(hsvA.xy, hsvB.z); // gets the outlines pretty nicely.
                    // float3 hsvC = float3(hsvB.xy, hsvA.z);
                    col.rgb = hsv2rgb(hsvC);
                }
                
                // fixed4 o = col;

                // // if there are 3 colors then 0.2 => 0th index
                // // 0.9 => *3 = 2.7 and floor
                // fixed4 col = tex2D(_MainTex, i.uv);

                float minSample = floor(col.r * (_ColorCount-1)) / (_ColorCount-1);
                float maxSample = ceil(col.r * (_ColorCount-1)) / (_ColorCount-1);

                fixed4 colA = tex2D(_GradientMap, float2(minSample, 0));
                fixed4 colB = tex2D(_GradientMap, float2(maxSample, 0));

                float t0 = col.r / _ColorCount;
                fixed4 o = t0 * colA + (1-t0) * colB;

                if (black < 0.01 & _KeepBlack > 0) {
                    o.rgb = float3(0, 0, 0);
                }

                return o;

            }
            ENDCG
        }
    }
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RadialSwapShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        // The palettes.
        _BasePalette("Base Palette", 2D) = "white" {}
        _TransitionPalette("TransitionPalette", 2D) = "white" {}
        _TargetPalette("Target Palette", 2D) = "white" {}
        
        // The number of colors in the palette.
        _ColorCount("Color Count", float) = 0

        // The radius between which the transition happens.
        _Radius("Radius", float) = 0

        // The center of where the radial transition occurs.
        _Origin("Origin", Vector) = (0, 0, 0, 0)

        //
        _SaturationRatioPerZ("Saturation Ratio Per Z", float) = 1

        //
        _ValueAddedPerZ("Value Added Per Z", float) = 0

        //
        _PrecisionThreshold("Precision Threshold", float) = 0.05

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

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            // --- RADIAL TRANSITION --- //

            float distToCenter(float4 vertex, float4 center, float scale) {
                return length(vertex.xy / scale + center);
            }

            // --- COLOR MATCHING --- //

            float _PrecisionThreshold;
            
            bool same(fixed4 colA, fixed4 colB) {
                bool r = abs(colA.r - colB.r) < _PrecisionThreshold;
                bool g = abs(colA.g - colB.g) < _PrecisionThreshold;
                bool b = abs(colA.b - colB.b) < _PrecisionThreshold;
                bool a = colA.a > _PrecisionThreshold;
                return r & g & b & a;
            };

            bool same2(fixed4 colA, fixed4 colB) {
                bool s = distance(colA.rgb, colB.rgb) < _PrecisionThreshold;
                bool a = colA.a > _PrecisionThreshold;
                return s & a;
            };

            bool same3(fixed4 colA, fixed4 colB) {
                float3 d = colA.rgb - colB.rgb;
                bool s1 = abs(d.r - d.b) < _PrecisionThreshold; 
                bool s2 = abs(d.b - d.g) < _PrecisionThreshold;
                bool a = colA.a > _PrecisionThreshold;
                return s1 & s2 & a;
            };

            // --- HSV (and those kinds of things)--- //
            // From https://www.chilliant.com/rgb2hsv.html, by Ian Taylor, and
            // From https://web.archive.org/web/20200207113336/http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl.

            // Standard luminance.
            // float4 Luminance = (0.2125, 0.7154, 0.0721, 0);
            float Epsilon = 1e-10;

            // Fast branchless RGB to HSV conversion in GLSL
            // Modified for HLSL
            // Porting to HLSL is straightforward: replace vec3 and vec4 with float3 and float4, mix with lerp, fract with frac, and clamp(…, 0.0, 1.0) with saturate(…).
            // From https://web.archive.org/web/20200207113336/http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl.
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

            // The texture being sampled.
            sampler2D _MainTex;
            float4 _MainTex_ST;

            // The palettes.
            sampler2D _BasePalette;
            sampler2D _TargetPalette;
            sampler2D _TransitionPalette;

            // The number of colors in the palette.
            float _ColorCount;

            // The radius between which the transition happens.
            float _Radius;

            // The center of where the radial transition occurs.
            float4 _Origin;

            // The saturation of the output color as a ratio of its original color.
            float _SaturationRatioPerZ;
            float _ValueAddedPerZ;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                
                float dist = distToCenter(i.vertex, _Origin, _ScreenParams.x);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Itterate through the possible colors to see if any match.
                bool swapped = false; // I need this because it gets angry if I try to return within the loop :(
                for (uint x=0; x < _ColorCount; x++) {
                    if (!swapped) {
                        fixed4 base = tex2D(_BasePalette, float2(x / _ColorCount, 0));
                        fixed4 target = tex2D(_TargetPalette, float2(x / _ColorCount, 0));
                        fixed4 transition = tex2D(_TransitionPalette, float2(x / _ColorCount, 0));
                        if (same(col, base)) {
                            col = target * col.a; 
                            // if (dist < _Radius * _Radius) { col = target * col.a; }
                            // else { col = transition * col.a; }
                            swapped = true;
                        }
                    }
                }
                
                if (i.worldPos.z > 0) {
                    col.rgb = adjustHSV(col.rgb, 1 + _SaturationRatioPerZ * (abs(i.worldPos.z) - 50), _ValueAddedPerZ * (abs(i.worldPos.z) - 50));
                }

                return col * col.a;

            }
            ENDCG
        }
    }
}

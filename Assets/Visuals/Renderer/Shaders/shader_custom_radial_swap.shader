// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RadialSwapShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _BasePalette("Base Palette", 2D) = "white" {}

        _TransitionPalette("TransitionPalette", 2D) = "white" {}
        _TargetPalette("Target Palette", 2D) = "white" {}

        _ColorCount("Color Count", float) = 0

        _Radius("Radius", float) = 0

        _Origin("Origin", Vector) = (0, 0, 0, 0)
        
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

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float3 swap(float3 a, float3 b)
            {

            };

            bool same(fixed4 colA, fixed4 colB)
            {
                // float n = a.r * b.r + a.g * b.g + a.b * b.b;
                // float m = a.r * a.r + a.g * a.g + a.b * a.b;
                // return (abs(n-m) < 0.01 & a.a > 0);

                bool r = abs(colA.r - colB.r) < 0.01;
                bool g = abs(colA.g - colB.g) < 0.01;
                bool b = abs(colA.b - colB.b) < 0.01;
                bool a = colA.a > 0.01;
                return r & g & b & a;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BasePalette;
            sampler2D _TargetPalette;
            sampler2D _TransitionPalette;

            float _ColorCount;

            float _Radius;
            float _Origin;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                // float2 screenCenter = float2(_ScreenParams.x / 2.0, _ScreenParams.y / 2.0);
                // float2 pixelCoord = float2(i.uv.x * _ScreenParams.x, i.uv.y * _ScreenParams.y);
                // float dist = dot(screenCenter, pixelCoord);

                // float2 vertex = i.vertex.xy; /// i.vertex.w;
                // float2 pixelCoord = float2(i.uv.x, i.uv.y);
                float2 vertex = i.vertex.xy;// / i.vertex.w;
                // vertex.y *= _ScreenParams.y;
                // vertex.x *= _ScreenParams.x;
                float dist = length(vertex-_ScreenParams / 2);
                // return fixed4(dist / 100, 0, 0, 1);

                fixed4 col = tex2D(_MainTex, i.uv);
                
                bool swapped = false;
                for (uint i=0; i < _ColorCount; i++) {
                    fixed4 base = tex2D(_BasePalette, float2(i / _ColorCount, 0));
                    fixed4 target = tex2D(_TargetPalette, float2(i / _ColorCount, 0));
                    fixed4 transition = tex2D(_TransitionPalette, float2(i / _ColorCount, 0));

                    if (same(col, base) & !swapped) {
                        if (dist < _Radius * _Radius) {
                            col = transition * col.a;
                        }
                        else {
                            col = target * col.a;
                        }
                        swapped = true;
                    }
                }

                return col * col.a;

            }
            ENDCG
        }
    }
}

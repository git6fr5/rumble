// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ColorSwapShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _BasePalette("Base Palette", 2D) = "white" {}
        _TargetPalette("Target Palette", 2D) = "white" {}
        _ColorCount("Color Count", float) = 0
        
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
            float _ColorCount;

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
                
                bool swapped = false;
                for (uint i=0; i < _ColorCount; i++) {
                    fixed4 base = tex2D(_BasePalette, float2(i / _ColorCount, 0));
                    fixed4 target = tex2D(_TargetPalette, float2(i / _ColorCount, 0));
                    if (same(col, base) & !swapped) {
                    // if (col.r == base.r & col.g == base.g & col.b == base.b & col.a > 0) {
                        col = target * col.a;
                        swapped = true;
                    }
                }

                return col * col.a;

            }
            ENDCG
        }
    }
}

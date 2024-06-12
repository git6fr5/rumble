
Shader "Custom/Kuwahara" {

    Properties {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _BoxSize("Box Size", Float) = 16
    }

    SubShader {

        Tags { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass {

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            SamplerState sampler_point_clamp;

            float4 _MainTex_ST;
            int _BoxSize;
            float _ScreenWidth;
            float _ScreenHeight;
            
            cbuffer segments : register( b0 )
            {
                float4 segCol[4];
                int segCount = 4;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float getCoord(float2 x0, float offset, float screenDim) {
                return min(max(0, x0 + offset / screenDim), 1);
            }

            float luminance(float3 color) {
                return dot(color, float3(0.299f, 0.587f, 0.114f));
            }

            float4 getBoxColor(float2 uv, float2 boxX, float2 boxY) {

                float4 col = float4(0, 0, 0, 0);
                float4 sumCol = float4(0, 0, 0, 0); 
                float std = 0;
                float mean = 0;

                int n = 0;

                [loop]
                for (int y = boxY.x; y < boxY.y; y++) {
                    [loop]
                    for (int x = boxX.x; x < boxX.y; x++) {
                        col = tex2D(_MainTex, float2(getCoord(uv.x, x, _ScreenWidth), getCoord(uv.y, y, _ScreenHeight)));

                        float l = luminance(col.rgb);
                        mean += l;
                        std += (l*l);
                        // sumCol += float4(saturate(col.rgb), 0);
                        sumCol += col;
                        n++;

                    }
                }

                sumCol /= n;
                mean /= n;
                std = abs(std / n - mean * mean);
                sumCol.a = std;

                return sumCol;

            }

            fixed4 frag(v2f i) : SV_Target {

                int boxSize = _BoxSize;
                if (boxSize > 10) {
                    boxSize = 10;
                }

                float2 minBox = float2(-boxSize, 0);
                float2 maxBox = float2(0, boxSize);

                float4 centerCol = tex2D(_MainTex, i.uv);
                
                float2 uv = float2(i.uv.x, i.uv.y);
                // uv.x = floor(i.uv.x * _ScreenWidth * _BoxSize) / (_ScreenWidth * _BoxSize);
                // uv.y = floor(i.uv.y * _ScreenHeight * _BoxSize) / (_ScreenHeight * _BoxSize);

                float4 q1 = getBoxColor(uv, minBox, minBox);
                float4 q2 = getBoxColor(uv, minBox, maxBox);
                float4 q3 = getBoxColor(uv, maxBox, minBox);
                float4 q4 = getBoxColor(uv, maxBox, maxBox);

                float minstd = min(segCol[0].a, min(segCol[1].a, min(segCol[2].a, segCol[3].a)));
                int4 q = float4(segCol[0].a, segCol[1].a, segCol[2].a, segCol[3].a) == minstd;
    
                if (dot(q, 1) > 1)
                    return saturate(float4((q1.rgb + q2.rgb + q3.rgb + q4.rgb) / 4.0f, 1.0f));
                else
                    return saturate(float4(q1.rgb * q.x + q2.rgb * q.y + q3.rgb * q.z + q4.rgb * q.w, 1.0f));

                // float minStd = -1;
                // float4 col = float4(0, 0, 0, 0);

                // for (int n = 0; n < 4; n++) {
                //     if (segCol[n].a < minStd || minStd < 0) {
                //         col = segCol[n];
                //         minStd = col.a;
                //     }
                // }
 
                // col.a = 1;
                // if (centerCol.a < 0.01) {
                //     col.a = 0;
                // }

                // return col;
            }

            ENDCG

        }
    }
}

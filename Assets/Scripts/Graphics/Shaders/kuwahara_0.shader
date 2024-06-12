
Shader "Custom/Kuwahara0" {

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
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 getBoxColor(float2 uv, float2 boxX, float2 boxY) {
                float4 col = float4(0, 0, 0, 0); 
                float _x = 0;
                float _y = 0;

                for (int y = boxY.x; y < boxY.y; y++) {
                    for (int x = boxX.x; x < boxX.y; x++) {
                        _x = uv.x + x / _ScreenWidth;
                        _y = uv.y + y / _ScreenHeight;
                        col += tex2D(_MainTex, float2(_x, _y));
                    }
                }

                col /= abs((boxX.y-boxX.x) * (boxY.y-boxY.x));
                return col;

            }

            fixed4 frag(v2f i) : SV_Target {

                float2 minBox = float2(-_BoxSize, 0);
                float2 maxBox = float2(0, _BoxSize);

                float4 centerCol = tex2D(_MainTex, i.uv);

                
                float2 uv = float2(i.uv.x, i.uv.y);
                // uv.x = floor(i.uv.x * _ScreenWidth * _BoxSize) / (_ScreenWidth * _BoxSize);
                // uv.y = floor(i.uv.y * _ScreenHeight * _BoxSize) / (_ScreenHeight * _BoxSize);

                segCol[0] = getBoxColor(uv, minBox, minBox);
                segCol[1] = getBoxColor(uv, minBox, maxBox);
                segCol[2] = getBoxColor(uv, maxBox, minBox);
                segCol[3] = getBoxColor(uv, maxBox, maxBox);

                float3 disp = float3(0, 0, 0);
                float dist = 0;
                float minDist = 1000;
                float4 col = float4(0, 0, 0, 0);
                for (int n = 0; n < 4; n++) {
                    disp = (centerCol - segCol[n]).rgb;
                    dist = disp.x * disp.x + disp.y * disp.y + disp.z + disp.z;
                    if (dist < minDist) {
                        col = segCol[n];
                        minDist = dist;
                    }
                }
 
                col.a = 1;
                if (centerCol.a < 0.01) {
                    col.a = 0;
                }

                return col;
            }

            ENDCG

        }
    }
}

Shader "Custom/Bush"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        // _rScale ("r Scale", float) = 0
        // _xScale ("x Scale", float) = 0
        // _yScale ("y Scale", float) = 0
        _r0 ("r 0", float) = 0
        _r1 ("r 1", float) = 0
        _r2 ("r 2", float) = 0
        _col0 ("col 0", Color) = (0, 0, 0, 1)
        _col1 ("col 1", Color) = (0, 0, 0, 1)
        _col2 ("col 2", Color) = (0, 0, 0, 1)
        _a ("a", float) = 0
        _b ("b", float) = 0
        _p ("position", Vector) = (0, 0, 0, 0)
        _pCount ("p Count", float) = 0
        _sCount ("s Count", float) = 0
        sX0 ("s X0", float) = 0
        _rot ("rot", float) = 0

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {

        Tags {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
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
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float _rot;

            float _pi = 3.1415;
            float _r0;
            float _r1;
            float _r2;
            fixed4 _col0;
            fixed4 _col1;
            fixed4 _col2;
            float _a;
            float _b;

            float4 _p;
            // float _r2;

            int _pCount = 0;
            // RWStructuredBuffer<float> pX;
            // RWStructuredBuffer<float> pY;

            cbuffer leafArray : register( b0 )
            {
                float pX[100];
                float pY[100];
                float _rScale[100];
                float _xScale[100];
                float _rotArr[100];
                // float _yScale[100];
            };

            int _sCount = 0;
            float dX = 0;
            float sX0 = 0;
            cbuffer lineArray : register( b1 )
            {
                float sY[100];
            };

            fixed4 drawCircle(v2f i, float n) {

                fixed4 col = fixed4(0, 0, 0, 0); // tex2D(_MainTex, i.uv);
                
                float x = i.worldPos.x-pX[n];
                float y = i.worldPos.y-pY[n];
                // float x = 2*(i.uv.x-0.5)-p.x;
                // float y = 2*(i.uv.y-0.5)-p.y;
                // float r = (y*y*_yScale[n]+x*x*_xScale[n])*_rScale[n];

                float t = _rotArr[n] / 180 * 3.1415;
                x = x * _xScale[n] * _rScale[n];
                y = y * _rScale[n];
                x = x * cos(t) - y * sin(t);
                y = y * cos(t) + x * sin(t);

                float r = (y*y+x*x);

                if (r < _r2) {
                    col = _col2;
                }
                else if (r < _r1) {
                    col = _col1;
                }
                else if (r < _r0) {
                    col = _col0;
                }

                // draw shadow.
                float e_x = _rScale[n]*x*x/(_b*_b);
                // float e_y = _rScale[n]*_yScale[n]*y*y/(_a*_a);
                float e_y = _rScale[n]*y*y/(_a*_a);

                if (1 < e_x + e_y & y < 0) {
                    col *= 0.5; //fixed4(, 0, 0, 0.5);
                    col.a *= 2;
                }

                // if (1 > e_x + e_y) {
                //     col = fixed4(0, 0, 0, 1);
                // }

                return col;
            }

            fixed4 shadeUnder(v2f i) {

                fixed4 col = fixed4(0, 0, 0, 0); // tex2D(_MainTex, i.uv);

                float x = i.worldPos.x - sX0;
                float y = i.worldPos.y;

                if (x > 0 & x < dX * (_sCount-1)) {

                    float left = floor(x / dX);
                    float right = ceil(x / dX);

                    int n0 = int(left);
                    int n1 = int(right);

                    float r = ((x/dX) - left);

                    float lineY = sY[n0] + r * (sY[n1]-sY[n0]);

                    if (y < lineY) {
                        col = _col0;
                        col *= 0.5; //fixed4(, 0, 0, 0.5);
                        col.a *= 2;
                        // fixed4(0, 0, 0, 1);
                    }
                    // col = fixed4(0, 0, 0, 1);
                    
                }
                
                return col;

            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = fixed4(0, 0, 0, 0);
                // fixed4 _col = fixed4(0, 0, 0, 0);
                fixed4 over = fixed4(0, 0, 0, 0);

                col = shadeUnder(i);
                for (int n = 0; n < _pCount; n++) {
                    
                    over = drawCircle(i, n);
                    if (over.a > 0.2) {
                        col = over;
                    }

                }

                // col.a /= col.a;
                return col;

            }
            ENDCG
        }
    }
}

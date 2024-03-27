Shader "Custom/Goop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", float) = 0.05
        _ScrollWavelength ("Scroll Wavelength", float) = 20
        _TimeWavelength ("Time Wavelength", float) = 3
    }
    SubShader
    {

        Tags {
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float _Amplitude;
            float _ScrollWavelength;
            float _TimeWavelength;

            fixed4 frag (v2f i) : SV_Target
            {
                float y = i.uv.y;
                y += _Amplitude * sin(_ScrollWavelength * i.uv.x + _TimeWavelength * _Time[1]);

                i.uv.y = y;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= col.a;

                return col;
            }
            ENDCG
        }
    }
}

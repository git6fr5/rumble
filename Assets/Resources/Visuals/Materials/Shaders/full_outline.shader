// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FullOutlineShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Float) = 0.1

        _WaveAmplitude("Wave Amplitude", Float) = 0.01
        _WaveSpeed("Wave Speed", Float) = 1

        _WaveScale("Wave Scale", Float) = 1

    }

    SubShader
    {
        Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Tags
		{ 
			"Queue"="Transparent+1000" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
    
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

            float3 stretch(float3 vec, float x, float y)
            {
                float2x2 stretchMatrix = float2x2(x, 0, 0, y);
                return float3(mul(stretchMatrix, vec.xy), vec.z).xyz;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _OutlineWidth;
            float4 _OutlineColor;
            
            float _WaveAmplitude;
            float _WaveSpeed;

            float _WaveScale;


            v2f vert(appdata v)
            {
                v2f o;
                // float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 vertex = stretch(v.vertex, 1 + _OutlineWidth / _MainTex_ST.x, 1 + _OutlineWidth / _MainTex_ST.y);
                
                // v.vertex.x += _OutlineWidth;
                // v.vertex.y -= _OutlineWidth;

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 xy = i.uv; // * (1 + _OutlineWidth);
                // float y = 2 * (i.uv.y - 0.5);
                // float x = 2 * (i.uv.x - 0.5);

                // float r = i.uv.y * i.uv.y + i.uv.x + i.uv.x;
                // float y = i.uv.y + _WaveAmplitude * sin(UNITY_PI * (_WaveSpeed * _Time[1] + _WaveScale * r));
                // float x = i.uv.x + _WaveAmplitude * sin(UNITY_PI * (_WaveSpeed * _Time[1] + _WaveScale * r));

                // fixed4 col = tex2D(_MainTex, float2(x, y));
                fixed4 col = tex2D(_MainTex, xy);

                return _OutlineColor * col.a;
            }
            ENDCG
        }
    }
}

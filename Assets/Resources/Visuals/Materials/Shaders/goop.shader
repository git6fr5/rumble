Shader "Custom/Goop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", float) = 0.05
        _ScrollWavelength ("Scroll Wavelength", float) = 20
        _TimeWavelength ("Time Wavelength", float) = 3
        _Rotation ("Rotation", float) = 0
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

            float _Amplitude;
            float _ScrollWavelength;
            float _TimeWavelength;
            float _Rotation;

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.y += sin(_Rotation * 3.1415 / 180.0) * _Amplitude * sin(_ScrollWavelength * i.worldPos.x + _TimeWavelength * _Time[1]);
                i.uv.y += cos(_Rotation * 3.1415 / 180.0) * _Amplitude * sin(_ScrollWavelength * i.worldPos.y + _TimeWavelength * _Time[1]);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= col.a;

                return col;
            }
            ENDCG
        }
    }
}

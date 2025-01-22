Shader "Custom/DappledGlobalLight2D"
{
    Properties
    {
        _MainTex ("Dappled Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _Intensity ("Light Intensity", Range(0, 1)) = 1.0
        _Scale ("Pattern Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
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

            float4 _LightColor;
            float _Intensity;
            float _Scale;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the dappled texture
                float2 scaledUV = i.uv * _Scale;
                fixed4 dapple = tex2D(_MainTex, scaledUV);

                // Modulate light color by dapple pattern and intensity
                fixed4 color = _LightColor * dapple * _Intensity;
                return color;
            }
            ENDCG
        }
    }
}

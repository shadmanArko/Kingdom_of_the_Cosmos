Shader "Custom/WaterWaveShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _WaveStrength ("Wave Strength", Range(0, 0.1)) = 0.02
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2.0
        _WaveFrequency ("Wave Frequency", Range(0, 20)) = 10.0
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.2
        _TextureDistortion ("Texture Distortion", Range(0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveStrength;
            float _WaveSpeed;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _TextureDistortion;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Create two separate waves
                float wave1 = sin(i.uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                float wave2 = sin(i.uv.x * _WaveFrequency * 1.3 + _Time.y * _WaveSpeed * 0.8) * _WaveAmplitude;
                
                // Combine waves
                float finalWave = (wave1 + wave2) * _WaveStrength;
                
                // Apply wave distortion to UV coordinates
                float2 distortedUV = i.uv;
                distortedUV.x += finalWave * _TextureDistortion;
                distortedUV.y += finalWave;
                
                // Sample texture with distorted UVs
                fixed4 col = tex2D(_MainTex, distortedUV);
                
                // Preserve original alpha
                col.a = tex2D(_MainTex, i.uv).a;
                
                return col * i.color;
            }
            ENDCG
        }
    }
}

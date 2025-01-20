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

//waves not affected by light moves properly but waves under light dont move
//Shader "Custom/WaveEffect"
//{
//    Properties
//    {
//        _MainTex ("Texture", 2D) = "white" {}
//        _WaveStrength ("Wave Strength", Float) = 1.0
//        _WaveSpeed ("Wave Speed", Float) = 1.0
//        _WaveFrequency ("Wave Frequency", Float) = 1.0
//        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
//        _TextureDistortion ("Texture Distortion", Float) = 0.1
//    }
//
//    SubShader
//    {
//        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
//        Blend SrcAlpha OneMinusSrcAlpha
//        
//        Pass
//        {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//                fixed4 color : COLOR;
//            };
//
//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//                float4 vertex : SV_POSITION;
//                fixed4 color : COLOR;
//            };
//
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//            float _WaveStrength;
//            float _WaveSpeed;
//            float _WaveFrequency;
//            float _WaveAmplitude;
//            float _TextureDistortion;
//
//            v2f vert(appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                o.color = v.color;
//                return o;
//            }
//
//            fixed4 frag(v2f i) : SV_Target
//            {
//                // Create two separate waves
//                float wave1 = sin(i.uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
//                float wave2 = sin(i.uv.x * _WaveFrequency * 1.3 + _Time.y * _WaveSpeed * 0.8) * _WaveAmplitude;
//
//                // Combine waves
//                float finalWave = (wave1 + wave2) * _WaveStrength;
//
//                // Apply wave distortion to UV coordinates
//                float2 distortedUV = i.uv;
//                distortedUV.x += finalWave * _TextureDistortion;
//                distortedUV.y += finalWave;
//
//                // Sample texture with distorted UVs
//                fixed4 col = tex2D(_MainTex, distortedUV);
//
//                // Preserve original alpha
//                col.a = tex2D(_MainTex, i.uv).a;
//
//                return col * i.color;
//            }
//            ENDCG
//        }
//    }
//}

//Shader "Custom/WaveEffect"
//{
//    Properties
//    {
//        _MainTex ("Texture", 2D) = "white" {}
//        _WaveStrength ("Wave Strength", Float) = 1.0
//        _WaveSpeed ("Wave Speed", Float) = 1.0
//        _WaveFrequency ("Wave Frequency", Float) = 1.0
//        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
//        _TextureDistortion ("Texture Distortion", Float) = 0.1
//    }
//
//    SubShader
//    {
//        Tags 
//        { 
//            "RenderType" = "Transparent"
//            "Queue" = "Transparent"
//            "RenderPipeline" = "UniversalPipeline"
//            "IgnoreProjector" = "True"
//            "LightMode" = "Universal2D"
//        }
//        
//        Blend SrcAlpha OneMinusSrcAlpha
//        ZWrite Off
//        Cull Off
//
//        Pass
//        {
//            HLSLPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//            
//            // Shape light cookies
//            TEXTURE2D(_ShapeLightTexture0);
//            SAMPLER(sampler_ShapeLightTexture0);
//            
//            // Light positions and colors
//            uniform float4 _ShapeLightPosition0;
//            uniform half4 _ShapeLightColor0;
//            uniform half _ShapeLightBlend0;
//
//            TEXTURE2D(_MainTex);
//            SAMPLER(sampler_MainTex);
//
//            CBUFFER_START(UnityPerMaterial)
//                float4 _MainTex_ST;
//                float _WaveStrength;
//                float _WaveSpeed;
//                float _WaveFrequency;
//                float _WaveAmplitude;
//                float _TextureDistortion;
//            CBUFFER_END
//
//            struct Attributes
//            {
//                float4 positionOS : POSITION;
//                float2 uv : TEXCOORD0;
//                float4 color : COLOR;
//            };
//
//            struct Varyings
//            {
//                float4 positionCS : SV_POSITION;
//                float2 uv : TEXCOORD0;
//                float4 color : COLOR;
//                float4 screenPosition : TEXCOORD1;
//            };
//
//            Varyings vert(Attributes input)
//            {
//                Varyings output = (Varyings)0;
//
//                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
//                output.screenPosition = ComputeScreenPos(output.positionCS);
//                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
//                output.color = input.color;
//                
//                return output;
//            }
//
//            half4 frag(Varyings input) : SV_Target
//            {
//                // Create two separate waves
//                float wave1 = sin(input.uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
//                float wave2 = sin(input.uv.x * _WaveFrequency * 1.3 + _Time.y * _WaveSpeed * 0.8) * _WaveAmplitude;
//
//                // Combine waves
//                float finalWave = (wave1 + wave2) * _WaveStrength;
//
//                // Apply wave distortion to UV coordinates
//                float2 distortedUV = input.uv;
//                distortedUV.x += finalWave * _TextureDistortion;
//                distortedUV.y += finalWave;
//
//                // Sample texture with distorted UVs
//                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
//                mainTex.a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
//
//                // Calculate screen position for lighting
//                float2 screenPos = input.screenPosition.xy / input.screenPosition.w;
//                
//                // Sample light texture
//                half4 lightColor = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, screenPos);
//                
//                // Apply lighting with ambient
//                half4 finalColor = mainTex * input.color;
//                finalColor.rgb *= (lightColor.rgb * _ShapeLightColor0.rgb) + 0.5;
//
//                return finalColor;
//            }
//            ENDHLSL
//        }
//    }
//}
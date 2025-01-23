Shader "Custom/DappledLight" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _LightIntensity ("Light Intensity", Range(0,2)) = 1
        _DappleScale ("Dapple Scale", Range(0.1, 10)) = 2
        _DappleSpeed ("Dapple Speed", Range(0, 10)) = 1
    }
    SubShader {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
        }
        
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 lightDirection : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _LightColor;
                float _LightIntensity;
                float _DappleScale;
                float _DappleSpeed;
            CBUFFER_END

            // Simplex noise function for dappled effect
            float snoise(float2 v) {
                // Existing Simplex noise function code
            }

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.lightDirection = normalize(_WorldSpaceLightPos0.xyz - output.positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float2 noiseCoord = input.lightDirection.xz * _DappleScale + _Time.y * _DappleSpeed;
                noiseCoord += _WorldSpaceLightPos0.xz * 0.1; // Adjust the 0.1 multiplier for desired effect
                
                float noiseVal = snoise(noiseCoord) * 0.5 + 0.5; // Remap noise to 0-1 range
                
                float dappleIntensity = lerp(0.5, 1.5, noiseVal);
                
                half3 finalColor = mainTex.rgb * _LightColor.rgb * _LightIntensity * dappleIntensity;
                
                return half4(finalColor, mainTex.a);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
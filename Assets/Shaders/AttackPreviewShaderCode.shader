Shader "Unlit/AttackPreviewShaderCode"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CutoffValue ("Cutoff Value", Range(0, 1)) = 0
        _Color ("Color", Color) = (1,1,1,1)  // Added color property
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CutoffValue;
            float4 _Color;  // Added color variable

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Adjust UV coordinates to use bottom center as origin
                float2 centeredUV = float2(i.uv.x - 0.5, i.uv.y);
                
                // Calculate angle from bottom center (in radians)
                float angle = atan2(centeredUV.y, centeredUV.x);
                
                // Convert to degrees and normalize to 0-180 range
                float angleDegrees = degrees(angle);
                angleDegrees = angleDegrees < 0 ? angleDegrees + 360 : angleDegrees;
                
                // Calculate the maximum angle based on cutoff value
                float halfAngle = 90 * (1 - _CutoffValue);
                float minAngle = 90 - halfAngle;
                float maxAngle = 90 + halfAngle;
                
                // Create the arc cutoff
                float cutoff = (angleDegrees >= minAngle && angleDegrees <= maxAngle) ? 1 : 0;
                
                // Apply the color tint
                col *= _Color;
                
                // Apply the cutoff to the alpha channel
                col.a *= cutoff;
                
                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
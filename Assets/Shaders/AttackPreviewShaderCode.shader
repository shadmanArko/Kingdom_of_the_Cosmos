Shader "Unlit/AttackPreviewShaderCode"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CutoffValue ("Cutoff Value", Range(0, 1)) = 0
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off // Disable depth writing
        ZTest LEqual // Ensure depth comparison works correctly

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
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; // Manual UV transformation
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 centeredUV = float2(i.uv.x - 0.5, i.uv.y);
                float angle = atan2(centeredUV.y, centeredUV.x);
                float angleDegrees = degrees(angle);
                angleDegrees = angleDegrees < 0 ? angleDegrees + 360 : angleDegrees;

                float halfAngle = 90 * (1 - _CutoffValue);
                float minAngle = 90 - halfAngle;
                float maxAngle = 90 + halfAngle;

                float cutoff = (angleDegrees >= minAngle && angleDegrees <= maxAngle) ? 1 : 0;

                col *= _Color;
                col.a *= cutoff;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

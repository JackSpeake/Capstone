Shader "Hidden/PostProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Radius, _Feathering, _VignetteActive;
            float4 _TintColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (_VignetteActive > 0.5)
                {
                    float2 newUV = i.uv * 2 - 1; // moves things to the center?
                    float circle = length(newUV);
                    float mask = 1 - smoothstep(_Radius, _Radius + _Feathering, circle);
                    float invertMask = 1 - mask;


                    float3 displayColor = col.rgb * mask;
                    float3 vignetteColor = col.rgb * invertMask * _TintColor;
                    return fixed4(displayColor + vignetteColor, 1);
                }
                else
                {
                    return col;
                }
            }
            ENDCG
        }
    }
}

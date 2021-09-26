Shader "Unlit/unitMarker"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SineVal ("Sine min value", float) = 0.25
        [MaterialToggle]_SineEnabled ("Sine effect enabled", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Offset -1000, -1000
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
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
            float _SineVal;
            bool _SineEnabled;
            float4 _MainTex_ST;

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                if(_SineEnabled)
                {
                    float phase = _Time * 200.0;
                    col.rgb *= _SineVal * sin(phase) + (1 - _SineVal);
                }

                return col;
            }
            ENDCG
        }
    }
}

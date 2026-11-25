Shader "Custom/PlacaLimpeza"
{
    Properties
    {
        _MainTex ("Placa", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _MaskTex;
            fixed4 _Color;   // ← ISSO É O QUE FALTAVA

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 placa = tex2D(_MainTex, i.uv) * _Color;
                fixed mask = tex2D(_MaskTex, i.uv).r;

                placa.a *= (1 - mask);
                return placa;
            }
            ENDCG
        }
    }
}

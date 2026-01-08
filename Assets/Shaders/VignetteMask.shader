Shader "UI/VignetteMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}   

        _Color ("Color", Color) = (0,0,0,1)
        _Aperture ("Aperture", Range(0, 1)) = 1
        _Softness ("Softness", Range(0.001, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;                 

            fixed4 _Color;
            float _Aperture;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 baseCol = tex2D(_MainTex, i.uv);

                float2 p = (i.uv - 0.5) * 2.0;

                float d = length(p);

                float radius = saturate(_Aperture);
                float edge0 = radius;
                float edge1 = radius + max(_Softness, 0.001);

                float vignetteAlpha = smoothstep(edge0, edge1, d);

                fixed4 col = _Color;
                col.a *= vignetteAlpha;

                col.a *= baseCol.a;

                return col;
            }
            ENDCG
        }
    }
}


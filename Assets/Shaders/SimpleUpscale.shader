Shader "Custom/SimpleUpscale"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UpscaleFactor ("Upscale Factor", Float) = 2.0
        [Toggle] _ShowPixelGrid ("Show Pixel Grid", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _UpscaleFactor;
            float _ShowPixelGrid;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Debug visualization: show pixel grid
                if (_ShowPixelGrid > 0)
                {
                    float2 pixelPos = i.uv * _ScreenParams.xy / _UpscaleFactor;
                    float2 pixelGrid = frac(pixelPos);
                    
                    // Draw grid lines
                    float gridLine = step(0.95, pixelGrid.x) + step(0.95, pixelGrid.y);
                    
                    // Blend grid with original color
                    col.rgb = lerp(col.rgb, float3(1,0,0), gridLine * 0.3);
                }
                
                return col;
            }
            ENDCG
        }
    }
}
//Shader that upscales the render texture created in the Downscale Pass.

Shader "Custom/UpscalePass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpscaleFactor ("Upscale Factor", Float) = 2.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

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
			float _UpscaleFactor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			//Bicubic sampling
			float4 cubic(float v)
			{
				float4 n = float4(1.0, 2.0, 3.0, 4.0) - v;
				float4 s = n * n * n;
				float x = s.x;
				float y = s.y - 4.0 * s.x;
				float z = s.z - 4.0 * s.y + 6.0 * s.x;
				float w = 6.0 - x - y - z;
				return float4(x, y, z, w) * (1.0/6.0);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 texSize = float2(_ScreenParams.x, _ScreenParams.y) / _UpscaleFactor;
				float2 pixel = i.uv * texSize + 0.5;

				float2 _frac = frac(pixel);
				pixel = floor(pixel) / texSize - float2(0.5 / texSize.x, 0.5 / texSize.y);

				float4 xcubic = cubic(_frac.x);
				float4 ycubic = cubic(_frac.y);

				float4 c = float4(pixel.x - 1, pixel.x, pixel.x + 1, pixel.x + 2);
				float4 s = float4(pixel.y - 1, pixel.y, pixel.y + 1, pixel.y + 2);
				float4 xsum = float4(0, 0, 0, 0);

				for (int i = 0 ; i < 4; i++)
				{
					float4 sample = tex2D(_MainTex, float2(c[i], s[i]));
					xsum += sample * xcubic[i];
				}

				return xsum;
			}
			ENDCG
		}
	}
}
Shader "Custom/ScreenBlurShader"
{
	Properties
	{
		_BlurSize("Blur Size", Range(0,0.01)) = 0
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		GrabPass{ "_GrabTexture" }

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			sampler2D _MainTex;
			sampler2D _GrabTexture;

			float _BlurSize;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_GrabTexture, i.uv) * 0.38774;
				col += tex2D(_GrabTexture, i.uv + float2(_BlurSize * 2, _BlurSize * 2)) * 0.06136;
				col += tex2D(_GrabTexture, i.uv + float2(_BlurSize, _BlurSize)) * 0.24477;
				col += tex2D(_GrabTexture, i.uv + float2(_BlurSize * -1, _BlurSize * -1)) * 0.24477;
				col += tex2D(_GrabTexture, i.uv + float2(_BlurSize * -2, _BlurSize * -2)) * 0.06136;

				return col;
			}
			ENDCG
		}
	}
}

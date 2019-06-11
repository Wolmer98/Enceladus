Shader "Custom/LoadingScreenShader"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseTiling("Noise Tiling", float) = 1
		_NoiseTransparency("Noise Transparency", float) = 1
		_NoiseSpeed("Noise Speed", float) = 1

		_Color("Color", Color) = (1,1,1,1)
		_LineWidth("LineWidth", float) = 5
		_LineSpeed("LienSpeed", float) = 2

		_MaskValue("TempValue", float) = 0.07
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Tags {"RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

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
				float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv2 = v.uv;
				o.uv = ComputeGrabScreenPos(o.vertex);

                return o;
            }

			sampler2D _MainTex;
            sampler2D _NoiseTex;
			sampler2D _GrabTexture;

			fixed4 _Color;
			half _LineWidth;
			half _LineSpeed;
			half _NoiseTiling;
			half _NoiseSpeed;
			half _NoiseTransparency;
			half _MaskValue;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2Dproj(_GrabTexture, i.uv);
				float lines = (sin(i.uv.y * _LineWidth + _Time.w * _LineSpeed) + 1) / 4;

				fixed4 noise = tex2D(_NoiseTex, (i.uv + float2(0, _Time.w * _NoiseSpeed)) * _NoiseTiling).x * _NoiseTransparency;
				float lineNoise = (tex2D(_NoiseTex, float2(0, i.uv.y + _Time.w * _NoiseSpeed)).x *  _NoiseTransparency);

				//fixed4 noiseT = tex2D(_MainTex, i.uv * _NoiseTiling);
				//fixed4 finalCol =  _Color * lines * noise;
				fixed4 finalCol = tex2D(_MainTex, i.uv);
				float mask = step(col, _MaskValue);
				finalCol *= step(col, _MaskValue) * noise * _Color;
				finalCol.a = mask * _Color.a;
                return finalCol;
            }
            ENDCG
        }
    }
}

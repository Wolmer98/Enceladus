Shader "Unlit/Hologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTexture ("Noise", 2D) = "white" {}
		_GridTexture("Grid", 2D) = "white" {}
		_MaskTexture("Mask", 2D) = "white" {}
		_FadeWidth ("Fade width", Range(0, 1)) = 0.05
		_LineWidth ("Line width", Float) = 600
		_NoiseTransparency ("Noise transparency", Range(0, 1)) = 0.75
		_LineSpeed ("Line speed", Float) = 4
		_NoiseSpeed ("Noise speed", Float) = 0.01
		_LineNoiseSpeed ("Linenoise speed", Float) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

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
				float4 color : COLOR;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _NoiseTexture;
			sampler2D _GridTexture;
			sampler2D _MaskTexture;
			float _FadeWidth;
			float _LineWidth;
			float _NoiseTransparency;
			float _LineSpeed;
			float _NoiseSpeed;
			float _LineNoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float lines = (sin(i.uv.y * _LineWidth + _Time.w * _LineSpeed) + 1) / 2;
				float noise = (tex2D(_NoiseTexture, (i.uv + float2(0, _Time.w * _NoiseSpeed)) * 3).x * _NoiseTransparency) + (1 - _NoiseTransparency);
				float lineNoise = (tex2D(_NoiseTexture, float2(0, i.uv.y + _Time.w * _LineNoiseSpeed)).x *  _NoiseTransparency) + (1 - _NoiseTransparency);

				float fade = smoothstep(0, _FadeWidth, i.uv.x) * smoothstep(0, _FadeWidth, 1-i.uv.x) * smoothstep(0, _FadeWidth, i.uv.y) * smoothstep(0, _FadeWidth, 1-i.uv.y);

				float grid = tex2D(_GridTexture, i.uv * 17).r;
				float mask = tex2D(_MaskTexture, i.uv).r;

				fixed3 finalColor = tex2D(_MainTex, i.uv).rgb * lines * i.color.rgb + grid * mask * i.color.rgb;

                fixed4 col = fixed4(finalColor, clamp(0, 1, noise * lineNoise + grid * mask * 0.7) * fade * i.color.a);

				finalColor = fade;

                return col;
            }
            ENDCG
        }
    }
}

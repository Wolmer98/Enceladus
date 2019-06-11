Shader "SciFiEffect"
{
    Properties
    {
		_RadialGradient("Radial gradient", 2D) = "white" {}
		_HoleGradient("Hole gradient", 2D) = "white" {}
		_Progress("Progress", Range(0, 1)) = 0
		_Color("Color", Color) = (1, 1, 1, 1)

		[Space(15)]
		[Header(Layer 0)]
		_Layer0("Texture", 2D) = "white" {}
		_Speed0("Speed", Float) = 0.1
		_Start0("Start", Float) = 0
		_Stop0("Stop", Float) = 1
		_Rotation0("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 1)]
		_Layer1("Texture", 2D) = "white" {}
		_Speed1("Speed", Float) = 0.1
		_Start1("Start", Float) = 0
		_Stop1("Stop", Float) = 1		
		_Rotation1("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 2)]
		_Layer2("Texture", 2D) = "white" {}
		_Speed2("Speed", Float) = 0.1
		_Start2("Start", Float) = 0
		_Stop2("Stop", Float) = 1
		_Rotation2("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 3)]
		_Layer3("Texture", 2D) = "white" {}
		_Speed3("Speed", Float) = 0.1
		_Start3("Start", Float) = 0
		_Stop3("Stop", Float) = 1
		_Rotation3("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 4)]
		_Layer4("Texture", 2D) = "white" {}
		_Speed4("Speed", Float) = 0.1
		_Start4("Start", Float) = 0
		_Stop4("Stop", Float) = 1
		_Rotation4("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 5)]
		_Layer5("Texture", 2D) = "white" {}
		_Speed5("Speed", Float) = 0.1
		_Start5("Start", Float) = 0
		_Stop5("Stop", Float) = 1
		_Rotation5("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 6)]
		_Layer6("Texture", 2D) = "white" {}
		_Speed6("Speed", Float) = 0.1
		_Start6("Start", Float) = 0
		_Stop6("Stop", Float) = 1
		_Rotation6("Rotation", Range(0, 359)) = 0

		[Space(15)]
		[Header(Layer 7)]
		_Layer7("Texture", 2D) = "white" {}
		_Speed7("Speed", Float) = 0.1
		_Start7("Start", Float) = 0
		_Stop7("Stop", Float) = 1
		_Rotation7("Rotation", Range(0, 359)) = 0

		//[HideInInspector]
		//_CurrentTime("_CurrentTime", Float) = 0
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


			sampler2D _RadialGradient;
			sampler2D _HoleGradient;
			float _Progress;
			float4 _Color;
			float4 _Layer0_ST;

			sampler2D _Layer0;
			float _Speed0;
			float _Start0;
			float _Stop0;
			float _Rotation0;

			sampler2D _Layer1;
			float _Speed1;
			float _Start1;
			float _Stop1;
			float _Rotation1;

			sampler2D _Layer2;
			float _Speed2;
			float _Start2;
			float _Stop2;
			float _Rotation2;

			sampler2D _Layer3;
			float _Speed3;
			float _Start3;
			float _Stop3;
			float _Rotation3;

			sampler2D _Layer4;
			float _Speed4;
			float _Start4;
			float _Stop4;
			float _Rotation4;

			sampler2D _Layer5;
			float _Speed5;
			float _Start5;
			float _Stop5;
			float _Rotation5;

			sampler2D _Layer6;
			float _Speed6;
			float _Start6;
			float _Stop6;
			float _Rotation6;

			sampler2D _Layer7;
			float _Speed7;
			float _Start7;
			float _Stop7;
			float _Rotation7;

			float _CurrentTime;

			float2 rotate(float2 xy, float rotation, float2 origin) 
			{
				float sinX = sin(rotation * 0.0174532925);
				float cosX = cos(rotation * 0.0174532925);
				float sinY = sin(rotation * 0.0174532925);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
				return mul(xy - origin, rotationMatrix) + origin;
			}

			float progress(sampler2D layerTex, sampler2D gradientTex, float2 uv, float rotation, float speed, float start, float stop) 
			{
				_CurrentTime += fmod(_Time.w * 0.1, 3);
				float time = clamp(0, 1, _CurrentTime);
				time = _Progress;

				float2 gradientUV = uv;
				float2 circleUV = uv;

				gradientUV = rotate(uv, rotation, float2(0.5, 0.5));
				circleUV = rotate(uv, fmod(_Time.w * speed, 359), float2(0.5, 0.5));

				float gradient = start + tex2D(gradientTex, gradientUV) * (stop - start);
				float layer = tex2D(layerTex, circleUV).r;
				float progress = step(gradient, time) * layer;
				return progress;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Layer0);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float time = clamp(0, 1, fmod(_Time.w * 0.1, 3));

				float2 gradientUV = i.uv;
				float2 circleUV = i.uv;

				float progress0 = progress(_Layer0, _RadialGradient, i.uv, _Rotation0, _Speed0, _Start0, _Stop0);
				float progress1 = progress(_Layer1, _HoleGradient, i.uv, _Rotation1, _Speed1, _Start1, _Stop1);
				float progress2 = progress(_Layer2, _RadialGradient, i.uv, _Rotation2, _Speed2, _Start2, _Stop2);
				float progress3 = progress(_Layer3, _RadialGradient, i.uv, _Rotation3, _Speed3, _Start3, _Stop3);
				float progress4 = progress(_Layer4, _RadialGradient, i.uv, _Rotation4, _Speed4, _Start4, _Stop4);
				float progress5 = progress(_Layer5, _RadialGradient, i.uv, _Rotation5, _Speed5, _Start5, _Stop5);
				float progress6 = progress(_Layer6, _RadialGradient, i.uv, _Rotation6, _Speed6, _Start6, _Stop6);
				float progress7 = progress(_Layer7, _RadialGradient, i.uv, _Rotation7, _Speed7, _Start7, _Stop7);

				float3 color = _Color.rgb;

				float alpha = progress0 + progress1 + progress2 + progress3 + progress4 + progress5 + progress6 + progress7;

				fixed4 result = fixed4(color, clamp(0, 1, alpha) * _Color.a);
				
                return result;
            }
            ENDCG
        }
    }
}

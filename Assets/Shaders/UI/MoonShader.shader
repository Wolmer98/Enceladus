Shader "Unlit/MoonShader"
{
    Properties
    {
        _Main ("Texture", 2D) = "white" {}
		_Progress ("Progress", Range (0, 1)) = 0
		_Gradient ("Gradient", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_AntiAliasing ("Antialiasing", Range (0, 1)) = 0.8
		_Distortion ("Distorion", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			#define PI 3.14159265359
			#define E 2.71828182846

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


            sampler2D _Main;
            float4 _Main_ST;
			float _Progress;
			sampler2D _Gradient;
			float _AntiAliasing;
			sampler2D _Mask;
			sampler2D _Distortion;
			float4 _Color;

			float2 rotate(float2 xy, float rotation, float2 origin)
			{
				float sinX = sin(rotation * 0.0174532925);
				float cosX = cos(rotation * 0.0174532925);
				float sinY = sin(rotation * 0.0174532925);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
				return mul(xy - origin, rotationMatrix) + origin;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Main);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

				float2 uv = tex2D(_Distortion, i.uv);

				float progress = clamp(fmod(_Time.w, 20), 0, 10)/10;

				
				

				float phaseTwo = step(10, fmod(_Time.y, 20));

				float phaseOne = step(0.001, fmod(_Time.y, 20)) - phaseTwo;

				float transOne = (clamp(fmod(_Time.y, 20), 0, 10) / 10);

				float transTwo = (clamp(fmod(_Time.y, 20), 10, 20) - 10) / 10;


				float standardDevation = 0.01;

				float sum = 0;
				float4 color = float4(0, 0, 0, 0);
				for (int x = -2; x < 3; x++) {
					for (int y = -2; y < 3; y++) {
						float2 offset = float2(x, y)*0.01;

						float stDevSquared = standardDevation * standardDevation;
						float gauss = (1 / sqrt(2 * PI*stDevSquared)) * pow(E, -((offset*offset) / (2 * stDevSquared)));

						sum += gauss;

						color += tex2D(_Mask, i.uv + offset) * gauss;
					}
				}

				color = color / sum;

				progress = phaseOne * transOne + phaseTwo * transTwo;

				//progress = _Progress;

				float gradient = tex2D(_Gradient, rotate(i.uv, 0, float2(0.5, 0.5)));
				float alpha = smoothstep(clamp(progress - _AntiAliasing/2, 0, 1), clamp(progress+ _AntiAliasing/2, 0, 1), gradient);

				alpha = phaseOne * alpha + phaseTwo * (1 - alpha);

				float mask = tex2D(_Mask, i.uv).a;
				float4 main = tex2D(_Main, rotate(uv, fmod(_Time.y * 2, 359), float2(0.5, 0.5)) /*repeat*/ + float2(0.15, -0.1) * _Time.y);

				float2 rotation = rotate(uv, fmod(_Time.y * 2, 359), float2(0.5, 0.5));
				main = tex2D(_Main, rotation + float2(0.15, -0.1) * _Time.y);
				//float step = smoothstep(0.3, 0.7, main.r);
                // sample the texture
                fixed4 col = main * _Color;
				col.a *= alpha * color.a;
                return col;
            }
            ENDCG
        }
    }
}

Shader "Custom/StandardWaterShader"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalTex("Normal Texture", 2D) = "bump" {}
		_NormalStrength("Normal Strength", float) = 1

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		[Header(Intersection Settings)]
		_IntersectionTex("Intersection Texture", 2D) = "white" {}
		_IntersectionColor("Intersection Color", Color) = (1, 1, 1, 1)
		_FadeLength("Fade Length",float) = 1
		_WaveSpeed("Wave Speed", float) = 1
    }
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		Tags
		{
			"RenderType" = "Opaque" //Fog does not work with transparent shader.
			/*"RenderType" = "Transparent"
			"Queue" = "Transparent"*/
		}
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
		#pragma multi_compile_fog

        sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _IntersectionTex;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalTex;
			float2 uv_IntersectionTex;
			float4 screenPos;
			float3 worldPos;
		};

		float _NormalStrength;
		half _Glossiness;
		half _Metallic;
		sampler2D _CameraDepthTexture;
		fixed4 _Color;
		fixed4 _IntersectionColor;
		float _FadeLength;
		float _WaveSpeed;

		float4 ApplyFog(float4 color, Input i)
		{
			float viewDistance = length(_WorldSpaceCameraPos - i.worldPos);
			UNITY_CALC_FOG_FACTOR_RAW(viewDistance);
			color.rgb = lerp(unity_FogColor.rgb, color.rgb, saturate(unityFogFactor));
			return color;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//Fetch each pixel-distance for both the object and the scene.
			//Then get the difference and add an intersection based on this distance.
			/*float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
			float surfZ = -mul(UNITY_MATRIX_V, float4(IN.worldPos.xyz, 1)).z;
			float diff = sceneZ - surfZ;
			float intersect = 1 - saturate(diff / _FadeLength);*/
			//fixed4 col = fixed4(lerp(tex2D(_MainTex, IN.uv_MainTex + _Time.x * _WaveSpeed) * _Color, _IntersectionColor * tex2D(_IntersectionTex, IN.uv_IntersectionTex - _Time.x * _WaveSpeed), pow(intersect, 4)));
			
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex + _Time.x * _WaveSpeed) * _Color; //Intersection does not work with opaque shader.
			col = ApplyFog(col, IN);

			o.Albedo = col.rgb;
			o.Alpha = col.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			//Noise normals based on a texture.
			o.Normal += UnpackScaleNormal(tex2D(_NormalTex, IN.uv_NormalTex + _Time.x * _WaveSpeed), _NormalStrength);
			o.Normal += UnpackScaleNormal(tex2D(_NormalTex, IN.uv_NormalTex - _Time.xy * _WaveSpeed / 10), _NormalStrength);
        }
        ENDCG
    }
    FallBack "Diffuse"
}

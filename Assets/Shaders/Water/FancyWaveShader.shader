Shader "Custom/FancyWaveShader Shader" {
	Properties{
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

		[Header(Wave Settings)]
		_WaveSpeed("Wave Speed", float) = 1
		_WaveA("Wave A (dir (x,y) , steepness, wavelength)", Vector) = (1, 0, 0.5, 10)
		_WaveB("Wave B", Vector) = (0, 1, 0.25, 20)
		_WaveC("Wave C", Vector) = (1, 1, 0.15, 10)
	}
		SubShader{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On

			Tags
			{
				//"RenderType" = "Opaque"
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
			}


			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert finalcolor:Aura2_Fog
			#pragma target 3.0
		    #pragma multi_compile_fog
			//#pragma multi_compile _ AURA
			//#pragma multi_compile _ AURA_USE_CUBIC_FILTERING
			//#pragma multi_compile _ AURA_DISPLAY_VOLUMETRIC_LIGHTING_ONLY
			//#include "Assets/Aura 2/System/Code/Shaders/Aura.cginc"

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _IntersectionTex;

			struct Input {
				float2 uv_MainTex;
				float2 uv_NormalTex;
				float2 uv_IntersectionTex;
				float4 screenPos;
				float3 worldPos;
				//UNITY_FOG_COORDS(1)
			};

			float _NormalStrength;
			half _Glossiness;
			half _Metallic;
			sampler2D _CameraDepthTexture;
			fixed4 _Color;
			fixed4 _IntersectionColor;
			float _FadeLength;


			float4 _WaveA, _WaveB, _WaveC;
			float _WaveSpeed;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			float4 Aura_FrustumRanges;
			sampler3D Aura_VolumetricLightingTexture;
			float InverseLerp(float lowThreshold, float hiThreshold, float value)
			{
				return (value - lowThreshold) / (hiThreshold - lowThreshold);
			}
			float4 Aura2_GetFogValue(float3 screenSpacePosition)
			{
				return tex3Dlod(Aura_VolumetricLightingTexture, float4(screenSpacePosition, 0));
			}
			void Aura2_ApplyFog(inout fixed4 colorToApply, float3 screenSpacePosition)
			{
				float4 fogValue = Aura2_GetFogValue(screenSpacePosition);
				// Always apply fog attenuation - also in the forward add pass.
				colorToApply.xyz *= fogValue.w;
				// Alpha premultiply mode (used with alpha and Standard lighting function, or explicitly alpha:premul)
#if _ALPHAPREMULTIPLY_ON
				fogValue.xyz *= colorToApply.w;
#endif
				// Add inscattering only once, so in forward base, but not forward add.
#ifndef UNITY_PASS_FORWARDADD
				colorToApply.xyz += fogValue.xyz;
#endif
			}

			void Aura2_Fog(Input IN, SurfaceOutputStandard o, inout fixed4 color)
			{
#if defined(AURA)
				half3 screenSpacePosition = IN.screenPos.xyz / IN.screenPos.w;
				screenSpacePosition.z = InverseLerp(Aura_FrustumRanges.x, Aura_FrustumRanges.y, LinearEyeDepth(screenSpacePosition.z));

				//// Debug fog only
#if defined(AURA_DISPLAY_VOLUMETRIC_LIGHTING_ONLY)
				color.xyz = float3(0.0f, 0.0f, 0.0f);
#endif

				Aura2_ApplyFog(color, screenSpacePosition);
#endif
			}



			float4 ApplyFog(float4 color, Input i) {
				float viewDistance = length(_WorldSpaceCameraPos - i.worldPos);
				UNITY_CALC_FOG_FACTOR_RAW(viewDistance);
				color.rgb = lerp(unity_FogColor.rgb, color.rgb, saturate(unityFogFactor));
				return color;
			}

			float3 GerstnerWave(float4 wave, float3 p, inout float3 tangent, inout float3 binormal)
			{
				float steepness = wave.z;
				float wavelength = wave.w;

				float w = 2 * UNITY_PI / wavelength;  //Wavelength converted to radians.
				float c = sqrt(9.8 / w); //Calculate gravitational pull/phasing of the waves.
				float2 d = normalize(wave.xy);
				float f = w * (dot(d, p.xz) - c * _Time.y * _WaveSpeed); //Main wave-function.
				float s = steepness / w; //Wave steepness/sharpness.


				p.x += d.x * (s * cos(f));
				p.y = s * sin(f);
				p.z += d.y * (s * cos(f));

				//Props to Catlikecoding.com for this shader, especially the normal-calculations.
				tangent += float3(-d.x * d.x * (steepness * sin(f)), d.x * (steepness * cos(f)), -d.x * d.y * (steepness * sin(f))); //Calculate tangent to use as crossproduct to calculate normals later.
				binormal += float3(-d.x * d.y * (steepness * sin(f)), d.y * (steepness * cos(f)), -d.y * d.y * (steepness * sin(f))); //Tangent in Z-dimension.

				return float3(d.x * (s *  cos(f)), s * sin(f), d.y * (s * cos(f)));
			}

			void vert(inout appdata_full vertexData)
			{
				float3 gridPoint = vertexData.vertex.xyz;
				float3 tangent = float3(1, 0, 0);
				float3 binormal = float3(0, 0, 1);
				float3 p = gridPoint;

				p += GerstnerWave(_WaveA, gridPoint, tangent, binormal); //Add a Gerstnerwave to our vertex-function.
				p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
				p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);

				float3 normal = normalize(cross(binormal, tangent)); //Cross-product gives the normal-vector.

				vertexData.vertex.xyz = p;
				vertexData.normal = normal;


				//UNITY_TRANSFER_FOG(vertexData, vertexData.vertex);
				
				//Aura2_GetFrustumSpaceCoordinates(vertexData);
				//vertexData.vertex.xyz = Aura2_GetFrustumSpaceCoordinates(float4 vertexData);
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				//Fetch each pixel-distance for both the object and the scene.
				//Then get the difference and add an intersection based on this distance.
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
				float surfZ = -mul(UNITY_MATRIX_V, float4(IN.worldPos.xyz, 1)).z;
				float diff = sceneZ - surfZ;
				float intersect = 1 - saturate(diff / _FadeLength);

				fixed4 col = fixed4(lerp(tex2D(_MainTex, IN.uv_MainTex + _Time.x * _WaveSpeed) * _Color, _IntersectionColor * tex2D(_IntersectionTex, IN.uv_IntersectionTex - _Time.x * _WaveSpeed), pow(intersect, 4)));
				
				//Aura2_ApplyLighting(col, IN.screenPos, 1);

				//UNITY_APPLY_FOG(IN.uv_MainTex, col);

				//IN.worldPos = mul(unity_ObjectToWorld, o.vertex);
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
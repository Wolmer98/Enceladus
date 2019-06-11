// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/PlanetShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "Transparent" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MetallicMap("Metallic Texture", 2D) = "white" {}
		_NormalMap("Normalmap", 2D) = "bump" {}
		_NormalStrength("Normal Strength", float) = 1
		_EmissionMap("Emmision Texture", 2D) = "black" {}
		_Distortion("Distortion", float) = 1
		_DistortionSpeed("Distortion Speed", float) = 1
		_DistTex("Distortion Texture", 2D) = "white" {}
		_DistMask("Distortion Mask Texture", 2D) = "white" {}
		_Overlay("Overlay", 2D) = "white" {}
		_OverlayColor("Overlay Color", Color) = (0,0,0)
		_OverlayTiling("Overlay Tiling", float) = 1
		_ScrollX("Overlay Scroll X", float) = 0
		_ScrollY("Overlay Scroll Y", float) = 0
		_Fresnel("Fresnel Factor", Range(0, 3)) = 0
		_FresnelColor("Fresnel Color", Color) = (0.73, 0.67, 1)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _NormalMap;
		float _NormalStrength;
		sampler2D _MetallicMap;
		sampler2D _EmissionMap;
		float _Distortion;
		float _DistortionSpeed;
		sampler2D _DistTex;
		sampler2D _DistMask;
		sampler2D _Overlay;
		fixed4 _OverlayColor;
		float _OverlayTiling, _ScrollX, _ScrollY;
		float _Fresnel;
		fixed4 _FresnelColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			//Fresnel.
			half factor = dot(normalize(IN.viewDir), o.Normal);
			o.Emission.rgb = (_Fresnel - factor * _Fresnel) * _FresnelColor;
			o.Emission.rgb += tex2D(_EmissionMap, IN.uv_MainTex);

			//Distortion.
			IN.uv_MainTex += tex2D(_DistTex, IN.uv_MainTex) * tex2D(_DistMask, IN.uv_MainTex + _Time.x * _DistortionSpeed) * _Distortion;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Metallic = _Metallic * tex2D(_MetallicMap, IN.uv_MainTex);
			o.Smoothness = _Glossiness;

			o.Normal = UnpackScaleNormal(tex2D(_NormalMap, IN.uv_MainTex), _NormalStrength);

			//Overlay.
			IN.uv_MainTex.x += _Time.x * _ScrollX;
			IN.uv_MainTex.y += _Time.x * _ScrollY;
			c += tex2D(_Overlay, IN.uv_MainTex * _OverlayTiling) * _OverlayColor;


			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

Shader "Standard_Roughness"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}		
		_GlossinessTex("Smoothness Map / Roughness", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,3)) = 1.5
		_MetallicGlossMap("Metallic Tex", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[HDR]_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		[Header(Stencil Settings)]
		[IntRange] _RefNumber("Reference Number", Range(0,255)) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _CompValue("Stencil Comparison Operation" , Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilFail("Stencil Fail Write Operation", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail("Stencil ZFail Write Operation", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Pass Write Operation", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Glowable" = "True"}
        LOD 200

		stencil
		{
			Ref[_RefNumber] //The stencils reference number.
			Comp[_CompValue] //How to compare to the stencil buffer to make the stencil-test pass or fail.
			Fail[_StencilFail] //What to do to the stencil buffer if depth-test passes but stencil-test fail.
			ZFail[_StencilZFail] //What tot do to the stencil buffer if depth-test fail but stencil-test passes.
			Pass[_StencilOp] //What to do to the stencil buffer if both depth-test and stencil-test passes.
		}

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _GlossinessTex;
        sampler2D _MetallicGlossMap;
        sampler2D _EmissionMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_GlossinessTex;
            float2 uv_BumpMap;
            float2 uv_MetallicGlossMap;
            float2 uv_EmissionMap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _EmissionColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic * tex2D(_MetallicGlossMap, IN.uv_MetallicGlossMap);
            o.Smoothness = _Glossiness * (1-tex2D(_GlossinessTex, IN.uv_GlossinessTex));
            o.Alpha = c.a;
			o.Emission = _EmissionColor * tex2D(_EmissionMap, IN.uv_EmissionMap);
        }
        ENDCG
    }
    FallBack "Diffuse"
}

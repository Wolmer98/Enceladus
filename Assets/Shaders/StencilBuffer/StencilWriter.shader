Shader "Custom/StencilWriter" {
	Properties
	{
		[IntRange] _RefNumber("Reference Number", Range(0,255)) = 1

		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilCompOp("Stencil Compare Operation", Float) = 8
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilFail("Stencil Fail Write Operation", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail("Stencil ZFail Write Operation", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Pass Write Operation", Float) = 2
	}

	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry+2"}
		ColorMask 0
		ZWrite off

		Stencil 
		{
			Ref [_RefNumber] //The stencils reference number.
			Comp [_StencilCompOp] //How to compare to the stencil buffer to make the stencil-test pass or fail.
			Fail [_StencilFail] //What to do to the stencil buffer if depth-test passes but stencil-test fail.
			ZFail [_StencilZFail] //What tot do to the stencil buffer if depth-test fail but stencil-test passes.
			Pass [_StencilOp] //What to do to the stencil buffer if both depth-test and stencil-test passes.
		}

		CGINCLUDE
			struct appdata {
				float4 vertex : POSITION;
			};
			struct v2f {
				float4 pos : SV_POSITION;
			};
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			half4 frag(v2f i) : SV_Target {
				return half4(1,1,0,1);
			}
		ENDCG

		Pass {
			Cull Front
			ZTest Less

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
		Pass {
			Cull Back
			ZTest Greater

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
Shader "MoonShell"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Cull Off

		Zwrite Off

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
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _Mask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 viewDir = float3(0, 0, 1);

				float rimlight = saturate(dot(viewDir, i.normal) + 0.7);

				float mask = tex2D(_Mask, i.uv);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				col.a = rimlight * mask;
                return col;
            }
            ENDCG
        }
    }
}

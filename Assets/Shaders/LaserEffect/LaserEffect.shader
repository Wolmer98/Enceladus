Shader "Unlit/LaserEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_Layer0 ("Layer 0", 2D) = "white" {}
		_Speed0 ("Speed", Float) = 0.1
		_Color0 ("Color", Color) = (1, 1, 1, 1)

		_Layer1	("Layer 1", 2D) = "white" {}
		_Speed1 ("Speed", Float) = 0.1
		_Color1("Color", Color) = (1, 1, 1, 1)

		_Layer2	("Layer 2", 2D) = "white" {}
		_Speed2 ("Speed", Float) = 0.1
		_Color2("Color", Color) = (1, 1, 1, 1)

		_AllLayers ("AllLayers", 2D) = "white"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _Layer0;
			float _Speed0;
			float4 _Color0;

			sampler2D _Layer1;
			float _Speed1;
			float4 _Color1;

			sampler2D _Layer2;
			float _Speed2;
			float4 _Color2;

			sampler2D _AllLayers;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float layer0 = tex2D(_Layer0, i.uv + float2(_Speed0, 0) * _Time.y).r;
				float layer1 = tex2D(_Layer1, i.uv + float2(_Speed1, 0) * _Time.y).r;
				float layer2 = tex2D(_Layer2, i.uv + float2(_Speed2, 0) * _Time.y).r; 

				/*layer0 = tex2D(_AllLayers, i.uv + float2(_Speed0, 0) * _Time.y).r;
				layer1 = tex2D(_AllLayers, i.uv + float2(_Speed1, 0) * _Time.y).g;
				layer2 = tex2D(_AllLayers, i.uv + float2(_Speed2, 0) * _Time.y).b;*/


				float4 result = layer0 * _Color0 + layer1 * _Color1 + layer2 * _Color2;

				result = layer0 * _Color0  + layer1 * _Color1 + layer2 * _Color2;
				result = saturate(result);

                // sample the texture
                fixed4 col = result;
                return col;
            }
            ENDCG
        }
    }
}

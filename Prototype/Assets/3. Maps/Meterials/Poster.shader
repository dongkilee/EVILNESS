Shader "Custom/Poster" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpTex ("BumpMap ", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		cull off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows 

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpTex;
		};

		half _Glossiness;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			float3 n = UnpackNormal(tex2D (_BumpTex, IN.uv_BumpTex));
			o.Albedo = c.rgb;
			o.Normal = n;
			//o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Emission = c.rgb * 0.02;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse/Transparent/Cutout/Diffuse"
}

Shader "Custom/Branches" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff ("Cutout",Range(0.01,0.5)) = 0.5
	}

	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }

		cull off

		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * 1.5;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Legacy Shaders/Transparent/Cutout/Diffuse"
}

Shader "Custom/mask" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGBa)", 2D) = "white" {}
		_Value ("밝기", range(0,3)) = 1.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {

		float2 uv_MainTex;

		};

		fixed4 _Color;
		half _Glossiness;
		float _Value;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//o.Emission = _Color *(cos( _Time.z) * 0.5f + 0.5f) *1.5+float3(0.2,0.22,0.4) ;
			o.Emission = _Color *(cos( _Time.z) * 0.5f + 0.5f) + float3(0.1,0.1,0.1) * _Color *_Value ;
			o.Smoothness = 0.8 ;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

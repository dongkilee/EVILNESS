Shader "Custom/Flicker" {
	Properties {
		_Color ("Color", Color) = (0,0,0,0)
		_Range ("Emission Power", Range(0,2)) = 1
		_timeRange("Speed", Range(0.3,2)) = 1
		_Ramp ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Flicker



		sampler2D _Ramp;
		float4 _Color;
		float _Range;
		float _timeRange;

		struct Input {
			float2 uv_Ramp;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_Ramp, float2(abs(sin(_Time.y))*_timeRange,0));
			o.Emission = c * _Color * _Range;
			o.Alpha = c.a;
		}

		float4 LightingFlicker(SurfaceOutput e, float3 lightDir, float atten) {
			return float4 (0.3,0.3,0.3,1);
		}



		ENDCG
	}
	FallBack "Diffuse"
}

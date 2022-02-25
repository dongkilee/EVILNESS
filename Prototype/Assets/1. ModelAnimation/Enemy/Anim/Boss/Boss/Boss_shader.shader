Shader "Custom/Boss_shader 1" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpTex("normal", 2D) = "white" {}
		_Oc("oc",2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_RimColor ("RimColor", Color) = (1,1,1,1)
		_RimPower ("RimPower", Range(1,5)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;
		sampler2D _Oc;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;

		};


		float4 _RimColor;
		float _RimPower;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_MainTex));
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 Oc = tex2D(_Oc, IN.uv_MainTex);
			float rim = dot(o.Normal,IN.viewDir);
			rim = 1 - rim;
			o.Emission = pow(rim, _RimPower) * _RimColor  ;
			o.Albedo = c.rgb *1.7 ;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

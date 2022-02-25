Shader "Custom/NewSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DirtyTex ("DirtyTex", 2D) = "white" {}
		_BumpMap ("normal", 2D) = "white" {}
		_BumpMap2 ("wetNormal", 2D) = "white" {}
		_glossmask ("glossmask", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DirtyTex;
		sampler2D _BumpMap;
		sampler2D _BumpMap2;
		sampler2D _glossmask;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DirtyTex;
			float2 uv_BumpMap;
			float2 uv_BumpMap2;
			float2 uv_glossmask;
		};

		half _Glossiness;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) *_Color ;
			fixed4 d = tex2D (_DirtyTex, IN.uv_DirtyTex);
			float3 n = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap)) ;
			float3 nn = UnpackNormal(tex2D(_BumpMap2,IN.uv_BumpMap2)) ;
			fixed4 m = tex2D (_glossmask, IN.uv_glossmask) ;

			o.Albedo = c.rgb * (1-(m*0.6)) * d.rgb ;
			o.Normal = (lerp(n,nn,m.r));
			o.Smoothness = _Glossiness * m + 0.3  ;
			o.Metallic = 0.5;
			o.Alpha = c.a ;
		}

		ENDCG
	}
	FallBack "Diffuse"
}

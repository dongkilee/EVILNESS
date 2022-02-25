Shader "Custom/char" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBa)", 2D) = "white" {}
		_BumpMap ("노말맵", 2D) = "bump" {}
		_AO ("AO맵",2D) = "white" {}
		_MaskMap("MaskMap",2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
			cull off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _AO;
		sampler2D _MaskMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_MaskMap;
			float3 viewDir;
			float3 worldNormal;
			INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 m = tex2D(_MaskMap, IN.uv_MaskMap);
			float3 nor = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Normal = nor;
			float3 WorldNor = WorldNormalVector(IN, o.Normal);


			float rim = abs(dot(IN.viewDir, nor));
			float invrim = 1 - rim;
			//rim
			float3 rim1 = invrim * c.rgb *0.5;
			//Worldrim
			float3 worldrim = c.rgb *c.rgb * saturate(WorldNor.y );

			//float3 2nd specular
			float3 fakespecular = pow(rim,30) * c.rgb ;


			o.Albedo = c.rgb * 2;
			
			o.Occlusion = tex2D(_AO, IN.uv_MainTex) ;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness *m.r;
			o.Emission = rim1 + worldrim*0.8 + (fakespecular * m.r);

			//o.Emission = saturate(WorldNor.y * 0.5 + 0.5);
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

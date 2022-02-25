Shader "Custom/Char_S" {
	Properties {
		_MainTex ("Albedo", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "BumpMap" {}
		_MaskMap ("Mask Map", 2D) = "BumpMap" {}
		_NormalRR ("Normal Power",Range (0,2)) = 0
		_RimCol ("Rim Color", Color) = (1,1,1,1)
		_RimPow ("Rim Power", Range(1,10)) = 1
		_SpecCol ("Specular Color", Color) = (1,1,1,1)
		_Gloss ("Specular Power", Range (0,3))=0
		_SpecPow ("Specular Range", Range(1,50)) =10

	}


	SubShader {
		Tags { "RenderType"="Opaque" }
		cull off
		CGPROGRAM
		#pragma surface surf EL

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MaskMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_MaskMap;
		};


		half _NormalRR;
		half _SpecPow;
		half _RimPow;
		half _FakeSpecPow;
		half _Gloss;

		fixed4 _Color;
		fixed4 _RimCol;
		fixed4 _SpecCol;
		fixed4 _FakeSpecCol;


		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			float3 n = UnpackNormal(tex2D (_BumpMap, IN.uv_MainTex));
			float3 m = tex2D (_MaskMap, IN.uv_MaskMap);
			o.Normal = n * _NormalRR;
			o.Albedo = c.rgb * 0.7 ;
			o.Emission = c.rgb *0.15;
			o.Gloss = _Gloss *m.rgb;
			o.Alpha = c.a;

		}



		float4 LightingEL (SurfaceOutput e, float3 lightDir, float3 viewDir, float atten){

		//Diffuse Color Room
			float3 DiffCol;
			float NdotL = saturate(dot(e.Normal,lightDir)) ;
			DiffCol = (NdotL * e.Albedo * _LightColor0.rgb * atten) * 1.5 ;

		//Specular Color Room
			float3 SpecCol1;
			float3 H = normalize(lightDir + viewDir) ;
			float Spec = saturate(dot(H,e.Normal));
			Spec = pow(Spec, _SpecPow);
			SpecCol1 = Spec * _SpecCol;
			SpecCol1 = SpecCol1 * e.Gloss;

		//Rim Light Room
			float3 rimCol;
			float rim = dot(e.Normal, viewDir) * 0.5 + 0.5;
			rimCol = 1- rim;
			rimCol = pow(rimCol, _RimPow) * _RimCol;



		//Outpur Room
			float4 finalColor;
			finalColor.rgb = DiffCol + rimCol + SpecCol1;

			return finalColor;

			}


		ENDCG
	}
	FallBack "Diffuse"
}

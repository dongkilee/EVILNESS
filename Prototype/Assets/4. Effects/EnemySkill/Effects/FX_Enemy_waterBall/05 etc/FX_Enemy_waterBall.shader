Shader "Custom/FX_Enemy_waterBall" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_rimColor ("rim Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecColor ("specular color" , Color) = (1,1,1,1)
		_SpecRange ("_SpecRange" , range(0,5)) = 0
		_GlossRange ("_GlossRange" , range(0,5)) = 0
		_alpharange ("_alpharange" , range(0,5)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf BlinnPhong //alpha:blend

		sampler2D _MainTex;
		float4 _rimColor;
		float _SpecRange;
		float _GlossRange;
		float _alpharange;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex)* _Color  ;

			float rim =1-saturate (dot (IN.worldNormal , IN.viewDir)) ; 
//			o.Albedo = c.rgb+ pow ( rim , 4)*_rimColor ;
			o.Emission = c.rgb+ pow ( rim ,2)*_rimColor ;
			o.Specular = 1 * _SpecRange;
			o.Gloss = 1*_GlossRange;
			o.Alpha = 1* _alpharange;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

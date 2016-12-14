Shader "Custom/terrainShaderImg" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_PeakColor("Albedo (RGB)", 2D) = "white" {}
		//_PeakColor("Albedo (RGB)", 2D) = "white" {}
		_PeakLevel("PeakLevel", Float) = 300
		_Level3Color("Albedo (RGB)", 2D) = "white" {}
		_Level3("Level3", Float) = 200
		_Level2Color("Albedo (RGB)", 2D) = "white" {}
		_Level2("Level2", Float) = 100
		_Level1Color("Albedo (RGB)", 2D) = "white" {}
		_WaterLevel("WaterLevel", Float) = 0
		_WaterColor("WaterColor", Color) = (0.37,0.78,0.92,1)


		//_Color ("Color", Color) = (1,1,1,1)
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows


#pragma target 3.0

		sampler2D _PeakColor;
		sampler2D _Level3Color;
		sampler2D _Level2Color;
		sampler2D _Level1Color;

	struct Input {
		float3 worldPos;
		float3 customColor;
		float2 uv_PeakColor;
		float2 uv_BumpMap;
	};

	float _PeakLevel;
	//float4 _PeakColor;
	float _Level3;
	//float4 _Level3Color;
	float _Level2;
	//float4 _Level2Color;
	float _Level1;
	//float4 _Level1Color;
	float _WaterLevel;
	float4 _WaterColor;

	float _Slope;

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	sampler2D _BumpMap;

	void vert(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.customColor = abs(v.normal.y);

	}

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c;
		//if (IN.worldrotation >= 0)
		//o.Albedo = _PeakColor;
		if (IN.worldPos.y >= _PeakLevel)
		{
			c = tex2D(_Level3Color, IN.uv_PeakColor) * _Color;
			o.Albedo = c.rgb;
		}
		if (IN.worldPos.y <= _PeakLevel)
		{
			fixed4 c = tex2D(_PeakColor, IN.uv_PeakColor) * _Color;
			o.Albedo = c.rgb;
		}
		if (IN.worldPos.y <= _Level3)
		{
			fixed4 c = tex2D(_Level2Color, IN.uv_PeakColor) * _Color;
			o.Albedo = c.rgb;
		}
		if (IN.worldPos.y <= _Level2)
		{
			fixed4 c = tex2D(_Level1Color, IN.uv_PeakColor) * _Color;
			o.Albedo = c.rgb;
		}
		if (IN.worldPos.y <= _WaterLevel)
			o.Albedo = _WaterColor;

		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	}
		FallBack "Diffuse"
}

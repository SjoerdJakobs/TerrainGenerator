Shader "Custom/DynamicTerrainShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Color("Main Color", Color) = (0,0,0,1)
		_WorldScale("World Scale", Float) = 0.25
		_AltitudeScale("Altitude Scale", float) = 0.25
		_TerrainBands("Terrain Bands", Int) = 4
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _MainTex;
	float _WorldScale;
	float _AltitudeScale;
	float4 _Color;
	int _TerrainBands;

	struct Input {
		float3 worldPos;
		float2 uv_MainTex;
		float2 uv_BumpMap;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		float y = IN.worldPos.y / _AltitudeScale + 0.5;
		float bandWidth = 1.0 / _TerrainBands;
		float s = clamp(y * (_TerrainBands + 2) - 2, 0, _TerrainBands);
		float t = frac(s);
		t = t < 0.25 ? t * 0.5 : (t > 0.75 ? (t - 0.75) * 0.5 + 0.875 : t * 1.5 - 0.25);
		float band = floor(s)  * bandWidth;
		float2 uv = frac(IN.uv_MainTex * _WorldScale) * (bandWidth - 0.006, bandWidth - 0.006) + (0.003, 0.003);
		uv.y = uv.y + band;
		float2 uv2 = uv;
		uv2.y = uv2.y + bandWidth;
		half4 c = tex2D(_MainTex, uv) * (1 - t) + tex2D(_MainTex, uv2) * t;
		o.Albedo = c.rgb * 0.5;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
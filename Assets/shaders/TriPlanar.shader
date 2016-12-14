// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "InfiniteTerrain/TriPlanar" {
	Properties{
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TopWeighting("Vertical Falloff", Range(0.1, 10)) = 5
		_UpTex("Top", 2D) = "red" {}
		_SideTex("Side", 2D) = "green" {}
		_DownTex("Bottom", 2D) = "blue" {}
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard vertex:vert fullforwardshadows
#pragma target 3.0

		//struct v2f {
		//	float4 pos : SV_POSITION;
		//	float4 vec : SV_NORMAL;
		//};

		struct Input {
			float3 worldPos : SV_POSITION;
			float3 worldNormal;
			float3 pos;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.pos = mul(unity_ObjectToWorld, v.vertex);
		}

		sampler2D _UpTex;
		float4 _UpTex_ST; //_ST for the Scale & Transform

		sampler2D _SideTex;
		float4 _SideTex_ST;

		sampler2D _DownTex;
		float4 _DownTex_ST;

		float _TopWeighting;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Sample the various textures we're using
			
			fixed4 up = tex2D(_UpTex, fmod(IN.pos.xz * _UpTex_ST, 1));
			fixed4 sidex = tex2D(_SideTex, IN.pos.yz * _SideTex_ST);
			fixed4 sidez = tex2D(_SideTex, IN.pos.xy * _SideTex_ST);
			fixed4 down = tex2D(_DownTex, IN.pos.xz * _DownTex_ST);

			// Work out if we're doing the top or bottom
			float top = clamp(IN.worldNormal.y * 100000, 0, 1);

			// Work out how much weight should be given to the various textures
			float3 blending = abs(float3(IN.worldNormal.x / _TopWeighting, IN.worldNormal.y, IN.worldNormal.z / _TopWeighting));

			// Force weights to sum to 1.0
			blending = normalize(max(blending, 0.00001)); 

			// scale the various weights proportionally
			float b = (blending.x + blending.y + blending.z);
			blending /= float3(b, b, b);

			// Combine the textures with the appropriate weighting
			float4 tex = sidex * blending.x +
						 (top) * up * blending.y +
						 (1-top) * down * blending.y +
						 sidez * blending.z;
			o.Albedo = tex;

			// Metallic and smoothness aren't currently used,
			// set them to some neutral values

			o.Metallic = 0;
			o.Smoothness = .1;
			o.Alpha = 0;
		}

		ENDCG
	}
	FallBack "Diffuse"
}

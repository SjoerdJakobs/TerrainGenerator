Shader "Custom/Terrain" 
{
	Properties 
	{
		_SandTex ("Sand", 2D) = "white" {}
		_GravelTex ("Gravel", 2D) = "white" {}
		_RockTex ("Rock", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		//Cull off
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _SandTex, _GravelTex, _RockTex;

		struct Input 
		{
			float3 worldPos;
			float3 worldNormal;
			float4 color : COLOR;
		};
		
		float4 TriplanarSample(sampler2D tex, float3 worldPosition, float3 projNormal, float scale)
		{
			float4 cZY = tex2D(tex, worldPosition.zy * scale);
			float4 cXZ = tex2D(tex, worldPosition.xz * scale);
			float4 cXY = tex2D(tex, worldPosition.xy * scale);
			
			cXY = lerp(cXY, cXZ, projNormal.y);
			return lerp(cXY, cZY, projNormal.x);
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			float3 projNormal = saturate(pow(IN.worldNormal * 1.5, 4));
			
			half4 sand = TriplanarSample(_SandTex, IN.worldPos, projNormal, 1.0);
			
			half4 gravel = TriplanarSample(_GravelTex, IN.worldPos, projNormal, 1.0);
			
			half4 rock = TriplanarSample(_RockTex, IN.worldPos, projNormal, 1.0);
			
			float4 controlMap = IN.color;
			
			half4 col = lerp(rock, gravel, controlMap.g);
			col = lerp(col, sand, controlMap.r);
			
			o.Albedo = col.rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

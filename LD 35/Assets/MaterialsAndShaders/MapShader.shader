Shader "Custom/Map"
{
	Properties
	{
		[PerRendererData] _BGTex("Empty map texture", 2D) = "white" {}
		[PerRendererData] _FullTex("Filled map texture", 2D) = "red" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector]_CameraUVRect("", Vector) = (0.2, 0.2, 0.6, 0.6)
		[HideInInspector] _UIAlpha("Alpha of UI", Float) = 0
		[HideInInspector] _UISize("Size of UI", Float) = 0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_MainTex("MainTex for sprite renderer", 2D) = "white" {}
	}

	SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#pragma shader_feature ETC1_EXTERNAL_ALPHA
#include "UnityCG.cginc"

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
	};

	fixed4 _Color;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}

	sampler2D _BGTex;
	sampler2D _FullTex;
	sampler2D _AlphaTex;
	float4 _CameraUVRect;
	float _UIAlpha;
	float _UISize;

	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 colorBG = tex2D(_BGTex, uv);
		fixed4 colorFull = tex2D(_FullTex, uv);
		fixed4 color;

		float2 cameraRectMax = _CameraUVRect.xy + _CameraUVRect.zw;

		if (uv.x > _CameraUVRect.x && uv.y > _CameraUVRect.y && uv.x < cameraRectMax.x && uv.y < cameraRectMax.y)
		{
			if (_UIAlpha > 0.0f)
			{				
				float size = _UISize;
				float curve = 4.0f;
				// y needs to go from top,
				// we want the inner circle to be just in the corder => right triangle with equal sides where we know c
				float2 squircle_ab = _CameraUVRect.xy + float2(0.0f, _CameraUVRect.w) + float2(size / sqrt(2.0f),-size / sqrt(2.0f));
				float actualR = pow(uv.x - squircle_ab.x, curve) + pow(uv.y - squircle_ab.y, curve);
				float fullR = pow(size, curve);
				if (actualR < fullR)
				{
					float blendR = fullR * 0.7f;
					float blendFactor = (actualR - blendR) / (fullR - blendR);

					if (blendFactor > 0.0f) {
						color = lerp(colorFull, colorBG, _UIAlpha * (1.0f - blendFactor));
						//color = fixed4(1, 0, 0, 1);
					}
					else
					{
						color = lerp(colorFull, colorBG, _UIAlpha);
					}

					
				}
				else
				{
					color = colorFull;
				}
			}
			else
			{
				color = colorFull;
			}
			
		}
		else
		{
			color = colorBG;
		}

#if ETC1_EXTERNAL_ALPHA
		// get the color from an external texture (usecase: Alpha support for ETC1 on android)
		color.a = tex2D(_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

		return color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
		c.rgb *= c.a;
		return c;
	}
		ENDCG
	}
	}
}




//Shader "Custom/MapShader" {
//	Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//	}
//	SubShader {
//		Tags { "RenderType"="Opaque" }
//		LOD 200
//		
//		CGPROGRAM
//		// Physically based Standard lighting model, and enable shadows on all light types
//		#pragma surface surf Standard fullforwardshadows
//
//		// Use shader model 3.0 target, to get nicer looking lighting
//		#pragma target 3.0
//
//		sampler2D _MainTex;
//
//		struct Input {
//			float2 uv_MainTex;
//		};
//
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
//
//		void surf (Input IN, inout SurfaceOutputStandard o) {
//			// Albedo comes from a texture tinted by color
//			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;
//			// Metallic and smoothness come from slider variables
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}

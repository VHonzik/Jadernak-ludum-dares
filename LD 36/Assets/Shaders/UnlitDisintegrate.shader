// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Disintegrate" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Disintegrate("Disintegrate map (RGB)", 2D) = "black" {}
	_Control("Disintegrate control (RGB)", float) = 0.0
	_BurnColor("Burn color", Color) = (0.949,0.525,0.1529,1)
	_BurnColorMore("Burn color more", Color) = (1.0,0.1647,0.0588,1)
	_BurnColorMost("Burn color most", Color) = (1.0,0.9176,0.6745,1)
	_BurnedColor("Burned", Color) = (0.0,0,0,1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Disintegrate;

			float _Control;

			fixed4 _BurnColor;
			fixed4	_BurnColorMore;
			fixed4	_BurnedColor;
			fixed4 _BurnColorMost;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
			    fixed disintegrate = 1.0 - tex2D(_Disintegrate, i.texcoord).r;

				float burn_speed = 0.01;
				float pre_burn_speed = 0.01;

				if (_Control >= disintegrate)
				{
					col.a = 0.0;
				}
				else if (_Control >= disintegrate - burn_speed)
				{
					col = _BurnedColor;
					col.a = (_Control - (disintegrate - burn_speed)) / burn_speed;
				}
				else if (_Control >= disintegrate - burn_speed -  pre_burn_speed)
				{
					float pre_burn_control = (_Control - (disintegrate - burn_speed - pre_burn_speed)) / pre_burn_speed;
					col = lerp(_BurnColorMost, _BurnedColor, pre_burn_control);					
				}
				else if (_Control >= disintegrate - burn_speed - 2 * pre_burn_speed)
				{
					float pre_burn_control = (_Control - (disintegrate - burn_speed - 2 * pre_burn_speed)) / pre_burn_speed;
					col = lerp(_BurnColorMore, _BurnColorMost, pre_burn_control);
				}
				else if (_Control >= disintegrate - burn_speed - 3 * pre_burn_speed)
				{
					float pre_burn_control = (_Control - (disintegrate - burn_speed - 3 * pre_burn_speed)) / pre_burn_speed;
					col = lerp(_BurnColor, _BurnColorMore, pre_burn_control);
				}
				else if (_Control >= disintegrate - burn_speed - 4 * pre_burn_speed)
				{
					float pre_burn_control = (_Control - (disintegrate - burn_speed - 4 * pre_burn_speed)) / pre_burn_speed;
					col = lerp(col, _BurnColor, pre_burn_control);
				}

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
		ENDCG
	}
}

}

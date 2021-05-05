
Shader "Unlit/TextureBlacken" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
    _BurnedSpeed("Burned blend map (RGB)", 2D) = "black" {}
	_Burned("Burned texture (RGB)", 2D) = "black" {}
    _Control("Burned control value", float) = 0.0
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
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

			sampler2D _BurnedSpeed;
			float4 _BurnedSpeed_ST;

			sampler2D _Burned;
			float4 _Burned_ST;

			float _Control;
			
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
				fixed4 burned = tex2D(_Burned, i.texcoord);
			    fixed burned_start = tex2D(_BurnedSpeed, i.texcoord).r;
				float speed = max(1 / clamp(1.0 - burned_start, 0.001, 1.0), 3.0);
				float control = clamp((_Control * speed) - burned_start, 0.0, 1.0);
				col = lerp(col, burned, control);

				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}

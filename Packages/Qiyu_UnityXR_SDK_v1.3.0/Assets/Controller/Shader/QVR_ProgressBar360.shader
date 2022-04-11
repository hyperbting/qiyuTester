Shader "QVR/ProgressBar360"
{
	Properties
	{
			_MaskTex("Mask Texture", 2D) = "white" {}
			_Color("Tint", Color) = (1,1,1,1)
			_Angle("Angle", range(0,361)) = 360
			_Width("Width", float) = 1
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
				ZTest[unity_GUIZTestMode]
				Blend SrcAlpha OneMinusSrcAlpha

				Pass
				{
					CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag
						#pragma shader_feature CLOCK_WISE

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
							half2 mask_uv  : TEXCOORD0;
						};

						half _Width;
						float _Angle;
						fixed4 _Color;
						sampler2D _MaskTex;
						float4 _MaskTex_ST;

						v2f vert(appdata_t IN)
						{
							v2f OUT;
							OUT.vertex = UnityObjectToClipPos(IN.vertex);
							OUT.mask_uv = TRANSFORM_TEX(IN.texcoord,_MaskTex);
							OUT.color = IN.color * _Color;
							return OUT;
						}

						fixed4 frag(v2f IN) : SV_Target
						{
							float2 center = float2(0.5f, 0.5f);
							half4 color = tex2D(_MaskTex, IN.mask_uv) * IN.color;
							float2 pos = IN.mask_uv.xy - center;
				
							float len2 = dot(pos, pos);
							float factor = step(len2, _Width * _Width);

							float ang = degrees(atan2(pos.x, -pos.y)) + 180;
							color.a = color.a * max(factor, step(_Angle, ang));
							return color;
						}
					ENDCG
				 }
			}
}
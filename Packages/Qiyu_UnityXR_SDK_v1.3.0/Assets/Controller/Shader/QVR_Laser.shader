Shader "QVR/Laser" {
    Properties {
        _Tex_color_01 ("Tex_color_01", Color) = (0.5,0.5,0.5,1)
        _Tex_color_02 ("Tex_color_02", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _color01or02 ("color01or02", Float ) = 0.5
        _Tex ("Tex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+100"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha,SrcAlpha DstAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform fixed4 _Tex_color_01;
            uniform fixed4 _Tex_color_02;
            uniform fixed _color01or02;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                fixed4 _Tex_var = tex2D(_Tex,TRANSFORM_TEX(i.uv0, _Tex));
                float3 finalColor = (lerp( _Tex_color_01.rgb, _Tex_color_02.rgb, _color01or02 )*_Tex_var.rgb);
                fixed4 finalRGBA = fixed4(finalColor, _Tex_var.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
}

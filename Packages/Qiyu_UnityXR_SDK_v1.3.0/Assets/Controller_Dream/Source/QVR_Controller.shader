// Upgrade NOTE: replaced 'defined FOG_COMBINED_WITH_WORLD_POS' with 'defined (FOG_COMBINED_WITH_WORLD_POS)'
Shader "QVR/Controller"
{
	Properties
	{
		_Highlight_color("Highlight_color", Color) = (0,0.5568628,1,1)
		_Type("Type", Range(0 , 1)) = 1
		_Diffuse_color("Diffuse_color", Color) = (0,0,0,0)
		_Diffuse_map("Diffuse_map", 2D) = "white" {}
		_Light("Light", Range(0 , 1)) = 1
		_Light_map("Light_map", CUBE) = "white" {}
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

		SubShader
		{
			Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
			Cull Back
			// ---- forward rendering base pass:
			Pass {
		CGPROGRAM
		// compile directives
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma multi_compile_fog
		#pragma multi_compile_fwdbase noshadow
		#include "HLSLSupport.cginc"
		#define UNITY_INSTANCED_LOD_FADE
		#define UNITY_INSTANCED_SH
		#define UNITY_INSTANCED_LIGHTMAPSTS
		#include "UnityShaderVariables.cginc"
		#include "UnityShaderUtilities.cginc"
		#if !defined(INSTANCING_ON)
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl
		#define WorldNormalVector(data,normal) normal

		// Original surface shader snippet:
		#line 18 ""
		#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
		#endif
		/* UNITY: Original start of shader */
				//#pragma target 3.0
				//#pragma surface surf Unlit keepalpha noshadow 
				struct Input
				{
					float2 uv_texcoord;
					float3 worldRefl;
					INTERNAL_DATA
				};

				uniform float4 _Highlight_color;
				uniform float4 _Diffuse_color;
				uniform sampler2D _Diffuse_map;
				uniform float4 _Diffuse_map_ST;
				uniform samplerCUBE _Light_map;
				uniform float _Light;
				uniform float _Type;

				inline half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
				{
					return half4 (0, 0, 0, s.Alpha);
				}

				void surf(Input i , inout SurfaceOutput o)
				{
					float2 uv_Diffuse_map = i.uv_texcoord * _Diffuse_map_ST.xy + _Diffuse_map_ST.zw;
					float3 ase_worldReflection = i.worldRefl;
					float4 lerpResult9 = lerp(_Highlight_color , ((_Diffuse_color * tex2D(_Diffuse_map, uv_Diffuse_map)) + (texCUBE(_Light_map, ase_worldReflection) * _Light)) , _Type);
					o.Emission = lerpResult9.rgb;
					o.Alpha = 1;
				}


				// vertex-to-fragment interpolation data
				// no lightmaps:
				#ifndef LIGHTMAP_ON
				// half-precision fragment shader registers:
				#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
				#define FOG_COMBINED_WITH_WORLD_POS
				struct v2f_surf {
				  UNITY_POSITION(pos);
				  float2 pack0 : TEXCOORD0; // _texcoord
				  half3 worldRefl : TEXCOORD1;
				  float3 worldNormal : TEXCOORD2;
				  float4 worldPos : TEXCOORD3;
				  fixed3 vlight : TEXCOORD4; // ambient/SH/vertexlights
				  DECLARE_LIGHT_COORDS(5)
				  #if SHADER_TARGET >= 30
				  float4 lmap : TEXCOORD6;
				  #endif
				  UNITY_VERTEX_INPUT_INSTANCE_ID
				  UNITY_VERTEX_OUTPUT_STEREO
				};
				#endif
				// high-precision fragment shader registers:
				#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
				struct v2f_surf {
				  UNITY_POSITION(pos);
				  float2 pack0 : TEXCOORD0; // _texcoord
				  half3 worldRefl : TEXCOORD1;
				  float3 worldNormal : TEXCOORD2;
				  float3 worldPos : TEXCOORD3;
				  fixed3 vlight : TEXCOORD4; // ambient/SH/vertexlights
				  UNITY_FOG_COORDS(5)
				  DECLARE_LIGHT_COORDS(6)
				  #if SHADER_TARGET >= 30
				  float4 lmap : TEXCOORD7;
				  #endif
				  UNITY_VERTEX_INPUT_INSTANCE_ID
				  UNITY_VERTEX_OUTPUT_STEREO
				};
				#endif
				#endif
				// with lightmaps:
				#ifdef LIGHTMAP_ON
				// half-precision fragment shader registers:
				#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
				#define FOG_COMBINED_WITH_WORLD_POS
				struct v2f_surf {
				  UNITY_POSITION(pos);
				  float2 pack0 : TEXCOORD0; // _texcoord
				  half3 worldRefl : TEXCOORD1;
				  float3 worldNormal : TEXCOORD2;
				  float4 worldPos : TEXCOORD3;
				  float4 lmap : TEXCOORD4;
				  DECLARE_LIGHT_COORDS(5)
				  UNITY_VERTEX_INPUT_INSTANCE_ID
				  UNITY_VERTEX_OUTPUT_STEREO
				};
				#endif
				// high-precision fragment shader registers:
				#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
				struct v2f_surf {
				  UNITY_POSITION(pos);
				  float2 pack0 : TEXCOORD0; // _texcoord
				  half3 worldRefl : TEXCOORD1;
				  float3 worldNormal : TEXCOORD2;
				  float3 worldPos : TEXCOORD3;
				  float4 lmap : TEXCOORD4;
				  UNITY_FOG_COORDS(5)
				  DECLARE_LIGHT_COORDS(6)
				  #ifdef DIRLIGHTMAP_COMBINED
				  float3 tSpace0 : TEXCOORD7;
				  float3 tSpace1 : TEXCOORD8;
				  float3 tSpace2 : TEXCOORD9;
				  #endif
				  UNITY_VERTEX_INPUT_INSTANCE_ID
				  UNITY_VERTEX_OUTPUT_STEREO
				};
				#endif
				#endif
				float4 _texcoord_ST;

				// vertex shader
				v2f_surf vert_surf(appdata_full v) {
				  UNITY_SETUP_INSTANCE_ID(v);
				  v2f_surf o;
				  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
				  UNITY_TRANSFER_INSTANCE_ID(v,o);
				  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				  o.pos = UnityObjectToClipPos(v.vertex);
				  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
				  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
				  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				  #endif
				  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
				  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				  #endif
				  o.worldPos.xyz = worldPos;
				  o.worldNormal = worldNormal;
				  float3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
				  o.worldRefl = reflect(-worldViewDir, worldNormal);
				  #ifdef DYNAMICLIGHTMAP_ON
				  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				  #endif
				  #ifdef LIGHTMAP_ON
				  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				  #endif

				  // SH/ambient and vertex lights
				  #ifndef LIGHTMAP_ON
				  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
				  float3 shlight = ShadeSH9(float4(worldNormal,1.0));
				  o.vlight = shlight;
				  #else
				  o.vlight = 0.0;
				  #endif
				  #ifdef VERTEXLIGHT_ON
				  o.vlight += Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, worldPos, worldNormal);
				  #endif // VERTEXLIGHT_ON
				  #endif // !LIGHTMAP_ON

				  COMPUTE_LIGHT_COORDS(o); // pass light cookie coordinates to pixel shader
				  #ifdef FOG_COMBINED_WITH_TSPACE
					UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
				  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
					UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
				  #else
					UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
				  #endif
				  return o;
				}

				// fragment shader
				fixed4 frag_surf(v2f_surf IN) : SV_Target {
				  UNITY_SETUP_INSTANCE_ID(IN);
				// prepare and unpack data
				Input surfIN;
				#ifdef FOG_COMBINED_WITH_TSPACE
				  UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
				#elif defined (FOG_COMBINED_WITH_WORLD_POS)
				  UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
				#else
				  UNITY_EXTRACT_FOG(IN);
				#endif
				UNITY_INITIALIZE_OUTPUT(Input,surfIN);
				surfIN.uv_texcoord.x = 1.0;
				surfIN.worldRefl.x = 1.0;
				surfIN.uv_texcoord = IN.pack0.xy;
				float3 worldPos = IN.worldPos.xyz;
				#ifndef USING_DIRECTIONAL_LIGHT
				  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
				  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif
				surfIN.worldRefl = IN.worldRefl;
				#ifdef UNITY_COMPILER_HLSL
				SurfaceOutput o = (SurfaceOutput)0;
				#else
				SurfaceOutput o;
				#endif
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				fixed3 normalWorldVertex = fixed3(0,0,1);
				o.Normal = IN.worldNormal;
				normalWorldVertex = IN.worldNormal;

				// call surface function
				surf(surfIN, o);

				// compute lighting & shadowing factor
				UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
				fixed4 c = 0;
				#ifndef LIGHTMAP_ON
				c.rgb += o.Albedo * IN.vlight;
				#endif // !LIGHTMAP_ON

				// lightmaps
				#ifdef LIGHTMAP_ON
				  #if DIRLIGHTMAP_COMBINED
					// directional lightmaps
					fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
					half3 lm = DecodeLightmap(lmtex);
				  #else
					// single lightmap
					fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
					fixed3 lm = DecodeLightmap(lmtex);
				  #endif

				#endif // LIGHTMAP_ON


					// realtime lighting: call lighting function
					#ifndef LIGHTMAP_ON
					c += LightingUnlit(o, lightDir, atten);
					#else
					  c.a = o.Alpha;
					#endif

					#ifdef LIGHTMAP_ON
					  // combine lightmaps with realtime shadows
					  #ifdef SHADOWS_SCREEN
						#if defined(UNITY_NO_RGBM)
						c.rgb += o.Albedo * min(lm, atten * 2);
						#else
						c.rgb += o.Albedo * max(min(lm,(atten * 2)*lmtex.rgb), lm*atten);
						#endif
					  #else // SHADOWS_SCREEN
						c.rgb += o.Albedo * lm;
					  #endif // SHADOWS_SCREEN
					#endif // LIGHTMAP_ON

					#ifdef DYNAMICLIGHTMAP_ON
					fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, IN.lmap.zw);
					c.rgb += o.Albedo * DecodeRealtimeLightmap(dynlmtex);
					#endif

					c.rgb += o.Emission;
					UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
					return c;
				  }


				  #endif
					#if defined(INSTANCING_ON)
					#include "UnityCG.cginc"
					#include "Lighting.cginc"
					#include "AutoLight.cginc"

					#define INTERNAL_DATA
					#define WorldReflectionVector(data,normal) data.worldRefl
					#define WorldNormalVector(data,normal) normal

					// Original surface shader snippet:
					#line 18 ""
					#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
					#endif
							struct Input
							{
								float2 uv_texcoord;
								float3 worldRefl;
								INTERNAL_DATA
							};

							uniform float4 _Highlight_color;
							uniform float4 _Diffuse_color;
							uniform sampler2D _Diffuse_map;
							uniform float4 _Diffuse_map_ST;
							uniform samplerCUBE _Light_map;
							uniform float _Light;
							uniform float _Type;

							inline half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
							{
								return half4 (0, 0, 0, s.Alpha);
							}

							void surf(Input i , inout SurfaceOutput o)
							{
								float2 uv_Diffuse_map = i.uv_texcoord * _Diffuse_map_ST.xy + _Diffuse_map_ST.zw;
								float3 ase_worldReflection = i.worldRefl;
								float4 lerpResult9 = lerp(_Highlight_color , ((_Diffuse_color * tex2D(_Diffuse_map, uv_Diffuse_map)) + (texCUBE(_Light_map, ase_worldReflection) * _Light)) , _Type);
								o.Emission = lerpResult9.rgb;
								o.Alpha = 1;
							}


							// vertex-to-fragment interpolation data
							// no lightmaps:
							#ifndef LIGHTMAP_ON
							// half-precision fragment shader registers:
							#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
							#define FOG_COMBINED_WITH_WORLD_POS
							struct v2f_surf {
							  UNITY_POSITION(pos);
							  float2 pack0 : TEXCOORD0; // _texcoord
							  half3 worldRefl : TEXCOORD1;
							  float3 worldNormal : TEXCOORD2;
							  float4 worldPos : TEXCOORD3;
							  fixed3 vlight : TEXCOORD4; // ambient/SH/vertexlights
							  DECLARE_LIGHT_COORDS(5)
							  #if SHADER_TARGET >= 30
							  float4 lmap : TEXCOORD6;
							  #endif
							  UNITY_VERTEX_INPUT_INSTANCE_ID
							  UNITY_VERTEX_OUTPUT_STEREO
							};
							#endif
							// high-precision fragment shader registers:
							#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
							struct v2f_surf {
							  UNITY_POSITION(pos);
							  float2 pack0 : TEXCOORD0; // _texcoord
							  half3 worldRefl : TEXCOORD1;
							  float3 worldNormal : TEXCOORD2;
							  float3 worldPos : TEXCOORD3;
							  fixed3 vlight : TEXCOORD4; // ambient/SH/vertexlights
							  UNITY_FOG_COORDS(5)
							  DECLARE_LIGHT_COORDS(6)
							  #if SHADER_TARGET >= 30
							  float4 lmap : TEXCOORD7;
							  #endif
							  UNITY_VERTEX_INPUT_INSTANCE_ID
							  UNITY_VERTEX_OUTPUT_STEREO
							};
							#endif
							#endif
							// with lightmaps:
							#ifdef LIGHTMAP_ON
							// half-precision fragment shader registers:
							#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
							#define FOG_COMBINED_WITH_WORLD_POS
							struct v2f_surf {
							  UNITY_POSITION(pos);
							  float2 pack0 : TEXCOORD0; // _texcoord
							  half3 worldRefl : TEXCOORD1;
							  float3 worldNormal : TEXCOORD2;
							  float4 worldPos : TEXCOORD3;
							  float4 lmap : TEXCOORD4;
							  DECLARE_LIGHT_COORDS(5)
							  UNITY_VERTEX_INPUT_INSTANCE_ID
							  UNITY_VERTEX_OUTPUT_STEREO
							};
							#endif
							// high-precision fragment shader registers:
							#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
							struct v2f_surf {
							  UNITY_POSITION(pos);
							  float2 pack0 : TEXCOORD0; // _texcoord
							  half3 worldRefl : TEXCOORD1;
							  float3 worldNormal : TEXCOORD2;
							  float3 worldPos : TEXCOORD3;
							  float4 lmap : TEXCOORD4;
							  UNITY_FOG_COORDS(5)
							  DECLARE_LIGHT_COORDS(6)
							  #ifdef DIRLIGHTMAP_COMBINED
							  float3 tSpace0 : TEXCOORD7;
							  float3 tSpace1 : TEXCOORD8;
							  float3 tSpace2 : TEXCOORD9;
							  #endif
							  UNITY_VERTEX_INPUT_INSTANCE_ID
							  UNITY_VERTEX_OUTPUT_STEREO
							};
							#endif
							#endif
							float4 _texcoord_ST;

							// vertex shader
							v2f_surf vert_surf(appdata_full v) {
							  UNITY_SETUP_INSTANCE_ID(v);
							  v2f_surf o;
							  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
							  UNITY_TRANSFER_INSTANCE_ID(v,o);
							  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
							  o.pos = UnityObjectToClipPos(v.vertex);
							  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
							  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
							  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
							  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
							  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
							  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
							  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
							  #endif
							  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
							  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
							  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
							  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
							  #endif
							  o.worldPos.xyz = worldPos;
							  o.worldNormal = worldNormal;
							  float3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
							  o.worldRefl = reflect(-worldViewDir, worldNormal);
							  #ifdef DYNAMICLIGHTMAP_ON
							  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
							  #endif
							  #ifdef LIGHTMAP_ON
							  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
							  #endif

							  // SH/ambient and vertex lights
							  #ifndef LIGHTMAP_ON
							  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
							  float3 shlight = ShadeSH9(float4(worldNormal,1.0));
							  o.vlight = shlight;
							  #else
							  o.vlight = 0.0;
							  #endif
							  #ifdef VERTEXLIGHT_ON
							  o.vlight += Shade4PointLights(
								unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
								unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
								unity_4LightAtten0, worldPos, worldNormal);
							  #endif // VERTEXLIGHT_ON
							  #endif // !LIGHTMAP_ON

							  COMPUTE_LIGHT_COORDS(o); // pass light cookie coordinates to pixel shader
							  #ifdef FOG_COMBINED_WITH_TSPACE
								UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
							  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
								UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
							  #else
								UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
							  #endif
							  return o;
							}

							// fragment shader
							fixed4 frag_surf(v2f_surf IN) : SV_Target {
							  UNITY_SETUP_INSTANCE_ID(IN);
							// prepare and unpack data
							Input surfIN;
							#ifdef FOG_COMBINED_WITH_TSPACE
							  UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
							#elif defined (FOG_COMBINED_WITH_WORLD_POS)
							  UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
							#else
							  UNITY_EXTRACT_FOG(IN);
							#endif
							UNITY_INITIALIZE_OUTPUT(Input,surfIN);
							surfIN.uv_texcoord.x = 1.0;
							surfIN.worldRefl.x = 1.0;
							surfIN.uv_texcoord = IN.pack0.xy;
							float3 worldPos = IN.worldPos.xyz;
							#ifndef USING_DIRECTIONAL_LIGHT
							  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
							#else
							  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
							#endif
							surfIN.worldRefl = IN.worldRefl;
							#ifdef UNITY_COMPILER_HLSL
							SurfaceOutput o = (SurfaceOutput)0;
							#else
							SurfaceOutput o;
							#endif
							o.Albedo = 0.0;
							o.Emission = 0.0;
							o.Specular = 0.0;
							o.Alpha = 0.0;
							o.Gloss = 0.0;
							fixed3 normalWorldVertex = fixed3(0,0,1);
							o.Normal = IN.worldNormal;
							normalWorldVertex = IN.worldNormal;

							// call surface function
							surf(surfIN, o);

							// compute lighting & shadowing factor
							UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
							fixed4 c = 0;
							#ifndef LIGHTMAP_ON
							c.rgb += o.Albedo * IN.vlight;
							#endif // !LIGHTMAP_ON

							// lightmaps
							#ifdef LIGHTMAP_ON
							  #if DIRLIGHTMAP_COMBINED
								// directional lightmaps
								fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
								half3 lm = DecodeLightmap(lmtex);
							  #else
								// single lightmap
								fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
								fixed3 lm = DecodeLightmap(lmtex);
							  #endif

							#endif // LIGHTMAP_ON


								// realtime lighting: call lighting function
								#ifndef LIGHTMAP_ON
								c += LightingUnlit(o, lightDir, atten);
								#else
								  c.a = o.Alpha;
								#endif

								#ifdef LIGHTMAP_ON
								  // combine lightmaps with realtime shadows
								  #ifdef SHADOWS_SCREEN
									#if defined(UNITY_NO_RGBM)
									c.rgb += o.Albedo * min(lm, atten * 2);
									#else
									c.rgb += o.Albedo * max(min(lm,(atten * 2)*lmtex.rgb), lm*atten);
									#endif
								  #else // SHADOWS_SCREEN
									c.rgb += o.Albedo * lm;
								  #endif // SHADOWS_SCREEN
								#endif // LIGHTMAP_ON

								#ifdef DYNAMICLIGHTMAP_ON
								fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, IN.lmap.zw);
								c.rgb += o.Albedo * DecodeRealtimeLightmap(dynlmtex);
								#endif

								c.rgb += o.Emission;
								UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
								return c;
							  }
							  #endif
							  ENDCG
							  }
		}
}

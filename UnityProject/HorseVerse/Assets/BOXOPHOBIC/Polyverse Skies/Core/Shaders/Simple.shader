// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Polyverse Skies/Simple"
{
	Properties
	{
		[StyledBanner(Polyverse Skies Simple)]_Banner("< Banner >", Float) = 1
		[StyledCategory(Background Settings, 5, 10)]_BackgroundCat("[ Background Cat ]", Float) = 1
		[KeywordEnum(Colors,Cubemap,Combined)] _BackgroundMode("Background Mode", Float) = 0
		[Space(10)]_SkyColor("Sky Color", Color) = (0.4980392,0.7450981,1,1)
		_EquatorColor("Equator Color", Color) = (1,0.747,0,1)
		_GroundColor("Ground Color", Color) = (0.4980392,0.497,0,1)
		_EquatorHeight("Equator Height", Range( 0 , 1)) = 0.5
		_EquatorSmoothness("Equator Smoothness", Range( 0.01 , 1)) = 0.5
		[NoScaleOffset][Space(10)][StyledTextureSingleLine]_BackgroundCubemap("Background Cubemap", CUBE) = "black" {}
		[Space(10)]_BackgroundExposure("Background Exposure", Range( 0 , 8)) = 1
		[StyledCategory(Stars Settings)]_StarsCat("[ Stars Cat ]", Float) = 1
		[Toggle(_ENABLESTARS_ON)] _EnableStars("Enable Stars", Float) = 0
		[NoScaleOffset][Space(10)][StyledTextureSingleLine]_StarsCubemap("Stars Cubemap", CUBE) = "white" {}
		[Space(10)]_StarsSize("Stars Size", Range( 0 , 0.99)) = 0.5
		_StarsIntensity("Stars Intensity", Range( 0 , 5)) = 2
		_StarsHeightMask("Stars Height Mask", Range( 0 , 1)) = 0
		[StyledToggle]_StarsBottomMask("Stars Bottom Mask", Float) = 0
		[StyledCategory(Sun Settings)]_SunCat("[ Sun Cat ]", Float) = 1
		[Toggle(_ENABLESUN_ON)] _EnableSun("Enable Sun", Float) = 0
		[NoScaleOffset][Space(10)][StyledTextureSingleLine]_SunTexture("Sun Texture", 2D) = "black" {}
		[Space(10)]_SunColor("Sun Color", Color) = (1,1,1,1)
		_SunSize("Sun Size", Range( 0.1 , 1)) = 0.5
		_SunIntensity("Sun Intensity", Range( 0 , 10)) = 1
		[StyledCategory(Clouds Settings)]_CloudsCat("[ Clouds Cat ]", Float) = 1
		[Toggle(_ENABLECLOUDS_ON)] _EnableClouds("Enable Clouds", Float) = 0
		[NoScaleOffset][Space(10)][StyledTextureSingleLine]_CloudsCubemap("Clouds Cubemap", CUBE) = "black" {}
		[Space(10)]_CloudsIntensity("Clouds Intensity", Range( 0 , 1)) = 1
		_CloudsHeight("Clouds Height", Range( -0.5 , 0.5)) = 0
		_CloudsLightColor("Clouds Light Color", Color) = (1,1,1,1)
		_CloudsShadowColor("Clouds Shadow Color", Color) = (0.4980392,0.7450981,1,1)
		[StyledCategory(Fog Settings)]_FogCat("[ Fog Cat ]", Float) = 1
		[Toggle(_ENABLEBUILTINFOG_ON)] _EnableBuiltinFog("Enable Fog", Float) = 0
		[StyledMessage(Info, The fog color is controlled by the fog color set in the Lighting panel., _EnableBuiltinFog, 1, 10, 0)]_EnableFogMessage("EnableFogMessage", Float) = 0
		[Space(10)]_FogHeight("Fog Height", Range( 0 , 1)) = 0
		_FogSmoothness("Fog Smoothness", Range( 0.01 , 1)) = 0
		_FogFill("Fog Fill", Range( 0 , 1)) = 0
		[StyledCategory(Skybox Settings)]_SkyboxCat("[ Skybox Cat ]", Float) = 1
		_SkyboxOffset("Skybox Offset", Range( -1 , 1)) = 0
		_SkyboxRotation("Skybox Rotation", Range( 0 , 1)) = 0
		[ASEEnd]_SkyboxRotationAxix("Skybox Rotation Axix", Vector) = (0,1,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Background" "Queue"="Background" "PreviewType"="Skybox" }
	LOD 100

		CGINCLUDE
		#pragma target 2.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _ENABLEBUILTINFOG_ON
			#pragma shader_feature_local _ENABLECLOUDS_ON
			#pragma shader_feature_local _ENABLESUN_ON
			#pragma shader_feature_local _ENABLESTARS_ON
			#pragma shader_feature_local _BACKGROUNDMODE_COLORS _BACKGROUNDMODE_CUBEMAP _BACKGROUNDMODE_COMBINED


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _FogCat;
			uniform half _Banner;
			uniform half _SunCat;
			uniform half _SkyboxCat;
			uniform half _StarsCat;
			uniform float _EnableFogMessage;
			uniform half _BackgroundCat;
			uniform half _CloudsCat;
			uniform half4 _EquatorColor;
			uniform half4 _GroundColor;
			uniform half4 _SkyColor;
			uniform float3 _SkyboxRotationAxix;
			uniform float _SkyboxRotation;
			uniform float _SkyboxOffset;
			uniform half _EquatorHeight;
			uniform half _EquatorSmoothness;
			uniform samplerCUBE _BackgroundCubemap;
			uniform half _BackgroundExposure;
			uniform half _StarsHeightMask;
			uniform samplerCUBE _StarsCubemap;
			uniform half _StarsSize;
			uniform half _StarsIntensity;
			uniform half _StarsBottomMask;
			uniform sampler2D _SunTexture;
			uniform half3 GlobalSunDirection;
			uniform half _SunSize;
			uniform half4 _SunColor;
			uniform half _SunIntensity;
			uniform half4 _CloudsShadowColor;
			uniform half4 _CloudsLightColor;
			uniform samplerCUBE _CloudsCubemap;
			uniform half _CloudsHeight;
			uniform half _CloudsIntensity;
			uniform half _FogHeight;
			uniform half _FogSmoothness;
			uniform half _FogFill;
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 appendResult1239 = (float3(0.0 , -_SkyboxOffset , 0.0));
				float3 rotatedValue1215 = RotateAroundAxis( appendResult1239, ( v.vertex.xyz + appendResult1239 ), normalize( _SkyboxRotationAxix ), ( _SkyboxRotation * ( 2.0 * UNITY_PI ) ) );
				float3 normalizeResult1241 = normalize( rotatedValue1215 );
				float3 vertexToFrag1216 = normalizeResult1241;
				o.ase_texcoord1.xyz = vertexToFrag1216;
				#ifdef _ENABLESTARS_ON
				float staticSwitch1166 = saturate( (0.1 + (abs( v.vertex.xyz.y ) - 0.0) * (1.0 - 0.1) / (_StarsHeightMask - 0.0)) );
				#else
				float staticSwitch1166 = 0.0;
				#endif
				float vertexToFrag856 = staticSwitch1166;
				o.ase_texcoord1.w = vertexToFrag856;
				half3 VertexPos1217 = vertexToFrag1216;
				float3 break1223 = VertexPos1217;
				float lerpResult268 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CAMERA_MODE300 = lerpResult268;
				float temp_output_673_0 = ( break1223.y * CAMERA_MODE300 );
				float3 appendResult675 = (float3(break1223.x , temp_output_673_0 , break1223.z));
				#ifdef _ENABLESTARS_ON
				float3 staticSwitch1165 = appendResult675;
				#else
				float3 staticSwitch1165 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag763 = staticSwitch1165;
				o.ase_texcoord2.xyz = vertexToFrag763;
				float3 temp_output_962_0 = cross( GlobalSunDirection , half3(0,1,0) );
				float3 normalizeResult967 = normalize( temp_output_962_0 );
				float dotResult968 = dot( normalizeResult967 , v.vertex.xyz );
				half3 GlobalSunDirection1005 = GlobalSunDirection;
				float3 normalizeResult965 = normalize( cross( GlobalSunDirection1005 , temp_output_962_0 ) );
				float dotResult969 = dot( normalizeResult965 , v.vertex.xyz );
				float2 appendResult970 = (float2(dotResult968 , dotResult969));
				float2 break972 = appendResult970;
				float2 appendResult980 = (float2(break972.x , ( break972.y * CAMERA_MODE300 )));
				float lerpResult1246 = lerp( 20.0 , 2.0 , _SunSize);
				#ifdef _ENABLESUN_ON
				float2 staticSwitch1168 = (( appendResult980 * lerpResult1246 )*0.5 + 0.5);
				#else
				float2 staticSwitch1168 = float2( 0,0 );
				#endif
				float2 vertexToFrag993 = staticSwitch1168;
				o.ase_texcoord3.xy = vertexToFrag993;
				float dotResult988 = dot( GlobalSunDirection1005 , v.vertex.xyz );
				#ifdef _ENABLESUN_ON
				float staticSwitch1169 = saturate( dotResult988 );
				#else
				float staticSwitch1169 = 0.0;
				#endif
				float vertexToFrag997 = staticSwitch1169;
				o.ase_texcoord2.w = vertexToFrag997;
				float3 break1225 = VertexPos1217;
				float3 appendResult246 = (float3(break1225.x , ( ( break1225.y + ( _CloudsHeight * -1.0 ) ) * CAMERA_MODE300 ) , break1225.z));
				#ifdef _ENABLECLOUDS_ON
				float3 staticSwitch1163 = appendResult246;
				#else
				float3 staticSwitch1163 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag1133 = staticSwitch1163;
				o.ase_texcoord4.xyz = vertexToFrag1133;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord4.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float3 vertexToFrag1216 = i.ase_texcoord1.xyz;
				half3 VertexPos1217 = vertexToFrag1216;
				float4 lerpResult180 = lerp( _GroundColor , _SkyColor , step( 0.0 , VertexPos1217.y ));
				float temp_output_7_0_g1 = 0.0;
				float saferPower470 = max( saturate( ( ( abs( VertexPos1217.y ) - temp_output_7_0_g1 ) / ( _EquatorHeight - temp_output_7_0_g1 ) ) ) , 0.0001 );
				float4 lerpResult288 = lerp( _EquatorColor , lerpResult180 , pow( saferPower470 , ( 1.0 - _EquatorSmoothness ) ));
				half4 SKY218 = lerpResult288;
				half4 BACKGROUND1202 = ( texCUBE( _BackgroundCubemap, VertexPos1217 ) * _BackgroundExposure );
				#if defined(_BACKGROUNDMODE_COLORS)
				float4 staticSwitch1207 = SKY218;
				#elif defined(_BACKGROUNDMODE_CUBEMAP)
				float4 staticSwitch1207 = BACKGROUND1202;
				#elif defined(_BACKGROUNDMODE_COMBINED)
				float4 staticSwitch1207 = ( SKY218 * BACKGROUND1202 );
				#else
				float4 staticSwitch1207 = SKY218;
				#endif
				float vertexToFrag856 = i.ase_texcoord1.w;
				float3 vertexToFrag763 = i.ase_texcoord2.xyz;
				float3 break1223 = VertexPos1217;
				float lerpResult268 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CAMERA_MODE300 = lerpResult268;
				float temp_output_673_0 = ( break1223.y * CAMERA_MODE300 );
				half Starts_Bottom_Mask1230 = step( 0.0 , temp_output_673_0 );
				float lerpResult1234 = lerp( 1.0 , saturate( Starts_Bottom_Mask1230 ) , _StarsBottomMask);
				half STARS630 = ( floor( ( vertexToFrag856 * ( texCUBE( _StarsCubemap, vertexToFrag763 ).g + _StarsSize ) ) ) * _StarsIntensity * lerpResult1234 );
				#ifdef _ENABLESTARS_ON
				float4 staticSwitch1170 = ( staticSwitch1207 + STARS630 );
				#else
				float4 staticSwitch1170 = staticSwitch1207;
				#endif
				float2 vertexToFrag993 = i.ase_texcoord3.xy;
				float4 tex2DNode995 = tex2D( _SunTexture, vertexToFrag993 );
				half4 SUN1004 = ( tex2DNode995.r * _SunColor * max( _SunIntensity , 1.0 ) );
				float vertexToFrag997 = i.ase_texcoord2.w;
				half SUN_MASK1003 = ( tex2DNode995.a * saturate( _SunIntensity ) * vertexToFrag997 );
				float4 lerpResult176 = lerp( staticSwitch1170 , SUN1004 , SUN_MASK1003);
				#ifdef _ENABLESUN_ON
				float4 staticSwitch1167 = lerpResult176;
				#else
				float4 staticSwitch1167 = staticSwitch1170;
				#endif
				float3 vertexToFrag1133 = i.ase_texcoord4.xyz;
				float4 texCUBENode41 = texCUBE( _CloudsCubemap, vertexToFrag1133 );
				float4 lerpResult101 = lerp( _CloudsShadowColor , _CloudsLightColor , texCUBENode41.g);
				half4 CLOUDS222 = lerpResult101;
				half CLOUDS_MASK223 = ( texCUBENode41.a * _CloudsIntensity );
				float4 lerpResult227 = lerp( staticSwitch1167 , CLOUDS222 , CLOUDS_MASK223);
				#ifdef _ENABLECLOUDS_ON
				float4 staticSwitch1162 = lerpResult227;
				#else
				float4 staticSwitch1162 = staticSwitch1167;
				#endif
				float temp_output_7_0_g2 = 0.0;
				float lerpResult678 = lerp( saturate( pow( ( ( abs( VertexPos1217.y ) - temp_output_7_0_g2 ) / ( _FogHeight - temp_output_7_0_g2 ) ) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogFill);
				half FOG_MASK359 = lerpResult678;
				float4 lerpResult317 = lerp( unity_FogColor , staticSwitch1162 , FOG_MASK359);
				#ifdef _ENABLEBUILTINFOG_ON
				float4 staticSwitch921 = lerpResult317;
				#else
				float4 staticSwitch921 = staticSwitch1162;
				#endif
				
				
				finalColor = staticSwitch921;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "PolyverseSkiesShaderGUI"
	
	Fallback "False"
}
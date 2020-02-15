//RealToon V5.0.5 (URP)
//MJQStudioWorks
//2019

Shader "Universal Render Pipeline/RealToon/Version 5/Default/Fade Transparency"
{
    Properties
    {

		[Enum(Off,2,On,0)] _DoubleSided("Double Sided", int) = 2

        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _TexturePatternStyle ("Texture Pattern Style", Float ) = 0
        [HDR] _MainColor ("Main Color", Color) = (0.2156863,0.2156863,0.2156863,1)

		[Toggle] _MVCOL ("Mix Vertex Color", Float ) = 0

		[Toggle] _MCIALO ("Main Color In Ambient Light Only", Float ) = 0

		[HDR] _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _HighlightColorPower ("Highlight Color Power", Float ) = 1

		_MCapIntensity ("Intensity", Range(0, 1)) = 1
		_MCap ("MatCap", 2D) = "white" {}
		_MCapMask ("Mask MatCap", 2D) = "white" {}

        _Opacity ("Opacity", Range(0, 1)) = 1
		_TransparentThreshold ("Transparent Threshold", Float ) = 0
        _MaskTransparency ("Mask Transparency", 2D) = "black" {}

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapIntensity ("Normal Map Intensity", Float ) = 1

        _Saturation ("Saturation", Range(0, 2)) = 1

        _OutlineWidth ("Width", Float ) = 0.5
        _OutlineWidthControl ("Width Control", 2D) = "white" {}

		[Enum(Normal,0,Origin,1)] _OutlineExtrudeMethod("Outline Extrude Method", int) = 0

		_ReduceOutlineBackFace ("Reduce Outline Backface", Range(0, 1)) = 0

		_OutlineOffset ("Outline Offset", Vector) = (0,0,0)
		_OutlineZPostionInCamera ("Outline Z Position In Camera", Float) = 0

		[Enum(Off,1,On,0)] _DoubleSidedOutline("Double Sided Outline", int) = 1

        [HDR] _OutlineColor ("Color", Color) = (0,0,0,1)

		[Toggle] _MixMainTexToOutline ("Mix Main Texture To Outline", Float ) = 0

        _NoisyOutlineIntensity ("Noisy Outline Intensity", Range(0, 1)) = 0
        [Toggle] _DynamicNoisyOutline ("Dynamic Noisy Outline", Float ) = 0
        [Toggle] _LightAffectOutlineColor ("Light Affect Outline Color", Float ) = 0

        [Toggle] _OutlineWidthAffectedByViewDistance ("Outline Width Affected By View Distance", Float ) = 0
		_FarDistanceMaxWidth ("Far Distance Max Width", Float ) = 10

        [Toggle] _VertexColorRedAffectOutlineWitdh ("Vertex Color Red Affect Outline Witdh", Float ) = 0

        _SelfLitIntensity ("Intensity", Range(0, 1)) = 0
        [HDR] _SelfLitColor ("Color", Color) = (1,1,1,1)
        _SelfLitPower ("Power", Float ) = 2
        [Toggle] _SelfLitHighContrast ("High Contrast", Float ) = 1
        _MaskSelfLit ("Mask Self Lit", 2D) = "white" {}

        _GlossIntensity ("Intensity", Range(0, 1)) = 1
        _Glossiness ("Glossiness", Range(0, 1)) = 0.6
        _GlossSoftness ("Softness", Range(0, 1)) = 0
        [HDR] _GlossColor ("Color", Color) = (1,1,1,1)
        _GlossColorPower ("Color Power", Float ) = 10
        _MaskGloss ("Mask Gloss", 2D) = "white" {}

        _GlossTexture ("Gloss Texture", 2D) = "black" {}
        _GlossTextureSoftness ("Softness", Float ) = 0
        _GlossTextureRotate ("Rotate", Float ) = 0
        [Toggle] _GlossTextureFollowObjectRotation ("Follow Object Rotation", Float ) = 0
        _GlossTextureFollowLight ("Follow Light", Range(0, 1)) = 0

        [HDR] _OverallShadowColor ("Overall Shadow Color", Color) = (0,0,0,1)
        _OverallShadowColorPower ("Overall Shadow Color Power", Float ) = 1

        [Toggle] _SelfShadowShadowTAtViewDirection ("Self Shadow & ShadowT At View Direction", Float ) = 0

        _SelfShadowIntensity ("Self Shadow Intensity", Range(0, 1)) = 1
        _SelfShadowThreshold ("Threshold", Range(0, 1)) = 0.85
        [Toggle] _VertexColorGreenControlSelfShadowThreshold ("Vertex Color Green Control Self Shadow Threshold", Float ) = 0
        _SelfShadowHardness ("Hardness", Range(0, 1)) = 1
        [HDR] _SelfShadowColor ("Color", Color) = (1,1,1,1)
        _SelfShadowColorPower ("Color Power", Float ) = 1
        [Toggle] _SelfShadowAffectedByLightShadowStrength ("Self Shadow Affected By Light Shadow Strength", Float ) = 0

        _SmoothObjectNormal ("Smooth Object Normal", Range(0, 1)) = 0
        [Toggle] _VertexColorBlueControlSmoothObjectNormal ("Vertex Color Blue Control Smooth Object Normal", Float ) = 0
        _XYZPosition ("XYZ Position", Vector) = (0,0,0,0)
        _XYZHardness ("XYZ Hardness", Float ) = 14
        [Toggle] _ShowNormal ("Show Normal", Float ) = 0

        _ShadowColorTexture ("Shadow Color Texture", 2D) = "white" {}
        _ShadowColorTexturePower ("Shadow Color Texture (Power)", Float ) = 0

        _ShadowTIntensity ("Intensity", Range(0, 1)) = 1
        _ShadowT ("ShadowT", 2D) = "white" {}
        _ShadowTLightThreshold ("Light Threshold", Float ) = 50
        _ShadowTShadowThreshold ("Shadow Threshold", Float ) = 0
        [HDR] _ShadowTColor ("Color", Color) = (1,1,1,1)
        _ShadowTColorPower ("Color Power", Float ) = 1
        _ShadowTHardness ("Hardness", Range(0, 1)) = 1

		[Toggle] _STIL ("Ignore Light", Float ) = 0

		[Toggle(N_F_STIS_ON)] _N_F_STIS ("Show In Shadow", Float ) = 0

		[Toggle(N_F_STIAL_ON)] _N_F_STIAL ("Show In Ambient Light", Float ) = 0
        _ShowInAmbientLightShadowIntensity ("Show In Ambient Light Intensity", Range(0, 1)) = 1
        _ShowInAmbientLightShadowThreshold ("Show In Ambient Light & Shadow Threshold", Float ) = 0.4

        [Toggle] _LightFalloffAffectShadowT ("Light Falloff Affect ShadowT", Float ) = 0

        _PTexture ("PTexture", 2D) = "white" {}
        _PTexturePower ("Power", Float ) = 1

        [Toggle] _GIFlatShade ("GI Flat Shade", Float ) = 0
        _GIShadeThreshold ("GI Shade Threshold", Range(0, 1)) = 0

        [Toggle] _LightAffectShadow ("Light Affect Shadow", Float ) = 0
        _LightIntensity ("Light Intensity", Float ) = -1

        _EnvironmentalLightingIntensity ("Environmental Lighting Intensity", Float ) = 1

		[Toggle] _UseTLB ("Use Traditional Light Blend", Float ) = 0
		[Toggle(N_F_EAL_ON)] _N_F_EAL ("Enable Additional Lights", Float ) = 1

		_DirectionalLightIntensity ("Directional Light Intensity", Float ) = 0
		_PointSpotlightIntensity ("Point and Spot Light Intensity", Float ) = 0.3
		_LightFalloffSoftness ("Light Falloff Softness", Range(0, 1)) = 1

        _CustomLightDirectionIntensity ("Intensity", Range(0, 1)) = 0
        [Toggle] _CustomLightDirectionFollowObjectRotation ("Follow Object Rotation", Float ) = 0
        _CustomLightDirection ("Custom Light Direction", Vector) = (0,0,10,0)

        _ReflectionIntensity ("Intensity", Range(0, 1)) = 0
        _ReflectionRoughtness ("Roughtness", Float ) = 0
		_RefMetallic ("Metallic", Range(0,1) ) = 0

        _MaskReflection ("Mask Reflection", 2D) = "white" {}

        [Toggle] _UseFReflection ("Use FReflection", Float ) = 0
        _FReflection ("FReflection", 2D) = "black" {}

        _RimLightUnfill ("Unfill", Float ) = 1.5
        [HDR] _RimLightColor ("Color", Color) = (1,1,1,1)
        _RimLightColorPower ("Color Power", Float ) = 10
        _RimLightSoftness ("Softness", Range(0, 1)) = 1
        [Toggle] _RimLightInLight ("Rim Light In Light", Float ) = 1
        [Toggle] _LightAffectRimLightColor ("Light Affect Rim Light Color", Float ) = 0

		_RefVal ("ID", int ) = 0
        [Enum(None,8,A,0,B,2)] _Oper("Set 1", int) = 8
        [Enum(None,8,A,6,B,7)] _Compa("Set 2", int) = 8

		[Toggle(N_F_MC_ON)] _N_F_MC ("MatCap", Float ) = 0
		[Toggle(N_F_NM_ON)] _N_F_NM ("Normal Map", Float ) = 0
		[Toggle(N_F_O_ON)] _N_F_O ("Outline", Float ) = 0
		[Toggle(N_F_CA_ON)] _N_F_CA ("Color Adjustment", Float ) = 0
		[Toggle(N_F_SL_ON)] _N_F_SL ("Self Lit", Float ) = 0
		[Toggle(N_F_GLO_ON)] _N_F_GLO ("Gloss", Float ) = 0
		[Toggle(N_F_GLOT_ON)] _N_F_GLOT ("Gloss Texture", Float ) = 0
		[Toggle(N_F_SS_ON)] _N_F_SS ("Self Shadow", Float ) = 1
		[Toggle(N_F_SON_ON)] _N_F_SON ("Smooth Object Normal", Float ) = 0
		[Toggle(N_F_SCT_ON)] _N_F_SCT ("Shadow Color Texture", Float ) = 0
		[Toggle(N_F_ST_ON)] _N_F_ST ("ShadowT", Float ) = 0
		[Toggle(N_F_PT_ON)] _N_F_PT ("PTexture", Float ) = 0
		[Toggle(N_F_CLD_ON)] _N_F_CLD ("Custom Light Direction", Float ) = 0
		[Toggle(N_F_R_ON)] _N_F_R ("Reflection", Float ) = 0
		[Toggle(N_F_FR_ON)] _N_F_FR ("FReflection", Float ) = 0
		[Toggle(N_F_RL_ON)] _N_F_RL ("Rim Light", Float ) = 0
		[Enum(On,1,Off,0)] _ZWrite("ZWrite", int) = 0

		[Toggle(N_F_DCS_ON)] _N_F_DCS ("Disable Cast Shadow", Float ) = 0
		[Toggle(N_F_NLASOBF_ON)] _N_F_NLASOBF ("No Light On BackFace", Float ) = 0
    }

    SubShader
    {

        Tags{"Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 300

		Pass {
            Name "Outline"
            Tags {
            }
            Cull [_DoubleSidedOutline]

			Stencil {
            	Ref[_RefVal]
            	Comp [_Compa]
            	Pass [_Oper]
            	Fail [_Oper]
            }

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 wiiu switch
            #pragma target 3.0

			#pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma shader_feature_local N_F_EAL_ON
			#pragma shader_feature_local N_F_O_ON

#if N_F_O_ON

			CBUFFER_START(UnityPerMaterial)

				uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
				uniform sampler2D _MaskTransparency; uniform float4 _MaskTransparency_ST;
				uniform half _Opacity;
				uniform half _TexturePatternStyle;

				uniform half _OutlineWidth;
				uniform sampler2D _OutlineWidthControl; uniform float4 _OutlineWidthControl_ST;
				uniform float3 _OEM;
				uniform int _OutlineExtrudeMethod;
				uniform half3 _OutlineOffset;
				uniform half _OutlineZPostionInCamera;
				uniform half4 _OutlineColor;
				uniform half _MixMainTexToOutline;
				uniform half _NoisyOutlineIntensity;
				uniform half _DynamicNoisyOutline;
				uniform half _LightAffectOutlineColor;
				uniform half _OutlineWidthAffectedByViewDistance;
				uniform half _FarDistanceMaxWidth;
				uniform half _VertexColorRedAffectOutlineWitdh;

				uniform half _ReduceOutlineBackFace;
				uniform half _TransparentThreshold;

			CBUFFER_END

#endif

			struct Attributes
            {
#if N_F_O_ON
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
				float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
				float4 vertexColor	: COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
#endif
            };

            struct Varyings
            {
#if N_F_O_ON
                float2 uv                       : TEXCOORD0;
				half3  normalWS                 : TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; 
				float4 posWorld					: TEXCOORD3;
				float4 projPos					: TEXCOORD7;
				float4 vertexColor				: COLOR;
                float4 positionCS               : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
#endif
            };


			Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;

#if N_F_O_ON

				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.uv = input.uv;
                output.vertexColor = input.vertexColor;

				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

				output.normalWS = vertexNormalInput.normalWS;
                output.posWorld = mul(unity_ObjectToWorld, input.positionOS);


                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);

				half RTD_OL_VCRAOW_OO = lerp( _OutlineWidth, (_OutlineWidth*(1.0 - output.vertexColor.r)), _VertexColorRedAffectOutlineWitdh );
				half RTD_OL_OLWABVD_OO = lerp( RTD_OL_VCRAOW_OO, ( clamp(RTD_OL_VCRAOW_OO*RTD_OB_VP_CAL, RTD_OL_VCRAOW_OO, _FarDistanceMaxWidth) ), _OutlineWidthAffectedByViewDistance );

                half4 _OutlineWidthControl_var = tex2Dlod(_OutlineWidthControl,float4(TRANSFORM_TEX(output.uv, _OutlineWidthControl),0.0,0));

                float4 node_427 = _Time;
                float node_4658_ang = node_427.g;
                float node_4658_spd = 0.002;
                float node_4658_cos = cos(node_4658_spd*node_4658_ang);
                float node_4658_sin = sin(node_4658_spd*node_4658_ang);
                float2 node_4658_piv = float2(0.5,0.5);
                half2 node_4658 = (mul(output.uv-node_4658_piv,float2x2( node_4658_cos, -node_4658_sin, node_4658_sin, node_4658_cos))+node_4658_piv);

				half2 RTD_OL_DNOL_OO = lerp( output.uv, node_4658, _DynamicNoisyOutline );

                float2 node_9863_skew = RTD_OL_DNOL_OO + 0.2127+RTD_OL_DNOL_OO.x*0.3713*RTD_OL_DNOL_OO.y;
                float2 node_9863_rnd = 4.789*sin(489.123*(node_9863_skew));
                half node_9863 = frac(node_9863_rnd.x*node_9863_rnd.y*(1+node_9863_skew.x));

				_OEM = lerp(input.normalOS.xyz, normalize(input.positionOS.xyz), _OutlineExtrudeMethod );

				half RTD_OL = ( RTD_OL_OLWABVD_OO*0.01 )*_OutlineWidthControl_var.r*lerp(1.0,node_9863,_NoisyOutlineIntensity);
                output.positionCS = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4( (input.positionOS.xyz + _OutlineOffset.xyz * 0.01) + _OEM * RTD_OL,1) ) );

				#if defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
					output.positionCS.z += _OutlineZPostionInCamera * 0.0005;
				#else
					output.positionCS.z -= _OutlineZPostionInCamera * 0.0005;
				#endif
				
                output.projPos = ComputeScreenPos (output.positionCS);
				float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
				output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);

#endif

                return output;

            }

            half4 LitPassFragment(Varyings input) : SV_Target
            {

#if N_F_O_ON

				UNITY_SETUP_INSTANCE_ID (input);

                float3 positionWS = input.positionWSAndFogFactor.xyz;

                Light mainLight = GetMainLight();
				half3 color = 1;

				float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float2 sceneUVs = (input.projPos.xy / input.projPos.w);

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);
				half2 RTD_VD_Cal = (float2((sceneUVs.x * 2 - 1)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2 - 1).rg*RTD_OB_VP_CAL);

				input.normalWS = normalize(input.normalWS);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
				float3 normalDirection = input.normalWS;

				half2 RTD_TC_TP_OO = lerp( input.uv, RTD_VD_Cal, _TexturePatternStyle );
                half2 node_4408 = RTD_TC_TP_OO;

                half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4408, _MainTex));

                half4 _MaskTransparency_var = tex2D(_MaskTransparency,TRANSFORM_TEX(input.uv, _MaskTransparency));

				half RTD_TRAN_MAS = (smoothstep(clamp(-20,1,_TransparentThreshold),1,_MainTex_var.a) *_MaskTransparency_var.r);
				half RTD_TRAN_OPA_Sli = lerp( RTD_TRAN_MAS, smoothstep(clamp(-20,1,_TransparentThreshold) , 1, _MainTex_var.a)  ,_Opacity);
				half RTD_TRAN = saturate(( 0.74 > 0.5 ? (1.0-(1.0-2.0*(0.74-0.5))*(1.0-RTD_TRAN_OPA_Sli)) : (2.0*0.74*RTD_TRAN_OPA_Sli) ));
				clip(RTD_TRAN - 0.5);

				float oc = (1.0-dot(normalDirection, viewDirection));
                clip(smoothstep( lerp(4,1.7,_ReduceOutlineBackFace), 0, oc ) - 0.5);
				
				float3 lightColor =  mainLight.color.rgb;

#ifdef _ADDITIONAL_LIGHTS

#if N_F_EAL_ON

                int additionalLightsCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light light = GetAdditionalLight(i, positionWS);
					lightColor += light.color * light.distanceAttenuation;
				}
#endif

#endif
                float fogFactor = input.positionWSAndFogFactor.w;

                float node_3413 = 0.0;
				half3 RTD_OL_LAOC_OO = lerp( lerp(_OutlineColor.rgb,_OutlineColor.rgb * _MainTex_var.rgb, _MixMainTexToOutline) , lerp(float3(node_3413,node_3413,node_3413), lerp(_OutlineColor.rgb,_OutlineColor.rgb * _MainTex_var.rgb, _MixMainTexToOutline) ,lightColor.rgb), _LightAffectOutlineColor );

				half3 finalRGBA = RTD_OL_LAOC_OO;

                color = MixFog(finalRGBA, fogFactor);
                return half4(color, 1);

#else

				return 0;

#endif

            }

			ENDHLSL
        }

        Pass
        {

            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Cull [_DoubleSided]
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite [_ZWrite]

			Stencil {
            	Ref[_RefVal]
            	Comp [_Compa]
            	Pass [_Oper]
            	Fail [_Oper]
            }

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 wiiu switch
            #pragma target 3.0

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma shader_feature_local _USETLB_ON

			#pragma shader_feature_local N_F_MC_ON
			#pragma shader_feature_local N_F_NM_ON
			#pragma shader_feature_local N_F_SL_ON
			#pragma shader_feature_local N_F_CA_ON
			#pragma shader_feature_local N_F_GLO_ON
			#pragma shader_feature_local N_F_GLOT_ON
			#pragma shader_feature_local N_F_SS_ON
			#pragma shader_feature_local N_F_SCT_ON
			#pragma shader_feature_local N_F_ST_ON
			#pragma shader_feature_local N_F_STIS_ON
			#pragma shader_feature_local N_F_STIAL_ON 
			#pragma shader_feature_local N_F_SON_ON
			#pragma shader_feature_local N_F_PT_ON
			#pragma shader_feature_local N_F_CLD_ON
			#pragma shader_feature_local N_F_R_ON
			#pragma shader_feature_local N_F_FR_ON
			#pragma shader_feature_local N_F_RL_ON
			#pragma shader_feature_local N_F_EAL_ON
			#pragma shader_feature_local N_F_NLASOBF_ON

			CBUFFER_START(UnityPerMaterial)
			
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform half4 _MainColor;
			uniform half _MVCOL;
			uniform half _MCIALO;
			uniform half _TexturePatternStyle;
			uniform half4 _HighlightColor;
			uniform half _HighlightColorPower;

			#if N_F_MC_ON

			uniform sampler2D _MCap; uniform float4 _MCap_ST;
			uniform sampler2D _MCapMask; uniform float4 _MCapMask_ST;
			uniform half _MCapIntensity;

			#endif

            uniform sampler2D _MaskTransparency; uniform float4 _MaskTransparency_ST;
            uniform half _Opacity;
			uniform half _TransparentThreshold;

			#if N_F_NM_ON
				uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
				uniform half _NormalMapIntensity;
			#endif

			#if N_F_CA_ON
				uniform half _Saturation;
			#endif

			#if N_F_SL_ON
				uniform half _SelfLitIntensity;
				uniform half4 _SelfLitColor;
				uniform half _SelfLitPower;
				uniform half _SelfLitHighContrast;
				uniform sampler2D _MaskSelfLit; uniform float4 _MaskSelfLit_ST;
			#endif

			#if N_F_GLO_ON
				uniform half _GlossIntensity;
				uniform half _Glossiness;
				uniform half _GlossSoftness;
				uniform half4 _GlossColor;
				uniform half _GlossColorPower;
				uniform sampler2D _MaskGloss; uniform float4 _MaskGloss_ST;
			#endif

			#if N_F_GLO_ON
				#if N_F_GLOT_ON
					uniform sampler2D _GlossTexture; uniform float4 _GlossTexture_ST;
					uniform half _GlossTextureSoftness;
					uniform half _GlossTextureRotate;
					uniform half _GlossTextureFollowObjectRotation;
					uniform half _GlossTextureFollowLight;
				#endif
			#endif

			uniform half4 _OverallShadowColor;
            uniform half _OverallShadowColorPower;

			uniform half _SelfShadowShadowTAtViewDirection;

			#if N_F_SS_ON
				uniform half _SelfShadowThreshold;
				uniform half _VertexColorGreenControlSelfShadowSize;
				uniform half _SelfShadowHardness;
				uniform half _SelfShadowAffectedByLightShadowStrength;
			#endif

			uniform half _SelfShadowIntensity;
			uniform half4 _SelfShadowColor;
			uniform half _SelfShadowColorPower;

			#if N_F_SON_ON
				uniform half _SmoothObjectNormal;
				uniform half _VertexColorBlueControlSmoothObjectNormal;
				uniform float4 _XYZPosition;
				uniform half _XYZHardness;
				uniform half _ShowNormal;
			#endif

			#if N_F_SCT_ON
				uniform sampler2D _ShadowColorTexture; uniform float4 _ShadowColorTexture_ST;
				uniform half _ShadowColorTexturePower;
			#endif

			#if N_F_ST_ON
				uniform half _ShadowTIntensity;
				uniform sampler2D _ShadowT; uniform float4 _ShadowT_ST;
				uniform half _ShadowTLightThreshold;
				uniform half _ShadowTShadowThreshold;
				uniform half4 _ShadowTColor;
				uniform half _ShadowTColorPower;
				uniform half _ShadowTHardness;
				uniform half _STIL;
				uniform half _ShowInAmbientLightShadowIntensity;
				uniform half _ShowInAmbientLightShadowThreshold;
				uniform half _LightFalloffAffectShadowT;
			#endif

			#if N_F_PT_ON
				uniform sampler2D _PTexture; uniform float4 _PTexture_ST;
				uniform half _PTexturePower;
			#endif

			uniform half _GIFlatShade;
			uniform half _GIShadeThreshold;
			uniform half _LightAffectShadow;
			uniform half _LightIntensity;
            uniform half _EnvironmentalLightingIntensity;
			uniform half _DirectionalLightIntensity;
			uniform half _PointSpotlightIntensity;
			uniform half _LightFalloffSoftness;

			#if N_F_CLD_ON
				uniform half _CustomLightDirectionIntensity;
				uniform half4 _CustomLightDirection;
				uniform half _CustomLightDirectionFollowObjectRotation;
			#endif

			#if N_F_R_ON
				uniform half _ReflectionIntensity;
				uniform half _ReflectionRoughtness;
				uniform half _RefMetallic;
				uniform sampler2D _MaskReflection; uniform float4 _MaskReflection_ST;
			#endif

			#if N_F_R_ON
				#if N_F_FR_ON
					uniform sampler2D _FReflection; uniform float4 _FReflection_ST;
				#endif
			#endif

			#if N_F_RL_ON
				uniform half _RimLightUnfill;
				uniform half _RimLightSoftness;	
				uniform half _LightAffectRimLightColor;
				uniform half4 _RimLightColor;
				uniform half _RimLightColorPower;
				uniform half _RimLightInLight;
			#endif

			CBUFFER_END
            
			half3 AL_GI( float3 N )
			{

				return SampleSH(N);

            }
            
			float3 Ref( half3 VR , half Mip )
			{

				float4 skyData = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, VR, Mip);
				return DecodeHDREnvironment(skyData, unity_SpecCube0_HDR);

            }

			half RTD_LVLC_F( float3 Light_Color_f3 )
			{

				#ifdef SHADER_API_MOBILE

					return saturate( dot( Light_Color_f3.rgb , float3(0.3,0.59,0.11) ) );

				#else

					float4 node_6613_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
					float4 node_6613_p = lerp(float4(float4(Light_Color_f3.rgb,0.0).zy, node_6613_k.wz), float4(float4(Light_Color_f3.rgb,0.0).yz, node_6613_k.xy), step(float4(Light_Color_f3.rgb,0.0).z, float4(Light_Color_f3.rgb,0.0).y));
					float4 node_6613_q = lerp(float4(node_6613_p.xyw, float4(Light_Color_f3.rgb,0.0).x), float4(float4(Light_Color_f3.rgb,0.0).x, node_6613_p.yzx), step(node_6613_p.x, float4(Light_Color_f3.rgb,0.0).x));
					float node_6613_d = node_6613_q.x - min(node_6613_q.w, node_6613_q.y);
					float node_6613_e = 1.0e-10;
					half3 node_6613 = float3(abs(node_6613_q.z + (node_6613_q.w - node_6613_q.y) / (6.0 * node_6613_d + node_6613_e)), node_6613_d / (node_6613_q.x + node_6613_e), node_6613_q.x);

					return saturate(node_6613.b);

				#endif

            }

            struct Attributes
            {

                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
				float4 vertexColor	: COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID

            };

            struct Varyings
            {

                float2 uv                       : TEXCOORD0;
                float4 positionWSAndFogFactor   : TEXCOORD2; 
                half3  normalWS                 : TEXCOORD3;
                half3  tangentWS                : TEXCOORD4;
                half3  bitangentWS              : TEXCOORD5;

#ifdef _MAIN_LIGHT_SHADOWS
                float4 shadowCoord              : TEXCOORD6; 
#endif
				float4 projPos					: TEXCOORD7;
				float4 posWorld					: TEXCOORD8;
				float4 vertexColor				: COLOR;
                float4 positionCS               : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO

            };

            Varyings LitPassVertex(Attributes input)
            {

                Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);


				float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
				
                output.normalWS = vertexNormalInput.normalWS;
                output.tangentWS = vertexNormalInput.tangentWS;
                output.bitangentWS = vertexNormalInput.bitangentWS;

                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                output.posWorld = mul(unity_ObjectToWorld, input.positionOS);
				output.uv = input.uv;
                output.vertexColor = input.vertexColor;
				output.positionCS = vertexInput.positionCS;

				output.projPos = ComputeScreenPos (output.positionCS);

#ifdef _MAIN_LIGHT_SHADOWS

                output.shadowCoord = GetShadowCoord(vertexInput);
#endif

                return output;
            }

            half4 LitPassFragment(Varyings input, float facing : VFACE) : SV_Target
            {

				UNITY_SETUP_INSTANCE_ID (input);
                float3 positionWS = input.positionWSAndFogFactor.xyz;

#ifdef _MAIN_LIGHT_SHADOWS

                Light mainLight = GetMainLight(input.shadowCoord);
#else
                Light mainLight = GetMainLight();
#endif

				#if N_F_NM_ON
					half3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(input.uv, _NormalMap)));
					float3 normalLocal = lerp(half3(0,0,1),_NormalMap_var.rgb,_NormalMapIntensity);
				#else
					float3 normalLocal = half3(0,0,1);
				#endif

				half3 color = 0;
				float3 A_L_O = 0;

				half isFrontFace = ( facing >= 0 ? 1 : 0 );
				float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
				float2 sceneUVs = (input.projPos.xy / input.projPos.w);

                input.normalWS = normalize(input.normalWS);
                float3x3 tangentTransform = float3x3( input.tangentWS, input.bitangentWS, input.normalWS);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform ));
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);
				half2 RTD_VD_Cal = (float2((sceneUVs.x * 2 - 1)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2 - 1).rg*RTD_OB_VP_CAL);

                half2 RTD_TC_TP_OO = lerp( input.uv, RTD_VD_Cal, _TexturePatternStyle );
                half2 node_4408 = RTD_TC_TP_OO;

				#if N_F_MC_ON 
            
				half2 MUV = (mul( UNITY_MATRIX_V, float4(normalDirection,0) ).xyz.rgb.rg*0.5+0.5); 
				half4 _MatCap_var = tex2D(_MCap,TRANSFORM_TEX(MUV, _MCap));
				half4 _MCapMask_var = tex2D(_MCapMask,TRANSFORM_TEX(input.uv, _MCapMask));
				float3 MCapOutP = lerp( 1, lerp( 1,_MatCap_var.rgb,_MCapIntensity) ,_MCapMask_var.rgb ); 
            
				#else
            
				half MCapOutP = 1;

				#endif

				half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4408, _MainTex));
				half3 _RTD_MVCOL = lerp(1, input.vertexColor.rgb, _MVCOL); 
				half3 RTD_TEX_COL = (_MainTex_var.rgb*_MainColor.rgb*MCapOutP*_RTD_MVCOL);
				
                half4 _MaskTransparency_var = tex2D(_MaskTransparency,TRANSFORM_TEX(input.uv, _MaskTransparency));

				half RTD_TRAN_MAS = (smoothstep(clamp(-20,1,_TransparentThreshold),1,_MainTex_var.a) *_MaskTransparency_var.r);
				half RTD_TRAN_OPA_Sli = lerp( RTD_TRAN_MAS, smoothstep(clamp(-20,1,_TransparentThreshold) , 1, _MainTex_var.a)  ,_Opacity);
				half RTD_TRAN = saturate(( 0.74 > 0.5 ? (1.0-(1.0-2.0*(0.74-0.5))*(1.0-RTD_TRAN_OPA_Sli)) : (2.0*0.74*RTD_TRAN_OPA_Sli) ));
				clip(RTD_TRAN - 0.5);

                float3 lightDirection = mainLight.direction;

				#if N_F_NLASOBF_ON
					float3 lightColor = lerp(0,mainLight.color.rgb,isFrontFace);
				#else
					float3 lightColor = mainLight.color.rgb;
				#endif

                float3 halfDirection = normalize(viewDirection+lightDirection);

                float attenuation = 1;

				#if N_F_SON_ON

					float3 node_76 = mul( unity_WorldToObject, float4((input.posWorld.rgb-objPos.rgb),0) ).xyz.rgb.rgb;
					half RTD_SON_VCBCSON_OO = lerp( _SmoothObjectNormal, (_SmoothObjectNormal*(1.0 - input.vertexColor.b)), _VertexColorBlueControlSmoothObjectNormal );
					half3 RTD_SON_ON_OTHERS = lerp(normalDirection,mul( unity_ObjectToWorld, float4(float3((_XYZPosition.r+(_XYZHardness*node_76.r)),(_XYZPosition.g+(_XYZHardness*node_76.g)),(_XYZPosition.b+(_XYZHardness*node_76.b))),0) ).xyz.rgb, RTD_SON_VCBCSON_OO);

					half3 RTD_SON = RTD_SON_ON_OTHERS;

					half3 RTD_SNorm_OO = lerp( 1.0, RTD_SON_ON_OTHERS, _ShowNormal );
					half3 RTD_SON_CHE_1 = RTD_SNorm_OO;

				#else

					half3 RTD_SON = normalDirection;
					half3 RTD_SON_CHE_1 = 1;

				#endif

				half3 RTD_GI_ST_Sli = (RTD_SON*_GIShadeThreshold);


				float node_2183 = 0;
                float node_8383 = 0.01;		

				half3 RTD_GI_FS_OO = lerp( RTD_GI_ST_Sli, float3(smoothstep( float2(node_2183,node_2183), float2(node_8383,node_8383), (RTD_SON.rb*_GIShadeThreshold) ),0.0), _GIFlatShade );

				#if N_F_SCT_ON

					half4 _ShadowColorTexture_var = tex2D(_ShadowColorTexture,TRANSFORM_TEX(input.uv, _ShadowColorTexture));
					half3 RTD_SCT_ON = lerp(_ShadowColorTexture_var.rgb,(_ShadowColorTexture_var.rgb*_ShadowColorTexture_var.rgb),_ShadowColorTexturePower);

					half3 RTD_SCT = RTD_SCT_ON;

				#else

					half3 RTD_SCT = 1;

				#endif

				#if N_F_PT_ON

					half2 node_1711 = RTD_VD_Cal;
					half4 _PTexture_var = tex2D(_PTexture,TRANSFORM_TEX(node_1711, _PTexture));
					half RTD_PT_ON = lerp((1.0 - _PTexturePower),1.0,_PTexture_var.r);

					half RTD_PT = RTD_PT_ON;

				#else

					half RTD_PT = 1;

				#endif
				
				half3 RTD_OSC = (_OverallShadowColor.rgb*_OverallShadowColorPower);

				half3 node_1860 = ((_SelfShadowColor.rgb*_SelfShadowColorPower)*RTD_OSC*RTD_SCT*RTD_PT);
				half3 node_6694 = (_LightIntensity+lightColor.rgb);

				half3 RTD_LAS = lerp(node_1860,(node_1860+node_6694),_LightAffectShadow);
				half3 RTD_HL = (_HighlightColor.rgb*_HighlightColorPower+_DirectionalLightIntensity);

				half RTD_LVLC = RTD_LVLC_F(lightColor.rgb);
				half3 RTD_MCIALO = lerp(RTD_TEX_COL , lerp(RTD_TEX_COL , _MainTex_var.rgb * MCapOutP * 0.7 , clamp((RTD_LVLC*1),0,1) ) , _MCIALO );

				#if N_F_GLO_ON

					#if N_F_GLOT_ON

						#ifndef SHADER_API_MOBILE
							float node_5992_ang = _GlossTextureRotate;
							float node_5992_spd = 1.0;
							float node_5992_cos = cos(node_5992_spd*node_5992_ang);
							float node_5992_sin = sin(node_5992_spd*node_5992_ang);
							float2 node_5992_piv = float2(0.5,0.5);
						#endif

							half3 RTD_GT_FL_Sli = lerp(viewDirection,halfDirection,_GlossTextureFollowLight);
							half3 node_2832 = reflect(RTD_GT_FL_Sli,normalDirection);

							half3 RTD_GT_FOR_OO = lerp( node_2832, mul( unity_WorldToObject, float4(node_2832,0) ).xyz.rgb, _GlossTextureFollowObjectRotation );
							half2 node_9280 = RTD_GT_FOR_OO.rg;

						#ifndef SHADER_API_MOBILE
							half2 node_5992 = (mul(float2((-1*node_9280.r),node_9280.g)-node_5992_piv,float2x2( node_5992_cos, -node_5992_sin, node_5992_sin, node_5992_cos))+node_5992_piv);
							half2 node_8759 = (node_5992*0.5+0.5);
						#endif

						#ifdef SHADER_API_MOBILE
							half4 _GlossTexture_var = tex2Dlod(_GlossTexture,float4(TRANSFORM_TEX((float2((-1*node_2832.r),node_2832.g)*0.5+0.5)  , _GlossTexture),0.0,_GlossTextureSoftness));
						#else
							half4 _GlossTexture_var = tex2Dlod(_GlossTexture,float4(TRANSFORM_TEX(node_8759, _GlossTexture),0.0,_GlossTextureSoftness));
						#endif

						half RTD_GT_ON = _GlossTexture_var.r;

						half3 RTD_GT = RTD_GT_ON;

					#else

						half RTD_GLO_MAIN_Sof_Sli = lerp(0.1,1.0,_GlossSoftness);
						half RTD_NDOTH = saturate(dot(halfDirection, normalDirection));
						half RTD_GLO_MAIN = smoothstep( 0.1, RTD_GLO_MAIN_Sof_Sli, pow(RTD_NDOTH,exp2(lerp(-2,15,_Glossiness))) );

						half3 RTD_GT = RTD_GLO_MAIN;

					#endif

					float node_49 = 0.0;
					half3 RTD_GLO_I_Sli = lerp(float3(node_49,node_49,node_49),RTD_GT,_GlossIntensity);
					half4 _MaskGloss_var = tex2D(_MaskGloss,TRANSFORM_TEX(input.uv, _MaskGloss));
					half3 RTD_GLO_MAS = lerp( RTD_HL, lerp( RTD_HL,(_GlossColor.rgb*_GlossColorPower), RTD_GLO_I_Sli ),_MaskGloss_var.r);

					half3 RTD_GLO = RTD_GLO_MAS;
            
				#else
            
					half3 RTD_GLO = RTD_HL;
            
				#endif


				half3 RTD_GLO_OTHERS = RTD_GLO;

				#if N_F_RL_ON

					float node_4353 = 0.0;
					float node_3687 = 0.0;
					half3 RTD_RL_LARL_OO = lerp( _RimLightColor.rgb, lerp(float3(node_3687,node_3687,node_3687),_RimLightColor.rgb,lightColor.rgb), _LightAffectRimLightColor );
					half RTD_RL_S_Sli = lerp(1.70,0.29,_RimLightSoftness);
					half3 RTD_RL_MAIN = lerp(float3(node_4353,node_4353,node_4353),(RTD_RL_LARL_OO*_RimLightColorPower),smoothstep( 1.71, RTD_RL_S_Sli, pow(abs( 1.0-max(0,dot(normalDirection, viewDirection)) ),(1.0 - _RimLightUnfill)) ));
					half3 RTD_RL_IL_OO = lerp(RTD_GLO_OTHERS,(RTD_GLO_OTHERS+RTD_RL_MAIN),_RimLightInLight);

					half3 RTD_RL_CHE_1 = RTD_RL_IL_OO;
            
				#else
            
					half3 RTD_RL_CHE_1 = RTD_GLO_OTHERS;
            
				#endif

				#if N_F_CLD_ON

					half3 RTD_CLD_CLDFOR_OO = lerp( _CustomLightDirection.rgb, mul( unity_ObjectToWorld, float4(_CustomLightDirection.rgb,0) ).xyz.rgb, _CustomLightDirectionFollowObjectRotation );
					half3 RTD_CLD_CLDI_Sli = lerp(lightDirection,RTD_CLD_CLDFOR_OO,_CustomLightDirectionIntensity);
					half3 RTD_CLD = RTD_CLD_CLDI_Sli;
            
				#else
            
					half3 RTD_CLD = lightDirection;
            
				#endif

				half3 RTD_ST_SS_AVD_OO = lerp( RTD_CLD, viewDirection, _SelfShadowShadowTAtViewDirection );
				half RTD_NDOTL = 0.5*dot(RTD_ST_SS_AVD_OO,RTD_SON)+0.5;

				half3 RTD_ST_OFF_OTHERS = (RTD_RL_CHE_1*RTD_SON_CHE_1*lightColor.rgb);

				#if N_F_ST_ON

					float node_4824 = 1.0;
					half node_4736 = 1.0; 
					float node_7121 = 1.0;
					float node_3187 = 0.22; 

					half4 _ShadowT_var = tex2D(_ShadowT,TRANSFORM_TEX(input.uv, _ShadowT));
					half3 node_338 = ((_ShadowTColor.rgb*_ShadowTColorPower)*RTD_SCT*RTD_PT*RTD_OSC);

					half RTD_ST_H_Sli = lerp(0.0,0.22,_ShadowTHardness);
					half3 RTD_ST_IS_ON = lerp(float3(node_4736,node_4736,node_4736),lerp(node_338,float3(node_7121,node_7121,node_7121),  smoothstep( RTD_ST_H_Sli, node_3187, (_ShowInAmbientLightShadowThreshold*_ShadowT_var.r) )),_ShadowTIntensity);

					#if N_F_STIAL_ON

						float node_3456 = 1.0; 
						half3 RTD_ST_ALI_Sli = lerp(float3(node_3456,node_3456,node_3456),RTD_ST_IS_ON,_ShowInAmbientLightShadowIntensity);
						half3 RTD_STIAL_ON = lerp(RTD_ST_ALI_Sli,float3(node_4824,node_4824,node_4824),clamp((RTD_LVLC*8.0),0,1));

						half3 RTD_STIAL = RTD_STIAL_ON;

					#else

						half3 RTD_STIAL = 1;

					#endif

					#if N_F_STIS_ON
					
						half3 RTD_ST_IS = lerp(1,RTD_ST_IS_ON,_ShowInAmbientLightShadowIntensity); 

            		#else
            
            			half3 RTD_ST_IS = 1;
            
            		#endif

					half RTD_ST_LFAST_OO = lerp(lerp( RTD_NDOTL, (attenuation*RTD_NDOTL), _LightFalloffAffectShadowT ) , 1 , _STIL );
					half RTD_ST_In_Sli = lerp(node_4736,smoothstep( RTD_ST_H_Sli, node_3187, ((_ShadowT_var.r*(1.0 - _ShadowTShadowThreshold))*(RTD_ST_LFAST_OO*_ShadowTLightThreshold*0.01)) ),_ShadowTIntensity);
					half3 RTD_ST_ON = lerp((lerp(node_338,(node_338+node_6694),_LightAffectShadow)*RTD_LVLC),RTD_ST_OFF_OTHERS,RTD_ST_In_Sli);

					half3 RTD_ST = RTD_ST_ON;

				#else

					half3 RTD_ST = RTD_ST_OFF_OTHERS;
					half3 RTD_STIAL = 1;
					half3 RTD_ST_IS = 1;

				#endif

				half RTD_SS_SV = 1.0;

				#if N_F_SS_ON
 
					half RTD_SS_SSH_Sil = lerp(0.3,1.0,_SelfShadowHardness);
					half RTD_SS_VCGCSSS_OO = lerp( _SelfShadowThreshold, (_SelfShadowThreshold*(1.0 - input.vertexColor.g)), _VertexColorGreenControlSelfShadowSize );
					half RTD_SS_SST = smoothstep( RTD_SS_SSH_Sil, 1.0, (RTD_NDOTL*lerp(7,RTD_SS_VCGCSSS_OO,_SelfShadowThreshold)) );
					half RTD_SS_SSABLSS_OO = lerp( RTD_SS_SST, lerp(RTD_SS_SST,RTD_SS_SV, (1.0 - _MainLightShadowData.x) ), _SelfShadowAffectedByLightShadowStrength );
					half RTD_SS_ON = lerp(RTD_SS_SV,(RTD_SS_SSABLSS_OO*attenuation),_SelfShadowIntensity);

					half RTD_SS = RTD_SS_ON;
            
				#else
    
					half RTD_SS_OFF = lerp(RTD_SS_SV,attenuation,_SelfShadowIntensity);

					half RTD_SS = RTD_SS_OFF;

				#endif
				
				half3 RTD_R_OFF_OTHERS = ( lerp( RTD_TEX_COL , _MainTex_var.rgb , _MCIALO)  * lerp((RTD_LAS*RTD_LVLC*RTD_ST_IS),RTD_ST,RTD_SS));

				#if N_F_R_ON

					half3 RTD_FR_OFF_OTHERS = Ref( viewReflectDirection , _ReflectionRoughtness );

					#if N_F_FR_ON

						half2 node_8431 = reflect(viewDirection,normalDirection).rg;
						half2 node_4207 = (float2(node_8431.r,(-1*node_8431.g))*0.5+0.5);
						half4 _FReflection_var = tex2Dlod(_FReflection,float4(TRANSFORM_TEX(node_4207, _FReflection),0.0,_ReflectionRoughtness));
						half3 RTD_FR_ON = _FReflection_var.rgb;

						half3 RTD_FR = RTD_FR_ON;

					#else

						half3 RTD_FR = RTD_FR_OFF_OTHERS;

					#endif

					half4 _MaskReflection_var = tex2D(_MaskReflection,TRANSFORM_TEX(input.uv, _MaskReflection));
					half3 RTD_R_MET_Sli = lerp(1,(9 * (RTD_TEX_COL - (9 * 0.005) ) ) , _RefMetallic);
					half3 RTD_R_MAS = lerp(RTD_R_OFF_OTHERS, (RTD_FR * RTD_R_MET_Sli) ,_MaskReflection_var.r);
					half3 RTD_R_ON = lerp(RTD_R_OFF_OTHERS,RTD_R_MAS,_ReflectionIntensity);

					half3 RTD_R = RTD_R_ON;

				#else
				            
					half3 RTD_R = RTD_R_OFF_OTHERS;
            
				#endif

				float node_3622 = 0.0;
				float node_491 = 1.0;

				half3 RTD_SL_OFF_OTHERS = (AL_GI( lerp(float3(node_3622,node_3622,node_3622),float3(node_491,node_491,node_491),RTD_GI_FS_OO) )*_EnvironmentalLightingIntensity);

				#if N_F_SL_ON

					half3 RTD_SL_HC_OO = lerp( 1.0, RTD_TEX_COL, _SelfLitHighContrast );
					half4 _MaskSelfLit_var = tex2D(_MaskSelfLit,TRANSFORM_TEX(input.uv, _MaskSelfLit));
					half3 RTD_SL_MAS = lerp(RTD_SL_OFF_OTHERS,((_SelfLitColor.rgb * RTD_TEX_COL * RTD_SL_HC_OO)*_SelfLitPower),_MaskSelfLit_var.r);
					half3 RTD_SL_ON = lerp(RTD_SL_OFF_OTHERS,RTD_SL_MAS,_SelfLitIntensity);

					half3 RTD_SL = RTD_SL_ON; 

					half3 RTD_R_SEL = lerp(RTD_R,lerp(RTD_R,RTD_TEX_COL,_MaskSelfLit_var.r),_SelfLitIntensity);
					half3 RTD_SL_CHE_1 = RTD_R_SEL;

				#else

					half3 RTD_SL = RTD_SL_OFF_OTHERS;
					half3 RTD_SL_CHE_1 = RTD_R;

				#endif

				#if N_F_RL_ON

					half3 RTD_RL_ON = lerp((RTD_SL_CHE_1+RTD_RL_MAIN),RTD_SL_CHE_1,_RimLightInLight);
					half3 RTD_RL = RTD_RL_ON;
            
				#else
            
					half3 RTD_RL = RTD_SL_CHE_1;
            
				#endif

				half3 RTD_CA_OFF =  ((RTD_MCIALO*RTD_SL*RTD_STIAL)+RTD_RL);

				#if N_F_CA_ON
            
					half3 RTD_CA_ON = lerp(RTD_CA_OFF,dot(RTD_CA_OFF,float3(0.3,0.59,0.11)),(1.0 - _Saturation));
					half3 RTD_CA = RTD_CA_ON;

				#else

					half3 RTD_CA = RTD_CA_OFF;

				#endif

				float3 main_light_output = RTD_CA;
               

#ifdef _ADDITIONAL_LIGHTS

#if N_F_EAL_ON

                int additionalLightsCount = GetAdditionalLightsCount();

                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light light = GetAdditionalLight(i, positionWS);

                float3 lightDirection = light.direction;

				#if N_F_NLASOBF_ON
					float3 lightColor = lerp(0,light.color.rgb,isFrontFace);
				#else
					float3 lightColor = light.color.rgb;
				#endif

                float3 halfDirection = normalize(viewDirection+lightDirection);

				half attenuation = 1; 
				half lightfos = smoothstep(0, _LightFalloffSoftness ,light.distanceAttenuation);

				half3 node_6694 = (_LightIntensity+lightColor.rgb);

				half3 RTD_LAS = lerp(node_1860,(node_1860+node_6694),_LightAffectShadow);
				half3 RTD_HL = (_HighlightColor.rgb*_HighlightColorPower+_PointSpotlightIntensity);

				half RTD_LVLC = RTD_LVLC_F(lightColor.rgb);
				half3 RTD_MCIALO = lerp(RTD_TEX_COL , lerp(RTD_TEX_COL , _MainTex_var.rgb  * MCapOutP * 0.7 , clamp((RTD_LVLC*1),0,1) ) , _MCIALO );

				#if N_F_GLO_ON

					#if N_F_GLOT_ON

						#ifndef SHADER_API_MOBILE
							float node_5992_ang = _GlossTextureRotate;
							float node_5992_spd = 1.0;
							float node_5992_cos = cos(node_5992_spd*node_5992_ang);
							float node_5992_sin = sin(node_5992_spd*node_5992_ang);
							float2 node_5992_piv = float2(0.5,0.5);
						#endif

							half3 RTD_GT_FL_Sli = lerp(viewDirection,halfDirection,_GlossTextureFollowLight);
							half3 node_2832 = reflect(RTD_GT_FL_Sli,normalDirection);

							half3 RTD_GT_FOR_OO = lerp( node_2832, mul( unity_WorldToObject, float4(node_2832,0) ).xyz.rgb, _GlossTextureFollowObjectRotation );
							half2 node_9280 = RTD_GT_FOR_OO.rg;

						#ifndef SHADER_API_MOBILE
							half2 node_5992 = (mul(float2((-1*node_9280.r),node_9280.g)-node_5992_piv,float2x2( node_5992_cos, -node_5992_sin, node_5992_sin, node_5992_cos))+node_5992_piv);
							half2 node_8759 = (node_5992*0.5+0.5);
						#endif

						#ifdef SHADER_API_MOBILE
							half4 _GlossTexture_var = tex2Dlod(_GlossTexture,float4(TRANSFORM_TEX((float2((-1*node_2832.r),node_2832.g)*0.5+0.5)  , _GlossTexture),0.0,_GlossTextureSoftness));
						#else
							half4 _GlossTexture_var = tex2Dlod(_GlossTexture,float4(TRANSFORM_TEX(node_8759, _GlossTexture),0.0,_GlossTextureSoftness));
						#endif

						half RTD_GT_ON = _GlossTexture_var.r;

						half3 RTD_GT = RTD_GT_ON;

					#else

						half RTD_GLO_MAIN_Sof_Sli = lerp(0.1,1.0,_GlossSoftness);
						half RTD_NDOTH = saturate(dot(halfDirection, normalDirection));
						half RTD_GLO_MAIN = smoothstep( 0.1, RTD_GLO_MAIN_Sof_Sli, pow(RTD_NDOTH,exp2(lerp(-2,15,_Glossiness))) );

						half3 RTD_GT = RTD_GLO_MAIN;

					#endif

					float node_49 = 0.0;
					half3 RTD_GLO_I_Sli = lerp(float3(node_49,node_49,node_49),RTD_GT,_GlossIntensity);
					half4 _MaskGloss_var = tex2D(_MaskGloss,TRANSFORM_TEX(input.uv, _MaskGloss));
					half3 RTD_GLO_MAS = lerp( RTD_HL, lerp( RTD_HL,(_GlossColor.rgb*_GlossColorPower), RTD_GLO_I_Sli ),_MaskGloss_var.r);

					half3 RTD_GLO = RTD_GLO_MAS;
            
				#else
            
					half3 RTD_GLO = RTD_HL;
            
				#endif


				half3 RTD_GLO_OTHERS = RTD_GLO;

				#if N_F_RL_ON

					float node_4353 = 0.0;
					float node_3687 = 0.0;
					half3 RTD_RL_LARL_OO = lerp( _RimLightColor.rgb, lerp(float3(node_3687,node_3687,node_3687),_RimLightColor.rgb,lightColor.rgb), _LightAffectRimLightColor );
					half RTD_RL_S_Sli = lerp(1.70,0.29,_RimLightSoftness);
					half3 RTD_RL_MAIN = lerp(float3(node_4353,node_4353,node_4353),(RTD_RL_LARL_OO*_RimLightColorPower),smoothstep( 1.71, RTD_RL_S_Sli, pow(abs( 1.0-max(0,dot(normalDirection, viewDirection)) ),(1.0 - _RimLightUnfill)) ));
					half3 RTD_RL_IL_OO = lerp(RTD_GLO_OTHERS,(RTD_GLO_OTHERS+RTD_RL_MAIN),_RimLightInLight);

					half3 RTD_RL_CHE_1 = RTD_RL_IL_OO;
            
				#else
            
					half3 RTD_RL_CHE_1 = RTD_GLO_OTHERS;
            
				#endif

				#if N_F_CLD_ON

					half3 RTD_CLD_CLDFOR_OO = lerp( _CustomLightDirection.rgb, mul( unity_ObjectToWorld, float4(_CustomLightDirection.rgb,0) ).xyz.rgb, _CustomLightDirectionFollowObjectRotation );
					half3 RTD_CLD_CLDI_Sli = lerp(lightDirection,RTD_CLD_CLDFOR_OO,_CustomLightDirectionIntensity);
					half3 RTD_CLD = RTD_CLD_CLDI_Sli;
            
				#else
            
					half3 RTD_CLD = lightDirection;
            
				#endif

				half3 RTD_ST_SS_AVD_OO = lerp( RTD_CLD, viewDirection, _SelfShadowShadowTAtViewDirection );
				half RTD_NDOTL = 0.5*dot(RTD_ST_SS_AVD_OO,RTD_SON)+0.5;
				
				half3 RTD_ST_OFF_OTHERS = (RTD_RL_CHE_1*RTD_SON_CHE_1*lightColor.rgb);

				#if N_F_ST_ON

					float node_4824 = 1.0;
					half node_4736 = 1.0; 
					float node_7121 = 1.0;
					float node_3187 = 0.22; 

					half4 _ShadowT_var = tex2D(_ShadowT,TRANSFORM_TEX(input.uv, _ShadowT));
					half3 node_338 = ((_ShadowTColor.rgb*_ShadowTColorPower)*RTD_SCT*RTD_PT*RTD_OSC);

					half RTD_ST_H_Sli = lerp(0.0,0.22,_ShadowTHardness);
					half3 RTD_ST_IS_ON = lerp(float3(node_4736,node_4736,node_4736),lerp(node_338,float3(node_7121,node_7121,node_7121),  smoothstep( RTD_ST_H_Sli, node_3187, (_ShowInAmbientLightShadowThreshold*_ShadowT_var.r) )),_ShadowTIntensity);

					#if N_F_STIAL_ON

						float node_3456 = 1.0; 
						half3 RTD_ST_ALI_Sli = lerp(float3(node_3456,node_3456,node_3456),RTD_ST_IS_ON,_ShowInAmbientLightShadowIntensity);
						half3 RTD_STIAL_ON = lerp(RTD_ST_ALI_Sli,float3(node_4824,node_4824,node_4824),clamp((RTD_LVLC*8.0),0,1));

						half3 RTD_STIAL = RTD_STIAL_ON;

					#else

						half3 RTD_STIAL = 1;

					#endif

					#if N_F_STIS_ON
					
						half3 RTD_ST_IS = lerp(1,RTD_ST_IS_ON,_ShowInAmbientLightShadowIntensity); 

            		#else
            
            			half3 RTD_ST_IS = 1;
            
            		#endif

					half RTD_ST_LFAST_OO = lerp(lerp( RTD_NDOTL, (lightfos*RTD_NDOTL), _LightFalloffAffectShadowT ) , 1 , _STIL );
					half RTD_ST_In_Sli = lerp(node_4736,smoothstep( RTD_ST_H_Sli, node_3187, ((_ShadowT_var.r*(1.0 - _ShadowTShadowThreshold))*(RTD_ST_LFAST_OO*_ShadowTLightThreshold*0.01)) ),_ShadowTIntensity);
					half3 RTD_ST_ON = lerp((lerp(node_338,(node_338+node_6694),_LightAffectShadow)*RTD_LVLC),RTD_ST_OFF_OTHERS,RTD_ST_In_Sli);

					half3 RTD_ST = RTD_ST_ON;

				#else

					half3 RTD_ST = RTD_ST_OFF_OTHERS;
					half3 RTD_STIAL = 1;
					half3 RTD_ST_IS = 1;

				#endif
				
				half RTD_SS_SV = 1.0;

				#if N_F_SS_ON
 
					half RTD_SS_SSH_Sil = lerp(0.3,1.0,_SelfShadowHardness);
					half RTD_SS_VCGCSSS_OO = lerp( _SelfShadowThreshold, (_SelfShadowThreshold*(1.0 - input.vertexColor.g)), _VertexColorGreenControlSelfShadowSize );
					half RTD_SS_SST = smoothstep( RTD_SS_SSH_Sil, 1.0, (RTD_NDOTL*lerp(7,RTD_SS_VCGCSSS_OO,_SelfShadowThreshold)) );
					half RTD_SS_SSABLSS_OO = lerp( RTD_SS_SST, lerp(RTD_SS_SST,RTD_SS_SV, (1.0 - GetAdditionalLightShadowParams(i).x) ), _SelfShadowAffectedByLightShadowStrength );
					half RTD_SS_ON = lerp(RTD_SS_SV,(RTD_SS_SSABLSS_OO*attenuation),_SelfShadowIntensity);

					half RTD_SS = RTD_SS_ON;
            
				#else
    
					half RTD_SS_OFF = lerp(RTD_SS_SV,attenuation,_SelfShadowIntensity);

					half RTD_SS = RTD_SS_OFF;

				#endif

				half3 RTD_R_OFF_OTHERS = ( lerp( RTD_TEX_COL , _MainTex_var.rgb , _MCIALO)  * lerp((RTD_LAS*RTD_LVLC*RTD_ST_IS),RTD_ST,RTD_SS));

				#if N_F_R_ON

					half3 RTD_FR_OFF_OTHERS = Ref( viewReflectDirection , _ReflectionRoughtness );

					#if N_F_FR_ON

						half2 node_8431 = reflect(viewDirection,normalDirection).rg;
						half2 node_4207 = (float2(node_8431.r,(-1*node_8431.g))*0.5+0.5);
						half4 _FReflection_var = tex2Dlod(_FReflection,float4(TRANSFORM_TEX(node_4207, _FReflection),0.0,_ReflectionRoughtness));
						half3 RTD_FR_ON = _FReflection_var.rgb;

						half3 RTD_FR = RTD_FR_ON;

					#else

						half3 RTD_FR = RTD_FR_OFF_OTHERS;

					#endif

					half4 _MaskReflection_var = tex2D(_MaskReflection,TRANSFORM_TEX(input.uv, _MaskReflection));
					half3 RTD_R_MET_Sli = lerp(1,(9 * (RTD_TEX_COL - (9 * 0.005) ) ) , _RefMetallic);
					half3 RTD_R_MAS = lerp(RTD_R_OFF_OTHERS, (RTD_FR * RTD_R_MET_Sli) ,_MaskReflection_var.r);
					half3 RTD_R_ON = lerp(RTD_R_OFF_OTHERS,RTD_R_MAS,_ReflectionIntensity);

					half3 RTD_R = RTD_R_ON;

				#else
				            
					half3 RTD_R = RTD_R_OFF_OTHERS;
            
				#endif

				#if N_F_SL_ON

                half3 RTD_SL_HC_OO = lerp( 1.0, RTD_TEX_COL, _SelfLitHighContrast );
				half4 _MaskSelfLit_var = tex2D(_MaskSelfLit,TRANSFORM_TEX(input.uv, _MaskSelfLit));
				half3 RTD_SL_MAS = lerp(RTD_SL_OFF_OTHERS,((_SelfLitColor.rgb * RTD_TEX_COL * RTD_SL_HC_OO)*_SelfLitPower),_MaskSelfLit_var.r);
				half3 RTD_SL_ON = lerp(RTD_SL_OFF_OTHERS,RTD_SL_MAS,_SelfLitIntensity);

				half3 RTD_SL = RTD_SL_ON; 

				half3 RTD_R_SEL = lerp(RTD_R,lerp(RTD_R,RTD_TEX_COL,_MaskSelfLit_var.r),_SelfLitIntensity);
				half3 RTD_SL_CHE_1 = RTD_R_SEL;

				#else

					half3 RTD_SL = RTD_SL_OFF_OTHERS;
					half3 RTD_SL_CHE_1 = RTD_R;

				#endif

				#if N_F_RL_ON

					half3 RTD_RL_ON = lerp((RTD_SL_CHE_1+RTD_RL_MAIN),RTD_SL_CHE_1,_RimLightInLight);
					half3 RTD_RL = RTD_RL_ON;
            
				#else
            
					half3 RTD_RL = RTD_SL_CHE_1;
            
				#endif

				half3 RTD_CA_OFF =  ((RTD_MCIALO*RTD_SL*RTD_STIAL)+RTD_RL);

				#if N_F_CA_ON
            
					half3 RTD_CA_ON = lerp(RTD_CA_OFF,dot(RTD_CA_OFF,float3(0.3,0.59,0.11)),(1.0 - _Saturation));
					half3 RTD_CA = RTD_CA_ON;

				#else

					half3 RTD_CA = RTD_CA_OFF;

				#endif

				float3 add_light_output = RTD_CA * lightfos;

				#if _USETLB_ON
					A_L_O = add_light_output + A_L_O;
				#else
					A_L_O = max (add_light_output,A_L_O);
				#endif

                }
#endif

#endif
				#if _USETLB_ON
					color = main_light_output + A_L_O;
				#else
					color = max (main_light_output,A_L_O);
				#endif

                float fogFactor = input.positionWSAndFogFactor.w;

                color = MixFog(color, fogFactor);
                return half4(color, RTD_TRAN_OPA_Sli);

            }

            ENDHLSL

        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            Cull Off

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 wiiu switch 
            #pragma target 3.0

            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)

            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform half _TexturePatternStyle;

			uniform half _Opacity;
            uniform sampler2D _MaskTransparency; uniform float4 _MaskTransparency_ST;
			uniform half _TransparentThreshold;

			uniform sampler3D _DitherMaskLOD;

			CBUFFER_END

			float4 _ShadowBias;
			float3 _LightDirection;

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normalOS     : NORMAL;
				float2 texcoord     : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
				float4 projPos		: TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
				float scale = invNdotL * _ShadowBias.y;

				positionWS = _LightDirection * _ShadowBias.xxx + positionWS;
				positionWS = normalWS * scale.xxx + positionWS;
				float4 positionCS = TransformWorldToHClip(positionWS);

			#if UNITY_REVERSED_Z
				positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#else
				positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#endif

			return positionCS;

			}

			Varyings ShadowPassVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.uv = input.texcoord;

				float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = vertexInput.positionCS;
                output.projPos = ComputeScreenPos (output.positionCS);
				output.positionCS = GetShadowPositionHClip(input);

				return output;

			}

			half4 ShadowPassFragment(Varyings input) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);

                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float2 sceneUVs = (input.projPos.xy / input.projPos.w);

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);
				half2 RTD_VD_Cal = (float2((sceneUVs.x * 2 - 1)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2 - 1).rg*RTD_OB_VP_CAL);

				half2 RTD_TC_TP_OO = lerp( input.uv, RTD_VD_Cal, _TexturePatternStyle );
				half2 node_4408 = RTD_TC_TP_OO;

				half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4408, _MainTex));

                half4 _MaskTransparency_var = tex2D(_MaskTransparency,TRANSFORM_TEX(input.uv, _MaskTransparency));

				half RTD_TRAN_MAS = (smoothstep(clamp(-20,1,_TransparentThreshold),1,_MainTex_var.a) *_MaskTransparency_var.r);
				half RTD_TRAN_OPA_Sli = lerp( RTD_TRAN_MAS, smoothstep(clamp(-20,1,_TransparentThreshold) , 1, _MainTex_var.a)  ,_Opacity);

				float dither = tex3D(_DitherMaskLOD, float3(input.positionCS.xy * 0.25, RTD_TRAN_OPA_Sli * 0.99)).a;

				clip(saturate(( 0.74 > 0.5 ? (1.0-(1.0-2.0*(0.74-0.5))*(1.0-dither)) : (2.0*0.74*dither) )) - 0.5);

				return 0;
			}

            ENDHLSL
        }

		Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            Cull [_DoubleSided]

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 wiiu switch 
            #pragma target 3.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 position     : POSITION;
				float2 texcoord     : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings DepthOnlyVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionCS = TransformObjectToHClip(input.position.xyz);
				return output;
			}

			half4 DepthOnlyFragment(Varyings input) : SV_TARGET
			{
				return 0;
			}


            ENDHLSL
        }

    }

    FallBack "Hidden/InternalErrorShader"

    CustomEditor "RealToonShaderGUI_URP_SRP"
}
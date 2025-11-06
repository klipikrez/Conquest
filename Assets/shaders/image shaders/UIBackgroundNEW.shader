Shader "UIBackgroundSwords"
{
    Properties
    {
        [NoScaleOffset]_Tex("Tex", 2D) = "white" {}
        _Color1("Color1", Color) = (1, 1, 1, 1)
        _Color2("Color2", Color) = (0.5660378, 0.5660378, 0.5660378, 1)
        _Color3("Color3", Color) = (0, 0, 0, 1)
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Pass
        {
            Name "Default"
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Tex_TexelSize;
        float4 _Color1;
        float4 _Color2;
        float4 _Color3;
        float4 _MainTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Tex);
        SAMPLER(sampler_Tex);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A - B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_8089e33da0bc4bcf98f91292080e737e_Out_0 = UnityBuildTexture2DStructNoScale(_Tex);
            float4 _UV_fbb43bffd1b941fc96f9b213a9140037_Out_0 = IN.uv0;
            float4 _Multiply_a41acd8ee589454d8b03304f0315ef43_Out_2;
            Unity_Multiply_float4_float4(_UV_fbb43bffd1b941fc96f9b213a9140037_Out_0, float4(2, 2, 2, 2), _Multiply_a41acd8ee589454d8b03304f0315ef43_Out_2);
            float4 _Subtract_33e7bfc80609478f87b140e2da557b9a_Out_2;
            Unity_Subtract_float4(float4(1, 0.5, 1, 1), _Multiply_a41acd8ee589454d8b03304f0315ef43_Out_2, _Subtract_33e7bfc80609478f87b140e2da557b9a_Out_2);
            float _Multiply_dc46d23217f144f1b2453412b4355784_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, 0.1, _Multiply_dc46d23217f144f1b2453412b4355784_Out_2);
            float _Multiply_d1f41b2973c04c99bdd48950135ccb80_Out_2;
            Unity_Multiply_float_float(_Multiply_dc46d23217f144f1b2453412b4355784_Out_2, -1, _Multiply_d1f41b2973c04c99bdd48950135ccb80_Out_2);
            float4 _Add_fbc5dfb92ec6408c8653ac5525ca9572_Out_2;
            Unity_Add_float4(_Subtract_33e7bfc80609478f87b140e2da557b9a_Out_2, (_Multiply_d1f41b2973c04c99bdd48950135ccb80_Out_2.xxxx), _Add_fbc5dfb92ec6408c8653ac5525ca9572_Out_2);
            float4 _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8089e33da0bc4bcf98f91292080e737e_Out_0.tex, _Property_8089e33da0bc4bcf98f91292080e737e_Out_0.samplerstate, _Property_8089e33da0bc4bcf98f91292080e737e_Out_0.GetTransformedUV((_Add_fbc5dfb92ec6408c8653ac5525ca9572_Out_2.xy)));
            float _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_R_4 = _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_RGBA_0.r;
            float _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_G_5 = _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_RGBA_0.g;
            float _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_B_6 = _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_RGBA_0.b;
            float _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_A_7 = _SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_RGBA_0.a;
            UnityTexture2D _Property_994016af618c4094a91baca265727a20_Out_0 = UnityBuildTexture2DStructNoScale(_Tex);
            float _Multiply_5c39fd8dadeb46919bf620ce3b5980db_Out_2;
            Unity_Multiply_float_float(_Multiply_dc46d23217f144f1b2453412b4355784_Out_2, -1, _Multiply_5c39fd8dadeb46919bf620ce3b5980db_Out_2);
            float4 _Add_6e682cec154840d18725d591bd9beca1_Out_2;
            Unity_Add_float4(_Multiply_a41acd8ee589454d8b03304f0315ef43_Out_2, (_Multiply_5c39fd8dadeb46919bf620ce3b5980db_Out_2.xxxx), _Add_6e682cec154840d18725d591bd9beca1_Out_2);
            float4 _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_994016af618c4094a91baca265727a20_Out_0.tex, _Property_994016af618c4094a91baca265727a20_Out_0.samplerstate, _Property_994016af618c4094a91baca265727a20_Out_0.GetTransformedUV((_Add_6e682cec154840d18725d591bd9beca1_Out_2.xy)));
            float _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_R_4 = _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_RGBA_0.r;
            float _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_G_5 = _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_RGBA_0.g;
            float _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_B_6 = _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_RGBA_0.b;
            float _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_A_7 = _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_RGBA_0.a;
            float _Add_801bb864eb7140f8839f68d5b7a9ce68_Out_2;
            Unity_Add_float(_SampleTexture2D_efd88ed658914be5afc4e96f90f2f1ba_A_7, _SampleTexture2D_c74f575977404c219bd22f40a8543c5f_A_7, _Add_801bb864eb7140f8839f68d5b7a9ce68_Out_2);
            float _Clamp_67416635085f4679a8edaf7e5599736b_Out_3;
            Unity_Clamp_float(_Add_801bb864eb7140f8839f68d5b7a9ce68_Out_2, 0, 1, _Clamp_67416635085f4679a8edaf7e5599736b_Out_3);
            float3 _Vector3_a458c9045876456d891ed84e99b121b2_Out_0 = float3(0, -0.03, 0);
            float3 _Multiply_565c510f244640beb231d7c918f73dcb_Out_2;
            Unity_Multiply_float3_float3(_Vector3_a458c9045876456d891ed84e99b121b2_Out_0, float3(-1, -1, -1), _Multiply_565c510f244640beb231d7c918f73dcb_Out_2);
            float3 _Subtract_2df18b1715964ef3bb6e0aff816e907e_Out_2;
            Unity_Subtract_float3((_Add_fbc5dfb92ec6408c8653ac5525ca9572_Out_2.xyz), _Multiply_565c510f244640beb231d7c918f73dcb_Out_2, _Subtract_2df18b1715964ef3bb6e0aff816e907e_Out_2);
            float4 _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8089e33da0bc4bcf98f91292080e737e_Out_0.tex, _Property_8089e33da0bc4bcf98f91292080e737e_Out_0.samplerstate, _Property_8089e33da0bc4bcf98f91292080e737e_Out_0.GetTransformedUV((_Subtract_2df18b1715964ef3bb6e0aff816e907e_Out_2.xy)));
            float _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_R_4 = _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_RGBA_0.r;
            float _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_G_5 = _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_RGBA_0.g;
            float _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_B_6 = _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_RGBA_0.b;
            float _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_A_7 = _SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_RGBA_0.a;
            float3 _Subtract_380c607f3a7243beadaf89c4b259e30f_Out_2;
            Unity_Subtract_float3((_Add_6e682cec154840d18725d591bd9beca1_Out_2.xyz), _Vector3_a458c9045876456d891ed84e99b121b2_Out_0, _Subtract_380c607f3a7243beadaf89c4b259e30f_Out_2);
            float4 _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_994016af618c4094a91baca265727a20_Out_0.tex, _Property_994016af618c4094a91baca265727a20_Out_0.samplerstate, _Property_994016af618c4094a91baca265727a20_Out_0.GetTransformedUV((_Subtract_380c607f3a7243beadaf89c4b259e30f_Out_2.xy)));
            float _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_R_4 = _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_RGBA_0.r;
            float _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_G_5 = _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_RGBA_0.g;
            float _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_B_6 = _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_RGBA_0.b;
            float _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_A_7 = _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_RGBA_0.a;
            float _Add_65a3c590d5cf49b9bb25e0a1b9797deb_Out_2;
            Unity_Add_float(_SampleTexture2D_9791b37651bb4d6ebe31cbce4339a041_A_7, _SampleTexture2D_2ee13cf50022442399893c5e1a9cacf4_A_7, _Add_65a3c590d5cf49b9bb25e0a1b9797deb_Out_2);
            float _Clamp_481728790eb04b01b679397364687b14_Out_3;
            Unity_Clamp_float(_Add_65a3c590d5cf49b9bb25e0a1b9797deb_Out_2, 0, 1, _Clamp_481728790eb04b01b679397364687b14_Out_3);
            float _Subtract_d99f32fd9e8a4e0eaf29666d575958f8_Out_2;
            Unity_Subtract_float(_Clamp_481728790eb04b01b679397364687b14_Out_3, _Clamp_67416635085f4679a8edaf7e5599736b_Out_3, _Subtract_d99f32fd9e8a4e0eaf29666d575958f8_Out_2);
            float _Clamp_6e2be452ac754d839d6814ebadccf9a2_Out_3;
            Unity_Clamp_float(_Subtract_d99f32fd9e8a4e0eaf29666d575958f8_Out_2, 0, 1, _Clamp_6e2be452ac754d839d6814ebadccf9a2_Out_3);
            float _Add_c63d6d574a72442dab02c601d7e483cf_Out_2;
            Unity_Add_float(_Clamp_67416635085f4679a8edaf7e5599736b_Out_3, _Clamp_6e2be452ac754d839d6814ebadccf9a2_Out_3, _Add_c63d6d574a72442dab02c601d7e483cf_Out_2);
            float _Subtract_097a2820382d412aaa4aa080ebf00415_Out_2;
            Unity_Subtract_float(1, _Add_c63d6d574a72442dab02c601d7e483cf_Out_2, _Subtract_097a2820382d412aaa4aa080ebf00415_Out_2);
            float4 _Property_e46f1d2de064436e9b691ec5423db598_Out_0 = _Color3;
            float4 _Multiply_6bee981ff91049cfbc0df64607ea40e4_Out_2;
            Unity_Multiply_float4_float4((_Subtract_097a2820382d412aaa4aa080ebf00415_Out_2.xxxx), _Property_e46f1d2de064436e9b691ec5423db598_Out_0, _Multiply_6bee981ff91049cfbc0df64607ea40e4_Out_2);
            float4 _Property_622ceb09352c46548a53b70a6fc45f1d_Out_0 = _Color1;
            float4 _Multiply_d559fe54b6fd41b19f245be2772e0f59_Out_2;
            Unity_Multiply_float4_float4((_Clamp_67416635085f4679a8edaf7e5599736b_Out_3.xxxx), _Property_622ceb09352c46548a53b70a6fc45f1d_Out_0, _Multiply_d559fe54b6fd41b19f245be2772e0f59_Out_2);
            float4 _Property_9d096fa9ae2a4c0ea1c14d4c54435b82_Out_0 = _Color2;
            float4 _Multiply_b2b056832c7b4a069a3b6e0a21cdb72c_Out_2;
            Unity_Multiply_float4_float4(_Property_9d096fa9ae2a4c0ea1c14d4c54435b82_Out_0, (_Clamp_6e2be452ac754d839d6814ebadccf9a2_Out_3.xxxx), _Multiply_b2b056832c7b4a069a3b6e0a21cdb72c_Out_2);
            float4 _Add_4618a3697ce248dda72574446ab8474d_Out_2;
            Unity_Add_float4(_Multiply_d559fe54b6fd41b19f245be2772e0f59_Out_2, _Multiply_b2b056832c7b4a069a3b6e0a21cdb72c_Out_2, _Add_4618a3697ce248dda72574446ab8474d_Out_2);
            float4 _Add_a9acec8d7b1e46a988127987dff04b7e_Out_2;
            Unity_Add_float4(_Multiply_6bee981ff91049cfbc0df64607ea40e4_Out_2, _Add_4618a3697ce248dda72574446ab8474d_Out_2, _Add_a9acec8d7b1e46a988127987dff04b7e_Out_2);
            float4 _Property_db277f04b3214240a5b63b88827c9bdb_Out_0 = _Color3;
            float _Split_158b255eff6c46079521f9e4063cf9a7_R_1 = _Property_db277f04b3214240a5b63b88827c9bdb_Out_0[0];
            float _Split_158b255eff6c46079521f9e4063cf9a7_G_2 = _Property_db277f04b3214240a5b63b88827c9bdb_Out_0[1];
            float _Split_158b255eff6c46079521f9e4063cf9a7_B_3 = _Property_db277f04b3214240a5b63b88827c9bdb_Out_0[2];
            float _Split_158b255eff6c46079521f9e4063cf9a7_A_4 = _Property_db277f04b3214240a5b63b88827c9bdb_Out_0[3];
            float _Multiply_447a7a5de2474f4f8fb6aef8b5136703_Out_2;
            Unity_Multiply_float_float(_Split_158b255eff6c46079521f9e4063cf9a7_A_4, _Subtract_097a2820382d412aaa4aa080ebf00415_Out_2, _Multiply_447a7a5de2474f4f8fb6aef8b5136703_Out_2);
            float _Split_ca9180fd69894f5693c768e390e11c91_R_1 = _Property_622ceb09352c46548a53b70a6fc45f1d_Out_0[0];
            float _Split_ca9180fd69894f5693c768e390e11c91_G_2 = _Property_622ceb09352c46548a53b70a6fc45f1d_Out_0[1];
            float _Split_ca9180fd69894f5693c768e390e11c91_B_3 = _Property_622ceb09352c46548a53b70a6fc45f1d_Out_0[2];
            float _Split_ca9180fd69894f5693c768e390e11c91_A_4 = _Property_622ceb09352c46548a53b70a6fc45f1d_Out_0[3];
            float _Multiply_de88bf5a21c24c4686f7d8a99775057c_Out_2;
            Unity_Multiply_float_float(_Split_ca9180fd69894f5693c768e390e11c91_A_4, _Clamp_67416635085f4679a8edaf7e5599736b_Out_3, _Multiply_de88bf5a21c24c4686f7d8a99775057c_Out_2);
            float _Split_3d3c14e1a76f482fa7e6c0dcc2417cca_R_1 = _Property_9d096fa9ae2a4c0ea1c14d4c54435b82_Out_0[0];
            float _Split_3d3c14e1a76f482fa7e6c0dcc2417cca_G_2 = _Property_9d096fa9ae2a4c0ea1c14d4c54435b82_Out_0[1];
            float _Split_3d3c14e1a76f482fa7e6c0dcc2417cca_B_3 = _Property_9d096fa9ae2a4c0ea1c14d4c54435b82_Out_0[2];
            float _Split_3d3c14e1a76f482fa7e6c0dcc2417cca_A_4 = _Property_9d096fa9ae2a4c0ea1c14d4c54435b82_Out_0[3];
            float _Multiply_4a2367b118014a76a1c9d4708056a397_Out_2;
            Unity_Multiply_float_float(_Split_3d3c14e1a76f482fa7e6c0dcc2417cca_A_4, _Clamp_6e2be452ac754d839d6814ebadccf9a2_Out_3, _Multiply_4a2367b118014a76a1c9d4708056a397_Out_2);
            float _Add_573d0817211c4389ad1563fc72fe559c_Out_2;
            Unity_Add_float(_Multiply_de88bf5a21c24c4686f7d8a99775057c_Out_2, _Multiply_4a2367b118014a76a1c9d4708056a397_Out_2, _Add_573d0817211c4389ad1563fc72fe559c_Out_2);
            float _Clamp_73d8e09f6c9445e3b5fa29aac199262a_Out_3;
            Unity_Clamp_float(_Add_573d0817211c4389ad1563fc72fe559c_Out_2, 0, 1, _Clamp_73d8e09f6c9445e3b5fa29aac199262a_Out_3);
            float _Add_f6c193950ce545a0a85bece70428e994_Out_2;
            Unity_Add_float(_Multiply_447a7a5de2474f4f8fb6aef8b5136703_Out_2, _Clamp_73d8e09f6c9445e3b5fa29aac199262a_Out_3, _Add_f6c193950ce545a0a85bece70428e994_Out_2);
            float _Clamp_7adfbc1bbe584e8ba3da96a97701d98a_Out_3;
            Unity_Clamp_float(_Add_f6c193950ce545a0a85bece70428e994_Out_2, 0, 1, _Clamp_7adfbc1bbe584e8ba3da96a97701d98a_Out_3);
            surface.BaseColor = (_Add_a9acec8d7b1e46a988127987dff04b7e_Out_2.xyz);
            surface.Alpha = _Clamp_7adfbc1bbe584e8ba3da96a97701d98a_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
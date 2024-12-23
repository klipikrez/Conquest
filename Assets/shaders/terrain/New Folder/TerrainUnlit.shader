Shader "Unlit Terrain Shader"
{
    Properties
    {
        _Normal_Value_1("Normal Value 1", Range(0, 1)) = 0.25
        _Normal_Value_2("Normal Value 2", Range(0, 1)) = 0.75
        _Smoothing("Smoothing", Range(0, 1)) = 0.1
        [NoScaleOffset]_Grass("Grass", 2D) = "white" {}
        Specular1("GrassColor", Color) = (0.6745098, 0.8666667, 0.6901961, 1)
        _Tiling_Grass("Tiling Grass", Range(0, 0.25)) = 0.04
        [NoScaleOffset]_Path("Path", 2D) = "white" {}
        Specular2("PathColor", Color) = (0.7921569, 0.7450981, 0.6, 1)
        _Tiling_Path("Tiling Path", Range(0, 0.25)) = 0.04
        [NoScaleOffset]_Dirt("Dirt", 2D) = "white" {}
        _Tiling_Dirt("Tiling Dirt", Range(0, 0.3)) = 0.3
        [NoScaleOffset]_Rock("Rock", 2D) = "white" {}
        _Rock_Tiling("Rock Tiling", Float) = 0.25
        [NoScaleOffset]_Control0("Control0", 2D) = "white" {}
        [NoScaleOffset]_Control1("Control1", 2D) = "white" {}
        [NoScaleOffset]_Splat3("Splat (3)", 2D) = "white" {}
        [NoScaleOffset]_Splat4("Splat (4)", 2D) = "white" {}
        [NoScaleOffset]_Splat5("Splat (5)", 2D) = "white" {}
        [NoScaleOffset]_Splat1("Splat (1)", 2D) = "white" {}
        [NoScaleOffset]_Splat2("Splat (2)", 2D) = "white" {}
        [NoScaleOffset]_Splat6("Splat (6)", 2D) = "white" {}
        [NoScaleOffset]_Splat7("Splat (7)", 2D) = "white" {}
        [NoScaleOffset]_Splat0("Splat", 2D) = "white" {}
        _Splat3_ST("Layer_ST (3)", Vector) = (0, 0, 0, 0)
        _Splat4_ST("Layer_ST (4)", Vector) = (0, 0, 0, 0)
        _Splat5_ST("Layer_ST (5)", Vector) = (0, 0, 0, 0)
        _Splat1_ST("Layer_ST (1)", Vector) = (0, 0, 0, 0)
        _Splat2_ST("Layer_ST (2)", Vector) = (0, 0, 0, 0)
        _Splat6_ST("Layer_ST (6)", Vector) = (0, 0, 0, 0)
        _Splat7_ST("Layer_ST (7)", Vector) = (0, 0, 0, 0)
        _Splat0_ST("Layer_ST", Vector) = (0, 0, 0, 0)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
            "SplatCount" = "8"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
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
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
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
             float3 WorldSpaceNormal;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
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
             float3 interp1 : INTERP1;
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
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
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
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Comparison_Equal_float(float A, float B, out float Out)
        {
            Out = A == B ? 1 : 0;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float
        {
        half4 uv0;
        };
        
        void SG_ControllerMix_789a062abd3cb714094b91068e12686a_float(float3 _col, UnityTexture2D _contoll, UnityTexture2D _splat1, UnityTexture2D _splat2, UnityTexture2D _splat3, UnityTexture2D _splat4, float4 _sizeOffset_1, float4 _sizeOffset_2, float4 _sizeOffset_3, float4 _sizeOffset_4, Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float IN, out float3 Out_Color_1)
        {
        float3 _Property_1ec6d1284e6945698c759c1b7135e1c2_Out_0 = _col;
        UnityTexture2D _Property_ebc7c076f7854c06b432c72fd62b140c_Out_0 = _splat1;
        float4 _Property_7df6606c80b648639d350473f71d806d_Out_0 = _sizeOffset_1;
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_R_1 = _Property_7df6606c80b648639d350473f71d806d_Out_0[0];
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_G_2 = _Property_7df6606c80b648639d350473f71d806d_Out_0[1];
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_B_3 = _Property_7df6606c80b648639d350473f71d806d_Out_0[2];
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_A_4 = _Property_7df6606c80b648639d350473f71d806d_Out_0[3];
        float2 _Vector2_24616485ceec4debb33a000ff5a117fa_Out_0 = float2(_Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_R_1, _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_G_2);
        float4 _UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0 = IN.uv0;
        float2 _Multiply_761e4e9a169e45c488e88e96a21b8cac_Out_2;
        Unity_Multiply_float2_float2(_Vector2_24616485ceec4debb33a000ff5a117fa_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_761e4e9a169e45c488e88e96a21b8cac_Out_2);
        float4 _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ebc7c076f7854c06b432c72fd62b140c_Out_0.tex, _Property_ebc7c076f7854c06b432c72fd62b140c_Out_0.samplerstate, _Property_ebc7c076f7854c06b432c72fd62b140c_Out_0.GetTransformedUV(_Multiply_761e4e9a169e45c488e88e96a21b8cac_Out_2));
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_R_4 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.r;
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_G_5 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.g;
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_B_6 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.b;
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_A_7 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.a;
        UnityTexture2D _Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0 = _contoll;
        float4 _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0.tex, _Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0.samplerstate, _Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0.GetTransformedUV(IN.uv0.xy));
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_R_4 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.r;
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_G_5 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.g;
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_B_6 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.b;
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_A_7 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.a;
        float3 _Lerp_89ef354dfde849828ce213dec26ab9a6_Out_3;
        Unity_Lerp_float3(_Property_1ec6d1284e6945698c759c1b7135e1c2_Out_0, (_SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_R_4.xxx), _Lerp_89ef354dfde849828ce213dec26ab9a6_Out_3);
        UnityTexture2D _Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0 = _splat2;
        float4 _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0 = _sizeOffset_2;
        float _Split_545e09a0d0c243bbb0c811e2037e0572_R_1 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[0];
        float _Split_545e09a0d0c243bbb0c811e2037e0572_G_2 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[1];
        float _Split_545e09a0d0c243bbb0c811e2037e0572_B_3 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[2];
        float _Split_545e09a0d0c243bbb0c811e2037e0572_A_4 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[3];
        float2 _Vector2_348fe07611c5478e8dd3df1d38e6114c_Out_0 = float2(_Split_545e09a0d0c243bbb0c811e2037e0572_R_1, _Split_545e09a0d0c243bbb0c811e2037e0572_G_2);
        float2 _Multiply_634de5058f134525a0c6f7646d6f1bc3_Out_2;
        Unity_Multiply_float2_float2(_Vector2_348fe07611c5478e8dd3df1d38e6114c_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_634de5058f134525a0c6f7646d6f1bc3_Out_2);
        float4 _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0.tex, _Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0.samplerstate, _Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0.GetTransformedUV(_Multiply_634de5058f134525a0c6f7646d6f1bc3_Out_2));
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_R_4 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.r;
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_G_5 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.g;
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_B_6 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.b;
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_A_7 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.a;
        float3 _Lerp_df0a0a3abcbe488ea1e553619957770a_Out_3;
        Unity_Lerp_float3(_Lerp_89ef354dfde849828ce213dec26ab9a6_Out_3, (_SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_G_5.xxx), _Lerp_df0a0a3abcbe488ea1e553619957770a_Out_3);
        UnityTexture2D _Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0 = _splat3;
        float4 _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0 = _sizeOffset_3;
        float _Split_e869d92ba68545168e10163ed6459c9e_R_1 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[0];
        float _Split_e869d92ba68545168e10163ed6459c9e_G_2 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[1];
        float _Split_e869d92ba68545168e10163ed6459c9e_B_3 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[2];
        float _Split_e869d92ba68545168e10163ed6459c9e_A_4 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[3];
        float2 _Vector2_5e65112bca354775a844a7b6ea4739ef_Out_0 = float2(_Split_e869d92ba68545168e10163ed6459c9e_R_1, _Split_e869d92ba68545168e10163ed6459c9e_G_2);
        float2 _Multiply_6d8e0c28e6674b35adec47d93b4c7423_Out_2;
        Unity_Multiply_float2_float2(_Vector2_5e65112bca354775a844a7b6ea4739ef_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_6d8e0c28e6674b35adec47d93b4c7423_Out_2);
        float4 _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0.tex, _Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0.samplerstate, _Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0.GetTransformedUV(_Multiply_6d8e0c28e6674b35adec47d93b4c7423_Out_2));
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_R_4 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.r;
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_G_5 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.g;
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_B_6 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.b;
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_A_7 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.a;
        float3 _Lerp_b3050b2d97954ef384fd4781e97b470a_Out_3;
        Unity_Lerp_float3(_Lerp_df0a0a3abcbe488ea1e553619957770a_Out_3, (_SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_B_6.xxx), _Lerp_b3050b2d97954ef384fd4781e97b470a_Out_3);
        UnityTexture2D _Property_fcba5df497d942de953e09cc396a2f4d_Out_0 = _splat4;
        float4 _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0 = _sizeOffset_4;
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_R_1 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[0];
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_G_2 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[1];
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_B_3 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[2];
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_A_4 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[3];
        float2 _Vector2_3154ae2be6204c44b1a892f8cca0266d_Out_0 = float2(_Split_268fccfe6f6d4e959b6d91727d99ec4f_R_1, _Split_268fccfe6f6d4e959b6d91727d99ec4f_G_2);
        float2 _Multiply_2271f10e688c4ef4959b98f7c990bcfa_Out_2;
        Unity_Multiply_float2_float2(_Vector2_3154ae2be6204c44b1a892f8cca0266d_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_2271f10e688c4ef4959b98f7c990bcfa_Out_2);
        float4 _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_fcba5df497d942de953e09cc396a2f4d_Out_0.tex, _Property_fcba5df497d942de953e09cc396a2f4d_Out_0.samplerstate, _Property_fcba5df497d942de953e09cc396a2f4d_Out_0.GetTransformedUV(_Multiply_2271f10e688c4ef4959b98f7c990bcfa_Out_2));
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_R_4 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.r;
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_G_5 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.g;
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_B_6 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.b;
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_A_7 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.a;
        float3 _Lerp_d4139ea025214845b57a5ef101476d62_Out_3;
        Unity_Lerp_float3(_Lerp_b3050b2d97954ef384fd4781e97b470a_Out_3, (_SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_A_7.xxx), _Lerp_d4139ea025214845b57a5ef101476d62_Out_3);
        Out_Color_1 = _Lerp_d4139ea025214845b57a5ef101476d62_Out_3;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_420450d5aa9142f687647c66648e6411_Out_0 = _Normal_Value_1;
            float _Property_60983b769e554385be6b6c9e7804ba8e_Out_0 = _Smoothing;
            float _Subtract_83a5f0be4d2f450cbc81c18ce9d89ad8_Out_2;
            Unity_Subtract_float(_Property_420450d5aa9142f687647c66648e6411_Out_0, _Property_60983b769e554385be6b6c9e7804ba8e_Out_0, _Subtract_83a5f0be4d2f450cbc81c18ce9d89ad8_Out_2);
            float _Add_2baff183e9a64dbca1d10eb74c9bbe8a_Out_2;
            Unity_Add_float(_Property_420450d5aa9142f687647c66648e6411_Out_0, _Property_60983b769e554385be6b6c9e7804ba8e_Out_0, _Add_2baff183e9a64dbca1d10eb74c9bbe8a_Out_2);
            float _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2;
            Unity_DotProduct_float3(float3(0, 1, 0), IN.WorldSpaceNormal, _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2);
            float _Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3;
            Unity_Smoothstep_float(_Subtract_83a5f0be4d2f450cbc81c18ce9d89ad8_Out_2, _Add_2baff183e9a64dbca1d10eb74c9bbe8a_Out_2, _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2, _Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3);
            float _Comparison_871fdf936b87482a8da6cc29098397a3_Out_2;
            Unity_Comparison_Equal_float(1, _Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3, _Comparison_871fdf936b87482a8da6cc29098397a3_Out_2);
            UnityTexture2D _Property_f91edbfe84304c67b45ece63579af760_Out_0 = UnityBuildTexture2DStructNoScale(_Dirt);
            float _Property_a3278f532c8a4b909321353f4991b5fb_Out_0 = _Tiling_Dirt;
            float3 Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV = IN.AbsoluteWorldSpacePosition * _Property_a3278f532c8a4b909321353f4991b5fb_Out_0;
            float3 Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(1, floor(log2(Min_float())/log2(1/sqrt(3)))) );
            Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend /= dot(Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend, 1.0);
            float4 Triplanar_5408bf1bf7854520ae56b756dbf837e5_X = SAMPLE_TEXTURE2D(_Property_f91edbfe84304c67b45ece63579af760_Out_0.tex, _Property_f91edbfe84304c67b45ece63579af760_Out_0.samplerstate, Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV.zy);
            float4 Triplanar_5408bf1bf7854520ae56b756dbf837e5_Y = SAMPLE_TEXTURE2D(_Property_f91edbfe84304c67b45ece63579af760_Out_0.tex, _Property_f91edbfe84304c67b45ece63579af760_Out_0.samplerstate, Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV.xz);
            float4 Triplanar_5408bf1bf7854520ae56b756dbf837e5_Z = SAMPLE_TEXTURE2D(_Property_f91edbfe84304c67b45ece63579af760_Out_0.tex, _Property_f91edbfe84304c67b45ece63579af760_Out_0.samplerstate, Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV.xy);
            float4 _Triplanar_5408bf1bf7854520ae56b756dbf837e5_Out_0 = Triplanar_5408bf1bf7854520ae56b756dbf837e5_X * Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend.x + Triplanar_5408bf1bf7854520ae56b756dbf837e5_Y * Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend.y + Triplanar_5408bf1bf7854520ae56b756dbf837e5_Z * Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend.z;
            UnityTexture2D _Property_d96353e685934444aaee5714b7160acd_Out_0 = UnityBuildTexture2DStructNoScale(_Control0);
            UnityTexture2D _Property_2e9aa4a97c7e4e398647999d935d814f_Out_0 = UnityBuildTexture2DStructNoScale(_Splat0);
            UnityTexture2D _Property_0374ee016f724455872b7e662814ff61_Out_0 = UnityBuildTexture2DStructNoScale(_Splat1);
            UnityTexture2D _Property_ba4b2aa6270f4434b5530b96c2ebe6c6_Out_0 = UnityBuildTexture2DStructNoScale(_Splat2);
            UnityTexture2D _Property_82eb16c8d2a9441ab8ec1aedca498c37_Out_0 = UnityBuildTexture2DStructNoScale(_Splat3);
            float4 _Property_68e6c6dca5bb46059e5a89d8db9d0030_Out_0 = _Splat0_ST;
            float4 _Property_08c94a165b9a49408f787fdfa3462733_Out_0 = _Splat1_ST;
            float4 _Property_31a569f8fb094143a8893062a9dde399_Out_0 = _Splat2_ST;
            float4 _Property_156866fff20b469491a17d8a710c5315_Out_0 = _Splat3_ST;
            Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float _ControllerMix_65640c8d4be64313a4a62f235f3a19c2;
            _ControllerMix_65640c8d4be64313a4a62f235f3a19c2.uv0 = IN.uv0;
            float3 _ControllerMix_65640c8d4be64313a4a62f235f3a19c2_OutColor_1;
            SG_ControllerMix_789a062abd3cb714094b91068e12686a_float(float3 (0, 0, 0), _Property_d96353e685934444aaee5714b7160acd_Out_0, _Property_2e9aa4a97c7e4e398647999d935d814f_Out_0, _Property_0374ee016f724455872b7e662814ff61_Out_0, _Property_ba4b2aa6270f4434b5530b96c2ebe6c6_Out_0, _Property_82eb16c8d2a9441ab8ec1aedca498c37_Out_0, _Property_68e6c6dca5bb46059e5a89d8db9d0030_Out_0, _Property_08c94a165b9a49408f787fdfa3462733_Out_0, _Property_31a569f8fb094143a8893062a9dde399_Out_0, _Property_156866fff20b469491a17d8a710c5315_Out_0, _ControllerMix_65640c8d4be64313a4a62f235f3a19c2, _ControllerMix_65640c8d4be64313a4a62f235f3a19c2_OutColor_1);
            UnityTexture2D _Property_26cfb7ef44ef4ecbbae6eaa7c38a387c_Out_0 = UnityBuildTexture2DStructNoScale(_Control1);
            UnityTexture2D _Property_b63e31a041ab44d78272fc2c7f74384c_Out_0 = UnityBuildTexture2DStructNoScale(_Splat4);
            UnityTexture2D _Property_e8f648728a684668a15bcd9568d5f754_Out_0 = UnityBuildTexture2DStructNoScale(_Splat5);
            UnityTexture2D _Property_562e44a60dfc44d88a62ca3e9d510628_Out_0 = UnityBuildTexture2DStructNoScale(_Splat6);
            UnityTexture2D _Property_41f48443ebc24f98b6aab69d353422e2_Out_0 = UnityBuildTexture2DStructNoScale(_Splat7);
            float4 _Property_8b3a05064f3c48dc85a60d419e482454_Out_0 = _Splat4_ST;
            float4 _Property_1bd77fc1441c4c329ec15759affd2b03_Out_0 = _Splat5_ST;
            float4 _Property_8d7f0c8e1025405dbeff4f51303eb328_Out_0 = _Splat6_ST;
            float4 _Property_612718f17a9240ad9abf358734f1e20b_Out_0 = _Splat7_ST;
            Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float _ControllerMix_1a8109716ccf4d9c921a059d826e11fd;
            _ControllerMix_1a8109716ccf4d9c921a059d826e11fd.uv0 = IN.uv0;
            float3 _ControllerMix_1a8109716ccf4d9c921a059d826e11fd_OutColor_1;
            SG_ControllerMix_789a062abd3cb714094b91068e12686a_float(_ControllerMix_65640c8d4be64313a4a62f235f3a19c2_OutColor_1, _Property_26cfb7ef44ef4ecbbae6eaa7c38a387c_Out_0, _Property_b63e31a041ab44d78272fc2c7f74384c_Out_0, _Property_e8f648728a684668a15bcd9568d5f754_Out_0, _Property_562e44a60dfc44d88a62ca3e9d510628_Out_0, _Property_41f48443ebc24f98b6aab69d353422e2_Out_0, _Property_8b3a05064f3c48dc85a60d419e482454_Out_0, _Property_1bd77fc1441c4c329ec15759affd2b03_Out_0, _Property_8d7f0c8e1025405dbeff4f51303eb328_Out_0, _Property_612718f17a9240ad9abf358734f1e20b_Out_0, _ControllerMix_1a8109716ccf4d9c921a059d826e11fd, _ControllerMix_1a8109716ccf4d9c921a059d826e11fd_OutColor_1);
            float _Property_1db37156eff740978079ff18f2f0ce9e_Out_0 = _Normal_Value_2;
            float _Property_9fcd4cd8f74d45de92a594f90d18c31b_Out_0 = _Smoothing;
            float _Subtract_e65022bc4bb3407099e74f2b7761c6c7_Out_2;
            Unity_Subtract_float(_Property_1db37156eff740978079ff18f2f0ce9e_Out_0, _Property_9fcd4cd8f74d45de92a594f90d18c31b_Out_0, _Subtract_e65022bc4bb3407099e74f2b7761c6c7_Out_2);
            float _Add_c4c66b3985164a2f9695583bacdbeeba_Out_2;
            Unity_Add_float(_Property_1db37156eff740978079ff18f2f0ce9e_Out_0, _Property_9fcd4cd8f74d45de92a594f90d18c31b_Out_0, _Add_c4c66b3985164a2f9695583bacdbeeba_Out_2);
            float _Smoothstep_6b1f2336ed724b0e9cca87c59f50a9e0_Out_3;
            Unity_Smoothstep_float(_Subtract_e65022bc4bb3407099e74f2b7761c6c7_Out_2, _Add_c4c66b3985164a2f9695583bacdbeeba_Out_2, _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2, _Smoothstep_6b1f2336ed724b0e9cca87c59f50a9e0_Out_3);
            float3 _Lerp_4863677f9d234d149c2ad8f86ac3b2c0_Out_3;
            Unity_Lerp_float3((_Triplanar_5408bf1bf7854520ae56b756dbf837e5_Out_0.xyz), _ControllerMix_1a8109716ccf4d9c921a059d826e11fd_OutColor_1, (_Smoothstep_6b1f2336ed724b0e9cca87c59f50a9e0_Out_3.xxx), _Lerp_4863677f9d234d149c2ad8f86ac3b2c0_Out_3);
            UnityTexture2D _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0 = UnityBuildTexture2DStructNoScale(_Rock);
            float _Property_79b6fad216144d9abc8c0a718bb1b0c3_Out_0 = _Rock_Tiling;
            float3 Triplanar_4d429963ec32436f9aaff3911609be4f_UV = IN.AbsoluteWorldSpacePosition * _Property_79b6fad216144d9abc8c0a718bb1b0c3_Out_0;
            float3 Triplanar_4d429963ec32436f9aaff3911609be4f_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(1, floor(log2(Min_float())/log2(1/sqrt(3)))) );
            Triplanar_4d429963ec32436f9aaff3911609be4f_Blend /= dot(Triplanar_4d429963ec32436f9aaff3911609be4f_Blend, 1.0);
            float4 Triplanar_4d429963ec32436f9aaff3911609be4f_X = SAMPLE_TEXTURE2D(_Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.tex, _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.samplerstate, Triplanar_4d429963ec32436f9aaff3911609be4f_UV.zy);
            float4 Triplanar_4d429963ec32436f9aaff3911609be4f_Y = SAMPLE_TEXTURE2D(_Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.tex, _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.samplerstate, Triplanar_4d429963ec32436f9aaff3911609be4f_UV.xz);
            float4 Triplanar_4d429963ec32436f9aaff3911609be4f_Z = SAMPLE_TEXTURE2D(_Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.tex, _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.samplerstate, Triplanar_4d429963ec32436f9aaff3911609be4f_UV.xy);
            float4 _Triplanar_4d429963ec32436f9aaff3911609be4f_Out_0 = Triplanar_4d429963ec32436f9aaff3911609be4f_X * Triplanar_4d429963ec32436f9aaff3911609be4f_Blend.x + Triplanar_4d429963ec32436f9aaff3911609be4f_Y * Triplanar_4d429963ec32436f9aaff3911609be4f_Blend.y + Triplanar_4d429963ec32436f9aaff3911609be4f_Z * Triplanar_4d429963ec32436f9aaff3911609be4f_Blend.z;
            float4 _Lerp_5349781ce30444538b2a6169481666b2_Out_3;
            Unity_Lerp_float4(_Triplanar_4d429963ec32436f9aaff3911609be4f_Out_0, _Triplanar_5408bf1bf7854520ae56b756dbf837e5_Out_0, (_Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3.xxxx), _Lerp_5349781ce30444538b2a6169481666b2_Out_3);
            float3 _Branch_2953d04323a94eabb636e8f3d6ecbdea_Out_3;
            Unity_Branch_float3(_Comparison_871fdf936b87482a8da6cc29098397a3_Out_2, _Lerp_4863677f9d234d149c2ad8f86ac3b2c0_Out_3, (_Lerp_5349781ce30444538b2a6169481666b2_Out_3.xyz), _Branch_2953d04323a94eabb636e8f3d6ecbdea_Out_3);
            surface.BaseColor = _Branch_2953d04323a94eabb636e8f3d6ecbdea_Out_3;
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
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask R
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
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
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
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
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
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
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
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
            output.interp0.xyz =  input.normalWS;
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
            output.normalWS = input.interp0.xyz;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
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
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
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
             float3 WorldSpaceNormal;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
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
             float3 interp1 : INTERP1;
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
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
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
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Comparison_Equal_float(float A, float B, out float Out)
        {
            Out = A == B ? 1 : 0;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float
        {
        half4 uv0;
        };
        
        void SG_ControllerMix_789a062abd3cb714094b91068e12686a_float(float3 _col, UnityTexture2D _contoll, UnityTexture2D _splat1, UnityTexture2D _splat2, UnityTexture2D _splat3, UnityTexture2D _splat4, float4 _sizeOffset_1, float4 _sizeOffset_2, float4 _sizeOffset_3, float4 _sizeOffset_4, Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float IN, out float3 Out_Color_1)
        {
        float3 _Property_1ec6d1284e6945698c759c1b7135e1c2_Out_0 = _col;
        UnityTexture2D _Property_ebc7c076f7854c06b432c72fd62b140c_Out_0 = _splat1;
        float4 _Property_7df6606c80b648639d350473f71d806d_Out_0 = _sizeOffset_1;
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_R_1 = _Property_7df6606c80b648639d350473f71d806d_Out_0[0];
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_G_2 = _Property_7df6606c80b648639d350473f71d806d_Out_0[1];
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_B_3 = _Property_7df6606c80b648639d350473f71d806d_Out_0[2];
        float _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_A_4 = _Property_7df6606c80b648639d350473f71d806d_Out_0[3];
        float2 _Vector2_24616485ceec4debb33a000ff5a117fa_Out_0 = float2(_Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_R_1, _Split_5a7127bbe07b4a0fbb6f9e163d5bd7bb_G_2);
        float4 _UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0 = IN.uv0;
        float2 _Multiply_761e4e9a169e45c488e88e96a21b8cac_Out_2;
        Unity_Multiply_float2_float2(_Vector2_24616485ceec4debb33a000ff5a117fa_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_761e4e9a169e45c488e88e96a21b8cac_Out_2);
        float4 _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ebc7c076f7854c06b432c72fd62b140c_Out_0.tex, _Property_ebc7c076f7854c06b432c72fd62b140c_Out_0.samplerstate, _Property_ebc7c076f7854c06b432c72fd62b140c_Out_0.GetTransformedUV(_Multiply_761e4e9a169e45c488e88e96a21b8cac_Out_2));
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_R_4 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.r;
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_G_5 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.g;
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_B_6 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.b;
        float _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_A_7 = _SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.a;
        UnityTexture2D _Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0 = _contoll;
        float4 _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0.tex, _Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0.samplerstate, _Property_b08f800d5a6442bba5cb96bf9e0fb1ec_Out_0.GetTransformedUV(IN.uv0.xy));
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_R_4 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.r;
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_G_5 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.g;
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_B_6 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.b;
        float _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_A_7 = _SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_RGBA_0.a;
        float3 _Lerp_89ef354dfde849828ce213dec26ab9a6_Out_3;
        Unity_Lerp_float3(_Property_1ec6d1284e6945698c759c1b7135e1c2_Out_0, (_SampleTexture2D_364b0275cfe84eb29c0b0ef38c907927_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_R_4.xxx), _Lerp_89ef354dfde849828ce213dec26ab9a6_Out_3);
        UnityTexture2D _Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0 = _splat2;
        float4 _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0 = _sizeOffset_2;
        float _Split_545e09a0d0c243bbb0c811e2037e0572_R_1 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[0];
        float _Split_545e09a0d0c243bbb0c811e2037e0572_G_2 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[1];
        float _Split_545e09a0d0c243bbb0c811e2037e0572_B_3 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[2];
        float _Split_545e09a0d0c243bbb0c811e2037e0572_A_4 = _Property_470e35e4f65b4b6c979d8b507b6b3607_Out_0[3];
        float2 _Vector2_348fe07611c5478e8dd3df1d38e6114c_Out_0 = float2(_Split_545e09a0d0c243bbb0c811e2037e0572_R_1, _Split_545e09a0d0c243bbb0c811e2037e0572_G_2);
        float2 _Multiply_634de5058f134525a0c6f7646d6f1bc3_Out_2;
        Unity_Multiply_float2_float2(_Vector2_348fe07611c5478e8dd3df1d38e6114c_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_634de5058f134525a0c6f7646d6f1bc3_Out_2);
        float4 _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0.tex, _Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0.samplerstate, _Property_e017b39810ce4a7e8cbe36e5d5cbd3da_Out_0.GetTransformedUV(_Multiply_634de5058f134525a0c6f7646d6f1bc3_Out_2));
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_R_4 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.r;
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_G_5 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.g;
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_B_6 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.b;
        float _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_A_7 = _SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.a;
        float3 _Lerp_df0a0a3abcbe488ea1e553619957770a_Out_3;
        Unity_Lerp_float3(_Lerp_89ef354dfde849828ce213dec26ab9a6_Out_3, (_SampleTexture2D_2c043d13f11e461ba9b630343a7e82e1_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_G_5.xxx), _Lerp_df0a0a3abcbe488ea1e553619957770a_Out_3);
        UnityTexture2D _Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0 = _splat3;
        float4 _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0 = _sizeOffset_3;
        float _Split_e869d92ba68545168e10163ed6459c9e_R_1 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[0];
        float _Split_e869d92ba68545168e10163ed6459c9e_G_2 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[1];
        float _Split_e869d92ba68545168e10163ed6459c9e_B_3 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[2];
        float _Split_e869d92ba68545168e10163ed6459c9e_A_4 = _Property_b99b5ddcfe1c4ca98b2609d96a239d89_Out_0[3];
        float2 _Vector2_5e65112bca354775a844a7b6ea4739ef_Out_0 = float2(_Split_e869d92ba68545168e10163ed6459c9e_R_1, _Split_e869d92ba68545168e10163ed6459c9e_G_2);
        float2 _Multiply_6d8e0c28e6674b35adec47d93b4c7423_Out_2;
        Unity_Multiply_float2_float2(_Vector2_5e65112bca354775a844a7b6ea4739ef_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_6d8e0c28e6674b35adec47d93b4c7423_Out_2);
        float4 _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0.tex, _Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0.samplerstate, _Property_e56c69a5bd4e446a820bc59fe875fdb1_Out_0.GetTransformedUV(_Multiply_6d8e0c28e6674b35adec47d93b4c7423_Out_2));
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_R_4 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.r;
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_G_5 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.g;
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_B_6 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.b;
        float _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_A_7 = _SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.a;
        float3 _Lerp_b3050b2d97954ef384fd4781e97b470a_Out_3;
        Unity_Lerp_float3(_Lerp_df0a0a3abcbe488ea1e553619957770a_Out_3, (_SampleTexture2D_b3c1d06e96d24947b5455cb14fb0d75a_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_B_6.xxx), _Lerp_b3050b2d97954ef384fd4781e97b470a_Out_3);
        UnityTexture2D _Property_fcba5df497d942de953e09cc396a2f4d_Out_0 = _splat4;
        float4 _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0 = _sizeOffset_4;
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_R_1 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[0];
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_G_2 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[1];
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_B_3 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[2];
        float _Split_268fccfe6f6d4e959b6d91727d99ec4f_A_4 = _Property_46fca82e758749d4845bbbd4c81f2ea9_Out_0[3];
        float2 _Vector2_3154ae2be6204c44b1a892f8cca0266d_Out_0 = float2(_Split_268fccfe6f6d4e959b6d91727d99ec4f_R_1, _Split_268fccfe6f6d4e959b6d91727d99ec4f_G_2);
        float2 _Multiply_2271f10e688c4ef4959b98f7c990bcfa_Out_2;
        Unity_Multiply_float2_float2(_Vector2_3154ae2be6204c44b1a892f8cca0266d_Out_0, (_UV_2a3a354bddd24383a958e3fa6bd37bec_Out_0.xy), _Multiply_2271f10e688c4ef4959b98f7c990bcfa_Out_2);
        float4 _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_fcba5df497d942de953e09cc396a2f4d_Out_0.tex, _Property_fcba5df497d942de953e09cc396a2f4d_Out_0.samplerstate, _Property_fcba5df497d942de953e09cc396a2f4d_Out_0.GetTransformedUV(_Multiply_2271f10e688c4ef4959b98f7c990bcfa_Out_2));
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_R_4 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.r;
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_G_5 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.g;
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_B_6 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.b;
        float _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_A_7 = _SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.a;
        float3 _Lerp_d4139ea025214845b57a5ef101476d62_Out_3;
        Unity_Lerp_float3(_Lerp_b3050b2d97954ef384fd4781e97b470a_Out_3, (_SampleTexture2D_b967c56da3f040f386d648579bdc5f5a_RGBA_0.xyz), (_SampleTexture2D_7d4e06461a3e4f07a43dc6c47da2272d_A_7.xxx), _Lerp_d4139ea025214845b57a5ef101476d62_Out_3);
        Out_Color_1 = _Lerp_d4139ea025214845b57a5ef101476d62_Out_3;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_420450d5aa9142f687647c66648e6411_Out_0 = _Normal_Value_1;
            float _Property_60983b769e554385be6b6c9e7804ba8e_Out_0 = _Smoothing;
            float _Subtract_83a5f0be4d2f450cbc81c18ce9d89ad8_Out_2;
            Unity_Subtract_float(_Property_420450d5aa9142f687647c66648e6411_Out_0, _Property_60983b769e554385be6b6c9e7804ba8e_Out_0, _Subtract_83a5f0be4d2f450cbc81c18ce9d89ad8_Out_2);
            float _Add_2baff183e9a64dbca1d10eb74c9bbe8a_Out_2;
            Unity_Add_float(_Property_420450d5aa9142f687647c66648e6411_Out_0, _Property_60983b769e554385be6b6c9e7804ba8e_Out_0, _Add_2baff183e9a64dbca1d10eb74c9bbe8a_Out_2);
            float _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2;
            Unity_DotProduct_float3(float3(0, 1, 0), IN.WorldSpaceNormal, _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2);
            float _Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3;
            Unity_Smoothstep_float(_Subtract_83a5f0be4d2f450cbc81c18ce9d89ad8_Out_2, _Add_2baff183e9a64dbca1d10eb74c9bbe8a_Out_2, _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2, _Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3);
            float _Comparison_871fdf936b87482a8da6cc29098397a3_Out_2;
            Unity_Comparison_Equal_float(1, _Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3, _Comparison_871fdf936b87482a8da6cc29098397a3_Out_2);
            UnityTexture2D _Property_f91edbfe84304c67b45ece63579af760_Out_0 = UnityBuildTexture2DStructNoScale(_Dirt);
            float _Property_a3278f532c8a4b909321353f4991b5fb_Out_0 = _Tiling_Dirt;
            float3 Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV = IN.AbsoluteWorldSpacePosition * _Property_a3278f532c8a4b909321353f4991b5fb_Out_0;
            float3 Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(1, floor(log2(Min_float())/log2(1/sqrt(3)))) );
            Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend /= dot(Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend, 1.0);
            float4 Triplanar_5408bf1bf7854520ae56b756dbf837e5_X = SAMPLE_TEXTURE2D(_Property_f91edbfe84304c67b45ece63579af760_Out_0.tex, _Property_f91edbfe84304c67b45ece63579af760_Out_0.samplerstate, Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV.zy);
            float4 Triplanar_5408bf1bf7854520ae56b756dbf837e5_Y = SAMPLE_TEXTURE2D(_Property_f91edbfe84304c67b45ece63579af760_Out_0.tex, _Property_f91edbfe84304c67b45ece63579af760_Out_0.samplerstate, Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV.xz);
            float4 Triplanar_5408bf1bf7854520ae56b756dbf837e5_Z = SAMPLE_TEXTURE2D(_Property_f91edbfe84304c67b45ece63579af760_Out_0.tex, _Property_f91edbfe84304c67b45ece63579af760_Out_0.samplerstate, Triplanar_5408bf1bf7854520ae56b756dbf837e5_UV.xy);
            float4 _Triplanar_5408bf1bf7854520ae56b756dbf837e5_Out_0 = Triplanar_5408bf1bf7854520ae56b756dbf837e5_X * Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend.x + Triplanar_5408bf1bf7854520ae56b756dbf837e5_Y * Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend.y + Triplanar_5408bf1bf7854520ae56b756dbf837e5_Z * Triplanar_5408bf1bf7854520ae56b756dbf837e5_Blend.z;
            UnityTexture2D _Property_d96353e685934444aaee5714b7160acd_Out_0 = UnityBuildTexture2DStructNoScale(_Control0);
            UnityTexture2D _Property_2e9aa4a97c7e4e398647999d935d814f_Out_0 = UnityBuildTexture2DStructNoScale(_Splat0);
            UnityTexture2D _Property_0374ee016f724455872b7e662814ff61_Out_0 = UnityBuildTexture2DStructNoScale(_Splat1);
            UnityTexture2D _Property_ba4b2aa6270f4434b5530b96c2ebe6c6_Out_0 = UnityBuildTexture2DStructNoScale(_Splat2);
            UnityTexture2D _Property_82eb16c8d2a9441ab8ec1aedca498c37_Out_0 = UnityBuildTexture2DStructNoScale(_Splat3);
            float4 _Property_68e6c6dca5bb46059e5a89d8db9d0030_Out_0 = _Splat0_ST;
            float4 _Property_08c94a165b9a49408f787fdfa3462733_Out_0 = _Splat1_ST;
            float4 _Property_31a569f8fb094143a8893062a9dde399_Out_0 = _Splat2_ST;
            float4 _Property_156866fff20b469491a17d8a710c5315_Out_0 = _Splat3_ST;
            Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float _ControllerMix_65640c8d4be64313a4a62f235f3a19c2;
            _ControllerMix_65640c8d4be64313a4a62f235f3a19c2.uv0 = IN.uv0;
            float3 _ControllerMix_65640c8d4be64313a4a62f235f3a19c2_OutColor_1;
            SG_ControllerMix_789a062abd3cb714094b91068e12686a_float(float3 (0, 0, 0), _Property_d96353e685934444aaee5714b7160acd_Out_0, _Property_2e9aa4a97c7e4e398647999d935d814f_Out_0, _Property_0374ee016f724455872b7e662814ff61_Out_0, _Property_ba4b2aa6270f4434b5530b96c2ebe6c6_Out_0, _Property_82eb16c8d2a9441ab8ec1aedca498c37_Out_0, _Property_68e6c6dca5bb46059e5a89d8db9d0030_Out_0, _Property_08c94a165b9a49408f787fdfa3462733_Out_0, _Property_31a569f8fb094143a8893062a9dde399_Out_0, _Property_156866fff20b469491a17d8a710c5315_Out_0, _ControllerMix_65640c8d4be64313a4a62f235f3a19c2, _ControllerMix_65640c8d4be64313a4a62f235f3a19c2_OutColor_1);
            UnityTexture2D _Property_26cfb7ef44ef4ecbbae6eaa7c38a387c_Out_0 = UnityBuildTexture2DStructNoScale(_Control1);
            UnityTexture2D _Property_b63e31a041ab44d78272fc2c7f74384c_Out_0 = UnityBuildTexture2DStructNoScale(_Splat4);
            UnityTexture2D _Property_e8f648728a684668a15bcd9568d5f754_Out_0 = UnityBuildTexture2DStructNoScale(_Splat5);
            UnityTexture2D _Property_562e44a60dfc44d88a62ca3e9d510628_Out_0 = UnityBuildTexture2DStructNoScale(_Splat6);
            UnityTexture2D _Property_41f48443ebc24f98b6aab69d353422e2_Out_0 = UnityBuildTexture2DStructNoScale(_Splat7);
            float4 _Property_8b3a05064f3c48dc85a60d419e482454_Out_0 = _Splat4_ST;
            float4 _Property_1bd77fc1441c4c329ec15759affd2b03_Out_0 = _Splat5_ST;
            float4 _Property_8d7f0c8e1025405dbeff4f51303eb328_Out_0 = _Splat6_ST;
            float4 _Property_612718f17a9240ad9abf358734f1e20b_Out_0 = _Splat7_ST;
            Bindings_ControllerMix_789a062abd3cb714094b91068e12686a_float _ControllerMix_1a8109716ccf4d9c921a059d826e11fd;
            _ControllerMix_1a8109716ccf4d9c921a059d826e11fd.uv0 = IN.uv0;
            float3 _ControllerMix_1a8109716ccf4d9c921a059d826e11fd_OutColor_1;
            SG_ControllerMix_789a062abd3cb714094b91068e12686a_float(_ControllerMix_65640c8d4be64313a4a62f235f3a19c2_OutColor_1, _Property_26cfb7ef44ef4ecbbae6eaa7c38a387c_Out_0, _Property_b63e31a041ab44d78272fc2c7f74384c_Out_0, _Property_e8f648728a684668a15bcd9568d5f754_Out_0, _Property_562e44a60dfc44d88a62ca3e9d510628_Out_0, _Property_41f48443ebc24f98b6aab69d353422e2_Out_0, _Property_8b3a05064f3c48dc85a60d419e482454_Out_0, _Property_1bd77fc1441c4c329ec15759affd2b03_Out_0, _Property_8d7f0c8e1025405dbeff4f51303eb328_Out_0, _Property_612718f17a9240ad9abf358734f1e20b_Out_0, _ControllerMix_1a8109716ccf4d9c921a059d826e11fd, _ControllerMix_1a8109716ccf4d9c921a059d826e11fd_OutColor_1);
            float _Property_1db37156eff740978079ff18f2f0ce9e_Out_0 = _Normal_Value_2;
            float _Property_9fcd4cd8f74d45de92a594f90d18c31b_Out_0 = _Smoothing;
            float _Subtract_e65022bc4bb3407099e74f2b7761c6c7_Out_2;
            Unity_Subtract_float(_Property_1db37156eff740978079ff18f2f0ce9e_Out_0, _Property_9fcd4cd8f74d45de92a594f90d18c31b_Out_0, _Subtract_e65022bc4bb3407099e74f2b7761c6c7_Out_2);
            float _Add_c4c66b3985164a2f9695583bacdbeeba_Out_2;
            Unity_Add_float(_Property_1db37156eff740978079ff18f2f0ce9e_Out_0, _Property_9fcd4cd8f74d45de92a594f90d18c31b_Out_0, _Add_c4c66b3985164a2f9695583bacdbeeba_Out_2);
            float _Smoothstep_6b1f2336ed724b0e9cca87c59f50a9e0_Out_3;
            Unity_Smoothstep_float(_Subtract_e65022bc4bb3407099e74f2b7761c6c7_Out_2, _Add_c4c66b3985164a2f9695583bacdbeeba_Out_2, _DotProduct_87977c19922e45078d57f8fbf239fa6d_Out_2, _Smoothstep_6b1f2336ed724b0e9cca87c59f50a9e0_Out_3);
            float3 _Lerp_4863677f9d234d149c2ad8f86ac3b2c0_Out_3;
            Unity_Lerp_float3((_Triplanar_5408bf1bf7854520ae56b756dbf837e5_Out_0.xyz), _ControllerMix_1a8109716ccf4d9c921a059d826e11fd_OutColor_1, (_Smoothstep_6b1f2336ed724b0e9cca87c59f50a9e0_Out_3.xxx), _Lerp_4863677f9d234d149c2ad8f86ac3b2c0_Out_3);
            UnityTexture2D _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0 = UnityBuildTexture2DStructNoScale(_Rock);
            float _Property_79b6fad216144d9abc8c0a718bb1b0c3_Out_0 = _Rock_Tiling;
            float3 Triplanar_4d429963ec32436f9aaff3911609be4f_UV = IN.AbsoluteWorldSpacePosition * _Property_79b6fad216144d9abc8c0a718bb1b0c3_Out_0;
            float3 Triplanar_4d429963ec32436f9aaff3911609be4f_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(1, floor(log2(Min_float())/log2(1/sqrt(3)))) );
            Triplanar_4d429963ec32436f9aaff3911609be4f_Blend /= dot(Triplanar_4d429963ec32436f9aaff3911609be4f_Blend, 1.0);
            float4 Triplanar_4d429963ec32436f9aaff3911609be4f_X = SAMPLE_TEXTURE2D(_Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.tex, _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.samplerstate, Triplanar_4d429963ec32436f9aaff3911609be4f_UV.zy);
            float4 Triplanar_4d429963ec32436f9aaff3911609be4f_Y = SAMPLE_TEXTURE2D(_Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.tex, _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.samplerstate, Triplanar_4d429963ec32436f9aaff3911609be4f_UV.xz);
            float4 Triplanar_4d429963ec32436f9aaff3911609be4f_Z = SAMPLE_TEXTURE2D(_Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.tex, _Property_adea2eb2bdb74b9b90b099171768bd49_Out_0.samplerstate, Triplanar_4d429963ec32436f9aaff3911609be4f_UV.xy);
            float4 _Triplanar_4d429963ec32436f9aaff3911609be4f_Out_0 = Triplanar_4d429963ec32436f9aaff3911609be4f_X * Triplanar_4d429963ec32436f9aaff3911609be4f_Blend.x + Triplanar_4d429963ec32436f9aaff3911609be4f_Y * Triplanar_4d429963ec32436f9aaff3911609be4f_Blend.y + Triplanar_4d429963ec32436f9aaff3911609be4f_Z * Triplanar_4d429963ec32436f9aaff3911609be4f_Blend.z;
            float4 _Lerp_5349781ce30444538b2a6169481666b2_Out_3;
            Unity_Lerp_float4(_Triplanar_4d429963ec32436f9aaff3911609be4f_Out_0, _Triplanar_5408bf1bf7854520ae56b756dbf837e5_Out_0, (_Smoothstep_2acfb48d9db94abc920df8d109f006ba_Out_3.xxxx), _Lerp_5349781ce30444538b2a6169481666b2_Out_3);
            float3 _Branch_2953d04323a94eabb636e8f3d6ecbdea_Out_3;
            Unity_Branch_float3(_Comparison_871fdf936b87482a8da6cc29098397a3_Out_2, _Lerp_4863677f9d234d149c2ad8f86ac3b2c0_Out_3, (_Lerp_5349781ce30444538b2a6169481666b2_Out_3.xyz), _Branch_2953d04323a94eabb636e8f3d6ecbdea_Out_3);
            surface.BaseColor = _Branch_2953d04323a94eabb636e8f3d6ecbdea_Out_3;
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
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask R
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
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
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
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
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
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
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
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
            output.interp0.xyz =  input.normalWS;
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
            output.normalWS = input.interp0.xyz;
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
        float4 _Control0_TexelSize;
        float4 _Control1_TexelSize;
        float4 _Splat3_TexelSize;
        float4 _Splat4_TexelSize;
        float4 _Splat5_TexelSize;
        float4 _Splat1_TexelSize;
        float4 _Splat2_TexelSize;
        float4 _Splat6_TexelSize;
        float4 _Splat7_TexelSize;
        float4 _Splat0_TexelSize;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;
        float4 _Splat0_ST;
        float _Normal_Value_2;
        float _Normal_Value_1;
        float _Smoothing;
        float4 _Grass_TexelSize;
        float _Tiling_Path;
        float4 _Dirt_TexelSize;
        float _Tiling_Dirt;
        float _Tiling_Grass;
        float4 _Rock_TexelSize;
        float _Rock_Tiling;
        float4 _Path_TexelSize;
        float4 Specular1;
        float4 Specular2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Control0);
        SAMPLER(sampler_Control0);
        TEXTURE2D(_Control1);
        SAMPLER(sampler_Control1);
        TEXTURE2D(_Splat3);
        SAMPLER(sampler_Splat3);
        TEXTURE2D(_Splat4);
        SAMPLER(sampler_Splat4);
        TEXTURE2D(_Splat5);
        SAMPLER(sampler_Splat5);
        TEXTURE2D(_Splat1);
        SAMPLER(sampler_Splat1);
        TEXTURE2D(_Splat2);
        SAMPLER(sampler_Splat2);
        TEXTURE2D(_Splat6);
        SAMPLER(sampler_Splat6);
        TEXTURE2D(_Splat7);
        SAMPLER(sampler_Splat7);
        TEXTURE2D(_Splat0);
        SAMPLER(sampler_Splat0);
        TEXTURE2D(_Grass);
        SAMPLER(sampler_Grass);
        TEXTURE2D(_Dirt);
        SAMPLER(sampler_Dirt);
        TEXTURE2D(_Rock);
        SAMPLER(sampler_Rock);
        TEXTURE2D(_Path);
        SAMPLER(sampler_Path);
        
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
        // GraphFunctions: <None>
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
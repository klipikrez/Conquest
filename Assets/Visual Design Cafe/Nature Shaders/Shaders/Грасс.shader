// This shader was created using ShaderX, a shader framework by Visual Design Cafe.
// https://www.visualdesigncafe.com/shaderx
//

Shader "Universal Render Pipeline/Nature Shaders/Грасс"
{
    Properties
    {
        // Transparency
        _AlphaTest("Alpha Test", Float) = 0
        _AlphaTestThreshold("Alpha Test Threshold", Range(0.0, 1.0)) = 0.5
        
        _ScaleFade ("Scale Fade", Vector) = (50, 20, 0, 0)
        [HideInInspector]_NatureRendererDistanceControl ("", Float) = 1
        
        // Color
        [Enum(Tint,0, HSL,1)]_ColorCorrection("Color Variation", Float) = 0
        _HSL ("Hue, Saturation, Lightness", Vector) = (0.02, 0.05, 0.1, 0)
        _HSLVariation ("Hue, Saturation, Lightness Variation", Vector) = (-0.02, -0.05, -0.1, 0)
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _TintVariation ("Tint Variation", Color) = (1, 1, 1, 1)
        _ColorVariationSpread ("Color Variation Spread", Float) = 0.2
        
        // Surface
        // Surface Settings
        [Enum(On, 0, Off, 2)] _DoubleSidedMode ("Double Sided", Float) = 2
        [Enum(Same, 0, Flip, 1)] _DoubleSidedNormalMode ("Double sided normals", Float) = 1
        
        _VertexNormalStrength("Vertex Normal Strength", Range(0, 1)) = 1
        [Enum(Off,0, MetallicGloss,1, Packed,2)]_SurfaceMapMethod("Surface Maps", Float) = 2
        
        // Base Maps
        [NoScaleOffset][HideInInspector]_MainTex("MainTex (legacy, use Albedo instead)", 2D) = "white" {}
        [NoScaleOffset][MainTexture]_Albedo("Albedo", 2D) = "white" {}
        [NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
        _NormalMapScale("Normal Map Strength", Range(0, 1)) = 1
        _Glossiness("Smoothness", Range(0, 1)) = 0.2
        _Metallic("Metallic", Range(0, 1)) = 0
        
        // Surface Maps
        [NoScaleOffset]_PackedMap("Packed Map", 2D) = "white" {}
        [NoScaleOffset]_MetallicGlossMap("Metallic Gloss Map", 2D) = "black" {}
        [NoScaleOffset]_OcclusionMap("Occlusion Map", 2D) = "white" {}
        _GlossRemap("Remap Smoothness", Vector) = (0, 1, 0, 0)
        _OcclusionRemap("Remap Occlusion", Vector) = (0, 1, 0, 0)
        
        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}
        _EmissionIntensity("Intensity", Float) = 0.0
        
        // Wind
        _ObjectHeight("Grass Height", Float) = 0.5
        _ObjectRadius("Grass Radius", Float) = 0.5
        
        [Enum(Off,0, Baked,1, Automatic,2)]_Wind("Wind Control", Float) = 2
        _WindVariation("Wind Variation", Range(0, 1)) = 0.3
        _WindStrength("Wind Strength", Range(0, 2)) = 1
        _TurbulenceStrength("Turbulence Strength", Range(0, 2)) = 1
        _RecalculateWindNormals("Recalculate Normals", Range(0,1)) = 0.5
        _WindFade("Wind Fade", Vector) = (50, 20, 0, 0)
        _TrunkBendFactor("Trunk Bending", Vector) = (1, 0, 0, 0)
        
        // Interaction
        [Enum(Off,0, Pivot,1, Vertex,2)]_Interaction("Interaction", Float) = 0
        _InteractionDuration("Interaction Duration", Range(0, 5)) = 1.5
        _InteractionStrength("Interaction Strength", Range(0, 1)) = 1
        _InteractionPushDown("Interaction PushDown", Range(0,1)) = 1
        
        // Translucency
        [ToggleOff]_Translucency("Translucency", Float) = 0
        
        [Enum(Add,0,Overlay,1)]_TranslucencyBlendMode("Blend Mode", Float) = 0
        _TranslucencyStrength("Translucency Strength", Range(0, 2)) = 1
        _TranslucencyDistortion("Translucency Distortion", Range(0, 1)) = 0.5
        _TranslucencyScattering("Translucency Scattering", Range(0, 3)) = 2
        _TranslucencyColor("Translucency Color", Color) = (1, 1, 1, 1)
        _TranslucencyAmbient("Translucency Ambient", Range(0, 1)) = 0.5
        
        [NoScaleOffset]_ThicknessMap("Thickness Map", 2D) = "black" {}
        _ThicknessRemap("Thickness Remap", Vector) = (0, 1, 0, 0)
        
        // Overlay
        [ToggleOff]_Overlay("Overlay", Float) = 0
        _SampleAlphaOverlay("Sample Alpha Overlay", Float) = 1.0
        _SampleColorOverlay("Sample Color Overlay", Float) = 1.0
        
        // Perspective Correction
        _PerspectiveCorrection("Perspective Correction", Range(0,1)) = 0.5
        
        // Rendering
        [Enum(High, 0, Low, 1)] _LightingQuality ("Lighting Quality", Float) = 0
        [ToggleOff] _SpecularHighlights ("Specular Highlights", Float) = 1.0
    }
    
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            
            "Queue" = "AlphaTest+0"
            "RenderType"= "TransparentCutout"
            "DisableBatching" = "True"
            "NatureRendererInstancing" = "True"
        }
        LOD 0
        
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            Blend One Zero, One Zero
            ZWrite On
            
            Cull [_DoubleSidedMode]
            
            ZTest LEqual
            
            // TODO: Make sure this works on all platforms.
            
            // Embed the default pass setup.
            // This will overwrite any values that need to be different for specifc passes.
            
            HLSLPROGRAM
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Nature Shaders Settings
            #define NATURE_SHADERS
            
            #define _TYPE_GRASS
            
            // Nature Renderer calculates values that are the same for all vertices once
            // for each object. This is a nice optimization that reduces per-vertex calculations.
            // This only works if Procedural Instancing is enabled.
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                #define PER_OBJECT_VALUES_CALCULATED
            #endif
            
            // Shader Features
            #ifdef _ALPHATEST
                #define _ALPHA_CLIP_ON
                #define _ALPHATEST_ON // HDRP
            #else
                #define _ALPHA_CLIP_OFF
                #define _ALPHATEST_OFF // HDRP
                #define _ALPHA_CLIP_DISABLED
            #endif
            
            #pragma shader_feature_local _COLOR_TINT _COLOR_HSL
            
            #pragma shader_feature_local _WIND_OFF _WIND_AUTO _WIND_BAKED
            #if defined(_WIND_AUTO) || defined(_WIND_BAKED)
                #define _WIND
                #define _WIND_ON
            #endif
            
            // Already defined in Lit template.
            // #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            
            // Already defined in Lit template.
            //#pragma shader_feature_local _EMISSION
            
            #ifndef _INTERACTION_OFF
                #define _INTERACTION_OFF
            #endif
            
            #pragma shader_feature_local _TRANSLUCENCY
            #ifdef _TRANSLUCENCY
                #define _TRANSLUCENCY_ON
                #define _MATERIAL_FEATURE_SUBSURFACE_SCATTERING 1
                #define _MATERIAL_FEATURE_TRANSMISSION 1
                
            #endif
            
            // Include the default cginc files and configurations
            // that are required for the current render pipeline.
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Default global keywords for material quality.
            // #pragma multi_compile MATERIAL_QUALITY_HIGH MATERIAL_QUALITY_MEDIUM MATERIAL_QUALITY_LOW
            #pragma shader_feature_local _LIGHTING_QUALITY_HIGH _LIGHTING_QUALITY_LOW
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            
            #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            #pragma shader_feature_local _ALPHATEST
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _EMISSION
            
            #pragma multi_compile_instancing
            
            #ifdef _SPECULARHIGHLIGHTS_OFF
                #define _ENVIRONMENTREFLECTIONS_OFF
                #define _GLOSSYREFLECTIONS_OFF
            #endif
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            //#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            
            // Variants
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            
            // Attributes and Varyings
            // These are all the possible settings for the URP shaders.
            //#define _SURFACE_TYPE_TRANSPARENT 1
            //#define _AlphaClip 1
            //#define _NORMALMAP 1
            //#define _SPECULAR_SETUP
            //#define _BLENDMODE_ADD 1
            //#define _ALPHAPREMULTIPLY_ON 1
            #define _NORMAL_DROPOFF_TS 1
            //#define _NORMAL_DROPOFF_OS 1
            //#define _NORMAL_DROPOFF_WS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            //#define ATTRIBUTES_NEED_TEXCOORD0
            //#define ATTRIBUTES_NEED_TEXCOORD1
            //#define ATTRIBUTES_NEED_TEXCOORD2
            //#define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            //#define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            //#define VARYINGS_NEED_VIEWDIRECTION_WS
            //#define VARYINGS_NEED_BITANGENT_WS
            //#define VARYINGS_NEED_SCREENPOSITION
            //#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_CULLFACE
            
            #ifdef _MAIN_LIGHT_SHADOWS
                #define VARYINGS_NEED_SHADOWCOORDS
            #endif
            
            #define ATTRIBUTES_NEED_TEXCOORD1
            //#define VARYINGS_NEED_POSITION_WS
            //#define VARYINGS_NEED_NORMAL_WS
            //#define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            
            #define SHADERPASS_FORWARD
            
            // Common API to unify the different render pipelines.
            
            // Return absolute world position of current object
            float3 GetObjectAbsolutePositionWS()
            {
                float4x4 modelMatrix = UNITY_MATRIX_M;
                return GetAbsolutePositionWS(modelMatrix._m03_m13_m23); // Translation object to world
            }
            
            float3 GetPrimaryCameraPosition()
            {
                #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
                    return float3(0, 0, 0);
                #else
                    return _WorldSpaceCameraPos;
                #endif
            }
            
            // Unity 2020.2 already includes these methods.
            
            uint2 ComputeFadeMaskSeed(float3 V, uint2 positionSS)
            {
                uint2 fadeMaskSeed;
                if (IsPerspectiveProjection())
                {
                    float2 pv = PackNormalOctQuadEncode(V);
                    pv *= _ScreenParams.xy;
                    pv *= UNITY_MATRIX_P._m00_m11;
                    fadeMaskSeed = asuint((int2)pv);
                }
                else
                {
                    fadeMaskSeed = positionSS;
                }
                
                return fadeMaskSeed;
            }
            
            half3 UnpackScaleNormalRGorAG(half4 packednormal, half bumpScale)
            {
                #if defined(UNITY_NO_DXT5nm)
                    half3 normal = packednormal.xyz * 2 - 1;
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    return normal;
                #else
                    // This do the trick
                    packednormal.x *= packednormal.w;
                    
                    half3 normal;
                    normal.xy = (packednormal.xy * 2 - 1);
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
                    return normal;
                #endif
            }
            
            half3 UnpackScaleNormal(half4 packednormal, half bumpScale)
            {
                return UnpackScaleNormalRGorAG(packednormal, bumpScale);
            }
            
            struct IndirectLightingCommon
            {
                half3 diffuse;
                half3 specular;
            };
            
            struct SurfaceCommon
            {
                half3 diffuse;
                half3 specular;
                half3 normalWS;
                half oneMinusReflectivity;
                half smoothness;
                half alpha;
            };
            
            struct LightingCommon
            {
                Light light;
                IndirectLightingCommon indirect;
            };
            
            struct Surface
            {
                float3 Albedo; // base (diffuse or specular) color
                float3 Normal; // tangent space normal, if written
                half3 Emission;
                half Metallic; // 0=non-metal, 1=metal
                half Smoothness; // 0=rough, 1=smooth
                half Occlusion; // occlusion (default 1)
                float Alpha; // alpha for transparencies
                #if defined(_TRANSLUCENCY)
                    float Thickness; // 0 for full translucency, 1 for no translucency.
                #endif
            };
            
            #pragma target 4.0
            
            // Nature Renderer integration
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:SetupNatureRenderer nolightmap forwardadd renderinglayer
            
            // Input
            struct VertexAttributes
            {
                
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1; // optional
                float4 uv2 : TEXCOORD2; // optional
                float4 uv3 : TEXCOORD3; // optional
                
                //
                //uint instanceID : INSTANCEID_SEMANTIC;
                DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct SurfaceInput
            {
                float noise : TEXCOORD11; // TODO: pack noise into positionWS.w or normalWS.w
                
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 color : COLOR;
                float4 texCoord0 : TEXCOORD3;
                float4 texCoord1 : TEXCOORD4;
                float4 texCoord2 : TEXCOORD5;
                float4 viewDirectionWS : TEXCOORD6; //.xyz:view direction .w:fogCoord (standard)
                
                // SH or Lightmap UV
                half4 ambientOrLightmapUV : TEXCOORD7;
                
                // Lighting and shadow coordinates.
                // These are different depending on the render pipeline, so they are wrapped in
                // render pipeline specific tags.
                
                float4 fogFactorAndVertexLight : TEXCOORD8;
                float4 shadowCoord : TEXCOORD9;
                
                // Meta for editor visualization
                
                // Unity's default instancing settings.
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                
                // VFACE always needs to be the last semantic in the list, otherwise the compiler will throw
                // an error. So we define it here, and let the user add SHADERX_VFACE to the end of
                // the SurfaceInput struct.
                #if defined(SHADER_STAGE_FRAGMENT) //&& defined(VARYINGS_NEED_CULLFACE)
                    #define SHADERX_VFACE FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #else
                    #define SHADERX_VFACE
                #endif
                
                SHADERX_VFACE
            };
            
            // Properties
            CBUFFER_START( UnityPerMaterial )
            //
            float _AlphaTest;
            float _AlphaTestThreshold;
            
            // Fade
            float2 _ScaleFade;
            
            // Color Correction
            float3 _HSL;
            float3 _HSLVariation;
            float4 _Tint;
            float4 _TintVariation;
            float _ColorVariationSpread;
            
            // Surface Settings
            float _DoubleSidedMode;
            float _DoubleSidedNormalMode;
            
            float _VertexNormalStrength;
            float _SurfaceMapMethod;
            
            // Base Maps
            float _NormalMapScale;
            float _Metallic;
            float _Glossiness;
            
            // Surface Maps
            float2 _GlossRemap;
            float2 _OcclusionRemap;
            
            float4 _EmissionColor;
            float _EmissionIntensity;
            
            // Wind
            float _ObjectHeight;
            float _ObjectRadius;
            
            float _Wind;
            float _WindVariation;
            float _WindStrength;
            float _TurbulenceStrength;
            float _RecalculateWindNormals;
            float4 _TrunkBendFactor;
            
            float2 _WindFade;
            
            // Interaction
            
            // Translucency
            float _Translucency;
            
            float _TranslucencyBlendMode;
            float _TranslucencyStrength;
            float _TranslucencyDistortion;
            float _TranslucencyScattering;
            float4 _TranslucencyColor;
            float _TranslucencyAmbient;
            
            float2 _ThicknessRemap;
            
            // Overlay
            
            // Rendering
            float _LightingQuality;
            float _SpecularHighlights;
            float _EnvironmentReflections;
            
            CBUFFER_END
            
            SAMPLER( sampler_Albedo );
            TEXTURE2D( _Albedo );
            #ifdef _NORMALMAP
                TEXTURE2D( _NormalMap );
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                TEXTURE2D( _PackedMap );
            #endif
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                TEXTURE2D( _MetallicGlossMap );
                TEXTURE2D( _OcclusionMap );
            #endif
            
            #ifdef _EMISSION
                TEXTURE2D( _EmissionMap );
            #endif
            
            #ifdef _TRANSLUCENCY
                TEXTURE2D( _ThicknessMap );
            #endif
            
            // Include common features.
            // Properties
            #define GRASS_DEFAULT_HEIGHT 0.5
            #define PLANT_DEFAULT_HEIGHT 1.0
            #define TRUNK_DEFAULT_HEIGHT 20.0
            #define TRUNK_BASE_BEND_FACTOR 0.3
            #define TRUNK_BEND_MULTIPLIER 2.0
            
            float4 g_SmoothTime;
            float3 g_WindDirection;
            float2 g_Wind;
            float2 g_Turbulence;
            sampler2D g_GustNoise;
            
            // Properties that are calculated per-object by Nature Renderer
            #ifdef PER_OBJECT_VALUES_CALCULATED
                float g_WindFade;
                float g_ScaleFade;
                float g_WorldNoise;
                float3 g_ObjectPivot;
                float3 g_ConstantWindOffset;
                float g_PivotOffset;
                float3 g_WorldNormal;
            #endif
            
            float pow2( float x )
            {
                return x*x;
            }
            
            /// <summary>
            /// Returns the height of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectHeight()
            {
                return _ObjectHeight;
            }
            
            /// <summary>
            /// Returns the pivot of the object in world space.
            /// </summary>
            float3 GetObjectPivot()
            {
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    return g_ObjectPivot;
                #else
                    return GetAbsolutePositionWS( float3(UNITY_MATRIX_M[0].w, UNITY_MATRIX_M[1].w, UNITY_MATRIX_M[2].w) );
                #endif
            }
            
            #define GRASS_DEFAULT_RADIUS 1.0
            #define PLANT_DEFAULT_RADIUS 1.0
            #define TREE_DEFAULT_RADIUS 6.0
            
            /// <summary>
            /// Returns the radius of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectRadius()
            {
                return _ObjectRadius;
            }
            
            /// <summary>
            /// Returns the vertex normal in world space when vertex normals are anbled.
            /// Otherwise, returns the object's forward (Z+) direction.
            /// </summary>
            float3 GetWorldNormal(
                float3 normalWS, // The vertex normal in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                // New behavior, nice and simple.
                return normalWS;
                
                // Old behavior.
                /*
                #if defined(PER_OBJECT_VALUES_CALCULATED) && !defined(_TYPE_TREE_LEAVES)
                    return g_WorldNormal;
                #else
                    #ifdef _TYPE_TREE_LEAVES
                        // Scramble the vertex normals in case they are projected onto spheres
                        // or other geometry for smooth lighting. Otherwise the wind turbulence will end
                        // up as weird expanding and shrinking spheres.
                        // Define DO_NOT_SCRAMBLE_VERTEX_NORMALS in the shader if the tree models have
                        // accurate normals.
                        #ifndef DO_NOT_SCRAMBLE_VERTEX_NORMALS
                            return normalWS.xzy;
                        #else
                            return normalWS.xyz;
                        #endif
                    #else
                        return TransformObjectToWorldDir( float3(0, 0, 1) );
                    #endif
                #endif
                */
            }
            
            /// <summary>
            /// Returns the mask for the vertex.
            /// Uses the red channel of the vertex color.
            /// </summary>
            float GetVertexMask( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return 1.0;
                #else
                    #ifdef _WIND_BAKED
                        return vertexColor.r;
                    #else
                        return 1.0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Calculates the phase offset for the branch, based on the baked data.
            /// If no baked data is available, it will calculate an approximation of the branch.
            /// Should only be called for trees.
            /// </summary>
            float GetBranchPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_WIND_BAKED)
                    return vertexColor.r;
                #else
                    #if defined(_TYPE_TREE_BARK)
                        return 0;
                    #else
                        float3 offset = vertexWorldPosition - objectPivot;
                        float randomOffset = ( offset.x + offset.y + offset.z ) * 0.005;
                        return randomOffset;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the phase offset for the vertex.
            /// </summary>
            float GetPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return GetBranchPhaseOffset( vertexColor, vertexWorldPosition, objectPivot );
                #else
                    #ifdef _WIND_BAKED
                        return 1.0 - vertexColor.g;
                    #else
                        return 0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the edge flutter for the vertex,
            /// based either the vertex colors or UV (depending on the Wind Control settings).
            /// </summary>
            float GetEdgeFlutter( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_BARK)
                    return 0;
                #else
                    #if defined(_WIND_BAKED) && defined(_TYPE_TREE_LEAVES)
                        return vertexColor.g;
                    #else
                        return 1;
                    #endif
                #endif
            }
            
            float MaskFromHeightAndRadius( float3 vertex, float height, float radius )
            {
                return pow2( saturate( max(vertex.y / height, length(vertex.xz) / radius) ));
            }
            
            /// <summary>
            /// Returns a mask based on the relative height of the vertex.
            /// </summary>
            float GetHeightMask(
                float3 vertex, // The vertex position in object space.
                float4 vertexColor, // The vertex color.
                float2 uv1 ) // The second UV channel.
            {
                #if defined(_WIND_BAKED)
                    #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                        return uv1.y;
                    #else
                        return vertexColor.a;
                    #endif
                #else
                    #if defined(_TYPE_GRASS)
                        return saturate( vertex.y / GetObjectHeight() );
                    #else
                        return MaskFromHeightAndRadius( vertex, GetObjectHeight(), GetObjectRadius() );
                    #endif
                #endif
            }
            
            float Remap( float value, float2 remap )
            {
                return remap.x + value * (remap.y - remap.x);
            }
            
            float4 SmoothCurve( float4 x )
            {
                return x * x *( 3.0 - 2.0 * x );
            }
            float4 TriangleWave( float4 x )
            {
                return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
            }
            float4 SmoothTriangleWave( float4 x )
            {
                return SmoothCurve( TriangleWave( x ) );
            }
            
            float4 FastSin( float4 x )
            {
                #ifndef PI
                    #define PI 3.14159265
                #endif
                #define DIVIDE_BY_PI 1.0 / (2.0 * PI)
                return (SmoothTriangleWave( x * DIVIDE_BY_PI ) - 0.5) * 2;
            }
            
            float3 FixStretching( float3 vertex, float3 original, float3 center )
            {
                return center + SafeNormalize(vertex - center) * length(original - center);
            }
            
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
            
            float3 RotateAroundAxisFast( float3 center, float3 original, float3 direction )
            {
                return original + direction;
            }
            
            uniform sampler2D g_PerlinNoise;
            uniform float g_PerlinNoiseScale;
            
            void PerlinNoise( float2 uv, float scale, out float noise )
            {
                noise =
                    tex2Dlod(
                        g_PerlinNoise,
                        float4(uv.xy, 0, 0) * scale * g_PerlinNoiseScale).r;
            }
            
            void PerlinNoise_float( float2 uv, float scale, out float noise )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    noise = g_WorldNoise;
                #else
                    PerlinNoise( uv, scale, noise );
                #endif
            }
            
            //
            #ifndef NODE_NATURE_RENDERER_INCLUDED
                #define NODE_NATURE_RENDERER_INCLUDED
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    
                    #define Use_Macro_UNITY_MATRIX_M_instead_of_unity_ObjectToWorld unity_ObjectToWorld
                    #define Use_Macro_UNITY_MATRIX_I_M_instead_of_unity_WorldToObject unity_WorldToObject
                    
                    struct CompressedFloat4x4
                    {
                        uint positionXY;
                        uint positionZ_scale;
                        uint rotationXY;
                        uint rotationZW;
                    };
                    
                    uniform float3 _CompressionRange;
                    uniform float3 _CompressionBase;
                    
                    uint CompressToUshort( float value, float precision )
                    {
                        return (uint)(value / precision * 65535.0);
                    }
                    
                    uint CompressToByte( float value, float precision )
                    {
                        return (uint)(value / precision * 255.0);
                    }
                    
                    float DecompressFromByte( uint value, float precision )
                    {
                        return value / 255.0 * precision;
                    }
                    
                    float DecompressFromUshort( uint value, float precision )
                    {
                        return value / 65535.0 * precision;
                    }
                    
                    void _UnpackInt( uint packedInt, out uint a, out uint b )
                    {
                        a = ( (uint) (packedInt >> 16) );
                        b = ( (uint) ((packedInt << 16) >> 16) );
                    }
                    
                    void _UnpackShort( uint packedShort, out uint a, out uint b )
                    {
                        a = ( (uint) (packedShort >> 8) );
                        b = ( (uint) ((packedShort << 24) >> 24) );
                    }
                    
                    uint _PackInt( uint ushortA, uint ushortB )
                    {
                        return ushortA << 16 | ushortB;
                    }
                    
                    uint _PackShort( uint byteA, uint byteB )
                    {
                        return (byteA << 8) | byteB;
                    }
                    
                    float4x4 QuaternionToMatrix(float4 quaternion)
                    {
                        float4x4 result = (float4x4)0;
                        float x = quaternion.x;
                        float y = quaternion.y;
                        float z = quaternion.z;
                        float w = quaternion.w;
                        
                        float x2 = x + x;
                        float y2 = y + y;
                        float z2 = z + z;
                        float xx = x * x2;
                        float xy = x * y2;
                        float xz = x * z2;
                        float yy = y * y2;
                        float yz = y * z2;
                        float zz = z * z2;
                        float wx = w * x2;
                        float wy = w * y2;
                        float wz = w * z2;
                        
                        result[0][0] = 1.0 - (yy + zz);
                        result[0][1] = xy - wz;
                        result[0][2] = xz + wy;
                        
                        result[1][0] = xy + wz;
                        result[1][1] = 1.0 - (xx + zz);
                        result[1][2] = yz - wx;
                        
                        result[2][0] = xz - wy;
                        result[2][1] = yz + wx;
                        result[2][2] = 1.0 - (xx + yy);
                        
                        result[3][3] = 1.0;
                        
                        return result;
                    }
                    
                    void DecompressInstanceMatrix( inout float4x4 m, CompressedFloat4x4 compressedMatrix )
                    {
                        uint positionX, positionY, positionZ;
                        uint scaleXYZ;
                        uint rotationX, rotationY, rotationZ, rotationW;
                        
                        _UnpackInt( compressedMatrix.positionXY, positionX, positionY );
                        _UnpackInt( compressedMatrix.positionZ_scale, positionZ, scaleXYZ );
                        _UnpackInt( compressedMatrix.rotationXY, rotationX, rotationY );
                        _UnpackInt( compressedMatrix.rotationZW, rotationZ, rotationW );
                        
                        uint scaleX, scaleY;
                        _UnpackShort( scaleXYZ, scaleX, scaleY );
                        
                        float3 position =
                            float3(
                                DecompressFromUshort(positionX, _CompressionRange.x) + _CompressionBase.x,
                                DecompressFromUshort(positionY, _CompressionRange.y) + _CompressionBase.y,
                                DecompressFromUshort(positionZ, _CompressionRange.z) + _CompressionBase.z );
                        
                        float3 scale =
                            float3(
                                DecompressFromByte(scaleX, 16.0),
                                DecompressFromByte(scaleY, 16.0),
                                DecompressFromByte(scaleX, 16.0) );
                        
                        float4 rotation =
                            float4(
                                DecompressFromUshort(rotationX, 2.0) - 1.0,
                                DecompressFromUshort(rotationY, 2.0) - 1.0,
                                DecompressFromUshort(rotationZ, 2.0) - 1.0,
                                DecompressFromUshort(rotationW, 2.0) - 1.0 );
                        
                        m = QuaternionToMatrix( rotation );
                        
                        m[0][0] *= scale.x; m[1][0] *= scale.y; m[2][0] *= scale.z;
                        m[0][1] *= scale.x; m[1][1] *= scale.y; m[2][1] *= scale.z;
                        m[0][2] *= scale.x; m[1][2] *= scale.y; m[2][2] *= scale.z;
                        m[0][3] *= scale.x; m[1][3] *= scale.y; m[2][3] *= scale.z;
                        
                        m[0][3] = position.x;
                        m[1][3] = position.y;
                        m[2][3] = position.z;
                    }
                    
                    #if defined(SHADER_API_GLCORE) \
                        || defined(SHADER_API_D3D11) \
                        || defined(SHADER_API_GLES3) \
                        || defined(SHADER_API_METAL) \
                        || defined(SHADER_API_VULKAN) \
                        || defined(SHADER_API_PSSL) \
                        || defined(SHADER_API_XBOXONE)
                        uniform StructuredBuffer<CompressedFloat4x4> _NatureRendererBuffer;
                    #endif
                    
                    float4x4 inverse(float4x4 input)
                    {
                        #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
                        
                        float4x4 cofactors = float4x4(
                            minor(_22_23_24, _32_33_34, _42_43_44),
                            -minor(_21_23_24, _31_33_34, _41_43_44),
                            minor(_21_22_24, _31_32_34, _41_42_44),
                            -minor(_21_22_23, _31_32_33, _41_42_43),
                            
                            -minor(_12_13_14, _32_33_34, _42_43_44),
                            minor(_11_13_14, _31_33_34, _41_43_44),
                            -minor(_11_12_14, _31_32_34, _41_42_44),
                            minor(_11_12_13, _31_32_33, _41_42_43),
                            
                            minor(_12_13_14, _22_23_24, _42_43_44),
                            -minor(_11_13_14, _21_23_24, _41_43_44),
                            minor(_11_12_14, _21_22_24, _41_42_44),
                            -minor(_11_12_13, _21_22_23, _41_42_43),
                            
                            -minor(_12_13_14, _22_23_24, _32_33_34),
                            minor(_11_13_14, _21_23_24, _31_33_34),
                            -minor(_11_12_14, _21_22_24, _31_32_34),
                            minor(_11_12_13, _21_22_23, _31_32_33)
                            );
                        #undef minor
                        return transpose(cofactors) / determinant(input);
                    }
                #endif
                
                // Pre-calculate and cache data for Nature Shaders that relies on
                // per-object data instead of per-vertex or per-pixel.
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    void PreCalculateNatureShadersData()
                    {
                        g_ObjectPivot = GetAbsolutePositionWS( float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w) );
                        g_ConstantWindOffset = cross( GetWindDirection().xyz, float3(0,1,0) );
                        g_PivotOffset = length( float3(g_ObjectPivot.x, 0, g_ObjectPivot.z) );
                        g_WorldNormal = TransformObjectToWorldDir( float3(0, 0, 1) );
                        
                        GetFade( g_ObjectPivot, g_WindFade, g_ScaleFade );
                        PerlinNoise( g_ObjectPivot.xz, _ColorVariationSpread, g_WorldNoise);
                    }
                #endif
                
                void SetupNatureRenderer()
                {
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                        DecompressInstanceMatrix(unity_ObjectToWorld, _NatureRendererBuffer[unity_InstanceID]);
                        unity_WorldToObject = inverse(unity_ObjectToWorld);
                    #endif
                    
                    #if defined(PER_OBJECT_VALUES_CALCULATED)
                        PreCalculateNatureShadersData();
                    #endif
                }
                
                void NatureRenderer_float( float3 vertex, out float3 vertexOut )
                {
                    vertexOut = vertex;
                }
            #endif
            
            struct TranslucencyInput
            {
                float Scale;
                float NormalDistortion;
                float Scattering;
                float Thickness;
                float Ambient;
                half3 Color;
            };
            
            half3 Translucency(
                TranslucencyInput input,
                float3 bakedGI,
                float3 surfaceAlbedo,
                float3 surfaceNormal,
                float3 viewDirectionWS,
                Light light )
            {
                half3 lightDir = light.direction + surfaceNormal * input.NormalDistortion;
                half transVdotL =
                    pow( saturate( dot( viewDirectionWS, -lightDir ) ), input.Scattering ) * input.Scale;
                half3 translucency =
                    (transVdotL + bakedGI * input.Ambient)
                    * (1-input.Thickness)
                    * light.shadowAttenuation
                    * light.distanceAttenuation;
                
                return half3( surfaceAlbedo * light.color * translucency * input.Color );
            }
            
            float3 Linear_to_HSV(float3 In)
            {
                float3 sRGBLo = In * 12.92;
                float3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
                float3 Linear = float3(In <= 0.0031308) ? sRGBLo : sRGBHi;
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(Linear.bg, K.wz), float4(Linear.gb, K.xy), step(Linear.b, Linear.g));
                float4 Q = lerp(float4(P.xyw, Linear.r), float4(Linear.r, P.yzx), step(P.x, Linear.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                return float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            }
            
            float3 HSV_to_Linear(float3 In)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                float3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                float3 linearRGBLo = RGB / 12.92;
                float3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                return float3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
            }
            
            void HSL_float( float4 color, float3 hsl, out float4 colorOut )
            {
                float3 hsv = Linear_to_HSV( color.rgb );
                hsv.x += hsl.x;
                hsv.y = saturate(hsv.y + hsl.y * 0.5);
                hsv.z = saturate(hsv.z + hsl.z * 0.5);
                colorOut = float4( HSV_to_Linear(hsv), color.a );
            }
            
            struct WindInput
            {
                // Global
                float3 direction;
                float speed;
                
                // Per-Object
                float3 objectPivot;
                float fade;
                
                // Per-Vertex
                float phaseOffset;
                float3 normalWS;
                float mask;
                float flutter;
            };
            
            /// <summary>
            /// Returns the time for gusts.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothGustOffset()
            {
                return g_SmoothTime.y;
            }
            
            /// <summary>
            /// Returns the time for turbulence.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothTurbulenceOffset()
            {
                return g_SmoothTime.z;
            }
            
            /// <summary>
            /// Returns the global wind direction.
            /// </summary>
            float3 GetWindDirection()
            {
                return g_WindDirection != 0
                ? normalize(float3(g_WindDirection.x, 0, g_WindDirection.z))
                : float3(1, 0, 0);
            }
            
            /// <summary>
            /// Returns the global wind speed.
            /// </summary>
            float GetWindSpeed()
            {
                return g_Wind.x;
            }
            
            /// <summary>
            /// Returns the global wind strength.
            /// </summary>
            float GetWindStrength()
            {
                return g_Wind.y * _WindStrength;
            }
            
            /// <summary>
            /// Returns a random windVariation value based on the object pivot.
            /// </summary>
            float GetWindVariation(
                float3 objectPivot ) // The object pivot in world space.
            {
                return 1.0 - frac( objectPivot.x * objectPivot.z * 10.0 ) * _WindVariation;
            }
            
            /// <summary>
            /// Returns the global turbulence strength.
            /// </summary>
            float GetTurbulenceStrength()
            {
                return g_Turbulence.y * _TurbulenceStrength;
            }
            
            /// <summary>
            /// Returns the time for ambient wind.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothAmbientOffset()
            {
                return g_SmoothTime.x;
            }
            
            /// <summary>
            /// Calculates the wind frequency for the ambient wind.
            /// Returns a vector4 containing 4 different sine waves.
            /// </summary>
            float4 AmbientFrequency(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset, // The wind phase offset. (Range: 0-1)
                float speed ) // The wind speed.
            {
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float footprint = 3;
                float time = GetSmoothAmbientOffset() - phaseOffset * footprint;
                
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float pivotOffset = g_PivotOffset;
                #else
                    float pivotOffset = length( float3(objectPivot.x, 0, objectPivot.z) );
                #endif
                
                float scale = 0.5;
                float frequency = pivotOffset * scale - time;
                return FastSin(
                    float4(
                        frequency + constantOffset.x,
                        frequency*0.5 + constantOffset.z,
                        frequency*0.25,
                        frequency*0.125) * speed );
            }
            
            /// <summary>
            /// Calculates the ambient wind.
            /// </summary>
            float3 AmbientWind(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset ) // The wind phase offset. (Range: 0-1)
            {
                float4 sine = AmbientFrequency( objectPivot, vertexWorldPosition, windDirection, phaseOffset, 1 );
                sine.w = abs(sine.w) + 0.5;
                float xz = 1.5 * sine.x * sine.z + sine.w + 1;
                float y = 1 * sine.y * sine.z + sine.w;
                return windDirection * float3(xz, 0, xz) + float3(0, y, 0);
            }
            
            /// <summary>
            /// Calculates the turbulence.
            /// </summary>
            float3 Turbulence(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 worldNormal, // The direction of the turbulence in world space (usually vertex normal).
                float phaseOffset, // The wind phase offset.
                float edgeFlutter, // The amount of edge flutter for tree leaves. (Range: 0-1)
                float speed ) // The wind speed.
            {
                #if defined(_TYPE_TREE_BARK)
                    return float3(0, 0, 0);
                #else
                    float time = GetSmoothTurbulenceOffset() - phaseOffset;
                    float frequency = (objectPivot.x + objectPivot.y + objectPivot.z) * 2.5 - time;
                    
                    // TODO: Add a secondary frequency.
                    float4 sine =
                        FastSin(
                            float4(
                                (1.65 * frequency) * speed,
                                (2 * 1.65 * frequency) * speed,
                                0,
                                0) );
                    
                    float x = 1 * sine.x + 1;
                    float z = 1 * sine.y + 1;
                    float y = (x + z) * 0.5;
                    
                    #if defined(_TYPE_TREE_LEAVES)
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1) * edgeFlutter;
                    #else
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1);
                    #endif
                #endif
            }
            
            float3 SampleGust(
                float3 objectPivot,
                float3 vertexWorldPosition,
                float3 windDirection,
                float phaseOffset,
                float edgeFlutter,
                float lod )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                    lod = 5;
                #else
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                #endif
                
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float2 windOffsetOverTime = windDirection.xz * time + constantOffset.xz;
                #if defined(_TYPE_TREE_LEAVES)
                    float3 vertexOffset = vertexWorldPosition - objectPivot;
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime + vertexOffset.xz * 0.0075 * edgeFlutter;
                #else
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime;
                #endif
                float strength = tex2Dlod( g_GustNoise, float4(offset, 0, lod) ).r;
                return strength * windDirection;
            }
            
            float3 CombineWind(
                float3 ambient, // Large constant ambient wind motion.
                float3 gust, // Local gust based on noise texture.
                float3 turbulence, // Constant turbulence.
                float3 shiver, // High frequency shivering during gust.
                float4 strength // The wind strength for each wind component.
                )
            {
                ambient *= strength.x;
                gust *= strength.y;
                turbulence *= strength.z;
                shiver *= strength.w;
                
                // Trees require more displacement for the wind to be visible because the objects are larger.
                // These are magic numbers that give a nice balance between the grass/plants and trees,
                // based on a common tree size.
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    ambient *= 3;
                    gust *= 1;
                    turbulence *= 3;
                    shiver *= 3;
                #endif
                
                float gustLength = length( gust );
                float increaseTurbelenceWithGust = smoothstep(0, 1, gustLength) + 1;
                
                // Calculate the balance between different wind types.
                // If we do it here then we can keep the input parameters in a 0-1 range.
                ambient *= 0.1;
                gust *= 1.5;
                turbulence *= 0.15;
                shiver *= 0.15;
                
                #if defined(_DEBUG_AMBIENT)
                    return ambient;
                #elif defined(_DEBUG_GUST)
                    return gust;
                #elif defined(_DEBUG_TURBULENCE)
                    return lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #else
                    return
                    ambient
                    + gust
                    + lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #endif
            }
            
            float3 ComputeWind( WindInput input, float3 positionWS )
            {
                #if defined(_TYPE_GRASS) || defined(_TYPE_PLANT)
                    input.phaseOffset *= dot( input.direction, (positionWS - input.objectPivot) );
                    input.phaseOffset += input.mask * 0.5;
                #endif
                
                float3 ambient =
                    AmbientWind(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset );
                
                float3 gust =
                    SampleGust(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset,
                        input.flutter,
                        0 );
                
                // Add a bit of a random phase offset to the tree leaves. Phase Offset is calculated
                // per-branch and we don't want to have the same turbulence for the entire branch.
                #if defined(_TYPE_TREE_LEAVES)
                    input.phaseOffset +=
                        dot( input.direction, (positionWS - input.objectPivot) ) * input.flutter;
                #endif
                
                float3 turbulence1 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        1 );
                
                float3 turbulence2 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        2 );
                
                return CombineWind(
                    ambient,
                    gust,
                    turbulence1,
                    turbulence2,
                    float4(GetWindStrength().xx, GetTurbulenceStrength().xx) );
            }
            
            float3 ApplyWind(
                float3 positionWS, // Vertex position in world space.
                float3 objectPivot, // Object Pivot in world space.
                float3 combinedWind, // Combined Wind vector in world space.
                float mask, // Wind mask. (Range: 0-1)
                float distanceFade) // Wind distance fade. (Range: 0-1)
            {
                #if defined(_TYPE_GRASS)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade,
                        positionWS,
                        float3( positionWS.x, objectPivot.y, positionWS.z ) );
                #elif defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade * 4,
                        positionWS,
                        objectPivot);
                #else
                    return FixStretching(
                        positionWS + combinedWind * mask * mask * distanceFade,
                        positionWS,
                        objectPivot);
                #endif
            }
            
            void Wind( WindInput input, inout float3 positionWS, inout float3 normalWS )
            {
                // Adjust the pivot for grass to use the XZ position of the vertex.
                // This is a decent workaround to get a per-grass-blade pivot until
                // we have proper pivot support.
                #ifdef _TYPE_GRASS
                    input.objectPivot = float3(positionWS.x, input.objectPivot.y, positionWS.z);
                #endif
                
                // Compute wind.
                float3 wind = ComputeWind( input, positionWS );
                
                // Apply wind to vertex.
                float3 outputWS =
                    ApplyWind(
                        positionWS,
                        input.objectPivot,
                        wind,
                        input.mask,
                        input.fade );
                
                // Recalculate normals for grass
                #if defined(_TYPE_GRASS)
                    float3 delta = outputWS - positionWS;
                    normalWS =
                        lerp(
                            normalWS,
                            normalWS + SafeNormalize( delta + float3(0, 0.1, 0) ),
                            length(delta) * _RecalculateWindNormals * input.fade );
                #endif
                
                positionWS = outputWS;
            }
            
            /// <summary>
            /// Returns the bend factor for the tree trunk.
            /// X contains the bend factor for the entire tree, Y for the base.
            /// </summary>
            float2 GetTrunkBendFactor()
            {
                return _TrunkBendFactor.xy;
            }
            
            float GetTrunkMask(
                float3 vertex, float2 uv1, float treeHeight, float bendFactor, float baseBendFactor )
            {
                #ifdef _WIND_BAKED
                    float trunkMask = saturate( uv1.x * bendFactor );
                #else
                    float trunkMask = pow2(saturate( vertex.y / treeHeight )) * bendFactor;
                #endif
                
                return saturate( trunkMask + saturate( vertex.y ) * baseBendFactor );
            }
            
            void Wind_Trunk(
                float3 vertex, // The vertex position in object space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 vertexWithWind, // The vertex position with wind displacement in world space.
                float2 uv1, // The second UV channel of the vertex. (UV1)
                float3 objectPivot, // The object pivot in world space.
                float3 windDirection, // The wind direction in world space. (normalized)
                out float3 vertexOut )
            {
                // Additional properties. Either global or baked.
                float2 bendFactor = GetTrunkBendFactor();
                float trunkMask = GetTrunkMask( vertex, uv1, GetObjectHeight(), bendFactor.x, bendFactor.y );
                float ambientStrength = GetWindStrength();
                
                // Calculate Ambient Wind
                float4 trunkAmbient =
                    AmbientFrequency(
                        objectPivot,
                        vertexWorldPosition,
                        windDirection,
                        0,
                        0.75 ) + ambientStrength;
                trunkAmbient *= trunkMask;
                
                // Calculate Gust
                float3 trunkGust =
                    SampleGust(
                        objectPivot, vertexWorldPosition, windDirection, 0, 0, 7);
                trunkGust *= trunkMask;
                
                // Apply
                float gustFrequency = trunkAmbient.w * length(trunkGust);
                float baseFrequency1 = trunkAmbient.x;
                float baseFrequency2 = trunkAmbient.x + trunkAmbient.y;
                float baseFrequency =
                    lerp( baseFrequency1, baseFrequency2, (_SinTime.x + 1) * 0.5 * ambientStrength);
                
                // TODO: Use the "FixStretching" approach?
                vertexOut =
                    RotateAroundAxis(
                        objectPivot,
                        vertexWithWind,
                        normalize( cross( float3(0,1,0) , windDirection ) ),
                        (baseFrequency * 0.75 + gustFrequency) * ambientStrength * 0.0375);
            }
            
            void AlphaTest( float alpha, float threshold )
            {
                
                clip( alpha - threshold );
            }
            
            void GetFade(
                float3 objectPivot, // The object pivot in world space.
                out float windFade,
                out float scaleFade )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    windFade = 1.0;
                    scaleFade = 1.0;
                #else
                    float distanceToCamera = distance( objectPivot, _WorldSpaceCameraPos );
                    windFade = 1.0 - saturate( (distanceToCamera - _WindFade.x) / _WindFade.y );
                    scaleFade = 1.0 - saturate( (distanceToCamera - _ScaleFade.x) / _ScaleFade.y );
                #endif
            }
            
            void PerVertexFade( float3 objectPivot, out float windFade, out float scaleFade )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    windFade = g_WindFade;
                    scaleFade = g_ScaleFade;
                #else
                    GetFade(
                        objectPivot,
                        windFade,
                        scaleFade );
                #endif
            }
            
            void Wind(
                inout VertexAttributes vertex,
                inout SurfaceInput surface,
                inout float3 positionWS,
                float windFade,
                float scaleFade,
                float heightMask,
                float phaseOffset,
                float3 objectPivot )
            {
                WindInput input;
                
                // Global
                input.direction = GetWindDirection();
                input.speed = GetWindSpeed();
                
                // Per-Object
                input.objectPivot = objectPivot;
                input.fade = windFade;
                
                // Per-Vertex
                input.phaseOffset = phaseOffset;
                input.normalWS = surface.normalWS;
                float windVariation = GetWindVariation( input.objectPivot );
                float vertexMask = GetVertexMask( vertex.color );
                input.mask = heightMask * vertexMask * windVariation;
                input.flutter = GetEdgeFlutter( vertex.color );
                
                float3 vertexOut = positionWS;
                float3 normalOut = surface.normalWS;
                Wind(
                    input,
                    vertexOut,
                    normalOut );
                
                #if defined(_TYPE_TREE_LEAVES) || defined( _TYPE_TREE_BARK )
                    Wind_Trunk(
                        vertex.positionOS.xyz,
                        positionWS.xyz,
                        vertexOut.xyz,
                        vertex.uv1.xy,
                        input.objectPivot,
                        input.direction,
                        vertexOut );
                #endif
                
                positionWS = vertexOut;
                surface.normalWS = normalOut;
            }
            
            float PerVertexPerlinNoise( float3 objectPivot )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    return g_WorldNoise;
                #else
                    float noise;
                    PerlinNoise_float( objectPivot.xz, _ColorVariationSpread, noise );
                    return noise;
                #endif
            }
            
            float3 ApplyScaleFade(
                float3 vertexWorldPosition, float3 objectPivot, float fade )
            {
                vertexWorldPosition.y = lerp(objectPivot.y, vertexWorldPosition.y, max(fade, 0.2));
                return vertexWorldPosition;
            }
            
            // Surface
            void SampleAlbedo( float2 uv0, out float4 albedo )
            {
                albedo = SAMPLE_TEXTURE2D( _Albedo, sampler_Albedo, uv0.xy );
            }
            
            void ApplyColorCorrection( inout float4 albedo, float noise )
            {
                #ifdef _COLOR_HSL
                    float4 albedo1 = albedo;
                    float4 albedo2 = albedo;
                    HSL_float( albedo1, _HSL, albedo1 );
                    HSL_float( albedo2, _HSLVariation, albedo2 );
                    albedo = lerp(albedo2, albedo1, noise);
                #else
                    albedo *= lerp(_TintVariation, _Tint, noise);
                #endif
            }
            
            #ifdef _NORMALMAP
                void SampleNormalMap( float2 uv0, out float3 normal )
                {
                    normal =
                        UnpackScaleNormal(
                            SAMPLE_TEXTURE2D( _NormalMap, sampler_Albedo, uv0.xy ), _NormalMapScale ).xyz;
                }
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                void SamplePackedMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 packed = SAMPLE_TEXTURE2D(_PackedMap, sampler_Albedo, uv0.xy);
                    metallic = packed.r;
                    smoothness = Remap(packed.a, _GlossRemap.xy);
                    occlusion = Remap(packed.g, _OcclusionRemap.xy);
                }
            #endif
            
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                void SampleMetallicGlossMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 metallicGloss = SAMPLE_TEXTURE2D( _MetallicGlossMap, sampler_Albedo, uv0.xy );
                    metallic = metallicGloss.r;
                    smoothness = Remap(metallicGloss.a, _GlossRemap.xy);
                    occlusion = Remap(SAMPLE_TEXTURE2D( _OcclusionMap, sampler_Albedo, uv0.xy ).g, _OcclusionRemap.xy);
                }
            #endif
            
            void SampleMetallicGlossConstants(
                float2 uv0, out float metallic, out float smoothness, out float occlusion)
            {
                metallic = _Metallic;
                smoothness = _Glossiness;
                occlusion = 1.0;
            }
            
            #ifdef _TRANSLUCENCY
                void SampleThickness( float2 uv0, out float thickness )
                {
                    thickness = SAMPLE_TEXTURE2D( _ThicknessMap, sampler_Albedo, uv0.xy ).r;
                    thickness = Remap( thickness, _ThicknessRemap.xy );
                }
            #endif
            
            #ifdef _EMISSION
                void SampleEmission( float2 uv0, out float3 emission )
                {
                    emission = SAMPLE_TEXTURE2D( _EmissionMap, sampler_Albedo, uv0.xy ).rgb;
                    emission *= _EmissionColor.rgb;
                    emission *= _EmissionIntensity;
                }
            #endif
            
            void FlipNormals( inout float3 normals, FRONT_FACE_TYPE cullFace )
            {
                normals.z *=
                    _DoubleSidedNormalMode > 0
                    ? IS_FRONT_VFACE(cullFace, 1, -1)
                    : 1.0;
            }
            
            // Called with raw vertex data before doing any kind of calculations or transformations.
            // Useful to modify the vertex data in object space.
            void PreVertexMethod( inout VertexAttributes vertex )
            {
            }
            
            // The main vertex method. Is used to modify the vertex data and
            // the input for the surface (fragment) method.
            void VertexMethod(
                VertexAttributes vertex,
                inout SurfaceInput surface )
            {
                float3 objectPivot = GetObjectPivot();
                float3 positionWS = GetAbsolutePositionWS( surface.positionWS.xyz );
                float3 positionWSOriginal = positionWS;
                
                surface.noise = PerVertexPerlinNoise( objectPivot );
                
                float windFade;
                float scaleFade;
                PerVertexFade( objectPivot, windFade, scaleFade );
                
                float heightMask =
                    GetHeightMask(
                        vertex.positionOS.xyz,
                        vertex.color,
                        vertex.uv1.xy );
                
                float phaseOffset =
                    GetPhaseOffset(
                        vertex.color,
                        positionWS,
                        objectPivot );
                
                #ifdef _WIND
                    Wind(
                        vertex,
                        surface,
                        positionWS,
                        windFade,
                        scaleFade,
                        heightMask,
                        phaseOffset,
                        objectPivot );
                #endif
                
                positionWS =
                    ApplyScaleFade( positionWS, objectPivot, scaleFade );
                
                surface.positionWS = GetCameraRelativePositionWS( positionWS );
                surface.normalWS = lerp(TransformObjectToWorldNormal(float3(0,1,0)), surface.normalWS, _VertexNormalStrength);
            }
            
            void SurfaceMethod(
                SurfaceInput input,
                inout Surface output )
            {
                // Albedo
                float4 albedo;
                SampleAlbedo( input.texCoord0.xy, albedo );
                
                // Overlay
                
                // Alpha clip
                #ifdef _ALPHATEST
                    
                    AlphaTest( albedo.a, _AlphaTestThreshold );
                    
                #else
                    albedo.a = 1;
                #endif
                
                // Color correction
                ApplyColorCorrection( albedo, input.noise );
                output.Albedo = albedo.rgb;
                output.Alpha = albedo.a;
                
                // Normal Map
                #ifdef _NORMALMAP
                    SampleNormalMap( input.texCoord0.xy, output.Normal );
                #endif
                
                // Surface Map
                #if defined(_SURFACE_MAP_PACKED)
                    SamplePackedMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #elif defined(_SURFACE_MAP_METALLIC_GLOSS)
                    SampleMetallicGlossMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #else
                    SampleMetallicGlossConstants(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #endif
                
                // Translucency
                #ifdef _TRANSLUCENCY
                    SampleThickness( input.texCoord0.xy, output.Thickness );
                #endif
                
                // Emission
                #ifdef _EMISSION
                    SampleEmission( input.texCoord0.xy, output.Emission );
                #endif
                
                // Flip double-sided normals
                #ifdef SHADER_STAGE_FRAGMENT
                    FlipNormals( output.Normal, input.cullFace );
                #endif
            }
            
            float3 Overlay(float3 a, float3 b)
            {
                return a < 0.5
                ? 2 * a * b
                : 1 - 2 * (1-a) * (1-b);
            }
            
            void PostLightingMethod(
                SurfaceInput input,
                Surface surface,
                SurfaceCommon surfaceCommon,
                LightingCommon lightingCommon,
                inout half4 color )
            {
                #ifdef _TRANSLUCENCY
                    #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
                        // Ignore these light types. Only directional is supported.
                    #else
                        TranslucencyInput translucencyInput;
                        translucencyInput.Scale = _TranslucencyStrength;
                        translucencyInput.NormalDistortion = _TranslucencyDistortion;
                        translucencyInput.Scattering = _TranslucencyScattering;
                        translucencyInput.Thickness = surface.Thickness;
                        translucencyInput.Color = _TranslucencyColor.rgb;
                        translucencyInput.Ambient = _TranslucencyAmbient;
                        
                        float3 translucency =
                            Translucency(
                                translucencyInput,
                                lightingCommon.indirect.diffuse,
                                surfaceCommon.diffuse,
                                surfaceCommon.normalWS,
                                -input.viewDirectionWS.xyz,
                                lightingCommon.light ).rgb;
                        
                        color.rgb +=
                            _TranslucencyBlendMode == 0
                            ? translucency
                            : Overlay(translucency, color.rgb);
                    #endif
                #endif
            }
            
            // Embed the default shader template for the current render pipeline. These templates
            // contains the actual shader code for the render-pipeline-specific lighting.
            
            // Vertex
            #if defined(SHADERPASS_SHADOWCASTER)
                float3 _LightDirection;
            #endif
            
            float4 UnityObjectToClipPos( float3 positionOS, float3 positionWS, float3 normalWS )
            {
                #if defined(SHADERPASS_SHADOWCASTER)
                    // Define shadow pass specific clip position for Universal
                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #endif
                #elif defined(SHADERPASS_META)
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #else
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #endif
                
                return positionCS;
            }
            
            SurfaceInput vert( VertexAttributes input )
            {
                SurfaceInput output = (SurfaceInput)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                PreVertexMethod( input );
                
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                output.positionWS = positionWS;
                output.normalWS = normalWS;			// normalized in TransformObjectToWorldNormal()
                output.tangentWS = tangentWS;		// normalized in TransformObjectToWorldDir()
                output.texCoord0 = input.uv0;
                output.texCoord1 = input.uv1;
                output.texCoord2 = input.uv2;
                //output.texCoord3 = input.uv3;
                output.color = input.color;
                output.viewDirectionWS.xyz = normalize( _WorldSpaceCameraPos.xyz - positionWS );
                
                VertexMethod( input, output );
                
                positionWS = output.positionWS;
                normalWS = output.normalWS;			// normalized in TransformObjectToWorldNormal()
                tangentWS = output.tangentWS;		// normalized in TransformObjectToWorldDir()
                
                // Recalculate vertex based on modifications to output
                // TODO: Only do this if the position changed
                input.positionOS = TransformWorldToObject( output.positionWS );
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                
                input.uv0 = output.texCoord0;
                input.uv1 = output.texCoord1;
                input.uv2 = output.texCoord2;
                
                #if defined(SHADERPASS_FORWARD)
                    OUTPUT_LIGHTMAP_UV(input.uv1, unity_LightmapST, output.ambientOrLightmapUV);
                    OUTPUT_SH(normalWS, output.ambientOrLightmapUV);
                #endif
                
                #ifdef VARYINGS_NEED_SCREENPOSITION
                    output.screenPosition = ComputeScreenPos(output.positionCS, _ProjectionParams.x);
                #endif
                
                #ifdef _MAIN_LIGHT_SHADOWS
                    output.shadowCoord = TransformWorldToShadowCoord( positionWS );
                #endif
                
                #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                    half3 vertexLight = VertexLighting(positionWS, normalWS);
                    half fogFactor = ComputeFogFactor(output.positionCS.z);
                    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
                #endif
                
                return output;
            }
            
            // Fragment
            
            void BuildCommonSurface(
                SurfaceInput input,
                Surface surface,
                out SurfaceCommon common )
            {
                common.alpha = surface.Alpha;
                common.smoothness = surface.Smoothness;
                common.diffuse = surface.Albedo;
                common.specular = 0;
                common.oneMinusReflectivity = 0;
                
                #if _NORMAL_DROPOFF_TS
                    // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
                    float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
                    float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    common.normalWS =
                        TransformTangentToWorld(
                            surface.Normal,
                            half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz));
                #elif _NORMAL_DROPOFF_OS
                    common.normalWS = TransformObjectToWorldNormal(surface.Normal);
                #elif _NORMAL_DROPOFF_WS
                    common.normalWS = surface.Normal;
                #endif
            }
            
            void BuildCommonLighting(
                SurfaceInput input,
                Surface surface,
                SurfaceCommon surfaceCommon,
                out LightingCommon lighting )
            {
                lighting = (LightingCommon)0;
                
                // The main light is sampled in the UniversalFragmentPBR, but it is not returned.
                // If we have a PostLightingMethod then we need to make sure that the lighting
                // data is available, so we sample the light here.
                // It is best to replace the UniversalFragmentPBR method with a custom method
                // that returns the data we need.
                Light light = GetMainLight( input.shadowCoord );
                lighting.light = light;
                lighting.light.direction *= -1; // TODO: Should this really be inverted?
                
                #ifdef LIGHTMAP_ON
                    lighting.indirect.diffuse =
                        SampleLightmap( input.ambientOrLightmapUV.xy, surfaceCommon.normalWS );
                #else
                    lighting.indirect.diffuse =
                        SampleSHPixel( input.ambientOrLightmapUV.xyz, surfaceCommon.normalWS );
                #endif
            }
            
            half4 frag(SurfaceInput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Normalize the ViewDirection per-pixel so that we have an accurate value.
                input.viewDirectionWS.xyz = normalize(input.viewDirectionWS.xyz);
                
                #ifdef LOD_FADE_CROSSFADE
                    // TODO: Dithering is not stable for shadows. Not a big issue since it is usually not noticeable, or the fade is further away than the shadow rendering distance.
                    #if !defined(SHADER_API_GLES)
                        LODDitheringTransition(
                            ComputeFadeMaskSeed(
                                GetWorldSpaceNormalizeViewDir(input.positionWS), // we need a very accurate view direction to get good dithering. The regular viewDirectionWS that we get as input is not accurate enough because it is calculated per-vertex and then interpolated. That is why we calculate the view direction again here.
                                input.positionCS.xy),
                            unity_LODFade.x);
                    #endif
                #endif
                
                Surface surface = (Surface)0;
                surface.Albedo = 1;
                surface.Emission = 0;
                surface.Metallic = 0;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                surface.Alpha = 1;
                surface.Normal = float3(0,0,1);
                SurfaceMethod( input, surface );
                
                SurfaceCommon surfaceCommon;
                LightingCommon lightingCommon;
                
                // Calculate per-pixel shadow coordinates for shadow cascades.
                #if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    input.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                #endif
                
                BuildCommonSurface( input, surface, surfaceCommon );
                BuildCommonLighting( input, surface, surfaceCommon, lightingCommon );
                
                // Unity's URP lighting method.
                InputData inputData;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = surfaceCommon.normalWS;
                inputData.viewDirectionWS = input.viewDirectionWS.xyz;
                inputData.shadowCoord = input.shadowCoord;
                inputData.fogCoord = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = lightingCommon.indirect.diffuse;
                
                #ifdef _SPECULAR_SETUP
                    float3 specular = surface.Specular;
                    float metallic = 1;
                #else
                    float3 specular = 0;
                    float metallic = surface.Metallic;
                #endif
                
                #ifdef _LIGHTING_QUALITY_LOW
                    
                    half4 specularSmoothness = half4(0.0h, 0.0h, 0.0h, 1.0h);
                    specularSmoothness.rgb = half3(1,1,1);
                    specularSmoothness.a = exp2(10 * specularSmoothness.a + 1);
                    
                    half4 color =
                        UniversalFragmentBlinnPhong(
                            inputData,
                            surface.Albedo,
                            specularSmoothness,
                            specularSmoothness.a,
                            surface.Emission,
                            surface.Alpha );
                    
                #else
                    half4 color =
                        UniversalFragmentPBR(
                            inputData,
                            surface.Albedo,
                            metallic,
                            specular,
                            surface.Smoothness,
                            surface.Occlusion,
                            surface.Emission,
                            surface.Alpha);
                #endif
                
                PostLightingMethod( input, surface, surfaceCommon, lightingCommon, color );
                
                color.rgb = MixFog( color.rgb, inputData.fogCoord );
                return color;
            }
            
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            
            Blend One Zero, One Zero
            ZWrite On
            
            Cull [_DoubleSidedMode]
            
            ZTest LEqual
            
            // TODO: Make sure this works on all platforms.
            
            // Embed the default pass setup.
            // This will overwrite any values that need to be different for specifc passes.
            
            AlphaToMask Off
            
            HLSLPROGRAM
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Nature Shaders Settings
            #define NATURE_SHADERS
            
            #define _TYPE_GRASS
            
            // Nature Renderer calculates values that are the same for all vertices once
            // for each object. This is a nice optimization that reduces per-vertex calculations.
            // This only works if Procedural Instancing is enabled.
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                #define PER_OBJECT_VALUES_CALCULATED
            #endif
            
            // Shader Features
            #ifdef _ALPHATEST
                #define _ALPHA_CLIP_ON
                #define _ALPHATEST_ON // HDRP
            #else
                #define _ALPHA_CLIP_OFF
                #define _ALPHATEST_OFF // HDRP
                #define _ALPHA_CLIP_DISABLED
            #endif
            
            #pragma shader_feature_local _COLOR_TINT _COLOR_HSL
            
            #pragma shader_feature_local _WIND_OFF _WIND_AUTO _WIND_BAKED
            #if defined(_WIND_AUTO) || defined(_WIND_BAKED)
                #define _WIND
                #define _WIND_ON
            #endif
            
            // Already defined in Lit template.
            // #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            
            // Already defined in Lit template.
            //#pragma shader_feature_local _EMISSION
            
            #ifndef _INTERACTION_OFF
                #define _INTERACTION_OFF
            #endif
            
            #pragma shader_feature_local _TRANSLUCENCY
            #ifdef _TRANSLUCENCY
                #define _TRANSLUCENCY_ON
                #define _MATERIAL_FEATURE_SUBSURFACE_SCATTERING 1
                #define _MATERIAL_FEATURE_TRANSMISSION 1
                
            #endif
            
            // Include the default cginc files and configurations
            // that are required for the current render pipeline.
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Default global keywords for material quality.
            // #pragma multi_compile MATERIAL_QUALITY_HIGH MATERIAL_QUALITY_MEDIUM MATERIAL_QUALITY_LOW
            #pragma shader_feature_local _LIGHTING_QUALITY_HIGH _LIGHTING_QUALITY_LOW
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            
            #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            #pragma shader_feature_local _ALPHATEST
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _EMISSION
            
            #pragma multi_compile_instancing
            
            #ifdef _SPECULARHIGHLIGHTS_OFF
                #define _ENVIRONMENTREFLECTIONS_OFF
                #define _GLOSSYREFLECTIONS_OFF
            #endif
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            //#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            
            // Variants
            
            // Attributes and Varyings
            // These are all the possible settings for the URP shaders.
            //#define _SURFACE_TYPE_TRANSPARENT 1
            //#define _AlphaClip 1
            //#define _NORMALMAP 1
            //#define _SPECULAR_SETUP
            //#define _BLENDMODE_ADD 1
            //#define _ALPHAPREMULTIPLY_ON 1
            #define _NORMAL_DROPOFF_TS 1
            //#define _NORMAL_DROPOFF_OS 1
            //#define _NORMAL_DROPOFF_WS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            //#define ATTRIBUTES_NEED_TEXCOORD0
            //#define ATTRIBUTES_NEED_TEXCOORD1
            //#define ATTRIBUTES_NEED_TEXCOORD2
            //#define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            //#define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            //#define VARYINGS_NEED_VIEWDIRECTION_WS
            //#define VARYINGS_NEED_BITANGENT_WS
            //#define VARYINGS_NEED_SCREENPOSITION
            //#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_CULLFACE
            
            #ifdef _MAIN_LIGHT_SHADOWS
                #define VARYINGS_NEED_SHADOWCOORDS
            #endif
            
            #define SHADERPASS_SHADOWCASTER
            
            // Common API to unify the different render pipelines.
            
            // Return absolute world position of current object
            float3 GetObjectAbsolutePositionWS()
            {
                float4x4 modelMatrix = UNITY_MATRIX_M;
                return GetAbsolutePositionWS(modelMatrix._m03_m13_m23); // Translation object to world
            }
            
            float3 GetPrimaryCameraPosition()
            {
                #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
                    return float3(0, 0, 0);
                #else
                    return _WorldSpaceCameraPos;
                #endif
            }
            
            // Unity 2020.2 already includes these methods.
            
            uint2 ComputeFadeMaskSeed(float3 V, uint2 positionSS)
            {
                uint2 fadeMaskSeed;
                if (IsPerspectiveProjection())
                {
                    float2 pv = PackNormalOctQuadEncode(V);
                    pv *= _ScreenParams.xy;
                    pv *= UNITY_MATRIX_P._m00_m11;
                    fadeMaskSeed = asuint((int2)pv);
                }
                else
                {
                    fadeMaskSeed = positionSS;
                }
                
                return fadeMaskSeed;
            }
            
            half3 UnpackScaleNormalRGorAG(half4 packednormal, half bumpScale)
            {
                #if defined(UNITY_NO_DXT5nm)
                    half3 normal = packednormal.xyz * 2 - 1;
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    return normal;
                #else
                    // This do the trick
                    packednormal.x *= packednormal.w;
                    
                    half3 normal;
                    normal.xy = (packednormal.xy * 2 - 1);
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
                    return normal;
                #endif
            }
            
            half3 UnpackScaleNormal(half4 packednormal, half bumpScale)
            {
                return UnpackScaleNormalRGorAG(packednormal, bumpScale);
            }
            
            struct IndirectLightingCommon
            {
                half3 diffuse;
                half3 specular;
            };
            
            struct SurfaceCommon
            {
                half3 diffuse;
                half3 specular;
                half3 normalWS;
                half oneMinusReflectivity;
                half smoothness;
                half alpha;
            };
            
            struct LightingCommon
            {
                Light light;
                IndirectLightingCommon indirect;
            };
            
            struct Surface
            {
                float3 Albedo; // base (diffuse or specular) color
                float3 Normal; // tangent space normal, if written
                half3 Emission;
                half Metallic; // 0=non-metal, 1=metal
                half Smoothness; // 0=rough, 1=smooth
                half Occlusion; // occlusion (default 1)
                float Alpha; // alpha for transparencies
                #if defined(_TRANSLUCENCY)
                    float Thickness; // 0 for full translucency, 1 for no translucency.
                #endif
            };
            
            #pragma target 4.0
            
            // Nature Renderer integration
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:SetupNatureRenderer nolightmap forwardadd renderinglayer
            
            // Input
            struct VertexAttributes
            {
                
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1; // optional
                float4 uv2 : TEXCOORD2; // optional
                float4 uv3 : TEXCOORD3; // optional
                
                //
                //uint instanceID : INSTANCEID_SEMANTIC;
                DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct SurfaceInput
            {
                float noise : TEXCOORD11; // TODO: pack noise into positionWS.w or normalWS.w
                
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 color : COLOR;
                float4 texCoord0 : TEXCOORD3;
                float4 texCoord1 : TEXCOORD4;
                float4 texCoord2 : TEXCOORD5;
                float4 viewDirectionWS : TEXCOORD6; //.xyz:view direction .w:fogCoord (standard)
                
                // SH or Lightmap UV
                half4 ambientOrLightmapUV : TEXCOORD7;
                
                // Lighting and shadow coordinates.
                // These are different depending on the render pipeline, so they are wrapped in
                // render pipeline specific tags.
                
                // Meta for editor visualization
                
                // Unity's default instancing settings.
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                
                // VFACE always needs to be the last semantic in the list, otherwise the compiler will throw
                // an error. So we define it here, and let the user add SHADERX_VFACE to the end of
                // the SurfaceInput struct.
                #if defined(SHADER_STAGE_FRAGMENT) //&& defined(VARYINGS_NEED_CULLFACE)
                    #define SHADERX_VFACE FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #else
                    #define SHADERX_VFACE
                #endif
                
                SHADERX_VFACE
            };
            
            // Properties
            CBUFFER_START( UnityPerMaterial )
            //
            float _AlphaTest;
            float _AlphaTestThreshold;
            
            // Fade
            float2 _ScaleFade;
            
            // Color Correction
            float3 _HSL;
            float3 _HSLVariation;
            float4 _Tint;
            float4 _TintVariation;
            float _ColorVariationSpread;
            
            // Surface Settings
            float _DoubleSidedMode;
            float _DoubleSidedNormalMode;
            
            float _VertexNormalStrength;
            float _SurfaceMapMethod;
            
            // Base Maps
            float _NormalMapScale;
            float _Metallic;
            float _Glossiness;
            
            // Surface Maps
            float2 _GlossRemap;
            float2 _OcclusionRemap;
            
            float4 _EmissionColor;
            float _EmissionIntensity;
            
            // Wind
            float _ObjectHeight;
            float _ObjectRadius;
            
            float _Wind;
            float _WindVariation;
            float _WindStrength;
            float _TurbulenceStrength;
            float _RecalculateWindNormals;
            float4 _TrunkBendFactor;
            
            float2 _WindFade;
            
            // Interaction
            
            // Translucency
            float _Translucency;
            
            float _TranslucencyBlendMode;
            float _TranslucencyStrength;
            float _TranslucencyDistortion;
            float _TranslucencyScattering;
            float4 _TranslucencyColor;
            float _TranslucencyAmbient;
            
            float2 _ThicknessRemap;
            
            // Overlay
            
            // Rendering
            float _LightingQuality;
            float _SpecularHighlights;
            float _EnvironmentReflections;
            
            CBUFFER_END
            
            SAMPLER( sampler_Albedo );
            TEXTURE2D( _Albedo );
            #ifdef _NORMALMAP
                TEXTURE2D( _NormalMap );
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                TEXTURE2D( _PackedMap );
            #endif
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                TEXTURE2D( _MetallicGlossMap );
                TEXTURE2D( _OcclusionMap );
            #endif
            
            #ifdef _EMISSION
                TEXTURE2D( _EmissionMap );
            #endif
            
            #ifdef _TRANSLUCENCY
                TEXTURE2D( _ThicknessMap );
            #endif
            
            // Include common features.
            // Properties
            #define GRASS_DEFAULT_HEIGHT 0.5
            #define PLANT_DEFAULT_HEIGHT 1.0
            #define TRUNK_DEFAULT_HEIGHT 20.0
            #define TRUNK_BASE_BEND_FACTOR 0.3
            #define TRUNK_BEND_MULTIPLIER 2.0
            
            float4 g_SmoothTime;
            float3 g_WindDirection;
            float2 g_Wind;
            float2 g_Turbulence;
            sampler2D g_GustNoise;
            
            // Properties that are calculated per-object by Nature Renderer
            #ifdef PER_OBJECT_VALUES_CALCULATED
                float g_WindFade;
                float g_ScaleFade;
                float g_WorldNoise;
                float3 g_ObjectPivot;
                float3 g_ConstantWindOffset;
                float g_PivotOffset;
                float3 g_WorldNormal;
            #endif
            
            float pow2( float x )
            {
                return x*x;
            }
            
            /// <summary>
            /// Returns the height of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectHeight()
            {
                return _ObjectHeight;
            }
            
            /// <summary>
            /// Returns the pivot of the object in world space.
            /// </summary>
            float3 GetObjectPivot()
            {
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    return g_ObjectPivot;
                #else
                    return GetAbsolutePositionWS( float3(UNITY_MATRIX_M[0].w, UNITY_MATRIX_M[1].w, UNITY_MATRIX_M[2].w) );
                #endif
            }
            
            #define GRASS_DEFAULT_RADIUS 1.0
            #define PLANT_DEFAULT_RADIUS 1.0
            #define TREE_DEFAULT_RADIUS 6.0
            
            /// <summary>
            /// Returns the radius of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectRadius()
            {
                return _ObjectRadius;
            }
            
            /// <summary>
            /// Returns the vertex normal in world space when vertex normals are anbled.
            /// Otherwise, returns the object's forward (Z+) direction.
            /// </summary>
            float3 GetWorldNormal(
                float3 normalWS, // The vertex normal in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                // New behavior, nice and simple.
                return normalWS;
                
                // Old behavior.
                /*
                #if defined(PER_OBJECT_VALUES_CALCULATED) && !defined(_TYPE_TREE_LEAVES)
                    return g_WorldNormal;
                #else
                    #ifdef _TYPE_TREE_LEAVES
                        // Scramble the vertex normals in case they are projected onto spheres
                        // or other geometry for smooth lighting. Otherwise the wind turbulence will end
                        // up as weird expanding and shrinking spheres.
                        // Define DO_NOT_SCRAMBLE_VERTEX_NORMALS in the shader if the tree models have
                        // accurate normals.
                        #ifndef DO_NOT_SCRAMBLE_VERTEX_NORMALS
                            return normalWS.xzy;
                        #else
                            return normalWS.xyz;
                        #endif
                    #else
                        return TransformObjectToWorldDir( float3(0, 0, 1) );
                    #endif
                #endif
                */
            }
            
            /// <summary>
            /// Returns the mask for the vertex.
            /// Uses the red channel of the vertex color.
            /// </summary>
            float GetVertexMask( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return 1.0;
                #else
                    #ifdef _WIND_BAKED
                        return vertexColor.r;
                    #else
                        return 1.0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Calculates the phase offset for the branch, based on the baked data.
            /// If no baked data is available, it will calculate an approximation of the branch.
            /// Should only be called for trees.
            /// </summary>
            float GetBranchPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_WIND_BAKED)
                    return vertexColor.r;
                #else
                    #if defined(_TYPE_TREE_BARK)
                        return 0;
                    #else
                        float3 offset = vertexWorldPosition - objectPivot;
                        float randomOffset = ( offset.x + offset.y + offset.z ) * 0.005;
                        return randomOffset;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the phase offset for the vertex.
            /// </summary>
            float GetPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return GetBranchPhaseOffset( vertexColor, vertexWorldPosition, objectPivot );
                #else
                    #ifdef _WIND_BAKED
                        return 1.0 - vertexColor.g;
                    #else
                        return 0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the edge flutter for the vertex,
            /// based either the vertex colors or UV (depending on the Wind Control settings).
            /// </summary>
            float GetEdgeFlutter( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_BARK)
                    return 0;
                #else
                    #if defined(_WIND_BAKED) && defined(_TYPE_TREE_LEAVES)
                        return vertexColor.g;
                    #else
                        return 1;
                    #endif
                #endif
            }
            
            float MaskFromHeightAndRadius( float3 vertex, float height, float radius )
            {
                return pow2( saturate( max(vertex.y / height, length(vertex.xz) / radius) ));
            }
            
            /// <summary>
            /// Returns a mask based on the relative height of the vertex.
            /// </summary>
            float GetHeightMask(
                float3 vertex, // The vertex position in object space.
                float4 vertexColor, // The vertex color.
                float2 uv1 ) // The second UV channel.
            {
                #if defined(_WIND_BAKED)
                    #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                        return uv1.y;
                    #else
                        return vertexColor.a;
                    #endif
                #else
                    #if defined(_TYPE_GRASS)
                        return saturate( vertex.y / GetObjectHeight() );
                    #else
                        return MaskFromHeightAndRadius( vertex, GetObjectHeight(), GetObjectRadius() );
                    #endif
                #endif
            }
            
            float Remap( float value, float2 remap )
            {
                return remap.x + value * (remap.y - remap.x);
            }
            
            float4 SmoothCurve( float4 x )
            {
                return x * x *( 3.0 - 2.0 * x );
            }
            float4 TriangleWave( float4 x )
            {
                return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
            }
            float4 SmoothTriangleWave( float4 x )
            {
                return SmoothCurve( TriangleWave( x ) );
            }
            
            float4 FastSin( float4 x )
            {
                #ifndef PI
                    #define PI 3.14159265
                #endif
                #define DIVIDE_BY_PI 1.0 / (2.0 * PI)
                return (SmoothTriangleWave( x * DIVIDE_BY_PI ) - 0.5) * 2;
            }
            
            float3 FixStretching( float3 vertex, float3 original, float3 center )
            {
                return center + SafeNormalize(vertex - center) * length(original - center);
            }
            
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
            
            float3 RotateAroundAxisFast( float3 center, float3 original, float3 direction )
            {
                return original + direction;
            }
            
            uniform sampler2D g_PerlinNoise;
            uniform float g_PerlinNoiseScale;
            
            void PerlinNoise( float2 uv, float scale, out float noise )
            {
                noise =
                    tex2Dlod(
                        g_PerlinNoise,
                        float4(uv.xy, 0, 0) * scale * g_PerlinNoiseScale).r;
            }
            
            void PerlinNoise_float( float2 uv, float scale, out float noise )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    noise = g_WorldNoise;
                #else
                    PerlinNoise( uv, scale, noise );
                #endif
            }
            
            //
            #ifndef NODE_NATURE_RENDERER_INCLUDED
                #define NODE_NATURE_RENDERER_INCLUDED
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    
                    #define Use_Macro_UNITY_MATRIX_M_instead_of_unity_ObjectToWorld unity_ObjectToWorld
                    #define Use_Macro_UNITY_MATRIX_I_M_instead_of_unity_WorldToObject unity_WorldToObject
                    
                    struct CompressedFloat4x4
                    {
                        uint positionXY;
                        uint positionZ_scale;
                        uint rotationXY;
                        uint rotationZW;
                    };
                    
                    uniform float3 _CompressionRange;
                    uniform float3 _CompressionBase;
                    
                    uint CompressToUshort( float value, float precision )
                    {
                        return (uint)(value / precision * 65535.0);
                    }
                    
                    uint CompressToByte( float value, float precision )
                    {
                        return (uint)(value / precision * 255.0);
                    }
                    
                    float DecompressFromByte( uint value, float precision )
                    {
                        return value / 255.0 * precision;
                    }
                    
                    float DecompressFromUshort( uint value, float precision )
                    {
                        return value / 65535.0 * precision;
                    }
                    
                    void _UnpackInt( uint packedInt, out uint a, out uint b )
                    {
                        a = ( (uint) (packedInt >> 16) );
                        b = ( (uint) ((packedInt << 16) >> 16) );
                    }
                    
                    void _UnpackShort( uint packedShort, out uint a, out uint b )
                    {
                        a = ( (uint) (packedShort >> 8) );
                        b = ( (uint) ((packedShort << 24) >> 24) );
                    }
                    
                    uint _PackInt( uint ushortA, uint ushortB )
                    {
                        return ushortA << 16 | ushortB;
                    }
                    
                    uint _PackShort( uint byteA, uint byteB )
                    {
                        return (byteA << 8) | byteB;
                    }
                    
                    float4x4 QuaternionToMatrix(float4 quaternion)
                    {
                        float4x4 result = (float4x4)0;
                        float x = quaternion.x;
                        float y = quaternion.y;
                        float z = quaternion.z;
                        float w = quaternion.w;
                        
                        float x2 = x + x;
                        float y2 = y + y;
                        float z2 = z + z;
                        float xx = x * x2;
                        float xy = x * y2;
                        float xz = x * z2;
                        float yy = y * y2;
                        float yz = y * z2;
                        float zz = z * z2;
                        float wx = w * x2;
                        float wy = w * y2;
                        float wz = w * z2;
                        
                        result[0][0] = 1.0 - (yy + zz);
                        result[0][1] = xy - wz;
                        result[0][2] = xz + wy;
                        
                        result[1][0] = xy + wz;
                        result[1][1] = 1.0 - (xx + zz);
                        result[1][2] = yz - wx;
                        
                        result[2][0] = xz - wy;
                        result[2][1] = yz + wx;
                        result[2][2] = 1.0 - (xx + yy);
                        
                        result[3][3] = 1.0;
                        
                        return result;
                    }
                    
                    void DecompressInstanceMatrix( inout float4x4 m, CompressedFloat4x4 compressedMatrix )
                    {
                        uint positionX, positionY, positionZ;
                        uint scaleXYZ;
                        uint rotationX, rotationY, rotationZ, rotationW;
                        
                        _UnpackInt( compressedMatrix.positionXY, positionX, positionY );
                        _UnpackInt( compressedMatrix.positionZ_scale, positionZ, scaleXYZ );
                        _UnpackInt( compressedMatrix.rotationXY, rotationX, rotationY );
                        _UnpackInt( compressedMatrix.rotationZW, rotationZ, rotationW );
                        
                        uint scaleX, scaleY;
                        _UnpackShort( scaleXYZ, scaleX, scaleY );
                        
                        float3 position =
                            float3(
                                DecompressFromUshort(positionX, _CompressionRange.x) + _CompressionBase.x,
                                DecompressFromUshort(positionY, _CompressionRange.y) + _CompressionBase.y,
                                DecompressFromUshort(positionZ, _CompressionRange.z) + _CompressionBase.z );
                        
                        float3 scale =
                            float3(
                                DecompressFromByte(scaleX, 16.0),
                                DecompressFromByte(scaleY, 16.0),
                                DecompressFromByte(scaleX, 16.0) );
                        
                        float4 rotation =
                            float4(
                                DecompressFromUshort(rotationX, 2.0) - 1.0,
                                DecompressFromUshort(rotationY, 2.0) - 1.0,
                                DecompressFromUshort(rotationZ, 2.0) - 1.0,
                                DecompressFromUshort(rotationW, 2.0) - 1.0 );
                        
                        m = QuaternionToMatrix( rotation );
                        
                        m[0][0] *= scale.x; m[1][0] *= scale.y; m[2][0] *= scale.z;
                        m[0][1] *= scale.x; m[1][1] *= scale.y; m[2][1] *= scale.z;
                        m[0][2] *= scale.x; m[1][2] *= scale.y; m[2][2] *= scale.z;
                        m[0][3] *= scale.x; m[1][3] *= scale.y; m[2][3] *= scale.z;
                        
                        m[0][3] = position.x;
                        m[1][3] = position.y;
                        m[2][3] = position.z;
                    }
                    
                    #if defined(SHADER_API_GLCORE) \
                        || defined(SHADER_API_D3D11) \
                        || defined(SHADER_API_GLES3) \
                        || defined(SHADER_API_METAL) \
                        || defined(SHADER_API_VULKAN) \
                        || defined(SHADER_API_PSSL) \
                        || defined(SHADER_API_XBOXONE)
                        uniform StructuredBuffer<CompressedFloat4x4> _NatureRendererBuffer;
                    #endif
                    
                    float4x4 inverse(float4x4 input)
                    {
                        #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
                        
                        float4x4 cofactors = float4x4(
                            minor(_22_23_24, _32_33_34, _42_43_44),
                            -minor(_21_23_24, _31_33_34, _41_43_44),
                            minor(_21_22_24, _31_32_34, _41_42_44),
                            -minor(_21_22_23, _31_32_33, _41_42_43),
                            
                            -minor(_12_13_14, _32_33_34, _42_43_44),
                            minor(_11_13_14, _31_33_34, _41_43_44),
                            -minor(_11_12_14, _31_32_34, _41_42_44),
                            minor(_11_12_13, _31_32_33, _41_42_43),
                            
                            minor(_12_13_14, _22_23_24, _42_43_44),
                            -minor(_11_13_14, _21_23_24, _41_43_44),
                            minor(_11_12_14, _21_22_24, _41_42_44),
                            -minor(_11_12_13, _21_22_23, _41_42_43),
                            
                            -minor(_12_13_14, _22_23_24, _32_33_34),
                            minor(_11_13_14, _21_23_24, _31_33_34),
                            -minor(_11_12_14, _21_22_24, _31_32_34),
                            minor(_11_12_13, _21_22_23, _31_32_33)
                            );
                        #undef minor
                        return transpose(cofactors) / determinant(input);
                    }
                #endif
                
                // Pre-calculate and cache data for Nature Shaders that relies on
                // per-object data instead of per-vertex or per-pixel.
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    void PreCalculateNatureShadersData()
                    {
                        g_ObjectPivot = GetAbsolutePositionWS( float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w) );
                        g_ConstantWindOffset = cross( GetWindDirection().xyz, float3(0,1,0) );
                        g_PivotOffset = length( float3(g_ObjectPivot.x, 0, g_ObjectPivot.z) );
                        g_WorldNormal = TransformObjectToWorldDir( float3(0, 0, 1) );
                        
                        GetFade( g_ObjectPivot, g_WindFade, g_ScaleFade );
                        PerlinNoise( g_ObjectPivot.xz, _ColorVariationSpread, g_WorldNoise);
                    }
                #endif
                
                void SetupNatureRenderer()
                {
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                        DecompressInstanceMatrix(unity_ObjectToWorld, _NatureRendererBuffer[unity_InstanceID]);
                        unity_WorldToObject = inverse(unity_ObjectToWorld);
                    #endif
                    
                    #if defined(PER_OBJECT_VALUES_CALCULATED)
                        PreCalculateNatureShadersData();
                    #endif
                }
                
                void NatureRenderer_float( float3 vertex, out float3 vertexOut )
                {
                    vertexOut = vertex;
                }
            #endif
            
            struct TranslucencyInput
            {
                float Scale;
                float NormalDistortion;
                float Scattering;
                float Thickness;
                float Ambient;
                half3 Color;
            };
            
            half3 Translucency(
                TranslucencyInput input,
                float3 bakedGI,
                float3 surfaceAlbedo,
                float3 surfaceNormal,
                float3 viewDirectionWS,
                Light light )
            {
                half3 lightDir = light.direction + surfaceNormal * input.NormalDistortion;
                half transVdotL =
                    pow( saturate( dot( viewDirectionWS, -lightDir ) ), input.Scattering ) * input.Scale;
                half3 translucency =
                    (transVdotL + bakedGI * input.Ambient)
                    * (1-input.Thickness)
                    * light.shadowAttenuation
                    * light.distanceAttenuation;
                
                return half3( surfaceAlbedo * light.color * translucency * input.Color );
            }
            
            float3 Linear_to_HSV(float3 In)
            {
                float3 sRGBLo = In * 12.92;
                float3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
                float3 Linear = float3(In <= 0.0031308) ? sRGBLo : sRGBHi;
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(Linear.bg, K.wz), float4(Linear.gb, K.xy), step(Linear.b, Linear.g));
                float4 Q = lerp(float4(P.xyw, Linear.r), float4(Linear.r, P.yzx), step(P.x, Linear.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                return float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            }
            
            float3 HSV_to_Linear(float3 In)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                float3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                float3 linearRGBLo = RGB / 12.92;
                float3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                return float3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
            }
            
            void HSL_float( float4 color, float3 hsl, out float4 colorOut )
            {
                float3 hsv = Linear_to_HSV( color.rgb );
                hsv.x += hsl.x;
                hsv.y = saturate(hsv.y + hsl.y * 0.5);
                hsv.z = saturate(hsv.z + hsl.z * 0.5);
                colorOut = float4( HSV_to_Linear(hsv), color.a );
            }
            
            struct WindInput
            {
                // Global
                float3 direction;
                float speed;
                
                // Per-Object
                float3 objectPivot;
                float fade;
                
                // Per-Vertex
                float phaseOffset;
                float3 normalWS;
                float mask;
                float flutter;
            };
            
            /// <summary>
            /// Returns the time for gusts.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothGustOffset()
            {
                return g_SmoothTime.y;
            }
            
            /// <summary>
            /// Returns the time for turbulence.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothTurbulenceOffset()
            {
                return g_SmoothTime.z;
            }
            
            /// <summary>
            /// Returns the global wind direction.
            /// </summary>
            float3 GetWindDirection()
            {
                return g_WindDirection != 0
                ? normalize(float3(g_WindDirection.x, 0, g_WindDirection.z))
                : float3(1, 0, 0);
            }
            
            /// <summary>
            /// Returns the global wind speed.
            /// </summary>
            float GetWindSpeed()
            {
                return g_Wind.x;
            }
            
            /// <summary>
            /// Returns the global wind strength.
            /// </summary>
            float GetWindStrength()
            {
                return g_Wind.y * _WindStrength;
            }
            
            /// <summary>
            /// Returns a random windVariation value based on the object pivot.
            /// </summary>
            float GetWindVariation(
                float3 objectPivot ) // The object pivot in world space.
            {
                return 1.0 - frac( objectPivot.x * objectPivot.z * 10.0 ) * _WindVariation;
            }
            
            /// <summary>
            /// Returns the global turbulence strength.
            /// </summary>
            float GetTurbulenceStrength()
            {
                return g_Turbulence.y * _TurbulenceStrength;
            }
            
            /// <summary>
            /// Returns the time for ambient wind.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothAmbientOffset()
            {
                return g_SmoothTime.x;
            }
            
            /// <summary>
            /// Calculates the wind frequency for the ambient wind.
            /// Returns a vector4 containing 4 different sine waves.
            /// </summary>
            float4 AmbientFrequency(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset, // The wind phase offset. (Range: 0-1)
                float speed ) // The wind speed.
            {
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float footprint = 3;
                float time = GetSmoothAmbientOffset() - phaseOffset * footprint;
                
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float pivotOffset = g_PivotOffset;
                #else
                    float pivotOffset = length( float3(objectPivot.x, 0, objectPivot.z) );
                #endif
                
                float scale = 0.5;
                float frequency = pivotOffset * scale - time;
                return FastSin(
                    float4(
                        frequency + constantOffset.x,
                        frequency*0.5 + constantOffset.z,
                        frequency*0.25,
                        frequency*0.125) * speed );
            }
            
            /// <summary>
            /// Calculates the ambient wind.
            /// </summary>
            float3 AmbientWind(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset ) // The wind phase offset. (Range: 0-1)
            {
                float4 sine = AmbientFrequency( objectPivot, vertexWorldPosition, windDirection, phaseOffset, 1 );
                sine.w = abs(sine.w) + 0.5;
                float xz = 1.5 * sine.x * sine.z + sine.w + 1;
                float y = 1 * sine.y * sine.z + sine.w;
                return windDirection * float3(xz, 0, xz) + float3(0, y, 0);
            }
            
            /// <summary>
            /// Calculates the turbulence.
            /// </summary>
            float3 Turbulence(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 worldNormal, // The direction of the turbulence in world space (usually vertex normal).
                float phaseOffset, // The wind phase offset.
                float edgeFlutter, // The amount of edge flutter for tree leaves. (Range: 0-1)
                float speed ) // The wind speed.
            {
                #if defined(_TYPE_TREE_BARK)
                    return float3(0, 0, 0);
                #else
                    float time = GetSmoothTurbulenceOffset() - phaseOffset;
                    float frequency = (objectPivot.x + objectPivot.y + objectPivot.z) * 2.5 - time;
                    
                    // TODO: Add a secondary frequency.
                    float4 sine =
                        FastSin(
                            float4(
                                (1.65 * frequency) * speed,
                                (2 * 1.65 * frequency) * speed,
                                0,
                                0) );
                    
                    float x = 1 * sine.x + 1;
                    float z = 1 * sine.y + 1;
                    float y = (x + z) * 0.5;
                    
                    #if defined(_TYPE_TREE_LEAVES)
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1) * edgeFlutter;
                    #else
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1);
                    #endif
                #endif
            }
            
            float3 SampleGust(
                float3 objectPivot,
                float3 vertexWorldPosition,
                float3 windDirection,
                float phaseOffset,
                float edgeFlutter,
                float lod )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                    lod = 5;
                #else
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                #endif
                
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float2 windOffsetOverTime = windDirection.xz * time + constantOffset.xz;
                #if defined(_TYPE_TREE_LEAVES)
                    float3 vertexOffset = vertexWorldPosition - objectPivot;
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime + vertexOffset.xz * 0.0075 * edgeFlutter;
                #else
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime;
                #endif
                float strength = tex2Dlod( g_GustNoise, float4(offset, 0, lod) ).r;
                return strength * windDirection;
            }
            
            float3 CombineWind(
                float3 ambient, // Large constant ambient wind motion.
                float3 gust, // Local gust based on noise texture.
                float3 turbulence, // Constant turbulence.
                float3 shiver, // High frequency shivering during gust.
                float4 strength // The wind strength for each wind component.
                )
            {
                ambient *= strength.x;
                gust *= strength.y;
                turbulence *= strength.z;
                shiver *= strength.w;
                
                // Trees require more displacement for the wind to be visible because the objects are larger.
                // These are magic numbers that give a nice balance between the grass/plants and trees,
                // based on a common tree size.
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    ambient *= 3;
                    gust *= 1;
                    turbulence *= 3;
                    shiver *= 3;
                #endif
                
                float gustLength = length( gust );
                float increaseTurbelenceWithGust = smoothstep(0, 1, gustLength) + 1;
                
                // Calculate the balance between different wind types.
                // If we do it here then we can keep the input parameters in a 0-1 range.
                ambient *= 0.1;
                gust *= 1.5;
                turbulence *= 0.15;
                shiver *= 0.15;
                
                #if defined(_DEBUG_AMBIENT)
                    return ambient;
                #elif defined(_DEBUG_GUST)
                    return gust;
                #elif defined(_DEBUG_TURBULENCE)
                    return lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #else
                    return
                    ambient
                    + gust
                    + lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #endif
            }
            
            float3 ComputeWind( WindInput input, float3 positionWS )
            {
                #if defined(_TYPE_GRASS) || defined(_TYPE_PLANT)
                    input.phaseOffset *= dot( input.direction, (positionWS - input.objectPivot) );
                    input.phaseOffset += input.mask * 0.5;
                #endif
                
                float3 ambient =
                    AmbientWind(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset );
                
                float3 gust =
                    SampleGust(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset,
                        input.flutter,
                        0 );
                
                // Add a bit of a random phase offset to the tree leaves. Phase Offset is calculated
                // per-branch and we don't want to have the same turbulence for the entire branch.
                #if defined(_TYPE_TREE_LEAVES)
                    input.phaseOffset +=
                        dot( input.direction, (positionWS - input.objectPivot) ) * input.flutter;
                #endif
                
                float3 turbulence1 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        1 );
                
                float3 turbulence2 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        2 );
                
                return CombineWind(
                    ambient,
                    gust,
                    turbulence1,
                    turbulence2,
                    float4(GetWindStrength().xx, GetTurbulenceStrength().xx) );
            }
            
            float3 ApplyWind(
                float3 positionWS, // Vertex position in world space.
                float3 objectPivot, // Object Pivot in world space.
                float3 combinedWind, // Combined Wind vector in world space.
                float mask, // Wind mask. (Range: 0-1)
                float distanceFade) // Wind distance fade. (Range: 0-1)
            {
                #if defined(_TYPE_GRASS)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade,
                        positionWS,
                        float3( positionWS.x, objectPivot.y, positionWS.z ) );
                #elif defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade * 4,
                        positionWS,
                        objectPivot);
                #else
                    return FixStretching(
                        positionWS + combinedWind * mask * mask * distanceFade,
                        positionWS,
                        objectPivot);
                #endif
            }
            
            void Wind( WindInput input, inout float3 positionWS, inout float3 normalWS )
            {
                // Adjust the pivot for grass to use the XZ position of the vertex.
                // This is a decent workaround to get a per-grass-blade pivot until
                // we have proper pivot support.
                #ifdef _TYPE_GRASS
                    input.objectPivot = float3(positionWS.x, input.objectPivot.y, positionWS.z);
                #endif
                
                // Compute wind.
                float3 wind = ComputeWind( input, positionWS );
                
                // Apply wind to vertex.
                float3 outputWS =
                    ApplyWind(
                        positionWS,
                        input.objectPivot,
                        wind,
                        input.mask,
                        input.fade );
                
                // Recalculate normals for grass
                #if defined(_TYPE_GRASS)
                    float3 delta = outputWS - positionWS;
                    normalWS =
                        lerp(
                            normalWS,
                            normalWS + SafeNormalize( delta + float3(0, 0.1, 0) ),
                            length(delta) * _RecalculateWindNormals * input.fade );
                #endif
                
                positionWS = outputWS;
            }
            
            /// <summary>
            /// Returns the bend factor for the tree trunk.
            /// X contains the bend factor for the entire tree, Y for the base.
            /// </summary>
            float2 GetTrunkBendFactor()
            {
                return _TrunkBendFactor.xy;
            }
            
            float GetTrunkMask(
                float3 vertex, float2 uv1, float treeHeight, float bendFactor, float baseBendFactor )
            {
                #ifdef _WIND_BAKED
                    float trunkMask = saturate( uv1.x * bendFactor );
                #else
                    float trunkMask = pow2(saturate( vertex.y / treeHeight )) * bendFactor;
                #endif
                
                return saturate( trunkMask + saturate( vertex.y ) * baseBendFactor );
            }
            
            void Wind_Trunk(
                float3 vertex, // The vertex position in object space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 vertexWithWind, // The vertex position with wind displacement in world space.
                float2 uv1, // The second UV channel of the vertex. (UV1)
                float3 objectPivot, // The object pivot in world space.
                float3 windDirection, // The wind direction in world space. (normalized)
                out float3 vertexOut )
            {
                // Additional properties. Either global or baked.
                float2 bendFactor = GetTrunkBendFactor();
                float trunkMask = GetTrunkMask( vertex, uv1, GetObjectHeight(), bendFactor.x, bendFactor.y );
                float ambientStrength = GetWindStrength();
                
                // Calculate Ambient Wind
                float4 trunkAmbient =
                    AmbientFrequency(
                        objectPivot,
                        vertexWorldPosition,
                        windDirection,
                        0,
                        0.75 ) + ambientStrength;
                trunkAmbient *= trunkMask;
                
                // Calculate Gust
                float3 trunkGust =
                    SampleGust(
                        objectPivot, vertexWorldPosition, windDirection, 0, 0, 7);
                trunkGust *= trunkMask;
                
                // Apply
                float gustFrequency = trunkAmbient.w * length(trunkGust);
                float baseFrequency1 = trunkAmbient.x;
                float baseFrequency2 = trunkAmbient.x + trunkAmbient.y;
                float baseFrequency =
                    lerp( baseFrequency1, baseFrequency2, (_SinTime.x + 1) * 0.5 * ambientStrength);
                
                // TODO: Use the "FixStretching" approach?
                vertexOut =
                    RotateAroundAxis(
                        objectPivot,
                        vertexWithWind,
                        normalize( cross( float3(0,1,0) , windDirection ) ),
                        (baseFrequency * 0.75 + gustFrequency) * ambientStrength * 0.0375);
            }
            
            void AlphaTest( float alpha, float threshold )
            {
                
                clip( alpha - threshold );
            }
            
            void GetFade(
                float3 objectPivot, // The object pivot in world space.
                out float windFade,
                out float scaleFade )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    windFade = 1.0;
                    scaleFade = 1.0;
                #else
                    float distanceToCamera = distance( objectPivot, _WorldSpaceCameraPos );
                    windFade = 1.0 - saturate( (distanceToCamera - _WindFade.x) / _WindFade.y );
                    scaleFade = 1.0 - saturate( (distanceToCamera - _ScaleFade.x) / _ScaleFade.y );
                #endif
            }
            
            void PerVertexFade( float3 objectPivot, out float windFade, out float scaleFade )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    windFade = g_WindFade;
                    scaleFade = g_ScaleFade;
                #else
                    GetFade(
                        objectPivot,
                        windFade,
                        scaleFade );
                #endif
            }
            
            void Wind(
                inout VertexAttributes vertex,
                inout SurfaceInput surface,
                inout float3 positionWS,
                float windFade,
                float scaleFade,
                float heightMask,
                float phaseOffset,
                float3 objectPivot )
            {
                WindInput input;
                
                // Global
                input.direction = GetWindDirection();
                input.speed = GetWindSpeed();
                
                // Per-Object
                input.objectPivot = objectPivot;
                input.fade = windFade;
                
                // Per-Vertex
                input.phaseOffset = phaseOffset;
                input.normalWS = surface.normalWS;
                float windVariation = GetWindVariation( input.objectPivot );
                float vertexMask = GetVertexMask( vertex.color );
                input.mask = heightMask * vertexMask * windVariation;
                input.flutter = GetEdgeFlutter( vertex.color );
                
                float3 vertexOut = positionWS;
                float3 normalOut = surface.normalWS;
                Wind(
                    input,
                    vertexOut,
                    normalOut );
                
                #if defined(_TYPE_TREE_LEAVES) || defined( _TYPE_TREE_BARK )
                    Wind_Trunk(
                        vertex.positionOS.xyz,
                        positionWS.xyz,
                        vertexOut.xyz,
                        vertex.uv1.xy,
                        input.objectPivot,
                        input.direction,
                        vertexOut );
                #endif
                
                positionWS = vertexOut;
                surface.normalWS = normalOut;
            }
            
            float PerVertexPerlinNoise( float3 objectPivot )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    return g_WorldNoise;
                #else
                    float noise;
                    PerlinNoise_float( objectPivot.xz, _ColorVariationSpread, noise );
                    return noise;
                #endif
            }
            
            float3 ApplyScaleFade(
                float3 vertexWorldPosition, float3 objectPivot, float fade )
            {
                vertexWorldPosition.y = lerp(objectPivot.y, vertexWorldPosition.y, max(fade, 0.2));
                return vertexWorldPosition;
            }
            
            // Surface
            void SampleAlbedo( float2 uv0, out float4 albedo )
            {
                albedo = SAMPLE_TEXTURE2D( _Albedo, sampler_Albedo, uv0.xy );
            }
            
            void ApplyColorCorrection( inout float4 albedo, float noise )
            {
                #ifdef _COLOR_HSL
                    float4 albedo1 = albedo;
                    float4 albedo2 = albedo;
                    HSL_float( albedo1, _HSL, albedo1 );
                    HSL_float( albedo2, _HSLVariation, albedo2 );
                    albedo = lerp(albedo2, albedo1, noise);
                #else
                    albedo *= lerp(_TintVariation, _Tint, noise);
                #endif
            }
            
            #ifdef _NORMALMAP
                void SampleNormalMap( float2 uv0, out float3 normal )
                {
                    normal =
                        UnpackScaleNormal(
                            SAMPLE_TEXTURE2D( _NormalMap, sampler_Albedo, uv0.xy ), _NormalMapScale ).xyz;
                }
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                void SamplePackedMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 packed = SAMPLE_TEXTURE2D(_PackedMap, sampler_Albedo, uv0.xy);
                    metallic = packed.r;
                    smoothness = Remap(packed.a, _GlossRemap.xy);
                    occlusion = Remap(packed.g, _OcclusionRemap.xy);
                }
            #endif
            
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                void SampleMetallicGlossMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 metallicGloss = SAMPLE_TEXTURE2D( _MetallicGlossMap, sampler_Albedo, uv0.xy );
                    metallic = metallicGloss.r;
                    smoothness = Remap(metallicGloss.a, _GlossRemap.xy);
                    occlusion = Remap(SAMPLE_TEXTURE2D( _OcclusionMap, sampler_Albedo, uv0.xy ).g, _OcclusionRemap.xy);
                }
            #endif
            
            void SampleMetallicGlossConstants(
                float2 uv0, out float metallic, out float smoothness, out float occlusion)
            {
                metallic = _Metallic;
                smoothness = _Glossiness;
                occlusion = 1.0;
            }
            
            #ifdef _TRANSLUCENCY
                void SampleThickness( float2 uv0, out float thickness )
                {
                    thickness = SAMPLE_TEXTURE2D( _ThicknessMap, sampler_Albedo, uv0.xy ).r;
                    thickness = Remap( thickness, _ThicknessRemap.xy );
                }
            #endif
            
            #ifdef _EMISSION
                void SampleEmission( float2 uv0, out float3 emission )
                {
                    emission = SAMPLE_TEXTURE2D( _EmissionMap, sampler_Albedo, uv0.xy ).rgb;
                    emission *= _EmissionColor.rgb;
                    emission *= _EmissionIntensity;
                }
            #endif
            
            void FlipNormals( inout float3 normals, FRONT_FACE_TYPE cullFace )
            {
                normals.z *=
                    _DoubleSidedNormalMode > 0
                    ? IS_FRONT_VFACE(cullFace, 1, -1)
                    : 1.0;
            }
            
            // Called with raw vertex data before doing any kind of calculations or transformations.
            // Useful to modify the vertex data in object space.
            void PreVertexMethod( inout VertexAttributes vertex )
            {
            }
            
            // The main vertex method. Is used to modify the vertex data and
            // the input for the surface (fragment) method.
            void VertexMethod(
                VertexAttributes vertex,
                inout SurfaceInput surface )
            {
                float3 objectPivot = GetObjectPivot();
                float3 positionWS = GetAbsolutePositionWS( surface.positionWS.xyz );
                float3 positionWSOriginal = positionWS;
                
                surface.noise = PerVertexPerlinNoise( objectPivot );
                
                float windFade;
                float scaleFade;
                PerVertexFade( objectPivot, windFade, scaleFade );
                
                float heightMask =
                    GetHeightMask(
                        vertex.positionOS.xyz,
                        vertex.color,
                        vertex.uv1.xy );
                
                float phaseOffset =
                    GetPhaseOffset(
                        vertex.color,
                        positionWS,
                        objectPivot );
                
                #ifdef _WIND
                    Wind(
                        vertex,
                        surface,
                        positionWS,
                        windFade,
                        scaleFade,
                        heightMask,
                        phaseOffset,
                        objectPivot );
                #endif
                
                positionWS =
                    ApplyScaleFade( positionWS, objectPivot, scaleFade );
                
                surface.positionWS = GetCameraRelativePositionWS( positionWS );
                surface.normalWS = lerp(TransformObjectToWorldNormal(float3(0,1,0)), surface.normalWS, _VertexNormalStrength);
            }
            
            void SurfaceMethod(
                SurfaceInput input,
                inout Surface output )
            {
                // Albedo
                float4 albedo;
                SampleAlbedo( input.texCoord0.xy, albedo );
                
                // Overlay
                
                // Alpha clip
                #ifdef _ALPHATEST
                    
                    AlphaTest( albedo.a, _AlphaTestThreshold );
                    
                #else
                    albedo.a = 1;
                #endif
                
                // Color correction
                ApplyColorCorrection( albedo, input.noise );
                output.Albedo = albedo.rgb;
                output.Alpha = albedo.a;
                
                // Normal Map
                #ifdef _NORMALMAP
                    SampleNormalMap( input.texCoord0.xy, output.Normal );
                #endif
                
                // Surface Map
                #if defined(_SURFACE_MAP_PACKED)
                    SamplePackedMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #elif defined(_SURFACE_MAP_METALLIC_GLOSS)
                    SampleMetallicGlossMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #else
                    SampleMetallicGlossConstants(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #endif
                
                // Translucency
                #ifdef _TRANSLUCENCY
                    SampleThickness( input.texCoord0.xy, output.Thickness );
                #endif
                
                // Emission
                #ifdef _EMISSION
                    SampleEmission( input.texCoord0.xy, output.Emission );
                #endif
                
                // Flip double-sided normals
                #ifdef SHADER_STAGE_FRAGMENT
                    FlipNormals( output.Normal, input.cullFace );
                #endif
            }
            
            float3 Overlay(float3 a, float3 b)
            {
                return a < 0.5
                ? 2 * a * b
                : 1 - 2 * (1-a) * (1-b);
            }
            
            void PostLightingMethod(
                SurfaceInput input,
                Surface surface,
                SurfaceCommon surfaceCommon,
                LightingCommon lightingCommon,
                inout half4 color )
            {
            }
            
            // Embed the default shader template for the current render pipeline. These templates
            // contains the actual shader code for the render-pipeline-specific lighting.
            
            // Vertex
            #if defined(SHADERPASS_SHADOWCASTER)
                float3 _LightDirection;
            #endif
            
            float4 UnityObjectToClipPos( float3 positionOS, float3 positionWS, float3 normalWS )
            {
                #if defined(SHADERPASS_SHADOWCASTER)
                    // Define shadow pass specific clip position for Universal
                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #endif
                #elif defined(SHADERPASS_META)
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #else
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #endif
                
                return positionCS;
            }
            
            SurfaceInput vert( VertexAttributes input )
            {
                SurfaceInput output = (SurfaceInput)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                PreVertexMethod( input );
                
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                output.positionWS = positionWS;
                output.normalWS = normalWS;			// normalized in TransformObjectToWorldNormal()
                output.tangentWS = tangentWS;		// normalized in TransformObjectToWorldDir()
                output.texCoord0 = input.uv0;
                output.texCoord1 = input.uv1;
                output.texCoord2 = input.uv2;
                //output.texCoord3 = input.uv3;
                output.color = input.color;
                output.viewDirectionWS.xyz = normalize( _WorldSpaceCameraPos.xyz - positionWS );
                
                VertexMethod( input, output );
                
                positionWS = output.positionWS;
                normalWS = output.normalWS;			// normalized in TransformObjectToWorldNormal()
                tangentWS = output.tangentWS;		// normalized in TransformObjectToWorldDir()
                
                // Recalculate vertex based on modifications to output
                // TODO: Only do this if the position changed
                input.positionOS = TransformWorldToObject( output.positionWS );
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                
                input.uv0 = output.texCoord0;
                input.uv1 = output.texCoord1;
                input.uv2 = output.texCoord2;
                
                #if defined(SHADERPASS_FORWARD)
                    OUTPUT_LIGHTMAP_UV(input.uv1, unity_LightmapST, output.ambientOrLightmapUV);
                    OUTPUT_SH(normalWS, output.ambientOrLightmapUV);
                #endif
                
                #ifdef VARYINGS_NEED_SCREENPOSITION
                    output.screenPosition = ComputeScreenPos(output.positionCS, _ProjectionParams.x);
                #endif
                
                return output;
            }
            
            // Fragment
            
            half4 frag(SurfaceInput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Normalize the ViewDirection per-pixel so that we have an accurate value.
                input.viewDirectionWS.xyz = normalize(input.viewDirectionWS.xyz);
                
                #ifdef LOD_FADE_CROSSFADE
                    // TODO: Dithering is not stable for shadows. Not a big issue since it is usually not noticeable, or the fade is further away than the shadow rendering distance.
                    #if !defined(SHADER_API_GLES)
                        LODDitheringTransition(
                            ComputeFadeMaskSeed(
                                GetWorldSpaceNormalizeViewDir(input.positionWS), // we need a very accurate view direction to get good dithering. The regular viewDirectionWS that we get as input is not accurate enough because it is calculated per-vertex and then interpolated. That is why we calculate the view direction again here.
                                input.positionCS.xy),
                            unity_LODFade.x);
                    #endif
                #endif
                
                Surface surface = (Surface)0;
                surface.Albedo = 1;
                surface.Emission = 0;
                surface.Metallic = 0;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                surface.Alpha = 1;
                surface.Normal = float3(0,0,1);
                SurfaceMethod( input, surface );
                
                return 0;
            }
            
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            
            Blend One Zero, One Zero
            ZWrite Off
            
            Cull [_DoubleSidedMode]
            
            ZTest LEqual
            
            // TODO: Make sure this works on all platforms.
            
            // Embed the default pass setup.
            // This will overwrite any values that need to be different for specifc passes.
            
            HLSLPROGRAM
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Nature Shaders Settings
            #define NATURE_SHADERS
            
            #define _TYPE_GRASS
            
            // Nature Renderer calculates values that are the same for all vertices once
            // for each object. This is a nice optimization that reduces per-vertex calculations.
            // This only works if Procedural Instancing is enabled.
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                #define PER_OBJECT_VALUES_CALCULATED
            #endif
            
            // Shader Features
            #ifdef _ALPHATEST
                #define _ALPHA_CLIP_ON
                #define _ALPHATEST_ON // HDRP
            #else
                #define _ALPHA_CLIP_OFF
                #define _ALPHATEST_OFF // HDRP
                #define _ALPHA_CLIP_DISABLED
            #endif
            
            #pragma shader_feature_local _COLOR_TINT _COLOR_HSL
            
            #pragma shader_feature_local _WIND_OFF _WIND_AUTO _WIND_BAKED
            #if defined(_WIND_AUTO) || defined(_WIND_BAKED)
                #define _WIND
                #define _WIND_ON
            #endif
            
            // Already defined in Lit template.
            // #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            
            // Already defined in Lit template.
            //#pragma shader_feature_local _EMISSION
            
            #ifndef _INTERACTION_OFF
                #define _INTERACTION_OFF
            #endif
            
            #pragma shader_feature_local _TRANSLUCENCY
            #ifdef _TRANSLUCENCY
                #define _TRANSLUCENCY_ON
                #define _MATERIAL_FEATURE_SUBSURFACE_SCATTERING 1
                #define _MATERIAL_FEATURE_TRANSMISSION 1
                
            #endif
            
            // Include the default cginc files and configurations
            // that are required for the current render pipeline.
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Default global keywords for material quality.
            // #pragma multi_compile MATERIAL_QUALITY_HIGH MATERIAL_QUALITY_MEDIUM MATERIAL_QUALITY_LOW
            #pragma shader_feature_local _LIGHTING_QUALITY_HIGH _LIGHTING_QUALITY_LOW
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            
            #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            #pragma shader_feature_local _ALPHATEST
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _EMISSION
            
            #pragma multi_compile_instancing
            
            #ifdef _SPECULARHIGHLIGHTS_OFF
                #define _ENVIRONMENTREFLECTIONS_OFF
                #define _GLOSSYREFLECTIONS_OFF
            #endif
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            //#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            
            // Variants
            
            // Attributes and Varyings
            // These are all the possible settings for the URP shaders.
            //#define _SURFACE_TYPE_TRANSPARENT 1
            //#define _AlphaClip 1
            //#define _NORMALMAP 1
            //#define _SPECULAR_SETUP
            //#define _BLENDMODE_ADD 1
            //#define _ALPHAPREMULTIPLY_ON 1
            #define _NORMAL_DROPOFF_TS 1
            //#define _NORMAL_DROPOFF_OS 1
            //#define _NORMAL_DROPOFF_WS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            //#define ATTRIBUTES_NEED_TEXCOORD0
            //#define ATTRIBUTES_NEED_TEXCOORD1
            //#define ATTRIBUTES_NEED_TEXCOORD2
            //#define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            //#define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            //#define VARYINGS_NEED_VIEWDIRECTION_WS
            //#define VARYINGS_NEED_BITANGENT_WS
            //#define VARYINGS_NEED_SCREENPOSITION
            //#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_CULLFACE
            
            #ifdef _MAIN_LIGHT_SHADOWS
                #define VARYINGS_NEED_SHADOWCOORDS
            #endif
            
            #define SHADERPASS_DEPTHONLY
            
            // Common API to unify the different render pipelines.
            
            // Return absolute world position of current object
            float3 GetObjectAbsolutePositionWS()
            {
                float4x4 modelMatrix = UNITY_MATRIX_M;
                return GetAbsolutePositionWS(modelMatrix._m03_m13_m23); // Translation object to world
            }
            
            float3 GetPrimaryCameraPosition()
            {
                #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
                    return float3(0, 0, 0);
                #else
                    return _WorldSpaceCameraPos;
                #endif
            }
            
            // Unity 2020.2 already includes these methods.
            
            uint2 ComputeFadeMaskSeed(float3 V, uint2 positionSS)
            {
                uint2 fadeMaskSeed;
                if (IsPerspectiveProjection())
                {
                    float2 pv = PackNormalOctQuadEncode(V);
                    pv *= _ScreenParams.xy;
                    pv *= UNITY_MATRIX_P._m00_m11;
                    fadeMaskSeed = asuint((int2)pv);
                }
                else
                {
                    fadeMaskSeed = positionSS;
                }
                
                return fadeMaskSeed;
            }
            
            half3 UnpackScaleNormalRGorAG(half4 packednormal, half bumpScale)
            {
                #if defined(UNITY_NO_DXT5nm)
                    half3 normal = packednormal.xyz * 2 - 1;
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    return normal;
                #else
                    // This do the trick
                    packednormal.x *= packednormal.w;
                    
                    half3 normal;
                    normal.xy = (packednormal.xy * 2 - 1);
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
                    return normal;
                #endif
            }
            
            half3 UnpackScaleNormal(half4 packednormal, half bumpScale)
            {
                return UnpackScaleNormalRGorAG(packednormal, bumpScale);
            }
            
            struct IndirectLightingCommon
            {
                half3 diffuse;
                half3 specular;
            };
            
            struct SurfaceCommon
            {
                half3 diffuse;
                half3 specular;
                half3 normalWS;
                half oneMinusReflectivity;
                half smoothness;
                half alpha;
            };
            
            struct LightingCommon
            {
                Light light;
                IndirectLightingCommon indirect;
            };
            
            struct Surface
            {
                float3 Albedo; // base (diffuse or specular) color
                float3 Normal; // tangent space normal, if written
                half3 Emission;
                half Metallic; // 0=non-metal, 1=metal
                half Smoothness; // 0=rough, 1=smooth
                half Occlusion; // occlusion (default 1)
                float Alpha; // alpha for transparencies
                #if defined(_TRANSLUCENCY)
                    float Thickness; // 0 for full translucency, 1 for no translucency.
                #endif
            };
            
            #pragma target 4.0
            
            // Nature Renderer integration
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:SetupNatureRenderer nolightmap forwardadd renderinglayer
            
            // Input
            struct VertexAttributes
            {
                
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1; // optional
                float4 uv2 : TEXCOORD2; // optional
                float4 uv3 : TEXCOORD3; // optional
                
                //
                //uint instanceID : INSTANCEID_SEMANTIC;
                DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct SurfaceInput
            {
                float noise : TEXCOORD11; // TODO: pack noise into positionWS.w or normalWS.w
                
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 color : COLOR;
                float4 texCoord0 : TEXCOORD3;
                float4 texCoord1 : TEXCOORD4;
                float4 texCoord2 : TEXCOORD5;
                float4 viewDirectionWS : TEXCOORD6; //.xyz:view direction .w:fogCoord (standard)
                
                // SH or Lightmap UV
                half4 ambientOrLightmapUV : TEXCOORD7;
                
                // Lighting and shadow coordinates.
                // These are different depending on the render pipeline, so they are wrapped in
                // render pipeline specific tags.
                
                // Meta for editor visualization
                
                // Unity's default instancing settings.
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                
                // VFACE always needs to be the last semantic in the list, otherwise the compiler will throw
                // an error. So we define it here, and let the user add SHADERX_VFACE to the end of
                // the SurfaceInput struct.
                #if defined(SHADER_STAGE_FRAGMENT) //&& defined(VARYINGS_NEED_CULLFACE)
                    #define SHADERX_VFACE FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #else
                    #define SHADERX_VFACE
                #endif
                
                SHADERX_VFACE
            };
            
            // Properties
            CBUFFER_START( UnityPerMaterial )
            //
            float _AlphaTest;
            float _AlphaTestThreshold;
            
            // Fade
            float2 _ScaleFade;
            
            // Color Correction
            float3 _HSL;
            float3 _HSLVariation;
            float4 _Tint;
            float4 _TintVariation;
            float _ColorVariationSpread;
            
            // Surface Settings
            float _DoubleSidedMode;
            float _DoubleSidedNormalMode;
            
            float _VertexNormalStrength;
            float _SurfaceMapMethod;
            
            // Base Maps
            float _NormalMapScale;
            float _Metallic;
            float _Glossiness;
            
            // Surface Maps
            float2 _GlossRemap;
            float2 _OcclusionRemap;
            
            float4 _EmissionColor;
            float _EmissionIntensity;
            
            // Wind
            float _ObjectHeight;
            float _ObjectRadius;
            
            float _Wind;
            float _WindVariation;
            float _WindStrength;
            float _TurbulenceStrength;
            float _RecalculateWindNormals;
            float4 _TrunkBendFactor;
            
            float2 _WindFade;
            
            // Interaction
            
            // Translucency
            float _Translucency;
            
            float _TranslucencyBlendMode;
            float _TranslucencyStrength;
            float _TranslucencyDistortion;
            float _TranslucencyScattering;
            float4 _TranslucencyColor;
            float _TranslucencyAmbient;
            
            float2 _ThicknessRemap;
            
            // Overlay
            
            // Rendering
            float _LightingQuality;
            float _SpecularHighlights;
            float _EnvironmentReflections;
            
            CBUFFER_END
            
            SAMPLER( sampler_Albedo );
            TEXTURE2D( _Albedo );
            #ifdef _NORMALMAP
                TEXTURE2D( _NormalMap );
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                TEXTURE2D( _PackedMap );
            #endif
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                TEXTURE2D( _MetallicGlossMap );
                TEXTURE2D( _OcclusionMap );
            #endif
            
            #ifdef _EMISSION
                TEXTURE2D( _EmissionMap );
            #endif
            
            #ifdef _TRANSLUCENCY
                TEXTURE2D( _ThicknessMap );
            #endif
            
            // Include common features.
            // Properties
            #define GRASS_DEFAULT_HEIGHT 0.5
            #define PLANT_DEFAULT_HEIGHT 1.0
            #define TRUNK_DEFAULT_HEIGHT 20.0
            #define TRUNK_BASE_BEND_FACTOR 0.3
            #define TRUNK_BEND_MULTIPLIER 2.0
            
            float4 g_SmoothTime;
            float3 g_WindDirection;
            float2 g_Wind;
            float2 g_Turbulence;
            sampler2D g_GustNoise;
            
            // Properties that are calculated per-object by Nature Renderer
            #ifdef PER_OBJECT_VALUES_CALCULATED
                float g_WindFade;
                float g_ScaleFade;
                float g_WorldNoise;
                float3 g_ObjectPivot;
                float3 g_ConstantWindOffset;
                float g_PivotOffset;
                float3 g_WorldNormal;
            #endif
            
            float pow2( float x )
            {
                return x*x;
            }
            
            /// <summary>
            /// Returns the height of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectHeight()
            {
                return _ObjectHeight;
            }
            
            /// <summary>
            /// Returns the pivot of the object in world space.
            /// </summary>
            float3 GetObjectPivot()
            {
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    return g_ObjectPivot;
                #else
                    return GetAbsolutePositionWS( float3(UNITY_MATRIX_M[0].w, UNITY_MATRIX_M[1].w, UNITY_MATRIX_M[2].w) );
                #endif
            }
            
            #define GRASS_DEFAULT_RADIUS 1.0
            #define PLANT_DEFAULT_RADIUS 1.0
            #define TREE_DEFAULT_RADIUS 6.0
            
            /// <summary>
            /// Returns the radius of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectRadius()
            {
                return _ObjectRadius;
            }
            
            /// <summary>
            /// Returns the vertex normal in world space when vertex normals are anbled.
            /// Otherwise, returns the object's forward (Z+) direction.
            /// </summary>
            float3 GetWorldNormal(
                float3 normalWS, // The vertex normal in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                // New behavior, nice and simple.
                return normalWS;
                
                // Old behavior.
                /*
                #if defined(PER_OBJECT_VALUES_CALCULATED) && !defined(_TYPE_TREE_LEAVES)
                    return g_WorldNormal;
                #else
                    #ifdef _TYPE_TREE_LEAVES
                        // Scramble the vertex normals in case they are projected onto spheres
                        // or other geometry for smooth lighting. Otherwise the wind turbulence will end
                        // up as weird expanding and shrinking spheres.
                        // Define DO_NOT_SCRAMBLE_VERTEX_NORMALS in the shader if the tree models have
                        // accurate normals.
                        #ifndef DO_NOT_SCRAMBLE_VERTEX_NORMALS
                            return normalWS.xzy;
                        #else
                            return normalWS.xyz;
                        #endif
                    #else
                        return TransformObjectToWorldDir( float3(0, 0, 1) );
                    #endif
                #endif
                */
            }
            
            /// <summary>
            /// Returns the mask for the vertex.
            /// Uses the red channel of the vertex color.
            /// </summary>
            float GetVertexMask( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return 1.0;
                #else
                    #ifdef _WIND_BAKED
                        return vertexColor.r;
                    #else
                        return 1.0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Calculates the phase offset for the branch, based on the baked data.
            /// If no baked data is available, it will calculate an approximation of the branch.
            /// Should only be called for trees.
            /// </summary>
            float GetBranchPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_WIND_BAKED)
                    return vertexColor.r;
                #else
                    #if defined(_TYPE_TREE_BARK)
                        return 0;
                    #else
                        float3 offset = vertexWorldPosition - objectPivot;
                        float randomOffset = ( offset.x + offset.y + offset.z ) * 0.005;
                        return randomOffset;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the phase offset for the vertex.
            /// </summary>
            float GetPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return GetBranchPhaseOffset( vertexColor, vertexWorldPosition, objectPivot );
                #else
                    #ifdef _WIND_BAKED
                        return 1.0 - vertexColor.g;
                    #else
                        return 0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the edge flutter for the vertex,
            /// based either the vertex colors or UV (depending on the Wind Control settings).
            /// </summary>
            float GetEdgeFlutter( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_BARK)
                    return 0;
                #else
                    #if defined(_WIND_BAKED) && defined(_TYPE_TREE_LEAVES)
                        return vertexColor.g;
                    #else
                        return 1;
                    #endif
                #endif
            }
            
            float MaskFromHeightAndRadius( float3 vertex, float height, float radius )
            {
                return pow2( saturate( max(vertex.y / height, length(vertex.xz) / radius) ));
            }
            
            /// <summary>
            /// Returns a mask based on the relative height of the vertex.
            /// </summary>
            float GetHeightMask(
                float3 vertex, // The vertex position in object space.
                float4 vertexColor, // The vertex color.
                float2 uv1 ) // The second UV channel.
            {
                #if defined(_WIND_BAKED)
                    #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                        return uv1.y;
                    #else
                        return vertexColor.a;
                    #endif
                #else
                    #if defined(_TYPE_GRASS)
                        return saturate( vertex.y / GetObjectHeight() );
                    #else
                        return MaskFromHeightAndRadius( vertex, GetObjectHeight(), GetObjectRadius() );
                    #endif
                #endif
            }
            
            float Remap( float value, float2 remap )
            {
                return remap.x + value * (remap.y - remap.x);
            }
            
            float4 SmoothCurve( float4 x )
            {
                return x * x *( 3.0 - 2.0 * x );
            }
            float4 TriangleWave( float4 x )
            {
                return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
            }
            float4 SmoothTriangleWave( float4 x )
            {
                return SmoothCurve( TriangleWave( x ) );
            }
            
            float4 FastSin( float4 x )
            {
                #ifndef PI
                    #define PI 3.14159265
                #endif
                #define DIVIDE_BY_PI 1.0 / (2.0 * PI)
                return (SmoothTriangleWave( x * DIVIDE_BY_PI ) - 0.5) * 2;
            }
            
            float3 FixStretching( float3 vertex, float3 original, float3 center )
            {
                return center + SafeNormalize(vertex - center) * length(original - center);
            }
            
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
            
            float3 RotateAroundAxisFast( float3 center, float3 original, float3 direction )
            {
                return original + direction;
            }
            
            uniform sampler2D g_PerlinNoise;
            uniform float g_PerlinNoiseScale;
            
            void PerlinNoise( float2 uv, float scale, out float noise )
            {
                noise =
                    tex2Dlod(
                        g_PerlinNoise,
                        float4(uv.xy, 0, 0) * scale * g_PerlinNoiseScale).r;
            }
            
            void PerlinNoise_float( float2 uv, float scale, out float noise )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    noise = g_WorldNoise;
                #else
                    PerlinNoise( uv, scale, noise );
                #endif
            }
            
            //
            #ifndef NODE_NATURE_RENDERER_INCLUDED
                #define NODE_NATURE_RENDERER_INCLUDED
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    
                    #define Use_Macro_UNITY_MATRIX_M_instead_of_unity_ObjectToWorld unity_ObjectToWorld
                    #define Use_Macro_UNITY_MATRIX_I_M_instead_of_unity_WorldToObject unity_WorldToObject
                    
                    struct CompressedFloat4x4
                    {
                        uint positionXY;
                        uint positionZ_scale;
                        uint rotationXY;
                        uint rotationZW;
                    };
                    
                    uniform float3 _CompressionRange;
                    uniform float3 _CompressionBase;
                    
                    uint CompressToUshort( float value, float precision )
                    {
                        return (uint)(value / precision * 65535.0);
                    }
                    
                    uint CompressToByte( float value, float precision )
                    {
                        return (uint)(value / precision * 255.0);
                    }
                    
                    float DecompressFromByte( uint value, float precision )
                    {
                        return value / 255.0 * precision;
                    }
                    
                    float DecompressFromUshort( uint value, float precision )
                    {
                        return value / 65535.0 * precision;
                    }
                    
                    void _UnpackInt( uint packedInt, out uint a, out uint b )
                    {
                        a = ( (uint) (packedInt >> 16) );
                        b = ( (uint) ((packedInt << 16) >> 16) );
                    }
                    
                    void _UnpackShort( uint packedShort, out uint a, out uint b )
                    {
                        a = ( (uint) (packedShort >> 8) );
                        b = ( (uint) ((packedShort << 24) >> 24) );
                    }
                    
                    uint _PackInt( uint ushortA, uint ushortB )
                    {
                        return ushortA << 16 | ushortB;
                    }
                    
                    uint _PackShort( uint byteA, uint byteB )
                    {
                        return (byteA << 8) | byteB;
                    }
                    
                    float4x4 QuaternionToMatrix(float4 quaternion)
                    {
                        float4x4 result = (float4x4)0;
                        float x = quaternion.x;
                        float y = quaternion.y;
                        float z = quaternion.z;
                        float w = quaternion.w;
                        
                        float x2 = x + x;
                        float y2 = y + y;
                        float z2 = z + z;
                        float xx = x * x2;
                        float xy = x * y2;
                        float xz = x * z2;
                        float yy = y * y2;
                        float yz = y * z2;
                        float zz = z * z2;
                        float wx = w * x2;
                        float wy = w * y2;
                        float wz = w * z2;
                        
                        result[0][0] = 1.0 - (yy + zz);
                        result[0][1] = xy - wz;
                        result[0][2] = xz + wy;
                        
                        result[1][0] = xy + wz;
                        result[1][1] = 1.0 - (xx + zz);
                        result[1][2] = yz - wx;
                        
                        result[2][0] = xz - wy;
                        result[2][1] = yz + wx;
                        result[2][2] = 1.0 - (xx + yy);
                        
                        result[3][3] = 1.0;
                        
                        return result;
                    }
                    
                    void DecompressInstanceMatrix( inout float4x4 m, CompressedFloat4x4 compressedMatrix )
                    {
                        uint positionX, positionY, positionZ;
                        uint scaleXYZ;
                        uint rotationX, rotationY, rotationZ, rotationW;
                        
                        _UnpackInt( compressedMatrix.positionXY, positionX, positionY );
                        _UnpackInt( compressedMatrix.positionZ_scale, positionZ, scaleXYZ );
                        _UnpackInt( compressedMatrix.rotationXY, rotationX, rotationY );
                        _UnpackInt( compressedMatrix.rotationZW, rotationZ, rotationW );
                        
                        uint scaleX, scaleY;
                        _UnpackShort( scaleXYZ, scaleX, scaleY );
                        
                        float3 position =
                            float3(
                                DecompressFromUshort(positionX, _CompressionRange.x) + _CompressionBase.x,
                                DecompressFromUshort(positionY, _CompressionRange.y) + _CompressionBase.y,
                                DecompressFromUshort(positionZ, _CompressionRange.z) + _CompressionBase.z );
                        
                        float3 scale =
                            float3(
                                DecompressFromByte(scaleX, 16.0),
                                DecompressFromByte(scaleY, 16.0),
                                DecompressFromByte(scaleX, 16.0) );
                        
                        float4 rotation =
                            float4(
                                DecompressFromUshort(rotationX, 2.0) - 1.0,
                                DecompressFromUshort(rotationY, 2.0) - 1.0,
                                DecompressFromUshort(rotationZ, 2.0) - 1.0,
                                DecompressFromUshort(rotationW, 2.0) - 1.0 );
                        
                        m = QuaternionToMatrix( rotation );
                        
                        m[0][0] *= scale.x; m[1][0] *= scale.y; m[2][0] *= scale.z;
                        m[0][1] *= scale.x; m[1][1] *= scale.y; m[2][1] *= scale.z;
                        m[0][2] *= scale.x; m[1][2] *= scale.y; m[2][2] *= scale.z;
                        m[0][3] *= scale.x; m[1][3] *= scale.y; m[2][3] *= scale.z;
                        
                        m[0][3] = position.x;
                        m[1][3] = position.y;
                        m[2][3] = position.z;
                    }
                    
                    #if defined(SHADER_API_GLCORE) \
                        || defined(SHADER_API_D3D11) \
                        || defined(SHADER_API_GLES3) \
                        || defined(SHADER_API_METAL) \
                        || defined(SHADER_API_VULKAN) \
                        || defined(SHADER_API_PSSL) \
                        || defined(SHADER_API_XBOXONE)
                        uniform StructuredBuffer<CompressedFloat4x4> _NatureRendererBuffer;
                    #endif
                    
                    float4x4 inverse(float4x4 input)
                    {
                        #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
                        
                        float4x4 cofactors = float4x4(
                            minor(_22_23_24, _32_33_34, _42_43_44),
                            -minor(_21_23_24, _31_33_34, _41_43_44),
                            minor(_21_22_24, _31_32_34, _41_42_44),
                            -minor(_21_22_23, _31_32_33, _41_42_43),
                            
                            -minor(_12_13_14, _32_33_34, _42_43_44),
                            minor(_11_13_14, _31_33_34, _41_43_44),
                            -minor(_11_12_14, _31_32_34, _41_42_44),
                            minor(_11_12_13, _31_32_33, _41_42_43),
                            
                            minor(_12_13_14, _22_23_24, _42_43_44),
                            -minor(_11_13_14, _21_23_24, _41_43_44),
                            minor(_11_12_14, _21_22_24, _41_42_44),
                            -minor(_11_12_13, _21_22_23, _41_42_43),
                            
                            -minor(_12_13_14, _22_23_24, _32_33_34),
                            minor(_11_13_14, _21_23_24, _31_33_34),
                            -minor(_11_12_14, _21_22_24, _31_32_34),
                            minor(_11_12_13, _21_22_23, _31_32_33)
                            );
                        #undef minor
                        return transpose(cofactors) / determinant(input);
                    }
                #endif
                
                // Pre-calculate and cache data for Nature Shaders that relies on
                // per-object data instead of per-vertex or per-pixel.
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    void PreCalculateNatureShadersData()
                    {
                        g_ObjectPivot = GetAbsolutePositionWS( float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w) );
                        g_ConstantWindOffset = cross( GetWindDirection().xyz, float3(0,1,0) );
                        g_PivotOffset = length( float3(g_ObjectPivot.x, 0, g_ObjectPivot.z) );
                        g_WorldNormal = TransformObjectToWorldDir( float3(0, 0, 1) );
                        
                        GetFade( g_ObjectPivot, g_WindFade, g_ScaleFade );
                        PerlinNoise( g_ObjectPivot.xz, _ColorVariationSpread, g_WorldNoise);
                    }
                #endif
                
                void SetupNatureRenderer()
                {
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                        DecompressInstanceMatrix(unity_ObjectToWorld, _NatureRendererBuffer[unity_InstanceID]);
                        unity_WorldToObject = inverse(unity_ObjectToWorld);
                    #endif
                    
                    #if defined(PER_OBJECT_VALUES_CALCULATED)
                        PreCalculateNatureShadersData();
                    #endif
                }
                
                void NatureRenderer_float( float3 vertex, out float3 vertexOut )
                {
                    vertexOut = vertex;
                }
            #endif
            
            struct TranslucencyInput
            {
                float Scale;
                float NormalDistortion;
                float Scattering;
                float Thickness;
                float Ambient;
                half3 Color;
            };
            
            half3 Translucency(
                TranslucencyInput input,
                float3 bakedGI,
                float3 surfaceAlbedo,
                float3 surfaceNormal,
                float3 viewDirectionWS,
                Light light )
            {
                half3 lightDir = light.direction + surfaceNormal * input.NormalDistortion;
                half transVdotL =
                    pow( saturate( dot( viewDirectionWS, -lightDir ) ), input.Scattering ) * input.Scale;
                half3 translucency =
                    (transVdotL + bakedGI * input.Ambient)
                    * (1-input.Thickness)
                    * light.shadowAttenuation
                    * light.distanceAttenuation;
                
                return half3( surfaceAlbedo * light.color * translucency * input.Color );
            }
            
            float3 Linear_to_HSV(float3 In)
            {
                float3 sRGBLo = In * 12.92;
                float3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
                float3 Linear = float3(In <= 0.0031308) ? sRGBLo : sRGBHi;
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(Linear.bg, K.wz), float4(Linear.gb, K.xy), step(Linear.b, Linear.g));
                float4 Q = lerp(float4(P.xyw, Linear.r), float4(Linear.r, P.yzx), step(P.x, Linear.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                return float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            }
            
            float3 HSV_to_Linear(float3 In)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                float3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                float3 linearRGBLo = RGB / 12.92;
                float3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                return float3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
            }
            
            void HSL_float( float4 color, float3 hsl, out float4 colorOut )
            {
                float3 hsv = Linear_to_HSV( color.rgb );
                hsv.x += hsl.x;
                hsv.y = saturate(hsv.y + hsl.y * 0.5);
                hsv.z = saturate(hsv.z + hsl.z * 0.5);
                colorOut = float4( HSV_to_Linear(hsv), color.a );
            }
            
            struct WindInput
            {
                // Global
                float3 direction;
                float speed;
                
                // Per-Object
                float3 objectPivot;
                float fade;
                
                // Per-Vertex
                float phaseOffset;
                float3 normalWS;
                float mask;
                float flutter;
            };
            
            /// <summary>
            /// Returns the time for gusts.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothGustOffset()
            {
                return g_SmoothTime.y;
            }
            
            /// <summary>
            /// Returns the time for turbulence.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothTurbulenceOffset()
            {
                return g_SmoothTime.z;
            }
            
            /// <summary>
            /// Returns the global wind direction.
            /// </summary>
            float3 GetWindDirection()
            {
                return g_WindDirection != 0
                ? normalize(float3(g_WindDirection.x, 0, g_WindDirection.z))
                : float3(1, 0, 0);
            }
            
            /// <summary>
            /// Returns the global wind speed.
            /// </summary>
            float GetWindSpeed()
            {
                return g_Wind.x;
            }
            
            /// <summary>
            /// Returns the global wind strength.
            /// </summary>
            float GetWindStrength()
            {
                return g_Wind.y * _WindStrength;
            }
            
            /// <summary>
            /// Returns a random windVariation value based on the object pivot.
            /// </summary>
            float GetWindVariation(
                float3 objectPivot ) // The object pivot in world space.
            {
                return 1.0 - frac( objectPivot.x * objectPivot.z * 10.0 ) * _WindVariation;
            }
            
            /// <summary>
            /// Returns the global turbulence strength.
            /// </summary>
            float GetTurbulenceStrength()
            {
                return g_Turbulence.y * _TurbulenceStrength;
            }
            
            /// <summary>
            /// Returns the time for ambient wind.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothAmbientOffset()
            {
                return g_SmoothTime.x;
            }
            
            /// <summary>
            /// Calculates the wind frequency for the ambient wind.
            /// Returns a vector4 containing 4 different sine waves.
            /// </summary>
            float4 AmbientFrequency(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset, // The wind phase offset. (Range: 0-1)
                float speed ) // The wind speed.
            {
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float footprint = 3;
                float time = GetSmoothAmbientOffset() - phaseOffset * footprint;
                
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float pivotOffset = g_PivotOffset;
                #else
                    float pivotOffset = length( float3(objectPivot.x, 0, objectPivot.z) );
                #endif
                
                float scale = 0.5;
                float frequency = pivotOffset * scale - time;
                return FastSin(
                    float4(
                        frequency + constantOffset.x,
                        frequency*0.5 + constantOffset.z,
                        frequency*0.25,
                        frequency*0.125) * speed );
            }
            
            /// <summary>
            /// Calculates the ambient wind.
            /// </summary>
            float3 AmbientWind(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset ) // The wind phase offset. (Range: 0-1)
            {
                float4 sine = AmbientFrequency( objectPivot, vertexWorldPosition, windDirection, phaseOffset, 1 );
                sine.w = abs(sine.w) + 0.5;
                float xz = 1.5 * sine.x * sine.z + sine.w + 1;
                float y = 1 * sine.y * sine.z + sine.w;
                return windDirection * float3(xz, 0, xz) + float3(0, y, 0);
            }
            
            /// <summary>
            /// Calculates the turbulence.
            /// </summary>
            float3 Turbulence(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 worldNormal, // The direction of the turbulence in world space (usually vertex normal).
                float phaseOffset, // The wind phase offset.
                float edgeFlutter, // The amount of edge flutter for tree leaves. (Range: 0-1)
                float speed ) // The wind speed.
            {
                #if defined(_TYPE_TREE_BARK)
                    return float3(0, 0, 0);
                #else
                    float time = GetSmoothTurbulenceOffset() - phaseOffset;
                    float frequency = (objectPivot.x + objectPivot.y + objectPivot.z) * 2.5 - time;
                    
                    // TODO: Add a secondary frequency.
                    float4 sine =
                        FastSin(
                            float4(
                                (1.65 * frequency) * speed,
                                (2 * 1.65 * frequency) * speed,
                                0,
                                0) );
                    
                    float x = 1 * sine.x + 1;
                    float z = 1 * sine.y + 1;
                    float y = (x + z) * 0.5;
                    
                    #if defined(_TYPE_TREE_LEAVES)
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1) * edgeFlutter;
                    #else
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1);
                    #endif
                #endif
            }
            
            float3 SampleGust(
                float3 objectPivot,
                float3 vertexWorldPosition,
                float3 windDirection,
                float phaseOffset,
                float edgeFlutter,
                float lod )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                    lod = 5;
                #else
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                #endif
                
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float2 windOffsetOverTime = windDirection.xz * time + constantOffset.xz;
                #if defined(_TYPE_TREE_LEAVES)
                    float3 vertexOffset = vertexWorldPosition - objectPivot;
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime + vertexOffset.xz * 0.0075 * edgeFlutter;
                #else
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime;
                #endif
                float strength = tex2Dlod( g_GustNoise, float4(offset, 0, lod) ).r;
                return strength * windDirection;
            }
            
            float3 CombineWind(
                float3 ambient, // Large constant ambient wind motion.
                float3 gust, // Local gust based on noise texture.
                float3 turbulence, // Constant turbulence.
                float3 shiver, // High frequency shivering during gust.
                float4 strength // The wind strength for each wind component.
                )
            {
                ambient *= strength.x;
                gust *= strength.y;
                turbulence *= strength.z;
                shiver *= strength.w;
                
                // Trees require more displacement for the wind to be visible because the objects are larger.
                // These are magic numbers that give a nice balance between the grass/plants and trees,
                // based on a common tree size.
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    ambient *= 3;
                    gust *= 1;
                    turbulence *= 3;
                    shiver *= 3;
                #endif
                
                float gustLength = length( gust );
                float increaseTurbelenceWithGust = smoothstep(0, 1, gustLength) + 1;
                
                // Calculate the balance between different wind types.
                // If we do it here then we can keep the input parameters in a 0-1 range.
                ambient *= 0.1;
                gust *= 1.5;
                turbulence *= 0.15;
                shiver *= 0.15;
                
                #if defined(_DEBUG_AMBIENT)
                    return ambient;
                #elif defined(_DEBUG_GUST)
                    return gust;
                #elif defined(_DEBUG_TURBULENCE)
                    return lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #else
                    return
                    ambient
                    + gust
                    + lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #endif
            }
            
            float3 ComputeWind( WindInput input, float3 positionWS )
            {
                #if defined(_TYPE_GRASS) || defined(_TYPE_PLANT)
                    input.phaseOffset *= dot( input.direction, (positionWS - input.objectPivot) );
                    input.phaseOffset += input.mask * 0.5;
                #endif
                
                float3 ambient =
                    AmbientWind(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset );
                
                float3 gust =
                    SampleGust(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset,
                        input.flutter,
                        0 );
                
                // Add a bit of a random phase offset to the tree leaves. Phase Offset is calculated
                // per-branch and we don't want to have the same turbulence for the entire branch.
                #if defined(_TYPE_TREE_LEAVES)
                    input.phaseOffset +=
                        dot( input.direction, (positionWS - input.objectPivot) ) * input.flutter;
                #endif
                
                float3 turbulence1 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        1 );
                
                float3 turbulence2 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        2 );
                
                return CombineWind(
                    ambient,
                    gust,
                    turbulence1,
                    turbulence2,
                    float4(GetWindStrength().xx, GetTurbulenceStrength().xx) );
            }
            
            float3 ApplyWind(
                float3 positionWS, // Vertex position in world space.
                float3 objectPivot, // Object Pivot in world space.
                float3 combinedWind, // Combined Wind vector in world space.
                float mask, // Wind mask. (Range: 0-1)
                float distanceFade) // Wind distance fade. (Range: 0-1)
            {
                #if defined(_TYPE_GRASS)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade,
                        positionWS,
                        float3( positionWS.x, objectPivot.y, positionWS.z ) );
                #elif defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade * 4,
                        positionWS,
                        objectPivot);
                #else
                    return FixStretching(
                        positionWS + combinedWind * mask * mask * distanceFade,
                        positionWS,
                        objectPivot);
                #endif
            }
            
            void Wind( WindInput input, inout float3 positionWS, inout float3 normalWS )
            {
                // Adjust the pivot for grass to use the XZ position of the vertex.
                // This is a decent workaround to get a per-grass-blade pivot until
                // we have proper pivot support.
                #ifdef _TYPE_GRASS
                    input.objectPivot = float3(positionWS.x, input.objectPivot.y, positionWS.z);
                #endif
                
                // Compute wind.
                float3 wind = ComputeWind( input, positionWS );
                
                // Apply wind to vertex.
                float3 outputWS =
                    ApplyWind(
                        positionWS,
                        input.objectPivot,
                        wind,
                        input.mask,
                        input.fade );
                
                // Recalculate normals for grass
                #if defined(_TYPE_GRASS)
                    float3 delta = outputWS - positionWS;
                    normalWS =
                        lerp(
                            normalWS,
                            normalWS + SafeNormalize( delta + float3(0, 0.1, 0) ),
                            length(delta) * _RecalculateWindNormals * input.fade );
                #endif
                
                positionWS = outputWS;
            }
            
            /// <summary>
            /// Returns the bend factor for the tree trunk.
            /// X contains the bend factor for the entire tree, Y for the base.
            /// </summary>
            float2 GetTrunkBendFactor()
            {
                return _TrunkBendFactor.xy;
            }
            
            float GetTrunkMask(
                float3 vertex, float2 uv1, float treeHeight, float bendFactor, float baseBendFactor )
            {
                #ifdef _WIND_BAKED
                    float trunkMask = saturate( uv1.x * bendFactor );
                #else
                    float trunkMask = pow2(saturate( vertex.y / treeHeight )) * bendFactor;
                #endif
                
                return saturate( trunkMask + saturate( vertex.y ) * baseBendFactor );
            }
            
            void Wind_Trunk(
                float3 vertex, // The vertex position in object space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 vertexWithWind, // The vertex position with wind displacement in world space.
                float2 uv1, // The second UV channel of the vertex. (UV1)
                float3 objectPivot, // The object pivot in world space.
                float3 windDirection, // The wind direction in world space. (normalized)
                out float3 vertexOut )
            {
                // Additional properties. Either global or baked.
                float2 bendFactor = GetTrunkBendFactor();
                float trunkMask = GetTrunkMask( vertex, uv1, GetObjectHeight(), bendFactor.x, bendFactor.y );
                float ambientStrength = GetWindStrength();
                
                // Calculate Ambient Wind
                float4 trunkAmbient =
                    AmbientFrequency(
                        objectPivot,
                        vertexWorldPosition,
                        windDirection,
                        0,
                        0.75 ) + ambientStrength;
                trunkAmbient *= trunkMask;
                
                // Calculate Gust
                float3 trunkGust =
                    SampleGust(
                        objectPivot, vertexWorldPosition, windDirection, 0, 0, 7);
                trunkGust *= trunkMask;
                
                // Apply
                float gustFrequency = trunkAmbient.w * length(trunkGust);
                float baseFrequency1 = trunkAmbient.x;
                float baseFrequency2 = trunkAmbient.x + trunkAmbient.y;
                float baseFrequency =
                    lerp( baseFrequency1, baseFrequency2, (_SinTime.x + 1) * 0.5 * ambientStrength);
                
                // TODO: Use the "FixStretching" approach?
                vertexOut =
                    RotateAroundAxis(
                        objectPivot,
                        vertexWithWind,
                        normalize( cross( float3(0,1,0) , windDirection ) ),
                        (baseFrequency * 0.75 + gustFrequency) * ambientStrength * 0.0375);
            }
            
            void AlphaTest( float alpha, float threshold )
            {
                
                clip( alpha - threshold );
            }
            
            void GetFade(
                float3 objectPivot, // The object pivot in world space.
                out float windFade,
                out float scaleFade )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    windFade = 1.0;
                    scaleFade = 1.0;
                #else
                    float distanceToCamera = distance( objectPivot, _WorldSpaceCameraPos );
                    windFade = 1.0 - saturate( (distanceToCamera - _WindFade.x) / _WindFade.y );
                    scaleFade = 1.0 - saturate( (distanceToCamera - _ScaleFade.x) / _ScaleFade.y );
                #endif
            }
            
            void PerVertexFade( float3 objectPivot, out float windFade, out float scaleFade )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    windFade = g_WindFade;
                    scaleFade = g_ScaleFade;
                #else
                    GetFade(
                        objectPivot,
                        windFade,
                        scaleFade );
                #endif
            }
            
            void Wind(
                inout VertexAttributes vertex,
                inout SurfaceInput surface,
                inout float3 positionWS,
                float windFade,
                float scaleFade,
                float heightMask,
                float phaseOffset,
                float3 objectPivot )
            {
                WindInput input;
                
                // Global
                input.direction = GetWindDirection();
                input.speed = GetWindSpeed();
                
                // Per-Object
                input.objectPivot = objectPivot;
                input.fade = windFade;
                
                // Per-Vertex
                input.phaseOffset = phaseOffset;
                input.normalWS = surface.normalWS;
                float windVariation = GetWindVariation( input.objectPivot );
                float vertexMask = GetVertexMask( vertex.color );
                input.mask = heightMask * vertexMask * windVariation;
                input.flutter = GetEdgeFlutter( vertex.color );
                
                float3 vertexOut = positionWS;
                float3 normalOut = surface.normalWS;
                Wind(
                    input,
                    vertexOut,
                    normalOut );
                
                #if defined(_TYPE_TREE_LEAVES) || defined( _TYPE_TREE_BARK )
                    Wind_Trunk(
                        vertex.positionOS.xyz,
                        positionWS.xyz,
                        vertexOut.xyz,
                        vertex.uv1.xy,
                        input.objectPivot,
                        input.direction,
                        vertexOut );
                #endif
                
                positionWS = vertexOut;
                surface.normalWS = normalOut;
            }
            
            float PerVertexPerlinNoise( float3 objectPivot )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    return g_WorldNoise;
                #else
                    float noise;
                    PerlinNoise_float( objectPivot.xz, _ColorVariationSpread, noise );
                    return noise;
                #endif
            }
            
            float3 ApplyScaleFade(
                float3 vertexWorldPosition, float3 objectPivot, float fade )
            {
                vertexWorldPosition.y = lerp(objectPivot.y, vertexWorldPosition.y, max(fade, 0.2));
                return vertexWorldPosition;
            }
            
            // Surface
            void SampleAlbedo( float2 uv0, out float4 albedo )
            {
                albedo = SAMPLE_TEXTURE2D( _Albedo, sampler_Albedo, uv0.xy );
            }
            
            void ApplyColorCorrection( inout float4 albedo, float noise )
            {
                #ifdef _COLOR_HSL
                    float4 albedo1 = albedo;
                    float4 albedo2 = albedo;
                    HSL_float( albedo1, _HSL, albedo1 );
                    HSL_float( albedo2, _HSLVariation, albedo2 );
                    albedo = lerp(albedo2, albedo1, noise);
                #else
                    albedo *= lerp(_TintVariation, _Tint, noise);
                #endif
            }
            
            #ifdef _NORMALMAP
                void SampleNormalMap( float2 uv0, out float3 normal )
                {
                    normal =
                        UnpackScaleNormal(
                            SAMPLE_TEXTURE2D( _NormalMap, sampler_Albedo, uv0.xy ), _NormalMapScale ).xyz;
                }
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                void SamplePackedMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 packed = SAMPLE_TEXTURE2D(_PackedMap, sampler_Albedo, uv0.xy);
                    metallic = packed.r;
                    smoothness = Remap(packed.a, _GlossRemap.xy);
                    occlusion = Remap(packed.g, _OcclusionRemap.xy);
                }
            #endif
            
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                void SampleMetallicGlossMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 metallicGloss = SAMPLE_TEXTURE2D( _MetallicGlossMap, sampler_Albedo, uv0.xy );
                    metallic = metallicGloss.r;
                    smoothness = Remap(metallicGloss.a, _GlossRemap.xy);
                    occlusion = Remap(SAMPLE_TEXTURE2D( _OcclusionMap, sampler_Albedo, uv0.xy ).g, _OcclusionRemap.xy);
                }
            #endif
            
            void SampleMetallicGlossConstants(
                float2 uv0, out float metallic, out float smoothness, out float occlusion)
            {
                metallic = _Metallic;
                smoothness = _Glossiness;
                occlusion = 1.0;
            }
            
            #ifdef _TRANSLUCENCY
                void SampleThickness( float2 uv0, out float thickness )
                {
                    thickness = SAMPLE_TEXTURE2D( _ThicknessMap, sampler_Albedo, uv0.xy ).r;
                    thickness = Remap( thickness, _ThicknessRemap.xy );
                }
            #endif
            
            #ifdef _EMISSION
                void SampleEmission( float2 uv0, out float3 emission )
                {
                    emission = SAMPLE_TEXTURE2D( _EmissionMap, sampler_Albedo, uv0.xy ).rgb;
                    emission *= _EmissionColor.rgb;
                    emission *= _EmissionIntensity;
                }
            #endif
            
            void FlipNormals( inout float3 normals, FRONT_FACE_TYPE cullFace )
            {
                normals.z *=
                    _DoubleSidedNormalMode > 0
                    ? IS_FRONT_VFACE(cullFace, 1, -1)
                    : 1.0;
            }
            
            // Called with raw vertex data before doing any kind of calculations or transformations.
            // Useful to modify the vertex data in object space.
            void PreVertexMethod( inout VertexAttributes vertex )
            {
            }
            
            // The main vertex method. Is used to modify the vertex data and
            // the input for the surface (fragment) method.
            void VertexMethod(
                VertexAttributes vertex,
                inout SurfaceInput surface )
            {
                float3 objectPivot = GetObjectPivot();
                float3 positionWS = GetAbsolutePositionWS( surface.positionWS.xyz );
                float3 positionWSOriginal = positionWS;
                
                surface.noise = PerVertexPerlinNoise( objectPivot );
                
                float windFade;
                float scaleFade;
                PerVertexFade( objectPivot, windFade, scaleFade );
                
                float heightMask =
                    GetHeightMask(
                        vertex.positionOS.xyz,
                        vertex.color,
                        vertex.uv1.xy );
                
                float phaseOffset =
                    GetPhaseOffset(
                        vertex.color,
                        positionWS,
                        objectPivot );
                
                #ifdef _WIND
                    Wind(
                        vertex,
                        surface,
                        positionWS,
                        windFade,
                        scaleFade,
                        heightMask,
                        phaseOffset,
                        objectPivot );
                #endif
                
                positionWS =
                    ApplyScaleFade( positionWS, objectPivot, scaleFade );
                
                surface.positionWS = GetCameraRelativePositionWS( positionWS );
                surface.normalWS = lerp(TransformObjectToWorldNormal(float3(0,1,0)), surface.normalWS, _VertexNormalStrength);
            }
            
            void SurfaceMethod(
                SurfaceInput input,
                inout Surface output )
            {
                // Albedo
                float4 albedo;
                SampleAlbedo( input.texCoord0.xy, albedo );
                
                // Overlay
                
                // Alpha clip
                #ifdef _ALPHATEST
                    
                    AlphaTest( albedo.a, _AlphaTestThreshold );
                    
                #else
                    albedo.a = 1;
                #endif
                
                // Color correction
                ApplyColorCorrection( albedo, input.noise );
                output.Albedo = albedo.rgb;
                output.Alpha = albedo.a;
                
                // Normal Map
                #ifdef _NORMALMAP
                    SampleNormalMap( input.texCoord0.xy, output.Normal );
                #endif
                
                // Surface Map
                #if defined(_SURFACE_MAP_PACKED)
                    SamplePackedMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #elif defined(_SURFACE_MAP_METALLIC_GLOSS)
                    SampleMetallicGlossMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #else
                    SampleMetallicGlossConstants(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #endif
                
                // Translucency
                #ifdef _TRANSLUCENCY
                    SampleThickness( input.texCoord0.xy, output.Thickness );
                #endif
                
                // Emission
                #ifdef _EMISSION
                    SampleEmission( input.texCoord0.xy, output.Emission );
                #endif
                
                // Flip double-sided normals
                #ifdef SHADER_STAGE_FRAGMENT
                    FlipNormals( output.Normal, input.cullFace );
                #endif
            }
            
            float3 Overlay(float3 a, float3 b)
            {
                return a < 0.5
                ? 2 * a * b
                : 1 - 2 * (1-a) * (1-b);
            }
            
            void PostLightingMethod(
                SurfaceInput input,
                Surface surface,
                SurfaceCommon surfaceCommon,
                LightingCommon lightingCommon,
                inout half4 color )
            {
            }
            
            // Embed the default shader template for the current render pipeline. These templates
            // contains the actual shader code for the render-pipeline-specific lighting.
            
            // Vertex
            #if defined(SHADERPASS_SHADOWCASTER)
                float3 _LightDirection;
            #endif
            
            float4 UnityObjectToClipPos( float3 positionOS, float3 positionWS, float3 normalWS )
            {
                #if defined(SHADERPASS_SHADOWCASTER)
                    // Define shadow pass specific clip position for Universal
                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #endif
                #elif defined(SHADERPASS_META)
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #else
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #endif
                
                return positionCS;
            }
            
            SurfaceInput vert( VertexAttributes input )
            {
                SurfaceInput output = (SurfaceInput)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                PreVertexMethod( input );
                
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                output.positionWS = positionWS;
                output.normalWS = normalWS;			// normalized in TransformObjectToWorldNormal()
                output.tangentWS = tangentWS;		// normalized in TransformObjectToWorldDir()
                output.texCoord0 = input.uv0;
                output.texCoord1 = input.uv1;
                output.texCoord2 = input.uv2;
                //output.texCoord3 = input.uv3;
                output.color = input.color;
                output.viewDirectionWS.xyz = normalize( _WorldSpaceCameraPos.xyz - positionWS );
                
                VertexMethod( input, output );
                
                positionWS = output.positionWS;
                normalWS = output.normalWS;			// normalized in TransformObjectToWorldNormal()
                tangentWS = output.tangentWS;		// normalized in TransformObjectToWorldDir()
                
                // Recalculate vertex based on modifications to output
                // TODO: Only do this if the position changed
                input.positionOS = TransformWorldToObject( output.positionWS );
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                
                input.uv0 = output.texCoord0;
                input.uv1 = output.texCoord1;
                input.uv2 = output.texCoord2;
                
                #if defined(SHADERPASS_FORWARD)
                    OUTPUT_LIGHTMAP_UV(input.uv1, unity_LightmapST, output.ambientOrLightmapUV);
                    OUTPUT_SH(normalWS, output.ambientOrLightmapUV);
                #endif
                
                #ifdef VARYINGS_NEED_SCREENPOSITION
                    output.screenPosition = ComputeScreenPos(output.positionCS, _ProjectionParams.x);
                #endif
                
                return output;
            }
            
            // Fragment
            
            half4 frag(SurfaceInput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Normalize the ViewDirection per-pixel so that we have an accurate value.
                input.viewDirectionWS.xyz = normalize(input.viewDirectionWS.xyz);
                
                #ifdef LOD_FADE_CROSSFADE
                    // TODO: Dithering is not stable for shadows. Not a big issue since it is usually not noticeable, or the fade is further away than the shadow rendering distance.
                    #if !defined(SHADER_API_GLES)
                        LODDitheringTransition(
                            ComputeFadeMaskSeed(
                                GetWorldSpaceNormalizeViewDir(input.positionWS), // we need a very accurate view direction to get good dithering. The regular viewDirectionWS that we get as input is not accurate enough because it is calculated per-vertex and then interpolated. That is why we calculate the view direction again here.
                                input.positionCS.xy),
                            unity_LODFade.x);
                    #endif
                #endif
                
                Surface surface = (Surface)0;
                surface.Albedo = 1;
                surface.Emission = 0;
                surface.Metallic = 0;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                surface.Alpha = 1;
                surface.Normal = float3(0,0,1);
                SurfaceMethod( input, surface );
                
                return 0;
            }
            
            ENDHLSL
        }
        
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }
            
            Blend One Zero, One Zero
            ZWrite On
            
            Cull [_DoubleSidedMode]
            
            ZTest LEqual
            
            // TODO: Make sure this works on all platforms.
            
            // Embed the default pass setup.
            // This will overwrite any values that need to be different for specifc passes.
            
            HLSLPROGRAM
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Nature Shaders Settings
            #define NATURE_SHADERS
            
            #define _TYPE_GRASS
            
            // Nature Renderer calculates values that are the same for all vertices once
            // for each object. This is a nice optimization that reduces per-vertex calculations.
            // This only works if Procedural Instancing is enabled.
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                #define PER_OBJECT_VALUES_CALCULATED
            #endif
            
            // Shader Features
            #ifdef _ALPHATEST
                #define _ALPHA_CLIP_ON
                #define _ALPHATEST_ON // HDRP
            #else
                #define _ALPHA_CLIP_OFF
                #define _ALPHATEST_OFF // HDRP
                #define _ALPHA_CLIP_DISABLED
            #endif
            
            #pragma shader_feature_local _COLOR_TINT _COLOR_HSL
            
            #pragma shader_feature_local _WIND_OFF _WIND_AUTO _WIND_BAKED
            #if defined(_WIND_AUTO) || defined(_WIND_BAKED)
                #define _WIND
                #define _WIND_ON
            #endif
            
            // Already defined in Lit template.
            // #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            
            // Already defined in Lit template.
            //#pragma shader_feature_local _EMISSION
            
            #ifndef _INTERACTION_OFF
                #define _INTERACTION_OFF
            #endif
            
            #pragma shader_feature_local _TRANSLUCENCY
            #ifdef _TRANSLUCENCY
                #define _TRANSLUCENCY_ON
                #define _MATERIAL_FEATURE_SUBSURFACE_SCATTERING 1
                #define _MATERIAL_FEATURE_TRANSMISSION 1
                
            #endif
            
            // Include the default cginc files and configurations
            // that are required for the current render pipeline.
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Default global keywords for material quality.
            // #pragma multi_compile MATERIAL_QUALITY_HIGH MATERIAL_QUALITY_MEDIUM MATERIAL_QUALITY_LOW
            #pragma shader_feature_local _LIGHTING_QUALITY_HIGH _LIGHTING_QUALITY_LOW
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            
            #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            #pragma shader_feature_local _ALPHATEST
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _EMISSION
            
            #pragma multi_compile_instancing
            
            #ifdef _SPECULARHIGHLIGHTS_OFF
                #define _ENVIRONMENTREFLECTIONS_OFF
                #define _GLOSSYREFLECTIONS_OFF
            #endif
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            //#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            
            // Variants
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            
            // Attributes and Varyings
            // These are all the possible settings for the URP shaders.
            //#define _SURFACE_TYPE_TRANSPARENT 1
            //#define _AlphaClip 1
            //#define _NORMALMAP 1
            //#define _SPECULAR_SETUP
            //#define _BLENDMODE_ADD 1
            //#define _ALPHAPREMULTIPLY_ON 1
            #define _NORMAL_DROPOFF_TS 1
            //#define _NORMAL_DROPOFF_OS 1
            //#define _NORMAL_DROPOFF_WS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            //#define ATTRIBUTES_NEED_TEXCOORD0
            //#define ATTRIBUTES_NEED_TEXCOORD1
            //#define ATTRIBUTES_NEED_TEXCOORD2
            //#define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            //#define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            //#define VARYINGS_NEED_VIEWDIRECTION_WS
            //#define VARYINGS_NEED_BITANGENT_WS
            //#define VARYINGS_NEED_SCREENPOSITION
            //#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_CULLFACE
            
            #ifdef _MAIN_LIGHT_SHADOWS
                #define VARYINGS_NEED_SHADOWCOORDS
            #endif
            
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define SHADERPASS_META
            
            // Common API to unify the different render pipelines.
            
            // Return absolute world position of current object
            float3 GetObjectAbsolutePositionWS()
            {
                float4x4 modelMatrix = UNITY_MATRIX_M;
                return GetAbsolutePositionWS(modelMatrix._m03_m13_m23); // Translation object to world
            }
            
            float3 GetPrimaryCameraPosition()
            {
                #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
                    return float3(0, 0, 0);
                #else
                    return _WorldSpaceCameraPos;
                #endif
            }
            
            // Unity 2020.2 already includes these methods.
            
            uint2 ComputeFadeMaskSeed(float3 V, uint2 positionSS)
            {
                uint2 fadeMaskSeed;
                if (IsPerspectiveProjection())
                {
                    float2 pv = PackNormalOctQuadEncode(V);
                    pv *= _ScreenParams.xy;
                    pv *= UNITY_MATRIX_P._m00_m11;
                    fadeMaskSeed = asuint((int2)pv);
                }
                else
                {
                    fadeMaskSeed = positionSS;
                }
                
                return fadeMaskSeed;
            }
            
            half3 UnpackScaleNormalRGorAG(half4 packednormal, half bumpScale)
            {
                #if defined(UNITY_NO_DXT5nm)
                    half3 normal = packednormal.xyz * 2 - 1;
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    return normal;
                #else
                    // This do the trick
                    packednormal.x *= packednormal.w;
                    
                    half3 normal;
                    normal.xy = (packednormal.xy * 2 - 1);
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
                    return normal;
                #endif
            }
            
            half3 UnpackScaleNormal(half4 packednormal, half bumpScale)
            {
                return UnpackScaleNormalRGorAG(packednormal, bumpScale);
            }
            
            struct IndirectLightingCommon
            {
                half3 diffuse;
                half3 specular;
            };
            
            struct SurfaceCommon
            {
                half3 diffuse;
                half3 specular;
                half3 normalWS;
                half oneMinusReflectivity;
                half smoothness;
                half alpha;
            };
            
            struct LightingCommon
            {
                Light light;
                IndirectLightingCommon indirect;
            };
            
            struct Surface
            {
                float3 Albedo; // base (diffuse or specular) color
                float3 Normal; // tangent space normal, if written
                half3 Emission;
                half Metallic; // 0=non-metal, 1=metal
                half Smoothness; // 0=rough, 1=smooth
                half Occlusion; // occlusion (default 1)
                float Alpha; // alpha for transparencies
                #if defined(_TRANSLUCENCY)
                    float Thickness; // 0 for full translucency, 1 for no translucency.
                #endif
            };
            
            #pragma target 4.0
            
            // Nature Renderer integration
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:SetupNatureRenderer nolightmap forwardadd renderinglayer
            
            // Input
            struct VertexAttributes
            {
                
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1; // optional
                float4 uv2 : TEXCOORD2; // optional
                float4 uv3 : TEXCOORD3; // optional
                
                //
                //uint instanceID : INSTANCEID_SEMANTIC;
                DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct SurfaceInput
            {
                float noise : TEXCOORD11; // TODO: pack noise into positionWS.w or normalWS.w
                
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 color : COLOR;
                float4 texCoord0 : TEXCOORD3;
                float4 texCoord1 : TEXCOORD4;
                float4 texCoord2 : TEXCOORD5;
                float4 viewDirectionWS : TEXCOORD6; //.xyz:view direction .w:fogCoord (standard)
                
                // SH or Lightmap UV
                half4 ambientOrLightmapUV : TEXCOORD7;
                
                // Lighting and shadow coordinates.
                // These are different depending on the render pipeline, so they are wrapped in
                // render pipeline specific tags.
                
                // Meta for editor visualization
                
                // Unity's default instancing settings.
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                
                // VFACE always needs to be the last semantic in the list, otherwise the compiler will throw
                // an error. So we define it here, and let the user add SHADERX_VFACE to the end of
                // the SurfaceInput struct.
                #if defined(SHADER_STAGE_FRAGMENT) //&& defined(VARYINGS_NEED_CULLFACE)
                    #define SHADERX_VFACE FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #else
                    #define SHADERX_VFACE
                #endif
                
                SHADERX_VFACE
            };
            
            // Properties
            CBUFFER_START( UnityPerMaterial )
            //
            float _AlphaTest;
            float _AlphaTestThreshold;
            
            // Fade
            float2 _ScaleFade;
            
            // Color Correction
            float3 _HSL;
            float3 _HSLVariation;
            float4 _Tint;
            float4 _TintVariation;
            float _ColorVariationSpread;
            
            // Surface Settings
            float _DoubleSidedMode;
            float _DoubleSidedNormalMode;
            
            float _VertexNormalStrength;
            float _SurfaceMapMethod;
            
            // Base Maps
            float _NormalMapScale;
            float _Metallic;
            float _Glossiness;
            
            // Surface Maps
            float2 _GlossRemap;
            float2 _OcclusionRemap;
            
            float4 _EmissionColor;
            float _EmissionIntensity;
            
            // Wind
            float _ObjectHeight;
            float _ObjectRadius;
            
            float _Wind;
            float _WindVariation;
            float _WindStrength;
            float _TurbulenceStrength;
            float _RecalculateWindNormals;
            float4 _TrunkBendFactor;
            
            float2 _WindFade;
            
            // Interaction
            
            // Translucency
            float _Translucency;
            
            float _TranslucencyBlendMode;
            float _TranslucencyStrength;
            float _TranslucencyDistortion;
            float _TranslucencyScattering;
            float4 _TranslucencyColor;
            float _TranslucencyAmbient;
            
            float2 _ThicknessRemap;
            
            // Overlay
            
            // Rendering
            float _LightingQuality;
            float _SpecularHighlights;
            float _EnvironmentReflections;
            
            CBUFFER_END
            
            SAMPLER( sampler_Albedo );
            TEXTURE2D( _Albedo );
            #ifdef _NORMALMAP
                TEXTURE2D( _NormalMap );
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                TEXTURE2D( _PackedMap );
            #endif
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                TEXTURE2D( _MetallicGlossMap );
                TEXTURE2D( _OcclusionMap );
            #endif
            
            #ifdef _EMISSION
                TEXTURE2D( _EmissionMap );
            #endif
            
            #ifdef _TRANSLUCENCY
                TEXTURE2D( _ThicknessMap );
            #endif
            
            // Include common features.
            // Properties
            #define GRASS_DEFAULT_HEIGHT 0.5
            #define PLANT_DEFAULT_HEIGHT 1.0
            #define TRUNK_DEFAULT_HEIGHT 20.0
            #define TRUNK_BASE_BEND_FACTOR 0.3
            #define TRUNK_BEND_MULTIPLIER 2.0
            
            float4 g_SmoothTime;
            float3 g_WindDirection;
            float2 g_Wind;
            float2 g_Turbulence;
            sampler2D g_GustNoise;
            
            // Properties that are calculated per-object by Nature Renderer
            #ifdef PER_OBJECT_VALUES_CALCULATED
                float g_WindFade;
                float g_ScaleFade;
                float g_WorldNoise;
                float3 g_ObjectPivot;
                float3 g_ConstantWindOffset;
                float g_PivotOffset;
                float3 g_WorldNormal;
            #endif
            
            float pow2( float x )
            {
                return x*x;
            }
            
            /// <summary>
            /// Returns the height of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectHeight()
            {
                return _ObjectHeight;
            }
            
            /// <summary>
            /// Returns the pivot of the object in world space.
            /// </summary>
            float3 GetObjectPivot()
            {
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    return g_ObjectPivot;
                #else
                    return GetAbsolutePositionWS( float3(UNITY_MATRIX_M[0].w, UNITY_MATRIX_M[1].w, UNITY_MATRIX_M[2].w) );
                #endif
            }
            
            #define GRASS_DEFAULT_RADIUS 1.0
            #define PLANT_DEFAULT_RADIUS 1.0
            #define TREE_DEFAULT_RADIUS 6.0
            
            /// <summary>
            /// Returns the radius of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectRadius()
            {
                return _ObjectRadius;
            }
            
            /// <summary>
            /// Returns the vertex normal in world space when vertex normals are anbled.
            /// Otherwise, returns the object's forward (Z+) direction.
            /// </summary>
            float3 GetWorldNormal(
                float3 normalWS, // The vertex normal in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                // New behavior, nice and simple.
                return normalWS;
                
                // Old behavior.
                /*
                #if defined(PER_OBJECT_VALUES_CALCULATED) && !defined(_TYPE_TREE_LEAVES)
                    return g_WorldNormal;
                #else
                    #ifdef _TYPE_TREE_LEAVES
                        // Scramble the vertex normals in case they are projected onto spheres
                        // or other geometry for smooth lighting. Otherwise the wind turbulence will end
                        // up as weird expanding and shrinking spheres.
                        // Define DO_NOT_SCRAMBLE_VERTEX_NORMALS in the shader if the tree models have
                        // accurate normals.
                        #ifndef DO_NOT_SCRAMBLE_VERTEX_NORMALS
                            return normalWS.xzy;
                        #else
                            return normalWS.xyz;
                        #endif
                    #else
                        return TransformObjectToWorldDir( float3(0, 0, 1) );
                    #endif
                #endif
                */
            }
            
            /// <summary>
            /// Returns the mask for the vertex.
            /// Uses the red channel of the vertex color.
            /// </summary>
            float GetVertexMask( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return 1.0;
                #else
                    #ifdef _WIND_BAKED
                        return vertexColor.r;
                    #else
                        return 1.0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Calculates the phase offset for the branch, based on the baked data.
            /// If no baked data is available, it will calculate an approximation of the branch.
            /// Should only be called for trees.
            /// </summary>
            float GetBranchPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_WIND_BAKED)
                    return vertexColor.r;
                #else
                    #if defined(_TYPE_TREE_BARK)
                        return 0;
                    #else
                        float3 offset = vertexWorldPosition - objectPivot;
                        float randomOffset = ( offset.x + offset.y + offset.z ) * 0.005;
                        return randomOffset;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the phase offset for the vertex.
            /// </summary>
            float GetPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return GetBranchPhaseOffset( vertexColor, vertexWorldPosition, objectPivot );
                #else
                    #ifdef _WIND_BAKED
                        return 1.0 - vertexColor.g;
                    #else
                        return 0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the edge flutter for the vertex,
            /// based either the vertex colors or UV (depending on the Wind Control settings).
            /// </summary>
            float GetEdgeFlutter( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_BARK)
                    return 0;
                #else
                    #if defined(_WIND_BAKED) && defined(_TYPE_TREE_LEAVES)
                        return vertexColor.g;
                    #else
                        return 1;
                    #endif
                #endif
            }
            
            float MaskFromHeightAndRadius( float3 vertex, float height, float radius )
            {
                return pow2( saturate( max(vertex.y / height, length(vertex.xz) / radius) ));
            }
            
            /// <summary>
            /// Returns a mask based on the relative height of the vertex.
            /// </summary>
            float GetHeightMask(
                float3 vertex, // The vertex position in object space.
                float4 vertexColor, // The vertex color.
                float2 uv1 ) // The second UV channel.
            {
                #if defined(_WIND_BAKED)
                    #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                        return uv1.y;
                    #else
                        return vertexColor.a;
                    #endif
                #else
                    #if defined(_TYPE_GRASS)
                        return saturate( vertex.y / GetObjectHeight() );
                    #else
                        return MaskFromHeightAndRadius( vertex, GetObjectHeight(), GetObjectRadius() );
                    #endif
                #endif
            }
            
            float Remap( float value, float2 remap )
            {
                return remap.x + value * (remap.y - remap.x);
            }
            
            float4 SmoothCurve( float4 x )
            {
                return x * x *( 3.0 - 2.0 * x );
            }
            float4 TriangleWave( float4 x )
            {
                return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
            }
            float4 SmoothTriangleWave( float4 x )
            {
                return SmoothCurve( TriangleWave( x ) );
            }
            
            float4 FastSin( float4 x )
            {
                #ifndef PI
                    #define PI 3.14159265
                #endif
                #define DIVIDE_BY_PI 1.0 / (2.0 * PI)
                return (SmoothTriangleWave( x * DIVIDE_BY_PI ) - 0.5) * 2;
            }
            
            float3 FixStretching( float3 vertex, float3 original, float3 center )
            {
                return center + SafeNormalize(vertex - center) * length(original - center);
            }
            
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
            
            float3 RotateAroundAxisFast( float3 center, float3 original, float3 direction )
            {
                return original + direction;
            }
            
            uniform sampler2D g_PerlinNoise;
            uniform float g_PerlinNoiseScale;
            
            void PerlinNoise( float2 uv, float scale, out float noise )
            {
                noise =
                    tex2Dlod(
                        g_PerlinNoise,
                        float4(uv.xy, 0, 0) * scale * g_PerlinNoiseScale).r;
            }
            
            void PerlinNoise_float( float2 uv, float scale, out float noise )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    noise = g_WorldNoise;
                #else
                    PerlinNoise( uv, scale, noise );
                #endif
            }
            
            //
            #ifndef NODE_NATURE_RENDERER_INCLUDED
                #define NODE_NATURE_RENDERER_INCLUDED
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    
                    #define Use_Macro_UNITY_MATRIX_M_instead_of_unity_ObjectToWorld unity_ObjectToWorld
                    #define Use_Macro_UNITY_MATRIX_I_M_instead_of_unity_WorldToObject unity_WorldToObject
                    
                    struct CompressedFloat4x4
                    {
                        uint positionXY;
                        uint positionZ_scale;
                        uint rotationXY;
                        uint rotationZW;
                    };
                    
                    uniform float3 _CompressionRange;
                    uniform float3 _CompressionBase;
                    
                    uint CompressToUshort( float value, float precision )
                    {
                        return (uint)(value / precision * 65535.0);
                    }
                    
                    uint CompressToByte( float value, float precision )
                    {
                        return (uint)(value / precision * 255.0);
                    }
                    
                    float DecompressFromByte( uint value, float precision )
                    {
                        return value / 255.0 * precision;
                    }
                    
                    float DecompressFromUshort( uint value, float precision )
                    {
                        return value / 65535.0 * precision;
                    }
                    
                    void _UnpackInt( uint packedInt, out uint a, out uint b )
                    {
                        a = ( (uint) (packedInt >> 16) );
                        b = ( (uint) ((packedInt << 16) >> 16) );
                    }
                    
                    void _UnpackShort( uint packedShort, out uint a, out uint b )
                    {
                        a = ( (uint) (packedShort >> 8) );
                        b = ( (uint) ((packedShort << 24) >> 24) );
                    }
                    
                    uint _PackInt( uint ushortA, uint ushortB )
                    {
                        return ushortA << 16 | ushortB;
                    }
                    
                    uint _PackShort( uint byteA, uint byteB )
                    {
                        return (byteA << 8) | byteB;
                    }
                    
                    float4x4 QuaternionToMatrix(float4 quaternion)
                    {
                        float4x4 result = (float4x4)0;
                        float x = quaternion.x;
                        float y = quaternion.y;
                        float z = quaternion.z;
                        float w = quaternion.w;
                        
                        float x2 = x + x;
                        float y2 = y + y;
                        float z2 = z + z;
                        float xx = x * x2;
                        float xy = x * y2;
                        float xz = x * z2;
                        float yy = y * y2;
                        float yz = y * z2;
                        float zz = z * z2;
                        float wx = w * x2;
                        float wy = w * y2;
                        float wz = w * z2;
                        
                        result[0][0] = 1.0 - (yy + zz);
                        result[0][1] = xy - wz;
                        result[0][2] = xz + wy;
                        
                        result[1][0] = xy + wz;
                        result[1][1] = 1.0 - (xx + zz);
                        result[1][2] = yz - wx;
                        
                        result[2][0] = xz - wy;
                        result[2][1] = yz + wx;
                        result[2][2] = 1.0 - (xx + yy);
                        
                        result[3][3] = 1.0;
                        
                        return result;
                    }
                    
                    void DecompressInstanceMatrix( inout float4x4 m, CompressedFloat4x4 compressedMatrix )
                    {
                        uint positionX, positionY, positionZ;
                        uint scaleXYZ;
                        uint rotationX, rotationY, rotationZ, rotationW;
                        
                        _UnpackInt( compressedMatrix.positionXY, positionX, positionY );
                        _UnpackInt( compressedMatrix.positionZ_scale, positionZ, scaleXYZ );
                        _UnpackInt( compressedMatrix.rotationXY, rotationX, rotationY );
                        _UnpackInt( compressedMatrix.rotationZW, rotationZ, rotationW );
                        
                        uint scaleX, scaleY;
                        _UnpackShort( scaleXYZ, scaleX, scaleY );
                        
                        float3 position =
                            float3(
                                DecompressFromUshort(positionX, _CompressionRange.x) + _CompressionBase.x,
                                DecompressFromUshort(positionY, _CompressionRange.y) + _CompressionBase.y,
                                DecompressFromUshort(positionZ, _CompressionRange.z) + _CompressionBase.z );
                        
                        float3 scale =
                            float3(
                                DecompressFromByte(scaleX, 16.0),
                                DecompressFromByte(scaleY, 16.0),
                                DecompressFromByte(scaleX, 16.0) );
                        
                        float4 rotation =
                            float4(
                                DecompressFromUshort(rotationX, 2.0) - 1.0,
                                DecompressFromUshort(rotationY, 2.0) - 1.0,
                                DecompressFromUshort(rotationZ, 2.0) - 1.0,
                                DecompressFromUshort(rotationW, 2.0) - 1.0 );
                        
                        m = QuaternionToMatrix( rotation );
                        
                        m[0][0] *= scale.x; m[1][0] *= scale.y; m[2][0] *= scale.z;
                        m[0][1] *= scale.x; m[1][1] *= scale.y; m[2][1] *= scale.z;
                        m[0][2] *= scale.x; m[1][2] *= scale.y; m[2][2] *= scale.z;
                        m[0][3] *= scale.x; m[1][3] *= scale.y; m[2][3] *= scale.z;
                        
                        m[0][3] = position.x;
                        m[1][3] = position.y;
                        m[2][3] = position.z;
                    }
                    
                    #if defined(SHADER_API_GLCORE) \
                        || defined(SHADER_API_D3D11) \
                        || defined(SHADER_API_GLES3) \
                        || defined(SHADER_API_METAL) \
                        || defined(SHADER_API_VULKAN) \
                        || defined(SHADER_API_PSSL) \
                        || defined(SHADER_API_XBOXONE)
                        uniform StructuredBuffer<CompressedFloat4x4> _NatureRendererBuffer;
                    #endif
                    
                    float4x4 inverse(float4x4 input)
                    {
                        #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
                        
                        float4x4 cofactors = float4x4(
                            minor(_22_23_24, _32_33_34, _42_43_44),
                            -minor(_21_23_24, _31_33_34, _41_43_44),
                            minor(_21_22_24, _31_32_34, _41_42_44),
                            -minor(_21_22_23, _31_32_33, _41_42_43),
                            
                            -minor(_12_13_14, _32_33_34, _42_43_44),
                            minor(_11_13_14, _31_33_34, _41_43_44),
                            -minor(_11_12_14, _31_32_34, _41_42_44),
                            minor(_11_12_13, _31_32_33, _41_42_43),
                            
                            minor(_12_13_14, _22_23_24, _42_43_44),
                            -minor(_11_13_14, _21_23_24, _41_43_44),
                            minor(_11_12_14, _21_22_24, _41_42_44),
                            -minor(_11_12_13, _21_22_23, _41_42_43),
                            
                            -minor(_12_13_14, _22_23_24, _32_33_34),
                            minor(_11_13_14, _21_23_24, _31_33_34),
                            -minor(_11_12_14, _21_22_24, _31_32_34),
                            minor(_11_12_13, _21_22_23, _31_32_33)
                            );
                        #undef minor
                        return transpose(cofactors) / determinant(input);
                    }
                #endif
                
                // Pre-calculate and cache data for Nature Shaders that relies on
                // per-object data instead of per-vertex or per-pixel.
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    void PreCalculateNatureShadersData()
                    {
                        g_ObjectPivot = GetAbsolutePositionWS( float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w) );
                        g_ConstantWindOffset = cross( GetWindDirection().xyz, float3(0,1,0) );
                        g_PivotOffset = length( float3(g_ObjectPivot.x, 0, g_ObjectPivot.z) );
                        g_WorldNormal = TransformObjectToWorldDir( float3(0, 0, 1) );
                        
                        GetFade( g_ObjectPivot, g_WindFade, g_ScaleFade );
                        PerlinNoise( g_ObjectPivot.xz, _ColorVariationSpread, g_WorldNoise);
                    }
                #endif
                
                void SetupNatureRenderer()
                {
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                        DecompressInstanceMatrix(unity_ObjectToWorld, _NatureRendererBuffer[unity_InstanceID]);
                        unity_WorldToObject = inverse(unity_ObjectToWorld);
                    #endif
                    
                    #if defined(PER_OBJECT_VALUES_CALCULATED)
                        PreCalculateNatureShadersData();
                    #endif
                }
                
                void NatureRenderer_float( float3 vertex, out float3 vertexOut )
                {
                    vertexOut = vertex;
                }
            #endif
            
            struct TranslucencyInput
            {
                float Scale;
                float NormalDistortion;
                float Scattering;
                float Thickness;
                float Ambient;
                half3 Color;
            };
            
            half3 Translucency(
                TranslucencyInput input,
                float3 bakedGI,
                float3 surfaceAlbedo,
                float3 surfaceNormal,
                float3 viewDirectionWS,
                Light light )
            {
                half3 lightDir = light.direction + surfaceNormal * input.NormalDistortion;
                half transVdotL =
                    pow( saturate( dot( viewDirectionWS, -lightDir ) ), input.Scattering ) * input.Scale;
                half3 translucency =
                    (transVdotL + bakedGI * input.Ambient)
                    * (1-input.Thickness)
                    * light.shadowAttenuation
                    * light.distanceAttenuation;
                
                return half3( surfaceAlbedo * light.color * translucency * input.Color );
            }
            
            float3 Linear_to_HSV(float3 In)
            {
                float3 sRGBLo = In * 12.92;
                float3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
                float3 Linear = float3(In <= 0.0031308) ? sRGBLo : sRGBHi;
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(Linear.bg, K.wz), float4(Linear.gb, K.xy), step(Linear.b, Linear.g));
                float4 Q = lerp(float4(P.xyw, Linear.r), float4(Linear.r, P.yzx), step(P.x, Linear.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                return float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            }
            
            float3 HSV_to_Linear(float3 In)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                float3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                float3 linearRGBLo = RGB / 12.92;
                float3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                return float3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
            }
            
            void HSL_float( float4 color, float3 hsl, out float4 colorOut )
            {
                float3 hsv = Linear_to_HSV( color.rgb );
                hsv.x += hsl.x;
                hsv.y = saturate(hsv.y + hsl.y * 0.5);
                hsv.z = saturate(hsv.z + hsl.z * 0.5);
                colorOut = float4( HSV_to_Linear(hsv), color.a );
            }
            
            struct WindInput
            {
                // Global
                float3 direction;
                float speed;
                
                // Per-Object
                float3 objectPivot;
                float fade;
                
                // Per-Vertex
                float phaseOffset;
                float3 normalWS;
                float mask;
                float flutter;
            };
            
            /// <summary>
            /// Returns the time for gusts.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothGustOffset()
            {
                return g_SmoothTime.y;
            }
            
            /// <summary>
            /// Returns the time for turbulence.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothTurbulenceOffset()
            {
                return g_SmoothTime.z;
            }
            
            /// <summary>
            /// Returns the global wind direction.
            /// </summary>
            float3 GetWindDirection()
            {
                return g_WindDirection != 0
                ? normalize(float3(g_WindDirection.x, 0, g_WindDirection.z))
                : float3(1, 0, 0);
            }
            
            /// <summary>
            /// Returns the global wind speed.
            /// </summary>
            float GetWindSpeed()
            {
                return g_Wind.x;
            }
            
            /// <summary>
            /// Returns the global wind strength.
            /// </summary>
            float GetWindStrength()
            {
                return g_Wind.y * _WindStrength;
            }
            
            /// <summary>
            /// Returns a random windVariation value based on the object pivot.
            /// </summary>
            float GetWindVariation(
                float3 objectPivot ) // The object pivot in world space.
            {
                return 1.0 - frac( objectPivot.x * objectPivot.z * 10.0 ) * _WindVariation;
            }
            
            /// <summary>
            /// Returns the global turbulence strength.
            /// </summary>
            float GetTurbulenceStrength()
            {
                return g_Turbulence.y * _TurbulenceStrength;
            }
            
            /// <summary>
            /// Returns the time for ambient wind.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothAmbientOffset()
            {
                return g_SmoothTime.x;
            }
            
            /// <summary>
            /// Calculates the wind frequency for the ambient wind.
            /// Returns a vector4 containing 4 different sine waves.
            /// </summary>
            float4 AmbientFrequency(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset, // The wind phase offset. (Range: 0-1)
                float speed ) // The wind speed.
            {
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float footprint = 3;
                float time = GetSmoothAmbientOffset() - phaseOffset * footprint;
                
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float pivotOffset = g_PivotOffset;
                #else
                    float pivotOffset = length( float3(objectPivot.x, 0, objectPivot.z) );
                #endif
                
                float scale = 0.5;
                float frequency = pivotOffset * scale - time;
                return FastSin(
                    float4(
                        frequency + constantOffset.x,
                        frequency*0.5 + constantOffset.z,
                        frequency*0.25,
                        frequency*0.125) * speed );
            }
            
            /// <summary>
            /// Calculates the ambient wind.
            /// </summary>
            float3 AmbientWind(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset ) // The wind phase offset. (Range: 0-1)
            {
                float4 sine = AmbientFrequency( objectPivot, vertexWorldPosition, windDirection, phaseOffset, 1 );
                sine.w = abs(sine.w) + 0.5;
                float xz = 1.5 * sine.x * sine.z + sine.w + 1;
                float y = 1 * sine.y * sine.z + sine.w;
                return windDirection * float3(xz, 0, xz) + float3(0, y, 0);
            }
            
            /// <summary>
            /// Calculates the turbulence.
            /// </summary>
            float3 Turbulence(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 worldNormal, // The direction of the turbulence in world space (usually vertex normal).
                float phaseOffset, // The wind phase offset.
                float edgeFlutter, // The amount of edge flutter for tree leaves. (Range: 0-1)
                float speed ) // The wind speed.
            {
                #if defined(_TYPE_TREE_BARK)
                    return float3(0, 0, 0);
                #else
                    float time = GetSmoothTurbulenceOffset() - phaseOffset;
                    float frequency = (objectPivot.x + objectPivot.y + objectPivot.z) * 2.5 - time;
                    
                    // TODO: Add a secondary frequency.
                    float4 sine =
                        FastSin(
                            float4(
                                (1.65 * frequency) * speed,
                                (2 * 1.65 * frequency) * speed,
                                0,
                                0) );
                    
                    float x = 1 * sine.x + 1;
                    float z = 1 * sine.y + 1;
                    float y = (x + z) * 0.5;
                    
                    #if defined(_TYPE_TREE_LEAVES)
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1) * edgeFlutter;
                    #else
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1);
                    #endif
                #endif
            }
            
            float3 SampleGust(
                float3 objectPivot,
                float3 vertexWorldPosition,
                float3 windDirection,
                float phaseOffset,
                float edgeFlutter,
                float lod )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                    lod = 5;
                #else
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                #endif
                
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float2 windOffsetOverTime = windDirection.xz * time + constantOffset.xz;
                #if defined(_TYPE_TREE_LEAVES)
                    float3 vertexOffset = vertexWorldPosition - objectPivot;
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime + vertexOffset.xz * 0.0075 * edgeFlutter;
                #else
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime;
                #endif
                float strength = tex2Dlod( g_GustNoise, float4(offset, 0, lod) ).r;
                return strength * windDirection;
            }
            
            float3 CombineWind(
                float3 ambient, // Large constant ambient wind motion.
                float3 gust, // Local gust based on noise texture.
                float3 turbulence, // Constant turbulence.
                float3 shiver, // High frequency shivering during gust.
                float4 strength // The wind strength for each wind component.
                )
            {
                ambient *= strength.x;
                gust *= strength.y;
                turbulence *= strength.z;
                shiver *= strength.w;
                
                // Trees require more displacement for the wind to be visible because the objects are larger.
                // These are magic numbers that give a nice balance between the grass/plants and trees,
                // based on a common tree size.
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    ambient *= 3;
                    gust *= 1;
                    turbulence *= 3;
                    shiver *= 3;
                #endif
                
                float gustLength = length( gust );
                float increaseTurbelenceWithGust = smoothstep(0, 1, gustLength) + 1;
                
                // Calculate the balance between different wind types.
                // If we do it here then we can keep the input parameters in a 0-1 range.
                ambient *= 0.1;
                gust *= 1.5;
                turbulence *= 0.15;
                shiver *= 0.15;
                
                #if defined(_DEBUG_AMBIENT)
                    return ambient;
                #elif defined(_DEBUG_GUST)
                    return gust;
                #elif defined(_DEBUG_TURBULENCE)
                    return lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #else
                    return
                    ambient
                    + gust
                    + lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #endif
            }
            
            float3 ComputeWind( WindInput input, float3 positionWS )
            {
                #if defined(_TYPE_GRASS) || defined(_TYPE_PLANT)
                    input.phaseOffset *= dot( input.direction, (positionWS - input.objectPivot) );
                    input.phaseOffset += input.mask * 0.5;
                #endif
                
                float3 ambient =
                    AmbientWind(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset );
                
                float3 gust =
                    SampleGust(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset,
                        input.flutter,
                        0 );
                
                // Add a bit of a random phase offset to the tree leaves. Phase Offset is calculated
                // per-branch and we don't want to have the same turbulence for the entire branch.
                #if defined(_TYPE_TREE_LEAVES)
                    input.phaseOffset +=
                        dot( input.direction, (positionWS - input.objectPivot) ) * input.flutter;
                #endif
                
                float3 turbulence1 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        1 );
                
                float3 turbulence2 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        2 );
                
                return CombineWind(
                    ambient,
                    gust,
                    turbulence1,
                    turbulence2,
                    float4(GetWindStrength().xx, GetTurbulenceStrength().xx) );
            }
            
            float3 ApplyWind(
                float3 positionWS, // Vertex position in world space.
                float3 objectPivot, // Object Pivot in world space.
                float3 combinedWind, // Combined Wind vector in world space.
                float mask, // Wind mask. (Range: 0-1)
                float distanceFade) // Wind distance fade. (Range: 0-1)
            {
                #if defined(_TYPE_GRASS)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade,
                        positionWS,
                        float3( positionWS.x, objectPivot.y, positionWS.z ) );
                #elif defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade * 4,
                        positionWS,
                        objectPivot);
                #else
                    return FixStretching(
                        positionWS + combinedWind * mask * mask * distanceFade,
                        positionWS,
                        objectPivot);
                #endif
            }
            
            void Wind( WindInput input, inout float3 positionWS, inout float3 normalWS )
            {
                // Adjust the pivot for grass to use the XZ position of the vertex.
                // This is a decent workaround to get a per-grass-blade pivot until
                // we have proper pivot support.
                #ifdef _TYPE_GRASS
                    input.objectPivot = float3(positionWS.x, input.objectPivot.y, positionWS.z);
                #endif
                
                // Compute wind.
                float3 wind = ComputeWind( input, positionWS );
                
                // Apply wind to vertex.
                float3 outputWS =
                    ApplyWind(
                        positionWS,
                        input.objectPivot,
                        wind,
                        input.mask,
                        input.fade );
                
                // Recalculate normals for grass
                #if defined(_TYPE_GRASS)
                    float3 delta = outputWS - positionWS;
                    normalWS =
                        lerp(
                            normalWS,
                            normalWS + SafeNormalize( delta + float3(0, 0.1, 0) ),
                            length(delta) * _RecalculateWindNormals * input.fade );
                #endif
                
                positionWS = outputWS;
            }
            
            /// <summary>
            /// Returns the bend factor for the tree trunk.
            /// X contains the bend factor for the entire tree, Y for the base.
            /// </summary>
            float2 GetTrunkBendFactor()
            {
                return _TrunkBendFactor.xy;
            }
            
            float GetTrunkMask(
                float3 vertex, float2 uv1, float treeHeight, float bendFactor, float baseBendFactor )
            {
                #ifdef _WIND_BAKED
                    float trunkMask = saturate( uv1.x * bendFactor );
                #else
                    float trunkMask = pow2(saturate( vertex.y / treeHeight )) * bendFactor;
                #endif
                
                return saturate( trunkMask + saturate( vertex.y ) * baseBendFactor );
            }
            
            void Wind_Trunk(
                float3 vertex, // The vertex position in object space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 vertexWithWind, // The vertex position with wind displacement in world space.
                float2 uv1, // The second UV channel of the vertex. (UV1)
                float3 objectPivot, // The object pivot in world space.
                float3 windDirection, // The wind direction in world space. (normalized)
                out float3 vertexOut )
            {
                // Additional properties. Either global or baked.
                float2 bendFactor = GetTrunkBendFactor();
                float trunkMask = GetTrunkMask( vertex, uv1, GetObjectHeight(), bendFactor.x, bendFactor.y );
                float ambientStrength = GetWindStrength();
                
                // Calculate Ambient Wind
                float4 trunkAmbient =
                    AmbientFrequency(
                        objectPivot,
                        vertexWorldPosition,
                        windDirection,
                        0,
                        0.75 ) + ambientStrength;
                trunkAmbient *= trunkMask;
                
                // Calculate Gust
                float3 trunkGust =
                    SampleGust(
                        objectPivot, vertexWorldPosition, windDirection, 0, 0, 7);
                trunkGust *= trunkMask;
                
                // Apply
                float gustFrequency = trunkAmbient.w * length(trunkGust);
                float baseFrequency1 = trunkAmbient.x;
                float baseFrequency2 = trunkAmbient.x + trunkAmbient.y;
                float baseFrequency =
                    lerp( baseFrequency1, baseFrequency2, (_SinTime.x + 1) * 0.5 * ambientStrength);
                
                // TODO: Use the "FixStretching" approach?
                vertexOut =
                    RotateAroundAxis(
                        objectPivot,
                        vertexWithWind,
                        normalize( cross( float3(0,1,0) , windDirection ) ),
                        (baseFrequency * 0.75 + gustFrequency) * ambientStrength * 0.0375);
            }
            
            void AlphaTest( float alpha, float threshold )
            {
                
                clip( alpha - threshold );
            }
            
            void GetFade(
                float3 objectPivot, // The object pivot in world space.
                out float windFade,
                out float scaleFade )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    windFade = 1.0;
                    scaleFade = 1.0;
                #else
                    float distanceToCamera = distance( objectPivot, _WorldSpaceCameraPos );
                    windFade = 1.0 - saturate( (distanceToCamera - _WindFade.x) / _WindFade.y );
                    scaleFade = 1.0 - saturate( (distanceToCamera - _ScaleFade.x) / _ScaleFade.y );
                #endif
            }
            
            void PerVertexFade( float3 objectPivot, out float windFade, out float scaleFade )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    windFade = g_WindFade;
                    scaleFade = g_ScaleFade;
                #else
                    GetFade(
                        objectPivot,
                        windFade,
                        scaleFade );
                #endif
            }
            
            void Wind(
                inout VertexAttributes vertex,
                inout SurfaceInput surface,
                inout float3 positionWS,
                float windFade,
                float scaleFade,
                float heightMask,
                float phaseOffset,
                float3 objectPivot )
            {
                WindInput input;
                
                // Global
                input.direction = GetWindDirection();
                input.speed = GetWindSpeed();
                
                // Per-Object
                input.objectPivot = objectPivot;
                input.fade = windFade;
                
                // Per-Vertex
                input.phaseOffset = phaseOffset;
                input.normalWS = surface.normalWS;
                float windVariation = GetWindVariation( input.objectPivot );
                float vertexMask = GetVertexMask( vertex.color );
                input.mask = heightMask * vertexMask * windVariation;
                input.flutter = GetEdgeFlutter( vertex.color );
                
                float3 vertexOut = positionWS;
                float3 normalOut = surface.normalWS;
                Wind(
                    input,
                    vertexOut,
                    normalOut );
                
                #if defined(_TYPE_TREE_LEAVES) || defined( _TYPE_TREE_BARK )
                    Wind_Trunk(
                        vertex.positionOS.xyz,
                        positionWS.xyz,
                        vertexOut.xyz,
                        vertex.uv1.xy,
                        input.objectPivot,
                        input.direction,
                        vertexOut );
                #endif
                
                positionWS = vertexOut;
                surface.normalWS = normalOut;
            }
            
            float PerVertexPerlinNoise( float3 objectPivot )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    return g_WorldNoise;
                #else
                    float noise;
                    PerlinNoise_float( objectPivot.xz, _ColorVariationSpread, noise );
                    return noise;
                #endif
            }
            
            float3 ApplyScaleFade(
                float3 vertexWorldPosition, float3 objectPivot, float fade )
            {
                vertexWorldPosition.y = lerp(objectPivot.y, vertexWorldPosition.y, max(fade, 0.2));
                return vertexWorldPosition;
            }
            
            // Surface
            void SampleAlbedo( float2 uv0, out float4 albedo )
            {
                albedo = SAMPLE_TEXTURE2D( _Albedo, sampler_Albedo, uv0.xy );
            }
            
            void ApplyColorCorrection( inout float4 albedo, float noise )
            {
                #ifdef _COLOR_HSL
                    float4 albedo1 = albedo;
                    float4 albedo2 = albedo;
                    HSL_float( albedo1, _HSL, albedo1 );
                    HSL_float( albedo2, _HSLVariation, albedo2 );
                    albedo = lerp(albedo2, albedo1, noise);
                #else
                    albedo *= lerp(_TintVariation, _Tint, noise);
                #endif
            }
            
            #ifdef _NORMALMAP
                void SampleNormalMap( float2 uv0, out float3 normal )
                {
                    normal =
                        UnpackScaleNormal(
                            SAMPLE_TEXTURE2D( _NormalMap, sampler_Albedo, uv0.xy ), _NormalMapScale ).xyz;
                }
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                void SamplePackedMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 packed = SAMPLE_TEXTURE2D(_PackedMap, sampler_Albedo, uv0.xy);
                    metallic = packed.r;
                    smoothness = Remap(packed.a, _GlossRemap.xy);
                    occlusion = Remap(packed.g, _OcclusionRemap.xy);
                }
            #endif
            
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                void SampleMetallicGlossMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 metallicGloss = SAMPLE_TEXTURE2D( _MetallicGlossMap, sampler_Albedo, uv0.xy );
                    metallic = metallicGloss.r;
                    smoothness = Remap(metallicGloss.a, _GlossRemap.xy);
                    occlusion = Remap(SAMPLE_TEXTURE2D( _OcclusionMap, sampler_Albedo, uv0.xy ).g, _OcclusionRemap.xy);
                }
            #endif
            
            void SampleMetallicGlossConstants(
                float2 uv0, out float metallic, out float smoothness, out float occlusion)
            {
                metallic = _Metallic;
                smoothness = _Glossiness;
                occlusion = 1.0;
            }
            
            #ifdef _TRANSLUCENCY
                void SampleThickness( float2 uv0, out float thickness )
                {
                    thickness = SAMPLE_TEXTURE2D( _ThicknessMap, sampler_Albedo, uv0.xy ).r;
                    thickness = Remap( thickness, _ThicknessRemap.xy );
                }
            #endif
            
            #ifdef _EMISSION
                void SampleEmission( float2 uv0, out float3 emission )
                {
                    emission = SAMPLE_TEXTURE2D( _EmissionMap, sampler_Albedo, uv0.xy ).rgb;
                    emission *= _EmissionColor.rgb;
                    emission *= _EmissionIntensity;
                }
            #endif
            
            void FlipNormals( inout float3 normals, FRONT_FACE_TYPE cullFace )
            {
                normals.z *=
                    _DoubleSidedNormalMode > 0
                    ? IS_FRONT_VFACE(cullFace, 1, -1)
                    : 1.0;
            }
            
            // Called with raw vertex data before doing any kind of calculations or transformations.
            // Useful to modify the vertex data in object space.
            void PreVertexMethod( inout VertexAttributes vertex )
            {
            }
            
            // The main vertex method. Is used to modify the vertex data and
            // the input for the surface (fragment) method.
            void VertexMethod(
                VertexAttributes vertex,
                inout SurfaceInput surface )
            {
                float3 objectPivot = GetObjectPivot();
                float3 positionWS = GetAbsolutePositionWS( surface.positionWS.xyz );
                float3 positionWSOriginal = positionWS;
                
                surface.noise = PerVertexPerlinNoise( objectPivot );
                
                float windFade;
                float scaleFade;
                PerVertexFade( objectPivot, windFade, scaleFade );
                
                float heightMask =
                    GetHeightMask(
                        vertex.positionOS.xyz,
                        vertex.color,
                        vertex.uv1.xy );
                
                float phaseOffset =
                    GetPhaseOffset(
                        vertex.color,
                        positionWS,
                        objectPivot );
                
                #ifdef _WIND
                    Wind(
                        vertex,
                        surface,
                        positionWS,
                        windFade,
                        scaleFade,
                        heightMask,
                        phaseOffset,
                        objectPivot );
                #endif
                
                positionWS =
                    ApplyScaleFade( positionWS, objectPivot, scaleFade );
                
                surface.positionWS = GetCameraRelativePositionWS( positionWS );
                surface.normalWS = lerp(TransformObjectToWorldNormal(float3(0,1,0)), surface.normalWS, _VertexNormalStrength);
            }
            
            void SurfaceMethod(
                SurfaceInput input,
                inout Surface output )
            {
                // Albedo
                float4 albedo;
                SampleAlbedo( input.texCoord0.xy, albedo );
                
                // Overlay
                
                // Alpha clip
                #ifdef _ALPHATEST
                    
                    AlphaTest( albedo.a, _AlphaTestThreshold );
                    
                #else
                    albedo.a = 1;
                #endif
                
                // Color correction
                ApplyColorCorrection( albedo, input.noise );
                output.Albedo = albedo.rgb;
                output.Alpha = albedo.a;
                
                // Normal Map
                #ifdef _NORMALMAP
                    SampleNormalMap( input.texCoord0.xy, output.Normal );
                #endif
                
                // Surface Map
                #if defined(_SURFACE_MAP_PACKED)
                    SamplePackedMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #elif defined(_SURFACE_MAP_METALLIC_GLOSS)
                    SampleMetallicGlossMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #else
                    SampleMetallicGlossConstants(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #endif
                
                // Translucency
                #ifdef _TRANSLUCENCY
                    SampleThickness( input.texCoord0.xy, output.Thickness );
                #endif
                
                // Emission
                #ifdef _EMISSION
                    SampleEmission( input.texCoord0.xy, output.Emission );
                #endif
                
                // Flip double-sided normals
                #ifdef SHADER_STAGE_FRAGMENT
                    FlipNormals( output.Normal, input.cullFace );
                #endif
            }
            
            float3 Overlay(float3 a, float3 b)
            {
                return a < 0.5
                ? 2 * a * b
                : 1 - 2 * (1-a) * (1-b);
            }
            
            void PostLightingMethod(
                SurfaceInput input,
                Surface surface,
                SurfaceCommon surfaceCommon,
                LightingCommon lightingCommon,
                inout half4 color )
            {
            }
            
            // Embed the default shader template for the current render pipeline. These templates
            // contains the actual shader code for the render-pipeline-specific lighting.
            
            // Vertex
            #if defined(SHADERPASS_SHADOWCASTER)
                float3 _LightDirection;
            #endif
            
            float4 UnityObjectToClipPos( float3 positionOS, float3 positionWS, float3 normalWS )
            {
                #if defined(SHADERPASS_SHADOWCASTER)
                    // Define shadow pass specific clip position for Universal
                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #endif
                #elif defined(SHADERPASS_META)
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #else
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #endif
                
                return positionCS;
            }
            
            SurfaceInput vert( VertexAttributes input )
            {
                SurfaceInput output = (SurfaceInput)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                PreVertexMethod( input );
                
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                output.positionWS = positionWS;
                output.normalWS = normalWS;			// normalized in TransformObjectToWorldNormal()
                output.tangentWS = tangentWS;		// normalized in TransformObjectToWorldDir()
                output.texCoord0 = input.uv0;
                output.texCoord1 = input.uv1;
                output.texCoord2 = input.uv2;
                //output.texCoord3 = input.uv3;
                output.color = input.color;
                output.viewDirectionWS.xyz = normalize( _WorldSpaceCameraPos.xyz - positionWS );
                
                VertexMethod( input, output );
                
                positionWS = output.positionWS;
                normalWS = output.normalWS;			// normalized in TransformObjectToWorldNormal()
                tangentWS = output.tangentWS;		// normalized in TransformObjectToWorldDir()
                
                // Recalculate vertex based on modifications to output
                // TODO: Only do this if the position changed
                input.positionOS = TransformWorldToObject( output.positionWS );
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                
                input.uv0 = output.texCoord0;
                input.uv1 = output.texCoord1;
                input.uv2 = output.texCoord2;
                
                #if defined(SHADERPASS_FORWARD)
                    OUTPUT_LIGHTMAP_UV(input.uv1, unity_LightmapST, output.ambientOrLightmapUV);
                    OUTPUT_SH(normalWS, output.ambientOrLightmapUV);
                #endif
                
                #ifdef VARYINGS_NEED_SCREENPOSITION
                    output.screenPosition = ComputeScreenPos(output.positionCS, _ProjectionParams.x);
                #endif
                
                return output;
            }
            
            // Fragment
            
            half4 frag(SurfaceInput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Normalize the ViewDirection per-pixel so that we have an accurate value.
                input.viewDirectionWS.xyz = normalize(input.viewDirectionWS.xyz);
                
                #ifdef LOD_FADE_CROSSFADE
                    // TODO: Dithering is not stable for shadows. Not a big issue since it is usually not noticeable, or the fade is further away than the shadow rendering distance.
                    #if !defined(SHADER_API_GLES)
                        LODDitheringTransition(
                            ComputeFadeMaskSeed(
                                GetWorldSpaceNormalizeViewDir(input.positionWS), // we need a very accurate view direction to get good dithering. The regular viewDirectionWS that we get as input is not accurate enough because it is calculated per-vertex and then interpolated. That is why we calculate the view direction again here.
                                input.positionCS.xy),
                            unity_LODFade.x);
                    #endif
                #endif
                
                Surface surface = (Surface)0;
                surface.Albedo = 1;
                surface.Emission = 0;
                surface.Metallic = 0;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                surface.Alpha = 1;
                surface.Normal = float3(0,0,1);
                SurfaceMethod( input, surface );
                
                MetaInput metaInput = (MetaInput)0;
                metaInput.Albedo = surface.Albedo;
                metaInput.Emission = surface.Emission;
                return MetaFragment(metaInput);
            }
            
            ENDHLSL
        }
        
        Pass
        {
            Name ""
            Tags
            {
                "LightMode" = "Universal2D"
            }
            
            Blend One Zero, One Zero
            ZWrite On
            
            Cull [_DoubleSidedMode]
            
            ZTest LEqual
            
            // TODO: Make sure this works on all platforms.
            
            // Embed the default pass setup.
            // This will overwrite any values that need to be different for specifc passes.
            
            HLSLPROGRAM
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Nature Shaders Settings
            #define NATURE_SHADERS
            
            #define _TYPE_GRASS
            
            // Nature Renderer calculates values that are the same for all vertices once
            // for each object. This is a nice optimization that reduces per-vertex calculations.
            // This only works if Procedural Instancing is enabled.
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                #define PER_OBJECT_VALUES_CALCULATED
            #endif
            
            // Shader Features
            #ifdef _ALPHATEST
                #define _ALPHA_CLIP_ON
                #define _ALPHATEST_ON // HDRP
            #else
                #define _ALPHA_CLIP_OFF
                #define _ALPHATEST_OFF // HDRP
                #define _ALPHA_CLIP_DISABLED
            #endif
            
            #pragma shader_feature_local _COLOR_TINT _COLOR_HSL
            
            #pragma shader_feature_local _WIND_OFF _WIND_AUTO _WIND_BAKED
            #if defined(_WIND_AUTO) || defined(_WIND_BAKED)
                #define _WIND
                #define _WIND_ON
            #endif
            
            // Already defined in Lit template.
            // #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            
            // Already defined in Lit template.
            //#pragma shader_feature_local _EMISSION
            
            #ifndef _INTERACTION_OFF
                #define _INTERACTION_OFF
            #endif
            
            #pragma shader_feature_local _TRANSLUCENCY
            #ifdef _TRANSLUCENCY
                #define _TRANSLUCENCY_ON
                #define _MATERIAL_FEATURE_SUBSURFACE_SCATTERING 1
                #define _MATERIAL_FEATURE_TRANSMISSION 1
                
            #endif
            
            // Include the default cginc files and configurations
            // that are required for the current render pipeline.
            
            // Local keywords are only supported since Unity 2019,
            // so for earlier versions of Unity we need to use global keywords.
            
            // Default global keywords for material quality.
            // #pragma multi_compile MATERIAL_QUALITY_HIGH MATERIAL_QUALITY_MEDIUM MATERIAL_QUALITY_LOW
            #pragma shader_feature_local _LIGHTING_QUALITY_HIGH _LIGHTING_QUALITY_LOW
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            
            #pragma shader_feature_local _SURFACE_MAP_OFF _SURFACE_MAP_PACKED _SURFACE_MAP_METALLIC_GLOSS
            #pragma shader_feature_local _ALPHATEST
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _EMISSION
            
            #pragma multi_compile_instancing
            
            #ifdef _SPECULARHIGHLIGHTS_OFF
                #define _ENVIRONMENTREFLECTIONS_OFF
                #define _GLOSSYREFLECTIONS_OFF
            #endif
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            //#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            
            // Variants
            
            // Attributes and Varyings
            // These are all the possible settings for the URP shaders.
            //#define _SURFACE_TYPE_TRANSPARENT 1
            //#define _AlphaClip 1
            //#define _NORMALMAP 1
            //#define _SPECULAR_SETUP
            //#define _BLENDMODE_ADD 1
            //#define _ALPHAPREMULTIPLY_ON 1
            #define _NORMAL_DROPOFF_TS 1
            //#define _NORMAL_DROPOFF_OS 1
            //#define _NORMAL_DROPOFF_WS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            //#define ATTRIBUTES_NEED_TEXCOORD0
            //#define ATTRIBUTES_NEED_TEXCOORD1
            //#define ATTRIBUTES_NEED_TEXCOORD2
            //#define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            //#define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            //#define VARYINGS_NEED_VIEWDIRECTION_WS
            //#define VARYINGS_NEED_BITANGENT_WS
            //#define VARYINGS_NEED_SCREENPOSITION
            //#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_CULLFACE
            
            #ifdef _MAIN_LIGHT_SHADOWS
                #define VARYINGS_NEED_SHADOWCOORDS
            #endif
            
            #define SHADERPASS_2D
            
            // Common API to unify the different render pipelines.
            
            // Return absolute world position of current object
            float3 GetObjectAbsolutePositionWS()
            {
                float4x4 modelMatrix = UNITY_MATRIX_M;
                return GetAbsolutePositionWS(modelMatrix._m03_m13_m23); // Translation object to world
            }
            
            float3 GetPrimaryCameraPosition()
            {
                #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
                    return float3(0, 0, 0);
                #else
                    return _WorldSpaceCameraPos;
                #endif
            }
            
            // Unity 2020.2 already includes these methods.
            
            uint2 ComputeFadeMaskSeed(float3 V, uint2 positionSS)
            {
                uint2 fadeMaskSeed;
                if (IsPerspectiveProjection())
                {
                    float2 pv = PackNormalOctQuadEncode(V);
                    pv *= _ScreenParams.xy;
                    pv *= UNITY_MATRIX_P._m00_m11;
                    fadeMaskSeed = asuint((int2)pv);
                }
                else
                {
                    fadeMaskSeed = positionSS;
                }
                
                return fadeMaskSeed;
            }
            
            half3 UnpackScaleNormalRGorAG(half4 packednormal, half bumpScale)
            {
                #if defined(UNITY_NO_DXT5nm)
                    half3 normal = packednormal.xyz * 2 - 1;
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    return normal;
                #else
                    // This do the trick
                    packednormal.x *= packednormal.w;
                    
                    half3 normal;
                    normal.xy = (packednormal.xy * 2 - 1);
                    #if (SHADER_TARGET >= 30)
                        // SM2.0: instruction count limitation
                        // SM2.0: normal scaler is not supported
                        normal.xy *= bumpScale;
                    #endif
                    normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
                    return normal;
                #endif
            }
            
            half3 UnpackScaleNormal(half4 packednormal, half bumpScale)
            {
                return UnpackScaleNormalRGorAG(packednormal, bumpScale);
            }
            
            struct IndirectLightingCommon
            {
                half3 diffuse;
                half3 specular;
            };
            
            struct SurfaceCommon
            {
                half3 diffuse;
                half3 specular;
                half3 normalWS;
                half oneMinusReflectivity;
                half smoothness;
                half alpha;
            };
            
            struct LightingCommon
            {
                Light light;
                IndirectLightingCommon indirect;
            };
            
            struct Surface
            {
                float3 Albedo; // base (diffuse or specular) color
                float3 Normal; // tangent space normal, if written
                half3 Emission;
                half Metallic; // 0=non-metal, 1=metal
                half Smoothness; // 0=rough, 1=smooth
                half Occlusion; // occlusion (default 1)
                float Alpha; // alpha for transparencies
                #if defined(_TRANSLUCENCY)
                    float Thickness; // 0 for full translucency, 1 for no translucency.
                #endif
            };
            
            #pragma target 4.0
            
            // Nature Renderer integration
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:SetupNatureRenderer nolightmap forwardadd renderinglayer
            
            // Input
            struct VertexAttributes
            {
                
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1; // optional
                float4 uv2 : TEXCOORD2; // optional
                float4 uv3 : TEXCOORD3; // optional
                
                //
                //uint instanceID : INSTANCEID_SEMANTIC;
                DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct SurfaceInput
            {
                float noise : TEXCOORD11; // TODO: pack noise into positionWS.w or normalWS.w
                
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 color : COLOR;
                float4 texCoord0 : TEXCOORD3;
                float4 texCoord1 : TEXCOORD4;
                float4 texCoord2 : TEXCOORD5;
                float4 viewDirectionWS : TEXCOORD6; //.xyz:view direction .w:fogCoord (standard)
                
                // SH or Lightmap UV
                half4 ambientOrLightmapUV : TEXCOORD7;
                
                // Lighting and shadow coordinates.
                // These are different depending on the render pipeline, so they are wrapped in
                // render pipeline specific tags.
                
                // Meta for editor visualization
                
                // Unity's default instancing settings.
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                
                // VFACE always needs to be the last semantic in the list, otherwise the compiler will throw
                // an error. So we define it here, and let the user add SHADERX_VFACE to the end of
                // the SurfaceInput struct.
                #if defined(SHADER_STAGE_FRAGMENT) //&& defined(VARYINGS_NEED_CULLFACE)
                    #define SHADERX_VFACE FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #else
                    #define SHADERX_VFACE
                #endif
                
                SHADERX_VFACE
            };
            
            // Properties
            CBUFFER_START( UnityPerMaterial )
            //
            float _AlphaTest;
            float _AlphaTestThreshold;
            
            // Fade
            float2 _ScaleFade;
            
            // Color Correction
            float3 _HSL;
            float3 _HSLVariation;
            float4 _Tint;
            float4 _TintVariation;
            float _ColorVariationSpread;
            
            // Surface Settings
            float _DoubleSidedMode;
            float _DoubleSidedNormalMode;
            
            float _VertexNormalStrength;
            float _SurfaceMapMethod;
            
            // Base Maps
            float _NormalMapScale;
            float _Metallic;
            float _Glossiness;
            
            // Surface Maps
            float2 _GlossRemap;
            float2 _OcclusionRemap;
            
            float4 _EmissionColor;
            float _EmissionIntensity;
            
            // Wind
            float _ObjectHeight;
            float _ObjectRadius;
            
            float _Wind;
            float _WindVariation;
            float _WindStrength;
            float _TurbulenceStrength;
            float _RecalculateWindNormals;
            float4 _TrunkBendFactor;
            
            float2 _WindFade;
            
            // Interaction
            
            // Translucency
            float _Translucency;
            
            float _TranslucencyBlendMode;
            float _TranslucencyStrength;
            float _TranslucencyDistortion;
            float _TranslucencyScattering;
            float4 _TranslucencyColor;
            float _TranslucencyAmbient;
            
            float2 _ThicknessRemap;
            
            // Overlay
            
            // Rendering
            float _LightingQuality;
            float _SpecularHighlights;
            float _EnvironmentReflections;
            
            CBUFFER_END
            
            SAMPLER( sampler_Albedo );
            TEXTURE2D( _Albedo );
            #ifdef _NORMALMAP
                TEXTURE2D( _NormalMap );
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                TEXTURE2D( _PackedMap );
            #endif
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                TEXTURE2D( _MetallicGlossMap );
                TEXTURE2D( _OcclusionMap );
            #endif
            
            #ifdef _EMISSION
                TEXTURE2D( _EmissionMap );
            #endif
            
            #ifdef _TRANSLUCENCY
                TEXTURE2D( _ThicknessMap );
            #endif
            
            // Include common features.
            // Properties
            #define GRASS_DEFAULT_HEIGHT 0.5
            #define PLANT_DEFAULT_HEIGHT 1.0
            #define TRUNK_DEFAULT_HEIGHT 20.0
            #define TRUNK_BASE_BEND_FACTOR 0.3
            #define TRUNK_BEND_MULTIPLIER 2.0
            
            float4 g_SmoothTime;
            float3 g_WindDirection;
            float2 g_Wind;
            float2 g_Turbulence;
            sampler2D g_GustNoise;
            
            // Properties that are calculated per-object by Nature Renderer
            #ifdef PER_OBJECT_VALUES_CALCULATED
                float g_WindFade;
                float g_ScaleFade;
                float g_WorldNoise;
                float3 g_ObjectPivot;
                float3 g_ConstantWindOffset;
                float g_PivotOffset;
                float3 g_WorldNormal;
            #endif
            
            float pow2( float x )
            {
                return x*x;
            }
            
            /// <summary>
            /// Returns the height of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectHeight()
            {
                return _ObjectHeight;
            }
            
            /// <summary>
            /// Returns the pivot of the object in world space.
            /// </summary>
            float3 GetObjectPivot()
            {
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    return g_ObjectPivot;
                #else
                    return GetAbsolutePositionWS( float3(UNITY_MATRIX_M[0].w, UNITY_MATRIX_M[1].w, UNITY_MATRIX_M[2].w) );
                #endif
            }
            
            #define GRASS_DEFAULT_RADIUS 1.0
            #define PLANT_DEFAULT_RADIUS 1.0
            #define TREE_DEFAULT_RADIUS 6.0
            
            /// <summary>
            /// Returns the radius of the object.
            /// Is used when no baked data is available.
            /// </summary>
            float GetObjectRadius()
            {
                return _ObjectRadius;
            }
            
            /// <summary>
            /// Returns the vertex normal in world space when vertex normals are anbled.
            /// Otherwise, returns the object's forward (Z+) direction.
            /// </summary>
            float3 GetWorldNormal(
                float3 normalWS, // The vertex normal in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                // New behavior, nice and simple.
                return normalWS;
                
                // Old behavior.
                /*
                #if defined(PER_OBJECT_VALUES_CALCULATED) && !defined(_TYPE_TREE_LEAVES)
                    return g_WorldNormal;
                #else
                    #ifdef _TYPE_TREE_LEAVES
                        // Scramble the vertex normals in case they are projected onto spheres
                        // or other geometry for smooth lighting. Otherwise the wind turbulence will end
                        // up as weird expanding and shrinking spheres.
                        // Define DO_NOT_SCRAMBLE_VERTEX_NORMALS in the shader if the tree models have
                        // accurate normals.
                        #ifndef DO_NOT_SCRAMBLE_VERTEX_NORMALS
                            return normalWS.xzy;
                        #else
                            return normalWS.xyz;
                        #endif
                    #else
                        return TransformObjectToWorldDir( float3(0, 0, 1) );
                    #endif
                #endif
                */
            }
            
            /// <summary>
            /// Returns the mask for the vertex.
            /// Uses the red channel of the vertex color.
            /// </summary>
            float GetVertexMask( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return 1.0;
                #else
                    #ifdef _WIND_BAKED
                        return vertexColor.r;
                    #else
                        return 1.0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Calculates the phase offset for the branch, based on the baked data.
            /// If no baked data is available, it will calculate an approximation of the branch.
            /// Should only be called for trees.
            /// </summary>
            float GetBranchPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_WIND_BAKED)
                    return vertexColor.r;
                #else
                    #if defined(_TYPE_TREE_BARK)
                        return 0;
                    #else
                        float3 offset = vertexWorldPosition - objectPivot;
                        float randomOffset = ( offset.x + offset.y + offset.z ) * 0.005;
                        return randomOffset;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the phase offset for the vertex.
            /// </summary>
            float GetPhaseOffset(
                float4 vertexColor, // The vertex color.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 objectPivot ) // The object pivot in world space.
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return GetBranchPhaseOffset( vertexColor, vertexWorldPosition, objectPivot );
                #else
                    #ifdef _WIND_BAKED
                        return 1.0 - vertexColor.g;
                    #else
                        return 0;
                    #endif
                #endif
            }
            
            /// <summary>
            /// Returns the edge flutter for the vertex,
            /// based either the vertex colors or UV (depending on the Wind Control settings).
            /// </summary>
            float GetEdgeFlutter( float4 vertexColor )
            {
                #if defined(_TYPE_TREE_BARK)
                    return 0;
                #else
                    #if defined(_WIND_BAKED) && defined(_TYPE_TREE_LEAVES)
                        return vertexColor.g;
                    #else
                        return 1;
                    #endif
                #endif
            }
            
            float MaskFromHeightAndRadius( float3 vertex, float height, float radius )
            {
                return pow2( saturate( max(vertex.y / height, length(vertex.xz) / radius) ));
            }
            
            /// <summary>
            /// Returns a mask based on the relative height of the vertex.
            /// </summary>
            float GetHeightMask(
                float3 vertex, // The vertex position in object space.
                float4 vertexColor, // The vertex color.
                float2 uv1 ) // The second UV channel.
            {
                #if defined(_WIND_BAKED)
                    #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                        return uv1.y;
                    #else
                        return vertexColor.a;
                    #endif
                #else
                    #if defined(_TYPE_GRASS)
                        return saturate( vertex.y / GetObjectHeight() );
                    #else
                        return MaskFromHeightAndRadius( vertex, GetObjectHeight(), GetObjectRadius() );
                    #endif
                #endif
            }
            
            float Remap( float value, float2 remap )
            {
                return remap.x + value * (remap.y - remap.x);
            }
            
            float4 SmoothCurve( float4 x )
            {
                return x * x *( 3.0 - 2.0 * x );
            }
            float4 TriangleWave( float4 x )
            {
                return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
            }
            float4 SmoothTriangleWave( float4 x )
            {
                return SmoothCurve( TriangleWave( x ) );
            }
            
            float4 FastSin( float4 x )
            {
                #ifndef PI
                    #define PI 3.14159265
                #endif
                #define DIVIDE_BY_PI 1.0 / (2.0 * PI)
                return (SmoothTriangleWave( x * DIVIDE_BY_PI ) - 0.5) * 2;
            }
            
            float3 FixStretching( float3 vertex, float3 original, float3 center )
            {
                return center + SafeNormalize(vertex - center) * length(original - center);
            }
            
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
            
            float3 RotateAroundAxisFast( float3 center, float3 original, float3 direction )
            {
                return original + direction;
            }
            
            uniform sampler2D g_PerlinNoise;
            uniform float g_PerlinNoiseScale;
            
            void PerlinNoise( float2 uv, float scale, out float noise )
            {
                noise =
                    tex2Dlod(
                        g_PerlinNoise,
                        float4(uv.xy, 0, 0) * scale * g_PerlinNoiseScale).r;
            }
            
            void PerlinNoise_float( float2 uv, float scale, out float noise )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    noise = g_WorldNoise;
                #else
                    PerlinNoise( uv, scale, noise );
                #endif
            }
            
            //
            #ifndef NODE_NATURE_RENDERER_INCLUDED
                #define NODE_NATURE_RENDERER_INCLUDED
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    
                    #define Use_Macro_UNITY_MATRIX_M_instead_of_unity_ObjectToWorld unity_ObjectToWorld
                    #define Use_Macro_UNITY_MATRIX_I_M_instead_of_unity_WorldToObject unity_WorldToObject
                    
                    struct CompressedFloat4x4
                    {
                        uint positionXY;
                        uint positionZ_scale;
                        uint rotationXY;
                        uint rotationZW;
                    };
                    
                    uniform float3 _CompressionRange;
                    uniform float3 _CompressionBase;
                    
                    uint CompressToUshort( float value, float precision )
                    {
                        return (uint)(value / precision * 65535.0);
                    }
                    
                    uint CompressToByte( float value, float precision )
                    {
                        return (uint)(value / precision * 255.0);
                    }
                    
                    float DecompressFromByte( uint value, float precision )
                    {
                        return value / 255.0 * precision;
                    }
                    
                    float DecompressFromUshort( uint value, float precision )
                    {
                        return value / 65535.0 * precision;
                    }
                    
                    void _UnpackInt( uint packedInt, out uint a, out uint b )
                    {
                        a = ( (uint) (packedInt >> 16) );
                        b = ( (uint) ((packedInt << 16) >> 16) );
                    }
                    
                    void _UnpackShort( uint packedShort, out uint a, out uint b )
                    {
                        a = ( (uint) (packedShort >> 8) );
                        b = ( (uint) ((packedShort << 24) >> 24) );
                    }
                    
                    uint _PackInt( uint ushortA, uint ushortB )
                    {
                        return ushortA << 16 | ushortB;
                    }
                    
                    uint _PackShort( uint byteA, uint byteB )
                    {
                        return (byteA << 8) | byteB;
                    }
                    
                    float4x4 QuaternionToMatrix(float4 quaternion)
                    {
                        float4x4 result = (float4x4)0;
                        float x = quaternion.x;
                        float y = quaternion.y;
                        float z = quaternion.z;
                        float w = quaternion.w;
                        
                        float x2 = x + x;
                        float y2 = y + y;
                        float z2 = z + z;
                        float xx = x * x2;
                        float xy = x * y2;
                        float xz = x * z2;
                        float yy = y * y2;
                        float yz = y * z2;
                        float zz = z * z2;
                        float wx = w * x2;
                        float wy = w * y2;
                        float wz = w * z2;
                        
                        result[0][0] = 1.0 - (yy + zz);
                        result[0][1] = xy - wz;
                        result[0][2] = xz + wy;
                        
                        result[1][0] = xy + wz;
                        result[1][1] = 1.0 - (xx + zz);
                        result[1][2] = yz - wx;
                        
                        result[2][0] = xz - wy;
                        result[2][1] = yz + wx;
                        result[2][2] = 1.0 - (xx + yy);
                        
                        result[3][3] = 1.0;
                        
                        return result;
                    }
                    
                    void DecompressInstanceMatrix( inout float4x4 m, CompressedFloat4x4 compressedMatrix )
                    {
                        uint positionX, positionY, positionZ;
                        uint scaleXYZ;
                        uint rotationX, rotationY, rotationZ, rotationW;
                        
                        _UnpackInt( compressedMatrix.positionXY, positionX, positionY );
                        _UnpackInt( compressedMatrix.positionZ_scale, positionZ, scaleXYZ );
                        _UnpackInt( compressedMatrix.rotationXY, rotationX, rotationY );
                        _UnpackInt( compressedMatrix.rotationZW, rotationZ, rotationW );
                        
                        uint scaleX, scaleY;
                        _UnpackShort( scaleXYZ, scaleX, scaleY );
                        
                        float3 position =
                            float3(
                                DecompressFromUshort(positionX, _CompressionRange.x) + _CompressionBase.x,
                                DecompressFromUshort(positionY, _CompressionRange.y) + _CompressionBase.y,
                                DecompressFromUshort(positionZ, _CompressionRange.z) + _CompressionBase.z );
                        
                        float3 scale =
                            float3(
                                DecompressFromByte(scaleX, 16.0),
                                DecompressFromByte(scaleY, 16.0),
                                DecompressFromByte(scaleX, 16.0) );
                        
                        float4 rotation =
                            float4(
                                DecompressFromUshort(rotationX, 2.0) - 1.0,
                                DecompressFromUshort(rotationY, 2.0) - 1.0,
                                DecompressFromUshort(rotationZ, 2.0) - 1.0,
                                DecompressFromUshort(rotationW, 2.0) - 1.0 );
                        
                        m = QuaternionToMatrix( rotation );
                        
                        m[0][0] *= scale.x; m[1][0] *= scale.y; m[2][0] *= scale.z;
                        m[0][1] *= scale.x; m[1][1] *= scale.y; m[2][1] *= scale.z;
                        m[0][2] *= scale.x; m[1][2] *= scale.y; m[2][2] *= scale.z;
                        m[0][3] *= scale.x; m[1][3] *= scale.y; m[2][3] *= scale.z;
                        
                        m[0][3] = position.x;
                        m[1][3] = position.y;
                        m[2][3] = position.z;
                    }
                    
                    #if defined(SHADER_API_GLCORE) \
                        || defined(SHADER_API_D3D11) \
                        || defined(SHADER_API_GLES3) \
                        || defined(SHADER_API_METAL) \
                        || defined(SHADER_API_VULKAN) \
                        || defined(SHADER_API_PSSL) \
                        || defined(SHADER_API_XBOXONE)
                        uniform StructuredBuffer<CompressedFloat4x4> _NatureRendererBuffer;
                    #endif
                    
                    float4x4 inverse(float4x4 input)
                    {
                        #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
                        
                        float4x4 cofactors = float4x4(
                            minor(_22_23_24, _32_33_34, _42_43_44),
                            -minor(_21_23_24, _31_33_34, _41_43_44),
                            minor(_21_22_24, _31_32_34, _41_42_44),
                            -minor(_21_22_23, _31_32_33, _41_42_43),
                            
                            -minor(_12_13_14, _32_33_34, _42_43_44),
                            minor(_11_13_14, _31_33_34, _41_43_44),
                            -minor(_11_12_14, _31_32_34, _41_42_44),
                            minor(_11_12_13, _31_32_33, _41_42_43),
                            
                            minor(_12_13_14, _22_23_24, _42_43_44),
                            -minor(_11_13_14, _21_23_24, _41_43_44),
                            minor(_11_12_14, _21_22_24, _41_42_44),
                            -minor(_11_12_13, _21_22_23, _41_42_43),
                            
                            -minor(_12_13_14, _22_23_24, _32_33_34),
                            minor(_11_13_14, _21_23_24, _31_33_34),
                            -minor(_11_12_14, _21_22_24, _31_32_34),
                            minor(_11_12_13, _21_22_23, _31_32_33)
                            );
                        #undef minor
                        return transpose(cofactors) / determinant(input);
                    }
                #endif
                
                // Pre-calculate and cache data for Nature Shaders that relies on
                // per-object data instead of per-vertex or per-pixel.
                #if defined(PER_OBJECT_VALUES_CALCULATED)
                    void PreCalculateNatureShadersData()
                    {
                        g_ObjectPivot = GetAbsolutePositionWS( float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w) );
                        g_ConstantWindOffset = cross( GetWindDirection().xyz, float3(0,1,0) );
                        g_PivotOffset = length( float3(g_ObjectPivot.x, 0, g_ObjectPivot.z) );
                        g_WorldNormal = TransformObjectToWorldDir( float3(0, 0, 1) );
                        
                        GetFade( g_ObjectPivot, g_WindFade, g_ScaleFade );
                        PerlinNoise( g_ObjectPivot.xz, _ColorVariationSpread, g_WorldNoise);
                    }
                #endif
                
                void SetupNatureRenderer()
                {
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                        DecompressInstanceMatrix(unity_ObjectToWorld, _NatureRendererBuffer[unity_InstanceID]);
                        unity_WorldToObject = inverse(unity_ObjectToWorld);
                    #endif
                    
                    #if defined(PER_OBJECT_VALUES_CALCULATED)
                        PreCalculateNatureShadersData();
                    #endif
                }
                
                void NatureRenderer_float( float3 vertex, out float3 vertexOut )
                {
                    vertexOut = vertex;
                }
            #endif
            
            struct TranslucencyInput
            {
                float Scale;
                float NormalDistortion;
                float Scattering;
                float Thickness;
                float Ambient;
                half3 Color;
            };
            
            half3 Translucency(
                TranslucencyInput input,
                float3 bakedGI,
                float3 surfaceAlbedo,
                float3 surfaceNormal,
                float3 viewDirectionWS,
                Light light )
            {
                half3 lightDir = light.direction + surfaceNormal * input.NormalDistortion;
                half transVdotL =
                    pow( saturate( dot( viewDirectionWS, -lightDir ) ), input.Scattering ) * input.Scale;
                half3 translucency =
                    (transVdotL + bakedGI * input.Ambient)
                    * (1-input.Thickness)
                    * light.shadowAttenuation
                    * light.distanceAttenuation;
                
                return half3( surfaceAlbedo * light.color * translucency * input.Color );
            }
            
            float3 Linear_to_HSV(float3 In)
            {
                float3 sRGBLo = In * 12.92;
                float3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
                float3 Linear = float3(In <= 0.0031308) ? sRGBLo : sRGBHi;
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(Linear.bg, K.wz), float4(Linear.gb, K.xy), step(Linear.b, Linear.g));
                float4 Q = lerp(float4(P.xyw, Linear.r), float4(Linear.r, P.yzx), step(P.x, Linear.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                return float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            }
            
            float3 HSV_to_Linear(float3 In)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                float3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                float3 linearRGBLo = RGB / 12.92;
                float3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                return float3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
            }
            
            void HSL_float( float4 color, float3 hsl, out float4 colorOut )
            {
                float3 hsv = Linear_to_HSV( color.rgb );
                hsv.x += hsl.x;
                hsv.y = saturate(hsv.y + hsl.y * 0.5);
                hsv.z = saturate(hsv.z + hsl.z * 0.5);
                colorOut = float4( HSV_to_Linear(hsv), color.a );
            }
            
            struct WindInput
            {
                // Global
                float3 direction;
                float speed;
                
                // Per-Object
                float3 objectPivot;
                float fade;
                
                // Per-Vertex
                float phaseOffset;
                float3 normalWS;
                float mask;
                float flutter;
            };
            
            /// <summary>
            /// Returns the time for gusts.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothGustOffset()
            {
                return g_SmoothTime.y;
            }
            
            /// <summary>
            /// Returns the time for turbulence.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothTurbulenceOffset()
            {
                return g_SmoothTime.z;
            }
            
            /// <summary>
            /// Returns the global wind direction.
            /// </summary>
            float3 GetWindDirection()
            {
                return g_WindDirection != 0
                ? normalize(float3(g_WindDirection.x, 0, g_WindDirection.z))
                : float3(1, 0, 0);
            }
            
            /// <summary>
            /// Returns the global wind speed.
            /// </summary>
            float GetWindSpeed()
            {
                return g_Wind.x;
            }
            
            /// <summary>
            /// Returns the global wind strength.
            /// </summary>
            float GetWindStrength()
            {
                return g_Wind.y * _WindStrength;
            }
            
            /// <summary>
            /// Returns a random windVariation value based on the object pivot.
            /// </summary>
            float GetWindVariation(
                float3 objectPivot ) // The object pivot in world space.
            {
                return 1.0 - frac( objectPivot.x * objectPivot.z * 10.0 ) * _WindVariation;
            }
            
            /// <summary>
            /// Returns the global turbulence strength.
            /// </summary>
            float GetTurbulenceStrength()
            {
                return g_Turbulence.y * _TurbulenceStrength;
            }
            
            /// <summary>
            /// Returns the time for ambient wind.
            /// Smoothly adjusted for changes in wind speed.
            /// </summary>
            float GetSmoothAmbientOffset()
            {
                return g_SmoothTime.x;
            }
            
            /// <summary>
            /// Calculates the wind frequency for the ambient wind.
            /// Returns a vector4 containing 4 different sine waves.
            /// </summary>
            float4 AmbientFrequency(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset, // The wind phase offset. (Range: 0-1)
                float speed ) // The wind speed.
            {
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float footprint = 3;
                float time = GetSmoothAmbientOffset() - phaseOffset * footprint;
                
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float pivotOffset = g_PivotOffset;
                #else
                    float pivotOffset = length( float3(objectPivot.x, 0, objectPivot.z) );
                #endif
                
                float scale = 0.5;
                float frequency = pivotOffset * scale - time;
                return FastSin(
                    float4(
                        frequency + constantOffset.x,
                        frequency*0.5 + constantOffset.z,
                        frequency*0.25,
                        frequency*0.125) * speed );
            }
            
            /// <summary>
            /// Calculates the ambient wind.
            /// </summary>
            float3 AmbientWind(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 windDirection, // The wind direction in world space.
                float phaseOffset ) // The wind phase offset. (Range: 0-1)
            {
                float4 sine = AmbientFrequency( objectPivot, vertexWorldPosition, windDirection, phaseOffset, 1 );
                sine.w = abs(sine.w) + 0.5;
                float xz = 1.5 * sine.x * sine.z + sine.w + 1;
                float y = 1 * sine.y * sine.z + sine.w;
                return windDirection * float3(xz, 0, xz) + float3(0, y, 0);
            }
            
            /// <summary>
            /// Calculates the turbulence.
            /// </summary>
            float3 Turbulence(
                float3 objectPivot, // The object pivot in world space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 worldNormal, // The direction of the turbulence in world space (usually vertex normal).
                float phaseOffset, // The wind phase offset.
                float edgeFlutter, // The amount of edge flutter for tree leaves. (Range: 0-1)
                float speed ) // The wind speed.
            {
                #if defined(_TYPE_TREE_BARK)
                    return float3(0, 0, 0);
                #else
                    float time = GetSmoothTurbulenceOffset() - phaseOffset;
                    float frequency = (objectPivot.x + objectPivot.y + objectPivot.z) * 2.5 - time;
                    
                    // TODO: Add a secondary frequency.
                    float4 sine =
                        FastSin(
                            float4(
                                (1.65 * frequency) * speed,
                                (2 * 1.65 * frequency) * speed,
                                0,
                                0) );
                    
                    float x = 1 * sine.x + 1;
                    float z = 1 * sine.y + 1;
                    float y = (x + z) * 0.5;
                    
                    #if defined(_TYPE_TREE_LEAVES)
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1) * edgeFlutter;
                    #else
                        return worldNormal * float3(x, y, z) * float3(1, 0.35, 1);
                    #endif
                #endif
            }
            
            float3 SampleGust(
                float3 objectPivot,
                float3 vertexWorldPosition,
                float3 windDirection,
                float phaseOffset,
                float edgeFlutter,
                float lod )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                    lod = 5;
                #else
                    float time = GetSmoothGustOffset() - phaseOffset * 0.05;
                #endif
                
                // Constant offset moving the noise texture slowly to the left to prevent
                // the same gust repeating at the same location.
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    float3 constantOffset = g_ConstantWindOffset * _Time.x * 0.5;
                #else
                    float3 constantOffset = cross( windDirection.xyz, float3(0,1,0) ) * _Time.x * 0.5;
                #endif
                
                float2 windOffsetOverTime = windDirection.xz * time + constantOffset.xz;
                #if defined(_TYPE_TREE_LEAVES)
                    float3 vertexOffset = vertexWorldPosition - objectPivot;
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime + vertexOffset.xz * 0.0075 * edgeFlutter;
                #else
                    float2 offset = objectPivot.xz * 0.02 - windOffsetOverTime;
                #endif
                float strength = tex2Dlod( g_GustNoise, float4(offset, 0, lod) ).r;
                return strength * windDirection;
            }
            
            float3 CombineWind(
                float3 ambient, // Large constant ambient wind motion.
                float3 gust, // Local gust based on noise texture.
                float3 turbulence, // Constant turbulence.
                float3 shiver, // High frequency shivering during gust.
                float4 strength // The wind strength for each wind component.
                )
            {
                ambient *= strength.x;
                gust *= strength.y;
                turbulence *= strength.z;
                shiver *= strength.w;
                
                // Trees require more displacement for the wind to be visible because the objects are larger.
                // These are magic numbers that give a nice balance between the grass/plants and trees,
                // based on a common tree size.
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    ambient *= 3;
                    gust *= 1;
                    turbulence *= 3;
                    shiver *= 3;
                #endif
                
                float gustLength = length( gust );
                float increaseTurbelenceWithGust = smoothstep(0, 1, gustLength) + 1;
                
                // Calculate the balance between different wind types.
                // If we do it here then we can keep the input parameters in a 0-1 range.
                ambient *= 0.1;
                gust *= 1.5;
                turbulence *= 0.15;
                shiver *= 0.15;
                
                #if defined(_DEBUG_AMBIENT)
                    return ambient;
                #elif defined(_DEBUG_GUST)
                    return gust;
                #elif defined(_DEBUG_TURBULENCE)
                    return lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #else
                    return
                    ambient
                    + gust
                    + lerp(
                        turbulence * increaseTurbelenceWithGust,
                        shiver * increaseTurbelenceWithGust,
                        gustLength);
                #endif
            }
            
            float3 ComputeWind( WindInput input, float3 positionWS )
            {
                #if defined(_TYPE_GRASS) || defined(_TYPE_PLANT)
                    input.phaseOffset *= dot( input.direction, (positionWS - input.objectPivot) );
                    input.phaseOffset += input.mask * 0.5;
                #endif
                
                float3 ambient =
                    AmbientWind(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset );
                
                float3 gust =
                    SampleGust(
                        input.objectPivot,
                        positionWS,
                        input.direction,
                        input.phaseOffset,
                        input.flutter,
                        0 );
                
                // Add a bit of a random phase offset to the tree leaves. Phase Offset is calculated
                // per-branch and we don't want to have the same turbulence for the entire branch.
                #if defined(_TYPE_TREE_LEAVES)
                    input.phaseOffset +=
                        dot( input.direction, (positionWS - input.objectPivot) ) * input.flutter;
                #endif
                
                float3 turbulence1 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        1 );
                
                float3 turbulence2 =
                    Turbulence(
                        input.objectPivot.xyz,
                        positionWS.xyz,
                        input.normalWS.xyz,
                        input.phaseOffset,
                        input.flutter,
                        2 );
                
                return CombineWind(
                    ambient,
                    gust,
                    turbulence1,
                    turbulence2,
                    float4(GetWindStrength().xx, GetTurbulenceStrength().xx) );
            }
            
            float3 ApplyWind(
                float3 positionWS, // Vertex position in world space.
                float3 objectPivot, // Object Pivot in world space.
                float3 combinedWind, // Combined Wind vector in world space.
                float mask, // Wind mask. (Range: 0-1)
                float distanceFade) // Wind distance fade. (Range: 0-1)
            {
                #if defined(_TYPE_GRASS)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade,
                        positionWS,
                        float3( positionWS.x, objectPivot.y, positionWS.z ) );
                #elif defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    return FixStretching(
                        positionWS + combinedWind * mask * distanceFade * 4,
                        positionWS,
                        objectPivot);
                #else
                    return FixStretching(
                        positionWS + combinedWind * mask * mask * distanceFade,
                        positionWS,
                        objectPivot);
                #endif
            }
            
            void Wind( WindInput input, inout float3 positionWS, inout float3 normalWS )
            {
                // Adjust the pivot for grass to use the XZ position of the vertex.
                // This is a decent workaround to get a per-grass-blade pivot until
                // we have proper pivot support.
                #ifdef _TYPE_GRASS
                    input.objectPivot = float3(positionWS.x, input.objectPivot.y, positionWS.z);
                #endif
                
                // Compute wind.
                float3 wind = ComputeWind( input, positionWS );
                
                // Apply wind to vertex.
                float3 outputWS =
                    ApplyWind(
                        positionWS,
                        input.objectPivot,
                        wind,
                        input.mask,
                        input.fade );
                
                // Recalculate normals for grass
                #if defined(_TYPE_GRASS)
                    float3 delta = outputWS - positionWS;
                    normalWS =
                        lerp(
                            normalWS,
                            normalWS + SafeNormalize( delta + float3(0, 0.1, 0) ),
                            length(delta) * _RecalculateWindNormals * input.fade );
                #endif
                
                positionWS = outputWS;
            }
            
            /// <summary>
            /// Returns the bend factor for the tree trunk.
            /// X contains the bend factor for the entire tree, Y for the base.
            /// </summary>
            float2 GetTrunkBendFactor()
            {
                return _TrunkBendFactor.xy;
            }
            
            float GetTrunkMask(
                float3 vertex, float2 uv1, float treeHeight, float bendFactor, float baseBendFactor )
            {
                #ifdef _WIND_BAKED
                    float trunkMask = saturate( uv1.x * bendFactor );
                #else
                    float trunkMask = pow2(saturate( vertex.y / treeHeight )) * bendFactor;
                #endif
                
                return saturate( trunkMask + saturate( vertex.y ) * baseBendFactor );
            }
            
            void Wind_Trunk(
                float3 vertex, // The vertex position in object space.
                float3 vertexWorldPosition, // The vertex position in world space.
                float3 vertexWithWind, // The vertex position with wind displacement in world space.
                float2 uv1, // The second UV channel of the vertex. (UV1)
                float3 objectPivot, // The object pivot in world space.
                float3 windDirection, // The wind direction in world space. (normalized)
                out float3 vertexOut )
            {
                // Additional properties. Either global or baked.
                float2 bendFactor = GetTrunkBendFactor();
                float trunkMask = GetTrunkMask( vertex, uv1, GetObjectHeight(), bendFactor.x, bendFactor.y );
                float ambientStrength = GetWindStrength();
                
                // Calculate Ambient Wind
                float4 trunkAmbient =
                    AmbientFrequency(
                        objectPivot,
                        vertexWorldPosition,
                        windDirection,
                        0,
                        0.75 ) + ambientStrength;
                trunkAmbient *= trunkMask;
                
                // Calculate Gust
                float3 trunkGust =
                    SampleGust(
                        objectPivot, vertexWorldPosition, windDirection, 0, 0, 7);
                trunkGust *= trunkMask;
                
                // Apply
                float gustFrequency = trunkAmbient.w * length(trunkGust);
                float baseFrequency1 = trunkAmbient.x;
                float baseFrequency2 = trunkAmbient.x + trunkAmbient.y;
                float baseFrequency =
                    lerp( baseFrequency1, baseFrequency2, (_SinTime.x + 1) * 0.5 * ambientStrength);
                
                // TODO: Use the "FixStretching" approach?
                vertexOut =
                    RotateAroundAxis(
                        objectPivot,
                        vertexWithWind,
                        normalize( cross( float3(0,1,0) , windDirection ) ),
                        (baseFrequency * 0.75 + gustFrequency) * ambientStrength * 0.0375);
            }
            
            void AlphaTest( float alpha, float threshold )
            {
                
                clip( alpha - threshold );
            }
            
            void GetFade(
                float3 objectPivot, // The object pivot in world space.
                out float windFade,
                out float scaleFade )
            {
                #if defined(_TYPE_TREE_LEAVES) || defined(_TYPE_TREE_BARK)
                    windFade = 1.0;
                    scaleFade = 1.0;
                #else
                    float distanceToCamera = distance( objectPivot, _WorldSpaceCameraPos );
                    windFade = 1.0 - saturate( (distanceToCamera - _WindFade.x) / _WindFade.y );
                    scaleFade = 1.0 - saturate( (distanceToCamera - _ScaleFade.x) / _ScaleFade.y );
                #endif
            }
            
            void PerVertexFade( float3 objectPivot, out float windFade, out float scaleFade )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    windFade = g_WindFade;
                    scaleFade = g_ScaleFade;
                #else
                    GetFade(
                        objectPivot,
                        windFade,
                        scaleFade );
                #endif
            }
            
            void Wind(
                inout VertexAttributes vertex,
                inout SurfaceInput surface,
                inout float3 positionWS,
                float windFade,
                float scaleFade,
                float heightMask,
                float phaseOffset,
                float3 objectPivot )
            {
                WindInput input;
                
                // Global
                input.direction = GetWindDirection();
                input.speed = GetWindSpeed();
                
                // Per-Object
                input.objectPivot = objectPivot;
                input.fade = windFade;
                
                // Per-Vertex
                input.phaseOffset = phaseOffset;
                input.normalWS = surface.normalWS;
                float windVariation = GetWindVariation( input.objectPivot );
                float vertexMask = GetVertexMask( vertex.color );
                input.mask = heightMask * vertexMask * windVariation;
                input.flutter = GetEdgeFlutter( vertex.color );
                
                float3 vertexOut = positionWS;
                float3 normalOut = surface.normalWS;
                Wind(
                    input,
                    vertexOut,
                    normalOut );
                
                #if defined(_TYPE_TREE_LEAVES) || defined( _TYPE_TREE_BARK )
                    Wind_Trunk(
                        vertex.positionOS.xyz,
                        positionWS.xyz,
                        vertexOut.xyz,
                        vertex.uv1.xy,
                        input.objectPivot,
                        input.direction,
                        vertexOut );
                #endif
                
                positionWS = vertexOut;
                surface.normalWS = normalOut;
            }
            
            float PerVertexPerlinNoise( float3 objectPivot )
            {
                #ifdef PER_OBJECT_VALUES_CALCULATED
                    return g_WorldNoise;
                #else
                    float noise;
                    PerlinNoise_float( objectPivot.xz, _ColorVariationSpread, noise );
                    return noise;
                #endif
            }
            
            float3 ApplyScaleFade(
                float3 vertexWorldPosition, float3 objectPivot, float fade )
            {
                vertexWorldPosition.y = lerp(objectPivot.y, vertexWorldPosition.y, max(fade, 0.2));
                return vertexWorldPosition;
            }
            
            // Surface
            void SampleAlbedo( float2 uv0, out float4 albedo )
            {
                albedo = SAMPLE_TEXTURE2D( _Albedo, sampler_Albedo, uv0.xy );
            }
            
            void ApplyColorCorrection( inout float4 albedo, float noise )
            {
                #ifdef _COLOR_HSL
                    float4 albedo1 = albedo;
                    float4 albedo2 = albedo;
                    HSL_float( albedo1, _HSL, albedo1 );
                    HSL_float( albedo2, _HSLVariation, albedo2 );
                    albedo = lerp(albedo2, albedo1, noise);
                #else
                    albedo *= lerp(_TintVariation, _Tint, noise);
                #endif
            }
            
            #ifdef _NORMALMAP
                void SampleNormalMap( float2 uv0, out float3 normal )
                {
                    normal =
                        UnpackScaleNormal(
                            SAMPLE_TEXTURE2D( _NormalMap, sampler_Albedo, uv0.xy ), _NormalMapScale ).xyz;
                }
            #endif
            
            #ifdef _SURFACE_MAP_PACKED
                void SamplePackedMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 packed = SAMPLE_TEXTURE2D(_PackedMap, sampler_Albedo, uv0.xy);
                    metallic = packed.r;
                    smoothness = Remap(packed.a, _GlossRemap.xy);
                    occlusion = Remap(packed.g, _OcclusionRemap.xy);
                }
            #endif
            
            #ifdef _SURFACE_MAP_METALLIC_GLOSS
                void SampleMetallicGlossMap(
                    float2 uv0, out float metallic, out float smoothness, out float occlusion)
                {
                    float4 metallicGloss = SAMPLE_TEXTURE2D( _MetallicGlossMap, sampler_Albedo, uv0.xy );
                    metallic = metallicGloss.r;
                    smoothness = Remap(metallicGloss.a, _GlossRemap.xy);
                    occlusion = Remap(SAMPLE_TEXTURE2D( _OcclusionMap, sampler_Albedo, uv0.xy ).g, _OcclusionRemap.xy);
                }
            #endif
            
            void SampleMetallicGlossConstants(
                float2 uv0, out float metallic, out float smoothness, out float occlusion)
            {
                metallic = _Metallic;
                smoothness = _Glossiness;
                occlusion = 1.0;
            }
            
            #ifdef _TRANSLUCENCY
                void SampleThickness( float2 uv0, out float thickness )
                {
                    thickness = SAMPLE_TEXTURE2D( _ThicknessMap, sampler_Albedo, uv0.xy ).r;
                    thickness = Remap( thickness, _ThicknessRemap.xy );
                }
            #endif
            
            #ifdef _EMISSION
                void SampleEmission( float2 uv0, out float3 emission )
                {
                    emission = SAMPLE_TEXTURE2D( _EmissionMap, sampler_Albedo, uv0.xy ).rgb;
                    emission *= _EmissionColor.rgb;
                    emission *= _EmissionIntensity;
                }
            #endif
            
            void FlipNormals( inout float3 normals, FRONT_FACE_TYPE cullFace )
            {
                normals.z *=
                    _DoubleSidedNormalMode > 0
                    ? IS_FRONT_VFACE(cullFace, 1, -1)
                    : 1.0;
            }
            
            // Called with raw vertex data before doing any kind of calculations or transformations.
            // Useful to modify the vertex data in object space.
            void PreVertexMethod( inout VertexAttributes vertex )
            {
            }
            
            // The main vertex method. Is used to modify the vertex data and
            // the input for the surface (fragment) method.
            void VertexMethod(
                VertexAttributes vertex,
                inout SurfaceInput surface )
            {
                float3 objectPivot = GetObjectPivot();
                float3 positionWS = GetAbsolutePositionWS( surface.positionWS.xyz );
                float3 positionWSOriginal = positionWS;
                
                surface.noise = PerVertexPerlinNoise( objectPivot );
                
                float windFade;
                float scaleFade;
                PerVertexFade( objectPivot, windFade, scaleFade );
                
                float heightMask =
                    GetHeightMask(
                        vertex.positionOS.xyz,
                        vertex.color,
                        vertex.uv1.xy );
                
                float phaseOffset =
                    GetPhaseOffset(
                        vertex.color,
                        positionWS,
                        objectPivot );
                
                #ifdef _WIND
                    Wind(
                        vertex,
                        surface,
                        positionWS,
                        windFade,
                        scaleFade,
                        heightMask,
                        phaseOffset,
                        objectPivot );
                #endif
                
                positionWS =
                    ApplyScaleFade( positionWS, objectPivot, scaleFade );
                
                surface.positionWS = GetCameraRelativePositionWS( positionWS );
                surface.normalWS = lerp(TransformObjectToWorldNormal(float3(0,1,0)), surface.normalWS, _VertexNormalStrength);
            }
            
            void SurfaceMethod(
                SurfaceInput input,
                inout Surface output )
            {
                // Albedo
                float4 albedo;
                SampleAlbedo( input.texCoord0.xy, albedo );
                
                // Overlay
                
                // Alpha clip
                #ifdef _ALPHATEST
                    
                    AlphaTest( albedo.a, _AlphaTestThreshold );
                    
                #else
                    albedo.a = 1;
                #endif
                
                // Color correction
                ApplyColorCorrection( albedo, input.noise );
                output.Albedo = albedo.rgb;
                output.Alpha = albedo.a;
                
                // Normal Map
                #ifdef _NORMALMAP
                    SampleNormalMap( input.texCoord0.xy, output.Normal );
                #endif
                
                // Surface Map
                #if defined(_SURFACE_MAP_PACKED)
                    SamplePackedMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #elif defined(_SURFACE_MAP_METALLIC_GLOSS)
                    SampleMetallicGlossMap(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #else
                    SampleMetallicGlossConstants(
                        input.texCoord0.xy, output.Metallic, output.Smoothness, output.Occlusion );
                #endif
                
                // Translucency
                #ifdef _TRANSLUCENCY
                    SampleThickness( input.texCoord0.xy, output.Thickness );
                #endif
                
                // Emission
                #ifdef _EMISSION
                    SampleEmission( input.texCoord0.xy, output.Emission );
                #endif
                
                // Flip double-sided normals
                #ifdef SHADER_STAGE_FRAGMENT
                    FlipNormals( output.Normal, input.cullFace );
                #endif
            }
            
            float3 Overlay(float3 a, float3 b)
            {
                return a < 0.5
                ? 2 * a * b
                : 1 - 2 * (1-a) * (1-b);
            }
            
            void PostLightingMethod(
                SurfaceInput input,
                Surface surface,
                SurfaceCommon surfaceCommon,
                LightingCommon lightingCommon,
                inout half4 color )
            {
            }
            
            // Embed the default shader template for the current render pipeline. These templates
            // contains the actual shader code for the render-pipeline-specific lighting.
            
            // Vertex
            #if defined(SHADERPASS_SHADOWCASTER)
                float3 _LightDirection;
            #endif
            
            float4 UnityObjectToClipPos( float3 positionOS, float3 positionWS, float3 normalWS )
            {
                #if defined(SHADERPASS_SHADOWCASTER)
                    // Define shadow pass specific clip position for Universal
                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                    #endif
                #elif defined(SHADERPASS_META)
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #else
                    float4 positionCS = TransformWorldToHClip(positionWS);
                #endif
                
                return positionCS;
            }
            
            SurfaceInput vert( VertexAttributes input )
            {
                SurfaceInput output = (SurfaceInput)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                PreVertexMethod( input );
                
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                output.positionWS = positionWS;
                output.normalWS = normalWS;			// normalized in TransformObjectToWorldNormal()
                output.tangentWS = tangentWS;		// normalized in TransformObjectToWorldDir()
                output.texCoord0 = input.uv0;
                output.texCoord1 = input.uv1;
                output.texCoord2 = input.uv2;
                //output.texCoord3 = input.uv3;
                output.color = input.color;
                output.viewDirectionWS.xyz = normalize( _WorldSpaceCameraPos.xyz - positionWS );
                
                VertexMethod( input, output );
                
                positionWS = output.positionWS;
                normalWS = output.normalWS;			// normalized in TransformObjectToWorldNormal()
                tangentWS = output.tangentWS;		// normalized in TransformObjectToWorldDir()
                
                // Recalculate vertex based on modifications to output
                // TODO: Only do this if the position changed
                input.positionOS = TransformWorldToObject( output.positionWS );
                output.positionCS = UnityObjectToClipPos( input.positionOS.xyz, positionWS, normalWS );
                
                input.uv0 = output.texCoord0;
                input.uv1 = output.texCoord1;
                input.uv2 = output.texCoord2;
                
                #if defined(SHADERPASS_FORWARD)
                    OUTPUT_LIGHTMAP_UV(input.uv1, unity_LightmapST, output.ambientOrLightmapUV);
                    OUTPUT_SH(normalWS, output.ambientOrLightmapUV);
                #endif
                
                #ifdef VARYINGS_NEED_SCREENPOSITION
                    output.screenPosition = ComputeScreenPos(output.positionCS, _ProjectionParams.x);
                #endif
                
                return output;
            }
            
            // Fragment
            
            half4 frag(SurfaceInput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Normalize the ViewDirection per-pixel so that we have an accurate value.
                input.viewDirectionWS.xyz = normalize(input.viewDirectionWS.xyz);
                
                #ifdef LOD_FADE_CROSSFADE
                    // TODO: Dithering is not stable for shadows. Not a big issue since it is usually not noticeable, or the fade is further away than the shadow rendering distance.
                    #if !defined(SHADER_API_GLES)
                        LODDitheringTransition(
                            ComputeFadeMaskSeed(
                                GetWorldSpaceNormalizeViewDir(input.positionWS), // we need a very accurate view direction to get good dithering. The regular viewDirectionWS that we get as input is not accurate enough because it is calculated per-vertex and then interpolated. That is why we calculate the view direction again here.
                                input.positionCS.xy),
                            unity_LODFade.x);
                    #endif
                #endif
                
                Surface surface = (Surface)0;
                surface.Albedo = 1;
                surface.Emission = 0;
                surface.Metallic = 0;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                surface.Alpha = 1;
                surface.Normal = float3(0,0,1);
                SurfaceMethod( input, surface );
                
                return half4(surface.Albedo, surface.Alpha);
            }
            
            ENDHLSL
        }
    }
    
    Fallback Off
    CustomEditor "VisualDesignCafe.Nature.Materials.Editor.NatureMaterialEditor"
}

Shader "Hidden/OutlineBlit"
{
    Properties
    {
	    [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineCol("color",  Color) = (0,0,0,1)
		_Delta ("Line Thickness", Range(0.0005, 0.25)) = 0.001
        _Pow ("power", Range(0,1000))= 50
        _Add("add", Range(0,0.1))= 0.0001
        _Cut("cut", Range(0,10))= 0.1
		[Toggle(RAW_OUTLINE)]_Raw ("Outline Only", Float) = 0
        [Toggle(REVERSED_OUTLINE)]_Reversed ("Outline Reversed", Float) = 0
    }
    SubShader
    {
		Tags { "RenderType"="Opaque" }
		LOD 200

        Pass
        {
            Name "Sobel Filter"
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            
            #pragma shader_feature RAW_OUTLINE
            #pragma shader_feature REVERSED_OUTLINE
            
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
#ifndef RAW_OUTLINE
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
#endif
            float _Delta;
            float _Pow;
            float _Add;
            float _Cut;
            half4 _OutlineCol;
            int _PosterizationCount;
            
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float SampleDepth(float2 uv)
            {
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
#else
                return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
#endif
            }
            
            float sobel (float2 uv) 
            {
                float2 delta = float2(_Delta, _Delta);
                
                float hr = 0;
                float vt = 0;
                
                hr += SampleDepth(uv + float2(-1.0, -1.0) * delta) *  1.0;
                hr += SampleDepth(uv + float2( 1.0, -1.0) * delta) * -1.0;
                hr += SampleDepth(uv + float2(-1.0,  0.0) * delta) *  2.0;
                hr += SampleDepth(uv + float2( 1.0,  0.0) * delta) * -2.0;
                hr += SampleDepth(uv + float2(-1.0,  1.0) * delta) *  1.0;
                hr += SampleDepth(uv + float2( 1.0,  1.0) * delta) * -1.0;
              
                vt += SampleDepth(uv + float2(-1.0, -1.0) * delta) *  1.0;
                vt += SampleDepth(uv + float2( 0.0, -1.0) * delta) *  2.0;
                vt += SampleDepth(uv + float2( 1.0, -1.0) * delta) *  1.0;
                vt += SampleDepth(uv + float2(-1.0,  1.0) * delta) * -1.0;
                vt += SampleDepth(uv + float2( 0.0,  1.0) * delta) * -2.0;
                vt += SampleDepth(uv + float2( 1.0,  1.0) * delta) * -1.0;

            
                 return sqrt((hr * hr + vt * vt)/**(SampleDepth(uv))*/);
            }
            


            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;
                
                return output;
            }
            
            half4 frag (Varyings input) : SV_Target 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float s = pow((1 - saturate(sobel(input.uv))+_Add), _Pow);
                
                s = clamp(s ,0 ,1 );
                                float mask = clamp(floor(s*_Cut),0 ,1);

                s = lerp(s, 1, mask);
#ifdef RAW_OUTLINE
                return half4(s.xxx, 1);
#else
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

#ifdef REVERSED_OUTLINE
                s = 1 - s ;
#endif         
                return col *s + (1-s) * _OutlineCol ;
#endif
            }
            
			#pragma vertex vert
			#pragma fragment frag
			
			ENDHLSL
        }
    }
}

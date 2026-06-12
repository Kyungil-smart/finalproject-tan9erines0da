Shader "Unlit/UI_ImageFixedShader"
{
    Properties
    {
        // UI 컴포넌트의 스프라이트를 자동으로 바인딩하기 위한 변수
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "black" {}
        
        // C# 스크립트와 슬라이더 바(Bar)로 제어할 프로퍼티들
        _Brightness ("Brightness", Range(-1, 1)) = 0
        _Contrast ("Contrast", Range(0, 2)) = 1
        _Saturation ("Saturation", Range(0, 2)) = 1
        _Temperature ("Temperature", Range(0, 1)) = 0.5
        
        _ColdColor ("Cold Target Color", Color) = (0.15, 0.35, 0.85, 1)
        _WarmColor ("Warm Target Color", Color) = (0.85, 0.65, 0.15, 1)

        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilWriteMask ("Write Mask", Float) = 255
        _StencilReadMask ("Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
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

        // UI 알파 블렌딩 및 뎁스 제어 기본 세팅
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            // 보정 제어 변수 선언
            half _Brightness;
            half _Contrast;
            half _Saturation;
            half _Temperature;
            
            half4 _ColdColor;
            half4 _WarmColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                // 스크린/카메라 렌더 모드 모두 호환되는 클립 좌표 변환
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                o.texcoord = v.texcoord;
                o.color = v.color; // Vertex Color (DOTween 페이드 지원)
                return o;
            }

            // 포토샵의 Soft Light(소프트 라이트) 공식 매크로 함수
            // 명암을 훼손하지 않고 필터 색상만 부드럽게 입히는 핵심 공식입니다.
            float3 BlendSoftLight(float3 base, float3 blend)
            {
                return lerp(
                    2.0f * base * blend + base * base * (1.0f - 2.0f * blend),
                    2.0f * base * (1.0f - blend) + sqrt(base) * (2.0f * blend - 1.0f),
                    step(0.5f, blend)
                );
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UI 원본 텍스처 샘플링
                fixed4 color = (tex2D(_MainTex, i.texcoord) 
                                + _TextureSampleAdd) * i.color;

                // [Step 1] 상대적 밝기 (Brightness)
                half origLum = dot(color.rgb, 
                               half3(0.2126f, 0.7152f, 0.0722f));
                color.rgb += _Brightness * (color.rgb * (1.0f - origLum) 
                             + 0.1f);

                // [Step 2] 상용 툴 규격 S-Curve 대비 (Contrast) 연산
                // 부드러운 삼차식 Hermite Interpolation(Smoothstep)을 활용
                // 대비가 1일 때는 변함없고, 수치에 따라 S-곡선률을 가감합니다.
                float3 sCurve = smoothstep(0.0f, 1.0f, color.rgb);
                color.rgb = lerp(color.rgb, sCurve, _Contrast - 1.0f);

                // [Step 3] 상용 툴 규격 활기 기반 채도 (Vibrance) 연산
                // RGB 채널 중 가장 강한 원색 지중값을 계산합니다.
                half maxColor = max(color.r, max(color.g, color.b));
                half minColor = min(color.r, min(color.g, color.b));
                half colorAmt = maxColor - minColor; // 현재 픽셀의 채도량

                // 이미 채도가 높은 곳은 가중치를 낮추는 상용 보정 공식
                // _Saturation 변수를 활기 수치(0~2 범위)로 그대로 바인딩합니다.
                half vibrance = (_Saturation - 1.0f) * 1.5f;
                half filterExt = vibrance * (1.0f - colorAmt);
                
                // 원색이 타버리는 것을 막으며 선별적 채도 증폭
                color.rgb = lerp(color.rgb, float3(maxColor, maxColor, maxColor), -filterExt);

                // [Step 4] 포토샵 Soft Light 기반 색온도 (Temperature)
                half3 filterColor = half3(0.5f, 0.5f, 0.5f);
                if (_Temperature < 0.5f)
                {
                    half t = (0.5f - _Temperature) * 2.0f;
                    filterColor = lerp(filterColor, _ColdColor.rgb, t);
                }
                else
                {
                    half t = (_Temperature - 0.5f) * 2.0f;
                    filterColor = lerp(filterColor, _WarmColor.rgb, t);
                }

                // 소프트 라이트 최종 합성 및 마감
                color.rgb = BlendSoftLight(color.rgb, filterColor);
                color.rgb = clamp(color.rgb, 0.0f, 1.0f);

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                return color;
            }
            ENDCG
        }
    }
}

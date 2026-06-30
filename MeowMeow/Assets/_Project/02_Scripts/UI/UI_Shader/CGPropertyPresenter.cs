using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGPropertyPresenter : MonoBehaviour, ISNSPanelPresenter, ISNSContextReceiver
{
    [Header("CG SO")]
    [SerializeField] CGImageDatabase _cgDatabase;

    [Header("프리뷰")]
    [SerializeField] private UIImageShaderController _shaderController;
    [SerializeField] private Image _previewImage;

    [Header("슬라이더")]
    [SerializeField] private Slider _sliderBrightness;
    [SerializeField] private Slider _sliderContrast;
    [SerializeField] private Slider _sliderSaturation;
    [SerializeField] private Slider _sliderTemperature;

    private SNSPostDTO _snapshot;

    public void RequestContext()
    {
        UnbindSlider();

        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }
    void OnDisable()
    {
        UnbindSlider();

        // ResetSliderComponentsToDefault();
    }

    /// <summary>
    /// 편집을 마치고 최종 수정된 셰이더 프로퍼티 스냅샷을 
    /// SNS 데이터 프레젠터에 최종 반영 및 저장 요청합니다.
    /// </summary>
    public void SubmitContext()
    {
        // 슬라이더의 밸류를 취합합니다
        UIShaderProperty property = new UIShaderProperty
        {
            Brightness = _sliderBrightness.value,
            Contrast = _sliderContrast.value,
            Saturation = _sliderSaturation.value,
            Temperature = _sliderTemperature.value
        };

        // 스냅샷에 셰이더프로퍼티를 수정합니다
        _snapshot.ShaderProperty = property;

        // 데이터 프레젠터에게 스냅샷을 담아 발행합니다
        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);

    }
    // 이전에 넘어온 DTO를 확인하기 위한 콜백 함수
    // 해당 데이터를 바탕으로 초기화 또한 진행합니다
    public void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        _snapshot = snapshot;

        SetPreviewImage();

        InitSliderValue();

        BindSlider();
    }

    // 슬라이더 값을 초기화하는 함수
    private void InitSliderValue()
    {
        UIShaderProperty savedShader = _snapshot.ShaderProperty;

        _sliderBrightness.value = savedShader.Brightness;
        _sliderContrast.value = savedShader.Contrast;
        _sliderSaturation.value = savedShader.Saturation;
        _sliderTemperature.value = savedShader.Temperature;

        UpdateShaderRendering(savedShader);
    }

    private void ResetSliderComponentsToDefault()
    {
        _sliderBrightness.value  = 0f;
        _sliderContrast.value    = 1f;
        _sliderSaturation.value  = 1f;
        _sliderTemperature.value = 0.5f;

        UpdateShaderRendering(new UIShaderProperty
        {
            Brightness  = 0f,
            Contrast    = 1f,
            Saturation  = 1f,
            Temperature = 0.5f
        });
    }
    // 스냅샷(SNSPostDTO)을 기준으로 프리뷰 이미지를 설정하는 함수
    private void SetPreviewImage()
    {
        if (_cgDatabase != null && _previewImage != null)
        {
            Sprite targetSprite = _cgDatabase.GetSprite(_snapshot.ImageIndex);
            _previewImage.sprite = targetSprite;
        }
    }

    private void BindSlider()
    {
        _sliderBrightness.onValueChanged.AddListener(OnSliderValueChanged);
        _sliderContrast.onValueChanged.AddListener(OnSliderValueChanged);
        _sliderSaturation.onValueChanged.AddListener(OnSliderValueChanged);
        _sliderTemperature.onValueChanged.AddListener(OnSliderValueChanged);
    }
    private void UnbindSlider()
    {
        _sliderBrightness.onValueChanged.RemoveListener(OnSliderValueChanged);
        _sliderContrast.onValueChanged.RemoveListener(OnSliderValueChanged);
        _sliderSaturation.onValueChanged.RemoveListener(OnSliderValueChanged);
        _sliderTemperature.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.ClickyButton3a);

        UIShaderProperty newProperties = new UIShaderProperty
        {
            Brightness = _sliderBrightness.value,
            Contrast = _sliderContrast.value,
            Saturation = _sliderSaturation.value,
            Temperature = _sliderTemperature.value
        };

        // 컨트롤러 API 호출 (실시간 렌더링 반영)
        UpdateShaderRendering(newProperties);
    }

    // 셰이더 프로퍼티 컨트롤러의 API를 사용해 프로퍼티 수정
    private void UpdateShaderRendering(UIShaderProperty properties)
    {
        if (_shaderController == null) return;

        _shaderController.UpdateShaderProperties(
            properties.Brightness,
            properties.Contrast,
            properties.Saturation,
            properties.Temperature
        );
    }
}

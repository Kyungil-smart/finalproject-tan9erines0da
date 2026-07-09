using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageShaderController : MonoBehaviour
{
    // 셰이더 내부에 정의된 프로퍼티 네이밍 상수를 ID로 캐싱
    private static readonly int IdBrightness     = Shader.PropertyToID("_Brightness");
    private static readonly int IdContrast       = Shader.PropertyToID("_Contrast");
    private static readonly int IdSaturation     = Shader.PropertyToID("_Saturation");
    private static readonly int IdTemperature    = Shader.PropertyToID("_Temperature");

    private Image _targetImage;
    private Material _clonedMaterial;

    private void Awake()
    {
        if(_clonedMaterial == null ) Init();
    }
    private void Init()
    {
        _targetImage = GetComponent<Image>();

        // 원본 머터리얼이 존재할 때만 런타임 인스턴스 복제 진행
        if (_targetImage != null && _targetImage.material != null)
        {
            // 인스턴스화를 통해 이 오브젝트만의 독립된 머터리얼 생성
            _clonedMaterial = new Material(_targetImage.material);
            _targetImage.material = _clonedMaterial;

            _clonedMaterial.SetFloat(IdBrightness, 0f);
            _clonedMaterial.SetFloat(IdContrast, 1f); 
            _clonedMaterial.SetFloat(IdSaturation, 1f); 
            _clonedMaterial.SetFloat(IdTemperature, 0.5f);
        }
    }
    private void OnDestroy()
    {
        // 런타임에 동적 생성한 머터리얼은 메모리 누수 방지를 위해 
        // 오브젝트 파괴 시점에 반드시 명시적으로 메모리 해제
        if (_clonedMaterial != null)
        {
            Destroy(_clonedMaterial);
        }
    }
    

    /// <summary>
    /// 외부 UI 슬라이더 등에서 변동된 4종 보정 수치를 받아 
    /// 격리된 머터리얼에 즉각 반영하는 함수입니다.
    /// </summary>
    /// <param name="brightness">밝기 (기본값: 0.0)</param>
    /// <param name="contrast">대비 (기본값: 1.0)</param>
    /// <param name="saturation">채도 (기본값: 1.0)</param>
    /// <param name="temperature">온도 (기본값: 0.5)</param>
    public void UpdateShaderProperties(
        float brightness, float contrast,
        float saturation, float temperature)
    {
        if (_clonedMaterial == null) Init();
        if (_clonedMaterial == null) return;

        // 캐싱된 Property ID를 활용하여 셰이더 내부 변수 값 교체
        _clonedMaterial.SetFloat(IdBrightness, brightness);
        _clonedMaterial.SetFloat(IdContrast, contrast);
        _clonedMaterial.SetFloat(IdSaturation, saturation);
        _clonedMaterial.SetFloat(IdTemperature, temperature);

        if (_targetImage != null)
        {
            _targetImage.SetMaterialDirty();
        }
    }
}

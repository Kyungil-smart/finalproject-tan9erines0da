using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InitLodingTweenAni : MonoBehaviour
{
    [Header("FakeLodingSlider의 Fill_Image를 참조")]
    [SerializeField] private RectTransform _fill;
    [Header("Title_Image를 참조")]
    [SerializeField] private GameObject _title;
    [Header("FakeLodingSlider의 Loding_Text (TMP)를 참조")]
    [SerializeField] private TextMeshProUGUI _lodingText;

    // Fill_Image의 원래 크기 저장
    private Vector2 _originalSize;

    private void Awake()
    {
        // 원본 크기 저장
        _originalSize = _fill.sizeDelta;
    }

    private async void Start()
    {
        await PlayAnimation();
    }

    [ContextMenu("로딩 애니메이션 재생")]
    public Task PlayAnimation()
    {
        if (_fill == null)
        {
            Debug.LogWarning("_fill가 비어있습니다.");
            return Task.CompletedTask;
        }

        _fill.DOKill();

        // 타이틀 화면 비활성화
        _title.SetActive(false);

        // 왼쪽 기준으로 늘어나게 하기위한 설정
        _fill.pivot = new Vector2(0f, 0.5f);
        _fill.sizeDelta = new Vector2(0f, _originalSize.y);
        _lodingText.text = "0%";

        Sequence seq = DOTween.Sequence();

        // 0% ~ 70% 까지 게이지 점점 빨라짐
        seq.Append(_fill.DOSizeDelta(new Vector2(_originalSize.x * 0.7f, _originalSize.y), 1.5f).SetEase(Ease.InQuad));

        // 타이틀 화면 활성화(원본 게임이 70% 정도에 타이틀 화면이 켜졌습니다.)
        seq.AppendCallback(() =>
        {
            _title.SetActive(true);
        });

        // 0.3초 대기
        seq.AppendInterval(0.3f);

        // 70% ~ 95% 까지 게이지 점점 느려짐
        seq.Append(_fill.DOSizeDelta(new Vector2(_originalSize.x * 0.95f, _originalSize.y), 1.5f).SetEase(Ease.OutQuad));

        // 0.5초 대기
        seq.AppendInterval(0.5f);

        // 100%
        seq.Append(_fill.DOSizeDelta(_originalSize,0.1f).SetEase(Ease.Linear));

        seq.OnUpdate(() =>
        {
            // 현재 진행률 계산
            float percent = _fill.sizeDelta.x / _originalSize.x;
            // 퍼센트 정수 변환
            int value = Mathf.RoundToInt(percent * 100f);
            // 텍스트 갱신
            _lodingText.text = value + "%";
        });

        return seq.AsyncWaitForCompletion();
    }
}

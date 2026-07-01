using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerPriorityButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    private Toggle _toggle;
    public Toggle Toggle
    {
        get => _toggle;
        set => _toggle = value;
    }

    // 토글이 눌렸는지 확인할 변수
    private bool _wasOn;

    // 토글에 번호를 표시할 TMP
    [Header("해당 StickerToggle의 StickerToggleNumberText를 참조")]
    [SerializeField] private TextMeshProUGUI _stickerToggleNumberText;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        StickerStateSingleton.Instance.StickerPriorityButtonChanged += StickerPriorityIndexUpdate;
        _toggle.onValueChanged.AddListener(OnClickSelectSticker);
    }

    private void OnDisable()
    {
        StickerStateSingleton.Instance.StickerPriorityButtonChanged -= StickerPriorityIndexUpdate;
        _toggle.onValueChanged.RemoveListener(OnClickSelectSticker);
    }

    // IPointerDownHandler를 상속받는 함수로 클릭직전의 상태를 저장하기 위해 사용
    public void OnPointerDown(PointerEventData eventData)
    {
        _wasOn = _toggle.isOn;
    }

    // IPointerClickHandler를 상속받는 함수로 클릭직전에 토글이 켜져있으면 끄기 위해 사용
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_wasOn)
        {
            _toggle.isOn = false;
        }
    }

    #region 토글버튼으로 스티커 선택 함수
    // 스티커 생선순 토글버튼에 구독할 스티커 선택 함수
    private void OnClickSelectSticker(bool isOn)
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        if (StickerStateSingleton.Instance == null) return;

        if (!StickerStateSingleton.Instance.ToggleToSticker.TryGetValue(_toggle, out GameObject sticker)) return;

        if (isOn)
        {
            // 토글 리스트를 순회하며 현재 눌린 토글이 아닌 버튼은 전부 끄는 코드
            foreach (Toggle toggle in StickerStateSingleton.Instance.ToggleList)
            {
                if (toggle != _toggle)
                {
                    toggle.isOn = false;
                }
            }

            // 해당 토글의 스티커를 선택 (TouchInteractor 스크립트가 붙어있어야 합니다.)
            TouchInputHandler.Instance.CallObjectSelectedForToggle(sticker.GetComponent<TouchInteractor>());
        }
        // 토글이 꺼지면 선택해제 및 삭제버튼 숨기기
        else
        {
            StickerStateSingleton.Instance.StickerToDelButton
                [StickerStateSingleton.Instance.ToggleToSticker[_toggle]].gameObject.SetActive(false);
            TouchInputHandler.Instance.CallOPScalerTargetNull();
        }
    }
    #endregion

    #region 토글버튼 인덱스 갱신(TMP)
    // 스티커 생선순 토글버튼 인덱스 갱신
    private void StickerPriorityIndexUpdate()
    {
        if (StickerStateSingleton.Instance == null) return;

        int index = StickerStateSingleton.Instance.ToggleList.IndexOf(_toggle);
        _stickerToggleNumberText.text = $"{index + 1}";
    }
    #endregion
}

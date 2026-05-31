using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StickerPriorityButton : MonoBehaviour
{
    private Toggle _toggle;

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

    // 스티커 생선순 토글버튼에 구독할 스티커 선택 함수
    private void OnClickSelectSticker(bool isOn)
    {
        if (StickerStateSingleton.Instance == null) return;
        if (!isOn) return;

        if (StickerStateSingleton.Instance.ToggleToSticker.TryGetValue(_toggle, out GameObject sticker))
        {
            ObjectPinchScaler.Instance.OnSelectForToggle(sticker.GetComponent<TouchInteractor>());
        }
    }

    // 스티커 생선순 토글버튼 인덱스 갱신
    private void StickerPriorityIndexUpdate()
    {
        if (StickerStateSingleton.Instance == null) return;

        int index = StickerStateSingleton.Instance.ToggleList.IndexOf(_toggle);

        _stickerToggleNumberText.text = $"{index + 1}";
    }
}

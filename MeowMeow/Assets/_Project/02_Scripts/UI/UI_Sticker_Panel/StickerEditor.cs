using UnityEngine;
using UnityEngine.UI;

public class StickerEditor : MonoBehaviour
{
    private Button _button;
    private Image _image;

    // StickerDB(SO)파일과 매칭시킬 자신의 인덱스
    [SerializeField] private int _myIndex;
    public int MyIndex
    {
        get => _myIndex;
        set => _myIndex = value;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _image.sprite = StickerStateSingleton.Instance.StickerDB.GetSprite(_myIndex);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickSetSticker);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickSetSticker);
    }

    // 스티커 생성버튼에 구독할 함수(스티커 생성)
    private void OnClickSetSticker()
    {
        if (StickerStateSingleton.Instance.IsTouching == true) return;
        if (!StickerStateSingleton.Instance.TryClickSticker())return;

        // 효과음
        SoundManager.Instance.Invoke(AudioType.Popup3);

        StickerStateSingleton.Instance.SetSticker(_myIndex);
    }
}

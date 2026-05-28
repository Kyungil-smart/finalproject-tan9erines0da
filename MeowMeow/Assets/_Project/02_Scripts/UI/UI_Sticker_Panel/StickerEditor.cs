using UnityEngine;
using UnityEngine.UI;
using static StickerStateSingleton;

public class StickerEditor : MonoBehaviour
{
    private Button _button;
    private Image _image;

    [Header("해당 스티커 프리펩 참조")]
    [SerializeField] private Image _stickerImage;

    [Header("프리뷰 이미지를 참조")]
    [SerializeField] private Image _targetImage;

    [Header("Sticker_Priority_Scroll View의 자식 Content를 참조")]
    [SerializeField] private RectTransform _content;

    [Header("Sticker_Priority_Button 프리펩을 참조")]
    [SerializeField] private Button _priorityButton;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickSetSticker);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickSetSticker);
    }

    private void OnClickSetSticker()
    {
        if (StickerStateSingleton.Instance.CurrentCount >= StickerStateSingleton.Instance.MaxStickerCount) return;

        StickerPair pair = new StickerPair();

        GameObject obj = Instantiate(_stickerImage.gameObject, _targetImage.transform);

        Image image = obj.GetComponent<Image>();
        image.sprite = _image.sprite;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;

        pair.sticker = obj;
   

        GameObject priorityButton = Instantiate(_priorityButton.gameObject, _content.transform);
        RectTransform priorityButtonRect = priorityButton.GetComponent<RectTransform>();

        pair.button = priorityButton;

        StickerStateSingleton.Instance.stickers.Add(pair);

        StickerStateSingleton.Instance.CurrentCount++;
        StickerStateSingleton.Instance.StickerCountUpload();
    }
}

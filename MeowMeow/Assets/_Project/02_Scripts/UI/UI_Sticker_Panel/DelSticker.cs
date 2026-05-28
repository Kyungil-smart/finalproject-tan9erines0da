using UnityEngine;
using UnityEngine.UI;

public class DelSticker : MonoBehaviour
{
    [Header("스티커 프리펩을 참조")]
    [SerializeField] private GameObject _stickerObject;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickDelSuicker);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickDelSuicker);
    }

    private void OnClickDelSuicker()
    {
        if (StickerStateSingleton.Instance == null) return;
        StickerStateSingleton.Instance.CurrentCount--;

        StickerStateSingleton.StickerPair pair = StickerStateSingleton.Instance.stickers[0];
        Destroy(pair.sticker);
        Destroy(pair.button);
        StickerStateSingleton.Instance.stickers.RemoveAt(0);

        StickerStateSingleton.Instance.StickerCountUpload();
    }
}

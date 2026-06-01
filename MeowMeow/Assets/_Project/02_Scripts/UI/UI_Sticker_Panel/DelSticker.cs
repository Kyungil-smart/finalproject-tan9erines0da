using System.Collections.Generic;
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
        _button.onClick.AddListener(OnClickDelSticker);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickDelSticker);
    }

    private void OnClickDelSticker()
    {
        if (StickerStateSingleton.Instance == null) return;

        List<StickerStateSingleton.StickerPair> list = StickerStateSingleton.Instance.stickers;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].sticker == _stickerObject)
            {
                Destroy(list[i].sticker);
                Destroy(list[i].button);

                list.RemoveAt(i);

                StickerStateSingleton.Instance.CurrentCount--;
                StickerStateSingleton.Instance.StickerCountUpload();
                return;
            }
        }
    }
}

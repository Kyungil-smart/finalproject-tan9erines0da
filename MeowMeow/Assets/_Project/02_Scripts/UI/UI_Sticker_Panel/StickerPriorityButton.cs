using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerPriorityButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickSelectSticker);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickSelectSticker);
    }

    private void OnClickSelectSticker()
    {
        if (StickerStateSingleton.Instance == null) return;

        List<StickerStateSingleton.StickerPair> list = StickerStateSingleton.Instance.stickers;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].button == gameObject)
            {
                ObjectPinchScaler.Instance.OnSelect(list[i].sticker.GetComponent<TouchInteractor>());
                return;
            }
        }
    }
}

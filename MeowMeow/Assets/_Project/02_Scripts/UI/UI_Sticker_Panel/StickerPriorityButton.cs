using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerPriorityButton : MonoBehaviour
{
    private Button _button;
    private StickerStateSingleton.StickerPair _pair;

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

     public void SetPair(StickerStateSingleton.StickerPair pair)
    {
        _pair = pair;
    }

    private void OnClickSelectSticker()
    {
        if (_pair == null) return;
    }
}

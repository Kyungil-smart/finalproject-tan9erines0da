using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerEditor : MonoBehaviour
{
    private Button _button;
    private Image _image;

    [Header("해당 스티커 오브젝트 프리펩 참조")]
    [SerializeField] private GameObject _gameObject;

    [SerializeField] private Image _targetImage;

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
        GameObject obj = Instantiate(_image.gameObject, _targetImage.transform);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(190f, 150f); // 원하는 크기
        rect.anchoredPosition = new Vector2(500f, 700f);
    }
}

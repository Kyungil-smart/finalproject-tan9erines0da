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

    [Header("프리뷰 이미지를 참조")]
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
        GameObject obj = Instantiate(_gameObject, _targetImage.transform);

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = _image.sprite;

        obj.transform.localPosition = new Vector3(500f, 700f, 0f);
        obj.transform.localScale = Vector3.one;
    }
}

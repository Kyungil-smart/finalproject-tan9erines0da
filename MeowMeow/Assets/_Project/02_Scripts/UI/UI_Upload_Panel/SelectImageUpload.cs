using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SelectImageUpload : MonoBehaviour
{
    private GetImageList _getImageList;

    private Button _button;
    private Image _Image;

    private void Awake()
    {
        _getImageList = GetComponentInParent<GetImageList>();
        _button = GetComponent<Button>();
        _Image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickImage);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickImage);
    }

    private void OnClickImage()
    {
        _getImageList.UpLoadImage(_Image.sprite);
    }
}

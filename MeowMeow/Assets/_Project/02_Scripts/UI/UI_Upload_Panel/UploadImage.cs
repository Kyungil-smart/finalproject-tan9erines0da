using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UploadImage : MonoBehaviour
{
    private Image _image;

    public Image Image
    {
        get => _image;
        set => _image = value;
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
}

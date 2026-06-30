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

    [Header("버튼 자신의 자식 Post_Icon_Image를 참조")]
    [SerializeField] private GameObject _postIconImage;

    private void Awake()
    {
        _getImageList = GetComponentInParent<GetImageList>();
        _button = GetComponent<Button>();
        _Image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickImage);

        if (_getImageList.UploadImage.Image.sprite == null)
        {
            _postIconImage.SetActive(false);
        }
    } 

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickImage);
    }

    private void OnClickImage()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        _getImageList.OnPostIconImage(this);
        _getImageList.UpLoadImage(_Image.sprite);
        _getImageList.IsSelectImage = true;
    }

    /// <summary>
    /// OnPostIconImage()함수와 연동하기 위한 함수 `이미 게시한 게시물 아이콘`을 켜고/끄는 역활
    /// </summary>
    /// <param name="selected">true/false로 `이미 게시한 게시물 아이콘` 켜고/끄는 인자</param>
    public void SetSelected(bool selected)
    {
        _postIconImage.SetActive(selected);
    }
}

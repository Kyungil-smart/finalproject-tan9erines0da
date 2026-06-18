using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetImageList : MonoBehaviour
{
    [Header("프리뷰할 이미지의 오브젝트를 참조")]
    public UploadImage UploadImage;

    // 이미 게시한 게시물 아이콘 SetActive관련 함수 호출을 위한 변수
    private SelectImageUpload _currentSelected;

    // 이미지 선택 여부를 판별
    public bool IsSelectImage = false;

    /// <summary>
    /// 냥스타그램에 업로드할 이미지 프리뷰 부분에 이미지를 넣는 함수
    /// </summary>
    /// <param name="sprite">프리뷰에 표시할 이미지 스프라이트</param>
    public void UpLoadImage(Sprite sprite)
    {
        UploadImage.Image.sprite = sprite;
    }

    /// <summary>
    /// 업로드할 이미지를 선택시 `이미 게시한 게시물 아이콘`를 SetActive 하는 함수(자기 자신이 아니면 false)
    /// </summary>
    /// <param name="select">이 함수를 호출할 SelectImageUpload 자신을 인자로 넣습니다.</param>
    public void OnPostIconImage(SelectImageUpload select)
    {
        if (_currentSelected != null)
        {
            _currentSelected.SetSelected(false);
        }

        _currentSelected = select;
        _currentSelected.SetSelected(true);
    }
}

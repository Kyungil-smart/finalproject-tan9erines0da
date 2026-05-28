using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetImageList : MonoBehaviour
{
    [Header("프리뷰할 이미지의 오브젝트를 참조")]
    public UploadImage UploadImage;

    /// <summary>
    /// 냥스타그램에 업로드할 이미지 프리뷰 부분에 이미지를 넣는 함수
    /// </summary>
    /// <param name="sprite">프리뷰에 표시할 이미지 스프라이트</param>
    public void UpLoadImage(Sprite sprite)
    {
        UploadImage.Image.sprite = sprite;
    }
}

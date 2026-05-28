using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SNSPostDTO
{
    // firestore 업로드에 필요한 uid나 피드의 고유 id, 인덱싱 필드 등

    // sns 콘텐츠 요소 필드
    public int ImageIndex;
    public UIShaderProperty ShaderProperty;
    public string Comment;
    public List<StickerTransformData> Stickers;
    public List<string> Hashtags;
}

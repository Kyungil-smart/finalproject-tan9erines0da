using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;

[System.Serializable]
[FirestoreData] //  파이어스토어 변환기 활성화
public struct SNSPostDTO
{
    // firestore 업로드에 필요한 uid나 피드의 고유 id, 인덱싱 필드 등
    [field: SerializeField][FirestoreProperty] public string WriterId {  get; set; }
    [field: SerializeField][FirestoreProperty] double RandomIndex { get; set; }

    // sns 콘텐츠 요소 필드
    [field: SerializeField][FirestoreProperty] public int ImageIndex {  get; set; }
    [field: SerializeField][FirestoreProperty] public UIShaderProperty ShaderProperty { get; set; }
    [field: SerializeField][FirestoreProperty] public string Comment { get; set; }
    [field: SerializeField][FirestoreProperty] public List<StickerTransformData> Stickers { get; set; }
    [field: SerializeField][FirestoreProperty] public List<string> Hashtags { get; set; }
}

using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct SNSPostDTO
{
    // firestore 업로드에 필요한 uid나 피드의 고유 id, 인덱싱 필드 등
    public string WriterId;
    public double RandomIndex;
    public long Timestamp;

    // sns 콘텐츠 요소 필드
    public int ImageIndex;
    public UIShaderProperty ShaderProperty;
    public string Comment;
    public List<StickerTransformData> Stickers;
    public List<string> Hashtags;
}

[FirestoreData] // ◀ 오직 파이어스토어 백엔드 통신 및 널 체크 전용
public class FirestoreSNSPostDoc
{
    [FirestoreProperty] public string WriterId { get; set; }
    [FirestoreProperty] public double RandomIndex { get; set; }
    [FirestoreProperty] public long Timestamp { get; set; }
    [FirestoreProperty] public int ImageIndex { get; set; }
    [FirestoreProperty] public UIShaderProperty ShaderProperty { get; set; }
    [FirestoreProperty] public string Comment { get; set; }
    [FirestoreProperty] public List<StickerTransformData> Stickers { get; set; }
    [FirestoreProperty] public List<string> Hashtags { get; set; }

    // 파이어스토어 ConvertTo<T> 복원을 위한 기본 생성자 필수
    public FirestoreSNSPostDoc() { }

    /// <summary>
    /// 구조체를 파이어스토어용 클래스로 압축 변환
    /// </summary>
    public FirestoreSNSPostDoc(SNSPostDTO structData)
    {
        WriterId = structData.WriterId;
        RandomIndex = structData.RandomIndex;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        ImageIndex = structData.ImageIndex;
        ShaderProperty = structData.ShaderProperty;
        Comment = structData.Comment;
        Stickers = structData.Stickers;
        Hashtags = structData.Hashtags;
    }

    /// <summary>
    /// 파이어스토어에서 내려온 클래스 데이터를 인게임 구조체 양식으로 복원
    /// </summary>
    public SNSPostDTO ToStruct()
    {
        return new SNSPostDTO
        {
            WriterId = this.WriterId,
            RandomIndex = this.RandomIndex,
            ImageIndex = this.ImageIndex,
            ShaderProperty = this.ShaderProperty,
            Comment = this.Comment,
            Stickers = this.Stickers,
            Hashtags = this.Hashtags
        };
    }
}

using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스티커 한개의 위치 크기 회전 정보를 담기 위한 구조체,
/// 배치가 완료된 후 다음 판넬로 넘어갈때 기록하는 용도입니다.
/// 배치를 재현할때 구조체의 값을 바탕으로 복원합니다.
/// </summary>
[System.Serializable]
[FirestoreData] //  파이어스토어 변환기 활성화
public struct StickerTransformData
{
    [field: SerializeField][FirestoreProperty] public int StickerId { get; set; }       // 스티커 이미지를 파악하기 위한 ID
    [field: SerializeField][FirestoreProperty] public double RelativeX { get; set; }     // 상대적인x좌표 (0.0 ~1.0)
    [field: SerializeField][FirestoreProperty] public double RelativeY { get; set; }     // 상대적인y좌표 (0.0 ~1.0)
    [field: SerializeField][FirestoreProperty] public double RelativeScale { get; set; } // 상대적인 크기 비율
    [field: SerializeField][FirestoreProperty] public double Rotation { get; set; }      // z축 회전 값
}
[System.Serializable]
[FirestoreData] //  파이어스토어 변환기 활성화
public struct UIShaderProperty
{
    [field: SerializeField][FirestoreProperty] public float Brightness  { get; set; }  // 밝기 (-1.0 ~1.0)
    [field: SerializeField][FirestoreProperty] public float Contrast    { get; set; }  // 대비 (0.0 ~2.0)
    [field: SerializeField][FirestoreProperty] public float Saturation  { get; set; }  // 채도 (0.0 ~2.0)
    [field: SerializeField][FirestoreProperty] public float Temperature { get; set; }  // 온도 (0.0 ~1.0)
}

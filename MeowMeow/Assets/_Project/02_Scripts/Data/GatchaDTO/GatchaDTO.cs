using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

[System.Serializable]
[FirestoreData]
public struct GatchaDTO
{
    // FirestoreProperty 속성은 프로퍼티(Property)에 적용하는 것이 좋습니다.

    [field: SerializeField][FirestoreProperty]
    public string UID { get; set; }

    [field: SerializeField][FirestoreProperty]
    public int OwnedTicketCount { get; set; } // 보유한 뽑기권

    [field: SerializeField][FirestoreProperty]
    public int TodayAttendanceTicketCount { get; set; } // 오늘 획득한 출석 뽑기권

    [field: SerializeField][FirestoreProperty]
    public int TodayQuestTicketCount { get; set; } // 오늘 획득한 퀘스트 뽑기권

    [field: SerializeField][FirestoreProperty]
    public int TotalGatchaCount { get; set; } // 누적 뽑기 횟수

    [field: SerializeField][FirestoreProperty]
    public List<int> ItemList { get; set; } // 42개 상품 리스트 (뽑기판 배치 순서)

    [field: SerializeField][FirestoreProperty]
    public List<int> OpenedIndices { get; set; } // 뽑기권을 써서 열어둔 인덱스

    [field: SerializeField][FirestoreProperty]
    public bool IsResetPerformed { get; set; } // 초기화 시행 여부
}

using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensionshuffle
{
    public static async Task<List<T>> GetRandomSixData<T>(this FirestoreRequestContext context)
    {
        List<T> resultList = new List<T>();

        // 1. 컬렉션 참조를 가져옵니다.
        var collectionRef = context.TargetStore.DB.Collection("SNSPostGroups");

        // 2.  클라이언트에서 무작위 기준점(0.0 ~ 1.0)을 하나 생성합니다.
        double randomTarget = UnityEngine.Random.value;

        // 3. 서버에 요청: "RandomId 필드 값이 방금 만든 기준점보다 큰 문서 중 상위 6개만 줘!"
        //  중요: 이렇게 하면 10만 개가 있어도 서버는 딱 6개만 찾아서 보내줍니다. (비용 극적인 절감)
        var query = collectionRef.WhereGreaterThanOrEqualTo("RandomIndex", randomTarget)
                                 .Limit(6);

        var snapshot = await query.GetSnapshotAsync();

        foreach (var document in snapshot)
        {
            if (document.Exists)
            {
                resultList.Add(document.ConvertTo<T>());
            }
        }

        // 4.  안전장치 (예외 케이스 처리)
        // 만약 무작위 기준점이 너무 높게 잡혔거나(예: 0.99) 전체 문서 개수 자체가 적어서 6개를 못 채웠다면?
        if (resultList.Count < 6)
        {
            int neededCount = 6 - resultList.Count;

            // 반대 방향(기준점보다 작은 쪽)에서 모자란 개수만큼 쿼리해서 채워줍니다.
            var fallbackQuery = collectionRef.WhereLessThan("RandomIndex", randomTarget)
                                             .Limit(neededCount);

            var fallbackSnapshot = await fallbackQuery.GetSnapshotAsync();
            foreach (var document in fallbackSnapshot)
            {
                if (document.Exists)
                {
                    resultList.Add(document.ConvertTo<T>());
                }
            }
        }

        // 5. 가져온 데이터(최대 6개)의 순서를 한 번 더 가볍게 섞어줍니다.
        // 데이터가 최대 6개뿐이므로 피셔-예이츠를 돌려도 가비지나 성능 저하가 전혀 없습니다.
        ShuffleList(resultList);

        // 확인용 로그
        foreach (T item in resultList)
        {
            Debug.Log($"[Random Data] {item}");
        }

        return resultList;
    }

    // 내부에서만 쓸 피셔-예이츠 셔플 가벼운 버전
    private static void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public static CollectionReference GetCollection(this BaseFireStore cxt)
    {
        return cxt.currentCollection;
    }

    /// <summary>
    /// 특정 유저의 UID를 기반으로 전체 작성 포스트를 
    /// 최신 시간순(Timestamp 내림차순)으로 정렬하여 다운로드합니다.
    /// </summary>
    public static async Task<List<T>> SyncMyHistoryData<T>(
    this FirestoreRequestContext context, string uid)
    {
        List<T> resultList = new List<T>();

        // 현재 백엔드 인증 객체가 정상적으로 살아있는지 확인합니다.
        if (BackendManager.Auth == null ||
            BackendManager.Auth.CurrentUser == null)
        {
            Debug.LogError("[Security] 인증 세션이 존재하지 않아 " +
                "데이터 동기화를 차단합니다.");
            return resultList;
        }

        // 실제 백엔드에 로그인된 진짜 UID를 가져옵니다.
        string sessionUid = BackendManager.Auth.CurrentUser.UserId;

        // 매개변수로 요청된 uid와 실제 세션의 uid가 다르다면,
        // 로직 꼬임이므로 서버 쿼리를 아예 차단합니다.
        if (sessionUid != uid)
        {
            Debug.LogError($"[Security] 잘못된 권한 접근입니다. " +
                $"실제세션: {sessionUid}, 요청UID: {uid}");
            return resultList; // 파이어스토어 읽기 비용 발생 전에 컷!
        }

        var collectionRef = context.TargetStore.GetCollection();

        try
        {
            // 서버단 필터링 쿼리
            Query query = collectionRef
                .WhereEqualTo("WriterId", uid)
                .OrderByDescending("Timestamp");

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (var document in snapshot)
            {
                if (document.Exists)
                {
                    if (document.TryGetValue("WriterId", out string parsed)
                        && parsed == sessionUid)
                    {
                        resultList.Add(document.ConvertTo<T>());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Sync Extension Error] " +
                $"히스토리 복원 실패: {ex.Message}");
        }

        return resultList;
    }
}

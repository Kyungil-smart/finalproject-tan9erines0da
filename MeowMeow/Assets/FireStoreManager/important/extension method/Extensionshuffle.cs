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
        var collectionRef = context.TargetStore.GetCollection();

        // 2.  클라이언트에서 무작위 기준점(0.0 ~ 1.0)을 하나 생성합니다.
        double randomTarget = UnityEngine.Random.value;

        // 3. 서버에 요청: "RandomId 필드 값이 방금 만든 기준점보다 큰 문서 중 상위 6개만 줘!"
        //  중요: 이렇게 하면 10만 개가 있어도 서버는 딱 6개만 찾아서 보내줍니다. (비용 극적인 절감)
        var query = collectionRef.WhereGreaterThanOrEqualTo("RandomId", randomTarget)
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
            var fallbackQuery = collectionRef.WhereLessThan("RandomId", randomTarget)
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
}
using System;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PostFireStoreSO",
    menuName = "Firestore/PostFireStoreSO")]
public class PostFireStoreSO : BaseFireStore
{
    /// <summary>
    /// 문서를 서버에 올리는 함수입니다.
    /// 예외처리 추가
    /// </summary>
    public override async Task SetDataAsync(object data)
    {
        if (data is FirestoreSNSPostDoc doc)
        {
            await currentRef.SetAsync(doc);
            await currentCollection.AddAsync(doc);
            Debug.Log($"[Firestore] {currentRef.Path}에 문서 업로드 성공");
        }
        else
        {
            Debug.LogError("[Firestore] 데이터가 FirestoreSNSPostDoc 양식이 아닙니다.");
        }
    }

    /// <summary>
    /// 단일 데이터 통째로 받아오는 함수입니다
    /// </summary>
    public override async Task<T> GetSnapshotAsync<T>()
    {
        DocumentSnapshot snapshot = await currentRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }

        Debug.LogWarning($"[Firestore] {currentRef.Path} 문서를 찾을 수 없습니다.");
        return default(T);
    }
}

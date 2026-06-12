using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using UnityEngine;
public class FireStoreNullSO : BaseFireStore
{
 
    public void PrintLog()
    {
        Debug.LogError("FireStoreManager 매니저 딕셔너리에 해당 enum 데이터 없습니다.");
        Debug.LogError("\"매니저 클래스를 확인 및 SO를 확인 바람\");"); 
       
    }
    public override   Task DeleteDataAsync()
    {
        PrintLog();
        //  "아무 일도 안 하고 이미 성공적으로 끝난 작업"을 던져줍니다.
        return Task.CompletedTask;
    }

    public override Task SetDataAsync(object data)
    {
        PrintLog();
        //   호출한 곳에서 await를 만나도 터지지 않고 부드럽게 넘어갑니다.
        return Task.CompletedTask;
    }

    public override Task UpdateDataAsync<T>(Dictionary<string, object> data,bool flag =false)
    {
        PrintLog();
        return Task.CompletedTask;
    }

    // 2. 반환값(결과물)이 있는 비동기 작업 (Task<T>)
    

    public override Task<T> GetSnapshotAsync<T>()
    {
        PrintLog();
        //   제네릭 타입 T의 기본값(클래스면 null, 구조체면 비어있는 객체)을 채워 리턴합니다.
        return Task.FromResult<T>(default);
    }
}

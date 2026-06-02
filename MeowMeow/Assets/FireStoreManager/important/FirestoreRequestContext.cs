using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirestoreRequestContext
{
    private readonly BaseFireStore _targetStore;
    public BaseFireStore TargetStore => _targetStore;
     
    public FirestoreRequestContext(BaseFireStore targetStore)
    {
        _targetStore = targetStore;
    }

    // 진짜 비동기(Task)를 반환하여 await가 가능하게 만듭니다.
    public async Task SetAsync(object data)
    {
        // BaseFireStore의 SetData도 내부적으로 Task를 반환하는 비동기 함수여야 합니다.
        // 예: public abstract Task SetData(object data);
        await _targetStore.SetDataAsync(data);
    }

    public async Task<T> GetAsync<T>()
    {
        return await _targetStore.GetSnapshotAsync<T>();
    }

    public async Task UpdateAsync<T>(Dictionary<string, object> data,bool falg=false)
    {
        await _targetStore.UpdateDataAsync<T>(data, falg);
    }

    public async Task DeleteAsync( )
    {
      await _targetStore.DeleteDataAsync();
    }

    
}
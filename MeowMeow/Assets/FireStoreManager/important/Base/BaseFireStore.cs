using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; // Task를 사용하기 위해 추가
using UnityEngine;



public  class BaseFireStore : ScriptableObject
{
    
    public List<string> m_collections = new List<string>();
    public List<string> m_documents = new List<string>();

    public DocumentReference currentRef;
    public CollectionReference currentCollection;
    // 매니저에서 딕셔너리 매핑용으로 쓸 식별자
   [SerializeField] private DataType m_EnumType;
    public DataType EnumType => m_EnumType;

   
    protected FirebaseFirestore db;
    public FirebaseFirestore  DB => db;
    protected static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> propertyCache =
    new Dictionary<Type, Dictionary<string, PropertyInfo>>();

    public virtual void InitDataBase(FirebaseFirestore database,string YouserID=null)
    {
        db = database;
        //m_documents  = Auth.uid 넣기
        //첫번째 경로는 컬렉션 Users  / 문서 :userUID 넣을 예정
        if(YouserID != null)
        {
            currentRef = db.Collection("Users").Document(YouserID);
            currentCollection = db.Collection("Users");
        }
        else
        {
            currentRef = db.Collection(m_collections[0]).Document(m_documents[0]);
            currentCollection = db.Collection(m_collections[0]);
        }
      
      
        for (int i = 1; i < m_documents.Count; i++)
        {
            currentCollection = currentRef.Collection(m_collections[i]);
            // 기존 주소 뒤에 다음 컬렉션과 다음 문서를 줄줄이 소시지처럼 엮습니다.
            currentRef = currentCollection.Document(m_documents[i]);
        }

       
    }
    public virtual async Task SetDataAsync(object data) => await currentRef.SetAsync(data);

    public virtual async Task<T> GetSnapshotAsync<T>()
    {
        DocumentSnapshot snapshot = await currentRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }
        Debug.LogWarning("문서를 찾을 수 없습니다.");
        return default(T);
    }
    //public virtual async Task UpdateDataAsync<T>(Dictionary<string, object> data, bool flag = false)
    //{
    //    Type m_TargetDataType = typeof(T);

    //    // DTO의 프로퍼티들을 검색하기 쉽게 사전에 딕셔너리 형태로 만듭니다.
    //    var propertyDict = m_TargetDataType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                                       .ToDictionary(p => p.Name);

    //    // 1️⃣ [검증 단계] 내가 변경 요청한 data 딕셔너리의 키들을 검사합니다.
    //    foreach (var kvp in data)
    //    {
    //        if (propertyDict.TryGetValue(kvp.Key, out PropertyInfo property))
    //        {
    //            if (flag == false)
    //            {
    //                Debug.Log("같음");
    //                var Datatype = property.PropertyType;
    //                object dictionaryValue = data[property.Name];
    //                var getTypvalue = dictionaryValue.GetType();

    //                Debug.Log($"Datatype: {Datatype}");
    //                Debug.Log($"getTypvalue: {getTypvalue}");
    //                bool sameType = Datatype == getTypvalue;
    //                if (sameType)
    //                {
    //                    Debug.Log($"데이터 타입 같음");
    //                    await currentRef.UpdateAsync(data);
    //                }
    //            }
    //        }

    //    }

    //    // 2️⃣ [실행 단계] 검증이 무사히 끝나거나, 처음부터 flag가 true였다면 
    //    // 루프 바깥에서 딱 "한 번만" 서버에 업데이트를 요청합니다.
    //    if (flag == true)
    //    {
    //        await currentRef.UpdateAsync(data);
    //        Debug.Log($"✅ {currentRef.Path} 문서 업데이트 완료!");
    //    }
    //}
    public virtual async Task UpdateDataAsync<T>(
     Dictionary<string, object> data,
     bool skipValidation = false)
    {
        if (data == null || data.Count == 0)
        {
            Debug.LogWarning("업데이트할 데이터가 없습니다.");
            return;
        }

        if (!skipValidation)
        {
            Type targetType = typeof(T);
            var propertyDict = GetPropertyCache(targetType);

            foreach (var kvp in data)
            {
                if (!propertyDict.TryGetValue(kvp.Key, out PropertyInfo property))
                {
                    Debug.LogError($"[Update Error] {targetType.Name} 구조체에 '{kvp.Key}' 필드가 존재하지 않습니다.");
                    return;
                }

                Type expectedType = property.PropertyType;
                Type inputType = kvp.Value?.GetType();

                if (kvp.Value != null && expectedType != inputType)
                {
                    Debug.LogError($"[Update Error] {targetType.Name}.{kvp.Key} 타입 불일치! 기대: {expectedType}, 입력: {inputType}");
                    return;
                }
            }
        }
        try
        {
            await currentRef.UpdateAsync(data);
            Debug.Log($" {currentRef.Path} 문서 업데이트 완료!");
        }
        catch (Exception ex)
        {
            Debug.LogError($" {currentRef.Path} 문서 업데이트 실패: {ex.Message}");
        }
    }

    public virtual async Task DeleteDataAsync()
    {
        try
        {
            // 콜백 대신 동기 코드처럼 슥 읽히도록 await로 대기합니다.
            await currentRef.DeleteAsync();
            Debug.Log($"  {currentRef.Path} 경로의 문서 삭제 완료!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"  {currentRef.Path} 경로의 문서 삭제 실패: {ex.Message}");
        }

    }
    private Dictionary<string, PropertyInfo> GetPropertyCache(Type targetType)
    {
        if (propertyCache.TryGetValue(targetType, out var cachedProperties))
        {
            return cachedProperties;
        }

        var propertyDict = targetType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);

        propertyCache[targetType] = propertyDict;

        return propertyDict;
    }
}

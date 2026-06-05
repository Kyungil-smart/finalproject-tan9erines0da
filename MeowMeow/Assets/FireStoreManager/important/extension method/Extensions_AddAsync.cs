using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions_AddAsync 
{
    public static async Task AddAsync(this FirestoreRequestContext context, object data)
    {
      var collectionRef = context.TargetStore.GetCollection();
      await collectionRef.AddAsync(data);
    }

     
}
/*
 *  private  void BindClass()
    {
        foreach (BaseFireStore item in m_Data)
        {
            string UID = null;

            try
            {
                // 1. 싱글톤 인스턴스 자체가 없는지 먼저 확인
                // 2. Auth 프로퍼티를 안전하게 호출할 수 있는 상태인지 검사 (예: IsInitialized 같은 플래그가 있다면 조건에 추가)
                if (BackendManager.Instance != null && BackendManager.Auth != null)
                {
                    if (BackendManager.Auth.CurrentUser != null)
                    {
                        UID = BackendManager.Auth.CurrentUser.UserId;
                    }
                }
            }
            catch (System.NullReferenceException)
            {
                // Auth 프로퍼티 내부에서 터지는 널 익셉션까지 방어막을 쳐줍니다.
                UID = null;
            }

            // UID 할당 여부에 따라 안전하게 분기 처리
            if (string.IsNullOrEmpty(UID))
            {
                item.InitDataBase(m_db);
            }
            else
            {
                item.InitDataBase(m_db, UID);
            }

            Debug.Log("BindClass");
        }

    }
*/

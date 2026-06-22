using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum DataType
{
    None,
    Test,
    Posts,
    CurrencyData,
    GatchaData,


}
public class FireStoreManager : MonoBehaviour
{
    public List<SNSPostDTO> TestList = new List<SNSPostDTO>();
    private static FirebaseFirestore m_db;
    public static FirebaseFirestore db => m_db;
    public static FireStoreManager Instance { get; private set; }
    [SerializeField] private List<BaseFireStore> m_Data;
    private static Dictionary<DataType, BaseFireStore> m_DataDictionary;
    private static FireStoreNullSO m_NullSO;

    private void Awake()
    {
        AutoSetting();
        InitSingleton();

        // InitFirebaseAsync();
    }
    public void AutoSetting()
    {
        m_Data.Clear();
        var datas = Resources.LoadAll<BaseFireStore>("SO");
        m_Data.AddRange(datas);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    private void InitSingleton()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
    // 초기화는 백엔드 매니져에서 진행 
    private void InitFirebaseAsync()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task =>
        {
            DependencyStatus status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공");
                m_NullSO = ScriptableObject.CreateInstance<FireStoreNullSO>();
                m_db = FirebaseFirestore.DefaultInstance;
                BindClass();
                InitDictionary();
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {status}");
            }
        });
    }
    public void InitF_M()
    {
        m_NullSO = ScriptableObject.CreateInstance<FireStoreNullSO>();
        m_db = FirebaseFirestore.DefaultInstance;
        BindClass();
        InitDictionary();
    }
    private void BindClass()
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

    private void InitDictionary()
    {
        m_DataDictionary = new Dictionary<DataType, BaseFireStore>();
        m_DataDictionary = m_Data.ToDictionary(x => x.EnumType, x => x);
    }

    public static FirestoreRequestContext DocumentType(DataType type)
    {
        if (!m_DataDictionary.ContainsKey(type))
        {
            return new FirestoreRequestContext(m_NullSO);
        }
        else
        {
            // 해당 데이터를 처리할 컨텍스트를 새로 생성해서 반환 (동시성 문제 해결)
            return new FirestoreRequestContext(m_DataDictionary[type]);
        }
    }
 
}

 
#if UNITY_EDITOR
[CustomEditor(typeof(FireStoreManager))]
public class FireStoreManagerrEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기존 인스펙터의 기본 필드들(sheetUrl 등)을 그대로 먼저 그려줍니다.
        DrawDefaultInspector();

        // 타겟 스크립트를 가져옵니다.
        FireStoreManager generator = (FireStoreManager)target;

        // 위아래 여백을 살짝 줍니다.
        GUILayout.Space(15);

        // 💡 버튼 만들기 (버튼이 클릭되면 true를 반환합니다)
        if (GUILayout.Button(" 데이터 수동 셋팅", GUILayout.Height(40)))
        {
            // 버튼을 누르면 실행될 로직
            generator.AutoSetting();
        }
    }
}
#endif

using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

/// <summary>
/// 파이어베이스(Firebase) 코어 및 인증(Auth) 시스템의 
/// 초기화와 글로벌 접근을 관리하는 백엔드 매니저 클래스입니다.
/// </summary>
public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp _app;
    public static FirebaseApp App => Instance._app;

    private FirebaseAuth _auth;
    public static FirebaseAuth Auth => Instance._auth;

    // Firebase 초기화 완료 + 성공 여부를 await 가능하도록 노출  
    // 자동로그인 흐름 시작 전에 'await BackendManager.ReadyTask'  
    private static readonly TaskCompletionSource<bool> _readyTcs = new();
    public static Task<bool> ReadyTask => _readyTcs.Task;

    private void Awake()
    {
        SetSingleton();
        CheckAndFixDependencies();
    }

    //firebase 초기화 함수자동 로그인 반영
    private void CheckAndFixDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task =>
        {
            bool isAvailable = task.Result == DependencyStatus.Available;

            if (isAvailable)
            {
                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;

                FireStoreManager.Instance.InitF_M();
            }
            else
            {
                Debug.LogError($"BackendManager : {task.Result}");
                _app = null;
                _auth = null;
            }

            _readyTcs.TrySetResult(isAvailable);
        });
    }

    private void SetSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

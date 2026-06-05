using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private int _sceneIndex;

    public void LoadScene()
    {
        SceneManager.LoadScene(_sceneName);
    }


    /// <summary>
    /// 인증 조건 충족 시에만 다음 단계로 인계하는 보안 관문
    /// </summary>
    public void LoadSceneIfLoggedIn()
    {
        // 1. Firebase 백엔드 인증 계정 상태 더블 체크
        bool isFirebaseAuthed = BackendManager.Auth != null &&
                                BackendManager.Auth.CurrentUser != null;

        // 3. 교집합 검증 방어벽 작동
        if (isFirebaseAuthed)
        {
            Debug.Log($"[Auth Success] 인증 확인 완료. {_sceneName} 씬으로 이동합니다.");
            SceneManager.LoadScene(_sceneIndex);
        }
        else
        {
            Debug.LogWarning("[Auth Denied] 로그인 상태가 올바르지 않아 씬 로드를 차단합니다.");
        }
    }
}

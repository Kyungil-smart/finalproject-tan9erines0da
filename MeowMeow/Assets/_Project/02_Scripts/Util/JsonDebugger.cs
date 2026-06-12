using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro; // 텍스트 컴포넌트용

public class MobileJsonDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _debugText;

    /// <summary>
    /// 디버그 버튼 클릭 시 로컬 파일을 찾아 화면에 텍스트로 띄웁니다.
    /// </summary>
    public void ReadJsonOnMobile()
    {
        string uid = BackendManager.Auth?.CurrentUser?.UserId
                     ?? "GuestUID";

        string path = Path.Combine(Application.persistentDataPath,
            $"MyPosts_{uid}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _debugText.text = $"[UID: {uid}]\n{json}";
            Debug.Log($"[모바일 JSON 스캔] {json}");
        }
        else
        {
            _debugText.text = $"[UID: {uid}]\n저장된 파일이 없습니다!";
        }
    }
}

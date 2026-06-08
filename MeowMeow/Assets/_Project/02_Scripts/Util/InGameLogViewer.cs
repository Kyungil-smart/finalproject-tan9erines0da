using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스

/// <summary>
/// 인게임 화면에서 이벤트 버스의 로그를 수신하여 TMP에 출력합니다.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class InGameLogViewer : MonoBehaviour
{
    private TextMeshProUGUI _logText;

    private void Start()
    {
        // TMP 컴포넌트를 캐싱하고 초기 안내 문구를 띄웁니다.
        _logText = GetComponent<TextMeshProUGUI>();
        _logText.text = "=== Mobile Log Viewer Ready ===\n";

        // 로그 수신 채널을 엽니다.
        if (SubscribeManager.instance != null)
        {
            SubscribeManager.instance.Subscribe<string>(
                SubscribeType.Log_Write, OnLogReceived);
        }
    }

    private void OnDestroy()
    {
        if (SubscribeManager.instance != null)
        {
            SubscribeManager.instance.Unsubscribe<string>(
                SubscribeType.Log_Write, OnLogReceived);
        }
    }

    /// <summary>
    /// Log_Write 이벤트가 발행될 때마다 자동으로 호출되는 훅
    /// </summary>
    private void OnLogReceived(string message)
    {
        if (_logText != null)
        {
            // 최신 로그가 아래로 계속 쌓이도록 줄바꿈(\n)과 함께 누적
            _logText.text += $"\n{message}";
        }
    }

    /// <summary>
    /// UI 버튼 등에 연결하여 화면의 로그를 비우는 함수
    /// </summary>
    public void ClearLogs()
    {
        if (_logText != null)
        {
            _logText.text = "=== Logs Cleared ===\n";
        }
    }
}

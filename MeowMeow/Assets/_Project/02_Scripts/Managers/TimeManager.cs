using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action OnOClock;

    private DateTime _verifiedServerTime;

    private bool _isTimerRunning = false;

    public string CurrentDate =>
        _verifiedServerTime.ToString("yyyy-MM-dd");

    public void SetVerifiedTime(DateTime serverUtcTime)
    {
        // 파이어스토어 메타데이터는 UTC 기준이므로 
        // 한국 시간(KST)으로 전환
        _verifiedServerTime = serverUtcTime.AddHours(9);

        Debug.Log($"[TimeManager] 서버 시간 주입 완료: " +
            $"{_verifiedServerTime:yyyy-MM-dd HH:mm:ss}");

        if (!_isTimerRunning)
        {
            StartCoroutine(CoMidnightScheduler());
        }
    }

    private void Awake()
    {
        SetSingleton();
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

    // 00시를 기다리는 코루틴
    private IEnumerator CoMidnightScheduler()
    {
        _isTimerRunning = true;

        while (true)
        {
            // 다음 자정(00:00:00)까지 남은 시간 계산
            float delaySeconds = GetSecondsUntilOClock(_verifiedServerTime);
            float elapsed = 0f;

            // 계산된 대기 시간만큼 루프
            while (elapsed < delaySeconds)
            {
                yield return null;

                float deltaTime = Time.unscaledDeltaTime;
                elapsed += deltaTime;

                _verifiedServerTime = _verifiedServerTime.AddSeconds(deltaTime);
            }

            // 자정에 도달하면 등록된 이벤트를 실행
            Debug.Log($"[TimeManager] 자정이 된 것으로 판단하여 이벤트를 실행합니다.");
            OnOClock?.Invoke();
            yield return null;
        }
    }

    // 초기화 시간까지 남은 시간을 계산하는 함수
    float GetSecondsUntilOClock(DateTime now)
    {
        float remaining = (float)(now.Date.AddDays(1) - now).TotalSeconds;
        return Mathf.Max(1f, remaining);
    }
}

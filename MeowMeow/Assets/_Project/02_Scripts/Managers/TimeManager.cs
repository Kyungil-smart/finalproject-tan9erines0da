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

    private CancellationTokenSource _cts;

    private string _currentDate;
    public string CurrentDate => _currentDate;

    public Task InitializationTask => _initTcs.Task;
    private TaskCompletionSource<bool> _initTcs =
        new TaskCompletionSource<bool>();

    private void Awake()
    {
        SetSingleton();
        _cts = new CancellationTokenSource();
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
    void Start()
    {
        StartSchedulingAsync(_cts.Token);
    }
    async void StartSchedulingAsync(CancellationToken token)
    {
        try
        {
            // 토큰을 통해 취소될때까지 반복
            while (!token.IsCancellationRequested)
            {
                // 현재 시간 가져오기
                DateTime severTime = await NtpTimeFetcher.GetNetworkTimeAsync(token);
                _currentDate = severTime.Date.ToString();

                // 다음 시간까지 남은 시간 계산
                float delay = GetSecondsUntilOClock(severTime);

                _initTcs.TrySetResult(true);

                // 다음 시간까지 대기
                await Task.Delay(TimeSpan.FromSeconds(delay), token);

                // 이벤트 인보크
                OnOClock?.Invoke();
            }
        }
        catch (OperationCanceledException) { }
    }

    // 초기화 시간까지 남은 시간을 계산하는 함수
    float GetSecondsUntilOClock(DateTime now)
    {
        float remaining = (float)(now.Date.AddDays(1) - now).TotalSeconds;
        return Mathf.Max(1f, remaining);
    }

    void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}

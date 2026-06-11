using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogExtensions
{
    public static void PublishLog(
        this UnityEngine.MonoBehaviour source,
        string message)
    {
        // source.name을 통해 발신 객체의 이름을 자동으로 포함
        string formattedMsg = $"[{source.name}] {message}";
        SubscribeManager.instance.Publish<string>(
            SubscribeType.Log_Write, formattedMsg);
    }
}

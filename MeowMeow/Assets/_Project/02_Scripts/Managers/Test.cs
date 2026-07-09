using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [ContextMenu("사이트 온")]
    public void OpenWebsite()
    {
        // 이동하고 싶은 주소를 입력하세요. 반드시 http:// 또는 https://를 포함해야 합니다.
        Application.OpenURL("https://www.google.com");
    }
}

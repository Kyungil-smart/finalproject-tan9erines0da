using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class UIAnimationEffect : MonoBehaviour
{
    /// <summary>
    /// UI가 화면에 나타나는 진입 애니메이션을 재생합니다.
    /// </summary>
    /// <param name="onComplete">연출이 완료된 후 실행할 콜백</param>
    public abstract void PlayIn(Action onComplete);

    /// <summary>
    /// UI가 화면에서 사라지는 퇴장 애니메이션을 재생합니다.
    /// </summary>
    /// <param name="onComplete">연출이 완료된 후 실행할 콜백</param>
    public abstract void PlayOut(Action onComplete);
}

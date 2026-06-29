using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayCloseTween : UIAnimationEffect
{
    [Header("새 화면이 위를 완전히 덮을 때까지 버틸 시간 (초)")]
    [SerializeField] private float _delaySeconds = 0.5f;
    public override void PlayIn(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public override void PlayOut(Action onComplete)
    {
        DOVirtual.DelayedCall(_delaySeconds, () =>
        {
            onComplete?.Invoke();
        });
    }
}

using System;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    // UI의 논리적 레이어 깊이를 정의하는 열거형
    public enum UIDepth { Depth1, Depth2 }

    [Header("UI 깊이 정체성 설정")]
    [SerializeField]
    private UIDepth _panelDepth = UIDepth.Depth1;

    [Header("가변 트윈 연출 부품 조립")]
    [SerializeField]
    private UIAnimationEffect _enterAnimationEffect;
    [SerializeField]
    private UIAnimationEffect _tabChangeAnimationEffect;

    /// <summary>
    /// 현재 이 화면 판넬이 속한 논리적 깊이 레이어를 반환합니다.
    /// </summary>
    public UIDepth PanelDepth => _panelDepth;

    /// <summary>
    /// 현재 화면 판넬이 트윈 연출을 재생 중인지 상태를 반환합니다.
    /// </summary>
    public bool IsTransitioning { get; private set; }

    /// <summary>
    /// 콘텐츠 진입 시점(처음 열릴 때)의 애니메이션을 사용하여 
    /// 화면 패널을 활성화합니다.
    /// </summary>
    public void OpenWithEnterAnimation(Action onComplete)
    {
        ExecuteOpenWithTween(_enterAnimationEffect, onComplete);
    }

    /// <summary>
    /// 수평 탭 전환 시점의 애니메이션을 사용하여 
    /// 화면 패널을 활성화합니다.
    /// </summary>
    public void OpenWithTabChangeAnimation(Action onComplete)
    {
        ExecuteOpenWithTween(_tabChangeAnimationEffect, onComplete);
    }

    /// <summary>
    /// 화면 패널을 비활성화하고 퇴장 애니메이션을 재생합니다.
    /// 기본적으로 진입 연출 부품의 PlayOut을 활용합니다.
    /// isExit가 true이면 연출 없이 종료합니다.
    /// </summary>
    public void Close(Action onComplete, bool isExit = false)
    {
        if (IsTransitioning) return;
        IsTransitioning = true;

        if (isExit || _enterAnimationEffect == null)
        {
            HandleCloseComplete(onComplete);
        }
        else
        {
            // 진입 연출 부품의 PlayOut(퇴장)을 호출
            _enterAnimationEffect.PlayOut(() =>
                HandleCloseComplete(onComplete));
        }
    }

    // 지정된 애니메이션 부품을 실행하고 오브젝트를 켜는 공통 내부 함수
    private void ExecuteOpenWithTween(
        UIAnimationEffect targetTween, Action onComplete)
    {
        if (IsTransitioning) return;

        IsTransitioning = true;
        gameObject.SetActive(true);

        if (targetTween != null)
        {
            targetTween.PlayIn(() =>
                HandleOpenComplete(onComplete));
        }
        else
        {
            HandleOpenComplete(onComplete);
        }
    }

    // 오픈 연출이 종료되었을 때 마커 상태를 정리하는 내부 함수
    private void HandleOpenComplete(Action onComplete)
    {
        IsTransitioning = false;
        onComplete?.Invoke();
    }

    // 클로즈 연출이 종료되었을 때 오브젝트를 끄고 상태를 정리하는 내부 함수
    private void HandleCloseComplete(Action onComplete)
    {
        gameObject.SetActive(false);
        IsTransitioning = false;
        onComplete?.Invoke();
    }
}

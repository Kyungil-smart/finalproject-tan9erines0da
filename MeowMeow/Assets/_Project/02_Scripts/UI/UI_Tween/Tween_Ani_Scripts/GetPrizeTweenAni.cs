using DG.Tweening;
using UnityEngine;

public class GetPrizeTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;

    private void Start()
    {
        PlayAnimation();
    }
    public void PlayAnimation()
    {
        if (targetRect == null)
        {
            Debug.LogWarning("Target Rect가 비어있습니다.");
            return;
        }

        gameObject.SetActive(true);

        targetRect.DOKill();

        // 시작 크기
        targetRect.sizeDelta = new Vector2(300f, 251f);

        Sequence seq = DOTween.Sequence();

        // 처음 느리고 갈수록 빨라짐
        seq.Append(
            targetRect.DOSizeDelta(
                new Vector2(622f, 521f),
                1f)
            .SetEase(Ease.InQuart));
    }
}

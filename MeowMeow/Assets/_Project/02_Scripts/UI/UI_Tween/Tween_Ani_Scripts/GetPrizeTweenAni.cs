using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class GetPrizeTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;

    public    Task PlayAnimation()
    {
        if (targetRect == null)
        {
            Debug.LogWarning("Target Rect가 비어있습니다.");
            return Task.CompletedTask;
        }

        gameObject.SetActive(true);

        targetRect.DOKill();

        // 시작 크기: 원본 크기(622x521) 기준으로 약 300x251 크기
        targetRect.localScale = new Vector3(0.482f, 0.482f, 1f);

        Sequence seq = DOTween.Sequence();

        // 처음 느리고 갈수록 빨라짐
        seq.Append(
             targetRect.DOScale(Vector3.one, 1f)
                 .SetEase(Ease.InQuart));

        return seq.AsyncWaitForCompletion();

    }
}

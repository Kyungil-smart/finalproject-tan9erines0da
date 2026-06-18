using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BoxCoverTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform leafRect;
    [SerializeField] private Graphic leafGraphic;
    
    public async Task PlayAnimation()
    {
        if (leafRect == null || leafGraphic == null)
        {
            Debug.LogWarning("Leaf가 비어있습니다.");
            return;
        }


        leafRect.DOKill();
        leafGraphic.DOKill();

        gameObject.SetActive(true);

        leafRect.anchoredPosition = Vector2.zero;
        Vector2 startPos = leafRect.anchoredPosition;

        Color color = leafGraphic.color;
        color.a = 1f;
        leafGraphic.color = color;

        Sequence seq = DOTween.Sequence();

        // 회전
        seq.Insert(
            0f,
            leafRect.DOLocalRotate(
                new Vector3(0f, 0f, -30f),
                1f)
            .SetEase(Ease.OutQuad));

        // 아래로 낙하
        seq.Insert(
            1.1f,
            leafRect.DOAnchorPosY(
                startPos.y - 125f,
                1.5f)
            .SetEase(Ease.InQuad));

        // 좌우 흔들림
        seq.Insert(
            1.1f,
            leafRect.DOAnchorPosX(
                startPos.x - 10f,
                0.375f)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));

        // 페이드아웃
        seq.Insert(
            1.6f,
            leafGraphic.DOFade(0f, 1f));

        await seq.AsyncWaitForCompletion();
        gameObject.SetActive(false);
    }
}

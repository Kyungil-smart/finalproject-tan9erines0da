using DG.Tweening;
using UnityEngine;

public class CatStampTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform catStamp;
    
    public void PlayAnimation()
    {
        if (catStamp == null)
        {
            Debug.LogWarning("CatStamp가 비어있습니다.");
            return;
        }
        gameObject.SetActive(true);

        catStamp.DOKill();

        catStamp.sizeDelta = new Vector2(415f, 415f);

        Sequence seq = DOTween.Sequence();

        seq.Append(
            catStamp.DOSizeDelta(
                new Vector2(663f, 663f),
                0.37f)
            .SetEase(Ease.OutQuad));

        seq.Append(
            catStamp.DOSizeDelta(
                new Vector2(171f, 171f),
                1.2f)
            .SetEase(Ease.InQuart));
        /*seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });*/
    }
}

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class CatStampTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform catStamp;

    public Task PlayAnimation()
    {
        if (catStamp == null)
        {
            Debug.LogWarning("CatStamp가 비어있습니다.");
            return Task.CompletedTask;
        }
        gameObject.SetActive(true);

        catStamp.DOKill();

        Vector2 endPos = catStamp.anchoredPosition;
        catStamp.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        catStamp.sizeDelta = new Vector2(415f, 415f);

        float totalDuration = 0.37f + 1.2f;

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

        seq.Insert(0f, catStamp.DOAnchorPos(endPos, totalDuration).SetEase(Ease.InCubic));

        return seq.AsyncWaitForCompletion();
    }
}

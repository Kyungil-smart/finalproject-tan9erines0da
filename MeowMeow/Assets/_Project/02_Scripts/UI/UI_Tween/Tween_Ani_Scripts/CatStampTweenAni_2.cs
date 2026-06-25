using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CatStampTweenAni_2 : MonoBehaviour
{
    [SerializeField] private RectTransform catStamp;
    [SerializeField] private Graphic stampGraphic;
    [SerializeField] private GatchaContentPresenter _contentPresenter;

    
    public Task PlayAnimation()
    {
        if (catStamp == null)
        {
            Debug.LogWarning("CatStamp가 비어있습니다.");
            return Task.CompletedTask;
        }

        gameObject.SetActive(true);

        catStamp.DOKill();

        if (stampGraphic != null)
        {
            stampGraphic.DOKill();

            Color color = stampGraphic.color;
            color.a = 1f;
            stampGraphic.color = color;
        }

        Sequence seq = DOTween.Sequence();

        catStamp.sizeDelta = new Vector2(307f, 307f);

        seq.Append(
            catStamp.DOSizeDelta(
                new Vector2(176f, 176f),
                0.72f)
            .SetEase(Ease.InQuad));

        seq.Append(
            catStamp.DOSizeDelta(
                new Vector2(69f, 69f),
                0.28f)
            .SetEase(Ease.InQuart));

        if (stampGraphic != null)
        {
            seq.Insert(
                1.0f,
                stampGraphic.DOFade(0.6f, 1f));
        }
        
        seq.OnComplete(() =>
        {
            // 스탬프 애니메이션이 끝나면 나가기 버튼 활성화
            _contentPresenter.ExitButton.interactable = true;
            _contentPresenter.MilestonStart = false;

            gameObject.SetActive(false);
        });

        return seq.AsyncWaitForCompletion();
    }
}

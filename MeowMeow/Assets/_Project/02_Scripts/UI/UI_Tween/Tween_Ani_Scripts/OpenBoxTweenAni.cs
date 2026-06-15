using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OpenBoxTweenAni : MonoBehaviour
{
    [SerializeField] private Image boxImage;
    [SerializeField] private Sprite[] boxFrames;

    [SerializeField] private RectTransform boxRect;
    [SerializeField] private RectTransform lightRect;
    [SerializeField] private RectTransform gemRect;

    public void PlayAnimation()
    {
        if (boxImage == null || boxFrames == null || boxFrames.Length == 0)
        {
            Debug.LogWarning("Box가 비어있습니다.");
            return;
        }
        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        float frameInterval = 0.4f / boxFrames.Length;

        for (int i = 0; i < boxFrames.Length; i++)
        {
            int index = i;

            seq.AppendCallback(() =>
            {
                boxImage.sprite = boxFrames[index];
            });

            seq.AppendInterval(frameInterval);
        }

        seq.OnComplete(() =>
        {
            if (boxRect != null)
                boxRect.SetAsFirstSibling();

            if (lightRect != null)
                lightRect.SetSiblingIndex(1);

            if (gemRect != null)
                gemRect.SetAsLastSibling();
        });
        /*seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });*/
    }
}

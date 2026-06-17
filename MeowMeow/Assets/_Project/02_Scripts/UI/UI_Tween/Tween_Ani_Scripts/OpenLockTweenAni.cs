using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OpenLockTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform lockRect;
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite[] lockFrames;
    [SerializeField] private float frameInterval = 0.05f;

    public void PlayAnimation()
    {
        if (lockRect == null || lockImage == null)
        {
            Debug.LogWarning("Lock 참조가 비어있습니다.");
            return;
        }

        gameObject.SetActive(true);
        lockRect.DOKill();

        float startX = lockRect.anchoredPosition.x;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.1f);

        // 약한 흔들림 (4.94도) - 좌 > 제자리 > 우 > 제자리
        seq.Append(lockRect.DOAnchorPosX(startX - 8f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, 4.94f), 0.1f));

        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

        seq.Append(lockRect.DOAnchorPosX(startX + 8f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, -4.94f), 0.1f));

        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

        seq.AppendInterval(0.3f);

        // 강한 흔들림 (15.6도) - 좌 > 제자리 > 우 > 제자리
        seq.Append(lockRect.DOAnchorPosX(startX - 15f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, 15.6f), 0.1f));

        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

        seq.Append(lockRect.DOAnchorPosX(startX + 15f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, -15.6f), 0.1f));

        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

        seq.AppendInterval(0.5f);

        seq.OnComplete(() => PlayFrameAnimation());
    }

    private void PlayFrameAnimation()
    {
        if (lockFrames == null || lockFrames.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        Sequence frameSeq = DOTween.Sequence();

        for (int i = 0; i < lockFrames.Length; i++)
        {
            int index = i;
            frameSeq.AppendCallback(() => lockImage.sprite = lockFrames[index]);
            frameSeq.AppendInterval(frameInterval);
        }

        frameSeq.OnComplete(() => gameObject.SetActive(false));
    }
}

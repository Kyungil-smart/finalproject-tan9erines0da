using DG.Tweening;
using UnityEngine;

public class LockTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform lockAni;
    [SerializeField] private RectTransform lock01;
    [SerializeField] private RectTransform lock02;

    public void Start()
    {
        PlayAnimation();
    }
    public void PlayAnimation()
    {
        if (lockAni == null || lock01 == null || lock02 == null)
        {
            Debug.LogWarning("Lock 참조가 비어있습니다.");
            return;
        }

        gameObject.SetActive(true);

        lockAni.DOKill();
        lock01.DOKill();
        lock02.DOKill();

        float startX = lockAni.anchoredPosition.x;

        Sequence seq = DOTween.Sequence();

        // 시작 전 0.1초 대기
        seq.AppendInterval(0.1f);

        // 좌측 약하게
        seq.Append(lockAni.DOAnchorPosX(startX - 8f, 0.1f));
        seq.Join(lock01.DOLocalRotate(new Vector3(0, 0, 5f), 0.1f));
        seq.Join(lock02.DOLocalRotate(new Vector3(0, 0, 5f), 0.1f));

        seq.Append(lockAni.DOAnchorPosX(startX, 0.1f));
        seq.Join(lock01.DOLocalRotate(Vector3.zero, 0.1f));
        seq.Join(lock02.DOLocalRotate(Vector3.zero, 0.1f));

        // 우측 약하게
        seq.Append(lockAni.DOAnchorPosX(startX + 8f, 0.1f));
        seq.Join(lock01.DOLocalRotate(new Vector3(0, 0, -5f), 0.1f));
        seq.Join(lock02.DOLocalRotate(new Vector3(0, 0, -5f), 0.1f));

        seq.Append(lockAni.DOAnchorPosX(startX, 0.1f));
        seq.Join(lock01.DOLocalRotate(Vector3.zero, 0.1f));
        seq.Join(lock02.DOLocalRotate(Vector3.zero, 0.1f));

        // 좌측 강하게
        seq.Append(lockAni.DOAnchorPosX(startX - 15f, 0.1f));
        seq.Join(lock01.DOLocalRotate(new Vector3(0, 0, 15.6f), 0.1f));
        seq.Join(lock02.DOLocalRotate(new Vector3(0, 0, 15.6f), 0.1f));

        seq.Append(lockAni.DOAnchorPosX(startX, 0.1f));
        seq.Join(lock01.DOLocalRotate(Vector3.zero, 0.1f));
        seq.Join(lock02.DOLocalRotate(Vector3.zero, 0.1f));

        // 우측 강하게
        seq.Append(lockAni.DOAnchorPosX(startX + 15f, 0.1f));
        seq.Join(lock01.DOLocalRotate(new Vector3(0, 0, -15.6f), 0.1f));
        seq.Join(lock02.DOLocalRotate(new Vector3(0, 0, -15.6f), 0.1f));

        seq.Append(lockAni.DOAnchorPosX(startX, 0.1f));
        seq.Join(lock01.DOLocalRotate(Vector3.zero, 0.1f));
        seq.Join(lock02.DOLocalRotate(Vector3.zero, 0.1f));

        seq.Append(
    lock02.DOAnchorPosY(
        lock02.anchoredPosition.y + 8f,
        0.15f)
    .SetEase(Ease.OutQuad));

        seq.Join(
            lock02.DOLocalRotate(
                new Vector3(0f, 180f, 0f),
                0.15f,
                RotateMode.Fast)
            .SetEase(Ease.OutBack));
    }
}

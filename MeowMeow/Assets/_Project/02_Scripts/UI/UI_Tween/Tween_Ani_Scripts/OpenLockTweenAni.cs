using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OpenLockTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform lockRect;
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite[] lockFrames;
    [SerializeField] private float frameInterval = 0.05f;

    public async Task PlayAnimation()
    {
        if (lockRect == null || lockImage == null) return;

        // 전체 애니메이션 완료를 보장할 TaskCompletionSource 생성
        var tcs = new TaskCompletionSource<bool>();

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
        // 첫 번째 시퀀스 완료 후 프레임 애니메이션 시작
        seq.OnComplete(() =>
        {
            PlayFrameAnimation().ContinueWith(_ => tcs.SetResult(true));
        });

        // 모든 과정이 끝날 때까지 대기
        await tcs.Task;
    }

    // Task를 반환하도록 수정
    private Task PlayFrameAnimation()
    {
        var tcs = new TaskCompletionSource<bool>();

        if (lockFrames == null || lockFrames.Length == 0)
        {
            gameObject.SetActive(false);
            tcs.SetResult(true);
            return tcs.Task;
        }

        Sequence frameSeq = DOTween.Sequence();
        for (int i = 0; i < lockFrames.Length; i++)
        {
            int index = i;
            frameSeq.AppendCallback(() => lockImage.sprite = lockFrames[index]);
            frameSeq.AppendInterval(frameInterval);
        }

        frameSeq.OnComplete(() => {
            gameObject.SetActive(false);
            tcs.SetResult(true);
        });

        return tcs.Task;
    }
}

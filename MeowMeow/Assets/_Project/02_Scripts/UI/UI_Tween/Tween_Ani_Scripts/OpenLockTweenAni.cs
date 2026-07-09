using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
/*
     DOKill(): 해당 RectTransform에 걸려있는 모든 트윈을 즉시 중지
    Sequence(): 여러 트윈을 순차적으로 또는 병렬로 실행하기 위한 컨테이너
    Append(Tween): 시퀀스의 끝에 트윈을 추가합니다. (이전 애니메이션이 끝난 후 실행)
    Join(Tween): 시퀀스의 바로 직전에 추가된 트윈과 동시에 실행합니다.
    Insert(float, Tween): 시퀀스의 특정 시간(float) 위치에 트윈을 삽입
    InsertCallback(float, Action): 특정 시점에 함수를 실행합니다. 프레임 교체(스프라이트 변경)에 최적
    AsyncWaitForCompletion(): C# async/await 문법을 사용하여 시퀀스가 완료될 때까지 비동기로 대기
     */
public class OpenLockTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform bodyRect;
    [SerializeField] private RectTransform lockRect;
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite[] lockFrames;
    [SerializeField] private float frameInterval = 0.05f;
    [SerializeField] private Button _limitedItemButton;

    private float startX;
    private float totalFrameTime;
    [SerializeField] private int Grade;
    private void OnEnable()
    {
        if(Grade == 1) chanage(GatchaDataManager.Instance.Grade_1);
        else if(Grade == 2) chanage(GatchaDataManager.Instance.Grade_2);
        else if(Grade == 3) chanage(GatchaDataManager.Instance.Grade_3);
 
    }
    private void chanage(bool flag)
    {
        if (flag == false)
        {
            lockImage.sprite = lockFrames[0];
        }
    }
    [ContextMenu("Test")]
    public async Task PlayAnimation()
    {
        var flag = OnPlayCheck();
        if (flag == false) return;

        startX = lockRect.anchoredPosition.x;
        totalFrameTime = lockFrames.Length * frameInterval;

        Sequence seq = DOTween.Sequence();

        PlayShakeAnimation(seq);
        PlayUnlockAnimation(seq);
        SetOnCompleteCallback(seq);

        await seq.AsyncWaitForCompletion();
    }
    private bool OnPlayCheck()
    {
        if (lockRect == null || lockImage == null || bodyRect == null || _limitedItemButton == null) return false;

        _limitedItemButton.interactable = false;
        lockRect.DOKill();
        return true;
    }

    private void PlayShakeAnimation(Sequence seq)
    {
        // 1. 초기 흔들림 로직 (시퀀스 시작)
        seq.AppendInterval(0.1f);

        // 약한 흔들림
        seq.Append(lockRect.DOAnchorPosX(startX - 8f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, 4.94f), 0.1f));
        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));
        seq.Append(lockRect.DOAnchorPosX(startX + 8f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, -4.94f), 0.1f));
        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

        seq.AppendInterval(0.3f);

        // 강한 흔들림
        seq.Append(lockRect.DOAnchorPosX(startX - 15f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, 15.6f), 0.1f));
        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));
        seq.Append(lockRect.DOAnchorPosX(startX + 15f, 0.1f));
        seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, -15.6f), 0.1f));
        seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
        seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

        // 효과음
        seq.AppendCallback(() =>
        {
            SoundManager.Instance.Invoke(AudioType.SFX_Player_Collect_Coin_3);
        });

        seq.AppendInterval(0.5f);
    }
    private void PlayUnlockAnimation(Sequence seq)
    {
        // 2. [커지면서 열리기 시작] (전체 프레임 시간 동안 커짐)
        // 현재 시퀀스 위치에서 바로 커지기 시작
        seq.Append(lockRect.DOSizeDelta(new Vector2(231, 231), 0.5f));

        // 3. [프레임 교체] 커지는 동안 병렬로 실행
        // 위 Append와 동시에 시작하도록 Join 사용
        for (int i = 0; i < lockFrames.Length; i++)
        {
            int index = i;
            // 커지는 타임라인에 맞추기 위해 Insert 사용
            seq.InsertCallback(seq.Duration() - totalFrameTime + (i * frameInterval), () =>
            {
                lockImage.sprite = lockFrames[index];
            });
        }

        // 4. [다 열리기 직전 원래 사이즈로 복귀]
        // 전체 시간의 90% 지점(끝나기 직전)부터 원래 크기로 축소
        float shrinkTime = 0.2f;
        seq.Insert(seq.Duration() - shrinkTime, lockRect.DOSizeDelta(new Vector2(169, 169), 0.5f));
    }
    private void SetOnCompleteCallback(Sequence seq)
    {
        // 5. 완료 처리
        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            _limitedItemButton.interactable = true;
        });
    }
    //public async Task PlayAnimation()
    //{
    //    if (lockRect == null || lockImage == null|| bodyRect ==null) return;

    //    // 애니메이션 시작 전에 한정상품 미리보기 버튼 비활성화
    //    _limitedItemButton.interactable = false;
    //   // bodyRect.DOAnchorPosY(13f, 1f);

    //    // 전체 애니메이션 완료를 보장할 TaskCompletionSource 생성
    //    var tcs = new TaskCompletionSource<bool>();

    //    lockRect.DOKill();
    //    float startX = lockRect.anchoredPosition.x;

    //    Sequence seq = DOTween.Sequence(); 
    //    seq.AppendInterval(0.1f);

    //    // 약한 흔들림 (4.94도) - 좌 > 제자리 > 우 > 제자리
    //    seq.Append(lockRect.DOAnchorPosX(startX - 8f, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, 4.94f), 0.1f));

    //    seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

    //    seq.Append(lockRect.DOAnchorPosX(startX + 8f, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, -4.94f), 0.1f));

    //    seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

    //    seq.AppendInterval(0.3f);

    //    // 강한 흔들림 (15.6도) - 좌 > 제자리 > 우 > 제자리
    //    seq.Append(lockRect.DOAnchorPosX(startX - 15f, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, 15.6f), 0.1f));

    //    seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

    //    seq.Append(lockRect.DOAnchorPosX(startX + 15f, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(new Vector3(0f, 0f, -15.6f), 0.1f));

    //    seq.Append(lockRect.DOAnchorPosX(startX, 0.1f));
    //    seq.Join(lockRect.DOLocalRotate(Vector3.zero, 0.1f));

    //    seq.AppendInterval(0.5f);

    //    // 첫 번째 시퀀스 완료 후 프레임 애니메이션 시작
    //    seq.OnComplete(() =>
    //    {

    //        PlayFrameAnimation().ContinueWith(_ => tcs.SetResult(true));

    //    });

    //    // 모든 과정이 끝날 때까지 대기
    //    await tcs.Task;
    //}

    // Task를 반환하도록 수정
    private async Task PlayFrameAnimation()
    {
        //var tcs = new TaskCompletionSource<bool>();

        if (lockFrames == null || lockFrames.Length == 0)
        {
            gameObject.SetActive(false);
            //tcs.SetResult(true);
            //return tcs.Task;
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
            //tcs.SetResult(true);
        });


        //return tcs.Task;
    }
}

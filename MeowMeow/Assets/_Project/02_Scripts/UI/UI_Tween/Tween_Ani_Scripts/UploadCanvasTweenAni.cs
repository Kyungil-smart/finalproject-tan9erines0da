using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UploadCanvasTweenAni : UIAnimationEffect
{
    [Header("Upload_Canvas의 Phone_frame_panel 참조")]
    [SerializeField] private RectTransform _uploadCanvas;
    [Header("팝업 연출 속도")]
    [SerializeField] private float _second;
    [Header("팝업 위치 옵셋값")]
    [SerializeField] private float _offsetY;

    UIPanel _backgroundPanel;

    [ContextMenu("업로드 캔버스 열기 애니메이션 재생")]
    public Task PlayOpenAnimation()
    {
        if (_uploadCanvas == null)
        {
            Debug.LogWarning("_uploadCanvas가 비어있습니다.");
            return Task.CompletedTask;
        }

        // 구현에 따라서 이부분은 삭제해도 됩니다.
        gameObject.SetActive(true);

        _uploadCanvas.DOKill();

        _uploadCanvas.anchoredPosition = new Vector2(0f, _offsetY);

        Sequence seq = DOTween.Sequence();

        seq.Append(_uploadCanvas.DOAnchorPosY(0f, _second).SetEase(Ease.InQuad));

        return seq.AsyncWaitForCompletion();
    }

    [ContextMenu("업로드 캔버스 닫기 애니메이션 재생")]
    public Task PlayCloseAnimation()
    {
        if (_uploadCanvas == null)
        {
            Debug.LogWarning("_uploadCanvas가 비어있습니다.");
            return Task.CompletedTask;
        }

        _uploadCanvas.DOKill();

        _uploadCanvas.anchoredPosition = new Vector2(0f, 500f);

        Sequence seq = DOTween.Sequence();

        seq.Append(_uploadCanvas.DOAnchorPosY(_offsetY, _second).SetEase(Ease.InQuad));

        // 구현에 따라서 이부분은 삭제해도 됩니다.
        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        return seq.AsyncWaitForCompletion();
    }

    public override void PlayIn(Action onComplete)
    {
        // 예외처리
        if (_uploadCanvas == null)
        {
            Debug.LogWarning("_uploadCanvas가 비어있습니다.");
            onComplete?.Invoke();
            return;
        }
        // 트윈 실행부
        if (_backgroundPanel != null)
            _backgroundPanel.gameObject.SetActive(true);

        _uploadCanvas.DOKill();
        _uploadCanvas.anchoredPosition = new Vector2(0f, _offsetY);

        _uploadCanvas.DOAnchorPosY(0f, _second)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (_backgroundPanel != null)
                    _backgroundPanel.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    public override void PlayOut(Action onComplete)
    {
        if (_uploadCanvas == null)
        {
            Debug.LogWarning("_uploadCanvas가 비어있습니다.");
            return;
        }

        if( _backgroundPanel != null)
        {
            _backgroundPanel.gameObject.SetActive(true);

            _uploadCanvas.DOKill();

            _uploadCanvas.DOAnchorPosY(_offsetY, _second)
                .SetEase(Ease.InQuad).OnComplete(() =>
                {
                    _uploadCanvas.anchoredPosition = new Vector2(0f, 0f);
                    onComplete?.Invoke();
                });
        }
        else
        {
            _uploadCanvas.anchoredPosition = new Vector2(0f, 0f);
            onComplete?.Invoke();
        }

    }

    public void SetupBackground(UIPanel target)
    {
        _backgroundPanel = target;
    }
}

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UploadCanvasTweenAni : MonoBehaviour
{
    [Header("Upload_Canvas의 Phone_frame_panel 참조")]
    [SerializeField] private RectTransform _uploadCanvas;

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

        _uploadCanvas.anchoredPosition = new Vector2(0f, -2110f);

        Sequence seq = DOTween.Sequence();

        seq.Append(_uploadCanvas.DOAnchorPosY(0f, 0.7f).SetEase(Ease.Linear));

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

        seq.Append(_uploadCanvas.DOAnchorPosY(-2110f, 0.8f).SetEase(Ease.Linear));

        // 구현에 따라서 이부분은 삭제해도 됩니다.
        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        return seq.AsyncWaitForCompletion();
    }
}

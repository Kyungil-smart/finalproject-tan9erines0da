using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StickerLimitTweenAni : MonoBehaviour
{
    [Header("Sticker_Limit_Reached를 참조")]
    [SerializeField] private CanvasGroup _stickerLimitCanvasGroup;
    private float _defaultAlpha;

    private bool _isPlayingLimitAni = false;

    private void Awake()
    {
        if (_stickerLimitCanvasGroup != null)
        {
            _defaultAlpha = _stickerLimitCanvasGroup.alpha;
        }
    }

    public Task PlayAnimation()
    {
        if (_isPlayingLimitAni) return Task.CompletedTask;

        _isPlayingLimitAni = true;

        if (_stickerLimitCanvasGroup == null)
        {
            Debug.LogWarning("_stickerLimitCanvasGroup가 비어있습니다.");
            _isPlayingLimitAni = false;
            return Task.CompletedTask;
        }

        gameObject.SetActive(true);

        _stickerLimitCanvasGroup.DOKill();

        _stickerLimitCanvasGroup.alpha = _defaultAlpha;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1f);

        if (_stickerLimitCanvasGroup != null)
        {
            seq.Append(_stickerLimitCanvasGroup.DOFade(0f, 0.3f).SetEase(Ease.OutCubic));
        }

        seq.OnComplete(() =>
        {
            _isPlayingLimitAni = false;
            gameObject.SetActive(false);
        });

        return seq.AsyncWaitForCompletion();
    }
}

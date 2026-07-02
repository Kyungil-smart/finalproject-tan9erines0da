using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StickerLimitTweenAni : MonoBehaviour
{
    [SerializeField] private RectTransform _stickerLimitReached;
    [SerializeField] private Graphic _stickerLimitGraphic;
    private bool _isPlayingLimitAni = false;

    public Task PlayAnimation()
    {
        if (_isPlayingLimitAni) return Task.CompletedTask;

        _isPlayingLimitAni = true;

        if (_stickerLimitReached == null)
        {
            Debug.LogWarning("_stickerLimitReached가 비어있습니다.");
            return Task.CompletedTask;
        }

        gameObject.SetActive(true);

        _stickerLimitReached.DOKill();

        if (_stickerLimitGraphic != null)
        {
            _stickerLimitGraphic.DOKill();

            Color color = _stickerLimitGraphic.color;
            color.a = 1f;
            _stickerLimitGraphic.color = color;
        }

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1f);

        if (_stickerLimitGraphic != null)
        {
            seq.Append(_stickerLimitGraphic.DOFade(0f, 0.3f).SetEase(Ease.OutQuad));
        }

        seq.OnComplete(() =>
        {
            _isPlayingLimitAni = false;
            gameObject.SetActive(false);
        });

        return seq.AsyncWaitForCompletion();
    }
}

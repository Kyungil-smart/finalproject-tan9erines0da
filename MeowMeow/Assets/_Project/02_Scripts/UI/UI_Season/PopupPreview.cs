using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPreview : MonoBehaviour, IPopupable
{
    [Header("참조 필요")]
    [SerializeField] Button _exitButton;
    [SerializeField] Image _resourceImage;
    [SerializeField] TextMeshProUGUI _description;

    GatchaContentPresenter _contentPresenter;

    public void Open()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        if (_exitButton != null)
        {
            _exitButton.interactable = true;
        }
    }

    public void Close()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        _resourceImage.sprite = null;
        _description.text = string.Empty;
    }

    public void SetData(int itemId)
    {
        // id로 데이터 가져오기
        var db = googleSheetManager.instance.GetClassData<PreviewPopupTable>();
        var data = db.FindById(itemId.ToString());

        // db 기준 이미지 교체
        _resourceImage.sprite = PopupSpriteCacheManager.Instance.GetPopupSprite(data.Resource);
        // 아이템 설명 교체
        _description.text = data.Description;
    }

    public void Bind(GatchaContentPresenter gcp)
    {
        _contentPresenter = gcp;
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    public void Unbind()
    {
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
        _contentPresenter = null;
    }

    void OnExitButtonClick()
    {
        if (_contentPresenter == null) return;
        // 중복 클릭 방지
        _exitButton.interactable = false;

        _contentPresenter.ClosePopup(this);
    }

    void OnDestroy()
    {
        Unbind();
    }
}

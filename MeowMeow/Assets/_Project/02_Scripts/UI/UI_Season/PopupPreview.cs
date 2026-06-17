using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupPreview : MonoBehaviour, IPopupable
{
    [Header("참조 필요")]
    [SerializeField] Button _exitButton;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _description;

    GatchaContentPresenter _contentPresenter;

    public void Open()
    {
        if (_exitButton != null)
        {
            _exitButton.interactable = true;
        }
    }

    public void Close()
    {
        _description.text = string.Empty;
    }

    public void SetData(int itemId)
    {
        // id로 데이터 가져오기
        var db = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        var data = db.FindById(itemId.ToString());

        // Todo db 기준 이미지 교체
         이미지 
        // Todo 아이템 설명 교체
         디스크립션 ./ Text

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

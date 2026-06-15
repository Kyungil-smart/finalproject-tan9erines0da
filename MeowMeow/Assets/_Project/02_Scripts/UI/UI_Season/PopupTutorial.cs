using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTutorial : MonoBehaviour, IPopupable
{
    [SerializeField] Button _exitButton;

    GatchaContentPresenter _contentPresenter;

    public void Open()
    {
        if(_exitButton != null)
        {
            _exitButton.interactable = true;
        }
    }
    public void Close()
    {
        // 필요한 처리 없음
    }

    public void SetData(int itemId)
    {
        // 필요 데이터 없음
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

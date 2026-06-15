using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupReset : MonoBehaviour, IPopupable
{
    [SerializeField] Button _exitButton;
    [SerializeField] Button _resetButton;

    GatchaContentPresenter _contentPresenter;

    public void Open()
    {
        if (_exitButton != null)
        {
            _exitButton.interactable = true;
        }
        if (_resetButton != null)
        {
            _resetButton.interactable = true;
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
        _resetButton.onClick.AddListener(OnResetButtonClick);
    }

    public void Unbind()
    {
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
        _resetButton.onClick.RemoveListener(OnResetButtonClick);
        _contentPresenter = null;
    }

    void OnExitButtonClick()
    {
        if (_contentPresenter == null) return;
        // 중복 클릭 방지
        _exitButton.interactable = false;

        _contentPresenter.ClosePopup(this);
    }

    void OnResetButtonClick()
    {
        if (_contentPresenter == null) return;
        // 중복 클릭 방지
        _resetButton.interactable = false;

        _contentPresenter.ClosePopup(this);
        // 보드 초기화 실행
        _contentPresenter.ResetGachaBlocks();
    }

    void OnDestroy()
    {
        Unbind();
    }
}

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

   async void OnResetButtonClick()
    {
        if (_contentPresenter == null) return;
        // 중복 클릭 방지
        _resetButton.interactable = false;

        // 보드 초기화 실행
       await _contentPresenter.ResetGachaBlocks();

        // 초기화 버튼을 처음 눌렀을때
        if (_contentPresenter.IsFirstReset == false) _contentPresenter.IsFirstReset = true;
        // 초기화 버튼을 2회차 이상 눌렀을때 일반 1등 보상 획득을 false 처리
        else GatchaDataManager.Instance.Normal_Grade_1 = false;

        // 초기화 버튼 비활성화
        _contentPresenter.ResetButton.interactable = false;

        _contentPresenter.ClosePopup(this);
    }

    void OnDestroy()
    {
        Unbind();
    }
}

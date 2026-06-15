using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTutorial : MonoBehaviour, IPopupable
{
    [SerializeField] Button _exitButton;

    public void Open()
    {
        // 필요 연출 없음
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
        _exitButton.onClick += gcp.ClosePopup(this);
    }

    public void Unbind()
    {
        throw new System.NotImplementedException();
    }
}

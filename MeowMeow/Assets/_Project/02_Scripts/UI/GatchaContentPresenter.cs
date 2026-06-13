using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatchaContentPresenter : MonoBehaviour
{
    [Header("팝업 관리목록 ")]
    [SerializeField] private IPopupable _tutorialCanvas;
    [SerializeField] private IPopupable _previewCanvas;
    [SerializeField] private IPopupable _gatchaCanvas;
    [SerializeField] private IPopupable _milestoneCanvas;
    [SerializeField] private IPopupable _resetCanvas;


    [Header("메인 캔버스 버튼")]
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private List<Button> _previewButtons;
    [SerializeField] private List<Button> _gatchaButtons;

    [Header("토글형 오브젝트")]
    [SerializeField] private List<ISwitchable> _milestonBlocks;
    [SerializeField] private List<ISwitchable> _gatchaBlocks;

    //--------------내부 필드-----------------------
    bool _isPopupOpen = false;

    public void OpenPopup(IPopupable popup, int itemId = 0)
    {
        if (_isPopupOpen) return;
                
        popup.gameObject.SetActive(true);
        if (itemId != 0)
        {
            popup.SetData(itemId);
        }
        popup.Unbind();
        popup.Bind(this);
        popup.Open();

        _isPopupOpen = true;
    }
}

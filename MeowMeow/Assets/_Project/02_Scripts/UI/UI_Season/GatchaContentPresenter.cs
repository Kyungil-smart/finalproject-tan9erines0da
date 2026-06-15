using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class GatchaContentPresenter : MonoBehaviour
{
    [Header("팝업 관리목록 ")]
    [SerializeField] private GameObject _tutorialCanvas;
    [SerializeField] private GameObject _previewCanvas;
    [SerializeField] private GameObject _gatchaCanvas;
    [SerializeField] private GameObject _milestoneCanvas;
    [SerializeField] private GameObject _resetCanvas;


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

    void Awake()
    {
        Bind();
    }
    public void OpenPopup(IPopupable popup, int itemId = 0)
    {
        if (_isPopupOpen || popup == null) return;
                
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
    public void ClosePopup(IPopupable popup)
    {
        if (popup == null || !_isPopupOpen) return;

        popup.Unbind();
        popup.Close();

        popup.gameObject.SetActive(false);
        _isPopupOpen= false;
        
    }

    void Bind()
    {
        _tutorialButton.onClick.AddListener(OnTutorialClick);
    }
    void OnTutorialClick()
    {
        IPopupable popup = _tutorialCanvas.GetComponent<IPopupable>();
        OpenPopup(popup);
    }

    
}

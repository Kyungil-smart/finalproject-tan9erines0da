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
    [SerializeField] private List<GachaStackBox> _milestonBlocks;
    [SerializeField] private List<Limited_Button> _LinitedBlocks;
    [SerializeField] private List<GatchaButton> _gatchaBlocks;

    //--------------내부 필드-----------------------
    bool _isPopupOpen = false;

    void Awake()
    {
        Bind();
    }
    /// <summary>
    /// 팝업 패널을 여는 함수입니다
    /// </summary>
    /// <param name="popup">열고자하는 팝업입니다</param>
    /// <param name="itemId">데이터가 필요할 때 아이템 아이디를 입력합니다</param>
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
    /// <summary>
    /// 팝업 패널을 닫는 함수입니다
    /// </summary>
    /// <param name="popup"></param>
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
    /// <summary>
    /// 메인 캔버스가 열릴때 호출하는 함수
    /// </summary>
    private void OnOpen()
    {
        // 뽑기 블럭 SetView 순회
        for(int i = 0; i < _gatchaBlocks.Count; i++)
        {
            bool isOpened = GatchaDataManager.Instance.IsOpened(i);
            _gatchaBlocks[i].SetView(isOpened);

            if (isOpened)
            {
                _gatchaBlocks[i].SetViewCover(i);
            }
        }

        // 누적 보상 SetView 순회
        int milestonCount = GatchaDataManager.Instance.GatchaData.TotalGatchaCount;

        for (int i = 0; i <_milestonBlocks.Count; i++)
        {
            // milestonCount <= (현재 _milestonBlocks은 인덱스 * 10 + 10)
            bool isOpened = (milestonCount <= i * 10 + 10);

            _milestonBlocks[i].SetView(isOpened);
        }
    }
}

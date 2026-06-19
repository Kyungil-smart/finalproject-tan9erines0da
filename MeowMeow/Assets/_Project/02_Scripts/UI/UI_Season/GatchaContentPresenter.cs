using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GatchaContentPresenter : MonoBehaviour
{
    [Header("메인 캔버스 오브젝트")]
    [SerializeField] private GameObject _mainCanvasOBJ;


    [Header("팝업 관리목록 ")]
    [SerializeField] private GameObject _tutorialCanvas;
    [SerializeField] private GameObject _previewCanvas;
    [SerializeField] private GameObject _gatchaCanvas;
    [SerializeField] private PopupMilestone _milestoneCanvas;
    [SerializeField] private GameObject _resetCanvas;


    [Header("메인 캔버스 버튼")]
    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private List<Button> _previewButtons;
     private List<Button> _gatchaButtons =new List<Button>();

    [Header("토글형 오브젝트")]
    [SerializeField] private List<GachaStackBox> _milestonBlocks;
    [SerializeField] private Limited_Button _1stLinitedBlock;//진열장
    [SerializeField] private Limited_Button _2ndLinitedBlock;//진열장
    [SerializeField] private Limited_Button _3rdLinitedBlock;//진열장
    [SerializeField] private Button _resetButton;//초기화 버튼 
    [SerializeField] private List<GatchaButton> _gatchaBlocks;

    [Header("뽑기 등수 별 보상 표시 그룹")]
    [SerializeField] private RefreshPrizeRank _refreshPrizeRank;

    [Header("뽑기권 정보관련 텍스트")]
    [SerializeField] private TextMeshProUGUI _dailyTicket;
    [SerializeField] private TextMeshProUGUI _questTicket;
    [SerializeField] private TextMeshProUGUI _ownedTickets;
    [SerializeField] private TextMeshProUGUI _gachaStack;

    // ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
    [SerializeField] private GameObject _limitedRewardPanel;
    [SerializeField] private Image _seasonPrizeLine;// 한정 보상 연출시 보상 이미지를 바꾸기 위한 변수
    [SerializeField] private GetPrizeTweenAni _limitedRewardTween;

    //--------------내부 필드-----------------------
    bool _isPopupOpen = false;
    bool CanReset { get { return GatchaDataManager.Instance.Grade_1; } }
    //리미티드 키 스트링 입니다.
    public int L_itemID;
    //-------------디버그 버튼 관련 필드
    [SerializeField] private GameObject _debugButton;
    public bool IsMainCanvasOpen = false;
    void Awake()
    {
        foreach(var item in _gatchaBlocks)
        {
            var temp_BTN=item.GetComponent<Button>();
            _gatchaButtons.Add(temp_BTN);
        }
        
        
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
    public async  void ClosePopup(IPopupable popup)
    {
        if (popup == null || !_isPopupOpen)
        {
            Debug.Log($"클로즈 실패 {_isPopupOpen}");
            return;
        }

        popup.Unbind();
        popup.Close();

        popup.gameObject.SetActive(false);
        _isPopupOpen = false;
        /*
        L_itemID 이 값이 있으면   자물쇠 연출 시작
        30001 : 1등급 유일 보상
        40001 : 2등급 유일 보상 
        30002 : 3등급 유일 보상
        */

        var Grade = L_itemID == 30001 ? 1 :
                    L_itemID == 40001 ? 2 :
                    L_itemID == 30002 ? 3 : -1;
        if (Grade == -1) return;

     var LinitedBlock = Grade == 1 ? _1stLinitedBlock :
                        Grade == 2 ? _2ndLinitedBlock :
                        Grade == 3 ? _3rdLinitedBlock : null;

        if (LinitedBlock == null) return;

        var TweenLogic=LinitedBlock.GetComponentInChildren<OpenLockTweenAni>(true);
        var _PopupGatcha= popup.gameObject.GetComponent<PopupGatcha>();
        TweenLogic.gameObject.transform.parent.gameObject.SetActive(true);
        await TweenLogic.PlayAnimation(); // 자물쇠 연출 끝날 때 까지 대기

        _limitedRewardPanel.gameObject.SetActive(true);
        var data= _limitedRewardTween.GetComponent<GetPrizeTweenAni>();
        // 한정 보상 팝업 이미지를 한번 초기화 후 등수에 맞게 적용
        _seasonPrizeLine.sprite = null;
        _seasonPrizeLine.sprite = PopupSpriteCacheManager.Instance.GetPopupSprite(_PopupGatcha.RewardResource);
        await data.PlayAnimation();//한정 보상 팝업 연출
        _limitedRewardPanel.SetActive(false);
         //도장 찍기 연출 

    }
    /// <summary>
    /// 호출하면 현재 DTO에 맞추어 초기화 가능 여부에 따라 초기화 버튼을 활성화 합니다.
    /// OnOpen, PopupGatcha.Close에서 호출 중입니다 추후 추가 혹은 변경이 필요할 수 있습니다
    /// </summary>
    public void ChangeResetButtonState()
    {
        _resetButton.interactable = CanReset;
    }

    void Bind()
    {
        _tutorialButton.onClick.AddListener(OnTutorialClick);
        for (int i = 0; i < _gatchaButtons.Count; i++)
        {
            int index = i; // 클로저 캡쳐 방지
            _gatchaButtons[i].onClick.AddListener(() => OnGatchaButtonClick(index));
        }
        _exitButton.onClick.AddListener(OnExitClick);
        _enterButton.onClick.AddListener(OnEnterClick);


        _1stLinitedBlock.GetComponent<Button>().onClick.AddListener(Button_Grade_1);
        _2ndLinitedBlock.GetComponent<Button>().onClick.AddListener(Button_Grade_2);
        _3rdLinitedBlock.GetComponent<Button>().onClick.AddListener(Button_Grade_3);

        _resetButton.onClick.AddListener(OpenResetCanvas);
    }
    private void Button_Grade_1()
    {
        var data = _previewCanvas.GetComponent<IPopupable>();
        var Grade = 50001;
        OpenPopup(data, Grade);
    }
    private void Button_Grade_2()
    {
        var data = _previewCanvas.GetComponent<IPopupable>();
        var Grade = 50002;
        OpenPopup(data, Grade);
    }
    private void Button_Grade_3()
    {
        var data = _previewCanvas.GetComponent<IPopupable>();
        var Grade = 50003;
        OpenPopup(data, Grade);
    }
    private void OpenResetCanvas()
    {
        var data = _resetCanvas.GetComponent<IPopupable>();
        OpenPopup(data);
    }
    private void OnEnterClick()
    {
        Debug.Log("클릭");
        _mainCanvasOBJ.SetActive(true);
        //---- 디버그 관련 ----
        IsMainCanvasOpen = true;
        _debugButton.SetActive(false);
        //---------------------
        OnOpen();
    }
    private void OnExitClick()
    {
        _mainCanvasOBJ.SetActive(false);
        //---- 디버그 관련 ----
        IsMainCanvasOpen = false;
        _debugButton.SetActive(true);
        //---------------------
    }
    void OnTutorialClick()
    {
        IPopupable popup = _tutorialCanvas.GetComponent<IPopupable>();
        OpenPopup(popup);
    }

    // 뽑기판의 뽑기 블럭을 클릭했을 때 실행되는 함수입니다.
    async void OnGatchaButtonClick(int index)
    {
        if (_isPopupOpen) return;
        if (GatchaDataManager.Instance.IsOpened(index)) return;
        if (GatchaDataManager.Instance.GatchaData.OwnedTicketCount <= 0) return;

        int itemId = GatchaDataManager.Instance.GetItemID(index);
        GatchaDataManager.Instance.ExecuteGacha(index);

        _gatchaBlocks[index].SetView(true);
        _gatchaBlocks[index].SetViewCover(index);

        await _gatchaBlocks[index].Do();

        IPopupable popup = _gatchaCanvas.GetComponent<IPopupable>();
        OpenPopup(popup, itemId);
    }

    // 뽑기 결과 팝업이 닫힌 뒤 메인 캔버스의 표시 정보를 갱신하는 함수입니다.
    public void RefreshAfterGacha()
    {
        RefreshOwnedTicketsTXT();
        RefreshGachaStackTXT();
    }
    /// <summary>
    /// 메인 캔버스가 열릴때 호출하는 함수
    /// </summary>
    private void OnOpen()
    {
        // 뽑기 블럭 SetView 순회
        for (int i = 0; i < _gatchaBlocks.Count; i++)
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

        for (int i = 0; i < _milestonBlocks.Count; i++)
        {
            // milestonCount <= (현재 _milestonBlocks은 인덱스 * 10 + 10)
            bool isOpened = (milestonCount >= i * 10 + 10);

            _milestonBlocks[i].PassIndex(i * 10 + 10);

            _milestonBlocks[i].SetView(isOpened);
        }

        // 한정 보상 SetView 호출
        LinitedBlockSetView();

        // 뽑기 등수 별 보상 표시 그룹 갱신
        bool isReset = GatchaDataManager.Instance.GatchaData.IsResetPerformed;
        _refreshPrizeRank.SetView(isReset);

        // 뽑기권 정보 관련 텍스트들 갱신
        RefreshDailyTicketTXT();
        RefreshQuestTicketTXT();
        RefreshOwnedTicketsTXT();
        RefreshGachaStackTXT();

        // 리셋 버튼 활성화 여부 갱신
        ChangeResetButtonState();
    }

    /// <summary>
    /// 초기화를 실행 했을때 뽑기 상품을 초기화 하는 함수입니다.
    /// </summary>
    public void ResetGachaBlocks()
    {
        GatchaDataManager.Instance.RewardReset();

        for (int i = 0; i < _gatchaBlocks.Count; i++)
        {
            bool isOpened = GatchaDataManager.Instance.IsOpened(i);
            _gatchaBlocks[i].SetView(isOpened);
        }
        // 등수별 상품목록 갱신
        bool isReset = GatchaDataManager.Instance.GatchaData.IsResetPerformed;
        _refreshPrizeRank.SetView(isReset);
    }

    // 한정 보상 SetView 호출 함수
    private void LinitedBlockSetView()
    {
        _1stLinitedBlock.SetView(GatchaDataManager.Instance.Grade_1);
        _2ndLinitedBlock.SetView(GatchaDataManager.Instance.Grade_2);
        _3rdLinitedBlock.SetView(GatchaDataManager.Instance.Grade_3);
    }

    #region 뽑기권 정보 관련 텍스트 갱신 함수
    /// <summary>
    /// 출석 보상 뽑기권 개수 갱신 함수입니다.
    /// </summary>
    public void RefreshDailyTicketTXT()
    {
        _dailyTicket.text = $"출석 {GatchaDataManager.Instance.GatchaData.TodayAttendanceTicketCount}/1";
    }
    /// <summary>
    /// 퀘스트 보상 뽑기권 개수 갱신 함수입니다.
    /// </summary>
    public void RefreshQuestTicketTXT()
    {
        _questTicket.text = $"퀘스트 {GatchaDataManager.Instance.GatchaData.TodayQuestTicketCount}/2";
    }
    /// <summary>
    /// 보유한 뽑기권 총 개수 갱신 함수입니다.
    /// </summary>
    public void RefreshOwnedTicketsTXT()
    {
        _ownedTickets.text = $"{GatchaDataManager.Instance.GatchaData.OwnedTicketCount} 개";
    }
    /// <summary>
    /// 누적 뽑기 횟수 갱신 함수입니다.
    /// </summary>
    public void RefreshGachaStackTXT()
    {
        _gachaStack.text = $"{GatchaDataManager.Instance.GatchaData.TotalGatchaCount} 회";
    }
    #endregion

    //====================================================
    // 테스트 코드입니다.
    //====================================================
    [ContextMenu("오픈 누적 보상 팝업")]
    public void OpenTestMilestonePopup()
    {
        OpenPopup(_milestoneCanvas);
    }
}

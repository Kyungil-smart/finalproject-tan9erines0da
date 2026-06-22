using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PopupGatcha : MonoBehaviour, IPopupable, ITweenable
{
    [Header("터치 결과 확인 이미지 (뽑기권 닫힘/열림)")]
    [SerializeField] private GameObject _closedCover;
    [SerializeField] private GameObject _openedCover;
    [SerializeField] private Button _touchCatchButton;

    [Header("뽑기 결과 - 1~3등 상품 그룹 (index 0=1등, 1=2등, 2=3등)")]
    [SerializeField] private GameObject[] _highRankGroups;
    [SerializeField] private Image[] _highRankImages;

    [Header("뽑기 결과 - 1~3등 랭크 이미지 (index 0=1등, 1=2등, 2=3등)")]
    [SerializeField] private GameObject[] _rankGroups;

    [Header("뽑기 결과 - 4~6등")]
    [SerializeField] private GameObject _lowRankGroup;
    [SerializeField] private TextMeshProUGUI _lowRankTMP;

    [Header("한정 보상 획득 패널")]
    [SerializeField] private GameObject _limitedRewardPanel;
[SerializeField] private TextMeshProUGUI _limitedRewardTMP;
    [SerializeField] private GetPrizeTweenAni _limitedRewardTween;
    [SerializeField] private GameObject _Test02;

   [SerializeField] private CatStampTweenAni_2 _Test1;
    [Header("확인 버튼")]
    [SerializeField] private Button _confirmButton;

    GatchaContentPresenter _contentPresenter;

    int _itemId;
    int _grade;
    #region LimitedReward_Panel의 이미지 변경을 위한 변수
    string[] _rewardResourceArr = new string[3];
    string _rewardResource;
    public string RewardResource
    {
        get => _rewardResource;
    }
    #endregion
    bool _isLimitedItem;

    public bool IsTransitioning { get; set; }

    //
    public int TempKey;
    public void Open()
    {
        IsTransitioning = false;

        _closedCover.SetActive(true);
        _openedCover.SetActive(false);
        _limitedRewardPanel.SetActive(false);

        if (_touchCatchButton != null)
            _touchCatchButton.interactable = true;

        if (_confirmButton != null)
            _confirmButton.interactable = false;

        if (TenthCheck())
        {
            _contentPresenter.Need_M_Open = true;
        }
    }

    public void Close()
    {
        _lowRankTMP.text = string.Empty;
        _limitedRewardTMP.text = string.Empty;
        _contentPresenter.L_itemID = TempKey;
        if (_isLimitedItem)
        {
            SubscribeManager.instance.Publish<int>(SubscribeType.GetLimited, _itemId);
        }
        _isLimitedItem = false;

        _contentPresenter.ChangeResetButtonState();
        _contentPresenter = null;
        TempKey = -1;


    }

    public void SetData(int itemId)
    {
        _itemId = itemId;

        var db = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        var data = db.FindById(itemId.ToString());

        _grade = data.Grade;
        _isLimitedItem = _grade <= 3 && data.Repeat == false;

        // 1~3등 아이템 이미지 로드
        if (_grade <= 3 && !string.IsNullOrEmpty(data.RewardResourceImage) && data.RewardResourceImage != "NULL")
        {
            _highRankImages[_grade - 1].sprite = PopupSpriteCacheManager.Instance.GetPopupSprite(data.RewardResourceImage);
            // LimitedReward_Panel의 이미지 변경을 위한 등수별 키값 저장
            _rewardResourceArr[_grade - 1] = data.RewardResourceImage;
        }

        // 한정 보상 텍스트 설정
        if (_isLimitedItem)
        {
            _limitedRewardTMP.text = data.ItemName;
        }
    }

    public void Bind(GatchaContentPresenter gcp)
    {
        _contentPresenter = gcp;
        _touchCatchButton.onClick.AddListener(OnTouchReveal);
        _confirmButton.onClick.AddListener(OnConfirmClick);
    }

    public void Unbind()
    {
        _touchCatchButton.onClick.RemoveListener(OnTouchReveal);
        _confirmButton.onClick.RemoveListener(OnConfirmClick);
    }

    public  void Play()
    {
        if (IsTransitioning) return;
        IsTransitioning = true;

        _closedCover.SetActive(false);
        _openedCover.SetActive(true);

        bool isHighRank = _grade <= 3;

        // 등수에 맞는 그룹만 활성화 (나머지 비활성화)
        for (int i = 0; i < _highRankGroups.Length; i++)
        {
            _highRankGroups[i].SetActive(isHighRank && i == _grade - 1 && _isLimitedItem);
            // LimitedReward_Panel의 이미지 변경을 위해 현재 이미지 키값을 _rewardResource 저장
            // 이후에 GatchaContentPresenter.cs에서 RewardResource를 불러와 이미지를 적용 합니다.
            if ((isHighRank && i == _grade - 1)) _rewardResource = _rewardResourceArr[i];
        }
        for (int i = 0; i < _rankGroups.Length; i++)
            _rankGroups[i].SetActive(isHighRank && i == _grade - 1);

        _lowRankGroup.SetActive(!isHighRank);

        if (!isHighRank)
            _lowRankTMP.text = $"{_grade}등";

        const int NULL_NUMBER= -1;
        TempKey = _isLimitedItem ? _itemId : NULL_NUMBER;

      

        if (_confirmButton != null)
            _confirmButton.interactable = true;

        IsTransitioning = false;
    }

    private   void OnTouchReveal()
    {
        if (IsTransitioning) return;
        // 중복 터치 방지
        _touchCatchButton.interactable = false;

          Play();
    }

    void OnConfirmClick()
    {
        if (_contentPresenter == null) return;
        // 중복 클릭 방지
        _confirmButton.interactable = false;

        _contentPresenter.RefreshAfterGacha();
        _contentPresenter.ClosePopup(this);
    }

    public Task OnlimitedRewardPopup()
    {
        _limitedRewardPanel.gameObject.SetActive(true);
        var data= _limitedRewardTween.GetComponent<GetPrizeTweenAni>();
        return data.PlayAnimation();
    }
    void OnDestroy()
    {
        Unbind();
    }

    private bool TenthCheck()
    {
        int i = GatchaDataManager.Instance.GatchaData.TotalGatchaCount;
        if (i % 10 != 0 || i == 0) return false;
        return true;
    }
}

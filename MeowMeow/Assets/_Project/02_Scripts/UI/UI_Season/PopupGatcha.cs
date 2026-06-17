using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PopupGatcha : MonoBehaviour, IPopupable, ITweenable
{
    [Header("터치 결과 확인 이미지 (뽑기권 닫힘/열림)")]
    [SerializeField] private GameObject _closedCover;
    [SerializeField] private GameObject _openedCover;
    [SerializeField] private Button _touchCatchButton;

    [Header("뽑기 결과 - 1~3등")]
    [SerializeField] private GameObject _highRankGroup;
    [SerializeField] private Image _highRankImage;
    [SerializeField] private TextMeshProUGUI _highRankTMP;

    [Header("뽑기 결과 - 4~6등")]
    [SerializeField] private GameObject _lowRankGroup;
    [SerializeField] private TextMeshProUGUI _lowRankTMP;

    [Header("한정 보상 획득 패널")]
    [SerializeField] private GameObject _limitedRewardPanel;
    [SerializeField] private Image _limitedRewardImage;
    [SerializeField] private TextMeshProUGUI _limitedRewardTMP;
    [SerializeField] private GetPrizeTweenAni _limitedRewardTween;

    [Header("확인 버튼")]
    [SerializeField] private Button _confirmButton;

    GatchaContentPresenter _contentPresenter;

    AsyncOperationHandle<Sprite> _itemImageHandle;
    AsyncOperationHandle<Sprite> _limitedImageHandle;

    int _itemId;
    int _grade;
    bool _isLimitedItem;

    public bool IsTransitioning { get; set; }

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
    }

    public void Close()
    {
        if (_itemImageHandle.IsValid()) Addressables.Release(_itemImageHandle);
        if (_limitedImageHandle.IsValid()) Addressables.Release(_limitedImageHandle);

        _highRankImage.sprite = null;
        _highRankTMP.text = string.Empty;
        _lowRankTMP.text = string.Empty;
        _limitedRewardTMP.text = string.Empty;

        if (_isLimitedItem)
        {
            SubscribeManager.instance.Publish<int>(SubscribeType.GetLimited, _itemId);
        }
        _isLimitedItem = false;
    }

    public async void SetData(int itemId)
    {
        _itemId = itemId;

        var db = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        var data = db.FindById(itemId.ToString());

        _grade = data.Grade;
        _isLimitedItem = _grade <= 3 && data.Repeat == false;

        // 1~3등 아이템 이미지 로드
        if (_grade <= 3)
        {
            if (_itemImageHandle.IsValid()) Addressables.Release(_itemImageHandle);
            _itemImageHandle = Addressables.LoadAssetAsync<Sprite>(data.RewardResourceImage);
            await _itemImageHandle.Task;

            if (_itemImageHandle.Status == AsyncOperationStatus.Succeeded)
                _highRankImage.sprite = _itemImageHandle.Result;
            else
                Debug.LogWarning("이미지 로드 실패");
        }

        // 한정 보상 이미지 로드
        if (_isLimitedItem)
        {
            if (_limitedImageHandle.IsValid()) Addressables.Release(_limitedImageHandle);
            _limitedImageHandle = Addressables.LoadAssetAsync<Sprite>(data.RewardResourceTextImage);
            await _limitedImageHandle.Task;

            if (_limitedImageHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _limitedRewardImage.sprite = _limitedImageHandle.Result;
                _limitedRewardTMP.text = data.ItemName;
            }
            else
                Debug.LogWarning("이미지 로드 실패");
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
        _contentPresenter = null;
    }

    public void Play()
    {
        if (IsTransitioning) return;
        IsTransitioning = true;

        _closedCover.SetActive(false);
        _openedCover.SetActive(true);

        bool isHighRank = _grade <= 3;
        _highRankGroup.SetActive(isHighRank);
        _lowRankGroup.SetActive(!isHighRank);

        if (isHighRank)
        {
            _highRankTMP.text = $"{_grade}등";
        }
        else
        {
            _lowRankTMP.text = $"{_grade}등";
        }

        if (_isLimitedItem)
        {
            _limitedRewardPanel.SetActive(true);

            if (_limitedRewardTween != null)
                _limitedRewardTween.PlayAnimation();
        }

        if (_confirmButton != null)
            _confirmButton.interactable = true;

        IsTransitioning = false;
    }

    void OnTouchReveal()
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

    void OnDestroy()
    {
        Unbind();
    }
}

using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PopupPreview : MonoBehaviour, IPopupable
{
    [Header("참조 필요")]
    [SerializeField] Button _exitButton;
    [SerializeField] Image _resourceImage;
    [SerializeField] TextMeshProUGUI _description;

    // Addressables가 로드 작업을 관리하기 위한 핸들
    private AsyncOperationHandle<Sprite> _resourceImageHandle;

    GatchaContentPresenter _contentPresenter;

    public void Open()
    {
        if (_exitButton != null)
        {
            _exitButton.interactable = true;
        }
    }

    public void Close()
    {
        _description.text = string.Empty;
    }

    public async void SetData(int itemId)
    {
        // id로 데이터 가져오기
        var db = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        var data = db.FindById(itemId.ToString());

        // 이전에 로드한 스프라이트가 있으면 해제
        if (_resourceImageHandle.IsValid()) Addressables.Release(_resourceImageHandle);

        // Addressables에서 스프라이트 비동기 로드
        _resourceImageHandle = Addressables.LoadAssetAsync<Sprite>(data.RewardResourceImage);

        // 로드 완료 대기
        await _resourceImageHandle.Task;

        // 로드 실패 시 처리
        if (_resourceImageHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning($"스프라이트 로드 실패: {data.RewardResourceImage}");
            return;
        }

        // db 기준 이미지 교체
        _resourceImage.sprite = _resourceImageHandle.Result;
        // 아이템 설명 교체
        _description.text = data.Description;
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

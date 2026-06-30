using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Comment_Zone 내 개별 단어 버튼 컴포넌트.
///
/// [추후 드래그 앤 드롭 순서 변경 구현 시 작업 위치]
///   1. implements 에 IBeginDragHandler, IDragHandler, IEndDragHandler 추가
///   2. OnBeginDrag: ghost 오브젝트 생성, 원본 sibling index 저장
///   3. OnDrag: ghost 를 포인터 위치로 이동, 인접 버튼과 sibling index 비교해 플레이스홀더 이동
///   4. OnEndDrag: 플레이스홀더 위치로 확정, ghost 삭제
///   5. CommentZoneManager.ReorderWord(this, targetIndex) 호출로 정렬 동기화
///   ※ Button 컴포넌트의 onClick 은 포인터가 드래그 중 버튼 영역을 벗어나면
///      자동으로 발동하지 않으므로 드래그와 클릭 이벤트가 자연스럽게 분리됨
/// </summary>
[RequireComponent(typeof(Button), typeof(Image))]
public class CommentWordButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Button _deleteBtn;
    [SerializeField] private GameObject _deleteBtnRoot;

    [Header("Appearance")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    private string _word;
    private CommentZoneManager _manager;
    private bool _isSelected;

    public string Word => _word;
    public bool IsSelected => _isSelected; // DraggableUI에서 드래그 허용 여부 판단에 사용

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        _deleteBtn.onClick.AddListener(OnDeleteClicked);
    }

    // <summary>버튼 생성 직후 한 번만 호출. word와 manager를 바인딩한다.</summary>
    public void Init(string word, CommentZoneManager manager)
    {
        _word = word;
        _manager = manager;
        _label.text = word;
        SetSelected(false);
    }

    // <summary>선택 상태를 갱신한다. CommentZoneManager에서만 호출한다.</summary>
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _deleteBtnRoot.SetActive(selected);
        if (_buttonImage != null)
            _buttonImage.sprite = selected ? _selectedSprite : _normalSprite;
    }

    private void OnClick()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Mouth_High_Sharp_1);

        if (_isSelected)
            _manager.DeselectAll();
        else
            _manager.Select(this);
    }

    private void OnDeleteClicked()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.Popup4b);

        _manager.RemoveWord(this);
    }
}

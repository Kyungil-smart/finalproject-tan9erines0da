using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Comment_Zone 관리자.
// 단어 버튼 추가/삭제/선택 상태를 관리하고,
// 선택 상태 변경 시 OnSelectionChanged 를 발행해 DeselectOverlay 등에 알린다.
public class CommentZoneManager : MonoBehaviour
{
    public static CommentZoneManager Instance { get; private set; }

    // 선택 상태가 바뀔 때 발행. DeselectOverlay / HashtagSelectPanel 이 구독한다.
    public static event Action OnSelectionChanged;

    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _wordButtonPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxChars = 50;
    [SerializeField] private int _maxSameWord = 3; // 동일 단어 최대 배치 횟수

    [Header("Popups")]
    [SerializeField] private GameObject _wordLimitPopup;  // 동일 단어 제한 초과 시 표시할 팝업
    [SerializeField] private GameObject _charLimitPopup;  // 글자수 제한 초과 시 표시할 팝업
    [SerializeField] private GameObject _noContentPopup;  // 코멘트·해시태그 둘 다 없을 때 표시할 팝업

    private int _totalChars;
    private CommentWordButton _selectedButton;

    // 단어별 현재 배치 횟수를 추적한다.
    private readonly Dictionary<string, int> _wordCounts = new Dictionary<string, int>();

    public bool HasSelection => _selectedButton != null;

    private void Awake() => Instance = this;

    private void Start() => UpdateCountText();

    public bool TryAddWord(string word)
    {
        word = word.Trim();
        if (word.Length == 0) return false;
        if (_totalChars + word.Length > _maxChars)
        {
            _charLimitPopup?.SetActive(true);
            return false;
        }

        // 동일 단어가 이미 최대 횟수만큼 배치됐으면 팝업 표시 후 차단
        _wordCounts.TryGetValue(word, out int count);
        if (count >= _maxSameWord)
        {
            _wordLimitPopup?.SetActive(true);
            return false;
        }

        _wordCounts[word] = count + 1;
        _totalChars += word.Length;
        UpdateCountText();
        CreateWordButton(word);
        return true;
    }

    // 동일 단어 제한 팝업 닫기 버튼에 연결한다.
    public void CloseWordLimitPopup() => _wordLimitPopup?.SetActive(false);

    // 글자수 제한 팝업 닫기 버튼에 연결한다.
    public void CloseCharLimitPopup() => _charLimitPopup?.SetActive(false);

    // 콘텐츠 없음 팝업을 표시한다. PlzAddComment()에서 호출한다.
    public void ShowNoContentPopup() => _noContentPopup?.SetActive(true);

    // 콘텐츠 없음 팝업 닫기 버튼에 연결한다.
    public void CloseNoContentPopup() => _noContentPopup?.SetActive(false);

    // 특정 버튼을 선택 상태로 전환한다.
    public void Select(CommentWordButton button)
    {
        if (_selectedButton != null)
            _selectedButton.SetSelected(false);

        _selectedButton = button;
        _selectedButton.SetSelected(true);
        OnSelectionChanged?.Invoke();
    }

    // 현재 선택을 해제한다.
    public void DeselectAll()
    {
        if (_selectedButton == null) return;
        _selectedButton.SetSelected(false);
        _selectedButton = null;
        OnSelectionChanged?.Invoke();
    }

    // 버튼을 제거하고 글자수 및 단어 카운트를 갱신한다.
    public void RemoveWord(CommentWordButton button)
    {
        _totalChars -= button.Word.Length;
        if (_totalChars < 0) _totalChars = 0;

        if (_wordCounts.TryGetValue(button.Word, out int count))
        {
            if (count <= 1) _wordCounts.Remove(button.Word);
            else _wordCounts[button.Word] = count - 1;
        }

        if (_selectedButton == button)
        {
            _selectedButton = null;
            OnSelectionChanged?.Invoke();
        }

        Destroy(button.gameObject);
        UpdateCountText();
    }

    // 현재 Content 의 자식 순서대로 단어 목록을 반환한다.
    // 드래그 앤 드롭으로 순서가 바뀐 이후에도 동일하게 동작한다.
    public List<string> GetWords()
    {
        var words = new List<string>();
        foreach (Transform child in _content)
        {
            var btn = child.GetComponent<CommentWordButton>();
            if (btn != null) words.Add(btn.Word);
        }
        return words;
    }

    // [드래그 앤 드롭 전용] 버튼의 sibling index 를 변경해 순서를 바꾼다.
    // CommentWordButton.OnEndDrag 에서 호출 예정.
    public void ReorderWord(CommentWordButton button, int targetIndex)
    {
        button.transform.SetSiblingIndex(targetIndex);
    }

    // Comment_Zone의 모든 단어 버튼을 한 번에 제거한다.
    public void ClearAll()
    {
        foreach (Transform child in _content)
            Destroy(child.gameObject);

        _totalChars = 0;
        _wordCounts.Clear();
        _selectedButton = null;
        OnSelectionChanged?.Invoke();
        UpdateCountText();
    }

    private void CreateWordButton(string word)
    {
        var go = Instantiate(_wordButtonPrefab, _content);
        var btn = go.GetComponent<CommentWordButton>();
        btn.Init(word, this);
    }

    private void UpdateCountText()
    {
        if (_countText != null)
            _countText.text = $"{_totalChars}/{_maxChars}";
    }
}

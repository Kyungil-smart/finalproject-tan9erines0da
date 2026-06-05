using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Comment_Zone 관리자.
/// 단어 버튼 추가/삭제/선택 상태를 관리하고,
/// 선택 상태 변경 시 OnSelectionChanged 를 발행해 DeselectOverlay 등에 알린다.
/// </summary>
public class CommentZoneManager : MonoBehaviour
{
    public static CommentZoneManager Instance { get; private set; }

    /// <summary>선택 상태가 바뀔 때 발행. DeselectOverlay / HashtagSelectPanel 이 구독한다.</summary>
    public static event Action OnSelectionChanged;

    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _wordButtonPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxChars = 50;

    private int _totalChars;
    private CommentWordButton _selectedButton;

    public bool HasSelection => _selectedButton != null;

    private void Awake() => Instance = this;

    private void Start() => UpdateCountText();

    // ── 외부 인터페이스 ──────────────────────────────────────────────────

    /// <summary>
    /// word 를 Comment_Zone 에 추가한다.
    /// 글자수 초과 시 false 반환.
    /// </summary>
    public bool TryAddWord(string word)
    {
        word = word.Trim();
        if (word.Length == 0) return false;
        if (_totalChars + word.Length > _maxChars) return false;

        _totalChars += word.Length;
        UpdateCountText();
        CreateWordButton(word);
        return true;
    }

    /// <summary>특정 버튼을 선택 상태로 전환한다.</summary>
    public void Select(CommentWordButton button)
    {
        if (_selectedButton != null)
            _selectedButton.SetSelected(false);

        _selectedButton = button;
        _selectedButton.SetSelected(true);
        OnSelectionChanged?.Invoke();
    }

    /// <summary>현재 선택을 해제한다.</summary>
    public void DeselectAll()
    {
        if (_selectedButton == null) return;
        _selectedButton.SetSelected(false);
        _selectedButton = null;
        OnSelectionChanged?.Invoke();
    }

    /// <summary>버튼을 제거하고 글자수를 갱신한다.</summary>
    public void RemoveWord(CommentWordButton button)
    {
        _totalChars -= button.Word.Length;
        if (_totalChars < 0) _totalChars = 0;

        if (_selectedButton == button)
        {
            _selectedButton = null;
            OnSelectionChanged?.Invoke();
        }

        Destroy(button.gameObject);
        UpdateCountText();
    }

    /// <summary>
    /// 현재 Content 의 자식 순서대로 단어 목록을 반환한다.
    /// 드래그 앤 드롭으로 순서가 바뀐 이후에도 동일하게 동작한다.
    /// </summary>
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

    /// <summary>
    /// [드래그 앤 드롭 전용] 버튼의 sibling index 를 변경해 순서를 바꾼다.
    /// CommentWordButton.OnEndDrag 에서 호출 예정.
    /// </summary>
    public void ReorderWord(CommentWordButton button, int targetIndex)
    {
        button.transform.SetSiblingIndex(targetIndex);
    }

    // ── 내부 ────────────────────────────────────────────────────────────

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

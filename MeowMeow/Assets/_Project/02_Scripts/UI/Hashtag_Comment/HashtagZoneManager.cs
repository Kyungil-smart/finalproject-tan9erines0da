using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hashtag_Zone 관리자.
/// 해시태그는 선택(X 표시) 상태 없이, 소스 버튼 재클릭으로 추가/제거한다.
/// 최대 _maxTags 개까지 추가 가능하며, 카운트는 글자수가 아닌 개수 기준이다.
/// </summary>
public class HashtagZoneManager : MonoBehaviour
{
    public static HashtagZoneManager Instance { get; private set; }

    /// <summary>
    /// 태그 추가/삭제 시 발행. HashtagSelectPanel 이 소스 버튼 색상 갱신에 사용한다.
    /// DeselectOverlay 도 구독하지만 HasSelection 이 항상 false 이므로 overlay 는 비활성 유지됨.
    /// </summary>
    public static event Action OnSelectionChanged;

    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _tagButtonPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxTags = 5;

    private readonly Dictionary<string, GameObject> _tagObjects = new();

    /// <summary>Hashtag 는 선택 상태가 없으므로 항상 false. DeselectOverlay 용.</summary>
    public bool HasSelection => false;

    private void Awake() => Instance = this;

    private void Start() => UpdateCountText();

    // ── 외부 인터페이스 ──────────────────────────────────────────────────

    /// <summary>id 로 현재 추가 여부를 확인한다.</summary>
    public bool IsSelected(string id) => _tagObjects.ContainsKey(id);

    /// <summary>
    /// 해시태그를 추가한다.
    /// 이미 추가됐거나 최대 개수 초과 시 false 반환.
    /// </summary>
    public bool TryAddHashtag(string id, string tagName)
    {
        if (_tagObjects.ContainsKey(id)) return false;
        if (_tagObjects.Count >= _maxTags) return false;

        CreateTagButton(id, tagName);
        UpdateCountText();
        OnSelectionChanged?.Invoke();
        return true;
    }

    /// <summary>해시태그를 제거한다.</summary>
    public void RemoveHashtag(string id)
    {
        if (!_tagObjects.TryGetValue(id, out var go)) return;
        _tagObjects.Remove(id);
        Destroy(go);
        UpdateCountText();
        OnSelectionChanged?.Invoke();
    }

    /// <summary>호환성 유지용. Hashtag 는 선택 상태가 없으므로 아무 것도 하지 않는다.</summary>
    public void DeselectAll() { }

    /// <summary>현재 추가된 태그 이름 목록을 반환한다.</summary>
    public List<string> GetSelectedTagNames()
    {
        var names = new List<string>();
        foreach (var go in _tagObjects.Values)
            names.Add(go.name);
        return names;
    }

    // ── 내부 ────────────────────────────────────────────────────────────

    private void CreateTagButton(string id, string tagName)
    {
        var go = Instantiate(_tagButtonPrefab, _content);
        go.name = tagName;
        _tagObjects[id] = go;
        go.GetComponentInChildren<TextMeshProUGUI>().text = tagName;
    }

    private void UpdateCountText()
    {
        if (_countText != null)
            _countText.text = $"{_tagObjects.Count}/{_maxTags}";
    }
}

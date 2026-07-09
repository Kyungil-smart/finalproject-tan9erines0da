using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Hashtag_Zone 관리자.
// 해시태그는 선택(X 표시) 상태 없이, 소스 버튼 재클릭으로 추가/제거한다.
// 최대 _maxTags 개까지 추가 가능하며, 카운트는 글자수가 아닌 개수 기준이다.
public class HashtagZoneManager : MonoBehaviour
{
    public static HashtagZoneManager Instance { get; private set; }

    // 태그 추가/삭제 시 발행. HashtagSelectPanel 이 소스 버튼 색상 갱신에 사용한다.
    // DeselectOverlay 도 구독하지만 HasSelection 이 항상 false 이므로 overlay 는 비활성 유지됨
    public static event Action OnSelectionChanged;

    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _tagButtonPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxTags = 5;

    private readonly Dictionary<string, GameObject> _tagObjects = new();

    //Hashtag 는 선택 상태가 없으므로 항상 false. DeselectOverlay 용.
    public bool HasSelection => false;

    private void Awake() => Instance = this;

    private void Start() => UpdateCountText();


    // id 로 현재 추가 여부를 확인한다.
    public bool IsSelected(string id) => _tagObjects.ContainsKey(id);

   
    // 해시태그를 추가한다.
    // 이미 추가됐거나 최대 개수 초과 시 false 반환.
    public bool TryAddHashtag(string id, string tagName)
    {
        if (_tagObjects.ContainsKey(id)) return false;
        if (_tagObjects.Count >= _maxTags) return false;

        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Mouth_High_Sharp_1);

        CreateTagButton(id, tagName);
        UpdateCountText();
        OnSelectionChanged?.Invoke();
        return true;
    }

    // 해시태그를 제거한다.
    public void RemoveHashtag(string id)
    {
        if (!_tagObjects.TryGetValue(id, out var go)) return;

        // 효과음
        SoundManager.Instance.Invoke(AudioType.Popup4b);

        _tagObjects.Remove(id);
        Destroy(go);
        UpdateCountText();
        OnSelectionChanged?.Invoke();
    }

    // 호환성 유지용. Hashtag 는 선택 상태가 없으므로 아무 것도 하지 않는다.
    public void DeselectAll() { }

    // Hashtag_Zone의 모든 태그를 한 번에 제거한다.
    public void ClearAll()
    {
        foreach (var go in _tagObjects.Values)
            Destroy(go);

        _tagObjects.Clear();
        OnSelectionChanged?.Invoke();
        UpdateCountText();
    }

    // 현재 추가된 태그 이름 목록을 반환한다.
    public List<string> GetSelectedTagNames()
    {
        var names = new List<string>();
        foreach (var go in _tagObjects.Values)
            names.Add(go.name);
        return names;
    }


    private void CreateTagButton(string id, string tagName)
    {
        var go = Instantiate(_tagButtonPrefab, _content);
        go.name = tagName;
        _tagObjects[id] = go;
        go.GetComponentInChildren<TextMeshProUGUI>().text = tagName;
    }

    private void UpdateCountText()
    {
        if (_countText == null) return;
        int count = _tagObjects.Count;
        _countText.text = $"{count}/{_maxTags}";
        _countText.color = count == 0  ? new Color32(85, 31, 52, 255)
                         : count <= 3  ? new Color32(114, 29, 144, 255)
                                       : new Color32(229, 0, 242, 255);
    }
}

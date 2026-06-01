using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HashtagZoneManager : MonoBehaviour
{
    public static HashtagZoneManager Instance { get; private set; }
    public static event System.Action OnSelectionChanged;

    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _tagButtonPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxTags;

    private readonly Dictionary<string, GameObject> _tagObjects = new Dictionary<string, GameObject>();
    private GameObject _selectedItem;

    private void Awake() => Instance = this;

    private void Start() => UpdateCountText();

    public bool IsSelected(string id) => _tagObjects.ContainsKey(id);

    public bool TryAddHashtag(string id, string tagName)
    {
        if (_tagObjects.ContainsKey(id)) return false;
        if (_tagObjects.Count >= _maxTags) return false;

        CreateTagButton(id, tagName);
        UpdateCountText();
        OnSelectionChanged?.Invoke();
        return true;
    }

    public void RemoveHashtag(string id)
    {
        if (!_tagObjects.TryGetValue(id, out var go)) return;
        if (_selectedItem == go) _selectedItem = null;
        _tagObjects.Remove(id);
        Destroy(go);
        UpdateCountText();
        OnSelectionChanged?.Invoke();
    }

    private void CreateTagButton(string id, string tagName)
    {
        var go = Instantiate(_tagButtonPrefab, _content);
        go.name = tagName;
        _tagObjects[id] = go;

        go.GetComponentInChildren<TextMeshProUGUI>().text = tagName;

        var xGO = go.transform.Find("X").gameObject;
        string capturedId = id;
        go.GetComponent<Button>().onClick.AddListener(() => OnTagButtonClick(go));
        xGO.GetComponent<Button>().onClick.AddListener(() => RemoveHashtag(capturedId));
    }

    private void OnTagButtonClick(GameObject tagGO)
    {
        if (_selectedItem == tagGO) { DeselectAll(); return; }
        DeselectAll();
        _selectedItem = tagGO;
        tagGO.transform.Find("X")?.gameObject.SetActive(true);
    }

    public void DeselectAll()
    {
        if (_selectedItem == null) return;
        _selectedItem.transform.Find("X")?.gameObject.SetActive(false);
        _selectedItem = null;
    }

    public List<string> GetSelectedTagNames()
    {
        var names = new List<string>();
        foreach (var go in _tagObjects.Values)
            names.Add(go.name);
        return names;
    }

    private void UpdateCountText()
    {
        if (_countText != null)
            _countText.text = $"{_tagObjects.Count}/{_maxTags}";
    }

    private void Update()
    {
        if (_selectedItem == null || !Input.GetMouseButtonDown(0)) return;

        var results = new List<RaycastResult>();
        var eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject.transform.IsChildOf(_content)) return;
        }

        DeselectAll();
    }
}

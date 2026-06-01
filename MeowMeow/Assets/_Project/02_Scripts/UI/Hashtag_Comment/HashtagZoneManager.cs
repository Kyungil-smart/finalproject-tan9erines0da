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
    [SerializeField] private TMP_FontAsset _font;

    [Header("Tag Style")]
    [SerializeField] private Color _tagColor;
    [SerializeField] private Color _tagTextColor;
    [SerializeField] private float _tagFontSize;

    [Header("X Button Style")]
    [SerializeField] private Sprite _xButtonSprite;
    [SerializeField] private Vector2 _xButtonOffset;
    [SerializeField] private Vector2 _xButtonSize;

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

    private void UpdateCountText()
    {
        if (_countText != null)
            _countText.text = $"{_tagObjects.Count}/{_maxTags}";
    }

    private void CreateTagButton(string id, string tagName)
    {
        var go = new GameObject(tagName, typeof(RectTransform));
        go.transform.SetParent(_content, false);
        _tagObjects[id] = go;

        var img = go.AddComponent<Image>();
        img.color = _tagColor;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => OnTagButtonClick(go));

        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        var textRt = textGO.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = tagName;
        tmp.fontSize = _tagFontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = _tagTextColor;
        if (_font != null) tmp.font = _font;

        var xGO = CreateXButton(go.transform);
        xGO.SetActive(false);

        string capturedId = id;
        xGO.GetComponent<Button>().onClick.AddListener(() => RemoveHashtag(capturedId));
    }

    private GameObject CreateXButton(Transform parent)
    {
        var xGO = new GameObject("X", typeof(RectTransform));
        xGO.transform.SetParent(parent, false);

        var xRt = xGO.GetComponent<RectTransform>();
        xRt.anchorMin = new Vector2(1f, 1f);
        xRt.anchorMax = new Vector2(1f, 1f);
        xRt.pivot = new Vector2(0.5f, 0.5f);
        xRt.anchoredPosition = _xButtonOffset;
        xRt.sizeDelta = _xButtonSize;

        var xImg = xGO.AddComponent<Image>();
        xImg.sprite = _xButtonSprite;

        var xBtn = xGO.AddComponent<Button>();
        xBtn.targetGraphic = xImg;

        return xGO;
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

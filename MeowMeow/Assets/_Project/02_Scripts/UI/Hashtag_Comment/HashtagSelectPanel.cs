using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HashtagSelectPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private googleSheetManager _sheetManager;
    [SerializeField] private Transform _content;
    [SerializeField] private TMP_FontAsset _font;

    [Header("Button Style")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _textColor;
    [SerializeField] private float _fontSize;

    private readonly Dictionary<string, Image> _buttonImages = new Dictionary<string, Image>();

    private async void Start()
    {
        if (_sheetManager == null)
        {
            Debug.LogError("[HashtagSelectPanel] SheetManager가 연결되지 않았습니다.");
            return;
        }

        await _sheetManager.DataLoadAsync();
        GenerateButtons();
        HashtagZoneManager.OnSelectionChanged += RefreshButtonStates;
    }

    private void OnDestroy()
    {
        HashtagZoneManager.OnSelectionChanged -= RefreshButtonStates;
    }

    private void GenerateButtons()
    {
        foreach (Transform child in _content)
            Destroy(child.gameObject);

        var so = _sheetManager.GetClassData<Hashtag>();
        if (so == null)
        {
            Debug.LogError("[HashtagSelectPanel] HashtagSO를 찾을 수 없습니다.");
            return;
        }

        foreach (var item in so.m_Data)
        {
            if (string.IsNullOrWhiteSpace(item.TagName) || item.TagName == "(Null)") continue;
            string capturedId = item.uniqueId;
            string capturedName = item.TagName;
            CreateButton(capturedId, capturedName);
        }
    }

    private void CreateButton(string id, string tagName)
    {
        var go = new GameObject(tagName, typeof(RectTransform));
        go.transform.SetParent(_content, false);

        var img = go.AddComponent<Image>();
        img.color = _normalColor;
        _buttonImages[id] = img;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => OnButtonClick(id, tagName));

        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        var textRt = textGO.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = tagName;
        tmp.fontSize = _fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = _textColor;
        if (_font != null) tmp.font = _font;
    }

    private void OnButtonClick(string id, string tagName)
    {
        if (HashtagZoneManager.Instance == null) return;

        if (HashtagZoneManager.Instance.IsSelected(id))
            HashtagZoneManager.Instance.RemoveHashtag(id);
        else
            HashtagZoneManager.Instance.TryAddHashtag(id, tagName);
    }

    private void RefreshButtonStates()
    {
        if (HashtagZoneManager.Instance == null) return;

        foreach (var kvp in _buttonImages)
        {
            bool selected = HashtagZoneManager.Instance.IsSelected(kvp.Key);
            kvp.Value.color = selected ? _selectedColor : _normalColor;
        }
    }
}

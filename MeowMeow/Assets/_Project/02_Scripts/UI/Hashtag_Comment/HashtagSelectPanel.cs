using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HashtagSelectPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _buttonPrefab;

    [Header("Button Style")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _selectedColor;

    private readonly Dictionary<string, Image> _buttonImages = new Dictionary<string, Image>();

    private async void Start()
    {
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

        var so = googleSheetManager.instance.GetClassData<Hashtag>();
        if (so == null)
        {
            Debug.LogError("[HashtagSelectPanel] HashtagSO를 찾을 수 없습니다.");
            return;
        }

        foreach (var item in so.m_Data)
        {
            if (string.IsNullOrWhiteSpace(item.TagName) || item.TagName == "(Null)") continue;
            CreateButton(item.uniqueId, item.TagName);
        }
    }

    private void CreateButton(string id, string tagName)
    {
        var go = Instantiate(_buttonPrefab, _content);
        go.name = tagName;

        var img = go.GetComponent<Image>();
        img.color = _normalColor;
        _buttonImages[id] = img;

        go.GetComponentInChildren<TextMeshProUGUI>().text = tagName;
        go.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(id, tagName));
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
            kvp.Value.color = HashtagZoneManager.Instance.IsSelected(kvp.Key)
                ? _selectedColor
                : _normalColor;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HashtagSelectPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _buttonPrefab;

    private readonly Dictionary<string, HashtagSelectButton> _buttonViews = new Dictionary<string, HashtagSelectButton>();

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
            Debug.LogError("HashtagSO를 찾을 수 없습니다.");
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

        var view = go.GetComponent<HashtagSelectButton>();
        view.SetSelected(false);
        _buttonViews[id] = view;

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

        foreach (var kvp in _buttonViews)
        {
            kvp.Value.SetSelected(HashtagZoneManager.Instance.IsSelected(kvp.Key));
        }
    }
}

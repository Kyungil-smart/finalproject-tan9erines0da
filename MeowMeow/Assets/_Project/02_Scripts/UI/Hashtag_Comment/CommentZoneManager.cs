using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CommentZoneManager : MonoBehaviour
{
    public static CommentZoneManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _wordButtonPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxChars;

    private int _totalChars = 0;
    private GameObject _selectedItem = null;

    private void Awake() => Instance = this;

    private void Start() => UpdateCountText();

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

    public bool TryAddWord(string word)
    {
        word = word.Trim();
        if (_totalChars + word.Length > _maxChars) return false;

        _totalChars += word.Length;
        UpdateCountText();
        CreateWordButton(word);
        return true;
    }

    private void CreateWordButton(string word)
    {
        var go = Instantiate(_wordButtonPrefab, _content);
        go.name = word;
        go.GetComponentInChildren<TextMeshProUGUI>().text = word;

        var xGO = go.transform.Find("X").gameObject;
        string capturedWord = word;
        go.GetComponent<Button>().onClick.AddListener(() => OnWordButtonClick(go, xGO));
        xGO.GetComponent<Button>().onClick.AddListener(() => RemoveWord(go, capturedWord));
    }

    private void OnWordButtonClick(GameObject wordGO, GameObject xGO)
    {
        if (_selectedItem == wordGO) { DeselectAll(); return; }
        DeselectAll();
        _selectedItem = wordGO;
        xGO.SetActive(true);
    }

    public void DeselectAll()
    {
        if (_selectedItem == null) return;
        _selectedItem.transform.Find("X")?.gameObject.SetActive(false);
        _selectedItem = null;
    }

    private void RemoveWord(GameObject wordGO, string word)
    {
        _totalChars -= word.Length;
        if (_totalChars < 0) _totalChars = 0;
        UpdateCountText();
        if (_selectedItem == wordGO) _selectedItem = null;
        Destroy(wordGO);
    }

    public List<string> GetWords()
    {
        var words = new List<string>();
        foreach (Transform child in _content)
            words.Add(child.name);
        return words;
    }

    private void UpdateCountText()
    {
        if (_countText != null)
            _countText.text = $"{_totalChars}/{_maxChars}";
    }
}

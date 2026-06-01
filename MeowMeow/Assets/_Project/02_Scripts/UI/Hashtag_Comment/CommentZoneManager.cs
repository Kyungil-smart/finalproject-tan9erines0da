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
    [SerializeField] private TMP_FontAsset _font;

    [Header("Button Style")]
    [SerializeField] private Color _buttonColor;
    [SerializeField] private Color _textColor;
    [SerializeField] private float _buttonFontSize;

    [Header("X Button Style")]
    [SerializeField] private Sprite _xButtonSprite;
    [SerializeField] private Vector2 _xButtonOffset;
    [SerializeField] private Vector2 _xButtonSize;

    [Header("Settings")]
    [SerializeField] private int _maxChars;

    private int _totalChars = 0;
    private GameObject _selectedItem = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateCountText();
    }

    private void Update()
    {
        if (_selectedItem == null) return;
        if (!Input.GetMouseButtonDown(0)) return;

        var results = new List<RaycastResult>();
        var eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject.transform.IsChildOf(_content))
                return;
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
        var go = new GameObject(word, typeof(RectTransform));
        go.transform.SetParent(_content, false);

        var img = go.AddComponent<Image>();
        img.color = _buttonColor;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        var textRt = textGO.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = word;
        tmp.fontSize = _buttonFontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = _textColor;
        if (_font != null) tmp.font = _font;

        var xGO = CreateXButton(go.transform);
        xGO.SetActive(false);

        string capturedWord = word;
        btn.onClick.AddListener(() => OnWordButtonClick(go, xGO));
        xGO.GetComponent<Button>().onClick.AddListener(() => RemoveWord(go, capturedWord));
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

    private void OnWordButtonClick(GameObject wordGO, GameObject xGO)
    {
        if (_selectedItem == wordGO)
        {
            DeselectAll();
            return;
        }

        DeselectAll();
        _selectedItem = wordGO;
        xGO.SetActive(true);
    }

    public void DeselectAll()
    {
        if (_selectedItem == null) return;
        var x = _selectedItem.transform.Find("X");
        if (x != null) x.gameObject.SetActive(false);
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

    private void UpdateCountText()
    {
        if (_countText != null)
            _countText.text = $"{_totalChars}/{_maxChars}";
    }
}

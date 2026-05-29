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
    [SerializeField] private Color _buttonColor = new Color(0.816f, 0.631f, 0.766f);
    [SerializeField] private Color _textColor = new Color(0.27f, 0.16f, 0.39f);
    [SerializeField] private Color _xButtonColor = new Color(0.53f, 0.29f, 0.76f);
    [SerializeField] private float _cellHeight = 80f;

    private const int MaxChars = 50;
    private int _totalChars = 0;
    private GameObject _selectedItem = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupContentLayout();
        UpdateCountText();
    }

    private void SetupContentLayout()
    {
        // Viewport(Content 부모)를 Comment_Zone 전체 영역으로 늘림
        if (_content.parent != null)
        {
            var viewportRt = _content.parent.GetComponent<RectTransform>();
            viewportRt.anchorMin = Vector2.zero;
            viewportRt.anchorMax = Vector2.one;
            viewportRt.sizeDelta = Vector2.zero;
            viewportRt.anchoredPosition = Vector2.zero;
        }

        // ScrollRect에 Viewport와 Content 자동 연결
        var scrollRect = GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            if (scrollRect.viewport == null && _content.parent != null)
                scrollRect.viewport = _content.parent.GetComponent<RectTransform>();
            if (scrollRect.content == null)
                scrollRect.content = _content.GetComponent<RectTransform>();
        }

        // Content: 위에서 아래로 쌓이도록 앵커/피벗 고정
        var contentRt = _content.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.anchoredPosition = Vector2.zero;
        contentRt.sizeDelta = new Vector2(0f, 0f);

        Canvas.ForceUpdateCanvases();

        float contentWidth = contentRt.rect.width;
        if (contentWidth <= 0 && _content.parent != null)
        {
            var parentRt = _content.parent.GetComponent<RectTransform>();
            if (parentRt != null) contentWidth = parentRt.rect.width;
        }
        if (contentWidth <= 0) contentWidth = 400f;

        const float paddingSide = 10f;
        const float spacingX = 8f;
        float cellWidth = Mathf.Floor((contentWidth - paddingSide * 2f - spacingX * 2f) / 3f);

        var grid = _content.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = _content.gameObject.AddComponent<GridLayoutGroup>();
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.cellSize = new Vector2(cellWidth, _cellHeight);
        grid.spacing = new Vector2(spacingX, 8f);
        grid.padding = new RectOffset((int)paddingSide, (int)paddingSide, 10, 10);

        var fitter = _content.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = _content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
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
        if (_totalChars + word.Length > MaxChars) return false;

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
        tmp.fontSize = 28f;
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
        xRt.anchoredPosition = new Vector2(10f, 10f);
        xRt.sizeDelta = new Vector2(26f, 26f);

        var xImg = xGO.AddComponent<Image>();
        xImg.color = _xButtonColor;

        var xBtn = xGO.AddComponent<Button>();
        xBtn.targetGraphic = xImg;

        var xTextGO = new GameObject("Text", typeof(RectTransform));
        xTextGO.transform.SetParent(xGO.transform, false);
        var xTextRt = xTextGO.GetComponent<RectTransform>();
        xTextRt.anchorMin = Vector2.zero;
        xTextRt.anchorMax = Vector2.one;
        xTextRt.offsetMin = Vector2.zero;
        xTextRt.offsetMax = Vector2.zero;
        var xTmp = xTextGO.AddComponent<TextMeshProUGUI>();
        xTmp.text = "×";
        xTmp.alignment = TextAlignmentOptions.Center;
        xTmp.fontSize = 18f;
        xTmp.color = Color.white;
        if (_font != null) xTmp.font = _font;

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
            _countText.text = $"{_totalChars}/50";
    }
}

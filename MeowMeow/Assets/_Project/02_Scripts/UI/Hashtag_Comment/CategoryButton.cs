using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CategoryButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _wordPanel;
    [SerializeField] private CanvasGroup _wordPanelGroup;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Transform _content;
    [SerializeField] private googleSheetManager _sheetManager;
    [SerializeField] private TMP_FontAsset _font;

    [Header("Category")]
    [SerializeField] private CommentType _commentType;

    [Header("Button Style")]
    [SerializeField] private Color _buttonColor = Color.white;
    [SerializeField] private Color _textColor = new Color(0.27f, 0.16f, 0.39f);
    [SerializeField] private float _fontSize = 36f;

    [Header("Animation")]
    [SerializeField] private float _animDuration = 0.3f;
    [SerializeField] private Vector2 _selectedPosition = new Vector2(0f, 60f);

    private RectTransform _rt;
    private Vector2 _originalPosition;
    private float _panelHeight;
    private bool _isExpanded;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _originalPosition = _rt.anchoredPosition;

        _panelHeight = _wordPanel.sizeDelta.y;

        _wordPanel.sizeDelta = new Vector2(_wordPanel.sizeDelta.x, 0f);
        _wordPanelGroup.alpha = 0f;
        _wordPanel.gameObject.SetActive(false);

        // WordPanel이 1Button보다 먼저 렌더링되도록 고정
        _wordPanel.SetAsFirstSibling();

        _button.onClick.AddListener(OnClick);
    }

    private async void Start()
    {
        if (_sheetManager == null)
        {
            Debug.LogError("[CategoryButton] Sheet Manager가 인스펙터에 연결되지 않았습니다.");
            return;
        }
        if (_content == null)
        {
            Debug.LogError("[CategoryButton] Content가 인스펙터에 연결되지 않았습니다.");
            return;
        }

        await _sheetManager.DataLoadAsync();
        GenerateItems();
    }

    private void GenerateItems()
    {
        foreach (Transform child in _content)
            Destroy(child.gameObject);

        var so = _sheetManager.GetClassData<comment>();
        if (so == null)
        {
            Debug.LogError("[CategoryButton] comment SO를 찾을 수 없습니다.");
            return;
        }

        var filtered = new List<comment>();
        foreach (var item in so.m_Data)
        {
            if (item.type == _commentType)
                filtered.Add(item);
        }

        filtered.Sort((a, b) => string.Compare(a.uniqueId, b.uniqueId));

        for (int i = 0; i < filtered.Count; i++)
        {
            string word = filtered[i].word;

            var go = new GameObject($"{i + 1}_Image", typeof(RectTransform));
            go.transform.SetParent(_content, false);

            var img = go.AddComponent<Image>();
            img.color = _buttonColor;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            string capturedWord = word;
            btn.onClick.AddListener(() =>
            {
                CommentZoneManager.Instance?.DeselectAll();
                CommentZoneManager.Instance?.TryAddWord(capturedWord);
            });

            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(go.transform, false);

            var textRt = textGO.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = word;
            tmp.fontSize = _fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = _textColor;
            if (_font != null) tmp.font = _font;
        }
    }

    private void OnClick()
    {
        CommentZoneManager.Instance?.DeselectAll();
        if (_isExpanded) Collapse();
        else Expand();
    }

    private void Expand()
    {
        _isExpanded = true;

        // CategoryButtonUnit 전체를 마지막으로 이동 → 다른 유닛들 위에 렌더링
        // WordPanel은 Awake에서 FirstSibling 고정이라 1Button보다 아래에 렌더링됨
        transform.parent.SetAsLastSibling();

        _wordPanel.gameObject.SetActive(true);

        if (_scrollRect != null && _scrollRect.content != null)
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.content.anchoredPosition = new Vector2(
                _scrollRect.content.anchoredPosition.x, 0f);
        }

        _rt.DOAnchorPos(_selectedPosition, _animDuration).SetEase(Ease.OutCubic);
        _wordPanel.DOSizeDelta(new Vector2(_wordPanel.sizeDelta.x, _panelHeight), _animDuration).SetEase(Ease.OutCubic);
        _wordPanelGroup.DOFade(1f, _animDuration);
    }

    private void Collapse()
    {
        _isExpanded = false;

        _rt.DOAnchorPos(_originalPosition, _animDuration).SetEase(Ease.OutCubic);
        _wordPanel.DOSizeDelta(new Vector2(_wordPanel.sizeDelta.x, 0f), _animDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => _wordPanel.gameObject.SetActive(false));
        _wordPanelGroup.DOFade(0f, _animDuration);
    }
}

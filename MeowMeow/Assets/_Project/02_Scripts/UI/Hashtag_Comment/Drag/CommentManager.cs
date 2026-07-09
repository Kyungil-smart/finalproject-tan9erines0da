using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentManager : MonoBehaviour
{
    public static CommentManager Instance { get; private set; }

    [SerializeField] public ScrollRect _scrollRect;
    [SerializeField] public float _scrollspeed = 1f;

    [SerializeField] public RectTransform _scrollUpArea;
    [SerializeField] public RectTransform _scrollDownArea;

    private bool _isAutoScrolling;
    private bool _scrollUp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetAutoScroll(bool enable, bool scrollUp)
    {
        _isAutoScrolling = enable;
        _scrollUp = scrollUp;
    }

    private void Update()
    {
        if (!_isAutoScrolling) return;

        if (_scrollUp)
        {
            _scrollRect.verticalNormalizedPosition += _scrollspeed * Time.deltaTime;
        }
        else
        {
            _scrollRect.verticalNormalizedPosition -= _scrollspeed * Time.deltaTime;
        }

        _scrollRect.verticalNormalizedPosition =
            Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
    }
}

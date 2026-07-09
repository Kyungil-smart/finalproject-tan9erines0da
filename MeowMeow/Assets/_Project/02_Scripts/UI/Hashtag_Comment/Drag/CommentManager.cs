using UnityEngine;
using UnityEngine.UI;

public class CommentManager : MonoBehaviour
{
    public static CommentManager Instance { get; private set; }

    [Header("Comment_Zone을 참조해 주세요")]
    public ScrollRect _scrollRect;
    [Header("스크롤 속도를 조절해 주세요")]
    public float _scrollspeed = 1f;

    [Header("Up_Image를 참조해 주세요")]
    public RectTransform _scrollUpArea;
    [Header("Down_Image를 참조해 주세요")]
    public RectTransform _scrollDownArea;
    [Header("Area_Image를 참조해 주세요")]
    public RectTransform _scrollActiveArea;

    // 자동 스크롤 활성화 여부
    private bool _isAutoScrolling;
    // 자동 스크롤 방향 (true: 위쪽, false: 아래쪽)
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

    /// <summary>
    /// 자동 스크롤 활성화 여부와 방향을 설정합니다.
    /// </summary>
    /// <param name="enable">자동 스크롤 활성화 여부</param>
    /// <param name="scrollUp">true: 위쪽 스크롤, false: 아래쪽 스크롤</param>
    public void SetAutoScroll(bool enable, bool scrollUp)
    {
        _isAutoScrolling = enable;
        _scrollUp = scrollUp;
    }

    private void Update()
    {
        if (!_isAutoScrolling) return;

        // 설정된 방향에 따라 스크롤 이동
        if (_scrollUp)
        {
            // 위쪽 방향으로 스크롤
            _scrollRect.verticalNormalizedPosition += _scrollspeed * Time.deltaTime;
        }
        else
        {
            // 아래쪽 방향으로 스크롤
            _scrollRect.verticalNormalizedPosition -= _scrollspeed * Time.deltaTime;
        }

        // 스크롤 범위를 0~1 사이로 제한
        _scrollRect.verticalNormalizedPosition =
            Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
    }
}

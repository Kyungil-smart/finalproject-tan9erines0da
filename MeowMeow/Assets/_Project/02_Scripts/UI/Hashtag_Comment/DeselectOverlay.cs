using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 빈 영역 터치 시 Comment_Zone 선택을 해제하는 전체화면 투명 오버레이.
///
/// [씬 계층 배치 규칙]
///   Canvas 의 첫 번째 자식으로 배치해 Content 패널들보다 낮은 sibling index 를 가지게 한다.
///   → Content 패널 내 버튼들이 렌더링 상 위에 있어 해당 버튼 클릭 시 오버레이가 가로채지 않음.
///
/// [동작 원리]
///   CommentZoneManager.HasSelection 이 true 일 때만 CanvasGroup.blocksRaycasts = true 로 전환.
///   빈 영역 클릭 → OnPointerDown → 전체 Deselect.
///   HashtagZoneManager.HasSelection 은 항상 false 이므로 Hashtag 선택에는 관여하지 않음.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DeselectOverlay : MonoBehaviour, IPointerDownHandler
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private void OnEnable()
    {
        CommentZoneManager.OnSelectionChanged += RefreshRaycastBlock;
        HashtagZoneManager.OnSelectionChanged += RefreshRaycastBlock;
    }

    private void OnDisable()
    {
        CommentZoneManager.OnSelectionChanged -= RefreshRaycastBlock;
        HashtagZoneManager.OnSelectionChanged -= RefreshRaycastBlock;
    }

    private void RefreshRaycastBlock()
    {
        bool anySelected = CommentZoneManager.Instance != null && CommentZoneManager.Instance.HasSelection;
        _canvasGroup.blocksRaycasts = anySelected;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CommentZoneManager.Instance?.DeselectAll();
        HashtagZoneManager.Instance?.DeselectAll();
    }
}

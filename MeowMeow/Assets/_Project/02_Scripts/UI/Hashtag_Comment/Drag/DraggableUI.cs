using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Canvas rootCanvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject placeholder;
    private Canvas _dragCanvas;

    // 드래그 시작 전 오브젝트의 원래 하이어라키 순서(Sibling Index)를 저장
    private int _originalSiblingIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var commentBtn = GetComponent<CommentWordButton>();
        if (commentBtn != null && !commentBtn.IsSelected)
        {
            eventData.pointerDrag = null;
            return;
        }

        originalParent = transform.parent;

        // 드래그 시작 시 현재 오브젝트의 하이어라키 순서 저장
        _originalSiblingIndex = transform.GetSiblingIndex();

        CreatePlaceholder();

        transform.SetParent(rootCanvas.transform);
        transform.SetAsLastSibling();

        // 씬 내 모든 Canvas보다 위에 렌더링되도록 오버라이드
        _dragCanvas = gameObject.AddComponent<Canvas>();
        _dragCanvas.overrideSorting = true;
        _dragCanvas.sortingOrder = 20;
        gameObject.AddComponent<GraphicRaycaster>();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placeholder == null) return;

        if (!RectTransformUtility.RectangleContainsScreenPoint(
           CommentManager.Instance._scrollActiveArea,
           eventData.position))
        {
            CancelDrag();
            return;
        }

        MoveDraggedObject(eventData);
        UpdatePlaceholderPosition(eventData);
        // 현재 드래그 위치를 기준으로 자동 스크롤 상태 갱신
        UpdateAutoScroll(eventData);
    }

    // 드래그 위치에 따라 자동 스크롤 방향을 설정합니다.
    private void UpdateAutoScroll(PointerEventData eventData)
    {
        // 드래그 위치가 위쪽 스크롤 영역에 있는 경우 위로 자동 스크롤
        if (RectTransformUtility.RectangleContainsScreenPoint(
        CommentManager.Instance._scrollUpArea,
        eventData.position))
        {
            CommentManager.Instance.SetAutoScroll(true, true);
        }
        // 드래그 위치가 아래쪽 스크롤 영역에 있는 경우 아래로 자동 스크롤
        else if (RectTransformUtility.RectangleContainsScreenPoint(
            CommentManager.Instance._scrollDownArea,
            eventData.position))
        {
            CommentManager.Instance.SetAutoScroll(true, false);
        }
        // 어느 영역에도 해당하지 않으면 자동 스크롤 중지
        else
        {
            CommentManager.Instance.SetAutoScroll(false, false);
        }
    }

    private void CancelDrag()
    {
        CommentManager.Instance.SetAutoScroll(false, false);

        if (placeholder == null) return;

        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Mouth_High_Sharp_1);

        Destroy(GetComponent<GraphicRaycaster>());
        Destroy(_dragCanvas);
        _dragCanvas = null;

        transform.SetParent(originalParent);
        // 드래그 시작 전 저장한 원래 하이어라키 순서로 복원
        transform.SetSiblingIndex(_originalSiblingIndex);

        Destroy(placeholder);
        placeholder = null;

        canvasGroup.blocksRaycasts = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CommentManager.Instance.SetAutoScroll(false, false);

        if (placeholder == null) return;

        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Mouth_High_Sharp_1);

        Destroy(GetComponent<GraphicRaycaster>());
        Destroy(_dragCanvas);
        _dragCanvas = null;

        transform.SetParent(originalParent);
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

        Destroy(placeholder);

        canvasGroup.blocksRaycasts = true;
    }

    private void MoveDraggedObject(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPos);
        rectTransform.position = worldPos;
    }

    private void CreatePlaceholder()
    {
        placeholder = Instantiate(gameObject, originalParent);
        placeholder.name = "Placeholder";

        Destroy(placeholder.GetComponent<CommentWordButton>());
        Destroy(placeholder.GetComponent<DraggableUI>());
        Destroy(placeholder.GetComponent<Button>());

        CanvasGroup cg = placeholder.GetComponent<CanvasGroup>();
        cg.alpha = 0.6f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void UpdatePlaceholderPosition(PointerEventData eventData)
    {
        for (int i = 0; i < originalParent.childCount; i++)
        {
            Transform child = originalParent.GetChild(i);

            if (child.gameObject == placeholder)
                continue;

            RectTransform rect = child as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                rect,
                eventData.position))
            {
                placeholder.transform.SetSiblingIndex(i);
                break;
            }
        }
    }
}

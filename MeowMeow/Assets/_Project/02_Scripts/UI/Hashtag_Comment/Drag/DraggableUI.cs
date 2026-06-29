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
        MoveDraggedObject(eventData);
        UpdatePlaceholderPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placeholder == null) return;

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

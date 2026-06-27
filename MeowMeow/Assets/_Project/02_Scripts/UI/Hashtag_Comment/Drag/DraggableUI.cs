using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Transform canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject placeholder;
    private GridLayoutGroup grid;

    private const int COLUMN_COUNT = 3;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 선택(X 표시) 상태가 아니면 드래그 차단
        var commentBtn = GetComponent<CommentWordButton>();
        if (commentBtn != null && !commentBtn.IsSelected)
        {
            eventData.pointerDrag = null;
            return;
        }

        originalParent = transform.parent;
        grid = originalParent.GetComponent<GridLayoutGroup>();

        CreatePlaceholder();

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placeholder == null) return; // 드래그 차단 시 null 가드
        MoveDraggedObject(eventData);
        UpdatePlaceholderPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placeholder == null) return; // 드래그 차단 시 null 가드
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

        Destroy(placeholder);

        canvasGroup.blocksRaycasts = true;
    }

    private void MoveDraggedObject(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    private void CreatePlaceholder()
    {
        // 원본 버튼을 복사해 고스트 이미지로 사용
        placeholder = Instantiate(gameObject, originalParent);
        placeholder.name = "Placeholder";

        // 클릭·드래그 등 기능 컴포넌트 제거
        Destroy(placeholder.GetComponent<CommentWordButton>());
        Destroy(placeholder.GetComponent<DraggableUI>());
        Destroy(placeholder.GetComponent<Button>());

        // 반투명 처리 및 레이캐스트 차단
        CanvasGroup cg = placeholder.GetComponent<CanvasGroup>();
        cg.alpha = 0.4f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex()); // 원래 버튼이 있던 위치에 배치
    }

    private void UpdatePlaceholderPosition(PointerEventData eventData) // 드래그 중 계속 호출되는 함수, Placeholder 위치 갱신용임
    {
        // Content 안의 모든 자식 오브젝트를 순회
        for (int i = 0; i < originalParent.childCount; i++)
        {
            // 현재 검사 중인 자식 오브젝트 가져오기
            Transform child = originalParent.GetChild(i);

            // Placeholder 자기 자신은 검사 대상에서 제외
            if (child.gameObject == placeholder)
                continue;

            // 현재 자식의 RectTransform 가져오기
            RectTransform rect = child as RectTransform;

            // 현재 마우스가 해당 버튼(RectTransform)의 영역 안에 있는지 검사
            if (RectTransformUtility.RectangleContainsScreenPoint(
                rect,                           // 검사할 UI 영역
                eventData.position))            // 현재 마우스 위치
            {
                // 마우스가 올라간 버튼의 위치로 Placeholder 이동
                // GridLayoutGroup이 자동으로 나머지 버튼들을 밀어냄
                placeholder.transform.SetSiblingIndex(i);

                // 위치를 찾았으므로 더 이상 검사할 필요 없음
                break;
            }
        }
    }
}

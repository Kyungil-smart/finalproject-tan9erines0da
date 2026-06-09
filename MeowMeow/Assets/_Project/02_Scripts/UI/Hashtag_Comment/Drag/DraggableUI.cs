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
        originalParent = transform.parent;
        grid = originalParent.GetComponent<GridLayoutGroup>();

        CreatePlaceholder();

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveDraggedObject(eventData);
        UpdatePlaceholderPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        placeholder = new GameObject("Placeholder"); //GridLayoutGroup 안에서 "임시 자리" 역할을 할 오브젝트 생성, 드래그 중인 버튼이 빠져나간 위치를 유지하기 위해 사용

        placeholder.transform.SetParent(originalParent);

        LayoutElement layout = placeholder.AddComponent<LayoutElement>();

        RectTransform myRect = GetComponent<RectTransform>(); // 현재 드래그 중인 버튼의 RectTransform 가져오기

        layout.preferredWidth = myRect.rect.width; // Placeholder 크기를 원본 버튼과 동일하게 설정 그래야 GridLayoutGroup이 같은 크기의 슬롯으로 취급함
        layout.preferredHeight = myRect.rect.height;

        layout.flexibleWidth = 0; // 크기 자동 확장 방지
        layout.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex()); // 원래 버튼이 있던 위치에 Placeholder 배치
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

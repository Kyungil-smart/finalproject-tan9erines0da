using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private	Transform		canvas;				// UI가 소속되어 있는 최상단의 Canvas Transform
	private	Transform		previousParent;		// 해당 오브젝트가 직전에 소속되어 있었던 부모 Transfron
	private	RectTransform	rect;				// UI 위치 제어를 위한 RectTransform

	private void Awake()
	{
		canvas		= FindObjectOfType<Canvas>().transform;
		rect		= GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		previousParent = transform.parent;

		transform.SetParent(canvas);
		transform.SetAsLastSibling();
	}

	public void OnDrag(PointerEventData eventData)
	{
		rect.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if ( transform.parent == canvas )
		{
			transform.SetParent(previousParent);
			rect.position = previousParent.GetComponent<RectTransform>().position;
		}
	}
}


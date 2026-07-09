using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

// 단어 버튼의 눌림 텍스트 색상 효과만 처리한다.
// EventTrigger는 IBeginDragHandler 등 드래그 인터페이스까지 구현하고 있어
// ScrollRect로 올라가야 할 드래그 이벤트를 가로채 버리므로 사용하지 않는다.
public class WordButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private TextMeshProUGUI _label;
    private Color _normalColor;
    private Color _pressedColor;

    public void Init(TextMeshProUGUI label, Color normalColor, Color pressedColor)
    {
        _label = label;
        _normalColor = normalColor;
        _pressedColor = pressedColor;
    }

    public void OnPointerDown(PointerEventData eventData) => _label.color = _pressedColor;
    public void OnPointerUp(PointerEventData eventData) => _label.color = _normalColor;
    public void OnPointerExit(PointerEventData eventData) => _label.color = _normalColor;
}

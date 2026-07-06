using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Hashtag_Block(HB_Content)에 동적 생성되는 개별 해시태그 버튼의 외형.
// 선택 상태에 따른 source image / 텍스트 색상을 인스펙터에서 지정한다.
[RequireComponent(typeof(Image))]
public class HashtagSelectButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _sourceImage;
    [SerializeField] private TextMeshProUGUI _label;

    [Header("Appearance")]
    [SerializeField] private Color _normalImageColor = Color.white;
    [SerializeField] private Color _selectedImageColor = Color.white;
    [SerializeField] private Color _normalTextColor = Color.black;
    [SerializeField] private Color _selectedTextColor = Color.black;

    public Image SourceImage => _sourceImage;

    // HashtagSelectPanel이 선택 상태가 바뀔 때마다 호출한다.
    public void SetSelected(bool selected)
    {
        if (_sourceImage != null)
            _sourceImage.color = selected ? _selectedImageColor : _normalImageColor;
        if (_label != null)
            _label.color = selected ? _selectedTextColor : _normalTextColor;
    }
}

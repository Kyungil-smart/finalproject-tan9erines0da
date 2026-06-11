using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 가상 댓글 한 칸의 UI 요소를 제어하는 
/// 순수 뷰를 위한 컴포넌트입니다.
/// </summary>
public class FakeCommentItem : MonoBehaviour
{
    [Header("하위 UI 컴포넌트 참조")]
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _commentText;

    /// <summary>
    /// 프레젠터로부터 가공된 청정 데이터를 주입받아 
    /// 화면에 즉각 렌더링합니다.
    /// </summary>
    public void SetData(string name, string comment)
    {
        // 1. 닉네임 문자열 방어 대입
        if (_nameText != null)
            _nameText.text = name;

        // 2. 댓글 본문 문자열 방어 대입
        if (_commentText != null)
            _commentText.text = comment;

        // 3. 프로필 아이콘 추가 시 작성
    }

    /// <summary>
    /// 화면에서 명시적으로 비워야 할 때 호출할 초기화 함수입니다.
    /// </summary>
    public void ClearView()
    {
        if (_nameText != null)
            _nameText.text = string.Empty;

        if (_commentText != null)
            _commentText.text = string.Empty;
    }
}

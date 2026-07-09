using UnityEngine;

// CG_Image 클릭 시 전체 화면 오버레이로 이미지를 확대 표시한다.
// Open(snapshot) : 스티커·셰이더 포함 전체 렌더링 후 오버레이 활성화
// Close()        : 오버레이 비활성화 (배경 버튼 OnClick에 연결)
public class CGImageZoomOverlay : MonoBehaviour
{
    [SerializeField] private SNSPostImageRenderer _overlayRenderer;

    public void Open(SNSPostDTO snapshot)
    {
        _overlayRenderer.RenderPreview(snapshot);
        gameObject.SetActive(true);
    }

    public void Close() => gameObject.SetActive(false);
}

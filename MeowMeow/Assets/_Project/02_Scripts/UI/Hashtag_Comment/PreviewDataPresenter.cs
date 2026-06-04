using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreviewDataPresenter : MonoBehaviour, ISNSPanelPresenter
{
    [Header("시각적 이미지/스티커 최종 복원 렌더러")]
    [SerializeField] private SNSPostImageRenderer _postImageRenderer;

    [Header("작업자 텍스트 UI 참조")]
    [SerializeField] private TextMeshProUGUI _hashtagText;
    [SerializeField] private TextMeshProUGUI _commentText;

    private SNSPostDTO _snapshot;

    /// <summary>
    /// UI 병합 씬에서 패널이 SetActive(true)로 켜지는 순간, 
    /// 다른 작업자의 구조처럼 즉시 최신 DTO 컨텍스트를 서버 마스터로부터 요청합니다.
    /// </summary>
    private void OnEnable()
    {
        RequestContext();
    }

    /// <summary>
    /// 전달받은 DTO 구조체를 바탕으로 복원합니다
    /// </summary>
    public void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        _snapshot = snapshot;

        // 이미지 복원
        if (_postImageRenderer != null)
        {
            _postImageRenderer.RenderPreview(_snapshot);
        }

        // 텍스트 복원
        if (_hashtagText != null && _snapshot.Hashtags != null)
        {
            _hashtagText.text = string.Join("  ", _snapshot.Hashtags);
        }

        if (_commentText != null)
        {
            _commentText.text = _snapshot.Comment;
        }
    }

    public void RequestContext()
    {
        if (SubscribeManager.instance == null)
        {
            Debug.LogWarning("[Preview] 아직 SubscribeManager가 준비되지 않았습니다.");
            return;
        }

        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }

    /// <summary>
    /// 현재 화면은 최종 확인 화면이므로 데이터를 더 수정할 여지가 없습니다.
    /// 네트워크 최종 업로드는 추후 분리된 별도 전역 함수나 관제탑에서 
    /// 안전하게 실행할 수 있도록 기본 파이프라인 싱크만 열어둡니다.
    /// </summary>
    public void SubmitContext()
    {
        if (_snapshot.Equals(default(SNSPostDTO))) return;

        // 추후 이 구역 또는 우측 하단 [최종 등록] 버튼 콜백 함수 내부에 
        // FirestoreSNSPostDoc 클래스 포장지를 씌워 업로드하는 로직이 들어갈 예정입니다.

        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);
    }
}

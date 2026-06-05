using System;
using UnityEngine;

/// <summary>
/// Comment 씬의 데이터 흐름 조율자.
/// 구 CommentDataPresenter + CommentScenePublisher 를 하나로 통합.
///
/// OnEnable 시 최신 DTO 를 요청해 미리보기를 복원하고,
/// 완료 버튼에서 SubmitContext() 를 호출해 수정된 DTO 를 중앙 저장소에 밀어 넣는다.
/// </summary>
public class CommentPanelPresenter : MonoBehaviour, ISNSPanelPresenter
{
    [Header("Zone References")]
    [SerializeField] private CommentZoneManager _commentZoneManager;
    [SerializeField] private HashtagZoneManager _hashtagZoneManager;

    [Header("Preview")]
    [SerializeField] private SNSPostImageRenderer _postImageRenderer;

    private SNSPostDTO _snapshot;

    private void OnEnable() => RequestContext();

    // ── ISNSPanelPresenter ───────────────────────────────────────────────

    public void RequestContext()
    {
        if (SubscribeManager.instance == null)
        {
            Debug.LogWarning("[CommentPanelPresenter] SubscribeManager가 없습니다.");
            return;
        }

        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }

    public void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        _snapshot = snapshot;

        if (_postImageRenderer != null)
            _postImageRenderer.RenderPreview(_snapshot);
    }

    /// <summary>
    /// 완료 버튼에 연결한다.
    /// Comment_Zone / Hashtag_Zone 의 현재 값을 DTO 에 기록하고 중앙 저장소를 갱신한다.
    /// </summary>
    public void SubmitContext()
    {
        if (SubscribeManager.instance == null) return;
        if (_snapshot.Equals(default(SNSPostDTO))) return;

        _snapshot.Comment = string.Join(" ", _commentZoneManager.GetWords());
        _snapshot.Hashtags = _hashtagZoneManager.GetSelectedTagNames();

        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);
    }
}

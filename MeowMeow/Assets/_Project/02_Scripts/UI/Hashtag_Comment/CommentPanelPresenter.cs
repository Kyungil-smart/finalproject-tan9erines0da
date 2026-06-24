using System;
using UnityEngine;

// Comment 씬의 데이터 흐름 조율자.
// OnEnable 시 최신 DTO를 요청해 미리보기를 복원하고,
// 완료 버튼에서 SubmitContext()를 호출해 수정된 DTO를 중앙 저장소에 밀어 넣는다.
public class CommentPanelPresenter : MonoBehaviour, ISNSPanelPresenter, ISNSContextReceiver, ISNSPanelClearable
{
    [Header("Zone References")]
    [SerializeField] private CommentZoneManager _commentZoneManager;
    [SerializeField] private HashtagZoneManager _hashtagZoneManager;

    [Header("Preview")]
    [SerializeField] private SNSPostImageRenderer _postImageRenderer;

    [Header("Navigation")]
    [SerializeField] private BaseScreenController _screenController;
    [SerializeField] private UIPanel _previewPanel;

    private SNSPostDTO _snapshot;

    private void OnEnable() => RequestContext();
    private void OnDisable() => ResetAll();

    // ── ISNSPanelPresenter ───────────────────────────────────────────────

    public void RequestContext()
    {
        if (SubscribeManager.instance == null)
        {
            Debug.LogWarning("SubscribeManager가 없습니다.");
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

    // Comment_Zone과 Hashtag_Zone을 동시에 초기화한다.
    public void ResetAll()
    {
        _commentZoneManager?.ClearAll();
        _hashtagZoneManager?.ClearAll();
    }

    // ISNSPanelClearable — 패널 전환/종료 시 SNS_UI_Controller가 호출한다.
    public void ClearPanelContext() => ResetAll();

    // 다음 버튼에 연결한다.
    // 코멘트와 해시태그가 둘 다 비어 있으면 팝업을 띄우고 이동을 차단한다.
    public void PlzAddComment()
    {
        bool hasContent = (_commentZoneManager != null && _commentZoneManager.GetWords().Count > 0)
                       || (_hashtagZoneManager != null && _hashtagZoneManager.GetSelectedTagNames().Count > 0);

        if (!hasContent)
        {
            _commentZoneManager?.ShowNoContentPopup();
            return;
        }

        _screenController?.RequestScreenChange(_previewPanel);
    }

    // 완료 버튼에 연결한다. Comment_Zone / Hashtag_Zone 값을 DTO에 기록하고 중앙 저장소를 갱신한다.
    public void SubmitContext()
    {
        if (SubscribeManager.instance == null) return;

        _snapshot.Comment = string.Join(" ", _commentZoneManager.GetWords());
        _snapshot.Hashtags = _hashtagZoneManager.GetSelectedTagNames();

        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);

        ResetAll();
    }
}

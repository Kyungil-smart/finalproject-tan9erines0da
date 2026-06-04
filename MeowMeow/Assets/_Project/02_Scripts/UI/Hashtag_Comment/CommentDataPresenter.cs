using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommentDataPresenter : MonoBehaviour, ISNSPanelPresenter
{
    [Header("매니저 참조")]
    [SerializeField] private CommentZoneManager _commentZoneManager;
    [SerializeField] private HashtagZoneManager _hashtagZoneManager;


    [Header("미리보기 이미지")]
    [SerializeField] private SNSPostImageRenderer _postImageRenderer;

    private SNSPostDTO _snapshot;

    public void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        _snapshot = snapshot;

        // 이미지 복원
        if (_postImageRenderer != null)
        {
            _postImageRenderer.RenderPreview(_snapshot);
        }
    }

    public void RequestContext()
    {
        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
                    SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }

    public void SubmitContext()
    {
        if (_snapshot.Equals(default(SNSPostDTO))) return;

        // 코멘트 존에서 단어 리스트를 받아와 공백 한 칸 기준으로 자동 합성 기록
        if (_commentZoneManager != null)
        {
            _snapshot.Comment = string.Join(" ", _commentZoneManager.GetWords());
        }

        // 해시태그 존에서 선택 완료된 태그 기록
        if (_hashtagZoneManager != null)
        {
            _snapshot.Hashtags = _hashtagZoneManager.GetSelectedTagNames();
        }

        // 갱신된 구조체를 업데이트
        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);
    }
}

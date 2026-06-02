using System;
using System.Collections.Generic;
using UnityEngine;

public class CommentScenePublisher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CommentZoneManager _commentZoneManager;
    [SerializeField] private HashtagZoneManager _hashtagZoneManager;

    // 등록 버튼에 연결
    public void Publish()
    {
        if (SubscribeManager.instance == null)
        {
            Debug.LogError("SubscribeManager가 없습니다.");
            return;
        }

        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext,
            dto =>
            {
                dto.Comment = string.Join(" ", _commentZoneManager.GetWords());
                dto.Hashtags = _hashtagZoneManager.GetSelectedTagNames();

                SubscribeManager.instance.Publish<SNSPostDTO>(
                    SubscribeType.Update_PostModelData, dto);
            });
    }
}

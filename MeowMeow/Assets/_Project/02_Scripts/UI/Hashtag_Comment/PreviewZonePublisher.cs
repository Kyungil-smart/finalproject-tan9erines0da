using System;
using UnityEngine;
using TMPro;

public class PreviewZonePublisher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _hashtagText;
    [SerializeField] private TextMeshProUGUI _commentText;

    private void OnEnable()
    {
        if (SubscribeManager.instance == null)
        {
            Debug.LogError("SubscribeManager가 없습니다.");
            return;
        }

        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, Populate);
    }

    private void Populate(SNSPostDTO dto)
    {
        if (_hashtagText != null)
            _hashtagText.text = string.Join("  ", dto.Hashtags);

        if (_commentText != null)
            _commentText.text = dto.Comment;
    }
}

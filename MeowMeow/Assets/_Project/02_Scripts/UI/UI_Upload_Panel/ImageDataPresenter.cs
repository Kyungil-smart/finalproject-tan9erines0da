using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDataPresenter : MonoBehaviour
{
    [Header("참조 필수")]
    [SerializeField] Image _previewImage;
    [SerializeField] CGImageDatabase _dB;

    private SNSPostDTO _snapshot;

    void OnEnable()
    {
        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }


    /// <summary>
    /// 선택한 이미지(프리뷰)를 SNS 데이터 프레젠터에 반영 및 저장 요청합니다
    /// </summary>
    public void SubmitCGIndex()
    {
        // 스냅샷을 프리뷰의 이미지 인덱스로 수정합니다
        _snapshot.ImageIndex = GetIndexSprite();

        // 데이터 프레젠터에게 스냅샷을 담아 발행합니다
        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);
    }

    // DTO를 초기화합니다
    private void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        _snapshot = snapshot;
    }

    //프리뷰 이미지에 적용된 스프라이트의 인덱스를 반환하는 배부 함수
    int GetIndexSprite()
    {
        int index = -1;
        Sprite sprite = _previewImage.sprite;

        for (int i = 0; i < _dB.Count; i++)
        {
            if(sprite == _dB.GetSprite(i))
            {
                index = i;
                break;
            }
        }

        return index;
    }
}

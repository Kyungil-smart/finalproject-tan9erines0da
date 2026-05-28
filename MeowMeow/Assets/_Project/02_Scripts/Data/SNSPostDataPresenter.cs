using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNSPostDataPresenter : MonoBehaviour
{
    // 관리할 데이터 원본
    private SNSPostDTO _postModel;

    private void Awake()
    {
        InitializeModel();
    }
    private void OnEnable()
    {
        BindEvents();
    }
    private void OnDisable()
    {
        UnbindEvents();
    }

    // 포스트에 적용할 데이터를 초기화 하는 함수
    private void InitializeModel()
    {
        _postModel = new SNSPostDTO
        {
            Stickers = new List<StickerTransformData>(),
            Hashtags = new List<string>()
        };
    }

    // 미리 구독이 필요한 이벤트 구독하는 함수
    private void BindEvents()
    {
        SubscribeManager.instance.Subscribe<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, OnDataRequested
            );
        SubscribeManager.instance.Subscribe<SNSPostDTO>(
            SubscribeType.Update_PostModelData, OnDataUpdated
            );
    }
    // 구독 해제
    private void UnbindEvents()
    {
        if (!SubscribeManager.instance) return;
        SubscribeManager.instance.Unsubscribe<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, OnDataRequested
            );
        SubscribeManager.instance.Unsubscribe<SNSPostDTO>(
            SubscribeType.Update_PostModelData, OnDataUpdated
            );
    }

    // 데이터 요청시 콜백 함수에 모델을 넣어서 실행
    // 데이터의 처리는 콜백 함수를 선언한 쪽에서 처리
    private void OnDataRequested(Action<SNSPostDTO> callback)
    {
        if (callback != null)
        {
            callback.Invoke(_postModel);
        }
    }

    // 패널에서 수정된 값을 덮어 프스트에 적용할 데이터에 덮어씌우는 함수
    private void OnDataUpdated(SNSPostDTO updatedData)
    {
        _postModel = updatedData;
    }
}

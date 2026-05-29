using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISNSPanelPresenter
{
    /// <summary>
    /// SNS 데이터 프레젠터에게 스냅샷을 요청하는 함수
    /// </summary>
    public void RequestContext();

    /// <summary>
    /// SNS 데이터 프레젠터에서 콜백을 통해 스냅샷 데이터를 받아오는 함수입니다
    /// RequestContext에서 SubscribeManager 이벤트를 통해 호출합니다
    /// SubscribeManager.instance.Publish<Action<SNSPostDTO>>(SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    /// </summary>
    /// <param name="snapshot"></param>
    void ReceiveSnapshot(SNSPostDTO snapshot);

    /// <summary>
    /// 패널에서 수정한 데이터를 SNS 데이터 프레젠터로 밀어 넣어 저장하는 함수
    /// </summary>
    public void SubmitContext();
}

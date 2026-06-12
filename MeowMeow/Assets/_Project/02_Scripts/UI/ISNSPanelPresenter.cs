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

/// <summary>
/// 외부 컨텍스트(DTO)를 수신하여 화면을 그릴 수 있는 
/// 읽기 전용 컴포넌트의 규격을 정의합니다.
/// </summary>
public interface ISNSContextReceiver
{
    void RequestContext();
}

// 편집한 데이터를 밖으로 제출하는 역할 (쓰기 전용)
public interface ISNSContextSubmitter
{
    void SubmitContext();
}

/// <summary>
/// 패널이 비활성화되거나 전환될 때 내부 동적 자원과 
/// 메모리를 스스로 소거할 수 있는 능력을 정의합니다.
/// </summary>
public interface ISNSPanelClearable
{
    void ClearPanelContext();
}

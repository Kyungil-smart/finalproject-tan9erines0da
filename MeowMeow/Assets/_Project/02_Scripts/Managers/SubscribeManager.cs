using System;
using System.Collections.Generic;
using UnityEngine;

public enum SubscribeType
{
    None,
    LoginEnter,    // 데이터 없는 로그인 엔터
    OnLoading,//로딩 시작
    OnLoadingComplete,//로딩 끝

    //==================
    EmailVerificationRequired,//활성화 이메일 인증 하라는 창
    DeleteBtnActive, //계정 삭제
    //==================
    SendMessage,
    //=============
    AddUserName,
    //----------
    PlayerSpawnCountUp, //플레이어 스폰
    PlayerSpawnCountDown,
    ///
    PlayerWinLose, //플레이어 승패
    //
    OnWinCanvas,
    OnLoseCanvas,
    OFFWinCanvas,
    OFFLoseCanvas,
    //
    DeSpawnObjects,
    DeSpawnObjectsComplete,
    //d
    dontDestroyBreak,
    // ==========================================
    // [빌드 버전 디버그에 활용하기 위한 라인]
    // ==========================================
    Log_Write,
    // ==========================================
    // [시즌 컨텐츠 호출 전용 라인]
    // ==========================================
    /// <summary>
    /// 누적 보상 팝업에서 애니메이션이 끝났을 때
    /// </summary>
    On_BoxAnimFinish,
    /// <summary>
    /// 누적 보상 팝업이 종료되었을때
    /// 도장 연출 실행을 위해서 호출합니다.
    /// </summary>
    Close_MilestonePopup,
    // ==========================================
    // [SNS 콘텐츠 MVP 데이터 흐름 전용 라인]
    // ==========================================

    /// <summary>
    /// UI 패널들이 현재까지 편집된 
    /// 전체 SNSPostDTO 데이터를 요구할 때 발행 (콜백 인자 필수)
    /// 인자 타입: Action<SNSPostDTO>
    /// </summary>
    Request_CurrentPostContext,

    /// <summary>
    /// 각 UI 패널이 편집을 완료하고 
    /// 중앙 프레젠터에 가공된 DTO 데이터를 밀어 넣을 때 발행
    /// 인자 타입: SNSPostDTO
    /// </summary>
    Update_PostModelData,

    /// <summary>
    /// 피드 팝업 화면 구성을 위한 DTO를 보냅니다
    /// 인자 타입:SNSPostDTO
    /// </summary>
    Send_DTO_for_Feed,

    /// <summary>
    /// 포스트 편집이 최종 완료되어 
    /// 로컬 JSON 저장 및 업로드 루틴을 트리거할 때 발행
    /// 인자 타입: 없음 (Action)
    /// </summary>
    On_SubmitPostProcess,

    /// <summary>
    /// 
    /// </summary>
    RandomSixData,
    // ==========================================
    // [매니져 단에서 관리할 전용 라인]
    // ==========================================
    /// <summary>
    /// 로그인이 완료 되었을때 발행
    /// 인자 타입 : 없음
    /// </summary>
    On_LoginComplete,

}
public class SubscribeManager : MonoBehaviour
{
    public static SubscribeManager instance { get; private set; }

    private Dictionary<SubscribeType, Delegate> subscriptions = new Dictionary<SubscribeType, Delegate>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 인자가 없는 구독 (Action)
    /// <summary>
    /// 매개변수가 없는 이벤트를 구독합니다.
    /// </summary>
    /// <param name="type">구독할 이벤트의 종류</param>
    /// <param name="action">이벤트 발생 시 실행할 메서드</param>
    public void Subscribe(SubscribeType type, Action action)
    {
        if (subscriptions.ContainsKey(type))
            subscriptions[type] = (Action)subscriptions[type] + action;
        else
            subscriptions[type] = action;

        Debug.Log($"Subscribed to {type}");
    }

    /// <summary>
    /// 등록된 매개변수가 없는 이벤트의 구독을 해제합니다.
    /// </summary>
    /// <param name="type">해제할 이벤트의 종류</param>
    /// <param name="action">해제할 메서드</param>
    public void Unsubscribe(SubscribeType type, Action action)
    {
        if (subscriptions.ContainsKey(type))
        {
            subscriptions[type] = (Action)subscriptions[type] - action;
            if (subscriptions[type] == null)
                subscriptions.Remove(type);
        }
    }

    /// <summary>
    /// 해당 이벤트를 발생시켜 구독 중인 모든 메서드를 호출합니다.
    /// </summary>
    /// <param name="type">발행할 이벤트의 종류</param>
    public void Publish(SubscribeType type)
    {
        if (subscriptions.TryGetValue(type, out var del))
        {
            if (del is Action action)
                action.Invoke();
            else
                Debug.LogError($"Type mismatch for {type}");
        }
    }
    #endregion

    #region 인자가 있는 구독 (Action<T>)
    /// <summary>
    /// 데이터(T)를 전달받는 이벤트를 구독합니다.
    /// </summary>
    /// <typeparam name="T">전달받을 데이터의 타입</typeparam>
    /// <param name="type">구독할 이벤트의 종류</param>
    /// <param name="action">이벤트 발생 시 데이터를 받아
    /// 실행할 메서드</param>
    public void Subscribe<T>(SubscribeType type, Action<T> action)
    {
        if (subscriptions.ContainsKey(type))
        {
            // 잘못된 타입 캐스팅 방지를 위한 안전 장치
            if (subscriptions[type] is Action<T> existing)
                subscriptions[type] = existing + action;
            else
                Debug.LogError($"Cast failed for {type}");
        }
        else
        {
            subscriptions[type] = action;
        }
    }

    /// <summary>
    /// 등록된 데이터(T) 기반 이벤트의 구독을 해제합니다.
    /// </summary>
    /// <typeparam name="T">전달받던 데이터의 타입</typeparam>
    /// <param name="type">해제할 이벤트의 종류</param>
    /// <param name="action">해제할 메서드</param>
    public void Unsubscribe<T>(SubscribeType type, Action<T> action)
    {
        if (subscriptions.ContainsKey(type))
        {
            if (subscriptions[type] is Action<T> existing)
            {
                subscriptions[type] = existing - action;
                if (subscriptions[type] == null)
                    subscriptions.Remove(type);
            }
        }
    }

    /// <summary>
    /// 데이터를 포함하여 이벤트를 발생시키고,
    /// 구독 중인 모든 메서드에 데이터를 전달합니다.
    /// </summary>
    /// <typeparam name="T">전달할 데이터의 타입</typeparam>
    /// <param name="type">발행할 이벤트의 종류</param>
    /// <param name="data">구독자들에게 보낼 데이터</param>
    public void Publish<T>(SubscribeType type, T data)
    {
        if (subscriptions.TryGetValue(type, out var del))
        {
            if (del is Action<T> action)
                action.Invoke(data);
            else
                Debug.LogError($"Type mismatch for {type}");
        }
    }
    #endregion
}

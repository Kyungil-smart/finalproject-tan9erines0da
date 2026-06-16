using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaStackBox : MonoBehaviour, ISwitchable
{
    [SerializeField] private GameObject _stamp;
    [SerializeField] private int _gachaStackBoxIndex;
    [SerializeField] private CatStampTweenAni _animStamp;

    public void SetView(bool isOpend)
    {
        if (_stamp == null) return;
        SubscribeManager.instance.Unsubscribe(SubscribeType.Close_MilestonePopup, PlayStamp);

        _stamp.SetActive(isOpend);
        if (!isOpend) SubscribeManager.instance.Subscribe(SubscribeType.Close_MilestonePopup, PlayStamp);
    }

    /// <summary>
    /// 자신이 몇번째 누적 보상인지 정보를 가져오는 함수
    /// </summary>
    /// <param name="index">인덱스를 넣어주세요(i * 10 + 10)</param>
    public void PassIndex(int index)
    {
        _gachaStackBoxIndex = index;
    }

    [ContextMenu("실행 테스트")]
    public void PlayStamp()
    {
        if (_gachaStackBoxIndex == GatchaDataManager.Instance.GatchaData.TotalGatchaCount)
        {
            _animStamp.PlayAnimation();
            SetView(true);
        }
    }
}

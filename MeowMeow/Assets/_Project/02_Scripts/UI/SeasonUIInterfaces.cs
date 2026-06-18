using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPopupable
{
    public GameObject gameObject { get; }
    /// <summary>
    /// 캔버스가 열릴때 실행 되어야하는것들을 작성합니다.
    /// 연출적인 요소들, 데이터 기반으로 뷰 갱신하기 등이 있습니다.
    /// </summary>
    public void Open();


    /// <summary>
    /// 캔버스가 닫힐 때 실행 되어야하는 것들을 작성합니다.
    /// 연출적인 요소들, 데이터 정리하기 등이 있습니다.
    /// </summary>
    public void Close();


    /// <summary>
    /// 열릴때 패널에서 보여주기 위해 필요한 정보를 googleSheetManager.instance.GetClassData<DrawBoardRewards>().FindById("") 방식으로 불러와서 필요한 정보를 필드에 할당합니다
    /// 호출은 기본적으로 GCP에서 itemID를 넣어서 합니다.
    /// </summary>
    /// <param name="itemId"></param>
    public void SetData(int itemId);

    /// <summary>
    /// 버튼 바인딩이 필요할경우 GCP의 캔버스를 열고 닫는 함수를 해당 버튼 이벤트에 등록합니다
    /// </summary>
    public void Bind(GatchaContentPresenter gcp);
    /// <summary>
    /// Bind에서 등록한것을 해제합니다.
    /// </summary>
    public void Unbind();
    
}

public interface ITweenable
{
    /// <summary>
    /// 트윈 애니메이션 등의 연출 요소 실행함수를 작성합니다.
    /// </summary>
    public  void Play();


    /// <summary>
    /// 전환이 진행중인지 여부를 반환하는 마커입니다
    /// 전환 진핸중 다른 처리가 시작되지 않도록 예외처리에 활용합니다.
    /// </summary>
    /// <returns></returns>
    public bool IsTransitioning { get; set; }
}

public interface ISwitchable
{
    /// <summary>
    /// 열려있을 때와 닫혀있을 때에 보여지는것이 달라야하는 경우 상태에따라 뷰를 변경합니다
    /// 뷰를 바꾸는 함수를 구현해주시면 됩니다 (ex : 두개의 뷰 오브젝트 활성 비활성 교체, 토글 전환 등)
    /// </summary>
    public void SetView(bool isOpend);
}

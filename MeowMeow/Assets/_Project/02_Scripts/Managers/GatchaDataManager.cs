using Firebase.Firestore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public partial class GatchaDataManager : MonoBehaviour
{
    public List<BaseGatcha> LogicList=new List<BaseGatcha>();
    public Dictionary<GatchaLogicType, BaseGatcha> LogicDic = new Dictionary<GatchaLogicType, BaseGatcha>();
    public static GatchaDataManager Instance { get; private set; }
   [SerializeField] public GatchaDTO GatchaData=new GatchaDTO();
    //출석으로 얻은 뽑기권 
    public int TodayAttendanceTicketCount => GatchaData.TodayAttendanceTicketCount;
    //퀘스트로 얻은 뽑기권
    public int TodayQuestTicketCount => GatchaData.TodayQuestTicketCount;
    //1등 보상 획득 여부
    public bool Grade_1 => GatchaData.Grade_1;
    //2등 보상 획득 여부
    public bool Grade_2 => GatchaData.Grade_2;
    //3등 보상 획득 여부
    public bool Grade_3 => GatchaData.Grade_3;
    /// ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        Init();
        initList();

        foreach (var item in LogicList)
        {
            item.Init(this);
        }
        LogicDic = LogicList.ToDictionary(x=>x.Type, x=>x);
    }
    public void initList()
    {
        LogicList.Clear();
        var LoadData = Resources.LoadAll<BaseGatcha>("SO");
        LogicList.AddRange(LoadData);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public Task Invoke(GatchaLogicType type, int value=-1)
    {
        if (LogicDic.ContainsKey(type) && value == -1)
        {
            LogicDic[type].TaskExecute();
            return Task.CompletedTask;
        }
        else if(LogicDic.ContainsKey(type))
        {
            LogicDic[type].TaskExecute(value);
            return Task.CompletedTask;
        }
        else
        {
            Debug.LogError($"LogicDic에 {type}이 존재하지 않습니다.");
        }
        return Task.CompletedTask;
    }

    [ContextMenu("Set_GatchaDTO")]
    public async Task Set_GatchaDTO()=> await Invoke(GatchaLogicType.set);
   
    /*
    Set_GatchaDTO 메소드
    GatchaDataManager 클래스 내부 필드인 GatchaData데이터를 저장
    */
    [ContextMenu("Get_GatchaDTO")]
    public async Task Get_GatchaDTO() => await Invoke(GatchaLogicType.get);
    /*
  Get_GatchaDTO 메소드
    Firestore에서 GatchaDataDTo데이터를  파싱 시도하고
    데이터가 없으면  초기 설정값으로 초기화 한다.
    데이터가 있으면 내부 필드로 캐싱한다.
  */
    [ContextMenu("RewardReset")]
    public async Task RewardReset()=> await Invoke(GatchaLogicType.RewardReset);
    /*
    RewardReset 메소드

    ItemList,OpenedIndices clear 하고
    googleSheetManager 에서 데이터 가져와서 List<T>로 변환
    foreach로 reward.Repeat가 false면 무시하고 true인 것들만
    GatchaData.ItemList에  추가한다.
  */

    public async Task InitGatchaData()=> await Invoke(GatchaLogicType.InitGatchaData);
    /*
     InitGatchaData 메소드
     초기 데이터 설정하는 메소드입니다.
     초기 데이터를 가져올 때  데이터가 없으면 초기화 하는 단계에서 호출시키는메소드
     GatchaData.ItemList에 DrawBoardRewards 데이터 uniqueId를 저장한다.
     */

    public int GetItemID(int index) => GatchaData.ItemList[index];
    /*
    GetItemID 메소드
    ItemList에 index번째에 있는 아이템의 ID를 반환하는 메소드입니다.
    */

    public async Task InitfirstDic() =>await Invoke(GatchaLogicType.InitFirstDic);
    /*
     InitfirstDic 메소드
     초기 데이터 설정하는 메소드입니다.
     초기 데이터를 가져올 때  데이터가 없으면 초기화 하는 단계에서 호출시키는메소드
     뽑기권을 써서 열어둔 인덱스를 딕셔너리 key: string value : bool로 관리
     초기 데이터는 0~41까지의 인덱스가 모두 false로 설정시킴
    */

    public void GetTicket() => GatchaData.OwnedTicketCount += 1;
    /*
     GetTicket 메소드
     티켓 획득 시 뽑기권 개수 1 증가시키는 메소드입니다.
    */

    [ContextMenu("테스트용 데이터 초기화 + 티켓 10장 지급")]
    async void Debug_InitForTest()
    {
        await InitGatchaData();
        await  InitfirstDic();
        for (int i = 0; i < 10; i++) GetTicket();
        Debug.Log($"테스트 초기화 완료. ItemList: {GatchaData.ItemList?.Count}개, 티켓: {GatchaData.OwnedTicketCount}장");
    }
   
    public bool IsOpened(int index) =>
        GatchaData.OpenedIndices != null &&
        GatchaData.OpenedIndices.TryGetValue(index.ToString(), out bool result) && result;
    /*
    IsOpened 메소드
    유저가 뽑았던 아이템이면 true 안뽑았던 아이템이면 false 반환
    */
    public async Task drawByIndex(int index) => await Invoke(GatchaLogicType.drawByIndex, index);
    /*
      drawByIndex 메소드
      뽑기를 했을 때 실행될 메소드 입니다.
      index를 받아 해당 인덱스의 보상을 받았는지 확인 후
      보상을 받지 않았으면 보상 받았다는  OpenedIndices[index.ToString()] = true; 처리 후
      OwnedTicketCount <-- 가지고 있는 티켓 카운트 1감소 시킴
    */
    [ContextMenu("IsCompensation")]
    public async Task IsCompensation() =>await Invoke(GatchaLogicType.IsCompensation);
    /*
    IsCompensation 메소드
    일일 출석 보상 지급 시 호출하는 메소드 입니다.
    bool receiveCompensation = Owner.GatchaData.TodayAttendanceTicketCount == 1;
    현재 출석 보상 티켓을 받았는지 판별 후 false면 
    TodayAttendanceTicketCount 일일 출석 티켓 보상 획득 했다고 값 증가 시키고
    현재 보유중인 티켓카운트 증가
    그리고 내부 DTO필드 값 firestore에 저장
    */
    [ContextMenu("IsQuestCompensation")]
    public async Task IsQuestCompensation() => await Invoke(GatchaLogicType.IsQuestCompensation);
    /*
     IsQuestCompensation 메소드
     퀘스트 획득한 티켓카운트 증가 시키는 메소드 입니다.
     bool receiveCompensation = Owner.GatchaData.TodayQuestTicketCount == 2; 로
     퀘스트로 일일 받을 수 있는 티켓 개수 2 를 이미 달성 했는지 확인
     receiveCompensation 가 false면  오늘 퀘스트로 인해 획득한 티켓카운트 증가시키고
     보유중인 티켓 카운트 증가시킴
     그리고 저장
     */
    public async Task ChangeGradeFlag(int value)=> await Invoke(GatchaLogicType.ChangeGradeFlag, value);
    /*
     ChangeGradeFlag 메소드
     초기 유일 보상 획득 했으면 Owner.GatchaData.Grade_* = true; 변경
    */
    public async Task ExecuteGacha(int index) =>await  Invoke(GatchaLogicType.ExecuteGacha, index);
    /*
    ExecuteGacha 메소드
    티켓 으로 뽑기를 실행할 경우 실행하는 메소드
    티켓 부족하면  무시처리
    누적 뽑기 횟수 증가
    뽑기권 감소
    해당 인덱스 개방
    item.Repeat가 false인 유일보상 일 경우 true로 바꿈
  */
}
#if UNITY_EDITOR
[CustomEditor(typeof(GatchaDataManager))]
public class GatchaDataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var data = (GatchaDataManager)target;
        GUILayout.Space(15);
        if (GUILayout.Button("수동 Logic List 데이터 셋팅(Awake 작동되긴 함)", GUILayout.Height(40)))
        {
            // 버튼을 누르면 실행될 로직
            data.initList();
        }
    }
  

}
#endif

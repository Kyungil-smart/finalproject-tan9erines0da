using Firebase.Firestore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GatchaDataManager : MonoBehaviour
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
        foreach(var item in LogicList)
        {
            item.Init(this);
        }
        LogicDic = LogicList.ToDictionary(x=>x.Type, x=>x);
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
    [ContextMenu("setTest")]
    public async void Set_GatchaDTO()=> LogicDic[GatchaLogicType.set].Excute();

    [ContextMenu("getTest")]
    public async void Get_GatchaDTO()
    {
        GatchaData = await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
       if(GatchaData.ItemList == null) 
       {
            InitGatchaData();
            InitfirstDic();
            Set_GatchaDTO();
             
       }
 
    }
    [ContextMenu("RewardReset")]
    public void RewardReset()
    {
        //아이템 리스트 초기화
        //1회성 보상 제외
        GatchaData.IsResetPerformed= true;
        GatchaData.ItemList.Clear();
        GatchaData.OpenedIndices.Clear();

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
            if (reward.Repeat == false) continue;
            else if (reward.Grade == 2)
            {
                for (int i = 0; i < 2; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 3)
            {
                for (int i = 0; i < 3; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 4)
            {
                for (int i = 0; i < 5; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 5)
            {
                for (int i = 0; i < 10; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 6)
            {
                for (int i = 0; i < 21; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else
            {
                GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }

        }
        GatchaData.ItemList.Shuffle();
        Set_GatchaDTO();
    }


    [ContextMenu("InitGatchaData")]
    private void InitGatchaData()
    {
        GatchaData.ItemList = new();
       

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
            if (reward.Grade == 1 && reward.Repeat == true) continue;

            else if (reward.Grade == 3 && reward.Repeat == true)
            {
                for (int i = 0; i < 2; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 4)
            {
                for (int i = 0; i < 5; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 5)
            {
                for (int i = 0; i < 10; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 6)
            {
                for (int i = 0; i < 21; i++)
                    GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else
            {
                GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
        }
        GatchaData.ItemList.Shuffle();
    }
    /*
 public bool IsOpened  메소드 제작해야함

 index가 매개변수

 index에 있는 상품이 뽑혀있는지 안뽑혀있는 반환

 유저가 뽑았던 아이템이면 true 안뽑았던 아이템이면 false
  */
    public bool IsOpened(int index)
    {
       var flag= GatchaData.OpenedIndices[index.ToString()];

        return flag;
    }
    public void  drawByIndex(int index)
    {
        if (IsOpened(index) == false)
        {
            GatchaData.OpenedIndices[index.ToString()] = true;
            GatchaData.OwnedTicketCount -= 1;
        }
    }

    public int GetItemID(int index)=> GatchaData.ItemList[index];
    [ContextMenu("IsCompensation")]
    public void IsCompensation()
    {
        bool receiveCompensation = GatchaData.TodayAttendanceTicketCount == 1;
        if (receiveCompensation == false)
        {
            const int TICKET_REWARD = 1;
            GatchaData.TodayAttendanceTicketCount += TICKET_REWARD;
            GetTicket();
            Set_GatchaDTO();
         
        }
    }
    [ContextMenu("IsQuestCompensation")]
    public void IsQuestCompensation()
    {
        bool receiveCompensation = GatchaData.TodayQuestTicketCount == 2;
        const int MAX_QUEST_TICKET = 2;
        const int MIN_QUEST_TICKET = 0;
        const int QUEST_TICKET_REWARD = 1;
        if (receiveCompensation == false)
        {
            var value = GatchaData.TodayQuestTicketCount + QUEST_TICKET_REWARD;
            GatchaData.TodayQuestTicketCount = Math.Clamp(value, MIN_QUEST_TICKET, MAX_QUEST_TICKET);
            GetTicket();
            Set_GatchaDTO();
        }
    }
    public void ChangeGradeFlag(int value)
    {
        if (value == 1)
            GatchaData.Grade_1 = true;
        else if (value == 2)
            GatchaData.Grade_2 = true;
        else if (value == 3)
            GatchaData.Grade_3 = true; 

      
    }
    public int Testvalue;
    [ContextMenu("ExecuteGacha")]
    public void Test_01()
    {
        ExecuteGacha(Testvalue);
    }
     public  void ExecuteGacha(int index)
     {
        bool ticketIsLack = GatchaData.OwnedTicketCount == 0 && GatchaData.OwnedTicketCount < 0;
        if (ticketIsLack)
        {
            Debug.LogError("뽑기권 부족");
            return;//뽑기권 부족하면 실행 안함
        }

        //누적 뽑기 횟수 증가
        GatchaData.TotalGatchaCount+=1;
        // 뽑기권 감소
        GatchaData.OwnedTicketCount+=-1;
        //해당 인덱스 개방
        GatchaData.OpenedIndices[index.ToString()] = true;

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        var sheetList=drawBoardRewards.m_Data;

       foreach(var item in sheetList)
        {
            if (item.uniqueId == GetItemID(index).ToString())
            {
                if (item.Grade == 1 && item.Repeat == false)
                {
                    ChangeGradeFlag(1);
                    break;
                }
                else if (item.Grade == 2 && item.Repeat == false)
                {
                    ChangeGradeFlag(2);
                    break;
                }
                else if (item.Grade == 3 && item.Repeat == false)
                {
                    ChangeGradeFlag(3);
                    break;
                }
            }
        }
        Set_GatchaDTO();

    }
    private void InitfirstDic()
    {
        GatchaData.OpenedIndices = new();
        for (int i = 0; i < GatchaData.ItemList.Count; i++)
        {

            GatchaData.OpenedIndices.Add(i.ToString(), false);
        }
    }
    public void GetTicket()
    {
        GatchaData.OwnedTicketCount += 1;
    }


}

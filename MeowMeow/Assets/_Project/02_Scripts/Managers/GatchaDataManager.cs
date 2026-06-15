using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatchaDataManager : MonoBehaviour
{
    public static GatchaDataManager Instance { get; private set; }

   [SerializeField] private GatchaDTO _gatchaData=new GatchaDTO();
    public GatchaDTO GatchaData => _gatchaData;
    private int _attendanceCount;
    //출석으로 얻은 뽑기권 
    public int attendanceCount
    {
        get => attendanceCount;
         
        set
        {
            //음수 방지
            _attendanceCount = Mathf.Max(0, value);
        }

    }
    //퀘스트로 얻은 뽑기권
    public int questCount { get; private set; }
    private void Awake()
    {
        Init();
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
    public async void Set_GatchaDTO()
    {
      
        await FireStoreManager.DocumentType(DataType.GatchaData).SetAsync(_gatchaData);
    }
    [ContextMenu("getTest")]
    public async void Get_GatchaDTO()
    {
        _gatchaData = await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
       if(_gatchaData.ItemList == null) 
       {
            InitGatchaData();
            Set_GatchaDTO();
       }
 
    }
    [ContextMenu("RewardReset")]
    public void RewardReset()
    {
        //아이템 리스트 초기화
        //1회성 보상 제외
        
        _gatchaData.ItemList.Clear();
        _gatchaData.OpenedIndices.Clear();

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
            if (reward.Repeat == false) continue;
            else if (reward.Grade == 2)
            {
                for (int i = 0; i < 2; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 3)
            {
                for (int i = 0; i < 3; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 4)
            {
                for (int i = 0; i < 5; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 5)
            {
                for (int i = 0; i < 10; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 6)
            {
                for (int i = 0; i < 21; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else
            {
                _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }

        }
        _gatchaData.ItemList.Shuffle();
    }



    private void InitGatchaData()
    {
        _gatchaData.ItemList = new();
        _gatchaData.OpenedIndices = new();

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
            if (reward.Grade == 1 && reward.Repeat == true) continue;

            else if (reward.Grade == 3 && reward.Repeat == true)
            {
                for (int i = 0; i < 2; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 4)
            {
                for (int i = 0; i < 5; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 5)
            {
                for (int i = 0; i < 10; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 6)
            {
                for (int i = 0; i < 21; i++)
                    _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else
            {
                _gatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
        }
        _gatchaData.ItemList.Shuffle();
    }
    /*
 public bool IsOpened  메소드 제작해야함

 index가 매개변수

 index에 있는 상품이 뽑혀있는지 안뽑혀있는 반환

 유저가 뽑았던 아이템이면 true 안뽑았던 아이템이면 false
  */
    public bool IsOpened(int index)
    {
       var flag= _gatchaData.OpenedIndices[index];

        return flag;
    }
    public void  drawByIndex(int index)
    {
        if (IsOpened(index) == false)
        {
            _gatchaData.OpenedIndices[index] = true;
            _gatchaData.OwnedTicketCount -= 1;
        }
    }

    public int GetItemID(int index)=> _gatchaData.ItemList[index];
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatchaDataManager : MonoBehaviour
{
    public GatchaDataManager Instance { get; private set; }

   [SerializeField] private GatchaDTO _gatchaData=new GatchaDTO();
    public GatchaDTO GatchaData => _gatchaData;
     
    
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
    private async void Set_GatchaDTO()
    {
        var  TestGatchaDTO = new GatchaDTO();
        await FireStoreManager.DocumentType(DataType.GatchaData).SetAsync(TestGatchaDTO);
    }
    [ContextMenu("getTest")]
    private async void Get_GatchaDTO()
    {
       var GetGatchaDTO =await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
       if(GetGatchaDTO.ItemList != null)//초기 데이터가 있는지 없는지 확인하기용도
       {
           _gatchaData = GetGatchaDTO;
       }
       else//초기 데이터가 없으면 초기 보상 셋팅
       {
            /*
             로직이 많이 별로인데 추후에 변경 할 수 도 있음

             */
            GetGatchaDTO.ItemList = new();
            GetGatchaDTO.OpenedIndices = new();

            var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
            foreach (var reward in drawBoardRewards.m_Data)
            {
                if (reward.Grade == 1 && reward.Repeat == true) continue;

                else if (reward.Grade == 3 && reward.Repeat == true)
                {
                    for (int i = 0; i < 2; i++)
                        GetGatchaDTO.ItemList.Add(int.Parse(reward.uniqueId));
                }
                else if (reward.Grade == 4)
                {
                    for (int i = 0; i < 5; i++)
                        GetGatchaDTO.ItemList.Add(int.Parse(reward.uniqueId));
                }
                else if (reward.Grade == 5)
                {
                    for (int i = 0; i < 10; i++)
                        GetGatchaDTO.ItemList.Add(int.Parse(reward.uniqueId));
                }
                else if (reward.Grade == 6)
                {
                    for (int i = 0; i < 21; i++)
                        GetGatchaDTO.ItemList.Add(int.Parse(reward.uniqueId));
                }
                else
                {
                    GetGatchaDTO.ItemList.Add(int.Parse(reward.uniqueId));
                }
            }
            GetGatchaDTO.ItemList.Shuffle();
           _gatchaData = GetGatchaDTO;
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





}

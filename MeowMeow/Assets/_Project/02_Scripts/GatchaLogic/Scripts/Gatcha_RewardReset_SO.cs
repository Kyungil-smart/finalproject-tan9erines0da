using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_RewardReset_SO", menuName = "GatchaSO/05_Gatcha_RewardReset_SO")]
public class Gatcha_RewardReset_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override async Task TaskExecute()
    {
        //아이템 리스트 초기화
        //1회성 보상 제외
        Owner.GatchaData.IsResetPerformed = true;
        Owner.GatchaData.ItemList.Clear();
        Owner.GatchaData.OpenedIndices.Clear();

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
            if (reward.Repeat == false) continue;
            else if (reward.Grade == 2)
            {
                for (int i = 0; i < 2; i++)
                    Owner.GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 3)
            {
                for (int i = 0; i < 3; i++)
                    Owner.GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 4)
            {
                for (int i = 0; i < 5; i++)
                    Owner.GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 5)
            {
                for (int i = 0; i < 10; i++)
                    Owner.GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else if (reward.Grade == 6)
            {
                for (int i = 0; i < 21; i++)
                    Owner.GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }
            else
            {
                Owner.GatchaData.ItemList.Add(int.Parse(reward.uniqueId));
            }

        }
        Owner.GatchaData.ItemList.Shuffle();

        try
        {
            await Owner.Set_GatchaDTO();
        }
        catch(Exception ex)
        {
            Debug.LogError($"Gatcha_RewardReset_SO Error");
        }
       
    }
}

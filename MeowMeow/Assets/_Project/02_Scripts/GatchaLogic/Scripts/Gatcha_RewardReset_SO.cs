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

            int[] counts = { 1, 2, 3, 5, 10, 21 };
            int grade = reward.Grade;
            int count = (grade >= 2 && grade <= 6) ? counts[grade - 1] : 1;
            int itemId = int.Parse(reward.uniqueId);

            for (int i = 0; i < count; i++)
            {
                Owner.GatchaData.ItemList.Add(itemId);
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

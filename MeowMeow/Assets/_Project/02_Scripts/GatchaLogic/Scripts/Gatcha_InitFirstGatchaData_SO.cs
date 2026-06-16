using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_InitFirstGatchaData_SO", menuName = "GatchaSO/04_Gatcha_InitFirstGatchaData_SO")]
public class Gatcha_InitFirstGatchaData_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }

    public override void Excute()
    {
        Owner.GatchaData.ItemList = new();


        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
            if (reward.Grade == 1 && reward.Repeat == true) continue;

            else if (reward.Grade == 3 && reward.Repeat == true)
            {
                for (int i = 0; i < 2; i++)
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
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_InitFirstGatchaData_SO", menuName = "GatchaSO/04_Gatcha_InitFirstGatchaData_SO")]
public class Gatcha_InitFirstGatchaData_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }

    public override Task TaskExecute()
    {
        Owner.GatchaData.ItemList = new();

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var reward in drawBoardRewards.m_Data)
        {
           
            if (reward.Grade == 1 && reward.Repeat == true) continue;
            int[] counts = { 1, 1, 2, 5, 10, 21 };
            int grade = reward.Grade;
            int count = reward.Grade >= 3 && reward.Grade <= 6 ? counts[reward.Grade - 1] : 1;
            int ID = int.Parse(reward.uniqueId);
            for(int i=0; i<count; i++)
            {
                Owner.GatchaData.ItemList.Add(ID);
            }
        }
        Owner.GatchaData.ItemList.Shuffle();
        return Task.CompletedTask;
    }

}

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
            //초기 1등급 보상은 reward.Repeat가false여서 저장하면 안됨
            if (reward.Repeat == false) continue;
            int[] counts = { 1, 1, 3, 5, 10, 21 };
            int grade = reward.Grade;
            int count = reward.Grade ==1  ? 1 : //초기 1등급 보상이면 1개
                        reward.Grade == 2 ? 2 ://2등급 보상, 최초 유일보상/반복보상 각각 1개씩
                        reward.Grade == 3 && reward.Repeat == true  ? 3 ://3등급 보상인데 반복보상이면 2개
                        reward.Grade >=4 && reward.Grade <= 6 ? counts[reward.Grade - 1] : 0;

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

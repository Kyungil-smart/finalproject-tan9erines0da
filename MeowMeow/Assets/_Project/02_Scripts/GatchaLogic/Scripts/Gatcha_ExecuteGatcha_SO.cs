using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "10_Gatcha_ExecuteGatcha_SO", menuName = "GatchaSO/10_Gatcha_ExecuteGatcha_SO")]
public class Gatcha_ExecuteGatcha_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }

    public override async Task TaskExecute(int index)
    {
        bool ticketIsLack = Owner.GatchaData.OwnedTicketCount == 0 && Owner.GatchaData.OwnedTicketCount < 0;
        if (ticketIsLack)
        {
            Debug.LogError("뽑기권 부족");
            return;//뽑기권 부족하면 실행 안함
        }

        //누적 뽑기 횟수 증가
        Owner.GatchaData.TotalGatchaCount += 1;
        // 뽑기권 감소
        Owner.GatchaData.OwnedTicketCount += -1;
        //해당 인덱스 개방
        Owner.GatchaData.OpenedIndices[index.ToString()] = true;

        var drawBoardRewards = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        var sheetList = drawBoardRewards.m_Data;

        foreach (var item in sheetList)
        {
            if (item.uniqueId == Owner.GetItemID(index).ToString())
            {
                if (item.Grade == 1 && item.Repeat == false)
                {
                    Owner.ChangeGradeFlag(1);
                    break;
                }
                else if (item.Grade == 2 && item.Repeat == false)
                {
                    Owner.ChangeGradeFlag(2);
                    break;
                }
                else if (item.Grade == 3 && item.Repeat == false)
                {
                    Owner.ChangeGradeFlag(3);
                    break;
                }
            }
        }
        try
        {
          await Owner.Set_GatchaDTO();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Gatcha_ExecuteGatcha_SO Error");
        }

        SubscribeManager.instance.Publish(SubscribeType.MarkMilestone);
    }


}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
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
             //일단 내가 뽑은거랑 비교
            var isSameID = item.uniqueId == Owner.GetItemID(index).ToString();
            //찾으면 실행
            if (isSameID)
            {
                //아이템 등급이 1등급에서 3등급인지 
                bool IsValidGrade = (item.Grade >= 1 && item.Grade <= 3);
                int CurrentGrade = item.Grade;
                bool flag = (CurrentGrade == 1 && Owner.GatchaData.Grade_1 == false) ? true :
                            (CurrentGrade == 2 && Owner.GatchaData.Grade_2 == false) ? true :
                            (CurrentGrade == 3 && Owner.GatchaData.Grade_3 == false) ? true :
                            false;
                if (flag)
                {
                    await Owner.ChangeGradeFlag(item.Grade);

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

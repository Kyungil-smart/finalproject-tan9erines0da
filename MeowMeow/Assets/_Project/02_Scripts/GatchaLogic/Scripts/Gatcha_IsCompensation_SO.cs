using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_IsCompensation_SO", menuName = "GatchaSO/07_Gatcha_IsCompensation_SO")]
public class Gatcha_IsCompensation_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override void Excute()
    {
        bool receiveCompensation = Owner.GatchaData.TodayAttendanceTicketCount == 1;
        if (receiveCompensation == false)
        {
            const int TICKET_REWARD = 1;
            Owner.GatchaData.TodayAttendanceTicketCount += TICKET_REWARD;
            Owner.GetTicket();
            Owner.Set_GatchaDTO();

        }
    }
}

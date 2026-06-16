using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "08_Gatcha_IsCompensation_SO", menuName = "GatchaSO/08_Gatcha_IsCompensation_SO")]
public class Gatcha_IsQuestCompensation_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override void Excute()
    {
        bool receiveCompensation = Owner.GatchaData.TodayQuestTicketCount == 2;
        const int MAX_QUEST_TICKET = 2;
        const int MIN_QUEST_TICKET = 0;
        const int QUEST_TICKET_REWARD = 1;
        if (receiveCompensation == false)
        {
            var value = Owner.GatchaData.TodayQuestTicketCount + QUEST_TICKET_REWARD;
            Owner.GatchaData.TodayQuestTicketCount = Math.Clamp(value, MIN_QUEST_TICKET, MAX_QUEST_TICKET);
            Owner.GetTicket();
            Owner.Set_GatchaDTO();
        }
    }
}

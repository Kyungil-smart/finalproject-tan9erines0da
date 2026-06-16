using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_DrawByIndex_SO", menuName = "GatchaSO/06_Gatcha_DrawByIndex_SO")]
public class Gatcha_DrawByIndex_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }

    public override void Excute(int index)
    {
        if (Owner.IsOpened(index) == false)
        {
            Owner.GatchaData.OpenedIndices[index.ToString()] = true;
            Owner.GatchaData.OwnedTicketCount -= 1;
        }
    }
}

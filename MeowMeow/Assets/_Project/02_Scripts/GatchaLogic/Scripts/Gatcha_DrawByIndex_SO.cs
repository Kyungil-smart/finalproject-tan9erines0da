using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_DrawByIndex_SO", menuName = "GatchaSO/06_Gatcha_DrawByIndex_SO")]
public class Gatcha_DrawByIndex_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }

    public override Task TaskExecute(int index)
    {
        if (Owner.IsOpened(index) == false)
        {
            Owner.GatchaData.OpenedIndices[index.ToString()] = true;
            Owner.GatchaData.OwnedTicketCount -= 1;
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gatcha_InitFirstDic_SO", menuName = "GatchaSO/03_Gatcha_InitFirstDic_SO")]
public class Gatcha_InitFirstDic_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }

    public override void Excute()
    {
        Owner.GatchaData.OpenedIndices = new();
        for (int i = 0; i < Owner.GatchaData.ItemList.Count; i++)
        {
            Owner.GatchaData.OpenedIndices.Add(i.ToString(), false);
        }
    }
}


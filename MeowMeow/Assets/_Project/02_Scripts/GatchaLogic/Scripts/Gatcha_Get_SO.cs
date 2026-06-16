using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_Get_SO", menuName = "GatchaSO/01_Gatcha_Get_SO")]
public class Gatcha_Get_SO :BaseGatcha
{
     
   
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override async void Excute()
    {
        var GatchaData = await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
        if(GatchaData.ItemList ==null || GatchaData.ItemList.Count == 0)
        {
            Owner.InitGatchaData();
            Owner.InitfirstDic();
            Owner.Set_GatchaDTO();
        }
        else
        {
            Owner.GatchaData = GatchaData;
        }
    }

     
}

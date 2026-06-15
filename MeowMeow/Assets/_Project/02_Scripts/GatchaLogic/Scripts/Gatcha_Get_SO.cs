using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_Get_SO", menuName = "GatchaSO/Gatcha_Get_SO")]
public class Gatcha_Get_SO :BaseGatcha
{
     
   
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override async void Excute()
    {
        Owner.GatchaData = await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
        if(Owner.GatchaData.ItemList ==null)
        {
            InitfirstDic();
        }
    }

    private void InitfirstDic()
    {
        Owner.GatchaData.OpenedIndices= new();
        for (int i = 0; i < Owner.GatchaData.ItemList.Count; i++)
        {
            Owner.GatchaData.OpenedIndices.Add(i.ToString(), false);
        }
    }
}

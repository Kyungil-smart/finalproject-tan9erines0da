using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_Get_SO", menuName = "GatchaSO/01_Gatcha_Get_SO")]
public class Gatcha_Get_SO :BaseGatcha
{
     
   
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override async Task TaskExecute()
    {
        var GatchaData = await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
        var text= FireStoreManager.DocumentType(DataType.GatchaData).TargetStore.currentRef.Path;
        SubscribeManager.instance.Publish(SubscribeType.Test_Path, "set \n"+text);
        if (GatchaData.ItemList ==null || GatchaData.ItemList.Count == 0)
        {
            await Owner.InitGatchaData();
            await Owner.InitfirstDic();
            try
            {
                await Owner.Set_GatchaDTO();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Gatcha_Get_SO Error");
            }
         
        }
        else
        {
            Owner.GatchaData = GatchaData;
        }
    }

     
}

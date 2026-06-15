using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_Set_SO", menuName = "GatchaSO/Gatcha_Set_SO")]
public class Gatcha_Set_SO : BaseGatcha
{
    private GatchaDTO _gatchaData=> Owner.GatchaData;
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override async void Excute()
    {
        await FireStoreManager.DocumentType(DataType.GatchaData).SetAsync(Owner.GatchaData);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "Gatcha_Set_SO", menuName = "GatchaSO/02_Gatcha_Set_SO")]
public class Gatcha_Set_SO : BaseGatcha
{
    private GatchaDTO _gatchaData=> Owner.GatchaData;
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override async Task TaskExecute()
    {
         
        try
        {
            await FireStoreManager.DocumentType(DataType.GatchaData).SetAsync(Owner.GatchaData);
            Debug.Log("Firestore 저장 성공");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Firestore 저장 실패: {ex.Message}\n{ex.StackTrace}");
        }
    }
}

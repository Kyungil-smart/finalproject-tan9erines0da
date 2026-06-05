using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[FirestoreData]
public class FireStoreCurrencyDataManager
{
    public async Task SaveNyangNyangStoneData(int value)
    {
        CurrencyDTO data = new CurrencyDTO
        {
            NyangNyangStone = value
        };

        await FireStoreManager.DocumentType(DataType.CurrencyData).SetAsync(data);
    }

    public async Task<CurrencyDTO> GetData()
    {
        return await FireStoreManager.DocumentType(DataType.CurrencyData).GetAsync<CurrencyDTO>();
    }
}

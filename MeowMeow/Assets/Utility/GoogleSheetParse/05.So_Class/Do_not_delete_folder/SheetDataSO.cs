using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SheetDataSO<T> : BaseDataSO where T : class, IIdentifiable, ISheetParsable, new()
{

    private SheetLoader<T> LoadData;
    [SerializeField] public List<T> m_Data = new List<T>();
    [ContextMenu("비동기 로드")]
    public override async Task InitAsync()
    {
        genericType = typeof(T).Name;
        LoadData = new SheetLoader<T>(url, gid);
        m_Data = await LoadData.GetDataAsync();
    }
    public T FindById(string _id)
    {
        foreach (var item in m_Data)
        {
            var data = item as IIdentifiable;
            if(data ==null)return null;
            if (data.uniqueId == _id)
            {
                return item;
            }
        }
        return null;
    }
    
}

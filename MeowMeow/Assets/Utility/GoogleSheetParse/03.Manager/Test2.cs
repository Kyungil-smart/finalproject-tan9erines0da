using System.Linq;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public googleSheetManager Manager;

 
    [ContextMenu("dsds")]
    public void dsds()
    {
        // 1. 매니저에서 Char2Data(SO)를 가져옵니다. 반환 타입이 Char2Data로 깔끔하게 나옵니다!
        var charSO = Manager.GetClassData<comment>();

        if (charSO != null)
        {
            var Listdata = charSO.m_Data;
            var saveList= Listdata.Where(x=>x.type == CommentType.Butler_Life).ToList();
            foreach (var item in saveList)
            {
                Debug.Log($"ID: {item.word} :{item.type} \n");
            }
        }
    }
}

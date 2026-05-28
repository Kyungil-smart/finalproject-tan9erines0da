 
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class googleSheetManager : MonoBehaviour
{
    public List<BaseDataSO> m_Listdata = new List<BaseDataSO>();
     

     
    public async void DataLoad()
    {
        await DataLoadAsync();
    }

    public async Task DataLoadAsync()
    {
        List<Task> tasks = new List<Task>();
        foreach (var item in m_Listdata)
        {
            tasks.Add(item.InitAsync());
        }
        await Task.WhenAll(tasks);
    }

    public SheetDataSO<T> GetClassData<T>() where T : class, IIdentifiable, ISheetParsable, new()
    {
        foreach (var item in m_Listdata)
        {
            // item이 우리가 찾는 T 타입인지 확인하고, 
            // 맞다면 result라는 변수에 T 타입으로 담아서 바로 반환합니다.
            if(item is SheetDataSO<T> result)
            {
                return result;
            }
        }
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(googleSheetManager))]
public class MyDataControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기존 인스펙터의 기본 필드들(sheetUrl 등)을 그대로 먼저 그려줍니다.
        DrawDefaultInspector();

        // 타겟 스크립트를 가져옵니다.
        googleSheetManager generator = (googleSheetManager)target;

        // 위아래 여백을 살짝 줍니다.
        GUILayout.Space(15);

        // 💡 버튼 만들기 (버튼이 클릭되면 true를 반환합니다)
        if (GUILayout.Button("구글 시트 불러오기", GUILayout.Height(40)))
        {
            // 버튼을 누르면 실행될 로직
            generator.DataLoad();
        }
    }
}
#endif

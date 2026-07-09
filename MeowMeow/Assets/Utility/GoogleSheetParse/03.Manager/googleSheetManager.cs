 
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class googleSheetManager : MonoBehaviour
{
    public   List<BaseDataSO> m_Listdata = new List<BaseDataSO>();

    public static googleSheetManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void AutoSetting()
    {
        m_Listdata.Clear();
       var datas= Resources.LoadAll<BaseDataSO>("99.SO");
        m_Listdata.AddRange(datas);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    public async void DataLoad()
    {
        AutoSetting();
        List<Task> tasks = new List<Task>();
        foreach (var item in m_Listdata)
        {
            tasks.Add(item.InitAsync());
        }
        await Task.WhenAll(tasks);
#if UNITY_EDITOR
        foreach (var item in m_Listdata)
        {
            if (item != null)
            {
                EditorUtility.SetDirty(item); // 이 SO 변경되었으니 다시 저장하라고 마킹
            }
        }
        AssetDatabase.SaveAssets(); // 마킹된 에셋들 실제로 디스크에 파일로 저장
        AssetDatabase.Refresh();    // 에디터 새로고침
#endif
    }

    public   SheetDataSO<T> GetClassData<T>() where T : class, IIdentifiable, ISheetParsable, new()
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

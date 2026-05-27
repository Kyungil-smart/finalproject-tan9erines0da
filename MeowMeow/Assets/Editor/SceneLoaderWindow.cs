#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using System.IO;

/// <summary>
///  SceneLoaderWindow
/// 프로젝트 내 모든 씬을 검색해 목록으로 표시하고,
/// 클릭 한 번으로 바로 열거나(Load), 재생(Play)할 수 있게 해주는 Unity Editor 툴입니다.
///
/// 주요 기능:
/// - 모든 씬 자동 탐색 (AssetDatabase.FindAssets)
/// - 검색 필터로 씬 이름 빠른 검색
/// - 버튼으로 즉시 열기(Open) 또는 실행(Play)
/// </summary>
public class SceneLoaderWindow : EditorWindow
{
    // ScrollView 위치 저장용
    private Vector2 scrollPos;

    // 검색창 입력 문자열
    private string searchFilter = "";

    // 씬 파일의 전체 경로 (ex: Assets/Scenes/Main.unity)
    private string[] scenePaths;

    // 씬 이름 배열 (ex: Main)
    private string[] sceneNames;

    /// <summary>
    /// Unity 상단 메뉴에 "Tools/Scene Loader" 추가
    /// 클릭 시 창이 열림
    /// </summary>
    [MenuItem("Tools/Scene Loader")]
    public static void ShowWindow()
    {
        GetWindow<SceneLoaderWindow>("Scene Loader");
    }

    /// <summary>
    /// 에디터 창이 활성화될 때 호출됨
    /// 씬 리스트를 초기화
    /// </summary>
    private void OnEnable()
    {
        RefreshSceneList();
    }

    /// <summary>
    /// 프로젝트 내 모든 씬(.unity) 파일을 찾아 목록 갱신
    /// </summary>
    private void RefreshSceneList()
    {
        // 1. 프로젝트 내 모든 씬 파일 경로 가져오기
        var allPaths = AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath);

        // 2. 필터링 조건을 적용하여 실제 필요한 씬만 선별
        var validScenes = allPaths.Where(path =>
        {
            // 파일명 추출 (ex: ~SerializationTests_Group)
            string fileName = Path.GetFileNameWithoutExtension(path);

            // [제외 조건 1] 파일 이름이 임시/테스트용 물결(~)로 시작하는 경우 제외
            if (fileName.StartsWith("~")) return false;

            // [제외 조건 2] 에셋 폴더(Assets/) 외부(예: Packages/ 폴더 안의 내장 씬들) 제외
            if (!path.StartsWith("Assets/")) return false;

            // [제외 조건 3] 만약 특정 폴더(예: 임시 폴더)를 통째로 빼고 싶다면 추가 가능
            // if (path.Contains("/Plugins/") || path.Contains("/Tests/")) return false;

            return true;
        }).ToList();

        // 3. 필터링된 결과를 배열에 할당
        scenePaths = validScenes.ToArray();
        sceneNames = validScenes.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
    }

    /// <summary>
    /// 에디터 GUI 렌더링
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.Space();

        //검색창 영역
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search:", GUILayout.Width(50));

        // 사용자가 입력한 검색어를 실시간 반영
        string newFilter = EditorGUILayout.TextField(searchFilter);
        if (newFilter != searchFilter)
        {
            searchFilter = newFilter;
        }

        // 새로고침 버튼
        if (GUILayout.Button("⟳", GUILayout.Width(30)))
        {
            RefreshSceneList();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 스크롤 가능한 씬 리스트 영역
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // 씬 목록 반복 출력
        for (int i = 0; i < scenePaths.Length; i++)
        {
            // 검색 필터 적용 (대소문자 무시)
            if (!string.IsNullOrEmpty(searchFilter) &&
                !sceneNames[i].ToLower().Contains(searchFilter.ToLower()))
                continue;

            EditorGUILayout.BeginHorizontal();

            // 씬 이름 표시
            EditorGUILayout.LabelField(sceneNames[i]);

            // Open 버튼: 씬 열기
            if (GUILayout.Button("Open", GUILayout.Width(60)))
            {
                // 현재 씬이 수정된 경우, 저장 여부 묻기
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // 선택된 씬 열기
                    EditorSceneManager.OpenScene(scenePaths[i]);
                }
            }

            // Play 버튼: 씬 열고 바로 재생
            if (GUILayout.Button("Play", GUILayout.Width(60)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePaths[i]);
                    EditorApplication.isPlaying = true; // 재생 모드 시작
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif

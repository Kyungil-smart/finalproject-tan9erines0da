using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIPresenter : MonoBehaviour
{
    [Header("참조 필요")]
    [Tooltip("레이아웃 그룹을 참조합니다")]
    [SerializeField] GridLayoutGroup _gridLayoutGroup;
    [Tooltip("이미지 버튼의 프리팹을 참조합니다")]
    [SerializeField] SNSPostImageRenderer _prefab;
    [Tooltip("화면전환 컨트롤러를 참조합니다")]
    [SerializeField] private BaseScreenController _uiController;
    [Tooltip("팝업할 판넬을 참조합니다")]
    [SerializeField] private UIPanel _feedDepth2Panel;

    private List<GameObject> _spawnedItem = new();

    private void OnEnable()
    {
        RefreshGrid();
        if(SNSPostManager.Instance != null)
            SNSPostManager.Instance.OnMyPostHistoryUpdated += RefreshGrid;
    }
    private void OnDisable()
    {
        if (SNSPostManager.Instance != null)
            SNSPostManager.Instance.OnMyPostHistoryUpdated -= RefreshGrid;

    }

    // 로컬에 기록된 포스트에 맞추어 버튼을 생성하는 함수
    private void RefreshGrid()
    {
        if (SNSPostManager.Instance == null) return;

        ClearGrid();

        var myPosts = SNSPostManager.Instance.MyPostHistory;

        foreach (var postDto in myPosts)
        {
            // 프리팹 생성 및 리스트 등록
            SNSPostImageRenderer spawned = Instantiate(_prefab, _gridLayoutGroup.transform);
            _spawnedItem.Add(spawned.gameObject);

            // 이미지 복원
            spawned.RenderPreview(postDto);

            // 버튼에 이베트 바인딩
            Button btn = spawned.GetComponent<Button>();
            if(btn != null)
            {
                SNSPostDTO buttonDto = postDto;
                // 람다식을 통한 dto 바인딩
                btn.onClick.AddListener(() => OnFeedClicked(buttonDto));
            }
        }

    }
    // 레이아웃 그룹을 청소하는 함수
    private void ClearGrid()
    {
        foreach (var item in _spawnedItem)
        {
            if (item != null) Destroy(item);
        }
        _spawnedItem.Clear();
    }

    void OnFeedClicked(SNSPostDTO dto)
    {
        SNSPostManager.Instance.SelectFeed(dto);

        if(_uiController != null && _feedDepth2Panel != null)
        {
            _uiController.RequestScreenChange(_feedDepth2Panel);
        }
    }
}

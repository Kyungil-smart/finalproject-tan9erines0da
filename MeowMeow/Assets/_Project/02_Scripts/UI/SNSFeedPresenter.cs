using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIPanel))]
public class SNSFeedPresenter : MonoBehaviour,
    ISNSContextReceiver, ISNSPanelClearable
{
    [Header("중심 UI 렌더러 및 텍스트")]
    [SerializeField]
    private SNSPostImageRenderer _imageRenderer;
    [SerializeField]
    private TextMeshProUGUI _commentText;
    [SerializeField]
    private TextMeshProUGUI _hashtagText;
    [SerializeField]
    private ScrollRect _scrollView;

    [Header("가상 댓글 생성 관련")]
    [SerializeField]
    private GameObject _commentPrefab;
    [SerializeField]
    private Transform _commentContainer;

    // 생성된 댓글 오브젝트 추적용 리스트
    private List<GameObject> _spawnedComments =
        new List<GameObject>();

    // 오브젝트 비활성화 체크 플래그
    private bool _needsLayoutRebuild = false;

    /// <summary>
    /// 전역 매니져의 현재 DTO를 기반으로 복원을 수행합니다.
    /// </summary>
    public void RequestContext()
    {
        if(SNSPostManager.Instance == null) return;

        SNSPostDTO snapshot = SNSPostManager.Instance.CurrentSelectedFeed;

        // 1. 이미지 및 상단 본문 복원 
        if (_imageRenderer != null) _imageRenderer.RenderPreview(snapshot);
        if (_commentText != null) _commentText.text = snapshot.Comment;

        if (_hashtagText != null && snapshot.Hashtags != null)
        {
            _hashtagText.text = string.Join(" ", snapshot.Hashtags);
        }

        // 2. 가상 댓글 생성 
        BuildFakeComments(snapshot);

        // 3. 현재 오브젝트가 켜져 있는지 확인
        if (gameObject.activeInHierarchy)
        {
            // 이미 켜져 있다면 즉시 정렬
            ExecuteLayoutRefresh();
        }
        else
        {
            // 꺼져 있다면 OnEnable 시점에 정렬
            _needsLayoutRebuild = true;
        }
    }
    void OnEnable()
    {
        if (_needsLayoutRebuild) ExecuteLayoutRefresh();
    }

    /// <summary>
    /// 레이아웃을 강제로 재계산하고 스크롤을 리셋하는 정렬 함수
    /// </summary>
    private void ExecuteLayoutRefresh()
    {
        // 플래그 초기화
        _needsLayoutRebuild = false;

        // 캔버스 재계산
        Canvas.ForceUpdateCanvases();

        if (_commentContainer is RectTransform rect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        if (_scrollView != null)
        {
            _scrollView.verticalNormalizedPosition = 1f;
        }
    }

    /// <summary>
    /// 패널 종료 시 패널을 청소합니다
    /// </summary>
    public void ClearPanelContext()
    {
        _needsLayoutRebuild = false;

        // 삭제 및 분리
        for (int i = _spawnedComments.Count - 1; i >= 0; i--)
        {
            if (_spawnedComments[i] != null)
            {
                GameObject target = _spawnedComments[i];
                target.transform.SetParent(null);
                DestroyImmediate(target);
            }
        }
        _spawnedComments.Clear();

        if (_commentText != null) _commentText.text = string.Empty;
        if (_hashtagText != null) _hashtagText.text = string.Empty;
    }

    /// <summary>
    /// RandomIndex 시드 기반 결정론적 가상 댓글 생성
    /// </summary>
    private void BuildFakeComments(SNSPostDTO snapshot)
    {
        if (_commentPrefab == null ||
            _commentContainer == null) return;

        // 고유 난수 값을 정수형 시드로 완전 고정
        int seed = Mathf.RoundToInt(
            (float) snapshot.RandomIndex * 100000f);
        Random.InitState(seed);

        // SO 데이터 풀 확보
        var mgr = googleSheetManager.instance;
        var tagSO = mgr.GetClassData<Hashtag>();
        var comSO = mgr.GetClassData<daesgeul>();
        var userSO = mgr.GetClassData<VirtualUserProfile>();

        if (tagSO == null || comSO == null || userSO == null) return;

        // 1) 포스트 해시태그와 호응하는 댓글 풀 필터링
        List<daesgeul> pool = new List<daesgeul>();
        HashSet<string> tagIds = new HashSet<string>();

        // NULL id 필수 포함
        tagIds.Add("60001");
        
        // 태그 아이디 풀에 설정된 해시태그 id 주입
        foreach (var tag in tagSO.m_Data)
        {
            if (snapshot.Hashtags.Contains(tag.TagName))
            {
                tagIds.Add(tag.uniqueId);
            }
        }
        // 태그 아이디 풀 기준으로 코멘트 순회후 추가
        foreach (var com in comSO.m_Data)
        {
            string depId = com.dependent_ID.ToString();
            
            if (tagIds.Contains(depId))
            {
                pool.Add(com);
            }
        }

        if (pool.Count == 0)
            pool = new List<daesgeul>(comSO.m_Data);

        // 댓글 풀 셔플
        for (int i = 0;  i < pool.Count; i++)
        {
            int rnd = Random.Range(i, pool.Count);
            var temp = pool[i];
            pool[i] = pool[rnd];
            pool[rnd] = temp;
        }

        // 2) 중복 없는 유저 배정을 위한 유저 리스트 셔플
        List<VirtualUserProfile> users =
            new List<VirtualUserProfile>(userSO.m_Data);

        for (int i = 0; i < users.Count; i++)
        {
            int rnd = Random.Range(i, users.Count);
            var temp = users[i];
            users[i] = users[rnd];
            users[rnd] = temp;
        }

        // 3) 최대 4개의 가상 댓글 인스턴스화 및 데이터 주입
        int maxCount = Mathf.Min(users.Count, pool.Count);
        int count = Mathf.Min(4, maxCount);
        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(
                _commentPrefab, _commentContainer, false);
            _spawnedComments.Add(item);

            FakeCommentItem script =
                item.GetComponent<FakeCommentItem>();

            if (script != null)
            {
                VirtualUserProfile u = users[i];
                
                string text = pool[i].sentence;

                script.SetData(u.UserName, text);
            }
        }
    }
}

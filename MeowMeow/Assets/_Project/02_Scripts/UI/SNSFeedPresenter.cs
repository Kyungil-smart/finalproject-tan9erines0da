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
    private UnityEngine.UI.ScrollRect _scrollView;

    [Header("가상 댓글 생성 관련")]
    [SerializeField]
    private GameObject _commentPrefab;
    [SerializeField]
    private Transform _commentContainer;

    // 생성된 댓글 오브젝트 추적용 리스트
    private List<GameObject> _spawnedComments =
        new List<GameObject>();

    /// <summary>
    /// 전역 매니져의 현재 DTO를 기반으로 복원을 수행합니다.
    /// </summary>
    public void RequestContext()
    {
        if (SNSPostManager.Instance == null) return;

        // 전역 매니져부터 DTO 획득
        SNSPostDTO snapshot = SNSPostManager.Instance.CurrentSelectedFeed;

        // 1. 이미지 및 상단 본문 복원
        if (_imageRenderer != null)
            _imageRenderer.RenderPreview(snapshot);

        if (_commentText != null)
            _commentText.text = snapshot.Comment;

        // 2. 해시태그 복원 문자열 가공
        if (_hashtagText != null && snapshot.Hashtags != null)
        {
            _hashtagText.text =
                string.Join(" ", snapshot.Hashtags);
        }

        // 3. 가상 댓글 생성 엔진 구동
        BuildFakeComments(snapshot);


        // 4. 스크롤 위치를 무조건 최상단(1.0f)으로 리셋
        if (_scrollView != null)
        {
            _scrollView.verticalNormalizedPosition = 1f;
        }

        // 5. 동적으로 생성된 댓글들의 레이아웃 높이 재계산
        if (_commentContainer is RectTransform rect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    /// <summary>
    /// 패널 종료 시 패널을 청소합니다
    /// </summary>
    public void ClearPanelContext()
    {
        string stackTrace = System.Environment.StackTrace;

        // 생성된 가상 댓글 오브젝트 완전 파괴
        foreach (var obj in _spawnedComments)
        {
            if (obj != null) Destroy(obj);
        }
        _spawnedComments.Clear();

        // 텍스트 컴포넌트 백지화
        if (_commentText != null)
            _commentText.text = string.Empty;
        if (_hashtagText != null)
            _hashtagText.text = string.Empty;
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

        foreach (var tag in tagSO.m_Data)
        {
            if (snapshot.Hashtags.Contains(tag.TagName))
            {
                tagIds.Add(tag.uniqueId);
            }
        }

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

        // 3) 최대 6개의 가상 댓글 인스턴스화 및 데이터 주입
        int count = Mathf.Min(6, users.Count);
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
                int idx = Random.Range(0, pool.Count);
                string text = pool[idx].sentence;

                script.SetData(u.UserName, text);
            }
        }
    }
}

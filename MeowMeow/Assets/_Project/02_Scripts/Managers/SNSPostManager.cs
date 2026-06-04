using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;

public class SNSPostManager : MonoBehaviour
{
    public static SNSPostManager Instance { get; private set; }

    public List<SNSPostDTO> CachedFeeds { get; private set; }
        = new List<SNSPostDTO>();

    public List<SNSPostDTO> MyPostHistory { get; private set; }
        = new List<SNSPostDTO>();

    public event Action OnFeedUpdated;
    public event Action OnMyPostHistoryUpdated;

    // 현재 로그인된 유저의 UID를 안전하게 가져오는 프로퍼티
    private string CurrentUID
        => BackendManager.Auth?.CurrentUser?.UserId ?? "GuestUID";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// [★초기화 시점] 로그인 직후 또는 메인 로비 진입 시 호출하여 
    /// 서버 요청 없이 로컬 기기에서 기존 데이터를 즉시 복원합니다.
    /// </summary>
    public void LoadLocalData()
    {
        MyPostHistory = LocalFeedStorage.LoadPosts(
            CurrentUID, "MyPosts");

        CachedFeeds = LocalFeedStorage.LoadPosts(
            CurrentUID, "RandomFeeds");

        Debug.Log($"[Manager] 로컬 데이터 복원 완료. " +
            $"내 글: {MyPostHistory.Count}개, 피드: {CachedFeeds.Count}개");
    }

    /// <summary>
    /// 내 포스트를 등록할 때 즉시 히스토리에 누적하고 JSON을 덮어씁니다.
    /// </summary>
    public async Task UploadAndCachePostAsync(SNSPostDTO data)
    {
        // 현재 로그인된 내 UID 주입
        data.WriterId = CurrentUID;

        // 무작위 6개 추출 쿼리 연산을 위한 난수 인덱스 부여
        data.RandomIndex = UnityEngine.Random.value;

        // 최신순 정렬을 위한 현재 UTC 시간 주입 (밀리초)
        data.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // 1. 작성 시기 순서(최신순)를 위해 리스트의 맨 앞(0번)에 삽입합니다.
        MyPostHistory.Insert(0, data);

        // 2. 즉시 로컬 JSON에 덮어써서 앱이 꺼져도 날아가지 않게 방어
        LocalFeedStorage.SavePosts(
            CurrentUID, "MyPosts", MyPostHistory);

        OnMyPostHistoryUpdated?.Invoke();

        // 3. 네트워크 통신 사출
        FirestoreSNSPostDoc doc = new FirestoreSNSPostDoc(data);

        try
        {
            await FireStoreManager.DocumentType(DataType.Posts)
                                  .SetAsync(doc);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Manager Error] 서버 업로드 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 타인 피드 갱신 시, 파이어스토어 읽기 후 로컬 JSON까지 업데이트합니다.
    /// </summary>
    public async Task RefreshRandomFeedsAsync()
    {
        try
        {
            var fetched = await FireStoreManager.DocumentType(DataType.Posts)
                .GetRandomSixData<FirestoreSNSPostDoc>();

            if (fetched == null || fetched.Count == 0) return;

            CachedFeeds.Clear();
            foreach (var doc in fetched)
            {
                if (doc != null) CachedFeeds.Add(doc.ToStruct());
            }

            // 다운로드 받은 6개의 피드도 다음 앱 실행을 위해 로컬 보존
            LocalFeedStorage.SavePosts(
                CurrentUID, "RandomFeeds", CachedFeeds);

            OnFeedUpdated?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Manager Error] 피드 갱신 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 타기기 접속시 로컬 업데이트를 위한 로드함수입니다.
    /// </summary>
    public async Task LoadLocalDataAsync()
    {
        // 1. 선제적인 로컬 스토리지 JSON 탐색
        MyPostHistory = LocalFeedStorage.LoadPosts(CurrentUID, "MyPosts");
        CachedFeeds = LocalFeedStorage.LoadPosts(CurrentUID, "RandomFeeds");

        // 2. 로컬 캐시가 완전히 비어있을 때 크로스 디바이스 동기화 발동
        if (MyPostHistory.Count == 0)
        {
            Debug.Log("[Manager] 로컬 데이터 전무. 서버 동기화 가동...");

            // [★일관성 확보] GetRandomSixData와 완벽하게 대칭을 이루는 
            // 콤팩트한 확장 메서드 호출 파이프라인이 완성되었습니다.
            List<FirestoreSNSPostDoc> serverDocs = await FireStoreManager
                .DocumentType(DataType.Posts)
                .SyncMyHistoryData<FirestoreSNSPostDoc>(CurrentUID);

            if (serverDocs != null && serverDocs.Count > 0)
            {
                foreach (var doc in serverDocs)
                {
                    MyPostHistory.Add(doc.ToStruct());
                }

                // 차기 재접속 시 서버 조회를 막기 위한 로컬 즉시 구워버리기
                LocalFeedStorage.SavePosts(
                    CurrentUID, "MyPosts", MyPostHistory);

                Debug.Log($"[Sync] {MyPostHistory.Count}개 동기화 보존 완료");
            }
        }

        OnMyPostHistoryUpdated?.Invoke();
    }
}

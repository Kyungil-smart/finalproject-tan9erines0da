using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _logoutButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _statusText;

    private bool _isProcessing;

    private void Awake() => UpdateStatus("초기화...");
    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();
    private async void Start() => await TryAutoLoginAsync();

    private void BindButtonEvents()
    {
        _loginButton.onClick.AddListener(OnLoginClicked);
        _logoutButton.onClick.AddListener(OnLogoutClicked);
    }

    private void UnbindButtonEvents()
    {
        _loginButton.onClick.RemoveListener(OnLoginClicked);
        _logoutButton.onClick.RemoveListener(OnLogoutClicked);
    }

    private async Task TryAutoLoginAsync()
    {
        bool firebaseOk = await BackendManager.ReadyTask;
        if (!firebaseOk)
        {
            UpdateStatus("Firebase 초기화 실패");
            return;
        }

        try
        {
            await UnityAuthService.InitializeAsync();

            FirebaseUser user = BackendManager.Auth.CurrentUser;
            if (user == null)
            {
                UpdateStatus("대기");
                return;
            }

            await RestoreSessionAsync(user);
        }
        catch (Exception e)
        {
            UpdateStatus($"자동 로그인 실패: {e.Message}");
        }
    }

    private async Task RestoreSessionAsync(FirebaseUser user)
    {
        UpdateStatus("세션 복원...");
        string firebaseIdToken = await user.TokenAsync(false);
        await UnityAuthService.SignInWithGoogleAsync(firebaseIdToken);

        await InitUserData();
        _startButton.interactable = true;

        UpdateStatus($"환영합니다, {GetDisplayName(user)}님");
    }

    private async void OnLoginClicked()
    {
        if (_isProcessing) return;
        _isProcessing = true;
        SetButtonsInteractable(false);

        try
        {
            await PerformLoginAsync();
        }
        catch (Exception e)
        {
            UpdateStatus($"로그인 실패: {e.Message}");
        }
        finally
        {
            _isProcessing = false;
            SetButtonsInteractable(true);
        }
    }

    private async Task PerformLoginAsync()
    {
        UpdateStatus("UGS 초기화...");
        await UnityAuthService.InitializeAsync();

        UpdateStatus("Google 로그인 시도...");
        FirebaseUser user = await GoogleSignInService.SignInAsync();

        UpdateStatus("UGS 인증 시도...");
        string firebaseIdToken = await user.TokenAsync(false);
        await UnityAuthService.SignInWithGoogleAsync(firebaseIdToken);

        await InitUserData();
        _startButton.interactable = true;

        UpdateStatus($"환영합니다, {GetDisplayName(user)}님");
    }

    private void OnLogoutClicked()
    {
        if (_isProcessing) return;
        UnityAuthService.SignOut();
        GoogleSignInService.SignOut();
        UpdateStatus("로그아웃");
        _startButton.interactable = false;
    }

    private static string GetDisplayName(FirebaseUser user)
        => string.IsNullOrEmpty(user.DisplayName) ? user.Email : user.DisplayName;

    private void UpdateStatus(string message)
        => _statusText.text = message;

    private void SetButtonsInteractable(bool interactable)
    {
        _loginButton.interactable = interactable;
        _logoutButton.interactable = interactable;
    }

    // 로그인 확인후 실행되는 유저관련 데이터 초기화 함수
    private async Task InitUserData()
    {
        UpdateStatus("로컬 저장 기록 확인 중...");

        // 유저가 작성한 포스트 확보
        if (SNSPostManager.Instance != null)
        {
            SNSPostManager.Instance.LoadLocalData();
        }

        try
        {
            UpdateStatus($"서버 시간 확인 중...");
            await TimeManager.Instance.InitializationTask;
        }
        catch
        {
            UpdateStatus("서버 시간을 가져올 수 없습니다...");
        }
        
        // 유저 보유 재화 확보
        if (LocalUserDataManager.Instance != null)
        {
            try
            {
                UpdateStatus("유저 정보 확인 중...");
                await LocalUserDataManager.Instance.LoadUserData();
            }
            catch
            {
                UpdateStatus("유저 정보를 확인 할 수 없습니다");
            }
        }
        

        FirebaseUser user = BackendManager.Auth.CurrentUser;

        try
        {
            UpdateStatus("시즌 콘텐츠 데이터 확인 중 ...");
            await GatchaDataManager.Instance.Get_GatchaDTO();
        }
        catch
        {
            UpdateStatus("시즌 콘텐츠 데이터를 업데이트 할 수 없습니다.");
        }

        // 날짜 바뀌는거 코드
        string NowDateTime = TimeManager.Instance.CurrentDate;
        string LastDateTime = LocalUserDataManager.Instance.LastDate;

        if (LastDateTime != NowDateTime)
        {
            GatchaDataManager.Instance.OnDailyReset();
            LocalFeedStorage.GetRandomSix();
            // CurrencyDTO의 LastDate 날짜 갱신
            try
            {
                UpdateStatus("날짜를 갱신 하였습니다.");
                await LocalUserDataManager.Instance.UpdateLastDate(NowDateTime);
            }
            catch
            {
                UpdateStatus("네트워크가 불안정 합니다.");
            }
        }

        try
        {
            UpdateStatus("출석 보상 확인 중");
            await GatchaDataManager.Instance.IsCompensation();
        }
        catch
        {
            UpdateStatus("출석 보상 확인 실패...");
        }
        


        var localFeedStorage = LocalFeedStorage.LoadPosts(user.UserId, "RandomFeeds");
        
        // 유저 로컬 데이터에 랜덤피드 6개 확보
        if (localFeedStorage.Count > 1)
        {
            // 이미 자료가 있으면 통과합니다.
        }
        else
        {
            // 로컬에 데이터가 존재하지 않을 때
            try
            {
                UpdateStatus("피드 정보 초기화...");
                await SNSPostManager.Instance.RefreshRandomFeedsAsync();
            }
            catch
            {
                UpdateStatus("피드 초기화 실패");
            }
        }

        // 자정이벤트 등록
        if(TimeManager.Instance != null)
        {
            TimeManager.Instance.OnOClock += GatchaDataManager.Instance.OnDailyReset;
            TimeManager.Instance.OnOClock += LocalFeedStorage.GetRandomSix;
        }
    }

    
}

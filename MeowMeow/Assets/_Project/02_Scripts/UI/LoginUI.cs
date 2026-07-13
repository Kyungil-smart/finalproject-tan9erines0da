using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Threading;
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
            HandleInitFailure($"자동 로그인 실패: {e.Message}");
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

        // 백엔드 매니져에 유저 정보 갱신 시간 확보를 위한 딜래이
        int timeoutCheck = 0;
        while (BackendManager.Auth.CurrentUser == null && timeoutCheck < 100)
        {
            timeoutCheck++;
            await Task.Yield(); // 1프레임 대기 및 연산 양보
        }

        // 유저정보 바인딩 실패 예외처리
        if (BackendManager.Auth.CurrentUser == null)
        {
            throw new Exception("Firebase 인증 바인딩 지연 에러");
        }

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

    private void HandleInitFailure(string errorMessage)
    {
        Debug.LogError($"[InitBlock] {errorMessage}");

        _startButton.interactable = false;

        // 로그아웃 후 재로그인을 안내
        UpdateStatus($"{errorMessage}\n오류가 지속되면 로그아웃 후 재로그인 해주세요.");
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
        if (SNSPostManager.Instance != null)
        {
            SNSPostManager.Instance.LoadLocalData();
        }


        // 2. 유저 정보 로드 검사
        if (LocalUserDataManager.Instance != null)
        {
            try
            {
                UpdateStatus("유저 정보 확인 중...");
                await LocalUserDataManager.Instance.LoadUserData();
            }
            catch (Exception ex)
            {
                throw new Exception($"[유저 정보 로드 실패] {ex.Message}", ex);
            }
        }

        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // 3. 가챠 데이터 검사
        try
        {
            UpdateStatus("시즌 콘텐츠 데이터 확인 중 ...");
            await GatchaDataManager.Instance.Get_GatchaDTO();
        }
        catch (Exception ex)
        {
            throw new Exception($"[시즌 데이터 확보 실패] {ex.Message}", ex);
        }

        // 4. 날짜 변경 및 갱신 프로세스
        string NowDateTime = TimeManager.Instance.CurrentDate;
        string LastDateTime = LocalUserDataManager.Instance.LastDate;

        if (LastDateTime != NowDateTime)
        {
            GatchaDataManager.Instance.OnDailyReset();
            LocalFeedStorage.GetRandomSix();
            try
            {
                UpdateStatus("날짜를 갱신 하였습니다.");
                await LocalUserDataManager.Instance.UpdateLastDate(NowDateTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"[날짜 동기화 실패] {ex.Message}", ex);
            }
        }

        // 5. 출석 보상 검사
        try
        {
            UpdateStatus("출석 보상 확인 중");
            await GatchaDataManager.Instance.IsCompensation();
        }
        catch (Exception ex)
        {
            throw new Exception($"[출석 보상 판정 실패] {ex.Message}", ex);
        }

        // 6. 로컬 피드 스토리지 연산
        var localFeedStorage = LocalFeedStorage.LoadPosts(user.UserId, "RandomFeeds");
        if (localFeedStorage.Count <= 1)
        {
            try
            {
                UpdateStatus("피드 정보 초기화...");
                await SNSPostManager.Instance.RefreshRandomFeedsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"[피드 초기화 실패] {ex.Message}", ex);
            }
        }

        // 7. 자정 이벤트 등록
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnOClock -= GatchaDataManager.Instance.OnDailyReset;
            TimeManager.Instance.OnOClock += GatchaDataManager.Instance.OnDailyReset;

            TimeManager.Instance.OnOClock -= LocalFeedStorage.GetRandomSix;
            TimeManager.Instance.OnOClock += LocalFeedStorage.GetRandomSix;
        }
    }

    
}

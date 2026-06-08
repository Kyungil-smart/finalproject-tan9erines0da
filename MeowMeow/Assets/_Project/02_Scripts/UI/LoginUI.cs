using System;
using System.Threading.Tasks;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _logoutButton;
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
        UpdateStatus($"환영합니다, {GetDisplayName(user)}님");

        // 로그인시 유저 데이터를 얻어오는 함수
        await InitUserData();
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

        UpdateStatus($"환영합니다, {GetDisplayName(user)}님");

        // 로그인시 유저 데이터를 얻어오는 함수
        await InitUserData();
    }

    private void OnLogoutClicked()
    {
        if (_isProcessing) return;
        UnityAuthService.SignOut();
        GoogleSignInService.SignOut();
        UpdateStatus("로그아웃");
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

    // 로그인시 유저 데이터를 얻어오는 함수
    private async Task InitUserData()
    {
        if (SNSPostManager.Instance != null)
        {
            SNSPostManager.Instance.LoadLocalData();
        }

        if (LocalDataManager.Instance != null)
        {
            await LocalDataManager.Instance.LoadNyangNyangStone();
        }
    }
}

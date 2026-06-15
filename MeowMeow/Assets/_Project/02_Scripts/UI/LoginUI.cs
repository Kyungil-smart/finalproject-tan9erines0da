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

    // 로그인 확인후 실행되는 유저관련 데이터 초기화 함수
    private async Task InitUserData()
    {
        // 유저가 작성한 포스트 확보
        if (SNSPostManager.Instance != null)
        {
            SNSPostManager.Instance.LoadLocalData();
        }
        
        
        // 유저 보유 재화 확보
        if (LocalDataManager.Instance != null)
        {
            await LocalDataManager.Instance.LoadNyangNyangStone();
        }
        

        FirebaseUser user = BackendManager.Auth.CurrentUser;
        /*
         
         */
        GatchaDataManager.Instance.Get_GatchaDTO();
        GatchaDataManager.Instance.IsCompensation();
       var localFeedStorage = LocalFeedStorage.LoadPosts(user.UserId, "RandomFeeds");
        
        // 유저 로컬 데이터에 랜덤피드 6개 확보
        if (localFeedStorage.Count > 1)
        {
            //SubscribeManager.instance.Publish(SubscribeType.RandomSixData, localFeedStorage);
        }
        else
        {
            // 로컬에 데이터가 존재하지 않을 때
            var Listdata = await FireStoreManager.DocumentType(DataType.Posts).GetRandomSixData<FirestoreSNSPostDoc>();
            var SNSList = new List<SNSPostDTO>();
            foreach (var item in Listdata)
            {
                SNSList.Add(item.ToStruct());
            }
            LocalFeedStorage.SavePosts(user.UserId, "RandomFeeds", SNSList);
            //SubscribeManager.instance.Publish(SubscribeType.RandomSixData, SNSList);
        }
    }
}

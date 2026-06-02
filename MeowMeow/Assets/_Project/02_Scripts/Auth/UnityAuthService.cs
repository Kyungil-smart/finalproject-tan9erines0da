using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class UnityAuthService
{
    // 다른 값으로 설정 했다면 수정  
    private const string PROVIDER_ID = "oidc-firebase";

    public static async Task InitializeAsync()
    {
        try
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;

            await UnityServices.InitializeAsync();
            Debug.Log("UnityAuthService: UGS Core 초기화 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"UnityAuthService: 초기화 실패({e.Message})");
            throw;
        }
    }

    public static async Task SignInWithGoogleAsync(string firebaseToken)
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log($"UnityAuthService: 이미 로그인 상태, PlayerID = {AuthenticationService.Instance.PlayerId}");
                return;
            }

            await AuthenticationService.Instance.SignInWithOpenIdConnectAsync(PROVIDER_ID, firebaseToken);
            Debug.Log($"UnityAuthService: UGS 로그인 완료, PlayerID = {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"UnityAuthService: UGS 로그인 실패({e.Message})");
            throw;
        }
    }

    public static void SignOut()
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;

        AuthenticationService.Instance.SignOut(clearCredentials: true);
        Debug.Log("UnityAuthService: UGS 로그아웃");
    }
}

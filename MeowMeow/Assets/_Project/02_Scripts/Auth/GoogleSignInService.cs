using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public static class GoogleSignInService
{
    // google-services.json 의 oauth_client 중 client_type:3 (Web 클라이언트) 의 client_id  
    // 자신에게 맞는 값으로 바꿔야 함  
    private const string WEB_CLIENT_ID = "897795581002-qr7s38o0cpr5lcaa0ieepsr37futefh0.apps.googleusercontent.com";

    public static async Task<FirebaseUser> SignInAsync()
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            
            string idToken = await GetGoogleIdTokenFromCredentialManagerAsync();  
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            
            FirebaseUser user = await BackendManager.Auth.SignInWithCredentialAsync(credential);  
            Debug.Log($"GoogleSignInService: Android Credential Manager 로그인 완료 - UID={user.UserId}");  
            return user;
#elif UNITY_IOS && !UNITY_EDITOR
            // iOS 환경에서의 로그인  
            // ...  
            // ...
#else
            return null;
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"GoogleSignInService: 로그인 실패 - {e.Message}");
            throw;
        }
    }

    public static void SignOut()
    {
        if (BackendManager.Auth?.CurrentUser == null) return;
        BackendManager.Auth.SignOut();
        Debug.Log("GoogleSignInService: Firebase 로그아웃");
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static Task<string> GetGoogleIdTokenFromCredentialManagerAsync()  
    {  
        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

        try  
        {  
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");  
            using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using AndroidJavaObject googleIdOptionBuilder = new AndroidJavaObject(  
                "com.google.android.libraries.identity.googleid.GetGoogleIdOption$Builder");  
            googleIdOptionBuilder.Call<AndroidJavaObject>("setFilterByAuthorizedAccounts", false);  
            googleIdOptionBuilder.Call<AndroidJavaObject>("setServerClientId", WEB_CLIENT_ID);  
            googleIdOptionBuilder.Call<AndroidJavaObject>("setAutoSelectEnabled", false);  
            AndroidJavaObject googleIdOption = googleIdOptionBuilder.Call<AndroidJavaObject>("build");

            using AndroidJavaObject requestBuilder = new AndroidJavaObject(  
                "androidx.credentials.GetCredentialRequest$Builder");  
            requestBuilder.Call<AndroidJavaObject>("addCredentialOption", googleIdOption);  
            AndroidJavaObject credentialRequest = requestBuilder.Call<AndroidJavaObject>("build");

            AndroidJavaObject credentialManager = new AndroidJavaClass("androidx.credentials.CredentialManager")  
                .CallStatic<AndroidJavaObject>("create", activity);  
            AndroidJavaObject mainExecutor = new AndroidJavaClass("androidx.core.content.ContextCompat")  
                .CallStatic<AndroidJavaObject>("getMainExecutor", activity);

            CredentialCallbackProxy callback = new CredentialCallbackProxy(tcs);  
            credentialManager.Call(  
                "getCredentialAsync",  
                activity,             
                credentialRequest,    
                null,                 
                mainExecutor,         
                callback);            
        }  
        catch (Exception e)  
        {  
            tcs.SetException(e);  
        }

        return tcs.Task;  
    }

    private class CredentialCallbackProxy : AndroidJavaProxy  
    {  
        private readonly TaskCompletionSource<string> _tcs;

        public CredentialCallbackProxy(TaskCompletionSource<string> tcs)  
            : base("androidx.credentials.CredentialManagerCallback")  
        {  
            _tcs = tcs;  
        }

        public void onResult(AndroidJavaObject getCredentialResponse)  
        {  
            try  
            {  
                using AndroidJavaObject credential = getCredentialResponse.Call<AndroidJavaObject>("getCredential");

                using AndroidJavaObject data = credential.Call<AndroidJavaObject>("getData");

                using AndroidJavaObject googleIdTokenCredential = new AndroidJavaClass(  
                    "com.google.android.libraries.identity.googleid.GoogleIdTokenCredential")  
                    .CallStatic<AndroidJavaObject>("createFrom", data);

                string idToken = googleIdTokenCredential.Call<string>("getIdToken");  
                _tcs.SetResult(idToken);  
            }  
            catch (Exception e)  
            {  
                _tcs.SetException(e);  
            }  
        }

        public void onError(AndroidJavaObject getCredentialException)  
        {  
            try  
            {  
                string message = getCredentialException.Call<string>("getMessage");  
                _tcs.SetException(new Exception($"Credential Manager 에러: {message}"));  
            }  
            catch (Exception e)  
            {  
                _tcs.SetException(e);  
            }  
        }  
    }
#endif
}

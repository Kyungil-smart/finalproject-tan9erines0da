using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PopupSpriteCacheManager : MonoBehaviour
{
    private static PopupSpriteCacheManager _instance;
    public static PopupSpriteCacheManager Instance => _instance;

    [Header("등록된 어드레서블의 키값을 입력해 주세요.")]
    [SerializeField] private List<string> addresses;

    // Addressables key를 기준으로 Sprite 로딩 비동기 핸들을 캐싱하는 딕셔너리
    // (핸들 내부에 Sprite 결과, 로딩 상태, 참조 카운트 정보 포함)
    private Dictionary<string, AsyncOperationHandle<Sprite>> _popupSpriteCache = new Dictionary<string, AsyncOperationHandle<Sprite>>();

    private void Awake()
    {
        Init();
    }

    private async void Start()
    {
        await PreloadAsync(addresses);
    }

    #region 초기화 함수
    private void Init()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // Addressables Sprite를 미리 로드해서 캐시에 저장합니다.
    private async Task PreloadAsync(List<string> addresses)
    {
        foreach (string address in addresses)
        {
            if (_popupSpriteCache.ContainsKey(address)) continue;

            // Sprite 비동기 로드 요청 (핸들 생성)
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
            // 로딩 완료까지 대기
            await handle.Task;

            // 로드 성공한 경우만 캐시에 저장
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _popupSpriteCache[address] = handle;
            }
        }
    }

    /// <summary>
    /// 캐싱된 Addressables Sprite를 반환합니다.
    /// </summary>
    /// <param name="addressKey">Addressables Sprite의 key값을 입력해주세요.</param>
    /// <returns></returns>
    public Sprite GetPopupSprite(string addressKey)
    {
        return _popupSpriteCache[addressKey].Result;
    }

    /// <summary>
    /// 캐싱된 Addressables Sprite를 모두 해제하고 메모리에서 정리하는 함수
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var handle in _popupSpriteCache.Values)
        {
            Addressables.Release(handle);
        }

        _popupSpriteCache.Clear();
    }
}

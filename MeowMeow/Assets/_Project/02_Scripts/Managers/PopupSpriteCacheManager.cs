using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PopupSpriteCacheManager : MonoBehaviour
{
    private static PopupSpriteCacheManager _instance;
    public static PopupSpriteCacheManager Instance => _instance;

    // "sprite" 라벨로 로드한 모든 Sprite의 Addressables 핸들
    // Release 시 사용하기 위해 보관
    private AsyncOperationHandle<IList<Sprite>> _loadHandle;

    private Dictionary<string, Sprite> _popupSpriteCache = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Init();
    }

    private async void Start()
    {
        await PreloadAsync();
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
    private async Task PreloadAsync()
    {
        // Sprite들을 한 번에 비동기 로드
        _loadHandle = Addressables.LoadAssetsAsync<Sprite>("sprite", null);
        // 모든 Sprite 로드가 완료될 때까지 대기
        await _loadHandle.Task;
        // 로드 실패 시 종료
        if (_loadHandle.Status != AsyncOperationStatus.Succeeded) return;
        // Sprite 이름을 Key로 하여 캐시에 저장
        foreach (Sprite sprite in _loadHandle.Result)
        {
            _popupSpriteCache[sprite.name] = sprite;
        }
    }

    /// <summary>
    /// 캐싱된 Addressables Sprite를 반환합니다.
    /// </summary>
    /// <param name="addressKey">Addressables Sprite의 key값을 입력해주세요.</param>
    /// <returns></returns>
    public Sprite GetPopupSprite(string addressKey)
    {
        return _popupSpriteCache[addressKey];
    }

    /// <summary>
    /// 캐싱된 Addressables Sprite를 모두 해제하고 메모리에서 정리하는 함수
    /// </summary>
    public void ReleaseAll()
    {
        if (_loadHandle.IsValid())
        {
            Addressables.Release(_loadHandle);
        }

        _popupSpriteCache.Clear();
    }
}

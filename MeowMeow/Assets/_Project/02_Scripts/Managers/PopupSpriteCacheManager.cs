using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PopupSpriteCacheManager : MonoBehaviour
{
    private static PopupSpriteCacheManager _instance;
    public static PopupSpriteCacheManager Instance => _instance;

    private AsyncOperationHandle<IList<Sprite>> _loadHandle;

    // Addressables key를 기준으로 Sprite 로딩 비동기 핸들을 캐싱하는 딕셔너리
    // (핸들 내부에 Sprite 결과, 로딩 상태, 참조 카운트 정보 포함)
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
        _loadHandle = Addressables.LoadAssetsAsync<Sprite>("sprite", null);

        await _loadHandle.Task;

        if (_loadHandle.Status != AsyncOperationStatus.Succeeded) return;

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

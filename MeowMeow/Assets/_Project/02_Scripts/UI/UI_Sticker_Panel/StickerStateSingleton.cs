using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StickerStateSingleton : MonoBehaviour
{
    public static StickerStateSingleton Instance { get; private set; }

    #region 스티커 개수 제한 변수들
    [Header("Sticker_Count_Text를 참조")]
    [SerializeField] private TextMeshProUGUI _stickerCountText;

    private int _currentCount;

    public int CurrentCount
    {
        get => _currentCount;
        set
        {
            _currentCount = Mathf.Clamp(value, 0, _maxStickerCount);
        }
    }

    private int _maxStickerCount = 5;

    public int MaxStickerCount
    {
        get => _maxStickerCount;
    }
    #endregion

    #region 스티커와 스티커 우선순위 버튼 묶음
    [System.Serializable]
    public class StickerPair
    {
        public GameObject sticker;
        public GameObject button;
        public int stickerIndex;
    }

    public List<StickerPair> stickers = new List<StickerPair>();
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region 스티커 개수 출력 함수
    public void StickerCountUpload()
    {
        _stickerCountText.text = $"{_currentCount}/{_maxStickerCount}";
    }
    #endregion
}

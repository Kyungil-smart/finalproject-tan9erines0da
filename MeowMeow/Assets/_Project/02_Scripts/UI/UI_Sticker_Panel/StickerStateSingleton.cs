using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StickerStateSingleton : MonoBehaviour
{
    public static StickerStateSingleton Instance { get; private set; }

    #region 스티커 개수 제한 관련 변수들
    // 개수 제한을 표시할 TMP
    [Header("Sticker_Count_Text를 참조")]
    [SerializeField] private TextMeshProUGUI _stickerCountText;

    [Header("Sticker_Limit_Reached를 참조")]
    [SerializeField] private StickerLimitTweenAni _stickerLimitTweenAni;

    // 현재 스티커 개수
    private int _currentCount;
    public int CurrentCount
    {
        get => _currentCount;
        set
        {
            _currentCount = Mathf.Clamp(value, 0, _maxStickerCount);
        }
    }

    // 스티커 제한 개수
    private int _maxStickerCount = 5;
    public int MaxStickerCount
    {
        get => _maxStickerCount;
    }

    // 스티커 제한 상황에서 스티커 생성 버튼 클릭시
    // ObjectPinchScaler의 OnUnselect()를 블럭 시키기 위한 마커
    public bool BlockUnselect { get; set; }
    #endregion

    #region 스티커 생성 관련 참조할 변수들
    [Header("생성할 스티커 프리펩 참조")]
    [SerializeField] private Image _stickerImage;

    [Header("생성할 스티커 삭제버튼 프리펩 참조")]
    [SerializeField] private Button _stickerDelButton;

    // 스티커 삭제버튼을 위한 캔버스(이후 Sticker_Canvas와 StickerDelButton_Canvas는 같이 켜고, 꺼져야 합니다.)
    [Header("StickerDelButton_Canvas를 참조")]
    [SerializeField] private Canvas _stickerDelButtonCanvas;

    // 스티커가 생성될 부모 오브젝트
    [Header("프리뷰 이미지를 참조")]
    [SerializeField] private Image _targetImage;

    // 인덱스 기반으로 이미지 데이터를 넘겨주기 위해서
    [Header("StickerDB(SO)파일을 참조")]
    [SerializeField] private StickerImageDatabase _stickerDB;
    public StickerImageDatabase StickerDB
    {
        get => _stickerDB;
        set => _stickerDB = value;
    }

    [Header("생성할 스티커 생선순 토글버튼 참조")]
    [SerializeField] private Toggle _stickerToggle;

    // 스티커 생선순 토글버튼 생성 위치
    [Header("Sticker_Priority_Scroll View의 자식 Content를 참조")]
    [SerializeField] private RectTransform _content;

    [Header("스티커 생성 버튼들을 모두 참조")]
    [SerializeField] private List<StickerEditor> _stickerEditors = new List<StickerEditor>();
    #endregion

    #region 스티커 생성 관련 자료구조들
    // 스티커의 이미지 인덱스를 저장하는 딕셔너리
    private Dictionary<GameObject, int> _stickerIndexes = new Dictionary<GameObject, int>();
    public Dictionary<GameObject, int> StickerIndexes
    {
        get => _stickerIndexes;
        set => _stickerIndexes = value;
    }

    // 스티커 생선순 토글버튼이 선택되면 스티커가 선택되게 하기위한 의도의 딕셔너리 
    private Dictionary<Toggle, GameObject> _toggleToSticker = new Dictionary<Toggle, GameObject>();
    public Dictionary<Toggle, GameObject> ToggleToSticker
    {
        get => _toggleToSticker;
        set => _toggleToSticker = value;
    }

    // 스티커 삭제하면 토글도 같이 삭제되게 하기위한 의도의 딕셔너리 
    private Dictionary<GameObject, Toggle> _stickerToToggle = new Dictionary<GameObject, Toggle>();
    public Dictionary<GameObject, Toggle> StickerToToggle
    {
        get => _stickerToToggle;
        set => _stickerToToggle = value;
    }

    // 스티커 생선순 토글버튼을 담을 리스트(숫자표시를 바꾸기 위한 용도)
    private List<Toggle> _toggleList = new List<Toggle>();
    public List<Toggle> ToggleList
    {
        get => _toggleList;
        set => _toggleList = value;
    }

    // 스티커를 키로 삭제버튼을 담을 딕셔너리(삭제버튼 On/Off를 위한 용도)
    private Dictionary<GameObject, GameObject> _stickerToDelButton = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, GameObject> StickerToDelButton
    {
        get => _stickerToDelButton;
        set => _stickerToDelButton = value;
    }
    #endregion

    // 스티커 생선순 토글버튼 숫자표시를 바꾸기 위한 이벤트 액션
    public event Action StickerPriorityButtonChanged;
    // 스티커 삭제버튼을 켜기 위한 이벤트 액션
    public event Action<GameObject> StickerDelButtonOn;
    // 스티커 삭제버튼을 끄기 위한 이벤트 액션
    public event Action StickerDelButtonOff;

    private void Awake()
    {
        Init();
    }

    #region 초기화 함수
    public void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 등록된 버튼에 버튼 자신의 인덱스 부여
        for (int i = 0; i < _stickerEditors.Count; i++)
        {
            _stickerEditors[i].MyIndex = i;
        }
    }
    #endregion

    #region 스티커 생성 함수
    /// <summary>
    /// 버튼에서 스티커를 생성할 함수입니다.
    /// </summary>
    /// <param name="stickerIndex">해당 버튼 자신의 인덱스를 넣어주세요.</param>
    public void SetSticker(int stickerIndex)
    {
        if (CurrentCount >= MaxStickerCount)
        {
            // 효과음
            SoundManager.Instance.Invoke(AudioType.SFX_UI_Error);
            _stickerLimitTweenAni.PlayAnimation();
            return;
        }

        // 스티커 생성
        GameObject sticker = Instantiate(_stickerImage.gameObject, _targetImage.transform);
        StickerDelOff stickerDelOff = sticker.GetComponent<StickerDelOff>();

        // 스티커 삭제버튼 생성
        GameObject stickerDel = Instantiate(_stickerDelButton.gameObject, _stickerDelButtonCanvas.transform);
        DelSticker delSticker = stickerDel.GetComponent<DelSticker>();

        // StickerDelOff 스크립트에 삭제버튼 초기화
        stickerDelOff.InitStickerDelOff(stickerDel);
        // delSticker 스크립트에 스티커 초기화
        delSticker.InitDelSticker(sticker);

        // 스티커에 현재 선택한 이미지 넣기
        sticker.GetComponent<Image>().sprite = _stickerDB.GetSprite(stickerIndex);

        // 스티커 위치 조정
        RectTransform stickerRect = sticker.GetComponent<RectTransform>();
        stickerRect.anchoredPosition = Vector2.zero;
        stickerRect.localScale = Vector3.one;

        // 스티커에 들어간 이미지 정보를 저장할 딕셔너리에 데이터 넣기
        _stickerIndexes.Add(sticker, stickerIndex);

        // 스티커 생선순 토글버튼 생성 
        Toggle priorityToggle = Instantiate(_stickerToggle, _content);

        // 각 자료구조에 데이터 저장
        _toggleList.Add(priorityToggle);
        _toggleToSticker.Add(priorityToggle, sticker);
        _stickerToToggle.Add(sticker, priorityToggle);
        _stickerToDelButton.Add(sticker, stickerDel);

        // 삭제버튼이 스티커를 따라가게 하기위해 StickerDelFollow 스크립트 초기화
        StickerDelFollow stickerDelFollow = stickerDel.GetComponent<StickerDelFollow>();
        stickerDelFollow.InitStickerDelFollow(stickerRect);

        // 스티커 생선순 토글버튼 번호 갱신
        RefreshPriorityButtons();

        // 터치로 움직이기 위해 타겟에 넣기
        TouchInputHandler.Instance.CallObjectSelectedForToggle(sticker.GetComponent<TouchInteractor>());

        // 스티커 제한 개수 증가 및 TMP 갱신
        CurrentCount++;
        StickerCountUpload();
    }
    #endregion

    #region 스티커 생선순 토글버튼 숫자표시를 바꾸기 위한 함수
    /// <summary>
    /// 스티커 생선순 토글버튼 숫자표시 갱신용 함수입니다.
    /// </summary>
    public void RefreshPriorityButtons()
    {
        StickerPriorityButtonChanged?.Invoke();
    }
    #endregion

    #region 스티커 삭제버튼 On/Off 함수
    /// <summary>
    /// 스티커 삭제버튼을 On/Off 하는 함수입니다.
    /// </summary>
    /// <param name="target">삭제버튼을 On/Off할 현재 오브젝트</param>
    public void StickerDelButtonSetOn(GameObject target)
    {
        StickerDelButtonOn?.Invoke(target);
    }

    /// <summary>
    /// 스티커 삭제버튼을 Off 하는 합수입니다.
    /// </summary>
    public void StickerDelButtonSetOff()
    {
        StickerDelButtonOff?.Invoke();
    }
    #endregion

    #region 스티커 개수제한 출력 함수
    /// <summary>
    /// 스티커 개수제한 TMP를 갱신하기 위한 함수입니다.
    /// </summary>
    public void StickerCountUpload()
    {
        _stickerCountText.text = $"{_currentCount}/{_maxStickerCount}";
    }
    #endregion

    #region 스티커 초기화 함수
    public void AllClearSticker()
    {
        _currentCount = 0;
        StickerCountUpload();

        foreach (Toggle toggle in _toggleList)
        {
            if (toggle != null)
            {
                Destroy(toggle.gameObject);
            }
        }

        foreach (KeyValuePair<GameObject, GameObject> pair in _stickerToDelButton)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }

        foreach (KeyValuePair<Toggle, GameObject> pair in _toggleToSticker)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }

        _stickerIndexes.Clear();
        _toggleToSticker.Clear();
        _stickerToToggle.Clear();
        _toggleList.Clear();
        _stickerToDelButton.Clear();
    }
    #endregion
}

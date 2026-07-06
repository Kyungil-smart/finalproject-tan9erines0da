using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LocalUserDataManager : MonoBehaviour
{
    public static LocalUserDataManager Instance { get; private set; }

    [Header("냥냥스톤 재화를 출력할 TMP를 참조")]
    [SerializeField] private TextMeshProUGUI NyangNyangStoneTMP;

    // 냥냥스톤 재화
    private int _nyangNyangStone;
    public int NyangNyangStone
    {
        get => _nyangNyangStone;
        set
        {
            int newValue = Mathf.Max(0, value);

            if (_nyangNyangStone == newValue)
                return;

            _nyangNyangStone = newValue;
            UpdateNyangNyangTMP();
        }
    }

    private string _lastDate;
    public string LastDate => _lastDate;

    private void Awake()
    {
        Init();
    }

    #region 초기화 함수
    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    #region FireStore 관련 함수(냥냥스톤)
    // FireStore에서 CurrencyDTO 데이터를 얻어옵니다.
    private async Task<CurrencyDTO> GetCurrencyAsync()
    {
        CurrencyDTO currency;

        currency = await FireStoreManager.DocumentType(DataType.CurrencyData).GetAsync<CurrencyDTO>();

        // 데이터가 없을때 예외처리 추가
        if(currency == null)
        {
            await SetCurrencyAsync();
            return new CurrencyDTO { NyangNyangStone = 10 };
        }

        return currency;
    }

    // FireStore에 CurrencyDTO 데이터를 생성/저장합니다.
    /// <summary>
    /// 최초 로그인시에만 한번 호출해 주세요.(FireStore에 CurrencyDTO 데이터를 생성/저장합니다.)
    /// </summary>
    /// <returns></returns>
    public async Task SetCurrencyAsync()
    {
        CurrencyDTO currencyDTO = new CurrencyDTO
        {
            NyangNyangStone = 10
        };

        await FireStoreManager.DocumentType(DataType.CurrencyData).SetAsync(currencyDTO);
    }

    public async Task SetUserData()
    {
        CurrencyDTO currencyDTO = new CurrencyDTO
        {
            NyangNyangStone = _nyangNyangStone,
            LastDate = _lastDate
        };

        await FireStoreManager.DocumentType(DataType.CurrencyData).SetAsync(currencyDTO);
    }

    // FireStore에서 NyangNyangStone을 갱신합니다.
    private async Task UpdateCurrencyAsync(int amount)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>
      {
            { "NyangNyangStone", FieldValue.Increment(amount) }
      };

        await FireStoreManager.DocumentType(DataType.CurrencyData).UpdateAsync<CurrencyDTO>(updates, true);
    }

    // FireStore 재화 로컬에 반영
    public async Task LoadUserData()
    {
        CurrencyDTO currencyDTO;
        currencyDTO = await GetCurrencyAsync();

        if (currencyDTO == null)
        {
            this.PublishLog("재화 널 발생");
            currencyDTO = new();
        }
        NyangNyangStone = currencyDTO.NyangNyangStone;

        if (currencyDTO.LastDate == null || string.IsNullOrWhiteSpace(currencyDTO.LastDate))
        {
            _lastDate = TimeManager.Instance.CurrentDate;
            
            await SetUserData();
        }
    }

    // 재화 증가 + 서버 저장
    /// <summary>
    /// NyangNyangStone의 재화를 증가하는 함수(증가량 자동으로 서버 저장)
    /// </summary>
    /// <param name="amount">증가시킬 재화량을 인자로 넣어주세요.</param>
    /// <returns></returns>
    public async Task AddNyangNyangStone(int amount)
    {
        NyangNyangStone += amount;
        await UpdateCurrencyAsync(amount);
    }

    // 재화 감소 + 서버 저장
    /// <summary>
    /// NyangNyangStone의 재화를 감소하는 함수(감소량 자동으로 서버 저장)
    /// </summary>
    /// <param name="amount">감소시킬 재화량을 양수값으로 인자로 넣어주세요.(음수로 넣어도 양수로 보정합니다.)</param>
    /// <returns></returns>
    public async Task SubNyangNyangStone(int amount)
    {
        int value = Mathf.Abs(amount);

        NyangNyangStone -= value;
        await UpdateCurrencyAsync(-value);
    }
    #endregion

    #region 냥냥스톤 재화 TMP 출력
    private void UpdateNyangNyangTMP()
    {
        if (NyangNyangStoneTMP == null) return;

        NyangNyangStoneTMP.text = NyangNyangStone.ToString();
    }
    #endregion

    #region FireStore 테스트용
    [ContextMenu("테스트용 더하기(+2)")]
    public async Task TAddNyangNyangStone()
    {
        await AddNyangNyangStone(2);
    }

    [ContextMenu("테스트용 빼기(+2)")]
    public async Task TSubNyangNyangStone()
    {
        await SubNyangNyangStone(-2);
    }
    #endregion
}


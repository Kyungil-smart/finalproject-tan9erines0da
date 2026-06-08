using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class LocalDataManager : MonoBehaviour
{
    public static LocalDataManager Instance { get; private set; }

    [Header("냥냥스톤 재화를 출력할 TMP를 참조")]
    public TextMeshProUGUI NyangNyangStoneTMP;

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

        //DontDestroyOnLoad(gameObject);

        UpdateNyangNyangTMP();
    }
    #endregion

    #region FireStore 관련 함수(냥냥스톤)
    // FireStore에서 재화 데이터 로드
    public async Task LoadCurrency()
    {
        CurrencyDTO currencyDTO = await FireStoreManager.Instance.GetCurrencyAsync();
        NyangNyangStone = currencyDTO.NyangNyangStone;
    }

    // FireStore에 현재 재화 값 저장
    public async Task UpdateCurrency()
    {
        await FireStoreManager.Instance.UpdateCurrencyAsync(NyangNyangStone);
    }

    // 재화 증가 + 서버 저장
    public async Task AddNyangNyangStone(int amount)
    {
        NyangNyangStone += amount;
        await UpdateCurrency();
    }

    // 재화 감소 + 서버 저장
    public async Task SubNyangNyangStone(int amount)
    {
        NyangNyangStone -= amount;
        await UpdateCurrency();
    }
    #endregion

    #region 냥냥스톤 재화 TMP 출력
    private void UpdateNyangNyangTMP()
    {
        if (NyangNyangStoneTMP == null) return;

        NyangNyangStoneTMP.text = NyangNyangStone.ToString();
    }
    #endregion
}


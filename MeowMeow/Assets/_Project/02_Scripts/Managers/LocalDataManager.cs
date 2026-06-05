using System;
using UnityEngine;

public class LocalDataManager : MonoBehaviour
{
    public static LocalDataManager Instance { get; private set; }

    [Header("FireStoreCurrencyDataManager 오브젝트를 참조")]
    public FireStoreCurrencyDataManager FireStoreCurrencyDataManager;

    private int _nyangNyangStone;
    public int NyangNyangStone
    {
        get => _nyangNyangStone;
        set
        {
            _nyangNyangStone = Mathf.Max(0, value);
            OnNyangNyangStoneChanged?.Invoke(_nyangNyangStone);
        }
    }

    public event Action<int> OnNyangNyangStoneChanged;

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

    public async void SaveCurrency()
    {
        CurrencyDTO data = new CurrencyDTO
        {
            NyangNyangStone = this.NyangNyangStone
        };

        await FireStoreManager.DocumentType(DataType.CurrencyData).SetAsync(data);
    }
}


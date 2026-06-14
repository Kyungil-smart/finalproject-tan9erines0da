using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatchaDataManager : MonoBehaviour
{
    public GatchaDataManager Instance { get; private set; }

   [SerializeField] private GatchaDTO _gatchaData=new GatchaDTO();
      public GatchaDTO GatchaData => _gatchaData;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    [ContextMenu("setTest")]
    private async void Test_set()
    {
       var  TestGatchaDTO = new GatchaDTO();
        await FireStoreManager.DocumentType(DataType.GatchaData).SetAsync(TestGatchaDTO);
    }
    [ContextMenu("getTest")]
    private async void Test_get()
    {
        
        _gatchaData =await FireStoreManager.DocumentType(DataType.GatchaData).GetAsync<GatchaDTO>();
    }
}

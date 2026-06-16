using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;  }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        Scribe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Scribe()
    {
        if (SubscribeManager.instance != null)
        {
            SubscribeManager.instance.Subscribe(SubscribeType.On_LoginComplete, Scribe);
        }
    }
}

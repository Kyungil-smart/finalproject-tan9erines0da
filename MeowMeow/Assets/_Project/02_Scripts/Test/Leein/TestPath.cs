using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestPath : MonoBehaviour
{
    public TextMeshProUGUI test;
    private void OnEnable()
    {
        SubscribeManager.instance.Subscribe<string>(SubscribeType.Test_Path, SetText);
    }
    private void OnDisable()
    {
        SubscribeManager.instance.Unsubscribe<string>(SubscribeType.Test_Path, SetText);
    }

    public void SetText(string path)
    {
        test.text= path;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaStackBox : MonoBehaviour, ISwitchable
{
    [SerializeField] private GameObject _stamp;

    public void SetView(bool isOpend)
    {
        if (_stamp == null) return;

        _stamp.SetActive(!isOpend);
    }
}

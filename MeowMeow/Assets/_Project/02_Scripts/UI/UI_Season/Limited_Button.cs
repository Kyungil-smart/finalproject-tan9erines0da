using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limited_Button : MonoBehaviour, ISwitchable
{
    [SerializeField] private GameObject _locker;
    [SerializeField] private GameObject _stamp;

    public void SetView(bool isOpend)
    {
        if (_stamp == null) return;

        _locker.SetActive(!isOpend);
        _stamp.SetActive(isOpend);
    }
}

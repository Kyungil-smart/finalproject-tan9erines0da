using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Limited_Button : MonoBehaviour, ISwitchable
{
    [SerializeField] private GameObject _locker;
    [SerializeField] private GameObject _stamp;
    [SerializeField] private Button _button;

    public void SetView(bool isOpend)
    {
        if (_stamp == null) return;

        _locker.SetActive(!isOpend);
        _stamp.SetActive(isOpend);
        _button.interactable = !isOpend;
    }
}

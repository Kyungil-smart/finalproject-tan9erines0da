using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ImageDropdown : MonoBehaviour
{
    private TMP_Dropdown _tmpDropdown;

    private List<string> _dropdownList = new List<string>()
    {
    "모든 사진",
    "뭉치",
    "일상"
    };

    private void Awake()
    {
        _tmpDropdown = GetComponent<TMP_Dropdown>();

        _tmpDropdown.ClearOptions();
        _tmpDropdown.AddOptions(_dropdownList);
    }

    private void OnEnable()
    {
        _tmpDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDisable()
    {
        _tmpDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int index)
    {
        Debug.Log(_dropdownList[index]);
    }
}

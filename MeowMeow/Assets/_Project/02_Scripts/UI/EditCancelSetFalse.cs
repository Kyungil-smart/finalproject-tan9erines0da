using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditCancelSetFalse : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private GameObject _editCancelPopup;

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(SetActiveFalse);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(SetActiveFalse);
    }

    private void SetActiveFalse()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        _editCancelPopup.SetActive(false);
        if (TouchInputHandler.Instance.OnEditCancel == true)
        {
            TouchInputHandler.Instance.OnEditCancel = false;
        }
    }
}

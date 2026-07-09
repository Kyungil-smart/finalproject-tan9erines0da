using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryButtonSound : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickStorySound);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickStorySound);
    }

    private void OnClickStorySound()
    {
        SoundManager.Instance.Invoke(AudioType.SFX_UI_Error);
    }
}

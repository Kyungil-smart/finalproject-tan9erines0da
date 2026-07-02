using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimSound : MonoBehaviour
{
    public void BoxAnimSoundPlay()
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Chest_Open_1);
    }
}

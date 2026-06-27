using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName ="Audio_",menuName = "ScriptableObjects/Audio")]
public class AudioSO :ScriptableObject
{
    [SerializeField]private AudioType audioName;
    [SerializeField]private AudioClip m_clip;
    public AudioClip clip=>m_clip;
    public AudioType audio_Name => audioName;
}

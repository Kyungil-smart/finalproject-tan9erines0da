using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum AudioType
{
    ClickyButton3a,
    nyanv2_3,
    Paper,
    Popup3,
    Popup4b,
    SFX_Chest_Open_1,
    SFX_Player_Collect_Boxy_2,
    SFX_Player_Collect_Bright_1,
    SFX_Player_Collect_Bright_3,
    SFX_Player_Collect_Coin_3,
    SFX_Player_Collect_Pop_1,
    SFX_Pop_Bottle_Designed_1,
    SFX_Pop_Bubble_Single_1,
    SFX_Pop_Mouth_High_Sharp_1,
    SFX_UI_Button_Click_Settings_1 = 14,
    SFX_UI_Button_Click_Settings_Switch_1,
    SFX_UI_Error,
    SnappyButton5,
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]private  List<AudioSO> audioSOs = new List<AudioSO>();
    [SerializeField] private AudioSource m_defaultBackGroundSound;
    [SerializeField] private AudioSource m_reactiveProcessingSound;
    private Dictionary<AudioType, AudioClip> m_AuidoDic=new Dictionary<AudioType, AudioClip>();


    private void Awake()
    {

        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;

        }
        Instance = this;
        AutoSetting();
        m_AuidoDic = audioSOs.ToDictionary(x=>x.audio_Name, x=>x.clip);
        SetBackGroundSound();
        DontDestroyOnLoad(this.gameObject);
    }
    private void AutoSetting()
    {
        audioSOs.Clear();
        var SO = Resources.LoadAll<AudioSO>("AudioSO");
        audioSOs.AddRange(SO);
    }
    private void  SetBackGroundSound()
    {
        var data = GetAudioClip(AudioType.nyanv2_3);
        m_defaultBackGroundSound.clip = data;
        m_defaultBackGroundSound.Play();
        m_defaultBackGroundSound.loop = true;
    }
    private AudioClip GetAudioClip(AudioType key)
    {
        if (m_AuidoDic.TryGetValue(key, out var value))
        {
            return value;
        }
        else
        {
            Debug.LogError($"m_AuidoDic 에서 해당 키값 :{key} 없습니다.");
            return null;
        }

    }
    //가져다 쓰실 부분
    //플레이 할 사운드 이넘 넣어주면 된다.
    public void Invoke(AudioType value)
    {
       var item= GetAudioClip(value);
        if(item != null)
        {
            m_reactiveProcessingSound.clip = item;
            m_reactiveProcessingSound.Play();
        }
        else
        {
            Debug.Log("에러 발생");
        }
    }

    // 인스펙터에서 직접 쓰기 위한 함수
    public void PlaySFX(int at)
    {
        var item = GetAudioClip((AudioType)at);
        if (item != null)
        {
            m_reactiveProcessingSound.clip = item;
            m_reactiveProcessingSound.Play();
        }
        else
        {
            Debug.Log("에러 발생");
        }
    }
}

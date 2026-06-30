using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeReference] List<Object> initdata;
    [SerializeField] List<BaseEvent> m_eventList = new();
  
    private void Awake()
    {
        foreach (BaseEvent e in m_eventList)
        {
            e.init(initdata.ToArray());
        }
    }
    public void OnEvent(int index)
    {
       
        m_eventList[index].OnAnimationEvent();
    }
}

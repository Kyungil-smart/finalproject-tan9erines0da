using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "objectsDisable_", menuName = "ScriptableObjects/AnimactionEvent/objectsDisable")]
public class AnimactionEvent_Objectdisable : BaseEvent
{
    private List<GameObject> m_gameobjects=new();  
    public override void init(params object[] datas)
    {
        foreach (var obj in datas)
        {
            if(obj is GameObject data)
            {
                m_gameobjects.Add(data);
            }
        }
    }
    public override void OnDisableEvent()
    {
        m_gameobjects.Clear();
    }
    public override void OnAnimationEvent()
    {
        foreach (var obj in m_gameobjects.ToArray())
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}

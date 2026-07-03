using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;

public abstract class BaseEvent : ScriptableObject
{
    public abstract void init(params object[] datas);
    public abstract void OnDisableEvent();
    public abstract void OnAnimationEvent();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GatchaLogicType
{
    None,
    get,//Firestore에서 가져오기
    set,//Firestore에 저장하기
}

public class BaseGatcha : ScriptableObject
{
    public GatchaLogicType Type;
    public GatchaDataManager Owner;
    public virtual void Init(GatchaDataManager manager)
    {
        Owner=manager;
    }
    public virtual void Excute()
    {
        Debug.Log("BaseGatcha Excute");
    }
    public virtual void Excute(int value)
    {
        Debug.Log("BaseGatcha Excute");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum GatchaLogicType
{
    None,
    get,//Firestore에서 가져오기
    set,//Firestore에 저장하기
    InitFirstDic,//처음에 딕셔너리 초기화하기
    InitGatchaData,//GatchaData 초기화하기
    RewardReset,//초기 보상 설정다음으로  보상 초기화
    drawByIndex,//
    IsCompensation,//
    IsQuestCompensation,//
    ChangeGradeFlag,//
    ExecuteGacha,//
}

public class BaseGatcha : ScriptableObject
{
    public GatchaLogicType Type;
    public GatchaDataManager Owner;
    public virtual void Init(GatchaDataManager manager)
    {
        Owner=manager;
    }
     
    public virtual Task TaskExecute()
    {
        return Task.CompletedTask;
    }
    public virtual Task TaskExecute(int index)
    {
        return Task.CompletedTask;
    }


}

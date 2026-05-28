using UnityEngine;

public class Test2 : MonoBehaviour
{
    public googleSheetManager Manager;

 
    [ContextMenu("dsds")]
    public void dsds()
    {
        // 1. 매니저에서 Char2Data(SO)를 가져옵니다. 반환 타입이 Char2Data로 깔끔하게 나옵니다!
        var charSO = Manager.GetClassData<PlayerData>();

        if (charSO != null)
        {
            // 2. 형변환할 필요 없이 자식 클래스의 메서드를 바로 쓸 수 있습니다.
            var myAttackInfo = charSO.FindById("1");
            Debug.Log($"공격력: {myAttackInfo.m_Attack}");
        }
    }
}

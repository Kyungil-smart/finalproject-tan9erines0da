using TMPro;
using UnityEngine;

public class RefreshPrizeRank : MonoBehaviour, ISwitchable
{
    [SerializeField] private TextMeshProUGUI _1stText;
    [SerializeField] private TextMeshProUGUI _2ndText;
    [SerializeField] private TextMeshProUGUI _3rdText;

    public void SetView(bool isReset)
    {
        // 구글시트 매니저에서 데이터 테이블에 해당하는 SO 가져오기
        var db = googleSheetManager.instance.GetClassData<DrawBoardRewards>();
        foreach (var data in db.m_Data)
        {
                if (data.Grade == 1 && GatchaDataManager.Instance.Grade_1 && data.Repeat == isReset)
                {
                    _1stText.text = data.ItemName.Replace("(","\n(");
                }
                else if (data.Grade == 2 && GatchaDataManager.Instance.Grade_2 && data.Repeat == isReset)
                {
                    _2ndText.text = data.ItemName.Replace("(", "\n(");
                }
                else if (data.Grade == 3 && GatchaDataManager.Instance.Grade_3 && data.Repeat == isReset)
                {
                    _3rdText.text = data.ItemName.Replace("(", "\n(");
                }
        }
    }
}

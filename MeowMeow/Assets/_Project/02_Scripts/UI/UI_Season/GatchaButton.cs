using System.Threading.Tasks;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;

public class GatchaButton : MonoBehaviour, ISwitchable
{
    [SerializeField] private GameObject _front;
    [SerializeField] private GameObject _1st;
    [SerializeField] private GameObject _2nd;
    [SerializeField] private GameObject _3rd;
    [SerializeField] private GameObject _lowRank;
    [SerializeField] private TextMeshProUGUI _lowRankTMP;
    [SerializeField] private BoxCoverTweenAni _anim;

    public void SetView(bool isOpend)
    {
        if (_front == null) return;

        if (!isOpend) AllCloseView();

        _front.SetActive(!isOpend);
    }

    /// <summary>
    /// 인덱스를 바탕으로 아이템등급을 확인해서 뷰를 설정하는 함수입니다.
    /// </summary>
    /// <param name="i">인덱스를 입력해주세요.</param>
    public void SetViewCover(int i)
    {
        int grade = GetGrade(i);

        switch (grade)
        {
            case 1:
                _1st.SetActive(true);
                _2nd.SetActive(false);
                _3rd.SetActive(false);
                _lowRank.SetActive(false);
                break;
            case 2:
                _1st.SetActive(false);
                _2nd.SetActive(true);
                _3rd.SetActive(false);
                _lowRank.SetActive(false);
                break;
            case 3:
                _1st.SetActive(false);
                _2nd.SetActive(false);
                _3rd.SetActive(true);
                _lowRank.SetActive(false);
                break;
            default:
                _1st.SetActive(false);
                _2nd.SetActive(false);
                _3rd.SetActive(false);
                _lowRank.SetActive(true);
                _lowRankTMP.text = $"{grade}등";
                break;
        }
    }

    // 뽑기 블럭을 뽑기전 상태로 되돌리기 위한 함수입니다.
    private void AllCloseView()
    {
        if (_1st == null || _2nd == null || _3rd == null || _lowRank == null) return;

        _1st.SetActive(false);
        _2nd.SetActive(false);
        _3rd.SetActive(false);
        _lowRank.SetActive(false);
        _lowRankTMP.text = string.Empty;
    }

    private int GetGrade(int index)
    {
        int id = GatchaDataManager.Instance.GetItemID(index);

        // 구글시트 매니저에서 데이터 테이블에 해당하는 SO 가져오기
        var db = googleSheetManager.instance.GetClassData<DrawBoardRewards>();

        var data = db.FindById(id.ToString());

        return data.Grade;
    }

    public async Task Do()
    {
        await _anim.PlayAnimation();
    }
}

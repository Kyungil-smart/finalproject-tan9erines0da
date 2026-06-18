using System.Threading.Tasks;
using UnityEngine;

public partial class GatchaDataManager : MonoBehaviour
{
    #region 디버깅 관련 함수
    [ContextMenu("OnClickCompleteQuestButton")]
    public async Task OnClickCompleteQuestButton()
    {
        IsQuestCompensation();
    }
    /*
    OnClickCompleteQuestButton 메소드
    디버깅용 버튼 기능으로 퀘스트 수행 버튼 메소드 입니다.
    OnClickCompleteQuestButton 메소드를 구독한 버튼을 누를 경우
    IsQuestCompensation 메소드를 호출하여 퀘스트 수행이 1회 완료됩니다.
    해당 기능은 Firestore에 변경된 데이터를 업로드 합니다.
*/

    [ContextMenu("OnClickGetDrawTicketButton")]
    public async Task OnClickGetDrawTicketButton()
    {
        if (GatchaData.OwnedTicketCount >= 99) return;
        for (int i = 0; i < 10; i++) GetTicket();
        await Set_GatchaDTO();
    }
    /*
    OnClickGetDrawTicketButton 메소드
    디버깅용 버튼 기능으로 뽑기권을 획득하는 버튼 메소드 입니다.
    OnClickGetDrawTicketButton 메소드를 구독한 버튼을 누를 경우
    GetTicket 메소드를 10번 호출하여 10개의 뽑기권을 획득합니다.
    뽑기권이 99개 이상이 되면 더이상 뽑기권을 획득하지 못합니다.
    해당 기능은 Firestore에 변경된 데이터를 업로드 합니다.
*/

    [ContextMenu("OnClickGachaResetButton")]
    public async Task OnClickGachaResetButton()
    {
        GatchaData = new();
        InitGatchaData();
        InitfirstDic();
        GatchaData.IsResetPerformed = false;
        GatchaData.Grade_1 = false;
        GatchaData.Grade_2 = false;
        GatchaData.Grade_3 = false;
        await Set_GatchaDTO();
    }
    /*
    OnClickGachaResetButton 메소드
    디버깅용 버튼 기능으로 뽑기 상황을 처음으로 리셋하는 버튼 메소드 입니다.
    OnClickGachaResetButton 메소드를 구독한 버튼을 누를 경우
    뽑기 상황이 모두 초기화 됩니다.
    해당 기능은 Firestore에 변경된 데이터를 업로드 합니다.
*/

    [ContextMenu("OnClickDailyResetButton")]
    public async Task OnClickDailyResetButton()
    {
        GatchaData.TodayAttendanceTicketCount = 0;
        GatchaData.TodayQuestTicketCount = 0;
        await Set_GatchaDTO();
    }
    /*
    OnClickDailyResetButton 메소드
    디버깅용 버튼 기능으로 하루 단위 초기화 버튼 메소드 입니다.
    OnClickDailyResetButton 메소드를 구독한 버튼을 누를 경우
    출석 뽑기권 획득 횟수 및 퀘스트 뽑기권 획득 횟수를 0 으로 만듭니다.
    해당 기능은 Firestore에 변경된 데이터를 업로드 합니다.
*/
    #endregion
}

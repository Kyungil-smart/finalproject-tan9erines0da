using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Debugging_Buttons : MonoBehaviour
{
    [Header("디버깅 버튼 참조")]
    [SerializeField] private Button _completeQuest_Button;
    [SerializeField] private Button _getDrawTicket_Button;
    [SerializeField] private Button _gachaReset_Button;
    [SerializeField] private Button _dailyReset_Button;
    [Header("GCP 참조")]
    [SerializeField] private GatchaContentPresenter _gatchaContentPresenter; 

    private void OnEnable()
    {
        _completeQuest_Button.onClick.AddListener(OnCompleteQuestButton);
        _getDrawTicket_Button.onClick.AddListener(OnGetDrawTicketButton);
        _gachaReset_Button.onClick.AddListener(OnGachaResetButton);
        _dailyReset_Button.onClick.AddListener(OnDailyResetButton);
    }

    private void OnDisable()
    {
        _completeQuest_Button.onClick.RemoveListener(OnCompleteQuestButton);
        _getDrawTicket_Button.onClick.RemoveListener(OnGetDrawTicketButton);
        _gachaReset_Button.onClick.RemoveListener(OnGachaResetButton);
        _dailyReset_Button.onClick.RemoveListener(OnDailyResetButton);
    }

    private async void OnCompleteQuestButton()
    {
        if (_gatchaContentPresenter.IsMainCanvasOpen == true) return;

            await GatchaDataManager.Instance.OnClickCompleteQuestButton();

        _gatchaContentPresenter.RefreshQuestTicketTXT();
    }

    private async void OnGetDrawTicketButton()
    {
        if (_gatchaContentPresenter.IsMainCanvasOpen == true) return;

        await GatchaDataManager.Instance.OnClickGetDrawTicketButton();

        _gatchaContentPresenter.RefreshOwnedTicketsTXT();
    }

    private async void OnGachaResetButton()
    {
        if (_gatchaContentPresenter.IsMainCanvasOpen == true) return;

        await GatchaDataManager.Instance.OnClickGachaResetButton();
    }

    private async void OnDailyResetButton()
    {
        if (_gatchaContentPresenter.IsMainCanvasOpen == true) return;

        await GatchaDataManager.Instance.OnClickDailyResetButton();
        await LocalFeedStorage.GetRandomSixAsync();

        _gatchaContentPresenter.RefreshQuestTicketTXT();
        _gatchaContentPresenter.RefreshDailyTicketTXT();
    }
}

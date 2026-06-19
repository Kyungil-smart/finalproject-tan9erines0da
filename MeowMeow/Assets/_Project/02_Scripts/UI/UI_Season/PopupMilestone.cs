using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupMilestone : MonoBehaviour, IPopupable
{
    [Header("참조 필요")]
    [SerializeField] Button _exitButton;
    [SerializeField] private Button _checkButton;
    [SerializeField] Animator _animator;

    GatchaContentPresenter _contentPresenter;

    public void Open()
    {
        _exitButton.interactable = false;
        StartCoroutine(Wait());
        SubscribeManager.instance.Subscribe(SubscribeType.On_BoxAnimFinish, OnFinish);
    }

    public void Bind(GatchaContentPresenter gcp)
    {
        _contentPresenter = gcp;
        _exitButton.onClick.AddListener(OnExitButtonClick);
        _checkButton.onClick.AddListener(OnCheckButtonClick);
    }
    public void Unbind()
    {
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
        _checkButton.onClick.RemoveListener(OnCheckButtonClick);
    }

    void OnExitButtonClick()
    {
        if (_contentPresenter == null) return;
        // 중복 클릭 방지
        _exitButton.interactable = false;

        _animator.Play("IdleBox", 0, 0f);
        // 만약 애니메이션만으로는 비활성화 처리가 안된다면 직접제어 필요

        _contentPresenter.ClosePopup(this);
    }

    void OnCheckButtonClick()
    {
        // 애니메이션 시작
        _checkButton.interactable = false;
        _checkButton.gameObject.SetActive(false);
        _animator.SetTrigger("Open");
    }

    public void Close()
    {
        _contentPresenter = null;
        SubscribeManager.instance.Unsubscribe(SubscribeType.On_BoxAnimFinish, OnFinish);
        SubscribeManager.instance.Publish(SubscribeType.Close_MilestonePopup);
    }

    public void SetData(int itemId)
    {
       // 필요 데이터 없습니다.
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.25f);

        _checkButton.gameObject.SetActive(true);
        _checkButton.interactable = true;
    }

    private void OnFinish()
    {
        _exitButton.interactable = true;
    }
}

using UnityEngine;

public class StickerDelOff : MonoBehaviour
{
    [Header("자신의 삭제 버튼을 참조")]
    [SerializeField] private GameObject _delButton;

    private void OnEnable()
    {
        StickerStateSingleton.Instance.StickerDelButtonOn += DelButtonOn;
        StickerStateSingleton.Instance.StickerDelButtonOff += DelButtonOffAction;
    }

    private void OnDisable()
    {
        StickerStateSingleton.Instance.StickerDelButtonOn -= DelButtonOn;
        StickerStateSingleton.Instance.StickerDelButtonOff -= DelButtonOffAction;
    }

    // 자신의 삭제버튼을 키는 함수(나머지는 다 꺼짐)
    private void DelButtonOn(GameObject target) => _delButton.SetActive(target == gameObject);

    // 삭제 버튼 누를때 타겟 푸는 함수랑 우선순위 문제로 딜레이 넣는 함수 
    private void DelButtonOffAction()
    {
        CancelInvoke(nameof(DelButtonOff));
        Invoke(nameof(DelButtonOff), 0.001f);
    }

    // 자신의 삭제버튼은 끄는 함수
    private void DelButtonOff()
    {
        _delButton.SetActive(false);
    }

}

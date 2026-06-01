using UnityEngine;

public class StickerDelOff : MonoBehaviour
{
    private GameObject _delButton;

    private void OnEnable()
    {
        StickerStateSingleton.Instance.StickerDelButtonOn += DelButtonOn;
        StickerStateSingleton.Instance.StickerDelButtonOff += DelButtonOff;
    }

    private void OnDisable()
    {
        StickerStateSingleton.Instance.StickerDelButtonOn -= DelButtonOn;
        StickerStateSingleton.Instance.StickerDelButtonOff -= DelButtonOff;
    }

    /// <summary>
    /// 외부에서 StickerDelOff 스크립트를 초기화 하기위해 만든 함수입니다.
    /// </summary>
    /// <param name="delButton">생성된 스티커 삭제버튼을 넣어줄 인자입니다.</param>
    public void InitStickerDelOff(GameObject delButton)
    {
        _delButton = delButton;
    }

    // 자신의 삭제버튼을 키는 함수
    private void DelButtonOn(GameObject target) => _delButton.SetActive(target == _delButton);

    // 자신의 삭제버튼은 끄는 함수
    private void DelButtonOff()
    {
        _delButton.SetActive(false);
    }
}

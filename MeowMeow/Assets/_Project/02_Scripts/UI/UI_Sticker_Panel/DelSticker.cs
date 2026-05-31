using UnityEngine;
using UnityEngine.UI;

public class DelSticker : MonoBehaviour
{
    [Header("스티커 프리펩을 참조")]
    [SerializeField] private GameObject _stickerObject;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickDelSticker);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickDelSticker);
    }

    // 스티커 삭제 버튼에 구독시킬 함수(스티커 삭제)
    private void OnClickDelSticker()
    {
        if (StickerStateSingleton.Instance == null) return;

        // 해당 스티커에 연결된 토글 가져오기
        Toggle toggle = StickerStateSingleton.Instance.StickerToToggle[_stickerObject];

        // 각 자료구조에서 해당 스티커 및 토글버튼 삭제
        StickerStateSingleton.Instance.ToggleList.Remove(toggle);
        StickerStateSingleton.Instance.StickerToToggle.Remove(_stickerObject);
        StickerStateSingleton.Instance.ToggleToSticker.Remove(toggle);
        StickerStateSingleton.Instance.StickerIndexes.Remove(_stickerObject);

        // 토글 버튼 삭제
        Destroy(toggle.gameObject);
        // 스티커 오브젝트 삭제
        Destroy(_stickerObject);

        // 스티커 생선순 토글버튼 번호 갱신
        StickerStateSingleton.Instance.RefreshPriorityButtons();

        // 스티커 제한개수 감소 및 갱신
        StickerStateSingleton.Instance.CurrentCount--;
        StickerStateSingleton.Instance.StickerCountUpload();
    }
}

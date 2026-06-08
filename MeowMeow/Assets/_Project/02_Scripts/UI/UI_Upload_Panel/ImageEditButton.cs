using UnityEngine;
using UnityEngine.UI;

public class ImageEditButton : MonoBehaviour
{
    private Button _button;
    [Header("냥냥스톤 부족 팝업 참조")]
    [SerializeField]private GameObject _nyangStoneEmptyImage;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickEditButton);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClickEditButton);
    }

    private void OnClickEditButton()
    {
        if (LocalDataManager.Instance == null) return;

        if (LocalDataManager.Instance.NyangNyangStone <= 0)
        {
            if (_nyangStoneEmptyImage.activeSelf) return;

            _nyangStoneEmptyImage.SetActive(true);
            Invoke(nameof(CloseNyangStonePopup), 0.5f);
            return;
        }
    }

    private void CloseNyangStonePopup()
    {
        _nyangStoneEmptyImage.SetActive(false);
    }
}

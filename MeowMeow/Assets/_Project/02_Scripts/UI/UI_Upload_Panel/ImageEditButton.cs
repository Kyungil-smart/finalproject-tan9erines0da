using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageEditButton : MonoBehaviour
{
    private Button _button;
    [Header("냥냥스톤 부족 팝업 참조")]
    [SerializeField]private GameObject _nyangStoneEmptyImage;
    [Header("화면전환 참조")]
    [SerializeField]private BaseScreenController _baseScreenController;
    [SerializeField]private UIPanel _panel;
    [Header("BottomPanel -> Upload_Folder_Scroll View -> Content -> 를 참조")]
    [SerializeField] private GetImageList _getImageList;

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

        if (_getImageList.IsSelectImage == false) return;

        if (Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            if (LocalDataManager.Instance.NyangNyangStone <= 0)
            {
                if (_nyangStoneEmptyImage.activeSelf) return;

                // 효과음
                SoundManager.Instance.Invoke(AudioType.SFX_UI_Error);

                _nyangStoneEmptyImage.SetActive(true);
                Invoke(nameof(CloseNyangStonePopup), 0.5f);
                return;
            }
            _baseScreenController.RequestScreenChange(_panel);
        }

        else
        {
            _baseScreenController.RequestScreenChange(_panel);
        }
    }

    private void CloseNyangStonePopup()
    {
        _nyangStoneEmptyImage.SetActive(false);
    }
}

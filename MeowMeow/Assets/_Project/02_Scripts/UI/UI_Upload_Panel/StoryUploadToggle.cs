using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryUploadToggle : MonoBehaviour
{
    [Header("각각 폴더 스크롤 뷰를 참조")]
    [SerializeField] private GameObject _storyFolderScrollView;
    [SerializeField] private GameObject _uploadFolderScrollView;
    [Header("스토리/업로드 표시할 텍스트 참조")]
    [SerializeField] private TextMeshProUGUI _storyUploadText;
    [Header("프리뷰 이미지를 참조")]
    [SerializeField] private Image _image;
    [Header("Story_Upload_Toggle의 자식 UnCheckmark 오브젝트를 참조")]
    [SerializeField] private GameObject _unCheckmark;
    [Header("Story_Upload_Toggle의 자식 Background 오브젝트를 참조")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _storyColor;
    [SerializeField] private Color _uploadColor;
    [Header("BottomPanel -> Upload_Folder_Scroll View -> Content -> 를 참조")]
    [SerializeField] private GetImageList _getImageList;

    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        _toggle.onValueChanged.AddListener(OnStoryUploadToggle);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnStoryUploadToggle);
    }

    private void OnStoryUploadToggle(bool isOn)
    {
        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_UI_Button_Click_Settings_1);

        if (!isOn)
        {
            _backgroundImage.color = _storyColor;

            _storyFolderScrollView.SetActive(true);
            _uploadFolderScrollView.SetActive(false);
            _unCheckmark.SetActive(true);
            _storyUploadText.text = "스토리";
            _image.sprite = null;
            _getImageList.IsSelectImage = false;
        }
        else
        {
            _backgroundImage.color = _uploadColor;

            _storyFolderScrollView.SetActive(false);
            _uploadFolderScrollView.SetActive(true);
            _unCheckmark.SetActive(false);
            _storyUploadText.text = "업로드";
            _image.sprite = null;
        }
    }
}

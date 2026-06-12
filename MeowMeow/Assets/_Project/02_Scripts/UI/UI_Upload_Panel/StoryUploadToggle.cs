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


    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        _toggle.onValueChanged.AddListener(OnStoryUploadToggle);
        _storyUploadText.text = "스토리";
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnStoryUploadToggle);
    }

    private void OnStoryUploadToggle(bool isOn)
    {
        if (!isOn)
        {
            _storyFolderScrollView.SetActive(true);
            _uploadFolderScrollView.SetActive(false);
            _unCheckmark.SetActive(true);
            _storyUploadText.text = "스토리";
            _image.sprite = null;
        }
        else
        {
            _storyFolderScrollView.SetActive(false);
            _uploadFolderScrollView.SetActive(true);
            _unCheckmark.SetActive(false);
            _storyUploadText.text = "업로드";
            _image.sprite = null;
        }
    }
}

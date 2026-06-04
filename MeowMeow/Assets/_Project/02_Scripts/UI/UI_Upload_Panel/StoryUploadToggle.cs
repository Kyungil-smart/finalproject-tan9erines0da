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
        if (isOn)
        {
            _storyFolderScrollView.SetActive(true);
            _uploadFolderScrollView.SetActive(false);
            _storyUploadText.text = "Story";
            _image.sprite = null;
        }
        else
        {
            _storyFolderScrollView.SetActive(false);
            _uploadFolderScrollView.SetActive(true);
            _storyUploadText.text = "Upload";
            _image.sprite = null;
        }
    }
}

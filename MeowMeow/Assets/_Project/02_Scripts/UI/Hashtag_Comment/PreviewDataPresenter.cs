using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PreviewDataPresenter : MonoBehaviour, ISNSPanelPresenter, ISNSContextReceiver
{
    [Header("시각적 이미지/스티커 최종 복원 렌더러")]
    [SerializeField] private SNSPostImageRenderer _postImageRenderer;

    [Header("작업자 텍스트 UI 참조")]
    [SerializeField] private TextMeshProUGUI _hashtagText;
    [SerializeField] private TextMeshProUGUI _commentText;

    [Header("UI 컨트롤러 및 타겟 패널 직접 참조")]
    [SerializeField] private BaseScreenController _uiController;
    [SerializeField] private UIPanel _profilePanel;
    [SerializeField] private Button _uploadButton;

    private SNSPostDTO _snapshot;

    /// <summary>
    /// 전달받은 DTO 구조체를 바탕으로 복원합니다
    /// </summary>
    public void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        _snapshot = snapshot;

        // 이미지 복원
        if (_postImageRenderer != null)
        {
            _postImageRenderer.RenderPreview(_snapshot);
        }

        // 텍스트 복원
        if (_hashtagText != null && _snapshot.Hashtags != null)
        {
            _hashtagText.text = string.Join("  ", _snapshot.Hashtags);
        }

        if (_commentText != null)
        {
            _commentText.text = _snapshot.Comment;
        }
        _uploadButton?.onClick.RemoveAllListeners();
        _uploadButton.onClick.AddListener(ExecuteUploadAndReturn);
    }

    public void RequestContext()
    {
        if (SubscribeManager.instance == null)
        {
            Debug.LogWarning("[Preview] 아직 SubscribeManager가 준비되지 않았습니다.");
            return;
        }

        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
            SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }

    /// <summary>
    /// 현재 화면은 최종 확인 화면이므로 데이터를 더 수정할 여지가 없습니다.
    /// 네트워크 최종 업로드는 추후 분리된 별도 전역 함수나 관제탑에서 
    /// 안전하게 실행할 수 있도록 기본 파이프라인 싱크만 열어둡니다.
    /// </summary>
    public void SubmitContext()
    {
        _uploadButton?.onClick.RemoveAllListeners();

        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);
    }

    /// <summary>
    /// 업로드를 진행하고, 다음 패널을 호출합니다
    /// </summary>
    public async void ExecuteUploadAndReturn()
    {
        // 1. 중복 클릭 방지
        if (_uploadButton != null) _uploadButton.interactable = false;

        try
        {
            // 2. 매니저에 업로드 지시 후 완료될 때까지 대기
            await SNSPostManager.Instance
                .UploadAndCachePostAsync(_snapshot);

            Debug.Log("[Preview] 업로드 성공! 프로필로 전환합니다.");

            _snapshot = SNSPostDTO.CreateEmpty();
            
            SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);

            // 3. 컨트롤러에 타겟 패널을 쥐여주며 전환 요청합니다
            if (_uiController != null && _profilePanel != null)
            {
                _uiController.RequestScreenChange(_profilePanel);
            }
            else
            {
                Debug.LogWarning("컨트롤러나 타겟 참조가 누락되었습니다.");
            }
        }
        catch (Exception ex)
        {
            // 4. 업로드 실패 시 패널을 닫지 않고 대기
            Debug.LogError($"업로드 실패. 전환 중단: {ex.Message}");
        }
        finally
        {
            // 5. 버튼 상태 복구
            if (_uploadButton != null) _uploadButton.interactable = true;
        }
    }
}

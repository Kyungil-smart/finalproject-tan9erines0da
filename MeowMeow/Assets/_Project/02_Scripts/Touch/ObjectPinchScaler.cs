using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class ObjectPinchScaler : MonoBehaviour
{
    [Header("스티커 이동 관련")]
    [SerializeField] private float _moveSpeed = 1f;

    [Header("스티커 확대 관련")]
    [SerializeField] private float _scaleSpeed = 0.005f;
    [SerializeField] private float _minScale = 0.3f;
    [SerializeField] private float _maxScale = 1.7f;

    [Header("스티커 회전 관련")]
    [SerializeField] private float _rotationSpeed = 1.5f;

    [Header("현재 타겟 확인용/참조X")]
    [SerializeField] private RectTransform _target;

    private RectTransform _parent;

    // 이전 터치 방향 저장
    private Vector2 _previousDirection;

    private void OnEnable()
    {
        TouchInputHandler.Instance.OnObjectSelected += OnSelect;
        TouchInputHandler.Instance.OnObjectSelectedForToggle += OnSelectForToggle;
        TouchInputHandler.Instance.OnSelectionCleared += OnUnselect;
        TouchInputHandler.Instance.OnDragDelta += OnDrag;
        TouchInputHandler.Instance.OnPinchDelta += OnPinch;
    }

    private void OnDisable()
    {
        TouchInputHandler.Instance.OnObjectSelected -= OnSelect;
        TouchInputHandler.Instance.OnObjectSelectedForToggle -= OnSelectForToggle;
        TouchInputHandler.Instance.OnSelectionCleared -= OnUnselect;
        TouchInputHandler.Instance.OnDragDelta -= OnDrag;
        TouchInputHandler.Instance.OnPinchDelta -= OnPinch;
    }

    private void Update()
    {
        RotateTarget();
    }

    // 스티커 선택 함수
    private void SelectSticker(TouchInteractor obj)
    {
        _target = obj.GetComponent<RectTransform>();
        _parent = _target.parent as RectTransform;

        GameObject sticker = obj.gameObject;
        GameObject delButton = StickerStateSingleton.Instance.StickerToDelButton[sticker];

        // 삭제 버튼 활성화
        StickerStateSingleton.Instance.StickerDelButtonSetOn(delButton);
    }

    // 터치로 스티커 선택
    public void OnSelect(TouchInteractor obj)
    {
        GameObject sticker = obj.gameObject;
        StickerPriorityButton stickerPriorityButton = StickerStateSingleton.Instance.StickerToToggle[sticker].GetComponent<StickerPriorityButton>();

        // 스티커 터치로 선택시 해당 토글버튼 눌림처리
        if (!stickerPriorityButton.Toggle.isOn)
        {
            stickerPriorityButton.Toggle.isOn = true;
        }
    }

    // 토글버튼으로 스티커 선택시 SelectSticker() 호출
    public void OnSelectForToggle(TouchInteractor obj)
    {
        SelectSticker(obj);
    }

    // 스티커 선택해제 함수
    public void OnUnselect()
    {
        if (StickerStateSingleton.Instance == null || StickerStateSingleton.Instance.ToggleList == null)
        {
            return;
        }

        // 토글 버든 모두 해제
        foreach (Toggle toggle in StickerStateSingleton.Instance.ToggleList)
        {
            toggle.isOn = false;
        }

        _target = null;
    }

    #region 스티커 이동 함수
    private void OnDrag(Vector2 delta)
    {
        if (!TouchInputHandler.Instance._isTouchingSticker) return;

        if (_target == null || _parent == null) return;

        // 이동할 위치 계산
        Vector2 nextPos = _target.anchoredPosition + delta * _moveSpeed;

        // 부모(사진)와 스티커 크기 저장
        Vector2 parentSize = _parent.rect.size;
        Vector2 targetSize = _target.rect.size;

        // 스티커 중심점이 이동할 수 있는 최소 좌표
        Vector2 min = new Vector2(
            -parentSize.x * 0.5f + targetSize.x * 0.5f,
            -parentSize.y * 0.5f + targetSize.y * 0.5f
        );
        // 스티커 중심점이 이동할 수 있는 최대 좌표
        Vector2 max = new Vector2(
            parentSize.x * 0.5f - targetSize.x * 0.5f,
            parentSize.y * 0.5f - targetSize.y * 0.5f
        );

        // 스티거 이동 범위 제한
        nextPos.x = Mathf.Clamp(nextPos.x, min.x, max.x);
        nextPos.y = Mathf.Clamp(nextPos.y, min.y, max.y);

        // 스티커 위치 적용
        _target.anchoredPosition = nextPos;
    }
    #endregion

    #region 스티커 확대 함수
    private void OnPinch(float pinchDelta)
    {
        if (!TouchInputHandler.Instance._isTouchingSticker) return;

        if (_target == null) return;

        // 현재 크기 저장
        Vector3 scale = _target.localScale;

        // 확대/축소량 계산
        float amount = pinchDelta * _scaleSpeed;
        // 확대/축소가 적용된 새 크기 계산
        Vector3 newScale = scale + new Vector3(amount, amount, 0f);

        // 최소/최대 확대 크기 제한
        float clampedX = Mathf.Clamp(newScale.x, _minScale, _maxScale);
        // X,Y 비율 유지
        newScale = new Vector3(clampedX, clampedX, 1f);

        // 스티커 크기 적용
        _target.localScale = newScale;
    }
    #endregion

    #region 스티커 회전 함수
    // TouchInputHandler에 제공된 이벤트 함수가 없어서 독립적으로 작성
    private void RotateTarget()
    {
        if (!TouchInputHandler.Instance._isTouchingSticker) return;

        if (_target == null) return;

        if (Touchscreen.current == null) return;
        // 두 손가락 터치 확인
        if (Touchscreen.current.touches.Count < 2)
        {
            // 회전 기준 초기화
            _previousDirection = Vector2.zero;
            return;
        }

        TouchControl first = Touchscreen.current.touches[0];
        TouchControl second = Touchscreen.current.touches[1];

        // 두 손가락 중 하나라도 손을 뗐을 때 회전 상태 종료
        if (!first.press.isPressed || !second.press.isPressed)
        {
            _previousDirection = Vector2.zero;
            return;
        }

        Vector2 p1 = first.position.ReadValue();
        Vector2 p2 = second.position.ReadValue();

        // 두 손가락 사이의 방향 벡터를 계산
        Vector2 currentDir = p2 - p1;

        // 회전 기준값을 처음 잡는 초기화 구간
        if (_previousDirection == Vector2.zero)
        {
            _previousDirection = currentDir;
            return;
        }

        // 이전 방향과 현재 방향의 각도 차이를 구함
        float angle = Vector2.SignedAngle(_previousDirection, currentDir);
        // 프레임당 최대 회전량 제한(이전에 회전중에 회전이 튀는 현상이 있었습니다.)
        angle = Mathf.Clamp(angle, -10f, 10f);

        // 미세 각도 무시
        if (Mathf.Abs(angle) < 0.1f) return;

        // 스티커를 Z축 기준으로 누적 회전
        _target.Rotate(0f, 0f, angle * _rotationSpeed);

        // 다음 프레임 회전 계산을 위한 기준값 갱신
        _previousDirection = currentDir;
    }
    #endregion
}

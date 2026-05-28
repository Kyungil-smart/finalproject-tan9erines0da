using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
        TouchInputHandler.Instance.OnSelectionCleared += OnUnselect;
        TouchInputHandler.Instance.OnDragDelta += OnDrag;
        TouchInputHandler.Instance.OnPinchDelta += OnPinch;
    }

    private void OnDisable()
    {
        TouchInputHandler.Instance.OnObjectSelected -= OnSelect;
        TouchInputHandler.Instance.OnSelectionCleared -= OnUnselect;
        TouchInputHandler.Instance.OnDragDelta -= OnDrag;
        TouchInputHandler.Instance.OnPinchDelta -= OnPinch;
    }

    private void Update()
    {
        RotateTarget();
    }

    private void OnSelect(TouchInteractor obj)
    {
        _target = obj.GetComponent<RectTransform>();
        _parent = _target.parent as RectTransform;
    }

    private void OnUnselect()
    {
        _target = null;
    }

    #region 스티커 이동 함수
    private void OnDrag(Vector2 delta)
    {
        if (_target == null || _parent == null) return;

        Vector2 nextPos = _target.anchoredPosition + delta * _moveSpeed;

        Vector2 parentSize = _parent.rect.size;
        Vector2 targetSize = _target.rect.size;

        Vector2 min = new Vector2(
            -parentSize.x * 0.5f + targetSize.x * 0.5f,
            -parentSize.y * 0.5f + targetSize.y * 0.5f
        );

        Vector2 max = new Vector2(
            parentSize.x * 0.5f - targetSize.x * 0.5f,
            parentSize.y * 0.5f - targetSize.y * 0.5f
        );

        nextPos.x = Mathf.Clamp(nextPos.x, min.x, max.x);
        nextPos.y = Mathf.Clamp(nextPos.y, min.y, max.y);

        _target.anchoredPosition = nextPos;
    }
    #endregion

    #region 스티커 확대 함수
    private void OnPinch(float pinchDelta)
    {
        if (_target == null) return;

        Vector3 scale = _target.localScale;
        float amount = pinchDelta * _scaleSpeed;

        Vector3 newScale = scale + new Vector3(amount, amount, 0f);

        float clampedX = Mathf.Clamp(newScale.x, _minScale, _maxScale);
        newScale = new Vector3(clampedX, clampedX, 1f);

        _target.localScale = newScale;
    }
    #endregion

    #region 스티커 회전 함수
    private void RotateTarget()
    {
        if (_target == null) return;

        if (Touchscreen.current == null) return;
        if (Touchscreen.current.touches.Count < 2)
        {
            _previousDirection = Vector2.zero;
            return;
        }

        TouchControl first = Touchscreen.current.touches[0];
        TouchControl second = Touchscreen.current.touches[1];

        if (!first.press.isPressed || !second.press.isPressed)
        {
            _previousDirection = Vector2.zero;
            return;
        }

        Vector2 p1 = first.position.ReadValue();
        Vector2 p2 = second.position.ReadValue();

        Vector2 currentDir = p2 - p1;

        if (_previousDirection == Vector2.zero)
        {
            _previousDirection = currentDir;
            return;
        }

        float angle = Vector2.SignedAngle(_previousDirection, currentDir);
        angle = Mathf.Clamp(angle, -10f, 10f);

        if (Mathf.Abs(angle) < 0.1f) return;

        _target.Rotate(0f, 0f, angle * _rotationSpeed);

        _previousDirection = currentDir;
    }
    #endregion
}

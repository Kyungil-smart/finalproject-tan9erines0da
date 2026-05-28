using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ObjectPinchScaler : MonoBehaviour
{
    [Header("스티커 이동 관련")]
    [SerializeField] private float _moveSpeed = 0.005f;

    [Header("스티커 확대 관련")]
    [SerializeField] private float _scaleSpeed = 0.005f;
    [SerializeField] private float _minScale = 0.3f;
    [SerializeField] private float _maxScale = 1.7f;

    [Header("스티커 회전 관련")]
    [SerializeField] private float _rotationSpeed = 1.5f;

    [Header("현재 타겟 확인용/참조X")]
    [SerializeField] private GameObject _target;

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
        _target = obj.gameObject;
    }

    private void OnUnselect()
    {
        _target = null;
    }

    #region 스티커 이동 함수
    private void OnDrag(Vector2 delta)
    {
        if (_target == null) return;

        Vector3 move = new Vector3(delta.x, delta.y, 0f) * _moveSpeed;

        _target.transform.position += move;
    }
    #endregion

    #region 스티커 확대 함수
    private void OnPinch(float pinchDelta)
    {
        if (_target == null) return;

        Vector3 currentScale = _target.transform.localScale;

        float scaleAmount = pinchDelta * _scaleSpeed;

        Vector3 newScale = currentScale + new Vector3(scaleAmount, scaleAmount, 0f);

        newScale.x = Mathf.Clamp(newScale.x, _minScale, _maxScale);
        newScale.y = Mathf.Clamp(newScale.y, _minScale, _maxScale);

        _target.transform.localScale = newScale;
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

        TouchControl firstTouch = Touchscreen.current.touches[0];
        TouchControl secondTouch = Touchscreen.current.touches[1];

        if (!firstTouch.press.isPressed || !secondTouch.press.isPressed)
        {
            _previousDirection = Vector2.zero;
            return;
        }

        Vector2 firstPos = firstTouch.position.ReadValue();
        Vector2 secondPos = secondTouch.position.ReadValue();

        Vector2 currentDirection = secondPos - firstPos;

        if (_previousDirection == Vector2.zero)
        {
            _previousDirection = currentDirection;
            return;
        }

        float angle = Vector2.SignedAngle(_previousDirection, currentDirection);

        // 회전하다 튀는 현상이 있어서 넣은 코드
        angle = Mathf.Clamp(angle, -10f, 10f);

        // 미세 회전 무시하는 코드
        if (Mathf.Abs(angle) < 0.1f) return;

        _target.transform.Rotate(0f, 0f, angle * _rotationSpeed);

        _previousDirection = currentDirection;
    }
    #endregion
}

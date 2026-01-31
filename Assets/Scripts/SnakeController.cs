using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SnakeController : MonoBehaviour
{
    private Vector2 direction = Vector2.right;
    private Vector2 nextDirection = Vector2.right;

    [Header("Swipe (Mobile)")]
    [Tooltip("스와이프로 인정할 최소 이동 거리(픽셀)")]
    [SerializeField] private float minSwipeDistance = 40f;
    [Tooltip("에디터에서 마우스 드래그를 터치로 시뮬레이션")]
    [SerializeField] private bool simulateTouchWithMouse = true;

    private Vector2? touchStartPosition;
    private Vector2? mouseStartPosition;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    public void Initialize(Vector2 startDirection)
    {
        direction = startDirection;
        nextDirection = startDirection;
        ApplyRotation();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // PC: 방향키
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.upArrowKey.wasPressedThisFrame && direction != Vector2.down)
                nextDirection = Vector2.up;
            else if (keyboard.downArrowKey.wasPressedThisFrame && direction != Vector2.up)
                nextDirection = Vector2.down;
            else if (keyboard.leftArrowKey.wasPressedThisFrame && direction != Vector2.right)
                nextDirection = Vector2.left;
            else if (keyboard.rightArrowKey.wasPressedThisFrame && direction != Vector2.left)
                nextDirection = Vector2.right;
        }

        // 모바일: 스와이프
        HandleSwipeInput();
    }

    private void HandleSwipeInput()
    {
        // 1) 실제 터치 (모바일/WebGL)
        if (Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    touchStartPosition = touch.screenPosition;
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (touchStartPosition.HasValue)
                    {
                        Vector2 delta = touch.screenPosition - touchStartPosition.Value;
                        ApplySwipeDirection(delta);
                        touchStartPosition = null;
                    }
                    break;
            }
            return;
        }
        touchStartPosition = null;

        // 2) 에디터: 마우스 드래그를 터치로 시뮬레이션
        if (simulateTouchWithMouse)
        {
            var mouse = Mouse.current;
            if (mouse == null) return;
            if (mouse.leftButton.wasPressedThisFrame)
                mouseStartPosition = mouse.position.ReadValue();
            else if (mouse.leftButton.wasReleasedThisFrame && mouseStartPosition.HasValue)
            {
                Vector2 delta = mouse.position.ReadValue() - mouseStartPosition.Value;
                ApplySwipeDirection(delta);
                mouseStartPosition = null;
            }
        }
    }

    private void ApplySwipeDirection(Vector2 delta)
    {
        if (delta.sqrMagnitude < minSwipeDistance * minSwipeDistance)
            return;

        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);

        if (absX > absY)
        {
            if (delta.x > 0 && direction != Vector2.left)
                nextDirection = Vector2.right;
            else if (delta.x < 0 && direction != Vector2.right)
                nextDirection = Vector2.left;
        }
        else
        {
            if (delta.y > 0 && direction != Vector2.down)
                nextDirection = Vector2.up;
            else if (delta.y < 0 && direction != Vector2.up)
                nextDirection = Vector2.down;
        }
    }

    public void Move()
    {
        direction = nextDirection;
        transform.position += (Vector3)direction;
        ApplyRotation();
    }

    /// <summary>
    /// 이동 방향에 맞춰 스프라이트 Z 회전 (헤드 스프라이트 기준으로 시계 방향 90° 보정)
    /// </summary>
    private void ApplyRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public Vector2 GetNextPosition()
    {
        return (Vector2)transform.position + nextDirection;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }
}

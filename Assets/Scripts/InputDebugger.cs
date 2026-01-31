using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// 방향키·터치(스와이프) 입력을 콘솔에 로그. 디버깅 후 비활성화하거나 오브젝트에서 제거.
/// </summary>
public class InputDebugger : MonoBehaviour
{
    [Header("디버그 로그")]
    [SerializeField] private bool logKeyboard = true;
    [SerializeField] private bool logTouch = true;
    [Tooltip("스와이프로 인정할 최소 거리(픽셀). 이거 이상일 때만 스와이프 로그")]
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

    private void Update()
    {
        if (logKeyboard)
            LogKeyboard();
        if (logTouch)
            LogTouch();
    }

    private void LogKeyboard()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame)
            Debug.Log("[Input] 방향키: 위");
        if (keyboard.downArrowKey.wasPressedThisFrame)
            Debug.Log("[Input] 방향키: 아래");
        if (keyboard.leftArrowKey.wasPressedThisFrame)
            Debug.Log("[Input] 방향키: 왼쪽");
        if (keyboard.rightArrowKey.wasPressedThisFrame)
            Debug.Log("[Input] 방향키: 오른쪽");
    }

    private void LogTouch()
    {
        // 1) 실제 터치 (모바일/WebGL)
        if (Touch.activeTouches.Count > 0)
        {
            LogTouchInternal(Touch.activeTouches[0], ref touchStartPosition, "터치");
            return;
        }
        touchStartPosition = null;

        // 2) 에디터: 마우스 드래그를 터치로 시뮬레이션
        if (simulateTouchWithMouse)
            LogMouseAsTouch();
    }

    private void LogTouchInternal(Touch touch, ref Vector2? startPos, string label)
    {
        switch (touch.phase)
        {
            case UnityEngine.InputSystem.TouchPhase.Began:
                startPos = touch.screenPosition;
                Debug.Log($"[Input] {label} 시작 pos={touch.screenPosition}");
                break;
            case UnityEngine.InputSystem.TouchPhase.Ended:
            case UnityEngine.InputSystem.TouchPhase.Canceled:
                if (startPos.HasValue)
                {
                    Vector2 delta = touch.screenPosition - startPos.Value;
                    float distance = delta.magnitude;
                    if (distance >= minSwipeDistance)
                    {
                        string dir = GetSwipeDirection(delta);
                        Debug.Log($"[Input] {label} 스와이프 끝 delta=({delta.x:F0}, {delta.y:F0}) 거리={distance:F0}px → {dir}");
                    }
                    else
                    {
                        Debug.Log($"[Input] {label} 끝 (스와이프 아님, 거리={distance:F0}px < {minSwipeDistance})");
                    }
                    startPos = null;
                }
                break;
        }
    }

    private void LogMouseAsTouch()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            mouseStartPosition = mouse.position.ReadValue();
            Debug.Log($"[Input] 마우스 드래그 시작 (에디터 시뮬레이션) pos={mouseStartPosition}");
        }
        else if (mouse.leftButton.wasReleasedThisFrame && mouseStartPosition.HasValue)
        {
            Vector2 current = mouse.position.ReadValue();
            Vector2 delta = current - mouseStartPosition.Value;
            float distance = delta.magnitude;
            if (distance >= minSwipeDistance)
            {
                string dir = GetSwipeDirection(delta);
                Debug.Log($"[Input] 마우스 스와이프 끝 delta=({delta.x:F0}, {delta.y:F0}) 거리={distance:F0}px → {dir}");
            }
            else
            {
                Debug.Log($"[Input] 마우스 끝 (스와이프 아님, 거리={distance:F0}px)");
            }
            mouseStartPosition = null;
        }
    }

    private static string GetSwipeDirection(Vector2 delta)
    {
        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);
        if (absX > absY)
            return delta.x > 0 ? "오른쪽" : "왼쪽";
        return delta.y > 0 ? "위" : "아래";
    }
}

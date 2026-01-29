using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    private Vector2 direction = Vector2.right;
    private Vector2 nextDirection = Vector2.right;

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
        // 새 Input System 사용
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame && direction != Vector2.down)
        {
            nextDirection = Vector2.up;
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame && direction != Vector2.up)
        {
            nextDirection = Vector2.down;
        }
        else if (keyboard.leftArrowKey.wasPressedThisFrame && direction != Vector2.right)
        {
            nextDirection = Vector2.left;
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame && direction != Vector2.left)
        {
            nextDirection = Vector2.right;
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

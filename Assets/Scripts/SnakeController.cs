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

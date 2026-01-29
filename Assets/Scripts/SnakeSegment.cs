using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    public void MoveTo(Vector2 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// 이동 방향에 맞춰 스프라이트 Z 회전 (오른쪽=0°, 위=90°)
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}

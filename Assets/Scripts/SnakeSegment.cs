using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    public void MoveTo(Vector2 position)
    {
        transform.position = position;
    }
}

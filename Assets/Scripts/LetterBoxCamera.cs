using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    // Target aspect ratio = width / height (768/960 = 0.8)
    [SerializeField] private float targetAspect = 768f / 960f;

    private Camera _cam;
    private int _lastW = -1;
    private int _lastH = -1;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        // Make sure the unused area is a solid color (black by default)
        _cam.clearFlags = CameraClearFlags.SolidColor;
        // _cam.backgroundColor = Color.black; // optional
        Apply();
    }

    private void Update()
    {
        // Re-apply only when resolution changes (mobile rotation / browser UI changes)
        if (Screen.width != _lastW || Screen.height != _lastH)
        {
            Apply();
        }
    }

    private void Apply()
    {
        _lastW = Screen.width;
        _lastH = Screen.height;

        float windowAspect = (float)Screen.width / Screen.height;

        if (windowAspect > targetAspect)
        {
            // Wider than target: pillarbox left/right
            float scale = targetAspect / windowAspect;
            float xOffset = (1f - scale) * 0.5f;
            _cam.rect = new Rect(xOffset, 0f, scale, 1f);
        }
        else
        {
            // Taller than target: letterbox top/bottom
            float scale = windowAspect / targetAspect;
            float yOffset = (1f - scale) * 0.5f;
            _cam.rect = new Rect(0f, yOffset, 1f, scale);
        }
    }
}


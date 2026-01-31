using UnityEngine;

/// <summary>
/// 지정한 비율을 유지하며 화면에 레터박스/필러박스를 적용합니다.
/// Main Camera에 붙이고, UI Canvas는 "Screen Space - Camera" + 이 카메라로 두면 UI도 같은 영역 안에 들어갑니다.
/// </summary>
[RequireComponent(typeof(Camera))]
public class Letterboxer : MonoBehaviour
{
    [Header("Design Aspect")]
    [Tooltip("기준 비율 (가로/세로). 1=정사각형, 16/9=가로로 넓은 화면")]
    [SerializeField] private float designAspectWidth = 11f;
    [SerializeField] private float designAspectHeight = 11f;

    [Header("Bar Color")]
    [Tooltip("레터박스/필러박스 색 (화면 밖 영역)")]
    [SerializeField] private Color barColor = Color.black;

    private Camera _camera;
    private Camera _clearCamera; // 전체 화면을 barColor로 채우는 카메라

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        // 전체 화면을 먼저 barColor로 채우는 카메라 생성 (레터박스 바 표시)
        var clearGo = new GameObject("LetterboxClearCamera");
        _clearCamera = clearGo.AddComponent<Camera>();
        _clearCamera.clearFlags = CameraClearFlags.SolidColor;
        _clearCamera.backgroundColor = barColor;
        _clearCamera.cullingMask = 0;
        _clearCamera.depth = _camera.depth - 1;
        _clearCamera.orthographic = true;
        _clearCamera.orthographicSize = 1;
        _clearCamera.useOcclusionCulling = false;
        _clearCamera.allowHDR = false;
        _clearCamera.allowMSAA = false;
    }

    private void Start()
    {
        ApplyLetterbox();
    }

    private void OnPreCull()
    {
        ApplyLetterbox();
    }

    private void ApplyLetterbox()
    {
        if (_camera == null) return;

        float designAspect = designAspectWidth / designAspectHeight;
        float screenAspect = (float)Screen.width / Screen.height;

        float w, h, x, y;
        if (screenAspect > designAspect)
        {
            // 화면이 더 넓음 → 좌우 필러박스 (검은 막대 좌/우)
            w = designAspect / screenAspect;
            h = 1f;
            x = (1f - w) * 0.5f;
            y = 0f;
        }
        else
        {
            // 화면이 더 좁거나 높음 → 상하 레터박스 (검은 막대 위/아래)
            w = 1f;
            h = screenAspect / designAspect;
            x = 0f;
            y = (1f - h) * 0.5f;
        }

        _camera.rect = new Rect(x, y, w, h);
    }
}

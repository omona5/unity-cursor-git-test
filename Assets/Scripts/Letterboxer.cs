using UnityEngine;

/// <summary>
/// 지정한 비율(게임 영역 = background.png 비율)을 유지하며 레터박스/필러박스를 적용합니다.
/// 모든 게임·UI는 이 영역 안에만 그려지고, 밖은 막대로 채워집니다.
/// Main Camera에 붙이고, UI Canvas는 "Screen Space - Camera" + 이 카메라로 두세요.
/// </summary>
[RequireComponent(typeof(Camera))]
public class Letterboxer : MonoBehaviour
{
    [Header("Design Aspect (게임 영역 = 월드 12×15 유닛 비율)")]
    [Tooltip("기준 비율 가로·세로. 12:15 = 월드 12×15 유닛, 이 안에서만 게임이 진행됩니다")]
    [SerializeField] private float designAspectWidth = 12f;
    [SerializeField] private float designAspectHeight = 15f;

    [Header("Bar Color")]
    [Tooltip("레터박스/필러박스 색 (화면 밖 영역)")]
    [SerializeField] private Color barColor = Color.red;

    [Header("Debug Log")]
    [Tooltip("1초마다 레터박스/필러박스 모드와 비율을 콘솔에 출력")]
    [SerializeField] private bool logEverySecond = true;

    private Camera _camera;
    private Camera _clearCamera; // 전체 화면을 barColor로 채우는 카메라
    private float _logTimer;

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

    private void Update()
    {
        if (!logEverySecond) return;
        _logTimer += Time.deltaTime;
        if (_logTimer >= 1f)
        {
            _logTimer -= 1f;
            LogLetterboxState();
        }
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

    private void LogLetterboxState()
    {
        float designAspect = designAspectWidth / designAspectHeight;
        float screenAspect = (float)Screen.width / Screen.height;
        bool isPillarbox = screenAspect > designAspect;
        string mode = isPillarbox ? "필러박스 (좌/우 막대)" : "레터박스 (위/아래 막대)";
        Debug.Log($"[Letterboxer] {mode} | designAspect={designAspect:F4} ({designAspectWidth}:{designAspectHeight}) | screenAspect={screenAspect:F4} | resolution={Screen.width}x{Screen.height} | rect={_camera.rect}");
    }
}

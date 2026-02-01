using UnityEngine;

/// <summary>
/// Canvas를 카메라 뷰포트(레터박스 영역) 픽셀 크기에 맞춰 스케일합니다.
/// Canvas Scaler는 전체 화면(Screen.width/height) 기준이라 레터박스 시 UI가 더 크게 나오므로,
/// 이 스크립트로 뷰포트 크기 기준으로 맞춰 2D 스프라이트 배경과 위치를 맞출 수 있습니다.
///
/// 사용법:
/// 1) Canvas에 이 스크립트 추가
/// 2) Reference Size를 배경 크기(768, 960)로 설정
/// 3) Canvas Scaler는 "Constant Pixel Size", Scale = 1 로 두거나 비활성화
/// 4) LateUpdate에서 뷰포트에 맞춰 스케일 적용
/// </summary>
[RequireComponent(typeof(Canvas))]
public class CanvasViewportScaler : MonoBehaviour
{
    [Header("Reference Size (배경/게임 영역 픽셀 크기)")]
    [Tooltip("Letterboxer 뷰포트와 동일한 비율. 768x960 = 64*12 x 64*15")]
    [SerializeField] private float referenceWidth = 768f;
    [SerializeField] private float referenceHeight = 960f;

    private Canvas _canvas;
    private RectTransform _rect;
    private Camera _camera;
    private int _lastScreenW, _lastScreenH;
    private Rect _lastRect;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _rect = GetComponent<RectTransform>();
        _camera = _canvas.worldCamera;
        if (_camera == null)
            _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_camera == null || _rect == null) return;

        Rect r = _camera.rect;
        int sw = Screen.width;
        int sh = Screen.height;

        if (_lastScreenW == sw && _lastScreenH == sh && _lastRect == r)
            return;

        _lastScreenW = sw;
        _lastScreenH = sh;
        _lastRect = r;

        float viewportPixelW = sw * r.width;
        float viewportPixelH = sh * r.height;

        float scaleX = viewportPixelW / referenceWidth;
        float scaleY = viewportPixelH / referenceHeight;

        _rect.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}

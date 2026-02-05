using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SnakeGameManager : MonoBehaviour
{
    public static SnakeGameManager Instance { get; private set; }

    [Header("Game Settings")]
    [Tooltip("기본 이동 간격(초). 작을수록 빠름. 점수 오를수록 이 값이 줄어듦")]
    public float baseMoveInterval = 0.2f;
    [Tooltip("점수 1당 줄어드는 간격(초). 0.01 = 10점에 0.1초 감소")]
    public float moveIntervalDecreasePerScore = 0.01f;
    [Tooltip("최소 이동 간격(초). 이보다 빨라지지 않음")]
    public float minMoveInterval = 0.05f;
    public int gridSize = 20; // 그리드 크기
    public Vector2 gameAreaMin = new Vector2(-5.5f, -7f); // 가로: -5~5 (양쪽 0.5 여유), 세로: 아래 0.5 여유 후 게임 공간 시작
    public Vector2 gameAreaMax = new Vector2(5.5f, 4f);   // 가로: -5~5, 세로: 게임 공간 끝 (UI 공간 시작 전)

    [Header("Prefabs")]
    public GameObject snakeHeadPrefab;
    public GameObject snakeSegmentPrefab;
    public GameObject foodPrefab;

    [Header("UI")]
    public GameUI gameUI;
    [Tooltip("3자리 점수 스프라이트. 비어 있으면 GameUI의 scoreDigitDisplay 사용")]
    public ScoreDigitDisplay scoreDigitDisplay;

    private SnakeController snakeHead;
    private List<SnakeSegment> snakeSegments = new List<SnakeSegment>();
    private GameObject currentFood;
    private int score = 0;
    private bool isGameOver = false;
    private bool isPlaying = false;  // false = 대기(3칸 뱀만 보임), true = 이동 중
    private float moveTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetToIdle();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // R 키: 리셋 (대기 상태로 되돌림)
            if (keyboard.rKey.wasPressedThisFrame)
            {
                ResetGame();
                return;
            }

            // 대기 상태에서 스페이스/엔터: 시작하기
            if (!isPlaying && !isGameOver && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame))
            {
                StartPlay();
                return;
            }
        }

        // 대기 상태이거나 게임 오버면 이동하지 않음
        if (!isPlaying || isGameOver) return;

        // 점수에 따라 이동 간격 감소 (높은 점수 = 더 빠름)
        float interval = Mathf.Max(minMoveInterval, baseMoveInterval - score * moveIntervalDecreasePerScore);

        moveTimer += Time.deltaTime;
        if (moveTimer >= interval)
        {
            moveTimer -= interval;
            MoveSnake();
        }
    }

    /// <summary>
    /// 대기 상태로 설정. 3칸 뱀만 보이고 움직이지 않음. "시작하기"를 눌러야 게임 시작.
    /// </summary>
    public void ResetToIdle()
    {
        isGameOver = false;
        isPlaying = false;
        score = 0;
        moveTimer = 0f;

        ClearGame();

        Vector3 startPos = new Vector3(0f, 0.5f, 0f);
        GameObject headObj = Instantiate(snakeHeadPrefab, startPos, Quaternion.identity);
        snakeHead = headObj.GetComponent<SnakeController>();
        snakeHead.Initialize(Vector2.right);

        AddSegment();
        AddSegment();

        SpawnFood();

        RefreshScoreDisplay();
        if (gameUI != null)
        {
            gameUI.HideGameOver();
            gameUI.ShowStartState();
        }
    }

    /// <summary>
    /// "시작하기" 버튼으로 호출. 대기 상태에서만 유효하며, 뱀 이동을 시작함.
    /// </summary>
    public void StartPlay()
    {
        if (isGameOver || snakeHead == null) return;
        isPlaying = true;
        moveTimer = 0f;
        if (gameUI != null)
            gameUI.SetStartButtonInteractable(false); // 게임 중에는 시작 버튼 비활성화
    }

    private void MoveSnake()
    {
        if (snakeHead == null) return;

        Vector2 nextPosition = snakeHead.GetNextPosition();
        Vector2 currentHeadPos = snakeHead.transform.position;

        // 벽 충돌 체크
        if (IsOutOfBounds(nextPosition))
        {
            Debug.Log($"Out of bounds! Position: {nextPosition}, Min: {gameAreaMin}, Max: {gameAreaMax}");
            GameOver();
            return;
        }

        // 자기 자신과 충돌 체크
        if (IsSnakePosition(nextPosition))
        {
            GameOver();
            return;
        }

        // 음식 충돌 체크
        if (currentFood != null && Vector2.Distance(nextPosition, currentFood.transform.position) < 0.5f)
        {
            EatFood();
        }

        // 스네이크 이동
        snakeHead.Move();

        // 세그먼트들 이동 + 방향에 맞춰 회전
        Vector2 frontPos = snakeHead.transform.position;  // 앞쪽(헤드) 위치
        Vector2 previousPos = currentHeadPos;
        foreach (var segment in snakeSegments)
        {
            Vector2 segmentPos = segment.transform.position;
            Vector2 segDir = frontPos - previousPos;
            segment.MoveTo(previousPos);
            segment.SetDirection(segDir);
            frontPos = previousPos;
            previousPos = segmentPos;
        }
    }

    private void EatFood()
    {
        score++;
        Destroy(currentFood);
        currentFood = null;

        // 세그먼트 추가
        AddSegment();

        // 새 음식 생성
        SpawnFood();

        // UI 업데이트
        RefreshScoreDisplay();
    }

    private void RefreshScoreDisplay()
    {
        if (gameUI != null)
            gameUI.UpdateScore(score);
        // ScoreDigitDisplay 직접 갱신 (GameUI에 연결 안 했을 때도 동작)
        var display = scoreDigitDisplay != null ? scoreDigitDisplay : (gameUI != null ? gameUI.scoreDigitDisplay : null);
        if (display != null)
            display.SetScore(score);
    }

    private void AddSegment()
    {
        Vector2 spawnPos;
        if (snakeSegments.Count > 0)
        {
            // 꼬리 한 칸 뒤에 생성 (기존: 꼬리와 같은 위치라 겹침 → 2칸만 보이던 문제 수정)
            Vector2 tailPos = snakeSegments[snakeSegments.Count - 1].transform.position;
            Vector2 inFrontPos = snakeSegments.Count >= 2
                ? (Vector2)snakeSegments[snakeSegments.Count - 2].transform.position
                : (Vector2)snakeHead.transform.position;
            Vector2 dirToBody = (inFrontPos - tailPos).normalized;
            spawnPos = tailPos - dirToBody;
        }
        else if (snakeHead != null)
        {
            spawnPos = (Vector2)snakeHead.transform.position - snakeHead.GetDirection();
        }
        else
        {
            spawnPos = Vector2.zero;
        }

        GameObject segmentObj = Instantiate(snakeSegmentPrefab, spawnPos, Quaternion.identity);
        SnakeSegment segment = segmentObj.GetComponent<SnakeSegment>();
        snakeSegments.Add(segment);
    }

    private void SpawnFood()
    {
        int minX = Mathf.CeilToInt(gameAreaMin.x);
        int maxX = Mathf.FloorToInt(gameAreaMax.x);
        int minY = Mathf.CeilToInt(gameAreaMin.y);
        int maxY = Mathf.FloorToInt(gameAreaMax.y - 0.5f);

        // 스네이크(헤드+세그먼트)가 없는 칸만 모은 뒤 그중에서만 랜덤 선택
        var emptyPositions = new List<Vector2>();
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2 pos = new Vector2(x, y + 0.5f);
                if (!IsSnakePosition(pos))
                    emptyPositions.Add(pos);
            }
        }

        if (emptyPositions.Count == 0 || foodPrefab == null)
            return;

        Vector2 foodPos = emptyPositions[Random.Range(0, emptyPositions.Count)];
        currentFood = Instantiate(foodPrefab, foodPos, Quaternion.identity);
    }

    private bool IsSnakePosition(Vector2 position)
    {
        if (snakeHead != null && Vector2.Distance(position, snakeHead.transform.position) < 0.5f)
            return true;

        foreach (var segment in snakeSegments)
        {
            if (Vector2.Distance(position, segment.transform.position) < 0.5f)
                return true;
        }

        return false;
    }

    private bool IsOutOfBounds(Vector2 position)
    {
        // 경계값을 포함해서 체크 (>=, <= 사용)
        return position.x < gameAreaMin.x || position.x >= gameAreaMax.x ||
               position.y < gameAreaMin.y || position.y >= gameAreaMax.y;
    }

    private void GameOver()
    {
        isGameOver = true;
        if (gameUI != null)
        {
            gameUI.ShowGameOver(score);
        }
    }

    private void ClearGame()
    {
        if (snakeHead != null)
        {
            Destroy(snakeHead.gameObject);
            snakeHead = null;
        }

        foreach (var segment in snakeSegments)
        {
            if (segment != null)
                Destroy(segment.gameObject);
        }
        snakeSegments.Clear();

        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }
    }

    /// <summary>
    /// "리셋" 버튼으로 호출. 대기 상태(3칸 뱀)로 되돌림. 다시 "시작하기"를 눌러야 시작.
    /// </summary>
    public void ResetGame()
    {
        ResetToIdle();
    }

    /// <summary>
    /// 이전 호환용. 리셋과 동일하게 대기 상태로 되돌림.
    /// </summary>
    public void RestartGame()
    {
        ResetGame();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}

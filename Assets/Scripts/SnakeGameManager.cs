using UnityEngine;
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
        StartGame();
    }

    private void Update()
    {
        if (isGameOver) return;

        // 점수에 따라 이동 간격 감소 (높은 점수 = 더 빠름)
        float interval = Mathf.Max(minMoveInterval, baseMoveInterval - score * moveIntervalDecreasePerScore);

        moveTimer += Time.deltaTime;
        if (moveTimer >= interval)
        {
            moveTimer -= interval;
            MoveSnake();
        }
    }

    public void StartGame()
    {
        isGameOver = false;
        score = 0;
        moveTimer = 0f;

        // 기존 오브젝트 정리
        ClearGame();

        // 스네이크 헤드 생성 (아래쪽 0.5유닛 여유를 위해 y=0.5에서 시작)
        Vector3 startPos = new Vector3(0f, 0.5f, 0f);
        GameObject headObj = Instantiate(snakeHeadPrefab, startPos, Quaternion.identity);
        snakeHead = headObj.GetComponent<SnakeController>();
        snakeHead.Initialize(Vector2.right);

        // 초기 세그먼트 추가
        AddSegment();
        AddSegment();

        // 음식 생성
        SpawnFood();

        // UI 업데이트
        RefreshScoreDisplay();
        if (gameUI != null)
            gameUI.HideGameOver();
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
        Vector2 spawnPos = Vector2.zero;
        if (snakeSegments.Count > 0)
        {
            spawnPos = snakeSegments[snakeSegments.Count - 1].transform.position;
        }
        else if (snakeHead != null)
        {
            spawnPos = (Vector2)snakeHead.transform.position - snakeHead.GetDirection();
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

    public void RestartGame()
    {
        StartGame();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class SnakeGameManager : MonoBehaviour
{
    public static SnakeGameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameSpeed = 0.2f; // 이동 간격 (초)
    public int gridSize = 20; // 그리드 크기
    public Vector2 gameAreaMin = new Vector2(-5f, -6.5f); // 가로: -5~5 (양쪽 0.5 여유), 세로: 아래 0.5 여유 후 게임 공간 시작
    public Vector2 gameAreaMax = new Vector2(5f, 3.5f);   // 가로: -5~5, 세로: 게임 공간 끝 (UI 공간 시작 전)

    [Header("Prefabs")]
    public GameObject snakeHeadPrefab;
    public GameObject snakeSegmentPrefab;
    public GameObject foodPrefab;

    [Header("UI")]
    public GameUI gameUI;

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

        moveTimer += Time.deltaTime;
        if (moveTimer >= gameSpeed)
        {
            moveTimer = 0f;
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
        if (gameUI != null)
        {
            gameUI.UpdateScore(score);
            gameUI.HideGameOver();
        }
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
        
        // 세그먼트들 이동
        Vector2 previousPos = currentHeadPos;
        foreach (var segment in snakeSegments)
        {
            Vector2 segmentPos = segment.transform.position;
            segment.MoveTo(previousPos);
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
        if (gameUI != null)
        {
            gameUI.UpdateScore(score);
        }
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
        Vector2 foodPos;
        int attempts = 0;
        do
        {
            // 게임 영역 내에서 정수 좌표로 생성 (격자에 맞춤)
            // gameAreaMin과 gameAreaMax는 경계값이므로, 실제 생성 가능한 정수 범위 계산
            // 예: -5.5 ~ 5.5 → 정수 -5 ~ 5
            int minX = Mathf.CeilToInt(gameAreaMin.x);
            int maxX = Mathf.FloorToInt(gameAreaMax.x);
            int minY = Mathf.CeilToInt(gameAreaMin.y);
            int maxY = Mathf.FloorToInt(gameAreaMax.y);
            
            int x = Random.Range(minX, maxX + 1);
            int y = Random.Range(minY, maxY + 1);
            // 스네이크가 (0, 0.5)에서 시작하므로 음식도 0.5 오프셋 적용
            foodPos = new Vector2(x, y + 0.5f);
            attempts++;
        } while (IsSnakePosition(foodPos) && attempts < 100);

        if (foodPrefab != null)
        {
            currentFood = Instantiate(foodPrefab, foodPos, Quaternion.identity);
        }
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

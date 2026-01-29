using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public ScoreDigitDisplay scoreDigitDisplay;  // 3자리 숫자 스프라이트 점수 (있으면 우선 사용)
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI restartHintText;
    public GameObject gameOverPanel;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreDigitDisplay != null)
        {
            scoreDigitDisplay.SetScore(score);
        }
        if (scoreText != null)
        {
            scoreText.text = "점수: " + score;
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = "게임 오버!\n최종 점수: " + finalScore;
        }

        if (restartHintText != null)
        {
            restartHintText.text = "R 키를 눌러 재시작";
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // R 키로 재시작
        if (Input.GetKeyDown(KeyCode.R) && SnakeGameManager.Instance != null)
        {
            if (SnakeGameManager.Instance.IsGameOver())
            {
                SnakeGameManager.Instance.RestartGame();
            }
        }
    }
}

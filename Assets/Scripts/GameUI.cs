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

    [Header("시작/리셋 버튼 (Inspector에서 연결)")]
    [Tooltip("시작하기 버튼. 대기 상태에서만 보이고, 누르면 게임 시작")]
    public GameObject startButton;
    [Tooltip("리셋 버튼. 누르면 3칸 뱀 대기 상태로 되돌림. 게임오버 패널 안의 버튼을 여기 연결해도 됨")]
    public Button resetButton;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        ShowStartState();

        // 시작하기 버튼 클릭을 코드에서도 연결 (Inspector에서 안 넣었을 때 대비)
        if (startButton != null)
        {
            var btn = startButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(OnStartClicked);
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
            restartHintText.text = "리셋을 누른 뒤 시작하기를 눌러 재시작";
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    /// <summary> 대기 상태 UI: 시작하기 버튼 보이기 + 활성화 </summary>
    public void ShowStartState()
    {
        if (startButton != null)
        {
            startButton.SetActive(true);
            SetStartButtonInteractable(true);
        }
    }

    /// <summary> 시작하기 버튼 클릭 가능 여부 (게임 진행 중에는 false) </summary>
    public void SetStartButtonInteractable(bool interactable)
    {
        if (startButton != null)
        {
            var btn = startButton.GetComponent<Button>();
            if (btn != null)
                btn.interactable = interactable;
        }
    }

    /// <summary> 게임 진행 중: 시작하기 버튼 숨기기 </summary>
    public void HideStartState()
    {
        if (startButton != null)
            startButton.SetActive(false);
    }

    /// <summary> 시작하기 버튼 OnClick에서 호출 (Inspector 연결용) </summary>
    public void OnStartClicked()
    {
        if (SnakeGameManager.Instance != null && !SnakeGameManager.Instance.IsPlaying() && !SnakeGameManager.Instance.IsGameOver())
            SnakeGameManager.Instance.StartPlay();
    }

    /// <summary> 리셋 버튼 OnClick에서 호출 (Inspector 연결용) </summary>
    public void OnResetClicked()
    {
        if (SnakeGameManager.Instance != null)
            SnakeGameManager.Instance.ResetGame();
    }

}

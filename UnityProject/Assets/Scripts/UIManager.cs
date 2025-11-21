using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI smurfsText;
    public TextMeshProUGUI ammoText;
    public GameObject hudPanel;

    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button playButton;
    
    [Header("Countdown")]
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    
    [Header("Round Complete")]
    public GameObject roundCompletePanel;
    public TextMeshProUGUI roundCompleteText;
    
    [Header("Victory")]
    public GameObject victoryPanel;
    public Button victoryPlayAgainButton;
    public Button victoryMenuButton;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;
    public Button gameOverPlayAgainButton;
    public Button gameOverMenuButton;

    [Header("Game Manager")]
    public GameManager gameManager;
    public SlingshotController slingshotController;

    void Start()
    {
        // Configurar botones
        if (playButton != null)
            playButton.onClick.AddListener(() => gameManager.OnPlayButtonPressed());
        
        if (victoryPlayAgainButton != null)
            victoryPlayAgainButton.onClick.AddListener(() => gameManager.RestartGame());
        
        if (victoryMenuButton != null)
            victoryMenuButton.onClick.AddListener(() => gameManager.GoToMainMenu());
        
        if (gameOverPlayAgainButton != null)
            gameOverPlayAgainButton.onClick.AddListener(() => gameManager.RestartGame());
        
        if (gameOverMenuButton != null)
            gameOverMenuButton.onClick.AddListener(() => gameManager.GoToMainMenu());
    }

    void Update()
    {
        if (gameManager == null) return;
        if (gameManager.GetCurrentState() != GameManager.GameState.RoundInProgress) return;

        UpdateHUD();
    }

    void UpdateHUD()
    {
        // Timer
        float time = gameManager.GetTimeRemaining();
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        
        if (timerText != null)
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
            timerText.color = time <= 10f ? Color.red : Color.white;
        }

        // Round info
        if (roundText != null)
        {
            roundText.text = $"Ronda {gameManager.GetCurrentRound()}/{gameManager.GetTotalRounds()}";
        }

        // Smurfs
        if (smurfsText != null)
        {
            smurfsText.text = $"Pitufos: {gameManager.GetSmurfsPainted()}/{gameManager.GetTotalSmurfsThisRound()}";
        }

        // Munición
        if (ammoText != null && slingshotController != null)
        {
            int currentAmmo = slingshotController.GetCurrentAmmo();
            int maxAmmo = slingshotController.GetMaxAmmo();
            ammoText.text = $"Tomates: {currentAmmo}/{maxAmmo}";
            
            if (currentAmmo <= 3)
                ammoText.color = Color.red;
            else if (currentAmmo <= 5)
                ammoText.color = Color.yellow;
            else
                ammoText.color = Color.white;
        }
    }

    // === MAIN MENU ===
    public void ShowMainMenu()
    {
        SetPanelActive(mainMenuPanel, true);
    }

    public void HideMainMenu()
    {
        SetPanelActive(mainMenuPanel, false);
    }

    // === HUD ===
    public void ShowHUD()
    {
        SetPanelActive(hudPanel, true);
    }

    public void HideHUD()
    {
        SetPanelActive(hudPanel, false);
    }

    // === COUNTDOWN ===
    public void ShowCountdown(string text)
    {
        if (countdownPanel != null) countdownPanel.SetActive(true);
        if (countdownText != null) countdownText.text = text;
    }

    public void HideCountdown()
    {
        SetPanelActive(countdownPanel, false);
    }

    // === ROUND COMPLETE ===
    public void ShowRoundComplete(int completedRound, int nextRound)
    {
        if (roundCompletePanel != null) roundCompletePanel.SetActive(true);
        if (roundCompleteText != null)
        {
            roundCompleteText.text = $"¡Ronda {completedRound} Completada!\n\nIniciando Ronda {nextRound}...";
        }
    }

    public void HideRoundComplete()
    {
        SetPanelActive(roundCompletePanel, false);
    }

    // === VICTORY ===
    public void ShowVictory()
    {
        SetPanelActive(victoryPanel, true);
    }

    public void HideVictory()
    {
        SetPanelActive(victoryPanel, false);
    }

    // === GAME OVER ===
    public void ShowGameOver(string reason, int reachedRound)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = $"GAME OVER\n\n{reason}\n\nLlegaste a la Ronda {reachedRound}";
        }
    }

    public void HideGameOver()
    {
        SetPanelActive(gameOverPanel, false);
    }

    // === HELPER ===
    void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int totalRounds = 3;
    public float[] roundDurations = { 40f, 35f, 30f };
    public int[] smurfsPerRound = { 5, 8, 12 };
    
    [Header("References")]
    public ARPlaneManager planeManager;
    public SpawnManager spawnManager;
    public SlingshotController slingshotController;
    public UIManager uiManager;
    
    [Header("Testing")]
    public bool skipMainMenu = false; // Para testing rápido
    public float autoStartDelay = 2f;
    
    // Estado del juego
    private int currentRound = 0;
    private float timeRemaining;
    private int smurfsPainted = 0;
    private int totalSmurfsThisRound = 0;
    private bool gameStarted = false;
    private bool roundInProgress = false;
    private GameState currentState = GameState.MainMenu;

    public enum GameState
    {
        MainMenu,
        Countdown,
        RoundInProgress,
        RoundComplete,
        Victory,
        GameOver
    }

    void Start()
    {
        if (planeManager != null)
        {
            planeManager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
        
        // Deshabilitar disparo al inicio
        if (slingshotController != null)
        {
            slingshotController.SetCanShoot(false);
        }
        
        // Ocultar HUD al inicio
        if (uiManager != null)
        {
            uiManager.HideHUD();
            uiManager.ShowMainMenu();
        }
        
        // Testing: skip main menu
        if (skipMainMenu)
        {
            Invoke("OnPlayButtonPressed", autoStartDelay);
        }
    }

    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (!gameStarted && args.added.Count > 0 && skipMainMenu)
        {
            OnPlayButtonPressed();
        }
    }

    // Llamado por el botón PLAY del main menu
    public void OnPlayButtonPressed()
    {
        if (currentState != GameState.MainMenu) return;
        
        Debug.Log("=== INICIANDO NUEVO JUEGO ===");
        
        if (uiManager != null)
        {
            uiManager.HideMainMenu();
        }
        
        gameStarted = true;
        StartNewGame();
    }

    void StartNewGame()
    {
        currentRound = 0;
        currentState = GameState.Countdown;
        
        Debug.Log("=== NUEVO JUEGO ===");
        StartNextRound();
    }

    void StartNextRound()
    {
        if (currentRound >= totalRounds)
        {
            Victory();
            return;
        }

        // Limpiar pitufos de la ronda anterior
        spawnManager.ClearAllSmurfs();

        currentRound++;
        smurfsPainted = 0;
        totalSmurfsThisRound = smurfsPerRound[currentRound - 1];
        timeRemaining = roundDurations[currentRound - 1];
        
        // Inicializar munición para esta ronda
        if (slingshotController != null)
        {
            slingshotController.InitializeAmmoForRound(currentRound);
            slingshotController.SetCanShoot(false); // No disparar durante countdown
        }

        Debug.Log($"=== PREPARANDO RONDA {currentRound}/{totalRounds} ===");
        Debug.Log($"Pitufos a pintar: {totalSmurfsThisRound}");
        Debug.Log($"Tiempo: {timeRemaining} segundos");

        // Mostrar countdown antes de empezar la ronda
        StartCoroutine(RoundCountdown());
    }

    IEnumerator RoundCountdown()
    {
        currentState = GameState.Countdown;
        
        // Spawnear pitufos pero sin permitir disparar
        Vector3 spawnCenter = GetSpawnCenter();
        spawnManager.SpawnSmurfsForRound(spawnCenter, totalSmurfsThisRound);
        
        // Mostrar HUD
        if (uiManager != null)
        {
            uiManager.ShowHUD();
        }
        
        // Countdown 3, 2, 1, GO
        if (uiManager != null)
        {
            uiManager.ShowCountdown("3");
            yield return new WaitForSeconds(1f);
            
            uiManager.ShowCountdown("2");
            yield return new WaitForSeconds(1f);
            
            uiManager.ShowCountdown("1");
            yield return new WaitForSeconds(1f);
            
            uiManager.ShowCountdown("GO!");
            yield return new WaitForSeconds(0.5f);
            
            uiManager.HideCountdown();
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
        
        // Iniciar la ronda
        BeginRound();
    }

    void BeginRound()
    {
        roundInProgress = true;
        currentState = GameState.RoundInProgress;
        
        // Permitir disparar
        if (slingshotController != null)
        {
            slingshotController.SetCanShoot(true);
        }
        
        Debug.Log($"=== ¡RONDA {currentRound} INICIADA! ===");
    }

    Vector3 GetSpawnCenter()
    {
        if (Camera.main != null)
        {
            Vector3 center = Camera.main.transform.position + Camera.main.transform.forward * 3f;
            center.y = 0f;
            return center;
        }
        return Vector3.zero;
    }

    void Update()
    {
        if (currentState != GameState.RoundInProgress) return;

        // Countdown del timer
        timeRemaining -= Time.deltaTime;
        
        // Game Over si se acaba el tiempo
        if (timeRemaining <= 0)
        {
            GameOver("¡Se acabó el tiempo!");
        }

        // Debug
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log($"Ronda {currentRound} | Tiempo: {timeRemaining:F1}s | Pintados: {smurfsPainted}/{totalSmurfsThisRound}");
        }
    }

    public void SmurfPainted()
    {
        smurfsPainted++;
        Debug.Log($"¡Pitufo pintado! ({smurfsPainted}/{totalSmurfsThisRound})");
        
        if (smurfsPainted >= totalSmurfsThisRound)
        {
            RoundComplete();
        }
    }

    public void OnAmmoEmpty()
    {
        if (smurfsPainted < totalSmurfsThisRound)
        {
            GameOver("¡Te quedaste sin tomates!");
        }
    }

    void RoundComplete()
    {
        roundInProgress = false;
        currentState = GameState.RoundComplete;
        
        // Deshabilitar disparo
        if (slingshotController != null)
        {
            slingshotController.SetCanShoot(false);
        }
        
        Debug.Log($"=== ¡RONDA {currentRound} COMPLETADA! ===");
        
        // Mostrar panel de ronda completada
        StartCoroutine(ShowRoundCompleteSequence());
    }

    IEnumerator ShowRoundCompleteSequence()
    {
        if (uiManager != null)
        {
            int nextRound = currentRound + 1;
            if (nextRound <= totalRounds)
            {
                uiManager.ShowRoundComplete(currentRound, nextRound);
            }
        }
        
        yield return new WaitForSeconds(3f);
        
        if (uiManager != null)
        {
            uiManager.HideRoundComplete();
        }
        
        StartNextRound();
    }

    void Victory()
    {
        currentState = GameState.Victory;
        roundInProgress = false;
        
        // Deshabilitar disparo
        if (slingshotController != null)
        {
            slingshotController.SetCanShoot(false);
        }
        
        Debug.Log("=================================");
        Debug.Log("========== ¡VICTORIA! ===========");
        Debug.Log("=================================");

        if (uiManager != null)
        {
            uiManager.HideHUD();
            uiManager.ShowVictory();
        }
    }

    void GameOver(string reason)
    {
        currentState = GameState.GameOver;
        roundInProgress = false;
        timeRemaining = 0;
        
        // Deshabilitar disparo
        if (slingshotController != null)
        {
            slingshotController.SetCanShoot(false);
        }
        
        Debug.Log("=================================");
        Debug.Log("========== GAME OVER ===========");
        Debug.Log($"Razón: {reason}");
        Debug.Log($"Llegaste hasta la Ronda {currentRound}");
        Debug.Log("=================================");

        if (uiManager != null)
        {
            uiManager.HideHUD();
            uiManager.ShowGameOver(reason, currentRound);
        }
    }

    // === BOTONES UI ===
    public void RestartGame()
    {
        Debug.Log("Reiniciando juego...");
        
        // Limpiar escena
        spawnManager.ClearAllSmurfs();
        
        // Ocultar todos los paneles
        if (uiManager != null)
        {
            uiManager.HideGameOver();
            uiManager.HideVictory();
        }
        
        // Reiniciar
        gameStarted = false;
        StartNewGame();
    }

    public void GoToMainMenu()
    {
        Debug.Log("Volviendo al menú principal...");
        
        // Limpiar escena
        spawnManager.ClearAllSmurfs();
        
        // Resetear estado
        gameStarted = false;
        currentState = GameState.MainMenu;
        
        // Mostrar main menu
        if (uiManager != null)
        {
            uiManager.HideGameOver();
            uiManager.HideVictory();
            uiManager.HideHUD();
            uiManager.ShowMainMenu();
        }
    }

    // === GETTERS ===
    public int GetCurrentRound() => currentRound;
    public int GetTotalRounds() => totalRounds;
    public float GetTimeRemaining() => timeRemaining;
    public int GetSmurfsPainted() => smurfsPainted;
    public int GetTotalSmurfsThisRound() => totalSmurfsThisRound;
    public GameState GetCurrentState() => currentState;

    void OnDestroy()
    {
        if (planeManager != null)
        {
            planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }
    }
}
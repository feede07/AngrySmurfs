using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int totalRounds = 3;
    public float[] roundDurations = { 40f, 35f, 30f }; // Duración por ronda
    public int[] smurfsPerRound = { 5, 8, 12 }; // Pitufos por ronda
    
    [Header("References")]
    public ARPlaneManager planeManager;
    public SpawnManager spawnManager;
    
    [Header("Testing")]
    public bool autoStartForTesting = true;
    public float autoStartDelay = 2f;
    
    // Estado del juego
    private int currentRound = 0;
    private float timeRemaining;
    private int smurfsPainted = 0;
    private int totalSmurfsThisRound = 0;
    private bool gameStarted = false;
    private bool roundInProgress = false;
    private GameState currentState = GameState.WaitingToStart;

    private enum GameState
    {
        WaitingToStart,
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
        
        // Para testing: auto-iniciar el juego
        if (autoStartForTesting)
        {
            Invoke("ForceStartGame", autoStartDelay);
        }
    }

    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (!gameStarted && args.added.Count > 0)
        {
            gameStarted = true;
            StartNewGame();
            Debug.Log("Plano AR detectado! Iniciando juego...");
        }
    }

    void ForceStartGame()
    {
        if (!gameStarted)
        {
            Debug.Log("=== AUTO-INICIANDO JUEGO PARA TESTING ===");
            gameStarted = true;
            StartNewGame();
        }
    }

    void StartNewGame()
    {
        currentRound = 0;
        currentState = GameState.WaitingToStart;
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

    // AÑADIR: Limpiar pitufos de la ronda anterior
    spawnManager.ClearAllSmurfs();

    currentRound++;
    smurfsPainted = 0;
    totalSmurfsThisRound = smurfsPerRound[currentRound - 1];
    timeRemaining = roundDurations[currentRound - 1];
    roundInProgress = true;
    currentState = GameState.RoundInProgress;

    Debug.Log($"=== RONDA {currentRound}/{totalRounds} ===");
    Debug.Log($"Pitufos a pintar: {totalSmurfsThisRound}");
    Debug.Log($"Tiempo: {timeRemaining} segundos");

    // Spawnear pitufos
    if (Camera.main != null)
    {
        Vector3 spawnCenter = Camera.main.transform.position + Camera.main.transform.forward * 3f;
        spawnCenter.y = 0f; // Spawn en el suelo
        
        spawnManager.SpawnSmurfsForRound(spawnCenter, totalSmurfsThisRound);
    }
    else
    {
        Debug.LogError("No se encontró Camera.main!");
    }

    // Placeholder: Aquí iría la UI de inicio de ronda
    ShowRoundStartUI();
}

    void Update()
{
    if (currentState != GameState.RoundInProgress) return;

    // Countdown del timer
    timeRemaining -= Time.deltaTime;
    
    // Game Over si se acaba el tiempo
    if (timeRemaining <= 0)
    {
        GameOver();
    }

    // Debug: presiona Space para ver info (nuevo Input System)
    if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
    {
        Debug.Log($"Ronda {currentRound} | Tiempo: {timeRemaining:F1}s | Pintados: {smurfsPainted}/{totalSmurfsThisRound}");
    }
}

    public void SmurfPainted()
    {
        smurfsPainted++;
        Debug.Log($"¡Pitufo pintado! ({smurfsPainted}/{totalSmurfsThisRound})");
        
        // Comprobar si se completó la ronda
        if (smurfsPainted >= totalSmurfsThisRound)
        {
            RoundComplete();
        }
    }

    void RoundComplete()
    {
        roundInProgress = false;
        currentState = GameState.RoundComplete;
        
        Debug.Log($"=== ¡RONDA {currentRound} COMPLETADA! ===");
        Debug.Log($"Tiempo restante: {timeRemaining:F1} segundos");

        // Placeholder: Aquí iría la UI de ronda completada
        ShowRoundCompleteUI();

        // Esperar 2 segundos antes de la siguiente ronda
        Invoke("StartNextRound", 2f);
    }

    void Victory()
    {
        currentState = GameState.Victory;
        roundInProgress = false;
        
        Debug.Log("=================================");
        Debug.Log("========== ¡VICTORIA! ===========");
        Debug.Log("=== ¡HAS COMPLETADO EL JUEGO! ===");
        Debug.Log("=================================");

        // Placeholder: Aquí iría la UI de victoria
        ShowVictoryUI();
    }

    void GameOver()
    {
        currentState = GameState.GameOver;
        roundInProgress = false;
        timeRemaining = 0;
        
        Debug.Log("=================================");
        Debug.Log("========== GAME OVER ===========");
        Debug.Log($"Llegaste hasta la Ronda {currentRound}");
        Debug.Log($"Pitufos pintados: {smurfsPainted}/{totalSmurfsThisRound}");
        Debug.Log("=================================");

        // Placeholder: Aquí iría la UI de Game Over
        ShowGameOverUI();
    }

    // === PLACEHOLDERS PARA UI ===
    void ShowRoundStartUI()
    {
        // TODO: Mostrar UI "Ronda X - ¡Empieza!"
        Debug.Log($"[UI PLACEHOLDER] Mostrando inicio Ronda {currentRound}");
    }

    void ShowRoundCompleteUI()
    {
        // TODO: Mostrar UI "¡Ronda Completada!"
        Debug.Log("[UI PLACEHOLDER] Mostrando ronda completada");
    }

    void ShowVictoryUI()
    {
        // TODO: Mostrar UI Victoria con botones: Volver a Jugar / Menú Principal
        Debug.Log("[UI PLACEHOLDER] Mostrando pantalla de Victoria");
    }

    void ShowGameOverUI()
    {
        // TODO: Mostrar UI Game Over con botones: Volver a Jugar / Menú Principal
        Debug.Log("[UI PLACEHOLDER] Mostrando pantalla de Game Over");
    }

    // === MÉTODOS PÚBLICOS PARA UI (para cuando implementes los botones) ===
    public void RestartGame()
    {
        Debug.Log("Reiniciando juego...");
        // Limpiar pitufos actuales
        spawnManager.ClearAllSmurfs();
        // Reiniciar juego
        StartNewGame();
    }

    public void GoToMainMenu()
    {
        Debug.Log("Volviendo al menú principal...");
        // TODO: Cargar escena del menú principal
        // SceneManager.LoadScene("MainMenu");
    }

    // === GETTERS para UI ===
    public int GetCurrentRound() => currentRound;
    public int GetTotalRounds() => totalRounds;
    public float GetTimeRemaining() => timeRemaining;
    public int GetSmurfsPainted() => smurfsPainted;
    public int GetTotalSmurfsThisRound() => totalSmurfsThisRound;
    //public GameState GetCurrentState() => currentState;

    void OnDestroy()
    {
        if (planeManager != null)
        {
            planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }
    }
}
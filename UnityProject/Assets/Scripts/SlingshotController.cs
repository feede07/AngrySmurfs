using UnityEngine;
using UnityEngine.InputSystem;

public class SlingshotController : MonoBehaviour
{
    [Header("References")]
    public GameObject tomatoPrefab;
    public Transform shootPoint;
    
    [Header("Shooting Settings")]
    public float shootForce = 15f;
    public float shootCooldown = 0.5f;
    
    [Header("Ammo Settings")]
    public int[] ammoPerRound = { 15, 20, 25 }; // Munición por ronda
    
    private int currentAmmo;
    private int maxAmmo;
    private float lastShootTime;
    private bool canShoot = true;
    private bool shootingEnabled = true;

    void Start()
    {
        if (shootPoint == null)
        {
            shootPoint = Camera.main.transform;
        }
    }

    [System.Obsolete]
    void Update()
    {
        if (!canShoot || !shootingEnabled) return;

        // Detectar touch usando el nuevo Input System
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                Shoot();
            }
        }
        
        // Para probar en editor con mouse
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    public void InitializeAmmoForRound(int roundNumber)
    {
        // roundNumber es 1-indexed (1, 2, 3)
        int roundIndex = roundNumber - 1;
        
        if (roundIndex >= 0 && roundIndex < ammoPerRound.Length)
        {
            maxAmmo = ammoPerRound[roundIndex];
            currentAmmo = maxAmmo;
            canShoot = true;
            Debug.Log($"Munición inicializada: {currentAmmo} tomates para Ronda {roundNumber}");
        }
        else
        {
            // Por si acaso hay más rondas, usar la última cantidad configurada
            maxAmmo = ammoPerRound[ammoPerRound.Length - 1];
            currentAmmo = maxAmmo;
            canShoot = true;
        }
    }
    public void SetCanShoot(bool enabled)
{
    shootingEnabled = enabled;
    Debug.Log($"Disparo {(enabled ? "habilitado" : "deshabilitado")}");
}

    [System.Obsolete]
    void Shoot()
    {
        // Comprobar munición
        if (currentAmmo <= 0)
        {
            Debug.Log("¡Sin munición!");
            return;
        }

        // Comprobar cooldown
        if (Time.time - lastShootTime < shootCooldown)
            return;

        lastShootTime = Time.time;

        // Verificar referencias
        if (tomatoPrefab == null)
        {
            Debug.LogError("¡Tomato Prefab no asignado!");
            return;
        }

        if (shootPoint == null)
        {
            Debug.LogError("¡Shoot Point no asignado!");
            return;
        }

        // Crear tomate
        GameObject tomato = Instantiate(tomatoPrefab, shootPoint.position, shootPoint.rotation);
        
        // Añadir fuerza
        Rigidbody rb = tomato.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * shootForce;
        }

        // Reducir munición
        currentAmmo--;
        Debug.Log($"¡Tomate lanzado! Munición restante: {currentAmmo}");

        // Comprobar si se acabó la munición
        if (currentAmmo <= 0)
        {
            OnAmmoEmpty();
        }
    }

    [System.Obsolete]
    void OnAmmoEmpty()
    {
        Debug.Log("¡Munición agotada!");
        canShoot = false;
        
        // Notificar al GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnAmmoEmpty();
        }
    }

    // Getters para UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool CanShoot() => canShoot && currentAmmo > 0;

    // Para debugging
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        canShoot = true;
    }
}
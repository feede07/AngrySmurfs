using UnityEngine;
using UnityEngine.InputSystem;

public class SlingshotController : MonoBehaviour
{
    [Header("References")]
    public GameObject tomatoPrefab;
    public Transform shootPoint; // La cámara AR
    
    [Header("Shooting Settings")]
    public float shootForce = 15f;
    public float shootCooldown = 0.5f;
    
    private float lastShootTime;

    void Start()
    {
        // Si no se asigna, usar la cámara principal
        if (shootPoint == null)
        {
            shootPoint = Camera.main.transform;
        }
    }

    void Update()
    {
        // Detectar touch usando el nuevo Input System
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                Shoot();
            }
        }
        
        // Para probar en editor con mouse (nuevo Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Comprobar cooldown
        if (Time.time - lastShootTime < shootCooldown)
            return;

        lastShootTime = Time.time;

        // Verificar que tenemos los prefabs asignados
        if (tomatoPrefab == null)
        {
            Debug.LogError("¡Tomato Prefab no asignado en SlingshotController!");
            return;
        }

        if (shootPoint == null)
        {
            Debug.LogError("¡Shoot Point no asignado en SlingshotController!");
            return;
        }

        // Crear tomate
        GameObject tomato = Instantiate(tomatoPrefab, shootPoint.position, shootPoint.rotation);
        
        // Añadir fuerza
        Rigidbody rb = tomato.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootPoint.forward * shootForce;
        }

        Debug.Log("¡Tomate lanzado!");
    }
}
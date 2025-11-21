using UnityEngine;

public class SmurfController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 2f;
    public float changeDirectionTime = 3f;
    public float maxDistanceFromSpawn = 3f;
    
    [Header("Visual")]
    public Material blueMaterial;
    public Material redMaterial;
    private Renderer smurfRenderer;
    
    private Vector3 spawnPoint;
    private Vector3 moveDirection;
    private float directionTimer;
    private bool isPainted = false;
    private Rigidbody rb;

    void Start()
    {
        smurfRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        
        // Configurar Rigidbody para que no caiga
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation; // No rotar con física
        }
        
        // Crear materiales si no están asignados
        if (blueMaterial == null)
        {
            blueMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            blueMaterial.color = new Color(0.2f, 0.4f, 1f); // Azul
        }
        
        if (redMaterial == null)
        {
            redMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            redMaterial.color = Color.red;
        }
        
        smurfRenderer.material = blueMaterial;
        spawnPoint = transform.position;
        ChangeDirection();
    }

    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
        spawnPoint.y = transform.position.y; // Mantener la altura
    }

    void Update()
    {
        if (isPainted) return;

        // Cambiar dirección periódicamente
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0)
        {
            ChangeDirection();
        }

        // Movimiento
        Move();
        
        // Mantener en área circular
        KeepInBounds();
    }

    void Move()
    {
        // Mover en la dirección actual
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        transform.position += movement;
        
        // Rotar hacia la dirección de movimiento
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ChangeDirection()
    {
        // Nueva dirección aleatoria (solo en horizontal)
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;
        
        directionTimer = changeDirectionTime + Random.Range(-1f, 1f); // Variación
    }

    void KeepInBounds()
    {
        // Calcular distancia desde el spawn point
        Vector3 flatPosition = transform.position;
        flatPosition.y = spawnPoint.y;
        
        Vector3 flatSpawn = spawnPoint;
        flatSpawn.y = spawnPoint.y;
        
        float distance = Vector3.Distance(flatPosition, flatSpawn);
        
        // Si está muy lejos, girar hacia el centro
        if (distance > maxDistanceFromSpawn)
        {
            Vector3 toCenter = (flatSpawn - flatPosition).normalized;
            moveDirection = toCenter;
            directionTimer = changeDirectionTime; // Resetear timer
        }
    }

    public void PaintRed()
    {
        if (isPainted) return;
        
        isPainted = true;
        smurfRenderer.material = redMaterial;
        
        // Detener movimiento
        moveSpeed = 0f;
        
        // Detener física
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        // Notificar al GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SmurfPainted();
        }
        
        Debug.Log("¡Pitufo pintado de rojo!");
        
        // NO destruir - se queda en escena pintado
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tomato"))
        {
            PaintRed();
        }
    }
}
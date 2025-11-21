using UnityEngine;

public class TomatoProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;
    public GameObject splashEffect; // Opcional: efecto de salpicadura
    
    void Start()
    {
        // Autodestruir después del tiempo de vida
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si toca un pitufo
        if (collision.gameObject.CompareTag("Smurf"))
        {
            SmurfController smurf = collision.gameObject.GetComponent<SmurfController>();
            if (smurf != null)
            {
                smurf.PaintRed();
            }
        }

        // Efecto de salpicadura (opcional)
        if (splashEffect != null)
        {
            Instantiate(splashEffect, transform.position, Quaternion.identity);
        }

        // Destruir el tomate al impactar
        Destroy(gameObject);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSpawn : MonoBehaviour
{
    public GameObject smurfPrefab;
    public Transform spawnPoint;
    
    void Update()
    {
        // Presiona T para spawnear un pitufo de prueba
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (smurfPrefab != null && spawnPoint != null)
            {
                Vector3 spawnPos = spawnPoint.position + spawnPoint.forward * 2f;
                GameObject smurf = Instantiate(smurfPrefab, spawnPos, Quaternion.identity);
                Debug.Log("Pitufo spawneado en: " + spawnPos);
            }
            else
            {
                Debug.LogError("Falta asignar SmurfPrefab o SpawnPoint!");
            }
        }
    }
}
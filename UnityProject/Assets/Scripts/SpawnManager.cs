using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    public ARPlaneManager planeManager;
    public GameObject smurfPrefab;
    public GameObject groundPlanePrefab; // Plano invisible para física
    
    [Header("Spawn Settings")]
    public float spawnRadius = 3f;
    public float spawnHeight = 0.15f; // Altura desde el suelo
    public float minSpawnDistance = 2f;
    public float maxSpawnDistance = 4f;
    
    private ARPlane detectedPlane;
    private List<GameObject> activeSmurfs = new List<GameObject>();
    private GameObject currentGroundPlane;

    void Start()
    {
        if (planeManager != null)
        {
            planeManager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
    }

    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (detectedPlane == null && args.added.Count > 0)
        {
            detectedPlane = args.added[0];
            Debug.Log("Plano AR detectado! Listo para spawnear pitufos.");
        }
    }

    public void SpawnSmurfs(int count)
    {
        if (detectedPlane == null)
        {
            Debug.LogWarning("No hay plano detectado aún.");
            return;
        }

        Vector3 planeCenter = detectedPlane.center;
        SpawnSmurfsForRound(planeCenter, count);
    }

    public void SpawnSmurfsForRound(Vector3 center, int count)
    {
        if (smurfPrefab == null)
        {
            Debug.LogError("¡SmurfPrefab es NULL! Asigna el prefab en el Inspector");
            return;
        }

        Debug.Log($"Spawneando {count} pitufos en posición: {center}");
        
        // Crear plano invisible para física
        CreateGroundPlane(center);
        
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = GetRandomSpawnPosition(center);
            GameObject smurf = Instantiate(smurfPrefab, randomPos, Quaternion.identity);
            
            // Rotación aleatoria
            smurf.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            // Configurar el pitufo
            SmurfController controller = smurf.GetComponent<SmurfController>();
            if (controller != null)
            {
                controller.SetSpawnPoint(center);
            }
            
            activeSmurfs.Add(smurf);
            Debug.Log($"Pitufo {i+1}/{count} spawneado en: {randomPos}");
        }
    }

    void CreateGroundPlane(Vector3 center)
    {
        // Eliminar plano anterior si existe
        if (currentGroundPlane != null)
        {
            Destroy(currentGroundPlane);
        }

        // Crear nuevo plano invisible
        currentGroundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        currentGroundPlane.name = "GroundPlane";
        currentGroundPlane.transform.position = center;
        currentGroundPlane.transform.localScale = new Vector3(2f, 1f, 2f); // 20m x 20m
        
        // Hacer invisible
        Renderer renderer = currentGroundPlane.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Invisible pero con colisión
        }
        
        Debug.Log("Plano de suelo creado en: " + center);
    }

    Vector3 GetRandomSpawnPosition(Vector3 center)
    {
        // Spawn en círculo entre minSpawnDistance y maxSpawnDistance
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            spawnHeight,
            Mathf.Sin(angle) * distance
        );
        
        return center + offset;
    }

    public void RemoveSmurf(GameObject smurf)
    {
        if (activeSmurfs.Contains(smurf))
        {
            activeSmurfs.Remove(smurf);
        }
    }

    public void ClearAllSmurfs()
    {
        foreach (GameObject smurf in activeSmurfs)
        {
            if (smurf != null)
            {
                Destroy(smurf);
            }
        }
        activeSmurfs.Clear();
        
        if (currentGroundPlane != null)
        {
            Destroy(currentGroundPlane);
        }
        
        Debug.Log("Todos los pitufos eliminados");
    }

    public int GetActiveSmurfCount()
    {
        // Limpiar referencias nulas
        activeSmurfs.RemoveAll(smurf => smurf == null);
        return activeSmurfs.Count;
    }

    void OnDestroy()
    {
        if (planeManager != null)
        {
            planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }
    }
}
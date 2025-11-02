using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    // this managment script spawns things and connects the buttons to relevant functions
    
    //public GameObject prefab;
    
    public float spacing = 2f; 
    public int gridSize = 5;   
    public Transform spawnOrigin;
    public List<GameObject> prefabs;
    
    [Header("Debug Visualization")]
    public bool showAntenna = true;
    public bool showSeparation = true;
    public bool showAlign = true;
    public bool showCohesion = true;

    
    void Start()
    {
        SpawnInGrid();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PathFollowerManager.RequestNewPathForAll();
        }
    }

    public void RandomSpot()
    {
        PathFollowerManager.RequestNewPathForAll();
    }

    public void GoToZones()
    {
        PathFollowerManager.RequestPathToRedZoneForAll();
    }

    public void RecalculatePath()
    {
        PathFollowerManager.RecalculatePathForAll();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateDebugVisibility();
        }
    }

    private void UpdateDebugVisibility()
    {
        // update Avoid (antenna)
        Avoid[] allAvoidScripts = FindObjectsByType<Avoid>(FindObjectsSortMode.None);
        foreach (Avoid avoid in allAvoidScripts)
        {
            avoid.showDebugRay = showAntenna;
        }

        // update Separation
        Separation[] allSeparationScripts = FindObjectsByType<Separation>(FindObjectsSortMode.None);
        foreach (Separation separation in allSeparationScripts)
        {
            separation.showDebugRay = showSeparation;
        }

        // update Align
        Align[] allAlignScripts = FindObjectsByType<Align>(FindObjectsSortMode.None);
        foreach (Align align in allAlignScripts)
        {
            align.showDebugRay = showAlign;
        }

        // update Cohesion
        Cohesion[] allCohesionScripts = FindObjectsByType<Cohesion>(FindObjectsSortMode.None);
        foreach (Cohesion cohesion in allCohesionScripts)
        {
            cohesion.showDebugRay = showCohesion;
        }
    }
    
    void SpawnInGrid()
    {
        Vector3 startPos = spawnOrigin != null ? spawnOrigin.position : transform.position;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = startPos + new Vector3(x * spacing, 0, z * spacing);
                GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Count)];
                
                // the only time I haven't used rb to rotate the object 
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                
                Instantiate(selectedPrefab, position, randomRotation);
            }
        }

        // fix debug visibility after spawning
        UpdateDebugVisibility();
    }
}
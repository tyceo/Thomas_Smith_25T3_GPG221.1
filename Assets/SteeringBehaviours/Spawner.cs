using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    //public GameObject prefab;
    
    public float spacing = 2f; 
    public int gridSize = 5;   
    public Transform spawnOrigin;
    public List<GameObject> prefabs;

    
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
    }


}
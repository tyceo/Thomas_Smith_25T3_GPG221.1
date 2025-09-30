using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public int amount;
    public float spacing = 2f; 
    public int gridSize = 5;   
    public Transform spawnOrigin;

    
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            //Instantiate(prefab, transform.position, Quaternion.identity);
            SpawnInGrid();
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    void SpawnInGrid()
    {
        Vector3 startPos = spawnOrigin != null ? spawnOrigin.position : transform.position;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = startPos + new Vector3(x * spacing, 0, z * spacing);
                Instantiate(prefab, position, Quaternion.identity);
            }
        }

    }
}
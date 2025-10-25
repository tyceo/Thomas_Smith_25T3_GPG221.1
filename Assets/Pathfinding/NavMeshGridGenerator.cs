using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NavMeshGridGenerator : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;
    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(50, 50);
    [SerializeField] private float navMeshSampleDistance = 0.5f;
    [SerializeField] private bool visualizeInEditor = true;
    [SerializeField] private bool excludeEdgeBoxes = true;
    [SerializeField] private bool showBoxes = true; // Toggle box visibility
    
    private List<GameObject> spawnedBoxes = new List<GameObject>();
    private List<Vector3> validPositions = new List<Vector3>();
    private bool[,] gridOccupancy; // Track which grid cells are valid
    
    public static NavMeshGridGenerator Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        GenerateGridFromNavMesh();
    }
    
    [ContextMenu("Generate Grid")]
    public void GenerateGridFromNavMesh()
    {
        ClearGrid();
        
        // Initialize the occupancy grid
        gridOccupancy = new bool[gridDimensions.x, gridDimensions.y];
        
        // First pass: Mark all valid NavMesh positions
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int z = 0; z < gridDimensions.y; z++)
            {
                Vector3 worldPosition = gridOrigin + new Vector3(x * cellSize, 0, z * cellSize);
                
                if (IsPositionOnNavMesh(worldPosition))
                {
                    gridOccupancy[x, z] = true;
                }
            }
        }
        
        // Second pass: Spawn boxes only at interior positions (if excludeEdgeBoxes is enabled)
        int validCount = 0;
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int z = 0; z < gridDimensions.y; z++)
            {
                if (gridOccupancy[x, z])
                {
                    // Check if this position should be included
                    if (!excludeEdgeBoxes || HasAllNeighbors(x, z))
                    {
                        Vector3 worldPosition = gridOrigin + new Vector3(x * cellSize, 0, z * cellSize);
                        SpawnBoxAtPosition(worldPosition);
                        validPositions.Add(worldPosition);
                        validCount++;
                    }
                }
            }
        }
        
        Debug.Log($"Generated {validCount} valid grid positions out of {gridDimensions.x * gridDimensions.y} total cells");
    }
    
    // Check if a grid cell has valid neighbors on all 4 sides
    private bool HasAllNeighbors(int x, int z)
    {
        // Check bounds first
        if (x <= 0 || x >= gridDimensions.x - 1 || z <= 0 || z >= gridDimensions.y - 1)
            return false;
        
        // Check all 4 cardinal directions
        bool hasLeft = gridOccupancy[x - 1, z];
        bool hasRight = gridOccupancy[x + 1, z];
        bool hasDown = gridOccupancy[x, z - 1];
        bool hasUp = gridOccupancy[x, z + 1];
        
        return hasLeft && hasRight && hasDown && hasUp;
    }
    
    // Alternative: Check all 8 directions (including diagonals)
    private bool HasAllNeighbors8Way(int x, int z)
    {
        // Check bounds first
        if (x <= 0 || x >= gridDimensions.x - 1 || z <= 0 || z >= gridDimensions.y - 1)
            return false;
        
        // Check all 8 directions
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue; // Skip center
                
                if (!gridOccupancy[x + dx, z + dz])
                    return false;
            }
        }
        
        return true;
    }
    
    private bool IsPositionOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            float distance = Vector3.Distance(position, hit.position);
            return distance < cellSize * 0.5f;
        }
        
        return false;
    }
    
    private void SpawnBoxAtPosition(Vector3 position)
    {
        if (boxPrefab != null)
        {
            GameObject box = Instantiate(boxPrefab, position, Quaternion.identity, transform);
            box.SetActive(showBoxes);
            spawnedBoxes.Add(box);
        }
    }
    
    // Toggle box visibility
    public void SetBoxVisibility(bool visible)
    {
        showBoxes = visible;
        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null)
            {
                box.SetActive(visible);
            }
        }
    }
    
    // Called when inspector value changes
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetBoxVisibility(showBoxes);
        }
    }
    
    public Vector3 GetRandomValidPosition()
    {
        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions available!");
            return Vector3.zero;
        }
        
        return validPositions[Random.Range(0, validPositions.Count)];
    }
    
    public Vector3 GetRandomValidPositionNear(Vector3 center, float maxDistance)
    {
        List<Vector3> nearbyPositions = new List<Vector3>();
        
        foreach (Vector3 pos in validPositions)
        {
            if (Vector3.Distance(center, pos) <= maxDistance)
            {
                nearbyPositions.Add(pos);
            }
        }
        
        if (nearbyPositions.Count == 0)
        {
            return GetRandomValidPosition();
        }
        
        return nearbyPositions[Random.Range(0, nearbyPositions.Count)];
    }
    
    // Get a random valid position inside a specific zone
    public Vector3 GetRandomValidPositionInZone(string zoneName)
    {
        GameObject zone = GameObject.Find(zoneName);
        if (zone == null)
        {
            Debug.LogWarning($"Zone '{zoneName}' not found!");
            return Vector3.zero;
        }
        
        Collider zoneCollider = zone.GetComponent<Collider>();
        if (zoneCollider == null)
        {
            Debug.LogWarning($"Zone '{zoneName}' has no collider!");
            return Vector3.zero;
        }
        
        List<Vector3> positionsInZone = new List<Vector3>();
        
        // Find all valid positions inside the zone
        foreach (Vector3 pos in validPositions)
        {
            if (zoneCollider.bounds.Contains(pos))
            {
                positionsInZone.Add(pos);
            }
        }
        
        if (positionsInZone.Count == 0)
        {
            Debug.LogWarning($"No valid positions found in zone '{zoneName}'!");
            return Vector3.zero;
        }
        
        return positionsInZone[Random.Range(0, positionsInZone.Count)];
    }
    
    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null)
            {
                DestroyImmediate(box);
            }
        }
        
        spawnedBoxes.Clear();
        validPositions.Clear();
    }
    
    private void OnDrawGizmos()
    {
        if (!visualizeInEditor) return;
        
        Gizmos.color = Color.green;
        foreach (Vector3 pos in validPositions)
        {
            Gizmos.DrawWireCube(pos, Vector3.one * cellSize * 0.8f);
        }
    }
}
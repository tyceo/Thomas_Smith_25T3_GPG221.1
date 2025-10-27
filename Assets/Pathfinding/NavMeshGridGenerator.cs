using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NavMeshGridGenerator : MonoBehaviour
{
    // grid settings that control how the system generates and displays boxes
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;
    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(50, 50);
    [SerializeField] private float navMeshSampleDistance = 0.5f;
    [SerializeField] private bool visualizeInEditor = true;
    [SerializeField] private bool excludeEdgeBoxes = true;
    [SerializeField] private bool showBoxes = true;
    
    // storing all the boxes we spawn and tracking which positions are valid or blocked
    private List<GameObject> spawnedBoxes = new List<GameObject>();
    private List<Vector3> validPositions = new List<Vector3>();
    private HashSet<Vector3> blockedPositions = new HashSet<Vector3>();
    private bool[,] gridOccupancy;
    
    public static NavMeshGridGenerator Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        GenerateGridFromNavMesh();
    }
    
    // creates the entire grid by checking the navmesh and spawning boxes at valid spots
    [ContextMenu("Generate Grid")]
    public void GenerateGridFromNavMesh()
    {
        ClearGrid();
        
        gridOccupancy = new bool[gridDimensions.x, gridDimensions.y];
        
        // first we figure out which spots are on the navmesh
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
        
        // then we spawn boxes but only at interior positions if that setting is on
        int validCount = 0;
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int z = 0; z < gridDimensions.y; z++)
            {
                if (gridOccupancy[x, z])
                {
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
    
    // checking if a grid spot has all its neighbors so we can filter out edge boxes
    private bool HasAllNeighbors(int x, int z)
    {
        if (x <= 0 || x >= gridDimensions.x - 1 || z <= 0 || z >= gridDimensions.y - 1)
            return false;
        
        bool hasLeft = gridOccupancy[x - 1, z];
        bool hasRight = gridOccupancy[x + 1, z];
        bool hasDown = gridOccupancy[x, z - 1];
        bool hasUp = gridOccupancy[x, z + 1];
        
        return hasLeft && hasRight && hasDown && hasUp;
    }
    
    private bool HasAllNeighbors8Way(int x, int z)
    {
        if (x <= 0 || x >= gridDimensions.x - 1 || z <= 0 || z >= gridDimensions.y - 1)
            return false;
        
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;
                
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
            
            MeshRenderer renderer = box.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = showBoxes;
            }
            
            spawnedBoxes.Add(box);
        }
    }
    
    // managing blocked positions so moving walls can tell us when spots become unavailable
    public void InvalidatePosition(Vector3 position)
    {
        blockedPositions.Add(position);
    }
    
    public void RevalidatePosition(Vector3 position)
    {
        blockedPositions.Remove(position);
    }
    
    private bool IsPositionAvailable(Vector3 position)
    {
        return !blockedPositions.Contains(position);
    }
    
    // checking if a position is blocked by comparing distances to available vs blocked spots
    public bool IsPositionBlocked(Vector3 position)
    {
        float closestAvailableDistance = float.MaxValue;
        float closestBlockedDistance = float.MaxValue;
        
        foreach (Vector3 validPos in validPositions)
        {
            if (IsPositionAvailable(validPos))
            {
                float distance = Vector3.Distance(position, validPos);
                if (distance < closestAvailableDistance)
                {
                    closestAvailableDistance = distance;
                }
            }
        }
        
        foreach (Vector3 blockedPos in blockedPositions)
        {
            float distance = Vector3.Distance(position, blockedPos);
            if (distance < closestBlockedDistance)
            {
                closestBlockedDistance = distance;
            }
        }
        
        if (blockedPositions.Count == 0)
        {
            return false;
        }
        
        return closestBlockedDistance < closestAvailableDistance;
    }
    
    // showing or hiding the grid boxes in the scene
    public void SetBoxVisibility(bool visible)
    {
        showBoxes = visible;
        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null)
            {
                MeshRenderer renderer = box.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = visible;
                }
            }
        }
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetBoxVisibility(showBoxes);
        }
    }
    
    // getting random positions from the grid while avoiding blocked spots
    public Vector3 GetRandomValidPosition()
    {
        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions available!");
            return Vector3.zero;
        }
        
        List<Vector3> availablePositions = new List<Vector3>();
        foreach (Vector3 pos in validPositions)
        {
            if (IsPositionAvailable(pos))
            {
                availablePositions.Add(pos);
            }
        }
        
        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("All positions are currently blocked!");
            return Vector3.zero;
        }
        
        return availablePositions[Random.Range(0, availablePositions.Count)];
    }
    
    public Vector3 GetRandomValidPositionNear(Vector3 center, float maxDistance)
    {
        List<Vector3> nearbyPositions = new List<Vector3>();
        
        foreach (Vector3 pos in validPositions)
        {
            if (Vector3.Distance(center, pos) <= maxDistance && IsPositionAvailable(pos))
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
    
    // finding random spots inside specific zones
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
        
        foreach (Vector3 pos in validPositions)
        {
            if (zoneCollider.bounds.Contains(pos) && IsPositionAvailable(pos))
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
    
    // drawing the grid in the editor so you can see whats available and whats blocked
    private void OnDrawGizmos()
    {
        if (!visualizeInEditor) return;
        
        foreach (Vector3 pos in validPositions)
        {
            if (blockedPositions.Contains(pos))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawWireCube(pos, Vector3.one * cellSize * 0.8f);
        }
    }
}
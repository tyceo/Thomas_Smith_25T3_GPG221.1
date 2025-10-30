using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    // variables for path management
    private NavMeshPath path;
    private Vector3 lastPoint;
    private int currentPathIndex = 0;
    private bool hasPath = false;
    private bool needsNewPath = true;
    private Vector3 currentTarget; // store current destination

    [SerializeField] private float pointReachedThreshold = 0.5f;
    [SerializeField] private float randomPointRange = 50f; 
    [SerializeField] private float minDistanceFromGround = 5f; // minimum distance from ground layer objects
    [SerializeField] private int maxAttempts = 30; // maximum attempts to find a valid point
    [SerializeField] private GameObject targetMarkerPrefab; // prefab to show at target
    private TurnTowards turnTowards;
    private GameObject targetMarkerInstance; // current spawned marker
    
    // variables for movement
    public bool IsAtEndOfPath { get; private set; }
    public bool EnableTurning = true;

    
    private void Awake()
    {
        path = new NavMeshPath();
        turnTowards = GetComponent<TurnTowards>();
    }

    private void OnEnable()
    {
        PathFollowerManager.RegisterPathFollower(this);
    }

    private void OnDisable()
    {
        PathFollowerManager.UnregisterPathFollower(this);
    }

    
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            RequestPathToRedZone();
        }
        */
        
        if (needsNewPath)
        {
            Vector3 randomTarget = GetRandomNavMeshPoint(transform.position, randomPointRange);
            if (randomTarget != Vector3.zero)
            {
                CalculatePath(randomTarget);
                needsNewPath = false;
            }
        }

        if (hasPath)
        {
            FollowPath();
            DrawPath();
        }
    }

    // pathfinding, chooses a random grid point
    private Vector3 GetRandomNavMeshPoint(Vector3 center, float range)
    {
        // use the grid system if available
        if (NavMeshGridGenerator.Instance != null)
        {
            return NavMeshGridGenerator.Instance.GetRandomValidPositionNear(center, range);
        }
        
        // old way using navmesh
        int groundLayer = LayerMask.GetMask("Ground");
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // generate a random point within the range
            Vector3 randomDirection = Random.insideUnitSphere * range;
            randomDirection += center;

            NavMeshHit hit;
            // sample the navmesh to find the nearest valid point
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                // check if this point is far enough from ground layer objects
                if (IsPointFarFromGround(hit.position, minDistanceFromGround, groundLayer))
                {
                    return hit.position;
                }
            }
        }

        return Vector3.zero; // return zero if no valid point found after all attempts
    }

    // pathfinding, check if a point is at least mindistance away from any ground layer object
    private bool IsPointFarFromGround(Vector3 point, float minDistance, int groundLayerMask)
    {
        
        Collider[] colliders = Physics.OverlapSphere(point, minDistance, groundLayerMask);
        
        return colliders.Length == 0;
    }

    // pathfinding, path calculation
    public void CalculatePath(Vector3 target)
    {
        currentTarget = target; // store the target
        Vector3 flatStart = FlattenVector(transform.position);
        Vector3 flatTarget = FlattenVector(target);
        if (NavMesh.CalculatePath(flatStart, flatTarget, NavMesh.AllAreas, path))
        {
            hasPath = true;
            currentPathIndex = 0;
            
            // Tell TurnTowards where the final destination is
            if (turnTowards != null && path.corners.Length > 0)
            {
                turnTowards.SetFinalDestination(path.corners[path.corners.Length - 1]);
            }
            
            // spawn target marker
            SpawnTargetMarker();
        }
    }
    
    // pathfinding, recalculate path to current target
    public void RecalculatePath()
    {
        if (currentTarget != Vector3.zero)
        {
            if (hasPath)
            {
                CalculatePath(currentTarget);
            }
            
        }
    }
    
    // spawn target marker
    private void SpawnTargetMarker()
    {
        if (targetMarkerPrefab != null)
        {
            // remove old marker if it exists
            RemoveTargetMarker();
            
            // spawn new marker at target
            targetMarkerInstance = Instantiate(targetMarkerPrefab, currentTarget, Quaternion.identity);
        }
    }
    
    // remove target marker
    private void RemoveTargetMarker()
    {
        if (targetMarkerInstance != null)
        {
            Destroy(targetMarkerInstance);
            targetMarkerInstance = null;
        }
    }
    
    /*
    // pathfinding - check if any point in the path is blocked
    private bool IsPathBlocked(NavMeshPath path)
    {
        if (NavMeshGridGenerator.Instance == null)
            return false;
        
        foreach (Vector3 corner in path.corners)
        {
            if (NavMeshGridGenerator.Instance.IsPositionBlocked(corner))
            {
                return true;
            }
        }
        
        return false;
    }
*/
    // movement, path following logic
    private void FollowPath()
    {
        if (!hasPath || path.corners.Length == 0 || currentPathIndex >= path.corners.Length)
        {
            IsAtEndOfPath = true;
            turnTowards.StopTurning();
            RemoveTargetMarker(); // remove marker when path ends
            return;
        }

        IsAtEndOfPath = false;
        
        if (EnableTurning)
        {
            turnTowards.StartTurning();
        }
        else
        {
            turnTowards.StopTurning();
        }

        Vector3 currentTarget = FlattenVector(path.corners[currentPathIndex]);
        Vector3 currentPosition = FlattenVector(transform.position);

        turnTowards.SetTarget(currentTarget);

        float distanceToTarget = Vector3.Distance(currentPosition, currentTarget);
        if (distanceToTarget <= pointReachedThreshold)
        {
            currentPathIndex++;
            
            if (currentPathIndex >= path.corners.Length)
            {
                hasPath = false;
                IsAtEndOfPath = true;
                turnTowards.StopTurning();
                RemoveTargetMarker(); // remove marker when reaching end
            }
        }
    }

    // showing the path of each npc
    private void DrawPath()
    {
        if (path.corners.Length < 2)
            return;

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 flatCorner = FlattenVector(path.corners[i]);
            
            // determine the start point for this segment
            Vector3 startPoint;
            if (i == 0)
            {
                
                startPoint = FlattenVector(transform.position);
            }
            else
            {
                
                startPoint = FlattenVector(path.corners[i - 1]);
            }
            
            // only draw segments that haven't been completed yet
            if (i >= currentPathIndex)
            {
                if (i == currentPathIndex)
                {
                    
                    Debug.DrawLine(FlattenVector(transform.position), flatCorner, Color.yellow);
                }
                else
                {
                    // future segments
                    Debug.DrawLine(startPoint, flatCorner, Color.green);
                }
            }
        }
    }

    // fixing y pos
    private Vector3 FlattenVector(Vector3 vector)
    {
        return new Vector3(vector.x, transform.position.y, vector.z);
    }

    
    public void RequestNewPath()
    {
        needsNewPath = true;
        IsAtEndOfPath = false;
    }
    
    // request a path to a random position in the redzone
    public void RequestPathToRedZone()
    {
        if (NavMeshGridGenerator.Instance != null)
        {
            
            if (gameObject.name.Contains("RedNPC"))
            {
                Vector3 redZoneTarget = NavMeshGridGenerator.Instance.GetRandomValidPositionInZone("RedZone");
                            if (redZoneTarget != Vector3.zero)
                            {
                                CalculatePath(redZoneTarget);
                                needsNewPath = false;
                            }
                            else
                            {
                                Debug.LogWarning("Could not find valid position in RedZone!");
                            }
            }

            if (gameObject.name.Contains("BlueNPC"))
            {
                Vector3 blueZoneTarget = NavMeshGridGenerator.Instance.GetRandomValidPositionInZone("BlueZone");
                            if (blueZoneTarget != Vector3.zero)
                            {
                                CalculatePath(blueZoneTarget);
                                needsNewPath = false;
                            }
                            else
                            {
                                Debug.LogWarning("Could not find valid position in BlueZone!");
                            }
            }

            if (gameObject.name.Contains("GreenNPC"))
            {
                Vector3 greenZoneTarget = NavMeshGridGenerator.Instance.GetRandomValidPositionInZone("GreenZone");
                            if (greenZoneTarget != Vector3.zero)
                            {
                                CalculatePath(greenZoneTarget);
                                needsNewPath = false;
                            }
                            else
                            {
                                Debug.LogWarning("Could not find valid position in GreenZone!");
                            }
            }

            if (gameObject.name.Contains("PinkNPC"))
            {
                Vector3 pinkZoneTarget = NavMeshGridGenerator.Instance.GetRandomValidPositionInZone("PinkZone");
                            if (pinkZoneTarget != Vector3.zero)
                            {
                                CalculatePath(pinkZoneTarget);
                                needsNewPath = false;
                            }
                            else
                            {
                                Debug.LogWarning("Could not find valid position in PinkZone!");
                            }
            }
        }
    }

    
    public void ResetPath()
    {
        hasPath = false;
        currentPathIndex = 0;
        IsAtEndOfPath = false;
        needsNewPath = true;
        RemoveTargetMarker(); // remove marker when resetting
        
        if (EnableTurning)
        {
            turnTowards.StartTurning();
        }
    }
    
    private void OnDestroy()
    {
        RemoveTargetMarker(); // cleanup on destroy
    }
}
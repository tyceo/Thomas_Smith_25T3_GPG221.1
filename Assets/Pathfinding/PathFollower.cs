using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    // variables - path management
    private NavMeshPath path;
    private Vector3 lastPoint;
    private int currentPathIndex = 0;
    private bool hasPath = false;
    private bool needsNewPath = true;

    [SerializeField] private float pointReachedThreshold = 0.5f;
    [SerializeField] private float randomPointRange = 50f; // how far from current position to search for random points
    [SerializeField] private float minDistanceFromGround = 5f; // minimum distance from ground layer objects
    [SerializeField] private int maxAttempts = 30; // maximum attempts to find a valid point
    private TurnTowards turnTowards;
    
    // variables - state tracking
    public bool IsAtEndOfPath { get; private set; }
    public bool EnableTurning = true;

    // unity callbacks - initialization
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

    // unity callbacks - per frame updates
    private void Update()
    {
        // check for redzone target request
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            RequestPathToRedZone();
        }
        
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

    // pathfinding - get a random valid point on the navmesh
    private Vector3 GetRandomNavMeshPoint(Vector3 center, float range)
    {
        // use the grid system if available
        if (NavMeshGridGenerator.Instance != null)
        {
            return NavMeshGridGenerator.Instance.GetRandomValidPositionNear(center, range);
        }
        
        // fallback to original method
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

    // pathfinding - check if a point is at least mindistance away from any ground layer object
    private bool IsPointFarFromGround(Vector3 point, float minDistance, int groundLayerMask)
    {
        // use overlapsphere to check for any ground layer colliders within the minimum distance
        Collider[] colliders = Physics.OverlapSphere(point, minDistance, groundLayerMask);
        
        // if no colliders found, the point is valid (far enough from ground objects)
        return colliders.Length == 0;
    }

    // pathfinding - path calculation
    public void CalculatePath(Vector3 target)
    {
        Vector3 flatStart = FlattenVector(transform.position);
        Vector3 flatTarget = FlattenVector(target);
        if (NavMesh.CalculatePath(flatStart, flatTarget, NavMesh.AllAreas, path))
        {
            hasPath = true;
            currentPathIndex = 0;
        }
    }

    // movement - path following logic
    private void FollowPath()
    {
        if (!hasPath || path.corners.Length == 0 || currentPathIndex >= path.corners.Length)
        {
            IsAtEndOfPath = true;
            turnTowards.StopTurning();
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
            }
        }
    }

    // debug - visualization
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
                // first segment starts from agent's position
                startPoint = FlattenVector(transform.position);
            }
            else
            {
                // other segments start from previous corner
                startPoint = FlattenVector(path.corners[i - 1]);
            }
            
            // only draw segments that haven't been completed yet
            if (i >= currentPathIndex)
            {
                if (i == currentPathIndex)
                {
                    // current segment (from agent to next waypoint)
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

    // utility - fixing variables
    private Vector3 FlattenVector(Vector3 vector)
    {
        return new Vector3(vector.x, transform.position.y, vector.z);
    }

    // public api - request new path
    public void RequestNewPath()
    {
        needsNewPath = true;
        IsAtEndOfPath = false;
    }
    
    // public api - request a path to a random position in the redzone
    public void RequestPathToRedZone()
    {
        if (NavMeshGridGenerator.Instance != null)
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
    }

    // public api - reset path
    public void ResetPath()
    {
        hasPath = false;
        currentPathIndex = 0;
        IsAtEndOfPath = false;
        needsNewPath = true;
        
        if (EnableTurning)
        {
            turnTowards.StartTurning();
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    // variables for path management
    private NavMeshPath path;
    public Transform targetPoint;
    private Vector3 lastPoint;
    private int currentPathIndex = 0;
    private bool hasPath = false;
    private bool needsNewPath = true;

    [SerializeField] private float pointReachedThreshold = 0.5f;
    private TurnTowards turnTowards;
    
    // state tracking
    public bool IsAtEndOfPath { get; private set; }
    public bool EnableTurning = true;

    // initialization
    private void Awake()
    {
        path = new NavMeshPath();
        turnTowards = GetComponent<TurnTowards>();
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestNewPath();
        }

        if (targetPoint != null && needsNewPath)
        {
            Vector3 flatTarget = FlattenVector(targetPoint.position);
            CalculatePath(flatTarget);
            needsNewPath = false;
        }

        if (hasPath)
        {
            FollowPath();
            DrawPath();
        }
    }

    // path calculation
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

    // path following logic
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

    // debug visualization
    private void DrawPath()
    {
        if (path.corners.Length < 2)
            return;

        lastPoint = FlattenVector(transform.position);

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 flatCorner = FlattenVector(path.corners[i]);
            
            if (i == currentPathIndex)
            {
                Debug.DrawLine(lastPoint, flatCorner, Color.yellow);
            }
            else
            {
                Debug.DrawLine(lastPoint, flatCorner, Color.green);
            }
            
            lastPoint = flatCorner;
        }
    }

    // fixing variables
    private Vector3 FlattenVector(Vector3 vector)
    {
        return new Vector3(vector.x, transform.position.y, vector.z);
    }

    public void RequestNewPath()
    {
        needsNewPath = true;
        IsAtEndOfPath = false;
    }

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
using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    // configuration
    [SerializeField] private float rotationSpeed = 180f;

    // state tracking
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private bool shouldRotate = true;

    // public control methods
    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void StopTurning()
    {
        shouldRotate = false;
    }

    public void StartTurning()
    {
        shouldRotate = true;
    }

    
    private void Update()
    {
        if (!hasTarget || !shouldRotate) return;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0;
        
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // utility stuff
    public bool IsTargetReached()
    {
        if (!hasTarget) return false;
        
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToTarget);
        return dot >= 0.99f;
    }

    public bool IsRotating => shouldRotate;
}
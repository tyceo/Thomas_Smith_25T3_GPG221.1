using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    // configuration
    [SerializeField] private float baseRotationSpeed = 100f;
    [SerializeField] private float maxRotationSpeed = 200f;
    [SerializeField] private float speedIncreaseDistance = 10f; // distance at which speed starts increasing

    // state tracking
    private Vector3 targetPosition;
    private Vector3 finalDestination; // last point in path
    private bool hasTarget = false;
    private bool shouldRotate = true;
    private bool hasFinalDestination = false;

    // public control methods
    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void SetFinalDestination(Vector3 position)
    {
        finalDestination = position;
        hasFinalDestination = true;
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
            
            // Calculate rotation speed based on distance to final destination
            float currentRotationSpeed = baseRotationSpeed;
            
            if (hasFinalDestination)
            {
                float distanceToFinalDestination = Vector3.Distance(transform.position, finalDestination);
                
                // Increase speed as we get closer to final destination
                if (distanceToFinalDestination < speedIncreaseDistance)
                {
                    float t = 1f - (distanceToFinalDestination / speedIncreaseDistance);
                    currentRotationSpeed = Mathf.Lerp(baseRotationSpeed, maxRotationSpeed, t);
                }
            }
            
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                currentRotationSpeed * Time.deltaTime
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
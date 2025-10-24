using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    public float turnSpeed = 2;
    public Transform targetObjectTransform;
    public Vector3 targetPosition;
    public Rigidbody rb;
    
    void Start()
    {
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 targetDir;
        if (targetObjectTransform != null)
        {
            // Has a target gameobject
            targetDir = (targetObjectTransform.position - transform.position).normalized;
        }
        else
        {
            // Just a raw position in the world (for pathfinding points)
            targetDir = (targetPosition - transform.position).normalized;
        }

        // calculate the signed angle between our forward direction and target direction
        float angle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
        
        
        rb.AddRelativeTorque(0f, angle * turnSpeed, 0f);
    }

    
}
using UnityEngine;

public class Avoid : MonoBehaviour
{
    public float rayDistance = 5f;
    public bool showDebugRay = true;
    public float turnSpeed = 50f;

    public LayerMask targetLayers;
    public Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()  
    {
        if (rb == null) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, targetLayers, QueryTriggerInteraction.Ignore))
        {
            
            Vector3 hitDirection = hit.point - transform.position;
            float dotProduct = Vector3.Dot(transform.right, hitDirection);
            
            
            float turnDirection = -Mathf.Sign(dotProduct); // Negative to turn away from thing infront
            
            
            rb.AddRelativeTorque(0, turnDirection * turnSpeed, 0);
            
            Debug.Log($"Turning {(turnDirection > 0 ? "left" : "right")} to avoid {hit.collider.gameObject.name}");
        }
    }

    void OnDrawGizmos()
    {
        if (showDebugRay)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
        }
    }

}

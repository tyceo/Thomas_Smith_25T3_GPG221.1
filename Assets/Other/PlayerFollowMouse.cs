using UnityEngine;

public class PlayerFollowMouse : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private LayerMask groundLayer; // Optional: specify which layer to raycast against
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("PlayerFollowMouse requires a Rigidbody component!");
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Create a ray from camera through mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Raycast to find where the mouse is pointing in the world
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // Get the position on the ground
            Vector3 targetPosition = hit.point;
            
            // Keep the same Y position as the GameObject
            targetPosition.y = transform.position.y;
            
            // Calculate direction towards mouse (only X and Z)
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;
            
            // Rotate towards the mouse
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
            
            // Apply force to move towards mouse
            rb.AddForce(direction * moveSpeed, ForceMode.Force);
            
            // Clamp velocity to max speed
            Vector3 velocity = rb.linearVelocity;
            if (velocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = velocity.normalized * maxSpeed;
            }
        }
    }
}

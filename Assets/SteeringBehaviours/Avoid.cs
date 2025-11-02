using UnityEngine;

public class Avoid : MonoBehaviour
{
    public float rayDistance = 5f;
    public bool showDebugRay = true;
    public float turnSpeed = 50f;
    private float currentRayDistance;

    public LayerMask targetLayers;
    public Rigidbody rb;
    private MoveForward moveForward;
    
    private bool isLeftObject;

    public string whatLeftIsHitting;
    public string whatRightIsHitting;
    
    public bool isHitting;

    public int forwardLeftOrRight = 1;
    
    

    void Start()
    {
        if (gameObject.name == "Forward")
        {
            //forwardLeftOrRight = Random.Range(1, 3);
            //forwardLeftOrRight = Random.Range(0, 2) == 0 ? -1 : 1;         //randomly choose if Object spins left or right. this is to try and make objects move better
            //turnSpeed = turnSpeed * forwardLeftOrRight;
        }
        rb = GetComponentInParent<Rigidbody>();
        moveForward = GetComponentInParent<MoveForward>();
        isLeftObject = gameObject.name == "Left";
        currentRayDistance = rayDistance;
    }
    
    // shoots a raycast forward and rotates the npc away from the obstacle 
    void FixedUpdate()  
    {
        if (rb == null) return;

        float speedBonus = Mathf.Min(rb.linearVelocity.magnitude / 5f * 3f, 3f);
        currentRayDistance = rayDistance + speedBonus;

        RaycastHit hit;
        isHitting = Physics.Raycast(transform.position, transform.forward, out hit, currentRayDistance, targetLayers, QueryTriggerInteraction.Ignore);
        
        
        // checks to see if the npc should rotate left or right and it rotates faster the closer it gets to the obstacle
        if (isHitting)
        {
            // reset speed when obstacle detected
            if (moveForward != null)
            {
                moveForward.ResetSpeed();
            }

            const float TURN_LEFT = -1f;
            const float TURN_RIGHT = 1f;
            
            float turnDirection;
            
            if (isLeftObject)
            {
                turnDirection = TURN_LEFT;
                whatLeftIsHitting = hit.collider.gameObject.layer.ToString();
            }
            else
            {
                turnDirection = TURN_RIGHT; 
                whatRightIsHitting = hit.collider.gameObject.layer.ToString();
            }
            float distanceBasedTurnSpeed = turnSpeed / hit.distance;
            Vector3 torqueForce = new Vector3(0, turnDirection * distanceBasedTurnSpeed, 0);
            rb.AddRelativeTorque(torqueForce);
        }
    }

    void OnDrawGizmos()
    {
        if (showDebugRay)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * (Application.isPlaying ? currentRayDistance : rayDistance));
        }
    }
}
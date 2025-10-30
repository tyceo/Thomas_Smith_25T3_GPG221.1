using UnityEngine;

public class ZoneConstraintManager : MonoBehaviour
{
    private PathFollower pathFollower;
    private Rigidbody rb;
    private bool isInZone = false;

    void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        rb = GetComponent<Rigidbody>();
        
        // default constraints
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        
    }

    void Update()
    {
        if (pathFollower != null && rb != null)
        {
            if (pathFollower.IsAtEndOfPath && isInZone)
            {
                
                rb.constraints = RigidbodyConstraints.FreezePositionX | 
                                 RigidbodyConstraints.FreezePositionY | 
                                 RigidbodyConstraints.FreezePositionZ |
                                 RigidbodyConstraints.FreezeRotationX |
                                 RigidbodyConstraints.FreezeRotationZ;
            }
            else
            {
                
                rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                                 RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name.Contains("ZoneZ") )
        {
            isInZone = true;
            //Debug.Log(other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.name.Contains("ZoneZ"))
        {
            isInZone = false;
        }
    }
}
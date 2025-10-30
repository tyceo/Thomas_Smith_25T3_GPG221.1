using System.Collections.Generic;
using UnityEngine;

public class Align : MonoBehaviour
{
    public Detector detector;
    public Rigidbody rb;
    public float force = 100f;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        if (detector == null)
        {
            detector = GetComponentInChildren<Detector>();
        }

        
    }

    void FixedUpdate()
    {
        
        List<GameObject> neighbours = detector.GetObjectsInTrigger();
        
        // Some are Torque, some are Force		
        Vector3 targetDirection = CalculateMove(neighbours);
		
        // Cross will take YOUR direction and the TARGET direction and turn it into a rotation force vector. It CROSSES through both at 90 degrees
        Vector3 cross = Vector3.Cross(transform.forward, targetDirection);

        // Where I WANT to face
        Debug.DrawRay(transform.position, targetDirection * 10f, Color.blue);
        
        // Where I'm facing right now
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);

        rb.AddTorque(cross * force);
    }

    public Vector3 CalculateMove(List<GameObject> neighbours)
    {
        if (neighbours.Count == 0)
            return Vector3.zero;

        Vector3 alignmentDirection = Vector3.zero;

        // Average of all neighbours directions
        foreach (GameObject item in neighbours)
        {
            alignmentDirection += item.transform.forward;
        }

        alignmentDirection /= neighbours.Count;



        return alignmentDirection;
    }
}


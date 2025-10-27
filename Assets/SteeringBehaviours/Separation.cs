using System.Collections.Generic;
using UnityEngine;

public class Separation : MonoBehaviour
{
    public Detector detector;
    public Rigidbody rb;
    public float speed = 100f;
    public float maxNeighbourDistance = 10f;

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
        
        if (detector != null)
        {
            SphereCollider sphereCollider = detector.GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                maxNeighbourDistance = sphereCollider.radius;
            }
        }
    }

    void FixedUpdate()
    {
        List<GameObject> neighbours = detector.GetObjectsInTrigger();
        
        Vector3 separationForce = CalculateSeparation(neighbours);
        
        // red for separation
        Debug.DrawRay(transform.position, separationForce.normalized * 5f, Color.red);
        
        rb.AddForce(separationForce * speed);
    }

    private Vector3 CalculateSeparation(List<GameObject> neighbours)
    {
        if (neighbours.Count == 0)
            return Vector3.zero;

        Vector3 totalSeparationDirection = Vector3.zero;

        foreach (GameObject neighbour in neighbours)
        {
            // direction aWAY from each neighbour
            Vector3 directionToNeighbour = neighbour.transform.position - transform.position;
            float distance = directionToNeighbour.magnitude;
            
            // negative to move away instead
            Vector3 directionAway = -directionToNeighbour.normalized;
            
            // strength based on distance
            float invertedDistance = maxNeighbourDistance - distance;
            
            //weighted force
            Vector3 weightedDirection = directionAway * invertedDistance;
            
            totalSeparationDirection += weightedDirection;
        }

        // Average of these forces
        totalSeparationDirection /= neighbours.Count;

        return totalSeparationDirection;
    }
}

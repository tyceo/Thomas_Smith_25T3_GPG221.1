using System.Collections.Generic;
using UnityEngine;

public class Cohesion : MonoBehaviour
{
    public Detector detector;
    public Rigidbody rb;
    public float speed = 100f;

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
        
        Vector3 cohesionForce = CalculateCohesion(neighbours);
        
        // cyan for cohesion
        Debug.DrawRay(transform.position, cohesionForce.normalized * 5f, Color.cyan);
        
        rb.AddForce(cohesionForce * speed);
    }

    private Vector3 CalculateCohesion(List<GameObject> neighbours)
    {
        if (neighbours.Count == 0)
            return Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;

        // calculate the average position of all neighbours
        foreach (GameObject neighbour in neighbours)
        {
            centerOfMass += neighbour.transform.position;
        }

        centerOfMass /= neighbours.Count;

        // calculate direction TOWARDS the center of mass
        Vector3 directionToCenterOfMass = (centerOfMass - transform.position).normalized;

        return directionToCenterOfMass;
    }
}

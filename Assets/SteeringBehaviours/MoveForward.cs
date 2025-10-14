using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Rigidbody Rb;
    public float zSpeed = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveObjectForward();
    }

    public void MoveObjectForward()
    {
        Rb.AddRelativeForce(Vector3.forward * zSpeed);

    }
}

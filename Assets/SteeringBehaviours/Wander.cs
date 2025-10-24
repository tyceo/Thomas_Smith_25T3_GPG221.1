using UnityEngine;

public class Wander : MonoBehaviour
{
    private float randomOffset;
    private Rigidbody rb;
    
    public float strength = 10f;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        
        
        randomOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        
        float perlin = Mathf.PerlinNoise(Time.time + randomOffset, 0f);
        
        
        float rotationForce = (perlin - 0.5f) * strength;
        
        
        rb.AddRelativeTorque(0f, rotationForce, 0f);
    }
}
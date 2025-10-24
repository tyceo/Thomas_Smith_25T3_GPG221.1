using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Rigidbody Rb;
    public float zSpeed = 500f;
    public float speedIncreaseRate = 50f; // Speed increase per second
    public float maxSpeed = 2000f; // Maximum speed limit

    private bool isResetting = false;
    private float resetStartSpeed;
    private float resetTimer;
    private const float RESET_DURATION = 1f;

    void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isResetting)
        {
            resetTimer += Time.fixedDeltaTime;
            float t = resetTimer / RESET_DURATION;
            if (t >= 1f)
            {
                zSpeed = 500f;
                isResetting = false;
            }
            else
            {
                zSpeed = Mathf.Lerp(resetStartSpeed, 500f, t);
            }
        }
        else if (zSpeed < maxSpeed)
        {
            zSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        }
        
        MoveObjectForward();
    }

    public void MoveObjectForward()
    {
        Rb.AddRelativeForce(Vector3.forward * zSpeed);
    }

    public void ResetSpeed()
    {
        resetStartSpeed = zSpeed;
        resetTimer = 0f;
        isResetting = true;
    }
}
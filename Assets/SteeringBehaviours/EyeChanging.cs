using UnityEngine;

public class EyeChanging : MonoBehaviour
{
    public Material normalMaterial;
    public Material changeMaterial;
    public float angularVelocityThreshold = 0.1f;
    public float minimumChangeTime = 0.5f; // Minimum time material stays changed in seconds

    private Rigidbody parentRb;
    private MeshRenderer eyeMeshRenderer;      //for Eye 
    private MeshRenderer eyeMeshRenderer2;     //Eye(1) 
    private float previousYRotation;
    
    private float eye1ChangeTimer;    // Timer for Eye
    private float eye2ChangeTimer;    // Timer for Eye (1)
    private bool eye1Changed;         // Track if Eye is in changed state
    private bool eye2Changed;         // Track if Eye (1) is in changed state

    void Start()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        
        Transform eyeTransform = transform.parent.Find("Eye");
        Transform eye2Transform = transform.parent.Find("Eye (1)");
        
        if (eyeTransform != null)
        {
            eyeMeshRenderer = eyeTransform.GetComponent<MeshRenderer>();
        }
        
        if (eye2Transform != null)
        {
            eyeMeshRenderer2 = eye2Transform.GetComponent<MeshRenderer>();
        }

        if (parentRb == null || eyeMeshRenderer == null || eyeMeshRenderer2 == null)
        {
            Debug.LogError("Missing required components!");
            enabled = false;
            return;
        }

        previousYRotation = transform.parent.eulerAngles.y;
        eye1ChangeTimer = 0f;
        eye2ChangeTimer = 0f;
        eye1Changed = false;
        eye2Changed = false;
    }

    void Update()
    {
        float currentYRotation = transform.parent.eulerAngles.y;
        float rotationDelta = Mathf.DeltaAngle(previousYRotation, currentYRotation);
        float deltaTime = Time.deltaTime;

        // Update timers
        if (eye1Changed)
        {
            eye1ChangeTimer -= deltaTime;
            if (eye1ChangeTimer <= 0f)
            {
                eye1Changed = false;
                eyeMeshRenderer.material = normalMaterial;
            }
        }

        if (eye2Changed)
        {
            eye2ChangeTimer -= deltaTime;
            if (eye2ChangeTimer <= 0f)
            {
                eye2Changed = false;
                eyeMeshRenderer2.material = normalMaterial;
            }
        }

        // Check rotation direction
        if (Mathf.Abs(rotationDelta) > angularVelocityThreshold)
        {
            if (rotationDelta > 0) // Rotating up/right
            {
                // Change Eye material and reset timer
                eyeMeshRenderer.material = changeMaterial;
                eye1ChangeTimer = minimumChangeTime;
                eye1Changed = true;

                // Only reset Eye (1) if it's not in its change period
                if (!eye2Changed)
                {
                    eyeMeshRenderer2.material = normalMaterial;
                }
            }
            else // Rotating down/left
            {
                // Change Eye (1) material and reset timer
                eyeMeshRenderer2.material = changeMaterial;
                eye2ChangeTimer = minimumChangeTime;
                eye2Changed = true;

                // Only reset Eye if it's not in its change period
                if (!eye1Changed)
                {
                    eyeMeshRenderer.material = normalMaterial;
                }
            }
        }

        previousYRotation = currentYRotation;
    }

    void OnDisable()
    {
        // Reset both eyes to normal material when script is disabled
        if (eyeMeshRenderer != null)
        {
            eyeMeshRenderer.material = normalMaterial;
        }
        if (eyeMeshRenderer2 != null)
        {
            eyeMeshRenderer2.material = normalMaterial;
        }
        
        // Reset timers and states
        eye1ChangeTimer = 0f;
        eye2ChangeTimer = 0f;
        eye1Changed = false;
        eye2Changed = false;
    }
}
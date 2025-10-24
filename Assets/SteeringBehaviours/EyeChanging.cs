using UnityEngine;

public class EyeChanging : MonoBehaviour
{
    public Material normalMaterial;
    public Material changeMaterial;
    public float angularVelocityThreshold = 0.1f;
    public float minimumChangeTime = 0.5f; // minimum time material stays changed in seconds

    private Rigidbody parentRb;
    private MeshRenderer eyeMeshRenderer;      
    private MeshRenderer eyeMeshRenderer2;     
    private float previousYRotation;
    
    private float eye1ChangeTimer;    
    private float eye2ChangeTimer;    
    private bool eye1Changed;         
    private bool eye2Changed;         

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

        
        if (Mathf.Abs(rotationDelta) > angularVelocityThreshold)
        {
            if (rotationDelta > 0) // rotating right
            {
                
                eyeMeshRenderer.material = changeMaterial;
                eye1ChangeTimer = minimumChangeTime;
                eye1Changed = true;

                
                if (!eye2Changed)
                {
                    eyeMeshRenderer2.material = normalMaterial;
                }
            }
            else 
            {
                
                eyeMeshRenderer2.material = changeMaterial;
                eye2ChangeTimer = minimumChangeTime;
                eye2Changed = true;

                
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
        //  both eyes to normal material when script is disabled
        if (eyeMeshRenderer != null)
        {
            eyeMeshRenderer.material = normalMaterial;
        }
        if (eyeMeshRenderer2 != null)
        {
            eyeMeshRenderer2.material = normalMaterial;
        }
        
        
        eye1ChangeTimer = 0f;
        eye2ChangeTimer = 0f;
        eye1Changed = false;
        eye2Changed = false;
    }
}
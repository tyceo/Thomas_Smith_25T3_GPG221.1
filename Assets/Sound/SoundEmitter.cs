using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    
    
    [SerializeField] private float soundRadius = 10f;
    [SerializeField] private LayerMask hearingLayer = -1; 
    [SerializeField] private bool useRaycastFiltering = true;
    
    
    [SerializeField] private Color gizmoColor = Color.yellow;
    
    
    public void EmitSound()
    {
        //get all colliders within radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius, hearingLayer);
        
        Debug.Log($"{gameObject.name} emitting sound. Found {colliders.Length} potential listeners.");
        
        //only for objects with hearing script
        foreach (Collider col in colliders)
        {
            Hearing hearing = col.GetComponent<Hearing>();
            
            if (hearing != null)
            {
                // couldn't get raycast here working properly without lagging game
                if (useRaycastFiltering)
                {
                    Vector3 directionToListener = (col.transform.position - transform.position).normalized;
                    float distanceToListener = Vector3.Distance(transform.position, col.transform.position);
                    
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToListener, out hit, distanceToListener))
                    {
                        // if something in way it blocks the sound
                        if (hit.collider == col)
                        {
                            hearing.HeardSomething(this);
                            Debug.DrawLine(transform.position, col.transform.position, Color.green, 2f);
                        }
                        else
                        {
                            
                            Debug.DrawLine(transform.position, hit.point, Color.red, 2f);
                        }
                    }
                }
                else
                {
                    //no filtering, just notify
                    hearing.HeardSomething(this);
                }
            }
        }
    }
    
    //add visual for size of sounding
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}

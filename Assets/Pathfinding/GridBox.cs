using UnityEngine;

public class GridBox : MonoBehaviour
{
    private bool isBlocked = false;
    
    public bool IsBlocked => isBlocked;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovingWall"))
        {
            isBlocked = true;
            NavMeshGridGenerator.Instance?.InvalidatePosition(transform.position);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovingWall"))
        {
            isBlocked = false;
            NavMeshGridGenerator.Instance?.RevalidatePosition(transform.position);
        }
    }
}

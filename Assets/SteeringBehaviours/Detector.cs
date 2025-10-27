using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private List<GameObject> objectsInTrigger = new List<GameObject>();
    private int playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (!objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Add(other.gameObject);
                //Debug.Log($"Player entered: {other.gameObject.name}. Total objects: {objectsInTrigger.Count}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Remove(other.gameObject);
                //Debug.Log($"Player exited: {other.gameObject.name}. Total objects: {objectsInTrigger.Count}");
            }
        }
    }

    void Update()
    {
        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        int layerMask = ~(1 << playerLayer); 
        
        for (int i = objectsInTrigger.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsInTrigger[i];
            
            if (obj == null)
            {
                objectsInTrigger.RemoveAt(i);
                continue;
            }

            Vector3 direction = obj.transform.position - transform.position;
            float distance = direction.magnitude;

            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, layerMask))
            {
                objectsInTrigger.RemoveAt(i);
                Debug.Log($"Object blocked: {obj.name}. Total objects: {objectsInTrigger.Count}");
            }
        }
    }

    // method to access the list
    public List<GameObject> GetObjectsInTrigger()
    {
        return objectsInTrigger;
    }
}

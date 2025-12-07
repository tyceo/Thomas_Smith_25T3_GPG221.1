using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    
    private Coroutine activeCoroutine;
    public bool playerScary = false;
    private List<GameObject> triggeredObjects = new List<GameObject>();
    
    public TMP_Text ammoText;
    
    
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameplayManager.instance.ammo >= 0) // Left click
        {
            ActivateObjectTemporarily();
            GameplayManager.instance.ammo = GameplayManager.instance.ammo - 1;
            ammoText.text = "Attack Charges: " + GameplayManager.instance.ammo;
        }
        ammoText.text = "Attack Charges: " + GameplayManager.instance.ammo;
    }
    

    private void ActivateObjectTemporarily()
    {
        if (objectToActivate == null) return;

        //stop any existing coroutine to prevent overlapping
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(ActivateForDuration(1f));
    }

    private IEnumerator ActivateForDuration(float duration)
    {
        objectToActivate.SetActive(true);
        yield return new WaitForSeconds(duration);
        objectToActivate.SetActive(false);
        activeCoroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //check if the collided object is an NPC
        if (collision.gameObject.name.Contains("NPC"))
        {
            //StartCoroutine(SetPlayerScaryTemporarily());
            Destroy(gameObject); // Destroy this PlayerManager's game object
        }
    }

    public IEnumerator SetPlayerScaryTemporarily()
    {
        playerScary = true;
        
        yield return new WaitForSeconds(2f);
        playerScary = false;
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (!triggeredObjects.Contains(other.gameObject))
        {
            triggeredObjects.Add(other.gameObject);
        }

        //check if the object has NPCManager and playerScary is true
        NPCManager npcManager = other.GetComponent<NPCManager>();
        if (npcManager != null && playerScary)
        {
            StartCoroutine(SetNPCScaredTemporarily(npcManager));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //remove from tracked objects when they exit
        if (triggeredObjects.Contains(other.gameObject))
        {
            triggeredObjects.Remove(other.gameObject);
        }
    }

    private IEnumerator SetNPCScaredTemporarily(NPCManager npcManager)
    {
        npcManager.isScared = true;
        yield return new WaitForSeconds(1f);
        npcManager.isScared = false;
    }
}

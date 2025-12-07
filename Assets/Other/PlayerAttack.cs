using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Start()
    {
        
        playerManager = GetComponentInParent<PlayerManager>();
        
        // if fail then
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object's name contains "NPC"
        if (other.gameObject.name.Contains("NPC"))
        {
            // scares all nearby NPCs
            if (playerManager != null)
            {
                playerManager.StartCoroutine(playerManager.SetPlayerScaryTemporarily());
            }
            
            Destroy(other.gameObject);
        }
    }
}

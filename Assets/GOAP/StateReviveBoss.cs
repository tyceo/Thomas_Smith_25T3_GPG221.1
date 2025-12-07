using UnityEngine;
using Anthill.AI;


public class StateReviveBoss : AntAIState
{
    private NPCManager _NPCManager;
    
    //private bool bossalive = false;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("Revive Boss");
        _NPCManager.SetPathToRespawnSpot();
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (_NPCManager.bossBlueExists == false)
        {
            _NPCManager.RoamingForAwhile = false;
            
            
        }
        if (_NPCManager.bossBlueExists == true)
        {
            _NPCManager.BossAlive = true;
            _NPCManager.ScaredOfPlayer = false;
            Finish();
        }
    }
}

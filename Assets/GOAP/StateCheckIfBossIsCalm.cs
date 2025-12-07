using UnityEngine;
using Anthill.AI;

public class StateCheckIfBossIsCalm : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("Check if boss is calm");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        _NPCManager.RoamingForAwhile = false;
        _NPCManager.GiveBossSpace = true;
        _NPCManager.PathToBoss = false;
        Finish();
    }
}

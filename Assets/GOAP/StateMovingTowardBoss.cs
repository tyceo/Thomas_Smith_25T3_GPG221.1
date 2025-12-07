using UnityEngine;
using Anthill.AI;

public class StateMovingTowardBoss : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("Move towards boss");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (_NPCManager.closeToBoss)
        {
            _NPCManager.NearBoss = true;
                    
            Finish();
        }
        
    }
}

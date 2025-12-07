using UnityEngine;
using Anthill.AI;

public class StateLookForBoss : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("can see boss");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        _NPCManager.CanSeeBoss = true;
        Finish();
    }
}

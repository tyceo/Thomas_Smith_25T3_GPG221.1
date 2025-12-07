using UnityEngine;
using Anthill.AI;

public class StateRunFromPlayer : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("run from player");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        _NPCManager.ScaredOfPlayer = false;
        _NPCManager.NearPlayer = false;
        Finish();
    }
}

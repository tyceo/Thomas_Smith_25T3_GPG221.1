using UnityEngine;
using Anthill.AI;

public class StateGoToBoss : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("go to boss");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        
        _NPCManager.PathToBoss = true;
        Finish();
    }
}

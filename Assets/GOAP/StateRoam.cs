using UnityEngine;
using Anthill.AI;

public class StateRoam : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
        Debug.Log("doing roam");
    }

    public override void Enter()
    {
        
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        
        Finish();
    }
}

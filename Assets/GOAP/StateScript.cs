using UnityEngine;
using Anthill.AI;


public class StateScript : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("this should not be here");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        
        Finish();
    }
}

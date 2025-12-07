using UnityEngine;
using Anthill.AI;
    

                   //I missnamed this, it's supposed to be "StateWantToKillPlayer"
public class StateWantToKillBoss : AntAIState
{
    private NPCManager _NPCManager;
    
    public override void Create(GameObject aGameObject)
    {
        _NPCManager = aGameObject.GetComponent<NPCManager>();
    }

    public override void Enter()
    {
        Debug.Log("want to kill boss");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        _NPCManager.WantToKillPlayer = true;
        Finish();
    }
}
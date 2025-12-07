using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class NPCSense : MonoBehaviour, ISense
{

	public NPCManager _NPCManager;

	private void Awake()
	{
		_NPCManager = GetComponentInParent<NPCManager>();
		
	}

	/// <summary>
	/// This method will be called automaticaly each time when AntAIAgent decide to update the plan.
	/// You should attach this script to the same object where is AntAIAgent.
	/// </summary>
	public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
	{
		aWorldState.BeginUpdate(aAgent.planner);
		{
			aWorldState.Set(NormalNPC.BossAlive, _NPCManager.BossAlive);
			aWorldState.Set(NormalNPC.NearBoss, _NPCManager.NearBoss);
			aWorldState.Set(NormalNPC.ScaredOfPlayer, _NPCManager.ScaredOfPlayer);
			aWorldState.Set(NormalNPC.NearPlayer, _NPCManager.NearPlayer);
			aWorldState.Set(NormalNPC.BossWantsToKillPlayer, _NPCManager.BossWantsToKillPlayer);
			aWorldState.Set(NormalNPC.RoamingForAwhile, _NPCManager.RoamingForAwhile);
			aWorldState.Set(NormalNPC.CanSeeBoss, _NPCManager.CanSeeBoss);
			aWorldState.Set(NormalNPC.PathToBoss, _NPCManager.PathToBoss);
			aWorldState.Set(NormalNPC.GiveBossSpace, _NPCManager.GiveBossSpace);
			aWorldState.Set(NormalNPC.WantToKillPlayer, _NPCManager.WantToKillPlayer);
		}
		aWorldState.EndUpdate();
		
	}
	
	

	
}
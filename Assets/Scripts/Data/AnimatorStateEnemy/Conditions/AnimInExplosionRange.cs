using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionInExplosionRange", menuName = "AI/AnimCondition/InExplosionRange")]
public class AnimInExplosionRange : AI_Condition
{

    private RaycastManager raycastManager;
    public override bool CheckCondition(AI ai)
    {
        if(Vector3.Distance(ai.transform.position, ai.Player.transform.position) >= ai.Agent.stoppingDistance)
        {
            return RaycastTest(ai);
        }
        else
        {
            return false;
        }
    }

    private bool RaycastTest(AI ai)
    {

        if (!raycastManager)
        {
            raycastManager = FindObjectOfType<RaycastManager>();
        }

        return raycastManager.IsPlayerInRaycastRange(ai.transform.position, ai.Stats.attackRange / 3);

    }
}

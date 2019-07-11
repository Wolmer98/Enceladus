using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionPlayerInChargeRange", menuName = "AI/AnimCondition/PlayerInChargeRange")]
public class AnimPlayerInChargeRange : AI_Condition
{
    private RaycastManager raycastManager;

    public override bool CheckCondition(AI ai)
    {

        if (ai.ChargeTimer(ai.Stats.chargeCooldown))
        {
            float dist = Vector3.Distance(ai.transform.position, ai.Player.transform.position);

            if (dist >= ai.Stats.chargeRangeInterval.x && dist <= ai.Stats.chargeRangeInterval.y && !ai.MoveAcrossNavMeshesStarted)
            {
                ai.ChargeDirection = (ai.Player.transform.position - ai.transform.position).normalized;
                return RaycastTest(ai);
            }
        }

        return false;
    }

    private bool RaycastTest(AI ai)
    {

        if (!raycastManager)
        {
            raycastManager = FindObjectOfType<RaycastManager>();
        }

        return raycastManager.IsPlayerInRaycastRange(ai.transform.position, ai.Stats.chargeRangeInterval.y);

    }
}

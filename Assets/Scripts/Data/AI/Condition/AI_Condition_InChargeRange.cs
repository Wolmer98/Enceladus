using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_InChargeDistance", menuName = "AI/Condition/InChargeDistance")]
public class AI_Condition_InChargeRange : AI_Condition
{
    private RaycastManager raycastManager;

    public override bool CheckCondition(AI ai)
    {
        return InChargeRange(ai);
    }

    public bool InChargeRange(AI ai)
    {
        if (ai.ChargeTimer(ai.Stats.chargeCooldown))
        {
            float dist = Vector3.Distance(ai.Target.position, ai.transform.position);
            if (dist <= ai.Stats.chargeRangeInterval.y && dist >= ai.Stats.chargeRangeInterval.x && !ai.IsStunned)
            {
                ai.ChargeDirection = (ai.Target.position - ai.transform.position).normalized;

                ai.Agent.destination = ai.Target.position;
                return RaycastTest(ai);
               //return true;
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

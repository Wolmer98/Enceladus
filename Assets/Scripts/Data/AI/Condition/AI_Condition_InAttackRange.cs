using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Condition_InAttackRange", menuName = "AI/Condition/InAttackRange")]
public class AI_Condition_InAttackRange : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        return TargetInRange(ai);
    }

    private bool TargetInRange(AI ai)
    {
        if (ai.Target == null || ai.Stats == null)
        {
            return false;
        }
        
        float dist = Vector3.Distance(ai.transform.position, ai.Target.position);
        if (dist < ai.Stats.attackRange && !ai.IsStunned)
        {
            return true;
        }
        else
            return false;
    }
}

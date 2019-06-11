using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_SearchForDuration", menuName = "AI/Condition/AI_SearchForDuration")]
public class AI_SearchForDuration : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        return SearchForDuration(ai);
    }

    private bool SearchForDuration(AI ai)
    {
        if(ai.Stats == null)
        {
            Debug.LogError("Ai stats was null");
        }
        if (ai.ConditionTimeCheck(ai.Stats.searchTime) && !ai.IsStunned)
        {
            return true;
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionStunOver", menuName = "AI/AnimCondition/StunOver")]
public class AnimStunOver : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        if(!ai.IsStunned)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

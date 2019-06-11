using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ShotAndUnaware", menuName = "AI/Condition/ShotAndUnaware")]
public class AI_Condition_ShotAndUnaware : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        return IsUnawareOfPlayerAndShot(ai);
    }

    private bool IsUnawareOfPlayerAndShot(AI ai)
    {
        if(!ai.ChasingPlayer || !ai.FleeingFromPlayer || !ai.IsStunned)
        {
            return true;
        }
        return false;
    }
}

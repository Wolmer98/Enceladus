using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionWaitForSeconds", menuName = "AI/AnimCondition/WaitForSeconds")]
public class AnimWairForSecondsCondition : AI_Condition
{
    public float secondToWait = 4f;
    public override bool CheckCondition(AI ai)
    {
        return ai.ConditionTimeCheck(secondToWait);
    }
}

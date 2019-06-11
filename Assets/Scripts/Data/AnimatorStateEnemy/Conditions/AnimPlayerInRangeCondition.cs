using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionPlayerInRage", menuName = "AI/AnimCondition/PlayerInRange")]
public class AnimPlayerInRangeCondition : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        if(Vector3.Distance(ai.transform.position, ai.Player.transform.position) < ai.DetectionSphere.radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

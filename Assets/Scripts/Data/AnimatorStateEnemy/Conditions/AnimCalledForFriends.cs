using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionCalledForFriends", menuName = "AI/AnimCondition/CalledForFriends")]
public class AnimCalledForFriends : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        if(ai.CalledForFriends)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_ReturnToStart(RB)", menuName = "AI/Action/ReturnToStart(RB)")]
public class AI_Action_ReturnStartPos_RB : AI_Action
{
    public override void PreformAction(AI ai)
    {
        ReturnToStartPos(ai);
    }

    private void ReturnToStartPos(AI ai)
    {
        if (ai.StartPosition == null || ai.Stats == null )
        {
            return;
        }
        ai.Agent.SetDestination(ai.StartPosition);
        ai.Agent.isStopped = false;

        Rotate(ai.StartPosition, ai);

        Move(ai, ai.Stats.moveSpeed);
        
    }
}

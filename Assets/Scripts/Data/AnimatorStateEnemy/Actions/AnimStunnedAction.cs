using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimStunnedAction", menuName = "AI/AnimAction/Stunned")]
public class AnimStunnedAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.isStopped = true;
        ai.Animator.Play("Stunned");
        ai.IsStunned = true;
        ai.ActionTime = 0.0f;
        
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.Agent.isStopped = false;
        ai.IsStunned = false;
        anim.ResetTrigger("Stunned");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if(ai.ActionTimeCheck(ai.Stats.stunDuration))
        {
            ai.Animator.SetTrigger("Stand");
        }
    }
}

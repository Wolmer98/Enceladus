using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimActionIdle", menuName = "AI/AnimAction/Idle")]
public class AnimIdleAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.isStopped = true;
        ai.Animator.SetBool("Walking", false);
        ai.Animator.Play("Idle");
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        anim.ResetTrigger("Patrol");
        ai.ConditionTime = 0f;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }
}

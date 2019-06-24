﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimActionIdle", menuName = "AI/AnimAction/Idle")]
public class AnimIdleAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        AIStoppedMoving(ai);
        ai.Animator.SetBool("Walking", false);
        ai.Animator.Play("Idle");
        ai.ConditionTime = 0f;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        anim.ResetTrigger("Patrol");
        anim.ResetTrigger("Chase");
        anim.ResetTrigger("Search");
        anim.ResetTrigger("Idle");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }
}

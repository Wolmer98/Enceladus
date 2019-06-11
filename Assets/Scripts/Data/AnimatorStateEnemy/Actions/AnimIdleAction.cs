using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimActionIdle", menuName = "AI/AnimAction/Idle")]
public class AnimIdleAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.isStopped = true;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }
}

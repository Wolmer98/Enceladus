using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimatorAction : ScriptableObject
{
    public abstract void EnterActions(AI ai, Animator anim);
    public abstract void UpdateAction(AI ai, Animator anim);
    public abstract void ExitAction(AI ai, Animator anim);

    protected void AIStoppedMoving(AI ai)
    {
        ai.Agent.isStopped = true;
        ai.IsMoving = false;
    }

    protected void AIStartedMoving(AI ai)
    {
        ai.Agent.isStopped = false;
        ai.IsMoving = true;
    }
}

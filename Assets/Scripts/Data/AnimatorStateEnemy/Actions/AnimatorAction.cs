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
        if (ai.Agent.isOnNavMesh)
        {
            ai.Agent.isStopped = false;
            ai.IsMoving = true;
        }
        else
        {
            Debug.Log("AI was not on a mesh");
        }
    }

    protected void ExitStateUpdate(AI ai)
    {
        ai.ExitRun = true;
    }

    protected void EnterStateUpdate(AI ai, AnimatorAction animatorAction)
    {
        ai.ExitRun = false;
        ai.animatorAction = animatorAction;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimStunnedAction", menuName = "AI/AnimAction/Stunned")]
public class AnimStunnedAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.isStopped = true;
        ai.Agent.updatePosition = false;
        ai.Agent.updateRotation = false;
        ai.Agent.destination = ai.transform.position;
        ai.Agent.enabled = false;
        ai.Animator.Play("Stunned");
        ai.IsStunned = true;
        ai.ActionTime = 0.0f;
        ai.Rigidbody.velocity = Vector3.zero;
        ai.Rigidbody.isKinematic = true;
        ai.IsMoving = false;
        anim.ResetTrigger("Stunned");
        Debug.Log("Enter Stunned");
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.Agent.updatePosition = true;
        ai.Agent.updateRotation = true;
        ai.Agent.nextPosition = ai.transform.position;
        ai.Agent.enabled = true;
        ai.Agent.isStopped = false;
        ai.IsStunned = false;
        anim.ResetTrigger("Stunned");
        ai.Rigidbody.isKinematic = false;
        Debug.Log("Exit Stunned");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if(!(ai.StateMachine.GetCurrentAnimatorStateInfo(0).IsName("Stunned") || ai.StateMachine.GetCurrentAnimatorStateInfo(0).IsName("Stand")))
        {
            ai.Animator.Play("Stunned");
        }
        if(ai.ActionTimeCheck(ai.Stats.stunDuration))
        {
            ai.Animator.SetTrigger("Stand");
        }
    }
}

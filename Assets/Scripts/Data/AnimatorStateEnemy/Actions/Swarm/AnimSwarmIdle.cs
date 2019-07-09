using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimIdleSwarmAction", menuName = "AI/AnimAction/IdleSwarm")]
public class AnimSwarmIdle : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        foreach (SwarmAI swarmAI in ai.SwarmAIs)
        {
            swarmAI.Agent.SetDestination(ai.transform.position);
            swarmAI.Animator.SetFloat("WalkingOffset", Random.Range(0f, 0.7f));
            swarmAI.Animator.SetFloat("IdleOffset", Random.Range(0f, 0.7f));
            swarmAI.Animator.Play("Idle");
        }
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        anim.ResetTrigger("Idle");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }
}

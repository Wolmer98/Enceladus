using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimSwarmChaseAction", menuName = "AI/AnimAction/SwarmChase")]
public class AnimSwarmChase : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        anim.ResetTrigger("Chase");
        ai.IsMoving = true;
        foreach (SwarmAI swarmAI in ai.SwarmAIs)
        {
            if (swarmAI != null)
            {
                swarmAI.Agent.SetDestination(ai.Player.transform.position);
                swarmAI.Animator.SetFloat("WalkingOffset", Random.Range(0f, 0.7f));
                swarmAI.Animator.Play("Walking");
            }
        }
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        anim.ResetTrigger("Chase");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        ai.SwarmAgent.SetDestination(ai.Player.transform.position);

        foreach (SwarmAI swarmAI in ai.SwarmAIs)
        {
            if (swarmAI != null)
            {
                swarmAI.UpdateChaseSwarm();
            }
        }
    }
}

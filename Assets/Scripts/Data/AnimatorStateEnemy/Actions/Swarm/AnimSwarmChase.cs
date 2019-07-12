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
        if (ai.SwarmAIs.Count <= 0)
        {
            ai.IsMoving = false;
            ai.StopSounds();
            ai.NotifyEnemyManagerDeath();
            Destroy(ai.gameObject);
        }
        foreach (SwarmAI swarmAI in ai.SwarmAIs)
        {
            if (swarmAI != null)
            {
                swarmAI.Agent.SetDestination(ai.Player.transform.position);
                swarmAI.Animator.SetFloat("WalkingOffset", Random.Range(0f, 0.7f));
                swarmAI.Animator.Play("Walking");

                swarmAI.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        anim.ResetTrigger("Chase");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if (ai.SwarmAIs.Count <= 0)
        {
            ai.IsMoving = false;
            ai.StopSounds();
            ai.NotifyEnemyManagerDeath();
            Destroy(ai.gameObject);
        }

        ai.SwarmAgent.SetDestination(ai.Player.transform.position);

        foreach (SwarmAI swarmAI in ai.SwarmAIs)
        {
            if (swarmAI != null)
            {
                swarmAI.UpdateChaseSwarm();
                if(Vector3.Distance(swarmAI.transform.position, ai.SwarmAgent.transform.position) >= ai.DetectionSphere.radius)
                {
                    swarmAI.Agent.nextPosition = ai.SwarmAgent.nextPosition;
                }
            }
            else
            {
                ai.IsMoving = false;
                ai.StopSounds();
                ai.NotifyEnemyManagerDeath();
                Destroy(ai.gameObject);
            }
        }

    }
}

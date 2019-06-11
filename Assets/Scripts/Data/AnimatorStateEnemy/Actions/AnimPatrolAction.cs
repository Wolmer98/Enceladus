using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimActionPatrol", menuName = "AI/AnimAction/Patrol")]
public class AnimPatrolAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        if(ai.PatrolWaypoints == null)
        {
            anim.SetTrigger("Idle");
            return;
        }

        ai.Agent.isStopped = false;

        ai.PatrolWaypointIndex = Random.Range(0, ai.PatrolWaypoints.Length);
        ai.Agent.speed = ai.Stats.moveSpeed;

        ai.Agent.SetDestination(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position);
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.Animator.SetBool("Walking", false);
        ai.Animator.ResetTrigger("EchoLocationOver");
        ai.SetATimer = false;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if (ai.Agent.remainingDistance <= ai.Agent.stoppingDistance && !ai.Agent.pathPending)
        {
            ai.PatrolWaypointIndex = (ai.PatrolWaypointIndex + 1) % ai.PatrolWaypoints.Length;
            ai.Agent.SetDestination(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position);
            if (Random.Range(0f, 1f) < ai.Stats.chanceOfNearRadius)
            {
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;

                ai.Animator.Play("EchoLocation");

                ai.Animator.SetBool("Walking", false);
                ai.IsMoving = false;
                ai.Agent.isStopped = true;
            }
        }
        else if (ai.SetATimer)
        {
            if (ai.ActionTimeCheck(ai.Stats.nearRadiusDuration))
            {
                ai.SetATimer = false;
                ai.Animator.SetTrigger("EchoLocationOver");
                ai.Agent.isStopped = false;
            }
        }
        else
        {
            ai.Animator.SetBool("Walking", true);
        }
    }
}

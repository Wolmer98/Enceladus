using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Patrol(RB)", menuName = "AI/Action/Patrol(RB)")]
public class AI_Action_Patrol_RB : AI_Action
{
    public override void PreformAction(AI ai)
    {
        Patrol(ai);
    }

    private void Patrol(AI ai)
    {
        if (ai.PatrolWaypoints == null || ai.PatrolWaypoints.Length == 0 || ai.Stats == null)
        {
            return;
        }

        if (ai.PatrolWaypoints[ai.PatrolWaypointIndex] == null)
        {
            return;
        }

        ai.Agent.SetDestination(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position);
        ai.Agent.stoppingDistance = 0.5f;
        if (ai.Agent.remainingDistance <= ai.Agent.stoppingDistance && !ai.Agent.pathPending)
        {
            ai.PatrolWaypointIndex = (ai.PatrolWaypointIndex + 1) % ai.PatrolWaypoints.Length;
            if (Random.Range(0f, 1f) < ai.Stats.chanceOfNearRadius && !ai.IsEchoLocating)
            {
                ai.IsEchoLocating = true;
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;

                ai.Animator.Play("EchoLocation");

                ai.Animator.SetBool("Walking", false);
                ai.IsMoving = false;
            }
        }

        if (ai.SetATimer)
        {
            if (ai.ActionTimeCheck(ai.Stats.nearRadiusDuration))
            {
                ai.SetATimer = false;
                ai.Animator.SetTrigger("EchoLocationOver");
            }
            else
            {
                ai.Agent.nextPosition = ai.Rigidbody.position;
                return;
            }
        }
        else if (!ai.SetATimer)
        {
            Rotate(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position, ai);

            ai.IsMoving = true;
            ai.Animator.SetBool("Walking", true);
            Move(ai, ai.Stats.moveSpeed);
        }

    }
}

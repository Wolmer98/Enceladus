﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimPatrolAction", menuName = "AI/AnimAction/Patrol")]
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
        //ai.Animator.Play("Walking");
        ai.PatrolWaypointIndex = Random.Range(0, ai.PatrolWaypoints.Length);
        ai.Agent.speed = ai.Stats.moveSpeed;
        ai.Animator.Play("Walking");
        ai.Animator.SetBool("Walking", true);
        AIStartedMoving(ai);
        ai.Agent.SetDestination(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position);
        ai.SetATimer = false;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.SetATimer = false;
        anim.ResetTrigger("Patrol");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if (ai.Agent.remainingDistance <= ai.Agent.stoppingDistance && !ai.Agent.pathPending)
        {
            ai.PatrolWaypointIndex = (ai.PatrolWaypointIndex + 1) % ai.PatrolWaypoints.Length;
            ai.Agent.SetDestination(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position);
            if (Random.Range(0f, 1f) < ai.Stats.chanceOfNearRadius && !ai.SetATimer)
            {
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;

                ai.Animator.Play("EchoLocation");

                ai.PlayEcho(0.0f);
                ai.Animator.SetBool("Walking", false);
                AIStoppedMoving(ai);
            }
        }
        else if (ai.SetATimer)
        {
            ai.DetectionSphere.radius = Mathf.Lerp(ai.DetectionSphere.radius, ai.Stats.echoDetectionNearRadius, ai.Stats.nearRadiusLerpSpeed * Time.deltaTime);
            if (ai.ActionTimeCheck(ai.Stats.nearRadiusDuration))
            {
                ai.SetATimer = false;
                ai.Animator.SetBool("Walking", true);
                ai.Animator.Play("Walking");
                AIStartedMoving(ai);
                ai.DetectionSphere.radius = ai.Stats.detectionRadius;
            }
        }
    }

}

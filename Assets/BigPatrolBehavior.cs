using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPatrolBehavior : StateMachineBehaviour
{
    private AI ai;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ai == null)
        {
            ai = animator.gameObject.GetComponent<AI>();
        }

        ai.PatrolWaypointIndex = Random.Range(0, ai.PatrolWaypoints.Length-1);
        ai.Agent.speed = ai.Stats.moveSpeed;

        ai.Agent.SetDestination(ai.PatrolWaypoints[ai.PatrolWaypointIndex].transform.position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
            Debug.Log("Atimer");
            if (ai.ActionTimeCheck(ai.Stats.nearRadiusDuration))
            {
                Debug.Log("detection Over");
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

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Search(RB)", menuName = "AI/Action/Search(RB)")]
public class AI_Action_Search : AI_Action
{
    public override void PreformAction(AI ai)
    {
        Search(ai);
    }

    private void Search(AI ai)
    {

        if (ai.SearchPoints == null || !ai.FoundSearchPoints || ai.SearchPoints[ai.SearchPointIndex] == null || ai.Stats == null)
        {
            return;
        }

        ai.Agent.SetDestination(ai.SearchPoints[ai.SearchPointIndex]);

        if (ai.Agent.remainingDistance <= ai.Agent.stoppingDistance && !ai.Agent.pathPending)
        {
            ai.SearchPointIndex = (ai.SearchPointIndex + 1) % ai.SearchPoints.Length;
            if(Random.Range(0f,1f) < ai.Stats.chanceOfFarRadius && !ai.IsEchoLocating)
            {
                ai.IsScreaming = true;
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;

                ai.Animator.Play("EchoLocationFar");

                ai.Animator.SetBool("Walking", false);
                ai.IsMoving = false;

            }
        }

        if (ai.SetATimer)
        {
            if (ai.ActionTimeCheck(ai.Stats.farRadiusDuration))
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
            //Rotate
            Rotate(ai.SearchPoints[ai.SearchPointIndex], ai);

            float moveSpeed = ai.Stats.chaseSpeed;
            if (ai.Enrage)
            {
                moveSpeed = ai.Stats.chaseSpeed * ai.Stats.speedMultiplier;
            }
            //Move
            ai.IsMoving = true;
            ai.Animator.SetBool("Running", true);
            Move(ai, moveSpeed);
        }

    }
}

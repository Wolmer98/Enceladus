using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_ChaseTarget(RB)", menuName = "AI/Action/ChaseTarget(RB)")]
public class AI_Action_ChaseTarget_RB : AI_Action
{
    public override void PreformAction(AI ai)
    {
        MoveTowardsTarget(ai);
    }

    private void MoveTowardsTarget(AI ai)
    {
        if (ai.Target == null || ai.Stats == null)
        {
            return;
        }
        ai.Agent.SetDestination(ai.Target.transform.position);
        ai.Agent.isStopped = false;

        //Rotation
        if (ai.EnemyType == enemyType.swarm)
        {
            Rotate(ai.Target.position, ai);
        }

        if (!ai.IsAttacking || !ai.GroundCheck())
        {
            float chaseSpeed = ai.Stats.chaseSpeed;
            if(ai.Enrage)
            {
                chaseSpeed = ai.Stats.chaseSpeed * ai.Stats.speedMultiplier;
            }

            if (ai.EnemyType == enemyType.big)
            {
                if (Vector3.Distance(ai.transform.position, ai.Target.position) < ai.Stats.attackRange - 1f)
                {
                    ai.IsMoving = false;
                    ai.Animator.SetBool("Walking", false);
                    ai.Animator.SetBool("Running", false);
                }
                else
                {
                    //Move
                    ai.IsMoving = true;
                    ai.Animator.SetBool("Walking", true);
                    ai.Animator.SetBool("Running", true);
                    Move(ai, chaseSpeed);
                }
            }
            else
            {
                //Move
                ai.IsMoving = true;
                ai.Animator.SetBool("Walking", true);
                ai.Animator.SetBool("Running", true);
                Move(ai, chaseSpeed);
            }

            if(ai.EnemyType == enemyType.big)
            {
                Rotate(ai.Target.position, ai);
            }
        }
    }
}

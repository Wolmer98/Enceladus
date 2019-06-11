using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AI_Action_Strafe(RB)", menuName = "AI/Action/Strafe(RB)")]
public class AI_Action_Strafe : AI_Action
{
    public override void PreformAction(AI ai)
    {
        //Rotate
        Rotate(ai.Target.position, ai);
        if(!ai.GroundCheck())
        {
            return;
        }
        FindStrafePoints(ai);
        Strafe(ai);

    }

    private void FindStrafePoints(AI ai)
    {
        if (ai.FoundSearchPoints || ai.Target == null)
        {
            return;
        }
        Vector3 randomDir = Random.insideUnitSphere * ai.Stats.attackRange;
        randomDir += ai.Target.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDir, out hit, ai.Stats.attackRange, NavMesh.AllAreas))
        {
            ai.SearchPoints[0] = hit.position;
            ai.FoundSearchPoints = true;
        }
    }

    private void Strafe(AI ai)
    {

        if (ai.SearchPoints == null || !ai.FoundSearchPoints || ai.Target == null || ai.Stats == null)
        {
            return;
        }

        if (ai.AttackTimer(ai.Stats.attackSpeed))
        {
            ai.Agent.SetDestination(ai.Target.transform.position);
            ai.Agent.isStopped = false;
        }
        else if (ai.SearchPoints != null && ai.FoundSearchPoints)
        {
            ai.Agent.SetDestination(ai.SearchPoints[0]);
        }


        //Move
        Move(ai, ai.Stats.chaseSpeed);
        ai.IsMoving = true;
        ai.Animator.SetBool("Walking", true);
    }

}

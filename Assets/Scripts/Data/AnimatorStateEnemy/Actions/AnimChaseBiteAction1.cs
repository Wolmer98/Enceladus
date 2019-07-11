using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AnimChaseBiteAction", menuName = "AI/AnimAction/ChaseBite")]
public class AnimChaseBiteAction1 : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.nextPosition = ai.transform.position;
        ai.Agent.isStopped = false;
        ai.Agent.updatePosition = true;
        ai.Agent.updateRotation = true;
        ai.ChasingPlayer = true;
        ai.IsMoving = true;
        if(Vector3.Distance(ai.transform.position, ai.Player.transform.position) > ai.Agent.stoppingDistance)
        {
            ai.Animator.Play("Walking");
        }
        ai.Agent.speed = ai.Stats.chaseSpeed;

        ai.DetectionSphere.radius = ai.Stats.detectionRadius * ai.Stats.chaseDetectionMultiplier;
        anim.ResetTrigger("Chase");
        ai.Agent.autoTraverseOffMeshLink = false;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.IsAttacking = false;
        ai.DetectionSphere.radius = ai.Stats.detectionRadius;
        anim.ResetTrigger("Chase");
        ai.Agent.autoTraverseOffMeshLink = true;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        // isAttacking is changed in the animation clip for the bite attack.

        if (ai.Agent.isOnOffMeshLink && !ai.MoveAcrossNavMeshesStarted)
        {
            ai.MoveAcrossNavMeshLinkStart();
           
        }
        else if(ai.MoveAcrossNavMeshesStarted)
        {
            return;
        }
        if(ai.IsAttacking)
        {
            return;
        }

        //if (ai.typeOfEnemy == enemyType.big)
        //{
        //    if (!ai.Agent.isOnNavMesh)
        //    {
        //        ai.Agent.enabled = false;
        //        return;
        //    }
        //    else if (!ai.Agent.enabled)
        //    {
        //        ai.Agent.enabled = true;
        //    }
        //}
        if(!ai.Agent.isOnNavMesh)
        {   
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(ai.transform.position, out hit, NavMesh.AllAreas))
            {
                Debug.Log("Found closest edge");
                ai.transform.position = hit.position;
                ai.Agent.nextPosition = hit.position;
            }
            ai.Agent.isStopped = false;
            Debug.Log("AI wasn't active");
            ai.Agent.SetDestination(ai.Player.transform.position);
        }
        else
        {
            ai.Agent.SetDestination(ai.Player.transform.position);
        }

        float dist = Vector3.Distance(ai.transform.position, ai.Player.transform.position);

        if (dist > ai.Agent.stoppingDistance)
        {
            ai.Animator.SetBool("Walking", true);
            ai.IsMoving = true;
        }
        else
        {
            ai.Animator.SetBool("Walking", false);
            ai.IsMoving = false;
            LookAtPlayer(ai);
        }

        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Stats.attackRange && ai.typeOfEnemy != enemyType.carrier)
        {
            ai.IsAttacking = true;
            ai.IsMoving = false;
            ai.AttackCooldown = 0;
            ai.Animator.Play("BiteAttack");
            FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
        }


        //if (ai.Agent.isOnOffMeshLink)
        //{
        //    ai.Agent.speed = ai.Stats.chaseSpeed / 4;
        //}
        //else
        //{
        //    ai.Agent.speed = ai.Stats.chaseSpeed;
        //}
    }

    private void LookAtPlayer(AI ai)
    {
        ai.transform.LookAt(new Vector3(ai.Player.transform.position.x, ai.transform.position.y, ai.Player.transform.position.z));
    }
}

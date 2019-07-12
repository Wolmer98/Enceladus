using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimChaseBiteAction", menuName = "AI/AnimAction/ChaseBite")]
public class AnimChaseBiteAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.ChasingPlayer = true;
        if(Vector3.Distance(ai.transform.position, ai.Player.transform.position) > ai.Agent.stoppingDistance)
        {
            ai.Animator.Play("Walking");
        }
        ai.Agent.speed = ai.Stats.chaseSpeed;
        AIStartedMoving(ai);

        ai.DetectionSphere.radius = ai.Stats.detectionRadius * ai.Stats.chaseDetectionMultiplier;
        anim.ResetTrigger("Chase");
        //ai.Agent.autoTraverseOffMeshLink = false;
        ai.Agent.SetDestination(ai.Player.transform.position);
        //Debug.Log("Enter ChaseBite");
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.IsAttacking = false;
        ai.DetectionSphere.radius = ai.Stats.detectionRadius;
        anim.ResetTrigger("Chase");
        //ai.Agent.autoTraverseOffMeshLink = true;
        //Debug.Log("Exit ChaseBite");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        // isAttacking is changed in the animation clip for the bite attack.
        if (ai.IsAttacking)
        {
            return;
        }
        if(!ai.Agent.isOnNavMesh)
        {
            ai.Agent.enabled = false;
            return;
        }
        else if (!ai.Agent.enabled)
        {
            ai.Agent.enabled = true;
        }

        //if (ai.Agent.isOnOffMeshLink && !ai.MoveAcrossNavMeshesStarted)
        //{
        //    ai.MoveAcrossNavMeshLinkStart();
        //}

        ai.Agent.SetDestination(ai.Player.transform.position);

        float dist = Vector3.Distance(ai.transform.position, ai.Player.transform.position);

        if (dist > ai.Agent.stoppingDistance)
        {
            ai.Animator.SetBool("Walking", true);
            AIStartedMoving(ai);
        }
        else
        {
            ai.Animator.SetBool("Walking", false);
            AIStoppedMoving(ai);
            ai.transform.LookAt(new Vector3 (ai.Player.transform.position.x, ai.transform.position.y, ai.Player.transform.position.z));
        }

        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Stats.attackRange && ai.typeOfEnemy != enemyType.carrier)
        {
            Vector3 toPlayer = (ai.Player.transform.position - ai.transform.position).normalized;
            float angle = Vector3.Dot(ai.transform.forward, toPlayer);
            if (angle > 0.7)
            {
                ai.IsAttacking = true;
                AIStoppedMoving(ai);
                ai.AttackCooldown = 0;
                ai.Animator.Play("BiteAttack");
                FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
            }
        }


        if (ai.Agent.isOnOffMeshLink)
        {
            ai.Agent.speed = ai.Stats.chaseSpeed / 3;
        }
        else
        {
            ai.Agent.speed = ai.Stats.chaseSpeed;
        }

    }
}

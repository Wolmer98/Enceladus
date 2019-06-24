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
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.ChasingPlayer = false;
        ai.IsAttacking = false;
        ai.Animator.SetBool("Walking", false);
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        // isAttacking is changed in the animation clip for the bite attack.
        if(ai.IsAttacking)
        {
            return;
        }

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

        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Stats.attackRange)
        {
            ai.IsAttacking = true;
            AIStoppedMoving(ai);
            ai.AttackCooldown = 0;
            ai.Animator.Play("BiteAttack");
            FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimChaseBiteAction", menuName = "AI/AnimAction/ChaseBite")]
public class AnimChaseBiteAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.ChasingPlayer = true;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.ChasingPlayer = false;
        ai.IsAttacking = false;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        // isAttacking is changed in the animation clip for the bite attack.
        if(ai.IsAttacking)
        {
            ai.Agent.isStopped = true;
            return;
        }

        ai.Agent.isStopped = false;
        ai.Agent.SetDestination(ai.Player.transform.position);

        float dist = Vector3.Distance(ai.transform.position, ai.Player.transform.position);

        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Stats.attackRange)
        {
            ai.IsAttacking = true;
            ai.Animator.SetBool("Walking", false);
            ai.AttackCooldown = 0;
            ai.Animator.Play("BiteAttack");
        }
    }
}

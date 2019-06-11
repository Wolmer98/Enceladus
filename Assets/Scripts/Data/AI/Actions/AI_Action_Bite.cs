using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_BiteAttack", menuName = "AI/Action/BiteAttack")]
public class AI_Action_Bite : AI_Action
{
    public LayerMask raycastMask;
    public LayerMask damageMask;

    public override void PreformAction(AI ai)
    {
        Bite(ai);
    }

    private void Bite(AI ai)
    {
        if (ai.Target == null || ai.Stats == null)
        {
            return;
        }

        float dist = Vector3.Distance(ai.transform.position, ai.Target.position);

        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Stats.attackRange)
        {
            ai.IsAttacking = true;
            ai.Animator.SetBool("Walking", false);
            ai.AttackCooldown = 0;
            ai.Animator.Play("BiteAttack");
        }
        if (ai.IsAttacking)
        {
            //CheckOverlap(ai);
        }
        else if (ai.AttackCooldown >= ai.Stats.attackSpeed / 2)
        {
            ai.IsAttacking = false;
        }
    }

}

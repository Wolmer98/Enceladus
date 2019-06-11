using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_JumpAttack", menuName = "AI/Action/JumpAttack")]
public class AI_Action_JumpAttack : AI_Action
{
    public LayerMask raycastMask;
    public LayerMask damageMask;

    public override void PreformAction(AI ai)
    {
        Attack(ai);
    }

    private void Attack(AI ai)
    {
        if(ai.Target == null || ai.Stats == null)
        {
            return;
        }

        float dist = Vector3.Distance(ai.transform.position, ai.Target.position);
        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Agent.stoppingDistance + ai.Stats.attackRange)
        {
            Vector3 dir = (ai.Target.position - ai.transform.position).normalized;

            RaycastHit hit;
            if(Physics.Raycast(ai.transform.position, dir, out hit, ai.Stats.attackRange, raycastMask))
            {
                PlayerController pc = hit.transform.gameObject.GetComponent<PlayerController>();
                if (pc != null)
                {
                    ai.Animator.SetTrigger("Jumping");
                    ai.IsAttacking = true;
                    ai.AttackCooldown = 0;
                    ai.FoundSearchPoints = false;
                }            
            }
        }
        else
        {
            if (ai.IsAttacking)
            {
                Collider[] colliders = new Collider[1];
                Physics.OverlapSphereNonAlloc(ai.DamagePosition, ai.DamageRadius, colliders, damageMask);

                if (colliders[0] != null)
                {
                    Destructible destructible = colliders[0].gameObject.GetComponent<Destructible>();
                    if (destructible != null)
                    {
                        destructible.Hurt(ai.Stats.damage * ai.DifficultyMod);
                        ai.IsAttacking = false;
                    }
                }
            }

            if (!ai.GroundCheck())
            {
                ai.Agent.nextPosition = ai.Rigidbody.position;
            }
            else if (ai.AttackCooldown >= ai.Stats.attackSpeed / 2)
            {
                ai.IsAttacking = false;
                ai.Animator.SetTrigger("Landing");
            }
        }
    }
}

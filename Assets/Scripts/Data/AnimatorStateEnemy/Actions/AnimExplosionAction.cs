using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimExplosionAction", menuName = "AI/AnimAction/Explosion")]
public class AnimExplosionAction : AnimatorAction
{
    public LayerMask explosionLayer;
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.enabled = false;
        ai.Animator.Play("Explode");
        ai.IsStunned = true;
        ai.ActionTime = 0.0f;
        ai.Rigidbody.velocity = Vector3.zero;
        ai.Rigidbody.isKinematic = true;
        ai.IsMoving = false;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if (ai.ActionTimeCheck(ai.Stats.attackSpeed) && !ai.IsAttacking)
        {
            ai.IsAttacking = true;
            Explode(ai);
        }
    }

    private void Explode(AI ai)
    {
        Collider[] colliders = new Collider[15];
        Physics.OverlapSphereNonAlloc(ai.transform.position, ai.Stats.attackRange, colliders, explosionLayer);

        foreach (Collider target in colliders)
        {
            if (target != null)
            {
                if (target.gameObject != ai.gameObject)
                {
                    Debug.Log(target.gameObject.name);
                    Destructible destructible = target.gameObject.GetComponent<Destructible>();
                    if (destructible != null)
                    {
                        destructible.Hurt(CalculateDamage(target.transform, ai));
                    }
                }
            }
        }
        ai.Destructible.Hurt(1000f);
    }

    private float CalculateDamage(Transform transform, AI ai)
    {
        float damageFallOff = 1 / Vector3.Distance(transform.position, ai.transform.position);
        return ai.Stats.damage * ai.DifficultyMod * damageFallOff;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] AI ai;
    public void JumpAttack()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        Vector3 jumpdir = (pc.MainCamera.transform.position - ai.transform.position);
        ai.Rigidbody.AddForce(jumpdir * 4, ForceMode.VelocityChange);
        FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
    }

    public void BiteAttack()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(ai.DamagePosition, ai.DamageRadius, colliders, ai.Stats.targetsLayerMask);

        if (colliders[0] != null)
        {
            Destructible destructible = colliders[0].gameObject.GetComponent<Destructible>();
            if (destructible != null)
            {
                if (ai.Enrage)
                {
                    destructible.Hurt(ai.Stats.damage * ai.Stats.damageMultiplier * ai.DifficultyMod);
                    ai.IsAttacking = false;
                }
                else
                {
                    destructible.Hurt(ai.Stats.damage * ai.DifficultyMod);
                    ai.IsAttacking = false;
                }
            }
        }
    }

    public void StunOver()
    {
        ai.IsStunned = false;
        ai.HasCharged = true;
        AttackOver();
        Debug.Log("Stun Over");
    }

    public void AttackOver()
    {
        ai.IsAttacking = false;
    }

}

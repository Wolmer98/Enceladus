﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimChaseActionBite", menuName = "AI/AnimAction/ChaseBite")]
public class AnimChaseBiteAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.ChasingPlayer = true;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.ChasingPlayer = false;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        ai.Agent.SetDestination(ai.Target.position);

        float dist = Vector3.Distance(ai.transform.position, ai.Target.position);

        if (ai.AttackTimer(ai.Stats.attackSpeed) && dist < ai.Stats.attackRange)
        {
            ai.IsAttacking = true;
            ai.Animator.SetBool("Walking", false);
            ai.AttackCooldown = 0;
            ai.Animator.Play("BiteAttack");
        }
    }
}

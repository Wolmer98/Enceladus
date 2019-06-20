﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimChargeAction", menuName = "AI/AnimAction/Charge")]
public class AnimChargeAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.ChasingPlayer = true;
        ai.Agent.isStopped = true;
        FMODUnity.RuntimeManager.PlayOneShot(ai.aggroSound, ai.transform.position);
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.ChasingPlayer = false;
        ai.Agent.isStopped = false;
        ai.SetATimer = false;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //Windup before charge.
        if (!ai.SetATimer)
        {
            //Debug.Log("AI set winduptime");
            ai.Animator.Play("Windup");
            ai.SetATimer = true;
            ai.ActionTime = 0.0f;
        }

        if (ai.ActionTimeCheck(ai.Stats.chargeWindupTime))
        {
            ai.ChargeCooldown = 0.0f;
            if (ai.Enrage)
            {
                Charge(ai, ai.Stats.chargeSpeed * ai.Stats.chargeSpeedMultiplier);
            }
            else
            {
                Charge(ai, ai.Stats.chargeSpeed);
            }
            //Debug.Log("Enemy Charging");
            ai.IsMoving = true;
        }

        //ai.transform.Rotate(ai.ChargeDirection);
        LookInDirection(ai);
    }


    private void Charge(AI ai, float speed)
    {
        ai.Rigidbody.MovePosition(ai.Rigidbody.position + ai.ChargeDirection * Time.deltaTime * speed);
        ai.Agent.nextPosition = ai.Rigidbody.position;
        ai.Animator.SetBool("Charging", true);
    }

    private void LookInDirection(AI ai)
    {
        float step = ai.Stats.rotationSpeed * Time.fixedDeltaTime;

        Vector3 newDir = Vector3.RotateTowards(ai.transform.forward, ai.ChargeDirection, step, 0.0f);
        ai.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
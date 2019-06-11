using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_ChargeAttack(RB)", menuName = "AI/Action/ChargeAttack(RB)")]
public class AI_Action_Charge : AI_Action
{
    public float windupChargeDuration = 2;

    public override void PreformAction(AI ai)
    {
        ChargeAttack(ai);
    }

    private void ChargeAttack(AI ai)
    {
        ai.ChasingPlayer = true;
        if(ai.IsStunned)
        {
            return;
        }

        //Windup before charge.
        if(!ai.SetATimer)
        {
            //Debug.Log("AI set winduptime");
            FMODUnity.RuntimeManager.PlayOneShot(ai.aggroSound, ai.transform.position);
            ai.Animator.Play("Windup");
            ai.SetATimer = true;
            ai.ActionTime = 0.0f;
        }

        if (ai.ActionTimeCheck(windupChargeDuration))
        {
            ai.ChargeCooldown = 0.0f;
            ai.IsAttacking = true;
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

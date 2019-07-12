using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AnimChargeAction", menuName = "AI/AnimAction/Charge")]
public class AnimChargeAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.ChasingPlayer = true;
        ai.Agent.isStopped = true;
        ai.Agent.updatePosition = false;
        ai.Agent.updateRotation = false;
        FMODUnity.RuntimeManager.PlayOneShot(ai.aggroSound, ai.transform.position);
        ai.HasCharged = true;
        ai.Agent.autoTraverseOffMeshLink = false;
        ai.ChargeCooldown = 0.0f;
        ai.Agent.enabled = false;
        anim.ResetTrigger("Chase");
        anim.ResetTrigger("Charge");
        Debug.Log("Enter Charge");
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        Debug.Log("Exit Charge");
        anim.ResetTrigger("Charge");
        ai.SetATimer = false;
        ai.Agent.updateRotation = true;
        ai.Agent.autoTraverseOffMeshLink = true;
        ai.Agent.updatePosition = true;
        ai.Agent.nextPosition = ai.transform.position;
        ai.Animator.SetBool("Charging", false);
        ai.Agent.enabled = true;
        ai.Agent.isStopped = false;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //ai.Agent.destination = ai.transform.position;
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
            ai.Rigidbody.freezeRotation = true;
            ai.HasCharged = false;
            ai.ChargeCooldown = 0.0f;
            if (ai.Enrage)
            {
                Charge(ai, ai.Stats.chargeSpeed * ai.Stats.chargeSpeedMultiplier);
            }
            else
            {
                Charge(ai, ai.Stats.chargeSpeed);
            }
        }
        else if (ai.HasCharged)
        {
            //Debug.Log("Looking");
            LookInDirection(ai);
        }
    }


    private void Charge(AI ai, float speed)
    {
        ai.Rigidbody.MovePosition(ai.Rigidbody.position + ai.ChargeDirection * Time.deltaTime * speed);
        ai.Agent.nextPosition = ai.Rigidbody.position;
        ai.Animator.SetBool("Charging", true);
        ai.IsAttacking = true;
    }

    private void LookInDirection(AI ai)
    {
        float step = ai.Stats.rotationSpeed * Time.fixedDeltaTime;

        Vector3 newDir = Vector3.RotateTowards(ai.transform.forward, ai.ChargeDirection, step, 0.0f);
        ai.transform.rotation = Quaternion.LookRotation(newDir);
    }
}

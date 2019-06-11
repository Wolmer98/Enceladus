using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_MoveToSound(RB)", menuName = "AI/Action/MoveToSound(RB)")]
public class AI_Action_MoveToSound : AI_Action
{
    public override void PreformAction(AI ai)
    {
        MoveToSoundPosition(ai);
    }

    private void MoveToSoundPosition(AI ai)
    {
        if(ai.SoundLastPosition != null || ai.Stats == null)
        {
            ai.Agent.SetDestination(ai.SoundLastPosition);
            ai.Agent.isStopped = false;

            ai.Agent.stoppingDistance = 0.2f;
            Rotate(ai.SoundLastPosition, ai);


            float moveSpeed = ai.Stats.chaseSpeed;
            if (ai.Enrage)
            {
                moveSpeed = ai.Stats.chaseSpeed * ai.Stats.speedMultiplier;
            }

            ai.IsMoving = true;
            ai.Animator.SetBool("Running", true);
            Move(ai, moveSpeed);
        }
    }
}

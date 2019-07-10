using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimSpawmSwarmAction", menuName = "AI/AnimAction/SpawmSwarm")]
public class AnimSpawnSwarm : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Animator.Play("Idle");
        ai.HasSpawnedEnemies = false;
        ai.Agent.SetDestination(ai.transform.position);
        ai.Agent.isStopped = true;
        ai.Agent.updatePosition = false;
        ai.IsMoving = false;
        anim.ResetTrigger("SpawnSwarm");

        ai.SpawningEnemies = true;
        ai.Animator.Play("SpawnEnemies");
        FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.IsMoving = true;
        ai.Agent.nextPosition = ai.transform.position;
        ai.Agent.updatePosition = true;
        ai.Agent.isStopped = false;
        ai.ConditionTime = 0f;
        ai.SpawningEnemies = false;
        anim.ResetTrigger("SpawnSwarm");
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if (ai.HasSpawnedEnemies)
        {
            anim.SetTrigger("Chase");
        }
    }

    private void LookAtPlayer(AI ai)
    {
        float dist = Vector3.Distance(ai.Player.transform.position, ai.transform.position);

        ai.transform.LookAt(new Vector3(ai.Player.transform.position.x, ai.transform.position.y, ai.Player.transform.position.z));
    }
}

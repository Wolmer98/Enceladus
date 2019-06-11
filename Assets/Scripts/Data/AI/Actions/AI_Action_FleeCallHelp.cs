using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Action_FleeCallHelp(RB)", menuName = "AI/Action/FleeCallHelp(RB)")]
public class AI_Action_FleeCallHelp : AI_Action
{
    public float distanceToTriggerSpawn = 2f;
    public float maxDistanceSpawnpoint = 50f;
    public float durationToSpawn = 2f;
    [Range(0f, 1f)]
    public float chanceToSpawn = 0.5f;

    public override void PreformAction(AI ai)
    {
        FleeCallForHelp(ai);
    }

    private void FleeCallForHelp(AI ai)
    {
        if(ai.SpawnPointForBackup == null)
        {
            return;
        }
        ai.Agent.SetDestination(ai.SpawnPointForBackup.transform.position);

        MoveAndCallFriends(ai);
    }

    private void MoveAndCallFriends(AI ai)
    {
        if (Vector3.Distance(ai.transform.position, ai.SpawnPointForBackup.transform.position) < distanceToTriggerSpawn)
        {
            if (!ai.SetATimer)
            {
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;
            }

            if (ai.SetATimer && !ai.CalledForFriends)
            {
                if (ai.ActionTimeCheck(durationToSpawn))
                {
                    if (Random.Range(0f, 1f) < chanceToSpawn)
                    {
                        ai.SpawnPointForBackup.SpawnEnemy();
                        Debug.Log("AI spawned a friend <3");
                    }
                    ai.FleeingFromPlayer = false;
                    ai.CalledForFriends = true;
                }
            }
        }

        Rotate(ai.SpawnPointForBackup.transform.position, ai);

        ai.IsMoving = true;
        ai.Animator.SetBool("Walking", true);
        Move(ai, ai.Stats.chaseSpeed);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Condition_CheckHPThreshold", menuName = "AI/Condition/CheckHPThreshold")]
public class AI_Condition_HPThresholdCheck : AI_Condition
{
    public float maxDistanceSpawnpoint = 50f;

    public override bool CheckCondition(AI ai)
    {
        return CheckHP(ai);
    }

    private bool CheckHP(AI ai)
    {
        if (ai.Destructible.Health <= ai.Stats.healthThresholdForReaction * ai.Destructible.MaxHealth)
        {
            if (ai.EnemyType == enemyType.big)
            {
                ai.Enrage = true;
                if (!ai.IsStunned)
                {
                    return true;
                }
            }

            if (ai.EnemyType == enemyType.small)
            {
                if (ai.CalledForFriends == true)
                {
                    return false;
                }
                else
                {
                    return FindSpawnPoint(ai);
                }
            }

        }
        return false;
    }



    private bool FindSpawnPoint(AI ai)
    {

        EnemySpawnPoint[] enemySpawnPoints = ai.EnemySpawnManager.EnemySpawnPoints;

        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            if (Vector3.Distance(ai.transform.position, enemySpawnPoints[i].transform.position) > maxDistanceSpawnpoint)
            {
                continue;
            }
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(ai.transform.position, enemySpawnPoints[i].transform.position, NavMesh.GetAreaFromName("walkable"), path))
            {
                if (GetPathLength(path) > maxDistanceSpawnpoint)
                {
                    continue;
                }
            }

            if (!ai.EnemySpawnManager.CanPlayerSeePoint(enemySpawnPoints[i].transform.position))
            {
                //Debug.Log("Found a spawnpoint");
                ai.SpawnPointForBackup = enemySpawnPoints[i];
                return true;
            }
        }

        //Debug.Log("Couldn't find a spawnpoint outside of vision");
        return false;
        
    }

    private float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Action_FindSearchPoints", menuName = "AI/Action/FindSearchPoints")]
public class AI_Action_FindSearchPoints : AI_Action
{
    public override void PreformAction(AI ai)
    {
        FindSearchPoints(ai);
    }

    private void FindSearchPoints(AI ai)
    {
        if (ai.FoundSearchPoints)
        {
            return;
        }

        NavMeshPath path = new NavMeshPath();

        ai.SearchPoints[0] = ai.SoundLastPosition;
        for (int i = 1; i < ai.Stats.nbrSearchPos; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * ai.Stats.searchRadius;
            randomDir += ai.SoundLastPosition;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDir, out hit, ai.Stats.searchRadius, NavMesh.AllAreas))
            {
               // Debug.Log("hitpos:" + hit.position + " transformPos: " + ai.transform.position);
                if (NavMesh.CalculatePath(ai.transform.position, hit.position, NavMesh.AllAreas, path))
                {
                    if(path.status != NavMeshPathStatus.PathPartial)
                    {
                        if (GetPathLength(path) < ai.Stats.searchRadius)
                        {
                            ai.SearchPoints[i] = hit.position;
                            ai.FoundSearchPoints = true;
                        }
                        else
                        {
                            i--;
                        }
                    }
                }
            }
        }
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

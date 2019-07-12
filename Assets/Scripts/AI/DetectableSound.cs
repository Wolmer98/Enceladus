using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class DetectableSound 
{
    public static void PlayDetectableSound(Vector3 position, float radius, LayerMask layerMask)
    {
        Collider[] colliders = new Collider[10];
        Physics.OverlapSphereNonAlloc(position, radius, colliders, layerMask);
        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                AI ai = colliders[i].gameObject.GetComponentInParent<AI>();
                if(ai != null && ai.Agent.isOnNavMesh)
                {
                    //Debug.Log("AI: " + ai.name);
                    if (ai.Agent.CalculatePath(position, path))
                    {
                        //Debug.Log("AI calc path");
                        if (path.status != NavMeshPathStatus.PathPartial)
                        {
                            //Debug.Log("Path exsists");
                            NavMesh.CalculatePath(ai.transform.position, position, NavMesh.GetAreaFromName("walkable"), path);
                            if (GetPathLength(path) < radius + radius / 2)
                            {
                                ai.DetectedSound(position, false);
                                //Debug.Log("AI was in range");
                            }
                        }
                        else
                        {
                            //Debug.Log("Path doesn't excists");
                        }
                    }
                }
            }
        }
    }

    public static void PlayDetectableSound(Vector3 position, float radius, LayerMask layerMask, bool knowExactLocation)
    {
        Collider[] colliders = new Collider[10];
        Physics.OverlapSphereNonAlloc(position, radius, colliders, layerMask);
        NavMeshPath path = new NavMeshPath();
      //  Debug.Log("KnowExactLocation");
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                AI ai = colliders[i].gameObject.GetComponentInParent<AI>();
                if (ai != null &&  ai.Agent.isOnNavMesh)
                {
                    if (ai.Agent.CalculatePath(position, path))
                    {
                        if (path.status != NavMeshPathStatus.PathPartial)
                        {
                            if (GetPathLength(path) < radius)
                            {
                                NavMesh.CalculatePath(ai.transform.position, position, NavMesh.GetAreaFromName("walkable"), path);
                                if (GetPathLength(path) < radius + radius / 2)
                                {
                                    ai.DetectedSound(position, knowExactLocation);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static float GetPathLength(NavMeshPath path)
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

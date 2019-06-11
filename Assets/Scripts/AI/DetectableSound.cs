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
                if(ai != null)
                {
                    if (NavMesh.CalculatePath(ai.transform.position, position, NavMesh.GetAreaFromName("walkable"), path))
                    {
                        if (GetPathLength(path) < radius)
                        {
                            ai.DetectedSound(position, false);
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
                if (ai != null)
                {
                    if (NavMesh.CalculatePath(ai.transform.position, position, NavMesh.GetAreaFromName("walkable"), path))
                    {
                        if (GetPathLength(path) < radius)
                        {
                            ai.DetectedSound(position, knowExactLocation);
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

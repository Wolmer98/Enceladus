using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AI_Condition : ScriptableObject
{
    public abstract bool CheckCondition(AI ai);

    protected bool IsPathInRange(NavMeshPath path, float range)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            if (lng < range)
            {
                return true;
            }
        }

        return false;

    }

}
